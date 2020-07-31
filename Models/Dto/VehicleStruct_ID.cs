using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toyota.Models.Dto
{
    public class VehicleStruct_ID
    {
        public VehicleStruct_ID()
        {

        }

        public VehicleStruct_ID(string vehicle_id)
        {
            this.model_id = vehicle_id.Substring(vehicle_id.LastIndexOf("model_"), vehicle_id.Length - vehicle_id.LastIndexOf("model_"));

            vehicle_id = vehicle_id.Substring(0, vehicle_id.LastIndexOf("model_"));
            string[] ArrTmp = vehicle_id.Split("_");

            this.catalog = ArrTmp[0];
            this.catalog_code = ArrTmp[1];
            this.compl_code = ArrTmp[2];
            this.sysopt = ArrTmp[3];

            this.frame = ArrTmp[4];
            this.engine1 = ArrTmp[5];

            this.model_code = ArrTmp[6];
            this.grade = ArrTmp[7];
        }

        public string catalog { get; set; }
        public string catalog_code { get; set; }
        public string compl_code { get; set; }
        public string sysopt { get; set; }
        public string frame { get; set; }
        public string engine1 { get; set; }
        public string model_code { get; set; }
        public string grade { get; set; }
        public string model_id { get; set; }


        public static string GetVehicleId()
        {
            return " CONCAT(mc.catalog, '_', mc.catalog_code, '_', mc.compl_code, '_', mc.sysopt, '_', mc.frame, '_', mc.engine1,  '_', mc.model_code,  '_', mc.grade, '_', mc.model_id) ";
        }

        public override string ToString()
        {
            return $" {catalog} {catalog_code} {compl_code} {sysopt} {engine1} {model_id} ";
        }
    }
}
