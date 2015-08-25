# Simple Email Sending Service

This is a simple email service for sending emails using different providers. 

When you need a simple email service for your application (e.g. a "a contact me" form for your web users to send email to you), there isn't much free and easy service available online. Major service providers do provide free email capabilities, often time require service specific setups and interfaces. 

This service utilize these service providers, wrap their API to provide a small simple SendMail API so that it's more consumable for small applications; In the back-end it can try one provider and if that doesn't work, it will fail over to other providers. 

## More about this project
The solution focus on back-end, it use C# and WCF to build the service. Underneath it use several open source libraries to help with the mission, e.g. MailGunApi, RestSharp. The code architecture provides abstraction over MailServiceProviders so providers can be easily added/removed/modified. The service is now hosted on IIS in Azure, which can be scaled out easily.

This is the initial version, due to time constraint there is a lot of room for improvement, e.g.:

1. More testing, thorough testing. Currently there are only several high level test cases. Need more detailed unit test and some end to end test.
2. Provide a simple web UI "contact me" form to invoke the service, this can verify the service and also provide a template for potential users of this service. 
3. This simple service does not currently support some email features like attachment, HTML body, CC, BCC, etc.

To know more about me see my [Linkedin Profile](https://www.linkedin.com/in/chunhua)

## Live Service!

The service is hosted live at [https://sess.azurewebsites.net](https://sess.azurewebsites.net)

## Build/Deploy
1. Close the project
2. Double click on SimpleEmailSendingService.sln to open the project
3. (make sure you install NuGet first) in Visual Studio, select Tools -> NuGet Package Manager -> Package Manager Console, type *Update-Package –reinstall*
4. (Optional) modify app.config to configure your provider API keys. 
4. Press F5 to build
5. Right click on the project and select "publish", then follow the instructions to deploy to your WCF hosting web location. 

## API Usage

**API:** The service provide a single API: SendMail

**End point:** The service expose two end point:

1. An SOAP end point: **https://sess.azurewebsites.net/soap**. This is recommended but it's more work for client to construct a request. If you are using a sophisticated client, you can construct a SOAP message and post to this end point. If you are using .NET, you can use svcutil.exe to generate strong typed client proxy stub. 
2. A transitional web end point which takes JSON or normal HTTP post: **https://sess.azurewebsites.net/service.svc/json/SendMail**.

**Parameters** - The SendMail API support following parameters:

1. **From** - this is the sender, it can be a simple email address (e.g. "a@mail.com"), or "Name <email>", e.g. "Alice Bob <a@mail.com>"
2. **To** - This is the recipients. It's a list of recipients, each recipient can be a single email address or "Name <email>"  
3. **Subject** - The subject of the email
4. **Text** - the body text of the email.
5. **ProviderInfo** - A list of ProviderInfo which specify email service provider API keys. If no API key is provided, the service will use pre-configured API keys. You can configure your own API keys if you host the service yourself. ProviderInfo={ProviderType=SendGrid/MailGun/ManDril, ApiKey="String"}

    
**Sample JSON posted**

{"From":"TestFrom@TestDomain.com","Providers":[{"ApiKey":"your-api-key","ProviderType":"SendGrid"}],"Subject":"Test Subject","Text":"My test text.","To":["simpleemailsendingservice@gmail.com"]}

**Sample response**

{"MessageId":"message-id-string","Messages":[list of messages],"ProviderUsed":SendGrid,"Status":Success}


## History

1. 8/24/2015 initial commit


## License

The [MIT license](https://en.wikipedia.org/wiki/MIT_License)
