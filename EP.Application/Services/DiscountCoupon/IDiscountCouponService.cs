using EP.Application.DTO_General.Generic;
using EP.Application.Services.DiscountCoupon.DTO;

namespace EP.Application.Services.DiscountCoupon
{
    public interface IDiscountCouponService
    {
        Task<Result<Infrastructure.Entities.DiscountCoupon>> GetById(int? id);

        Task<Result<Infrastructure.Entities.DiscountCoupon>> CreateCoupon(DiscountCouponDTO coupon);

        Task<Result<DiscountCouponDTO>> GetByCode(string code);

        Task<Result<DiscountCouponDTO>> RemoveUse(string code);
    }
}
