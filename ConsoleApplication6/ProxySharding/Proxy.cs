using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace ProxySharding
{
    public class Proxy
    {
        public static Dictionary<int, string> tmpData;
        public static DataSender sender = new DataSender();

        public static int ShardFunction(int id, int countNodes)
        {
            return id % countNodes;
        }

        public static string GetHost(int num)
        {
           
            int port = GetPort(num);
            return "http://localhost:" + port + "/api/values/";
        }

        public static int GetPort(int num)
        {
            return 9000 + num;
        }

        public static bool Resharding()
        {
            var shardPorts = Program.portsNodes;
            foreach (var item in shardPorts)
            {
                sender.host = "http://localhost:" + item + "/api/values/";
                var res = sender.Get();
                var list = JsonConvert.DeserializeObject<int[]>(res.ReadAsStringAsync().Result);
                if (list.Length != 0)
                {
                    foreach (var id in list)
                    {
                        var value = sender.Get(id);
                        var func = ShardFunction(Convert.ToInt32(id), Program.nodeCount);
                        var tmpPort = GetPort(func);
                        Console.WriteLine(tmpPort);
                        if (tmpPort != Convert.ToInt32(item))
                        {
                            sender.host = "http://localhost:" + tmpPort + "/api/values/";
                            sender.Put(id, value);
                            sender.host = "http://localhost:" + item + "/api/values/";
                            sender.Delete(id);
                        }

                    }
                }
            }

            return true;

        }


    }
}
