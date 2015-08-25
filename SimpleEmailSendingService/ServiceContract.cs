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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SimpleEmailSendingService
{
    /// <summary>
    /// Main service contract
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string Ping();

        [OperationContract]
        [WebInvoke(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        SendMailResult SendMail(SendMailRequest request);
    }

    /// <summary>
    /// Mail service provider Type
    /// </summary>
    [DataContract]
    public enum ProviderType
    {
        SendGrid,

        MailGun,

        Mandrill,
    }

    /// <summary>
    /// Mail service provider information
    /// </summary>
    [DataContract]
    public class ProviderInfo
    {
        [DataMember]
        public ProviderType ProviderType { get; set; }

        [DataMember]
        public string ApiKey { get; set; }

        /// <summary>
        /// Send domain required by some service provider(e.g. MailGun), usually this is the domain the user registered with the provider
        /// </summary>
        [DataMember]
        public string SenderDomain { get; set; }
    }

    /// <summary>
    /// Send mail request
    /// </summary>
    [DataContract]
    public class SendMailRequest
    {
        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string From {get; set;}

        [DataMember]
        public List<string> To { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public List<ProviderInfo> Providers { get; set; }
    }

    /// <summary>
    /// Send mail result status
    /// </summary>
    [DataContract]
    public enum SendMailResultStatus
    {
        Success,
        Error,
    }

    /// <summary>
    /// Send mail result
    /// </summary>
    [DataContract]
    public class SendMailResult
    {
        public SendMailResult()
        {
            Messages = new List<string>();
        }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string MessageId { get; set; }

        [DataMember]
        public string ProviderUsed { get; set; }

        [DataMember]
        public List<string> Messages { get; set; }
    }
}
