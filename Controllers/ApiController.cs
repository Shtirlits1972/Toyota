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

namespace Toyota.Controllers
{
    public class ApiController : Controller
    {
        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin)
        {
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin); 
            List<header> headerList = ClassCrud.GetHeaders();

            var result = new
            {
                headers = headerList,
                items = list,
                cntitems = list.Count,
                page = 1
            };

            return Json(result);
        }

        [Route("/image")]
        public IActionResult GetImage(string image_id)
        {
            //  TOYOTA/EU/grimages/47A909C.png
            string FilderImagePath = Ut.GetImagePath();  //"wwwroot/image/";
            string fullPath = FilderImagePath + image_id;

            if(System.IO.File.Exists(fullPath))
            {
                byte[] file = System.IO.File.ReadAllBytes(fullPath);
                return Ok(file);
            }

            return NotFound("Картинка не найдена.");
        }

        [Route("/models")]
        public IActionResult GetModels(string brand_id = "TOYOTA")
        {
            List<ModelCar> list = ClassCrud.GetModelCars(brand_id);
            return Json(list);
        }

        [Route("/vehicle/{vehicle_id:required}/mgroups")]
        public IActionResult GetPartsGroups(string vehicle_id, string code_lang = "EN")
        {
            List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id, code_lang);
            return Json(list);
        }
 
        [Route("/vehicle/{vehicle_id:required}/sgroups/{node_id:required}")]
        public IActionResult GetSpareParts(string vehicle_id, string node_id, string lang, string brand_id = "TOYOTA")
        { 
            DetailsInNode detailsInNode = ClassCrud.GetDetailsInNode(node_id, lang, brand_id);
            return Json(detailsInNode);
        }

        [Route("/vehicle/{vehicle_id:required}/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, code_lang);
            return Json(list);
        }

        [Route("/ﬁlters")]   //  [FromQuery] and [FromRoute]
        public IActionResult GetFilters(string model_id, [FromQuery(Name = "params")] string[] param, string brand_id = "TOYOTA")  //  
        {
            List<Filters> list = ClassCrud.GetFilters(model_id,  param, brand_id);
            return Json(list);
        }

        [Route("/ﬁlter-cars")]
        public IActionResult GetListCarTypeInfoFilterCars(string model_id, [FromQuery(Name = "params")] string[] param, string brand_id = "TOYOTA", int page=1, int page_size=10)
        {
            List<header> headerList = ClassCrud.GetHeaders();
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(model_id, param, brand_id); // 

            list = list.Skip((page - 1) * page_size).Take(page_size).ToList();

            var result = new
            {
                headers = headerList,
                items = list,
                cntitems = list.Count,
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

        [HttpPost]
        [Route("/vehicle/{vehicle_id:required}/sgroups")]
        public IActionResult GetNodes(string vehicle_id, string [] codes, string [] node_ids)
        {
            List<node> list = ClassCrud.GetNodes(codes, node_ids);
            return Json(list);
        }

    }
}