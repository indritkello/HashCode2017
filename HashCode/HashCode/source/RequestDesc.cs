using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode
{
    public class RequestDesc
    {
        public int EndPointId { get; set; }
        public int VideoId { get; set; }
        public int RequestNumber { get; set; }
        public decimal Probability { get; set; }
        public Endpoint EndPoint { get; set; }
    }
}
