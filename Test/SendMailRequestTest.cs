using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleEmailSendingService;

namespace Test
{
    [TestClass]
    public class SendMailRequestTest
    {
        [TestMethod]
        public void TestEmailFormat()
        {
            SendMailRequest request = Utility.GetTemplateRequest();

            // sender with name is accepted
            request.From = @"Test sender <sender@senderDomain.com>";

            // multiple recipients are accepted
            // recipient with name is accepted
            request.To.Add(@"SimpleMail SendingService <simpleemailsendingservice@gmail.com");

            request.Providers = new List<ProviderInfo>()
            {
                new ProviderInfo { ProviderType = ProviderType.SendGrid, ApiKey= Configuration.SendGridApiKey },
            };

            SendMailResult result = SendMailRequestProcessor.ProcessSendMailRequest(request);

            Assert.AreEqual(result.ProviderUsed, ProviderType.SendGrid);
        }
    }
}
