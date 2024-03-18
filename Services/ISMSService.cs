using Twilio.Rest.Api.V2010.Account;

namespace e_commerce.Services
{
    public interface ISMSService
    {
        MessageResource Send(string phoneNumber, string message);
    }
}
