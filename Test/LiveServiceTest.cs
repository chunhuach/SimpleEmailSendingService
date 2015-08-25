#region License
/*
Copyright (c) 2015 Vincent Chunhua Chen


Permission is hereby granted, free of charge, to any person obtaining a copy 
of this software and associated documentation files (the "Software"), to deal 
in the Software without restriction, including without limitation the rights 
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions: 


The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software. 
 

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
OR OTHER DEALINGS IN THE SOFTWARE. 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEmailSendingService;
using System.IO;
using System.Net.Http.Headers;
using System.Configuration;

namespace Test
{
    [TestClass]
    public class LiveServiceTest
    {
        /// <summary>
        /// Test posting request to the live service
        /// sample request: {"From":"TestFrom@TestDomain.com","Providers":[{"ApiKey":"your-api-key","ProviderType":"SendGrid"}],"Subject":"Test Subject","Text":"My test text.","To":["simpleemailsendingservice@gmail.com"]}
        /// Sample response: {"MessageId":"message-id-string","Messages":[list of messages],"ProviderUsed":SendGrid,"Status":Success}
        /// </summary>
        [TestMethod]
        public void TestLiveSiteSendThroughSendGrid()
        {
            //======================================================
            // Note: set these values before your test
            // =====================================================
            string url = ConfigurationManager.AppSettings["LiveSiteUrl"] + "/SendMail";
            string apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];

            SendMailRequest request = new SendMailRequest()
            {
                From = "TestFrom@TestDomain.com",
                To = new List<string>() { "simpleemailsendingservice@gmail.com" },
                Subject = "Test Subject",
                Text = "My test text.",
                Providers = new List<ProviderInfo>(){
                    new ProviderInfo(){ ProviderType = ProviderType.SendGrid, ApiKey=apiKey}
                }
            };

            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(SendMailRequest));
            ser.WriteObject(stream1, request);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);
            string jsonReqStr = sr.ReadToEnd();
            var content = new StringContent(jsonReqStr, Encoding.UTF8, "application/json"); 

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
                var task = client.PostAsync(url, content);
                task.Wait();
                
                var readStrTask = task.Result.Content.ReadAsStringAsync();
                readStrTask.Wait();

                string response = readStrTask.Result;

                Console.WriteLine(response);
            }
        }
    }
}