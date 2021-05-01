using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CDS.APICore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[,] a = new int[2, 3];

            for (int i = 0; i < a.GetLength(0); i++)
            {
                int[] innerArr = Array.ConvertAll(Console.ReadLine().Split(' '), int.Parse);

                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = innerArr[j];
                }
            }



            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
