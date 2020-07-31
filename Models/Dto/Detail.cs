using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class Detail
    {
        public string name { get; set; }
        public string number { get; set; }
        public string pnc_label2 { get; set; } // соединяет pnc детали и label2 images
        public string attr { get; set; }
        public List<hotspots> hotspots { get; set; }
        public List<attributes> attributes { get; set; }
        public List<attributes> links { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
