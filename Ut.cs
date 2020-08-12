using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Toyota.Models.Dto;

namespace Toyota
{
    public class Ut
    {
        public static string GetMySQLConnect(string lang = "EN")
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();

            return Configuration.GetConnectionString("MySqlConnect");
        }

        public static string GetImagePath()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();

            return Configuration.GetSection("MySettings").GetSection("imagePath").Value;
        }

        public static List<header> GetBrand()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();

            List<header> list = new List<header>();

            header toyota = new header();
            toyota.code = Configuration.GetSection("brand").GetSection("toyota").GetSection("brand_id").Value;
            toyota.title = Configuration.GetSection("brand").GetSection("toyota").GetSection("brand_name").Value;
            list.Add(toyota);

            header Lexus = new header();
            Lexus.code = Configuration.GetSection("brand").GetSection("Lexus").GetSection("brand_id").Value;
            Lexus.title = Configuration.GetSection("brand").GetSection("Lexus").GetSection("brand_name").Value;
            list.Add(Lexus);

            return list;
        }
    }
}
