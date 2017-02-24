using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Serializer
{
    public interface IRSSerializable
    {
        void Save(RSFileWriter W);
        void Load(RSFileReader R);
    }

    public interface IRSUniqueIdProvider
    {
        long GetNextId();
    }
}
