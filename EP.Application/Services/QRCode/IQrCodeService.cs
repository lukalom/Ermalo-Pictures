using EP.Application.DTO_General.Generic;

namespace EP.Application.Services.QRCode
{
    public interface IQrCodeService
    {
        string GenerateQrCode(string data);
        Task<Result<bool>> CheckQrCode(string uniqueCode);
    }
}
