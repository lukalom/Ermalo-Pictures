using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Configuration;
using EP.Shared.Exceptions.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using ZXing;
using ZXing.QrCode;

namespace EP.Application.Services.QRCode
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<QrCodeService> _logger;
        private readonly EnvConfig _envConfig;
        private readonly Shared.Configuration.QRCode _qrCode;


        public QrCodeService(IUnitOfWork unitOfWork,
            ILogger<QrCodeService> logger,
            IOptionsMonitor<EnvConfig> envConfig,
            IOptionsMonitor<Shared.Configuration.QRCode> qrCode)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _envConfig = envConfig.CurrentValue;
            _qrCode = qrCode.CurrentValue;
        }

        public async Task<Result<bool>> CheckQrCode(string uniqueCode)
        {
            var result = new Result<bool>();
            var payment = await _unitOfWork.Payment.GetFirstOrDefaultAsync(x =>
                x.TransactionUniqueCode.ToString() == uniqueCode);

            if (payment == null)
            {
                result.Error = ErrorHandler.PopulateError(
                    (int)StatusCode.NotFound,
                    "Invalid Qr Code payment with this id not found",
                    ErrorMessages.Generic.TypeBadRequest);

                _logger.LogInformation($"Invalid Qr Code payment with this id not found");

                return result;
            }

            result.Content = true;
            return result;
        }

        public string GenerateQrCode(string data)
        {
            var appUrl = _qrCode.ApplicationUrl;
            var writer = new QRCodeWriter();
            var resultBit = writer.encode(appUrl + data, BarcodeFormat.QR_CODE, 200, 200);

            const int scale = 2;
            const string extension = ".JPEG";
            var fileName = Guid.NewGuid().ToString();

            var resultImg = new Bitmap(resultBit.Width * scale, resultBit.Height * scale);
            for (var x = 0; x < resultBit.Height; x++)
            {
                for (var y = 0; y < resultBit.Width; y++)
                {
                    var pixel = resultBit[x, y] ? Color.Black : Color.White;
                    for (var i = 0; i < scale; i++)
                    {
                        for (var j = 0; j < scale; j++)
                        {
                            resultImg.SetPixel(x * scale + i, y * scale + j, pixel);
                        }
                    }
                }
            }

            var wwWebRootPath = _envConfig.WebRootPath;

            //var uploads = Path.Combine(wwWebRootPath, "\\Images\\QrCodes");
            var uploads = wwWebRootPath + "\\Images\\QrCodes\\";
            resultImg.Save(Path.Combine(uploads, fileName + extension));


            return fileName + extension;
        }
    }
}
