using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class CarTypeInfo
    {

        public string vehicle_id { get; set; }
        public string model_name { get; set; }
        public string brand { get; set; }
        public string catalog { get; set; }
        public string model_code { get; set; }
        public string add_codes { get; set; }
        public string engine { get; set; }
        public string prod_start { get; set; }
        public string grade { get; set; }
        public string atm_mtm { get; set; }
        public string trans { get; set; }
        public string f1 { get; set; }

        public override string ToString()
        {
            return $" {brand} {model_name} ";
        }
    }
}
