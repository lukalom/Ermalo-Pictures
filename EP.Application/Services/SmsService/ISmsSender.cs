using EP.Application.DTO_General.Generic;

namespace EP.Application.Services.SmsService
{
    public interface ISmsSender
    {
        Task<Result<string>> SendSms(string smsMessage);
    }
}
