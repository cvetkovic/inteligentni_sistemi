using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace etf.dotsandboxes.cl160127d
{
    public class VTuple<T, U> : System.Tuple<T, U>
    {
        public VTuple(T t, U u) : base(t, u) { }

        public override bool Equals(object obj)
        {
            if (this is Tuple<int, int> && obj is Tuple<int, int>)
                return (Convert.ToInt32(Item1) == Convert.ToInt32(((Tuple<T, U>)obj).Item1)) &&
                       (Convert.ToInt32(Item2) == Convert.ToInt32(((Tuple<T, U>)obj).Item2));
            else
                return this.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator==(VTuple<T,U> t1, VTuple<T, U> t2)
        {
            return t1.Equals(t2);
        }

        public static bool operator !=(VTuple<T, U> t1, VTuple<T, U> t2)
        {
            return !t1.Equals(t2);
        }
    }
}