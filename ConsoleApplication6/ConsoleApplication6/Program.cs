using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Owin.Hosting;
using System.Net.Http;
using System.IO;

namespace ConsoleApplication6
{
    class Program
    {
        public static Dictionary<int, string> dataBase = new Dictionary<int, string>();
        public static int iter = 0;
        public static string fileName;
        private string portNode;
        private string extensionFile = ".txt";
        private string dbFolderName = "/db/";
        static public string[] portsSlaves = null;
        private static DataSender dataSender = new DataSender();
       


        static void Main(string[] args)
        {
            var pr = new Program();
            var slaves = args.Skip(1).ToArray();
            pr.CreateNode(args[0], slaves.Length!=0 ? slaves : null);
            pr.CreateFileForDB();
            pr.ReadFile();
            pr.Start();
        }


        public void CreateNode(string port, string[] slaves = null)
        {
            portNode = port;
            portsSlaves = slaves;
            Console.WriteLine(slaves);
            fileName = AppDomain.CurrentDomain.BaseDirectory + dbFolderName + port + extensionFile ;
        }


        public void Start()
        {
            using (WebApp.Start<Startup>(url: "http://localhost:" + portNode + "/"))
            {
                Console.WriteLine("port " + portNode);
                while(true) { }
            }
        }
        public void CreateFileForDB()
        {

            if (!File.Exists(fileName))
            {
                File.Create(fileName);
            }
          
        }


        public void ReadFile()
        {
            StreamReader sr = new StreamReader(fileName);
            while (!sr.EndOfStream)
            {
                string tmp = sr.ReadLine();
                if (tmp.IndexOf("-") != -1)
                {
                    string[] data = tmp.Split('-');
                    dataBase.Add(Convert.ToInt32(data[0]), data[1]);

                }

            }

            sr.Close();
        }

        static public void PutDataToSlave(int id, string value)
        {
            foreach (var port in portsSlaves)
            {
                dataSender.host = GetSlaveHost(port);
               
                dataSender.Put(id, value);
            }
        }

        static public void DeleteDataFromSlave(int id)
        {
            foreach (var port in portsSlaves)
            {
                dataSender.host = GetSlaveHost(port);
                dataSender.Delete(id);
            }
        }

        static public string GetSlaveHost(string port)
        {
            return "http://localhost:" + port + "/api/values/";
        }

    }
}
