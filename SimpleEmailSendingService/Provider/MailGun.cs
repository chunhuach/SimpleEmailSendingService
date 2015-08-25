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
using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
using Newtonsoft.Json.Linq;

namespace SimpleEmailSendingService
{
    /// <summary>
    /// Wraps functionality of MailGun, provide unified interface for processing a SendMail request
    /// </summary>
    internal class MailGun : MailServiceProvider
    {
        /// <summary>
        /// The MailServiceProvider Type
        /// </summary>
        public override ProviderType ProviderType
        {
            get
            {
                return ProviderType.MailGun;
            }
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="request">Contains detailed email info and optionally provider info (such as ApiKeys)</param>
        /// <param name="messageId">The message id returned by the service provider</param>
        /// <param name="detailMessage">The message returned by the service provider, e.g. "failed to send message", "Sent", "Message Queued for delivery"</param>
        /// <returns>True if email was sent successfully</returns>
        public override bool SendMail(SendMailRequest mailRequest, out string messageId, out string detailMessage)
        {
            // create request
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["MailGunBaseUrl"]);

            string apiKey = GetApiKey(mailRequest);
            client.Authenticator = new HttpBasicAuthenticator("api", apiKey);

            RestRequest request = new RestRequest();
            string domain;
            ProviderInfo pi = mailRequest.Providers.SingleOrDefault(p => p.ProviderType == ProviderType);
            if (pi != null)
            {
                domain = pi.SenderDomain;
            }
            else
            {
                domain = ConfigurationManager.AppSettings["MailGunDomain"];
            }

            // Email detail
            request.AddParameter("domain", domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", mailRequest.From);

            foreach (string s in mailRequest.To)
            {
                request.AddParameter("to", s);
            }

            request.AddParameter("subject", mailRequest.Subject);
            request.AddParameter("text", mailRequest.Text);
            request.Method = Method.POST;

            // Send email
            var response = client.Execute(request);

            // process response
            messageId = "";
            detailMessage = "";

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    dynamic json = JObject.Parse(response.Content);
                    messageId = json.id;
                    detailMessage = json.message;
                }
                catch
                {
                    detailMessage = "Failed to parse Mailgun response.";
                }

                // If status is OK we assume message is sent
                // Even if the message from MailGun says something else - we have done our job sending the email
                return true;
            }
            else
            {
                detailMessage = String.Format("Https status code: {0}", response.StatusCode);
                return false;
            }
        }
    }
}