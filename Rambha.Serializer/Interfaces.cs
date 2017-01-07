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

    public interface IRSObjectOrigin
    {
        void setReference(int tag, object obj);
    }

    public interface IRSObjectResolver
    {
        object IRSResolver_FindObject(string objType, long objId);
    }

    public interface IRSUniqueIdProvider
    {
        long GetNextId();
    }
}
