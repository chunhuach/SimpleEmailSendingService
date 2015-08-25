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
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Configuration;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;

namespace SimpleEmailSendingService
{
    /// <summary>
    /// Wraps functionality of Mandrill, provide unified interface for processing a SendMail request
    /// </summary>
    internal class Mandrill : MailServiceProvider
    {
        /// <summary>
        /// The MailServiceProvider Type
        /// </summary>
        public override ProviderType ProviderType
        {
            get
            {
                return ProviderType.Mandrill;
            }
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="request">Contains detailed email info and optionally provider info (such as ApiKeys)</param>
        /// <param name="messageId">The message id returned by the service provider</param>
        /// <param name="detailMessage">The message returned by the service provider, e.g. "failed to send message", "Sent", "Message Queued for delivery"</param>
        /// <returns>True if email was sent successfully</returns>
        public override bool SendMail(SendMailRequest request, out string messageId, out string detailMessage)
        {
            // Create request
            string apiKey = GetApiKey(request);
            MandrillApi api = new MandrillApi(apiKey);

            // Email details
            EmailMessage msg = new EmailMessage();
            msg.FromEmail = request.From;
            msg.To = request.To.Select(e=>new EmailAddress(e));
            msg.Text = request.Text;

            // Send email
            SendMessageRequest smReq = new SendMessageRequest(msg);
            Task<List<EmailResult>> task = api.SendMessage(smReq);
            task.Wait();

            // process response
            messageId = "";
            detailMessage = "";

            EmailResult er = null;
            if (task.Result != null && task.Result.Count >0)
            {
                er = task.Result[0];
            }

            if (er == null)
            {
                detailMessage = "Invalid return result from provider.";
                return false;
            }

            messageId = er.Id;
            detailMessage = er.Status.ToString();

            if (er.Status == EmailResultStatus.Queued
                || er.Status == EmailResultStatus.Scheduled
                || er.Status == EmailResultStatus.Sent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}