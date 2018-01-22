using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using System.IO;

namespace ConsoleApplication6
{
    public class ValuesController : ApiController
    {

        public IHttpActionResult Get()
        {
            var db = Program.dataBase.Keys;
            return Ok(db);
        }
       
        // GET api/values/5 
        public IHttpActionResult Get(int id)
        {
            
            string value = null;
            try
            {
                value = Program.dataBase[id];
                WriteToLog("get ", id);
                return Ok(value);
            }
            catch (Exception)
            {
                return NotFound();
            }
            
   
        }

     

        // PUT api/values/5 
        public IHttpActionResult Put(int id, [FromBody]string value)
        {
          
            if (Program.dataBase.ContainsKey(id))
            {
                Program.dataBase[id] = value;
            }
            else
            {
                Program.dataBase.Add(id, value);
            }
           
            WriteDataToFile();
            WriteToLog("put", id, value);
            if (Program.portsSlaves.Length != 0)
            {
                Program.PutDataToSlave(id, value);
            }
           

            return Ok();

        }

        public IHttpActionResult Delete(int id)
        {
            if (Program.dataBase.ContainsKey(id))
            {
                Program.dataBase.Remove(id);
            }
            WriteDataToFile();
            WriteToLog("delete", id);
            if (Program.portsSlaves.Length != 0)
            {
                Program.DeleteDataFromSlave(id);
            }
            return Ok();
        }


        private void WriteDataToFile()
        {
            using (StreamWriter sw = new StreamWriter(Program.fileName, false))
            {
                foreach (var data in Program.dataBase)
                {
                    sw.Write(data.Key + "-");
                    sw.WriteLine(data.Value);
                }
            }
        }

        private void WriteToLog(string method, int id, string value = "")
        {

            using (StreamWriter log = File.AppendText("logdb.txt"))
            {
                log.WriteLineAsync(method + " " + id + " " + value);
            }
                
        }


    }


}
