using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.DataAccessModule.Interface
{
    internal abstract class IConverter<T1,T2>
    {
        private protected abstract T2 VOTransformToDO(T1 VO);
        private protected abstract T1 DOTransformToVO(T2 DO);
    }
}
