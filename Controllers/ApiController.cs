using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toyota.Models.Dto;
using Toyota.Models;
using System.IO;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Reflection.Metadata;

namespace Toyota.Controllers
{
    public class ApiController : Controller
    {
        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin, int page = 1, [FromQuery(Name = "per-page")] int qty = 10)
        {
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin); 
            List<header> headerList = ClassCrud.GetHeaders(8);

            List<CarTypeInfo> items = list.Skip((page - 1) * qty).Take(qty).ToList();

            var result = new
            {
                header = headerList,
                items,
                cnt_items = list.Count,
                page
            };

            return Json(result);
        }

        [Route("/image/{image_id}")]
        public async Task<FileContentResult> GetImageAsync(string image_id)
        {
            //  http://185.101.204.28:4489/toyota/EU/images_eu_a1/090002A.png
            //   EU_A1_47A909C.png
            //   eu_a1_MT0856A-png

            string[] strArr = image_id.Replace("-",".").Split("_");
            byte[] result = new byte[0];
            string fullPath = Ut.GetImagePath() + strArr[0] + "/images_" + strArr[0].ToLower() + "_" + strArr[1].ToLower() + "/" + strArr[2];

            try
            {
                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler))
                    {
                       result = await client.GetByteArrayAsync(fullPath);
                    }
                }
            }
            catch(Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            return File(result, "image/png");
        }

        [Route("/models")]
        public IActionResult GetModels(string brand_id = "TOYOTA")
        {
            List<ModelCar> list = ClassCrud.GetModelCars(brand_id);
            return Json(list);
        }

        [Route("/vehicle/{vehicle_id:required}/mgroups")]
        public IActionResult GetPartsGroups(string vehicle_id)
        {
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }

            List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id, lang);
            return Json(list);
        }
        //  /vehicle/EU_252230_007_515G/sgroups/EU_252230_1201
        [Route("/vehicle/{vehicle_id:required}/sgroups/{node_id:required}")]    //   5
        public IActionResult GetSpareParts(string vehicle_id, string node_id, string brand_id = "TOYOTA")
        {
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }

            DetailsInNode detailsInNode = ClassCrud.GetDetailsInNode(vehicle_id, node_id, lang, brand_id);
            return Json(detailsInNode);
        }
        [HttpPost]    //   6.1
        [Route("/vehicle/{vehicle_id:required}/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id,  string[] codes, string[] node_id, [FromBody] codePost n1)
        {
            VehicleStruct_ID vs = new VehicleStruct_ID(vehicle_id);

            //string catalog = 
            //string catalog_code,
            //string part_group,
            //string compl_code,

            #region lang
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }
            #endregion

            if (!String.IsNullOrEmpty(group_id))
            {
                string part_group = group_id.Substring(group_id.LastIndexOf("_") + 1, group_id.Length - (group_id.LastIndexOf("_") + 1));

                List<Sgroups> list = ClassCrud.GetSgroups(vs.catalog, vs.catalog_code, part_group, vs.compl_code, lang);
                return Json(list);
            }
            //   else if ((codes != null && codes.Length > 0) || (node_id != null && node_id.Length > 0))
            else if (n1 != null)
            {
                string part_group = string.Empty;

                for(int i=0; i<n1.codes.Length; i++)
                {
                    if(i==0)
                    {
                        part_group += "'" + n1.codes[i] + "'";
                    }
                    else
                    {
                        part_group += ", '" + n1.codes[i] + "'";
                    }
                }

                List<Sgroups> list = ClassCrud.GetSgroups(vs.catalog, vs.catalog_code, part_group, vs.compl_code, lang);
                return Json(list);
            }

            return NotFound("Проверте параметры!");
        }

        [Route("/filters")]   //  [FromQuery] and [FromRoute]
        public IActionResult GetFilters(string model_id, [FromQuery(Name = "params[]")] string[] param, string brand_id = "TOYOTA")  
        {
            List<Filters> list = ClassCrud.GetFilters(model_id,  param, brand_id);
            return Json(list);
        }

        [Route("/filter-cars")]
        public IActionResult GetListCarTypeInfoFilterCars(string model_id, [FromQuery(Name = "params[]")] string[] param, string brand_id = "TOYOTA", int page=1, int page_size=10)
        {
            List<header> headerList = ClassCrud.GetHeaders(8);
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(model_id, param, brand_id); // 

            int count = list.Count;
            list = list.Skip((page - 1) * page_size).Take(page_size).ToList();

            var result = new
            {
                header = headerList,
                items = list,
                cnt_items = count,
                page = page
            };
            return Json(result);
        }

        [Route("/locales")]
        public IActionResult GetLang()
        {
            List<lang> list = ClassCrud.GetLang();
            return Json(list);
        }

        [Route("/vehicle/wmi")]
        public IActionResult GetWmi()
        {
            List<string> list = ClassCrud.GetWmi();
            return Json(list);
        }

        [Route("/vehicle/{vehicle_id:required}")]
        public IActionResult GetVehiclePropArr(string vehicle_id)
        {
            try
            {
                VehiclePropArr result = ClassCrud.GetVehiclePropArr(vehicle_id);
                return Json(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("/model")]
        public IActionResult GetModelAndBrand(string model_id, string brand_id = "10")
        {
            model model = ClassCrud.GetModelAndBrand(model_id);
            return Json(model);
        }

        [Route("/codes-for-tree")]
        public IActionResult CodesForTree(string vehicle_id)
        {
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }

            List<string> list = ClassCrud.CodesForTree(vehicle_id, lang);
            return Json(list);
        }
        //[HttpPost]
        //[Route("/vehicle/{vehicle_id:required}/sgroups")]
        //public IActionResult GetNodes(string vehicle_id, string[] codes, string[] node_ids)
        //{
        //    List<node> list = ClassCrud.GetNodes(codes, node_ids);
        //    return Json(list);
        //}

    }
}