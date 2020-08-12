using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class codePost
    {
        public string vehicle_id { get; set; }
        public string [] codes { get; set; }
        public override string ToString()
        {
            return vehicle_id;
        }
    }
}
