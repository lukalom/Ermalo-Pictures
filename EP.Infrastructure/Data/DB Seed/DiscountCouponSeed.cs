using EP.Infrastructure.Entities;
using EP.Infrastructure.IConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Data.DB_Seed
{
    public class DiscountCouponSeed : ISeeder
    {
        public readonly IUnitOfWork UnitOfWork;

        public DiscountCouponSeed(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public int Index { get; set; } = 9;

        public async Task Seed()
        {
            if (await UnitOfWork.DiscountCoupon.Query().CountAsync() == 0)
            {
                var discountCoupon = new DiscountCoupon { Code = "forstud", Uses = 10, Discount = 50 };
                await UnitOfWork.DiscountCoupon.AddAsync(discountCoupon);
                await UnitOfWork.SaveAsync();
            }
        }
    }
}
