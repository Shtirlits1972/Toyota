using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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
    }
}
