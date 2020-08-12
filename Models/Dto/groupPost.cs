using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class groupPost
    {
        public string vehicle_id { get; set; }
        public string group_id { get; set; }
        public override string ToString()
        {
            return vehicle_id;
        }
    }
}
