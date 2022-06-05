using EP.Application.DTO_General.Generic;
using EP.Shared.Configuration;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EP.Application.Services.SmsService
{
    public class SmsSender : ISmsSender
    {
        private TwilioConfig _twilioConfig { get; set; }
        public SmsSender(IOptionsMonitor<TwilioConfig> optionsMonitor)
        {
            _twilioConfig = optionsMonitor.CurrentValue;
        }

        //აქ უნდა შემოვიდეს ვის უნდა გაგეზავნო სმს და ტექსტი მარა ფული ღირს
        public async Task<Result<string>> SendSms(string smsMessage)
        {
            TwilioClient.Init(_twilioConfig.AccountSID, _twilioConfig.AuthToken);
            var to = new PhoneNumber(_twilioConfig.SendTo);
            var from = new PhoneNumber(_twilioConfig.PhoneNumber);
            var message = await MessageResource.CreateAsync(to: to, from: from, body: smsMessage);

            var result = new Result<string>
            {
                Content = message.Sid
            };
            return result;
        }
    }
}
