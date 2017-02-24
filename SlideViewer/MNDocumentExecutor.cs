using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public MNPage CurrentPage { get; set; }

        private List<string> scheduledScripts = new List<string>();

        public MNDocumentExecutor(GSCore vc)
        {
            ViewController = vc;
            CurrentPage = null;
            Executor = new GSExecutor();
            Executor.SetVariable("executor", Executor);
            Executor.SetVariable("view", ViewController);
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
                Executor.SetVariable("page", ((SMControl)obj).Page);
            }
            else if (obj is MNPage)
            {
                Executor.SetVariable("control", null);
                Executor.SetVariable("page", (MNPage)obj);
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
                ExtractControlConnections(control, scheduledScripts, "OnClick");
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
                if (eventName.Equals("OnPlaySound") && parent is MNReferencedSound)
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
    }
}
