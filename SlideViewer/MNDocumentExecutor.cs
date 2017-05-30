using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Rambha.Script;
using Rambha.Document;

namespace SlideViewer
{
    public class MNDocumentExecutor: IDocumentViewDelegate
    {
        public GSExecutor Executor { get; set; }

        public GSCore ViewController { get; set; }

        private MNDocument p_doc = null;

        public MNDocument Document 
        {
            get
            {
                return p_doc;
            }
            set
            {
                p_doc = value;
                if (value != null && Executor != null)
                    Executor.SetVariable("document", p_doc);
            }
        }

        private MNPage _current_page = null;

        public MNPage CurrentPage 
        {
            get { return _current_page; }
            set { _current_page = value; SetExecVars(value); }
        }

        private List<string> scheduledScripts = new List<string>();

        public MNDocumentExecutor(GSCore vc)
        {
            ViewController = vc;
            CurrentPage = null;
            Executor = new GSExecutor();
        }

        public void SetExecVars(MNPage page)
        {
            if (page != null && Executor != null)
            {
                Executor.ClearVariables();
                foreach (SMControl ctrl in page.Objects)
                {
                    if (ctrl.UniqueName.Length > 0)
                        Executor.SetVariable(ctrl.UniqueName, ctrl);
                }
                Executor.SetVariable("executor", Executor);
                Executor.SetVariable("view", ViewController);
                Executor.SetVariable("document", page.Document);
                Executor.SetVariable("page", page);
            }
        }

        public GSCore GetPropertyValue(string token)
        {
            if (token.Equals("page"))
                return CurrentPage;
            else if (token.Equals("document"))
                return Document;
            else if (token.Equals("view"))
                return ViewController;
            else
                return Executor;
        }

        public GSCore ResolveProperty(string Token)
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

        /// <summary>
        /// Executing of selected script
        /// Here we respect levels of processing:
        /// - control
        /// - control group
        /// - page
        /// - application (document, book)
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="area"></param>
        /// <param name="scriptKey"></param>
        private void ExecuteScriptForKey(GSCore obj, GSScript os)
        {
            if (obj is SMControl)
            {
                Executor.SetVariable("control", (SMControl)obj);
            }
            else if (obj is MNPage)
            {
                Executor.SetVariable("control", null);
            }
            Executor.ExecuteElement(os);
        }

        // control events
        public void OnDragFinished(SMControl control, SMTokenItem token, PVDragContext context)
        {
            // find all connections of type "partOfGroup" where target is control
            // and notify parent group that this event has occured

            // find all activities in graph that are target conected to this control as source and have 
            // name of connection as this event
            // and execute them and then execute their successors
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDragFinished");
                ExecuteScheduledObjects(control);
            }
        }
        public bool OnDropWillFinish(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDropWillFinish");
                ExecuteScheduledObjects(control);
            }
            return true;
        }

        public void OnDropDidFinish(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDropDidFinish");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnLongClick(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnLongClick");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnDoubleClick(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDoubleClick");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnClick(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                if (control.ScriptOnClick.Length > 0)
                    scheduledScripts.Add(control.ScriptOnClick);
                ExecuteScheduledObjects(control);
            }
        }

        public void OnDropMove(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDropMove");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnTapBegin(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnTapBegin");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnTapCancel(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnTapCancel");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnTapMove(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnTapMove");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnTapEnd(SMControl control, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnTapEnd");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnDragStarted(SMControl control, SMTokenItem token, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDragStarted");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnDragMove(SMControl control, SMTokenItem token, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(control, scheduledScripts, "OnDragMove");
                ExecuteScheduledObjects(control);
            }
        }

        public void OnDragHotTrackStarted(SMTokenItem item, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(context.trackedControl, scheduledScripts, "OnDragHotTrackStarted");
                ExecuteScheduledObjects(context.trackedControl);
            }
        }

        public void OnDragHotTrackEnded(SMTokenItem item, PVDragContext context)
        {
            if (Document != null)
            {
                scheduledScripts.Clear();
                ExtractControlConnections(context.trackedControl, scheduledScripts, "OnDragHotTrackEnded");
                ExecuteScheduledObjects(context.trackedControl);
            }
        }

        public void OnEvent(string eventName, GSCore parent)
        {
            if (Document != null)
            {
                if (eventName.Equals("invalidate"))
                {
                    if (ViewController != null)
                        ViewController.ExecuteMessage("imvalidate");
                }
                else if (eventName.Equals("OnPlaySound") && parent is MNReferencedSound)
                {
                    if (ViewController != null)
                        ViewController.ExecuteMessage("playSound", parent);
                }
                else
                {
                    scheduledScripts.Clear();
                    ExtractObjectConnections(parent, scheduledScripts, eventName);
                    ExecuteScheduledObjects(parent);
                }
            }
        }


        /// <summary>
        /// Executes objects in the list and possibly add new objects in succession
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="scheduled"></param>
        private void ExecuteScheduledObjects(GSCore parent)
        {
            if (scheduledScripts != null)
            {
                for (int i = 0; i < scheduledScripts.Count; i++)
                {
                    GSScript go = new GSScript();
                    go.readList(scheduledScripts[i]);
                    ExecuteScriptForKey(parent, go);
                }
            }

            scheduledScripts.Clear();
        }

        private void ExtractControlConnections(SMControl control, List<string> scheduled, string name)
        {
            if (control != null)
            {
                MNReferencedText rt = control.FindScript(name);
                if (rt != null)
                    scheduled.Add(rt.Text);
            }
        }

        private void ExtractObjectConnections(GSCore obj, List<string> scheduled, string name)
        {
            if (obj is MNDocument)
            {
                MNDocument doc = obj as MNDocument;
                foreach (MNReferencedText rs in doc.Data.Scripts)
                {
                    if (rs.Name.Equals(name))
                        scheduled.Add(rs.Text);
                }
                foreach (MNPage page in doc.Data.Pages)
                    ExtractObjectConnections(page, scheduled, name);
            }
            else if (obj is MNPage)
            {
                MNPage page = obj as MNPage;
                foreach (MNReferencedText rt in page.Scripts)
                {
                    if (rt.Name.Equals(name))
                        scheduled.Add(rt.Text);
                }
                foreach (SMControl smc in page.Objects)
                {
                    if (smc.ContainsScript(name))
                        ExtractObjectConnections(smc, scheduled, name);
                }

            }
            else if (obj is SMControl)
            {
                MNReferencedText rt = (obj as SMControl).FindScript(name);
                if (rt != null)
                    scheduled.Add(rt.Text);
            }
        }


        public void OnMenuItem(MNMenuItem mi, MNPage page)
        {
            Executor.SetVariable("page", page);
            GSScript os = new GSScript();
            os.readList(mi.ActionScript);
            Executor.ExecuteElement(os);
        }

        public void AddNextScript(string scriptText)
        {
            scheduledScripts.Add(scriptText);
        }

        public Image GetBuiltInImage(string BuiltInImage)
        {
            if (BuiltInImage.Equals("navigIconHome"))
                return Properties.Resources.navigIconHome;
            if (BuiltInImage.Equals("navigIconBack"))
                return Properties.Resources.navigIconBack;
            if (BuiltInImage.Equals("navigIconFwd"))
                return Properties.Resources.navigIconFwd;
            if (BuiltInImage.Equals("navigIconHelp"))
                return Properties.Resources.navigIconHelp;
            if (BuiltInImage.Equals("SpeakerOn"))
                return Properties.Resources.SpeakerOn;
            if (BuiltInImage.Equals("menuIconSelLang"))
                return Properties.Resources.menuIconSelLang;
            if (BuiltInImage.Equals("menuIconSelBook"))
                return Properties.Resources.menuIconSelBook;
            if (BuiltInImage.Equals("menuIconBack"))
                return Properties.Resources.menuIconBack;
            if (BuiltInImage.Equals("menuIconBook"))
                return Properties.Resources.menuIconBook;
            if (BuiltInImage.Equals("menuIconExercA"))
                return Properties.Resources.menuIconExercA;
            if (BuiltInImage.Equals("menuIconExercB"))
                return Properties.Resources.menuIconExercB;
            if (BuiltInImage.Equals("menuIconFwd"))
                return Properties.Resources.menuIconFwd;
            if (BuiltInImage.Equals("menuIconHome"))
                return Properties.Resources.menuIconHome;
            if (BuiltInImage.Equals("menuIconShowHints"))
                return Properties.Resources.menuIconShowHints;
            if (BuiltInImage.Equals("menuIconMenu"))
                return Properties.Resources.menuIconMenu;
            if (BuiltInImage.Equals("menuIconParents"))
                return Properties.Resources.menuIconParents;
            if (BuiltInImage.Equals("menuItemRestart"))
                return Properties.Resources.menuItemRestart;
            if (BuiltInImage.Equals("menuItemShowSpots"))
                return Properties.Resources.menuItemShowSpots;

            return null;
        }


        public void ScheduleCall(int delayMilli, GSCore target, params object[] args)
        {
            GSCoreCollection cs = new GSCoreCollection();
            cs.Add(new GSInt32(delayMilli));
            cs.Add(target);
            foreach (object a in args)
            {
                if (a is string)
                {
                    cs.Add(new GSString(a as string));
                }
                else if (a is int)
                {
                    cs.Add(new GSInt32((int)a));
                }
                else if (a is GSCore)
                {
                    cs.Add(a as GSCore);
                }
            }

            ViewController.ExecuteMessage("scheduleCall", cs);
        }
    }
}
