using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;

namespace Rambha.Script
{
    /// <summary>
    /// Class for execution of entities. Execution means that we take element as command, and we are running this commmand
    /// and we get some results and side effects of the command
    /// 
    ///        GSExecutor es = new GSExecutor();
    ///        GSScript scr = new GSScript();
    ///        scr.readTextTemplate("[alg1:08d] [alg2:-18s] [alg3:5.3f]");
    ///        es.ExecuteElement(scr);
    ///        scr.Parts.Clear();
    ///
    /// </summary>
    public class GSExecutor: GSCore
    {
        private List<Dictionary<string, GSCore>> stackVars = new List<Dictionary<string, GSCore>>();

        private static Dictionary<string, GSCore> typesList = null;

        private StringBuilder output = new StringBuilder();
        private string markupStart = "[";
        private string markupEnd = "]";
        private string escapeChar = "\\";

        public GSExecutor()
        {
            stackVars.Add(new Dictionary<string, GSCore>());
            SetVariable("Int", new GSExecutorMathInteger() { Parent = this });
            SetVariable("Double", new GSExecutorDoubleMath() { Parent = this });
            SetVariable("Bool", new GSExecutorLog() { Parent = this });
            SetVariable("Bin", new GSExecutorBin() { Parent = this });
            SetVariable("String", new GSExecutorString() { Parent = this });
            SetVariable("System", this);
        }

        public override GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            GSCore result = null;
            if (token.Equals("print"))
                result = execPrint(args, false);
            else if (token.Equals("println"))
                result = execPrint(args, true);
            else if (token.Equals("if"))
                result = execIf(args);
            else if (token.Equals("while"))
                result = execWhile(args);
            else if (token.Equals("foreach"))
                result = execForeach(args);
            else if (token.Equals("x"))
                result = execMessage(args);
            else if (token.Equals("do"))
                result = execDo(args);
            else if (token.Equals("return"))
                result = new GSReturn(args.getSafe(0));
            else if (token.Equals("break"))
                result = new GSReturn(GSReturn.TYPE_BREAK);
            else if (token.Equals("continue"))
                result = new GSReturn(GSReturn.TYPE_CONTINUE);
            else if (token.Equals("set") && args.Count > 1)
                result = execSet(args[0], args[1]);
            else if (token.Equals("random"))
                result = execRandom(args);
            else
            {
                if (token.IndexOf('.') >= 0)
                {
                    string[] tp = token.Split('.');
                    GSCore obj = this;
                    for (int a = 0; a < tp.Length - 1; a++)
                    {
                        if (obj == null) break;
                        obj = obj.GetPropertyValue(tp[a]);
                    }
                    if (obj != null)
                    {
                        return obj.ExecuteMessage(tp[tp.Length - 1], args);
                    }
                }

                Debugger.Log(0, "", "UNKNOWN MESSAGE: " + token + " ");
            }

            return result;
        }

        /// <summary>
        /// Returns instance of given type, identified by name derived from implementation class (without prefix GS)
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GSCore GetTypeInstance(string type)
        {
            if (typesList == null)
            {
                Type coreType = typeof(GSCore);
                typesList = new Dictionary<string, GSCore>();
                foreach (Type t in coreType.Assembly.GetTypes())
                {
                    if (t.IsSubclassOf(coreType))
                    {
                        string name = t.Name;
                        if (name.IndexOf('.') >= 0)
                        {
                            name = name.Substring(name.LastIndexOf('.') + 1);
                            if (name.StartsWith("GS"))
                                name = name.Substring(2);
                        }
                        typesList.Add(name, (GSCore)Activator.CreateInstance(t));
                    }
                }
            }

            if (typesList.ContainsKey(type))
                return typesList[type];
            return null;
        }

        //
        // Methods that are helpful for execution of the program
        //
        #region Helper Methods

        public GSCoreCollection getNativeValues(GSCoreCollection args)
        {
            GSCoreCollection coll = new GSCoreCollection();
            foreach (GSCore item in args)
            {
                coll.Add(ExecuteElement(item));
            }
            return coll;
        }

        private Dictionary<string, GSCore> getLastVars()
        {
            return stackVars[stackVars.Count - 1];
        }

        public long[] getIntegerArray(GSCoreCollection C)
        {
            long[] result = new long[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = ExecuteElement(C[i]).getIntegerValue();
            }
            return result;
        }
        public double[] getDoubleArray(GSCoreCollection C)
        {
            double[] result = new double[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = ExecuteElement(C[i]).getDoubleValue();
            }
            return result;
        }
        public string[] getStringArray(GSCoreCollection C)
        {
            string[] result = new string[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = ExecuteElement(C[i]).getStringValue();
            }
            return result;
        }
        public bool[] getBooleanArray(GSCoreCollection C)
        {
            bool[] result = new bool[C.Count];
            for (int i = 0; i < C.Count; i++)
            {
                result[i] = ExecuteElement(C[i]).getBooleanValue();
            }
            return result;
        }

        public string ReplaceVariables(string str)
        {
            int lastPut = 0;
            int idx = 0;
            int ide = 0;
            StringBuilder sb = new StringBuilder();

            while ((idx = FindVariablePlaceholder(str, idx)) >= 0)
            {
                ide = FindVariablePlaceholderEnd(str, idx);
                if (ide >= idx)
                {
                    sb.Append(str.Substring(lastPut, idx - markupStart.Length - lastPut));
                    string ph = str.Substring(idx, ide - idx);
                    sb.Append(ReplaceVariablePlaceholder(ph));
                    lastPut = ide + markupEnd.Length;
                }
                else
                {
                    ide = idx;
                    break;
                }
                idx = ide;
            }

            sb.Append(str.Substring(lastPut));

            // replacing escape sequences
            sb.Replace(escapeChar + markupStart, markupStart);

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">Source string</param>
        /// <param name="from">Index in the string where first occurence of markupStart CAN be</param>
        /// <returns>Index of text after markup substring or -1 if not found</returns>
        public int FindVariablePlaceholder(string s, int from)
        {
            int i = from;
            int n;

            while (i >= 0 && i < s.Length)
            {
                n = s.IndexOf(markupStart, i);
                if (n < 0)
                {
                    return -1;
                }
                else if (n >= escapeChar.Length)
                {
                    if (s.Substring(n - escapeChar.Length, escapeChar.Length).Equals(escapeChar))
                        i = n + markupStart.Length;
                    else
                        return n + markupStart.Length;
                }
                else
                {
                    return n + markupStart.Length;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s">source string</param>
        /// <param name="from">index in string where next occurence of markupEnd can be</param>
        /// <returns>Index of next markupEnd substring</returns>
        public int FindVariablePlaceholderEnd(string s, int from)
        {
            int i = s.IndexOf(markupEnd, from);
            if (i > 0)
                return i;
            else
                return -1;
        }

        /// <summary>
        /// Formating string:
        /// 20s   - string with padding to 20 chars, left align
        /// -20s  - string with padding to 20 chars, right align
        /// 2d    - integer number padded to 2 chars with spaces, right align
        /// 02d   - integer number padded to 2 chars with zero, right align
        /// 1.7f  - floating point value with at least 1 digit before point and 7 digits after point
        /// </summary>
        /// <param name="ph">Input placeholder (without markup substrings,
        /// this can contain also some formatting after : character</param>
        /// <returns></returns>
        public string ReplaceVariablePlaceholder(string ph)
        {
            string fmt = "";
            int phi = ph.IndexOf(':');
            if (phi >= 0)
            {
                fmt = ph.Substring(phi + 1);
                ph = ph.Substring(0, phi);
            }

            GSCore cs = EvaluateProperty(ph);
            if (fmt.EndsWith("s"))
            {
                string value = cs.getStringValue();
                int places;
                if (int.TryParse(fmt.Substring(0,fmt.Length - 1), out places))
                {
                    if (places > 0)
                        value = value.PadRight(places);
                    else
                        value = value.PadLeft(-places);
                }
                return value;
            }
            else if (fmt.EndsWith("m"))
            {
                string value = cs.getStringValue();
                int places;
                if (int.TryParse(fmt.Substring(0, fmt.Length - 1), out places))
                {
                    if (value.Length > places)
                    {
                        value = value.Substring(0, places - 3) + "...";
                    }
                    else
                    {
                        if (places > 0)
                            value = value.PadRight(places);
                        else
                            value = value.PadLeft(-places);
                    }
                }
                return value;
            }
            else if (fmt.EndsWith("d"))
            {
                bool padWithZero = false;
                int places;
                string result = "";
                long ival = cs.getIntegerValue();
                if (int.TryParse(fmt.Substring(0, fmt.Length - 1), out places))
                {
                    if (fmt.StartsWith("0"))
                        padWithZero = true;
                    if (padWithZero)
                    {
                        result = string.Format("{0:0".PadRight(places - 1, '0') + "}", ival);
                        result = result.PadLeft(places, '0');
                    }
                    else
                    {
                        result = string.Format("{0:#".PadRight(places - 1, '#') + "}", ival);
                        result = result.PadLeft(places, ' ');
                    }

                }
                else
                {
                    result = ival.ToString();
                }
                return result;
            }
            else if (fmt.EndsWith("f"))
            {
                string a, b;
                fmt = fmt.Substring(0, fmt.Length - 1);
                int i = fmt.IndexOf('.');
                if (i >= 0)
                {
                    a = fmt.Substring(0, i);
                    b = fmt.Substring(i + 1);
                }
                else
                {
                    a = fmt;
                    b = "0";
                }
                int ia, ib;
                double d = cs.getDoubleValue();
                string result;
                if (int.TryParse(a, out ia) && int.TryParse(b, out ib))
                {
                    result = string.Format("{0:" + string.Format("F{0}", ib) + "}", d);
                    result = result.PadLeft(ia + ib + 1);
                }
                else
                {
                    result = d.ToString();
                }
                return result;
            }
            else
            {
                return cs.getStringValue();
            }
        }

        #endregion

        private GSCore execDo(GSCoreCollection args)
        {
            GSCore last = null;
            foreach (GSCore item in args)
            {
                last = ExecuteElement(item);
                if (last is GSReturn)
                {
                    return last;
                }

            }
            return last;
        }

        private GSCore execMessage(GSCoreCollection args)
        {
            GSCore result = GSVoid.Void;

            // first is token, name of variable, object
            // second is token, message name
            // third etc are arguments
            if (args.Count >= 2 && args.getSafe(0) is GSToken && args.getSafe(1) is GSToken)
            {
                // evaluate the remaining portion of list
                GSCoreCollection subArgs = getNativeValues(args.getSublist(2));
                // first and second tokens
                GSToken t1 = args.getSafe(0) as GSToken;
                GSToken t2 = args.getSafe(1) as GSToken;
                // evaluate reference to object
                GSCore obj = ExecuteElement(t1);
                // execute message in the object
                result = obj.ExecuteMessage(t2.Token, subArgs);
            }

            return result;
        }

        private GSCore execForeach(GSCoreCollection args)
        {
            if (args.Count < 4)
            {
                Debugger.Log(0, "", "Insufficient arguments for (FOREACH varName : list commands ) ");
                return null;
            }
            GSCore t1 = args.getSafe(0);
            GSCore l1 = ExecuteElement(args.getSafe(2));
            if (!(t1 is GSToken))
            {
                Debugger.Log(0, "", "Token shoudl be second argument in FOREACH ");
                return null;
            }
            if (!(l1 is GSList))
            {
                Debugger.Log(0, "", "List should be fourth argument in FOREACH ");
                return null;
            }
            GSToken tk = (GSToken)t1;
            GSList lst = (GSList)l1;
            GSCore r = null;
            int ik = 0;

            foreach (GSCore item in lst.Parts)
            {
                SetVariable(tk.Token, item);
                for (int i = 3; i < args.Count; i++)
                {
                    r = ExecuteElement(args.getSafe(i));
                    if (r is GSReturn)
                    {
                        break;
                    }
                }
                ik++;

                if (r is GSReturn)
                {
                    GSReturn ret = r as GSReturn;
                    if (ret.Type == GSReturn.TYPE_BREAK)
                        break;
                    if (ret.Type == GSReturn.TYPE_RETURN)
                        return ret;
                }
            }

            return new GSInt64(ik);
        }

        private GSCore execWhile(GSCoreCollection args)
        {
            GSCoreCollection commands = args.getSublist(1);
            GSCore r = null;

            while (ExecuteElement(args.getSafe(0)).getBooleanValue())
            {
                foreach (GSCore cmd in commands)
                {
                    r = ExecuteElement(cmd);
                    if (r is GSReturn)
                    {
                        break;
                    }
                }

                if (r is GSReturn)
                {
                    GSReturn ret = r as GSReturn;
                    if (ret.Type == GSReturn.TYPE_BREAK)
                        break;
                    if (ret.Type == GSReturn.TYPE_RETURN)
                        return ret;
                }
            }

            return GSVoid.Void;
        }

        private GSCore execIf(GSCoreCollection args)
        {
            GSCore cond = ExecuteElement(args.getSafe(0));
            GSCore cmd1 = args.getSafe(1);
            GSCore cmd2 = args.getSafe(2);
            GSCore r = null;

            if (cond.getBooleanValue())
            {
                bool running = false;
                foreach (GSCore cmd in args)
                {
                    if (cmd is GSToken && cmd.ToString().Equals("then"))
                        running = true;
                    if (cmd is GSToken && cmd.ToString().Equals("else"))
                        running = false;
                    if (running)
                    {
                        r = ExecuteElement(cmd);
                        if (r is GSReturn)
                            return r;
                    }
                }
            }
            else
            {
                bool running = false;
                foreach (GSCore cmd in args)
                {
                    if (cmd is GSToken && cmd.ToString().Equals("else"))
                        running = true;
                    if (running)
                    {
                        r = ExecuteElement(cmd);
                        if (r is GSReturn)
                            return r;
                    }
                }
            }

            return cond;
        }

        private GSCore execRandom(GSCoreCollection args)
        {
            int count = args.Count;
            Random rnd = new Random(DateTime.Now.Millisecond);
            return args.getSafe(rnd.Next(count));
        }

        private GSCore execPrint(GSCoreCollection arg, bool newLine)
        {
            foreach (GSCore argument in arg)
            {
                GSCore val = ExecuteElement(argument);
                string str = val.getStringValue();
                str = ReplaceVariables(str);
                output.Append(str);
            }

            if (newLine)
                output.AppendLine();

            return GSVoid.Void;
        }


        private GSCore execSet(GSCore keyElem, GSCore valueElem)
        {
            string key;
            if (keyElem is GSToken)
                key = (keyElem as GSToken).Token;
            else
                key = keyElem.getStringValue();
            GSCore value = ExecuteElement(valueElem);
            SetVariable(key, value);
            SetSystemVariables(key, value);
            return value;
        }

        private void SetSystemVariables(string key, GSCore value)
        {
            if (key.Equals("escapeChar"))
                escapeChar = value.getStringValue();
            else if (key.Equals("markupStart"))
                markupStart = value.getStringValue();
            else if (key.Equals("markupEnd"))
                markupEnd = value.getStringValue();
        }



        // this function is also in GSCore object
        // evaluates property into value
        public override GSCore GetPropertyValue(string Token)
        {
            // find in variables
            GSCore obj = GetVariable(Token);
            if (obj != null)
                return obj;

            // find types
            obj = GetTypeInstance(Token);
            if (obj != null)
                return obj;

            // find built-in property
            if (Token.Equals("name"))
                return new GSString() { Value = "Executor" };

            // return default empty string
            return new GSString();
        }


        /// <summary>
        /// Looks for object with given name in the stack of variables
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GSCore GetVariable(string name)
        {
            for (int i = stackVars.Count - 1; i >= 0; i--)
            {
                if (stackVars[i].ContainsKey(name))
                {
                    return stackVars[i][name];
                }
            }

            return null;
        }

        public void ClearVariables()
        {
            for (int i = stackVars.Count - 1; i >= 0; i--)
            {
                stackVars[i].Clear();
            }
        }

        /// <summary>
        /// Sets value for variable.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        public void SetVariable(string varName, GSCore varValue)
        {
            Dictionary<string, GSCore> vars = getLastVars();
            if (vars.ContainsKey(varName))
            {
                if (varValue == null)
                    vars.Remove(varName);
                else
                    vars[varName] = varValue;
            }
            else
            {
                if (varValue != null)
                    vars.Add(varName, varValue);
            }
        }

        /// <summary>
        /// Returning back to previous variable context
        /// </summary>
        public void PopStack()
        {
            if (stackVars.Count > 1)
                stackVars.RemoveAt(stackVars.Count - 1);
        }

        /// <summary>
        /// Creating new variable context
        /// </summary>
        public void PushStack()
        {
            stackVars.Add(new Dictionary<string, GSCore>());
        }

        /// <summary>
        /// Returns output text as a result from script execution.
        /// </summary>
        /// <returns></returns>
        public string getOutput()
        {
            return output.ToString();
        }

        /// <summary>
        /// Executing element. For most of the elements in the program it is element itself,
        /// but for the list it is result of executing operation that is mentioned in the head
        /// of the list.
        /// </summary>
        /// <param name="E"></param>
        /// <returns></returns>
        public GSCore ExecuteElement(GSCore E)
        {
            if (E is GSList)
            {
                GSList L = (GSList)E;
                if (L.Count == 0)
                {
                    return GSVoid.Void;
                }
                else if (L.Count == 1)
                {
                    return ExecuteElement(L[0]);
                }
                else
                {
                    GSCore result = null;
                    GSCore target = ExecuteElement(L.Parts[0]);
                    string message = (L.Parts[1] is GSToken ? (L.Parts[1] as GSToken).Token : L.Parts[1].getStringValue());
                    if (target != null)
                    {
                        result = target.ExecuteMessage(message, L.Parts.getEvaluatedSublist(this, 2));
                    }

                    if (result == null)
                        return new GSString();
                    return result;
                }
            }
            else if (E is GSToken)
            {
                GSToken t = E as GSToken;
                if (t.Token.StartsWith("#"))
                    return new GSString(t.Token);
                return EvaluateProperty(((GSToken)E).Token);
            }
            else
            {
                return E;
            }
        }

        public void resetOutput()
        {
            output.Clear();
        }


    }
}
