using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using System.IO;

namespace ProxySharding
{
    public class ProxyController : ApiController
    {

        private DataSender sender = new DataSender();
      
        
        public void Put(int id, [FromBody]string value)
        {

            var result = Proxy.ShardFunction(Convert.ToInt32(id), Program.nodeCount);
            sender.host = Proxy.GetHost(result);
            var res = sender.Put(id, value);

        }

        public string Get(int id)
        {
            var result = Proxy.ShardFunction(Convert.ToInt32(id), Program.nodeCount);
            sender.host = Proxy.GetHost(result);
            return sender.Get(id);
        }

        public void Delete(int id)
        {
            var result = Proxy.ShardFunction(Convert.ToInt32(id), Program.nodeCount);
            sender.host = Proxy.GetHost(result);
            sender.Delete(id);

        }


    }
}
