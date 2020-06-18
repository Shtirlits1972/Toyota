using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class PartsGroup
    {
        public string group_id { get; set; }
        public string name { get; set; }
        public List<Sgroups> childs { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
