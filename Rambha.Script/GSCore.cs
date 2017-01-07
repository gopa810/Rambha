using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Rambha.Script
{
    /// <summary>
    /// This class forms basic for all objects in GCAL Script Engine
    /// As per object definition in OOP, object has properties
    /// and methods (to manipulate with properties).
    /// Properties are accessed in read-only mode via functions GetPropertyValue
    /// and EvaluateProperty
    /// Methods are executed via function ExecuteMessage
    /// </summary>
    public class GSCore
    {
        /// <summary>
        /// This is executing instance message with arguments.
        /// Static methods (messages) are also defined here without the using the instance's data.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual GSCore ExecuteMessage(string token, GSCoreCollection args)
        {
            return GSVoid.Void;
        }

        /// <summary>
        /// This should return value for single token name
        /// Token name does not contain dot separator
        /// Static properties are also defined here without the using the instance's data.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual GSCore GetPropertyValue(string s)
        {
            switch (s)
            {
                case "integer":
                    return new GSInt64(getIntegerValue());
                case "string":
                    return new GSString(getStringValue());
                case "double":
                    return new GSDouble(getDoubleValue());
                case "boolean":
                    return new GSBoolean(getBooleanValue());
                default:
                    return GSVoid.Void;
            }
        }
        
        public virtual GSCore[] GetChildren()
        {
            return null;
        }

        public GSCore ExecuteMessage(string token, GSCore obj, GSCore obj2)
        {
            GSCoreCollection args = new GSCoreCollection();
            args.Add(obj);
            args.Add(obj2);
            return ExecuteMessage(token, args);
        }

        /// <summary>
        /// Short-cut method for 1 argument
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public GSCore ExecuteMessage(string token, GSCore obj)
        {
            GSCoreCollection args = new GSCoreCollection();
            args.Add(obj);
            return ExecuteMessage(token, args);
        }

        public GSCore ExecuteMessage(string token)
        {
            GSCoreCollection args = new GSCoreCollection();
            return ExecuteMessage(token, args);
        }


        /// <summary>
        /// Token can contain dot separator
        /// We should not normally override this method
        /// Rather method GetTokenValue shoudl be overriden
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public GSCore EvaluateProperty(string Token)
        {
            int dotPos = Token.IndexOf('.');
            if (dotPos >= 0)
            {
                string str = Token.Substring(0, dotPos);
                GSCore obj = GetPropertyValue(str);
                if (obj == null)
                    return GSVoid.Void;
                return obj.EvaluateProperty(Token.Substring(dotPos + 1));
            }
            else
            {
                return GetPropertyValue(Token);
            }
        }


        //
        // methods for overriding to get value in specified data type
        //
        #region Methods - Should be overriden for correct results

        /// <summary>
        /// This should be native value enforced to STRING datatype
        /// </summary>
        /// <returns></returns>
        public virtual string getStringValue()
        {
            return String.Empty;
        }

        /// <summary>
        /// This should be native value enforced to INT datatype
        /// </summary>
        /// <returns></returns>
        public virtual long getIntegerValue()
        {
            return 0L;
        }

        /// <summary>
        /// This should be native value enforced to DOUBLE datatype
        /// </summary>
        /// <returns></returns>
        public virtual double getDoubleValue()
        {
            return 0.0;
        }

        /// <summary>
        /// This should be native value enforced to BOOL datatype
        /// </summary>
        /// <returns></returns>
        public virtual bool getBooleanValue()
        {
            return false;
        }



        #endregion



        public virtual void writeScript(int level, StreamWriter sw)
        {
        }
    }
}
