using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Rambha.GraphView
{
    public interface IGraphDelegate
    {
        bool WillInsertConnection(GVGraphViewContext context, GVGraphConnection conn);
        void DidInsertConnection(GVGraphConnection conn);
        void OnObjectDoubleClick(GVGraphViewContext context, GVGraphObject obj);
        void OnObjectSelected(GVGraphObject obj);
        void OnObjectTextEdited(GeneralDictionary data);
        DragDropEffects DragEnterEventCallback(GVGraphView view, DragEventArgs e);
        void DragDropEventCallback(GVGraphView view, DragEventArgs e);

        // returns true if processing of this event should continue
        bool OnTouchWillEndTracker(GVGraphViewContext context, GVTrackerBase tracker);
        void OnTouchDidEndTracker(GVGraphViewContext context, GVTrackerBase tracker);
    }

}
