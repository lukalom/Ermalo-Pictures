using EP.Application.DTO_General.Generic;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Extensions
{
    public static class DataPagerExtension
    {
        public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> query,
            int page, int pageSize) where T : class
        {
            var pagedResult = new PagedResult<T>();

            page = (page <= 0) ? 1 : page;
            pagedResult.CurrentPage = page;
            pagedResult.PageSize = pageSize == 0 ? 10 : pageSize;

            var startRow = (page - 1) * pagedResult.PageSize;
            pagedResult.Items = await query.Skip(startRow).Take(pagedResult.PageSize).ToListAsync();

            pagedResult.TotalItems = await query.CountAsync();
            pagedResult.TotalPages = (int)Math.Ceiling(pagedResult.TotalItems / (double)pagedResult.PageSize);

            return pagedResult;
        }
    }
}
