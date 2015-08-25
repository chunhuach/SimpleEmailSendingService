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
using System.Text;
using Exceptions;

namespace SimpleEmailSendingService
{
    /// <summary>
    /// Main driver class to process a SendMailRequest
    /// </summary>
    public class SendMailRequestProcessor
    {
        // All available MailServiceProviders
        private static List<ProviderType> allProviderTypes = new List<ProviderType>()
        {
            ProviderType.SendGrid,
            ProviderType.MailGun,
            ProviderType.Mandrill,
        };

        /// <summary>
        /// Given a SendMailRequest, return a list of MailServiceProvidersSuitable to process this request
        /// If use has provided an API key in the request, the related provider will be put at the begining of the list
        /// if user did not provided an API key in the request, the provider is put at the end
        /// If no API key is found, the provider won't be used
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static List<MailServiceProvider> GetOrderedMailServiceProviders(SendMailRequest request)
        {
            List<MailServiceProvider> providerList = new List<MailServiceProvider>();

            // Add providers user has specified and provided an API key for
            foreach (ProviderInfo p in request.Providers)
            {
                if (!String.IsNullOrWhiteSpace(p.ApiKey))
                {
                    providerList.Add(MailServiceProvider.GetProvider(p.ProviderType));
                }
            }

            // Add the rest of providers that have an API key in configuration file
            foreach (ProviderType pt in allProviderTypes)
            {
                if (!providerList.Exists(p => p.ProviderType == pt))
                {
                    MailServiceProvider p = MailServiceProvider.GetProvider(pt);
                    string apiKey = p.GetApiKey(request);
                    if (!String.IsNullOrWhiteSpace(apiKey))
                    {
                        providerList.Add(p);
                    }
                }
            }

            return providerList;
        }

        /// <summary>
        /// Process a SendMailRequest, send the email and generate proper response 
        /// </summary>
        /// <param name="request">The request containing email details</param>
        /// <returns>The result of the sending email action</returns>
        public static SendMailResult ProcessSendMailRequest(SendMailRequest request)
        {
            SendMailResult result = new SendMailResult();

            var providers = GetOrderedMailServiceProviders(request);

            // if no ApiKeys are found, we can't use any providers
            if (providers.Count == 0)
            {
                result.Status = SendMailResultStatus.Error.ToString();
                result.Messages.Add("No mail service provider available.");
                return result;
            }

            for (int i = 0; i < providers.Count; i++)
            {
                try
                {
                    string messageId;
                    string detailMessage;
                    
                    bool success = providers[i].SendMail(request, out messageId, out detailMessage);

                    // add message returned by the provider, if any
                    if (!String.IsNullOrWhiteSpace(detailMessage))
                    {
                        result.Messages.Add(detailMessage);
                    }

                    if (success)
                    {
                        result.Status = SendMailResultStatus.Success.ToString();
                        result.MessageId = messageId;
                        result.ProviderUsed = providers[i].ProviderType.ToString();
                        return result;
                    }
                    else
                    {
                        // we don't know why it failed, so continue and try next provider
                        result.Status = SendMailResultStatus.Error.ToString();
                        continue;
                    }
                }
                catch (Exception e)
                {
                    result.Status = SendMailResultStatus.Error.ToString();
                    result.Messages.Add(SendMailRequestProcessor.GetErrorStringFromException(e));

                    if (e is FormatException)
                    {
                        // some of the email address is not correctly formated. stop trying and return the error
                        return result;
                    }
                    else
                    {
                        // for other exceptions, ingore and try next provider
                        continue;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get error message from an exception. the error message will be returned as part of the SendEmailResult
        /// Need to deal with different kinds of exception and parse out the error messages
        /// </summary>
        /// <param name="ex">The exeption to be checked</param>
        /// <returns>The error message</returns>
        public static string GetErrorStringFromException(Exception ex)
        {
            List<Exception> list = new List<Exception>();

            AggregateException ae = ex as AggregateException;
            if (ae != null)
            {

                foreach (Exception ie in ae.InnerExceptions)
                {
                    list.Add(ie);
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (Exception e in list)
            {
                string msg = null;

                InvalidApiRequestException apiEx = e as InvalidApiRequestException;
                if (apiEx != null)
                {
                    // Error message from SendGrid a wrapped in InvalidApiRequestException.Errors
                    msg = String.Join(",", apiEx.Errors);
                }
                else
                {
                    msg = e.Message;
                }

                sb.Append(msg);
            }

            return sb.ToString();
        }
    }
}