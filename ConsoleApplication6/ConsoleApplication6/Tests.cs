using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;


   
    public class GenerationData
    {
        public Dictionary<int, string> GenerateData()
        {
            Dictionary<int, string> testData = new Dictionary<int, string>();
            for (int i = 0; i < 200; i++)
            {
                testData.Add(i, i+"value");
            }
            return testData;
        }

    }

    public class DataSender
    {
        public string host;
        public HttpClient client =  new HttpClient();

        public string Put(int id, string value)
        {
            var data = new StringContent(JsonConvert.SerializeObject(value));
            data.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json"); ;
            var res = client.PutAsync(host + id, data).Result;
            return res.ToString();

        }

         public HttpContent Get()
        {
            var res = client.GetAsync(host).Result;
            Console.WriteLine();
            var codeResult = res.StatusCode;
            /*if (codeResult.ToString() != "OK")
            {
                return codeResult.ToString();
            }
            else
            {*/
                return res.Content;
            //}
         }
        public string Get(int key)
        {
  
            var res = client.GetAsync(host + key).Result;
            var codeResult = res.StatusCode;
            if (codeResult.ToString() != "OK")
            {
                return codeResult.ToString();
            }
            else
            {
                return JsonConvert.DeserializeObject(res.Content.ReadAsStringAsync().Result).ToString();
            }

        }
        public void Delete(int key)
        {
            var res = client.DeleteAsync(host + key).Result;

        }
    }

    [TestClass]
    public class Tests
    {
        static private string port = "9000";
        static private string extensionFile = ".txt";
        static private string dbFolderName = "/db/";
        private string fileName = AppDomain.CurrentDomain.BaseDirectory + dbFolderName + port + extensionFile;
        public Dictionary<int, string> testData;
        public DataSender sender = new DataSender();

        public Tests()
        {
           
            sender.host = "http://localhost:" + port + "/api/values/";
            testData = new GenerationData().GenerateData();

        }

        [TestMethod]
        public void TestStartNode()
        {
            Process.Start("ConsoleApplication6.exe", port);

        }
        [TestMethod]
        public void TestClearDBFile()
        {
            File.WriteAllText(fileName, string.Empty);
            Assert.AreEqual(new FileInfo(fileName).Length, 0);
        }



        [TestMethod]
        public void TestPutData()
        {
            int lenghtFile = File.ReadAllLines(fileName).Length;

            foreach (var item in testData)
            {
                sender.Put(item.Key, item.Value);
            }
            int newlentghtFile = File.ReadAllLines(fileName).Length;

            Assert.AreEqual(newlentghtFile, lenghtFile + testData.Count);

        }

        [TestMethod]
        public void TestGetData()
        {

            foreach (var item in testData)
            {
                string content = sender.Get(item.Key);
                Assert.AreNotEqual(content, "NotFound");
                Assert.AreEqual(content, item.Value);
            }

        }

        [TestMethod]
        public void TestUpdateData()
        {
            foreach (var item in testData)
            {
                sender.Put(item.Key, item.Value);
            }

            TestGetData();

        }
        [TestMethod]
        public void TestRemoveData()
        {
            foreach (var item in testData)
            {
                sender.Delete(item.Key);
            }
            Assert.AreEqual(File.ReadAllLines(fileName).Length, 0);
        }

        [TestMethod]
        public void TestCheckRemovedData()
        {
            foreach (var item in testData)
            {
                var value = sender.Get(item.Key);
                Assert.AreEqual(value, "NotFound");
            }
         
        }

    }
[TestClass]
public class TestSlaves
{
    public static string masterPort = "9000";
    public static string[] slavesPorts = { "9001", "9002" };
    public DataSender sender = new DataSender();
    public Dictionary<int, string> testData;

    public TestSlaves()
    {
        sender.host = "http://localhost:" + masterPort + "/api/values/";
        testData = new GenerationData().GenerateData();
        StartSlavesWithMaster();
        PutDataToMaster();
    }

    public void StartSlavesWithMaster()
    {
        Process.Start("ConsoleApplication6.exe", slavesPorts[0]);
        Process.Start("ConsoleApplication6.exe", slavesPorts[1]);
        Process.Start("ConsoleApplication6.exe", masterPort + " " + slavesPorts[0] + " " + slavesPorts[1]);

    }

    public void PutDataToMaster()
    {
        foreach (var item in testData)
        {
            sender.Put(item.Key, item.Value);
        }
    }

    [TestMethod]
    public void TestCheckSlaveData()
    {
       
        sender.host = "http://localhost:" + slavesPorts[0] + "/api/values/";
        foreach (var item in testData)
        {
            string content = sender.Get(item.Key);
            Assert.AreNotEqual(content, "NotFound");
            Assert.AreEqual(content, item.Value);
        }

        sender.host = "http://localhost:" + slavesPorts[1] + "/api/values/";
        foreach (var item in testData)
        {
            string content = sender.Get(item.Key);
            Assert.AreNotEqual(content, "NotFound");
            Assert.AreEqual(content, item.Value);
        }

    }



    }

[TestClass]
public class TestSharding
{
    public static string proxyPort = "8000";
    public static string[] ports = { "9000", "9001", "9002" };
    public static int countNodes = 3;
    public DataSender sender = new DataSender();
    public Dictionary<int, string> testData;

    public TestSharding()
    {
        sender.host = "http://localhost:" + proxyPort + "/api/proxy/";
        testData = new GenerationData().GenerateData();
        StartProxyWithNodes();
    }


    public void StartProxyWithNodes()
    {
        Process.Start("ConsoleApplication6.exe", ports[0]);
        Process.Start("ConsoleApplication6.exe", ports[1]);
        Process.Start("ConsoleApplication6.exe", ports[2]);
        Process.Start(AppDomain.CurrentDomain.BaseDirectory+ "/../../../ProxySharding/bin/Debug/ProxySharding.exe", proxyPort + " " + countNodes + " " + ports[0] + " " + ports[1] + " " + ports[2]);

    }

    [TestMethod]
    public void TestPutDataToProxy()
    {
        foreach (var item in testData)
        {
            sender.Put(item.Key, item.Value);
        }
    }


}



