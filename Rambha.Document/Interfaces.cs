using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Document
{
    public interface IDocumentViewDelegate
    {
        // control events
        void OnDragStarted(SMControl control, SMTokenItem token, PVDragContext context);
        void OnDragMove(SMControl control, SMTokenItem token, PVDragContext context);
        void OnDragFinished(SMControl control, SMTokenItem token, PVDragContext context);
        bool OnDropWillFinish(SMControl control, PVDragContext context);
        void OnDropDidFinish(SMControl control, PVDragContext context);
        void OnClick(SMControl control, PVDragContext context);
        void OnLongClick(SMControl control, PVDragContext context);
        void OnDoubleClick(SMControl control, PVDragContext context);
        void OnDropMove(SMControl control, PVDragContext context);
        void OnTapBegin(SMControl control, PVDragContext context);
        void OnTapCancel(SMControl control, PVDragContext context);
        void OnTapMove(SMControl control, PVDragContext context);
        void OnTapEnd(SMControl control, PVDragContext context);
        void OnDragHotTrackStarted(SMTokenItem item, PVDragContext context);
        void OnDragHotTrackEnded(SMTokenItem item, PVDragContext context);

        // page events
        void OnPageWillAppear(MNPage page);
        void OnPageDidAppear(MNPage page);
        void OnPageWillDisappear(MNPage page);
        void OnPageDidDisappear(MNPage page);

    }
}
