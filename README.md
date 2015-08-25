# Simple Email Sending Service

This is a simple email service for sending emails using different providers. Client post information to the service, the service will try to send the email using one of 3 email providers: SendGrid, MailGun and Mandrill. If one provider is down or return an error, it will try the next provider.    

## Installation

The service is hosted live at [https://sess.azurewebsites.net](https://sess.azurewebsites.net)

To deploy your own copy, open the solution in Visual Studio, build it, right click on the project and select "publish", then follow the instructions to deploy to your WCF hosting web location. Optionally you can modify app.config to configure your provider API keys.

## Usage

**End point:** The service expose two end point:

1. An SOAP end point: **https://sess.azurewebsites.net/soap**. This is recommended but it's more work for client to construct a request. If you are using a sophisticated client, you can construct a SOAP message and post to this end point. If you are using .NET, you can use svcutil.exe to generate strong typed client proxy stub. 
2. A transitional web end point which takes JSON or normal HTTP post: **https://sess.azurewebsites.net/service.svc/json/SendMail**.

**Parameters** The service support following parameters:

1. **From** - this is the sender, it can be a simple email address (e.g. "a@mail.com"), or "Name <email>", e.g. "Alice Bob <a@mail.com>"
2. **To** - This is the recipients. It's a list of recipients, each recipient can be a single email address or "Name <email>"  
3. **Subject** - The subject of the email
4. **Text** - the body text of the email.
5. **ProviderInfo** - A list of ProviderInfo which specify email service provider API keys. If no API key is provided, the service will use pre-configured API keys. You can configure your own API keys if you host the service yourself. ProviderInfo={ProviderType=SendGrid/MailGun/ManDril, ApiKey="String"}

This simple service does not currently support other email features like attachment, HTML body, CC, BCC, etc.
    


## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## History

1. 8/24/2015 initial commit


## License

The [MIT license](https://en.wikipedia.org/wiki/MIT_License)
