using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class CarTypeInfo
    {

        public string id  { get; set; }
        public string model_name { get; set; }
        public string model_code { get; set; }
        public string engine1 { get; set; }
        public string body { get; set; }
        public string grade { get; set; }
        public string trans { get; set; }
        public string frame { get; set; }
        public string sysopt { get; set; }
        public string f1 { get; set; }
        public string f2 { get; set; }
        public string f3 { get; set; }

        public override string ToString()
        {
            return $" {model_name} {model_code}  ";
        }
    }
}
