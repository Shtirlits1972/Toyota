using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class model
    {
        public string catalog_brand_id { get; set; }
        public string catalog_brand_name { get; set; }
        public string catalog_model_id { get; set; }
        public string catalog_model_name { get; set; }
        public override string ToString()
        {
            return catalog_model_name;
        }
    }
}
