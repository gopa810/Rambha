using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rambha.Script
{

    public class GSCoreCollection : List<GSCore>
    {
        public GSCoreCollection()
        {
        }

        public GSCoreCollection(IEnumerable<GSCore> list)
        {
            foreach (GSCore c in list)
            {
                Add(c);
            }
        }

        public bool IsFirstToken()
        {
            if (Count > 0 && this[0] is GSToken)
                return true;
            return false;
        }

        public GSCore getSafe(int index)
        {
            if (index < 0 || index >= Count)
                return GSVoid.Void;
            return this[index];
        }

        public string getFirstToken()
        {
            if (Count > 0)
            {
                if (this[0] is GSToken)
                    return (this[0] as GSToken).Token;
                else
                    return this[0].getStringValue();
            }
            return String.Empty;
        }

        public GSCoreDataType getArithmeticDataType()
        {
            int strings = 0;
            int numbers = 0;
            int doubles = 0;
            int bools = 0;

            foreach (GSCore item in this)
            {
                if (item is GSInt64 || item is GSInt32 || item is GSInt16)
                    numbers++;
                else if (item is GSDouble)
                    doubles++;
                else if (item is GSBoolean)
                    bools++;
                else if (item is GSString || item is GSCharacter)
                    strings++;
            }

            if (strings > 0)
                return GSCoreDataType.String;
            if (doubles > 0)
                return GSCoreDataType.Double;
            if (numbers > 0)
                return GSCoreDataType.Integer;
            if (bools > 0)
                return GSCoreDataType.Boolean;

            return GSCoreDataType.Void;
        }

        public GSCoreCollection getSublist(int fromIndex)
        {
            GSCoreCollection args = new GSCoreCollection();
            for (int n = fromIndex; n < Count; n++)
                args.Add(this[n]);
            return args;
        }

        public GSCoreCollection getEvaluatedSublist(GSExecutor x, int fromIndex)
        {
            GSCoreCollection args = new GSCoreCollection();
            for (int n = fromIndex; n < Count; n++)
                args.Add(x.ExecuteElement(this[n]));
            return args;
        }

    }

    public enum GSCoreDataType
    {
        String,
        Integer,
        Double,
        Boolean,
        Void
    }


}
