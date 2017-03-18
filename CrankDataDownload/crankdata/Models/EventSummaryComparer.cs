using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class EventSummaryComparer<T> : IEqualityComparer<T> where T : EventSummary
    {
        public bool Equals(T x, T y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}

