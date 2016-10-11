using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlideMaker.Document
{
    public interface IDocumentDelegate
    {
        void documentHasChanged(MNDocument doc);
    }
}
