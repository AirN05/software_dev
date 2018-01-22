using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.IO;
using ConsoleApplication6;


namespace ProxySharding
{
    class Program
    {
        public static string portProxy;
        public static int nodeCount;
        public static string[] portsNodes;
        static void Main(string[] args)
        {
            portProxy = args[0];
            nodeCount = Convert.ToInt32(args[1]);
            portsNodes = args.Skip(2).ToArray();
            Start();
        }

        public static void Start()
        {
            using (WebApp.Start<Startup>(url: "http://localhost:" + portProxy + "/"))
            {
                Console.WriteLine("port " + portProxy);
                Proxy.Resharding();
                
                while (true) { }
               
               
            }
        }
    }
}
