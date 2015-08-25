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
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using SendGrid;
using Exceptions;

namespace SimpleEmailSendingService
{
    /// <summary>
    /// Wraps functionality of SendGrid, provide unified interface for processing a SendMail request
    /// </summary>
    internal class SendGrid : MailServiceProvider
    {
        /// <summary>
        /// The MailServiceProvider Type
        /// </summary>
        public override ProviderType ProviderType
        {
            get
            {
                return ProviderType.SendGrid;
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
            // Create the email object
            var msg = new SendGridMessage();

            // email detail
            msg.Subject = request.Subject;
            msg.From = new MailAddress(request.From);
            msg.AddTo(request.To);
            msg.Text = request.Text;

            // send email
            string apiKey = GetApiKey(request);

            var transportWeb = new Web(apiKey);
            var t = transportWeb.DeliverAsync(msg);
            t.Wait();

            // process response
            // send grid does not provide any of the information, so set them to be empty
            messageId = "";
            detailMessage = "";
            return true;
        }
    }
}
