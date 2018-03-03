using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode
{
    public class CacheEndpointLatency
    {
        public int CacheId { get; set; }
        public int EndPointId { get; set; }
        public int Value { get; set; }
        public int Difference { get; set; }
    }
}
