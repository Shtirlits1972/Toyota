using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toyota.Models.Dto;
using Toyota.Models;
using System.IO;
using System.Configuration;

namespace Toyota.Controllers
{
    public class ApiController : Controller
    {
        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin8)
        {
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin8); 
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
        public IActionResult GetModels()
        {
            List<ModelCar> list = ClassCrud.GetModelCars();
            return Json(list);
        }

        [Route("/mgroups")]
        public IActionResult GetPartsGroups(string vehicle_id, string code_lang)
        {
            List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id, code_lang);
            return Json(list);
        }

        [Route("/vehicle")]
        public IActionResult GetSpareParts(string group_id, string code_lang)
        {   
            List<SpareParts> list = ClassCrud.GetSpareParts(group_id, code_lang);   
            return Json(list);
        }

        [Route("/vehicle/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, code_lang);
            return Json(list);
        }

        [Route("/ﬁlters")]
        public IActionResult GetFilters(string vin8)
        {
            List<Filters> list = ClassCrud.GetFilters(vin8);   
            return Json(list);
        }

        [Route("/ﬁlter-cars")]
        public IActionResult GetListCarTypeInfoFilterCars(string vin8, string [] param, int page=1, int page_size=10)
        {
            if(param.Length == 11)
            {
                List<header> headerList = ClassCrud.GetHeaders();
                List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(vin8, param); // 

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

            return NotFound("Проверьте параметры запроса!");
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

        [Route("/vehicleAttr")]
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
        [Route("/vehicle/sgroups")]
        public IActionResult GetNodes(string [] codes, string [] node_ids)
        {
            List<node> list = ClassCrud.GetNodes(codes, node_ids);
            return Json(list);
        }

    }
}