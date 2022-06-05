using EP.Application.DTO_General.Errors;
using EP.Application.DTO_General.Extension;
using EP.Application.DTO_General.Generic;
using EP.Application.Services.DiscountCoupon.DTO;
using EP.Infrastructure.Enums;
using EP.Infrastructure.IConfiguration;
using EP.Shared.Exceptions;
using EP.Shared.Exceptions.Messages;

namespace EP.Application.Services.DiscountCoupon
{
    public class DiscountCouponService : IDiscountCouponService
    {

        private readonly IUnitOfWork _unitOfWork;

        public DiscountCouponService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Infrastructure.Entities.DiscountCoupon>> CreateCoupon(DiscountCouponDTO couponDTO)
        {
            var result = new Result<Infrastructure.Entities.DiscountCoupon>();

            var coupon = new Infrastructure.Entities.DiscountCoupon();

            if (couponDTO.Discount <= 0 || couponDTO.Discount > 100)
            {
                result.Error = ErrorHandler.PopulateError(
                        (int)StatusCode.BadRequest,
                        "Discount Can't Be Less or Equal to 0 and Greater Than 100",
                        ErrorMessages.Generic.TypeBadRequest);

                return result;
            }

            coupon.Code = couponDTO.Code;

            coupon.Uses = couponDTO.Uses <= 0 ? 5 : couponDTO.Uses;

            coupon.Discount = couponDTO.Discount;

            await _unitOfWork.DiscountCoupon.AddAsync(coupon);

            await _unitOfWork.SaveAsync();

            result.Content = coupon;

            return result;
        }

        public async Task<Result<DiscountCouponDTO>> GetByCode(string code)
        {
            var result = new Result<DiscountCouponDTO>();

            if (code == null)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.InvalidPayload,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var coupon = await _unitOfWork.DiscountCoupon.GetFirstOrDefaultAsync(x => x.Code == code);

            if (coupon == null || coupon.IsDeleted)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.ObjectNotFound,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var couponDTO = new DiscountCouponDTO
            {
                Code = coupon.Code,
                Discount = coupon.Discount,
                Uses = coupon.Uses
            };

            result.Content = couponDTO;

            return result;
        }

        public async Task<Result<Infrastructure.Entities.DiscountCoupon>> GetById(int? id)
        {
            var result = new Result<Infrastructure.Entities.DiscountCoupon>();

            if (id is null or 0)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.InvalidPayload,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            var coupon = await _unitOfWork.DiscountCoupon.GetFirstOrDefaultAsync(x => x.Id == id);

            if (coupon == null)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.ObjectNotFound,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }


            result.Content = coupon;

            return result;
        }

        public async Task<Result<DiscountCouponDTO>> RemoveUse(string code)
        {
            var result = new Result<DiscountCouponDTO>();

            var coupon = await _unitOfWork.DiscountCoupon.GetFirstOrDefaultAsync(x =>
                x.Code == code
                && x.IsDeleted == false);

            if (coupon == null)
            {
                result.Error = new Error()
                {
                    Code = (int)StatusCode.NotFound,
                    Message = ErrorMessages.Generic.ObjectNotFound,
                    Type = ErrorMessages.Generic.TypeBadRequest
                };

                return result;
            }

            if (coupon.Uses <= 0)
            {
                throw new AppException("coupon has been expired");
            }

            coupon.Uses -= 1;

            if (coupon.Uses == 0)
            {
                coupon.IsDeleted = true;
            }

            var couponDTO = new DiscountCouponDTO
            {
                Code = coupon.Code,
                Uses = coupon.Uses,
                Discount = coupon.Discount,
            };

            result.Content = couponDTO;

            return result;
        }
    }
}
