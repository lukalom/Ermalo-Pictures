using EP.Application.Services.DiscountCoupon;
using EP.Application.Services.DiscountCoupon.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EP.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DiscountCouponController : ControllerBase
    {

        private readonly IDiscountCouponService _couponService;
        public DiscountCouponController(IDiscountCouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            var result = await _couponService.GetById(id);
            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountCouponDTO model)
        {
            var result = await _couponService.CreateCoupon(model);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _couponService.GetByCode(code);

            if (result.IsSuccess)
            {
                return Ok(result.Content);
            }

            return BadRequest(result.Error);
        }
    }
}