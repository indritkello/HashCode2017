using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode
{
    public class Endpoint
    {
        public int Id { get; set; }
        public List<CacheEndpointLatency> CacheLatencies { get; set; }
        public int DataCenterLatency { get; set; }
        public int TotalRequests { get; set; }
    }
}
