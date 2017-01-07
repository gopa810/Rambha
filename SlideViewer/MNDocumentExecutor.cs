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
        public class GVGraphConnection
        {
            public string Title;
            public GVGraphObject Source;
            public GVGraphObject Target;
        }

        public class GVGraphConnControlFlow : GVGraphConnection { }
        public class GVGraphConnOwnership : GVGraphConnection { }

        public class GVGraph
        {
            public List<GVGraphConnection> Connections = null;
        }

        public class MNDocumentX
        {
            public GVGraph PageDynamics = null;
        }

        public void SetDocument(MNDocument doc)
        {
        }

        public class GVGraphObject
        {
            public string executeAction(GVGraphAction a)
            {
                return "";
            }
        }

        public class GVGraphAction : GVGraphObject { }
        public class GVObjectScript : GVGraphObject
        {
            public GSCore CompiledScript;
        }
        public class GVObjectControlEntity : GVGraphObject { public SMControl Control; }
        public class GVPageWithControls : GVGraphObject { public MNPage Page; }

        public GSExecutor Executor { get; set; }

        public GSCore ViewController { get; set; }

        public MNDocumentX Document { get; set; }

        public MNPage CurrentPage { get; set; }

        public MNDocumentExecutor(GSCore vc)
        {
            ViewController = vc;
            Document = null;
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
        private void ExecuteScriptForKey(GSCore obj, GVObjectScript os)
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
            Executor.ExecuteElement(os.CompiledScript);
        }

        private void ExecuteScriptForKey(MNPage page, GVObjectScript os)
        {
            Executor.ExecuteElement(os.CompiledScript);
        }


        // control events
        public void OnDragFinished(SMControl control, SMTokenItem token, PVDragContext context)
        {
            // find all connections of type "partOfGroup" where target is control
            // and notify parent group that this event has occured

            // find all activities in graph that are target conected to this control as source and have 
            // name of connection as this event
            // and execute them and then execute their successors
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDragFinished");
                ExecuteScheduledObjects(control, scheduled);
            }
        }
        public bool OnDropWillFinish(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDropWillFinish");
                ExecuteScheduledObjects(control, scheduled);
            }
            return true;
        }

        public void OnDropDidFinish(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDropDidFinish");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnLongClick(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnLongClick");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnDoubleClick(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDoubleClick");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnClick(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnClick");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnDropMove(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDropMove");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnTapBegin(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnTapBegin");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnTapCancel(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnTapCancel");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnTapMove(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnTapMove");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnTapEnd(SMControl control, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnTapEnd");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnDragStarted(SMControl control, SMTokenItem token, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDragStarted");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnDragMove(SMControl control, SMTokenItem token, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(control, scheduled, "OnDragMove");
                ExecuteScheduledObjects(control, scheduled);
            }
        }

        public void OnDragHotTrackStarted(SMTokenItem item, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(context.trackedControl, scheduled, "OnDragHotTrackStarted");
                ExecuteScheduledObjects(context.trackedControl, scheduled);
            }
        }

        public void OnDragHotTrackEnded(SMTokenItem item, PVDragContext context)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractControlConnections(context.trackedControl, scheduled, "OnDragHotTrackEnded");
                ExecuteScheduledObjects(context.trackedControl, scheduled);
            }
        }


        // page events
        public void OnPageWillAppear(MNPage page)
        {
            // to all procedures, that are connected as target with current page
            // and control flow named "OnPageWillAppear"
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractPageConnections(page, scheduled, "OnPageWillAppear");
                ExecuteScheduledObjects(page, scheduled);
            }
        }

        public void OnPageDidAppear(MNPage page)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractPageConnections(page, scheduled, "OnPageDidAppear");
                ExecuteScheduledObjects(page, scheduled);
            }
        }

        public void OnPageWillDisappear(MNPage page)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractPageConnections(page, scheduled, "OnPageWillDisappear");
                ExecuteScheduledObjects(page, scheduled);
            }
        }

        public void OnPageDidDisappear(MNPage page)
        {
            if (Document != null && Document.PageDynamics != null)
            {
                List<GVGraphObject> scheduled = new List<GVGraphObject>();
                ExtractPageConnections(page, scheduled, "OnPageDidDisappear");
                ExecuteScheduledObjects(page, scheduled);
            }
        }




        /// <summary>
        /// Executes objects in the list and possibly add new objects in succession
        /// </summary>
        /// <param name="control"></param>
        /// <param name="scheduled"></param>
        private void ExecuteScheduledObjects(GSCore control, List<GVGraphObject> scheduled)
        {
            string returnValue = null;

            for (int i = 0; i < scheduled.Count; i++)
            {
                GVGraphObject go = scheduled[i];
                if (go is GVGraphAction)
                {
                    returnValue = null;
                    // find source object of action
                    GVGraphObject source = FindOwnerOfObject(go);
                    if (source != null)
                    {
                        // execute action
                        returnValue = source.executeAction(go as GVGraphAction);
                    }

                    // after execution, include also successor objects for execution
                    if (returnValue != null)
                    {
                        foreach (GVGraphConnection gc2 in Document.PageDynamics.Connections)
                        {
                            if (gc2 is GVGraphConnControlFlow && gc2.Source == go && gc2.Title.Equals(returnValue))
                            {
                                scheduled.Add(gc2.Target);
                            }
                        }
                    }
                }
                else if (go is GVObjectScript)
                {
                    ExecuteScriptForKey(control, go as GVObjectScript);
                }
            }
        }

        private GVGraphObject FindOwnerOfObject(GVGraphObject go)
        {
            foreach (GVGraphConnection gc in Document.PageDynamics.Connections)
            {
                if (gc is GVGraphConnOwnership && gc.Target == go)
                {
                    return gc.Source;
                }
            }
            return null;
        }

        private void ExtractControlConnections(SMControl control, List<GVGraphObject> scheduled, string name)
        {
            foreach (GVGraphConnection gc in Document.PageDynamics.Connections)
            {
                if (gc is GVGraphConnControlFlow && gc.Source is GVObjectControlEntity
                    && (gc.Source as GVObjectControlEntity).Control == control
                    && gc.Title == name)
                {
                    scheduled.Add(gc.Target);
                }
            }
        }

        private void ExtractPageConnections(MNPage page, List<GVGraphObject> scheduled, string name)
        {
            foreach (GVGraphConnection gc in Document.PageDynamics.Connections)
            {
                if (gc is GVGraphConnControlFlow && gc.Source is GVPageWithControls
                    && (gc.Source as GVPageWithControls).Page == page
                    && gc.Title == name)
                {
                    scheduled.Add(gc.Target);
                }
            }
        }



    }
}
