using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flowent
{
    public struct Input<T>
    {
        public T Value { get; private set; }

        public static implicit operator T(Input<T> val) => val.Value = val;
    }
}
