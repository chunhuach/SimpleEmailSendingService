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
using System.Configuration;
using SendGrid;
using Exceptions;

namespace SimpleEmailSendingService
{
    internal abstract class MailServiceProvider
    {
        /// <summary>
        /// The MailServiceProvider Type
        /// </summary>
        public abstract ProviderType ProviderType
        {
            get;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="request">Contains detailed email info and optionally provider info (such as ApiKeys)</param>
        /// <param name="messageId">The message id returned by the service provider</param>
        /// <param name="detailMessage">The message returned by the service provider, e.g. "failed to send message", "Sent", "Message Queued for delivery"</param>
        /// <returns>True if email was sent successfully</returns>
        public abstract bool SendMail(SendMailRequest request, out string messageId, out string detailMessage);

        /// <summary>
        /// Get the API key for calling the provider
        /// Will try to get the key from SendMailRequest if it's available; or else it will try to get it from AppConfig file 
        /// </summary>
        /// <param name="request">The request which might contain the API Key</param>
        /// <returns>The API Key, or null if not found.</returns>
        public string GetApiKey(SendMailRequest request)
        {
            string apiKey;

            ProviderInfo pi = request.Providers.SingleOrDefault(p => p.ProviderType == ProviderType);
            if (pi != null && !String.IsNullOrWhiteSpace(pi.ApiKey))    // ApiKey is provided in the request
            {
                apiKey = pi.ApiKey;
            }
            else    // ApiKey is not provided in the request, try get it from configuration file
            {
                switch (ProviderType)
                {
                    case ProviderType.SendGrid:
                        apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
                        break;

                    case ProviderType.MailGun:
                        apiKey = ConfigurationManager.AppSettings["MailGunApiKey"];
                        break;

                    case ProviderType.Mandrill:
                        apiKey = ConfigurationManager.AppSettings["MandrillApiKey"];
                        break;

                    default:
                        apiKey = null;
                        break;
                }
            }

            return apiKey;
        }

        /// <summary>
        /// Create and return a MailServiceProvider instance
        /// </summary>
        /// <param name="type">The type identifying the provider</param>
        /// <returns>MailServiceProvide object</returns>
        public static MailServiceProvider GetProvider(ProviderType type)
        {
            switch(type)
            {
                case ProviderType.SendGrid:
                    return new SendGrid();

                case ProviderType.MailGun:
                    return new MailGun();

                case ProviderType.Mandrill:
                    return new Mandrill();

                default:
                    throw new InvalidProgramException();
            }
        }
    }
}
