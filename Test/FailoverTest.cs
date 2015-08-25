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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEmailSendingService;

namespace Test
{
    [TestClass]
    public class FailoverTest
    {
        // Test the scenario where user specified valid API Key for one provider, verify that provider will be used
        [TestMethod]
        public void TestSendThroughProviderSpecified()
        {
            SendMailRequest request = Utility.GetTemplateRequest();
            request.Providers = new List<ProviderInfo>()
            {
                new ProviderInfo { ProviderType = ProviderType.SendGrid, ApiKey= Configuration.SendGridApiKey },
            };

            SendMailResult result = SendMailRequestProcessor.ProcessSendMailRequest(request);

            Assert.AreEqual(result.ProviderUsed, ProviderType.SendGrid);
        }

        // Test the scenario where user specified invalid API Key for one provider, verify that provider will be not used
        [TestMethod]
        public void TestFailoverToSecondProvider()
        {
            SendMailRequest request = Utility.GetTemplateRequest();
            request.Providers = new List<ProviderInfo>()
            {
                new ProviderInfo { ProviderType = ProviderType.SendGrid, ApiKey= "IncorrectApiKey" },
                new ProviderInfo { ProviderType = ProviderType.MailGun, ApiKey= Configuration.MailGunApiKey },
            };

            SendMailResult result = SendMailRequestProcessor.ProcessSendMailRequest(request);
            Assert.AreEqual(result.ProviderUsed, ProviderType.MailGun);
        }

        // Test the scenario where user specified valid API Key for two provider, verify failover heppens and third provider is used
        [TestMethod]
        public void TestFailoverToThirdProvider()
        {
            SendMailRequest request = Utility.GetTemplateRequest();
            // specify incorrect key for two providers, the third one should be used
            request.Providers = new List<ProviderInfo>()
            {
                new ProviderInfo { ProviderType = ProviderType.SendGrid, ApiKey= "IncorrectApiKey" },
                new ProviderInfo { ProviderType = ProviderType.MailGun, ApiKey= "IncorrectApiKey" },
            };

            SendMailResult result = SendMailRequestProcessor.ProcessSendMailRequest(request);
            Assert.AreEqual(result.ProviderUsed, ProviderType.Mandrill);
        }
    }
}
