using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paginator
{
    public static class Paginator
    {
        /// <summary>
        /// Paginates an IQueryable; page is 1-based, 0 page size skips pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">Source to paginate</param>
        /// <param name="pageSize">Size of the page; if 0 skips pagination</param>
        /// <param name="page">Page to take; 1-based</param>
        /// <returns></returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int pageSize, int page)
        {
            if (pageSize < 0)
            {
                throw new ArgumentOutOfRangeException("Cannot paginate with negative page size");
            }
            else if (pageSize == 0)
            {
                return queryable;
            }

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException("Cannot get a page below 1");
            }

            return queryable.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Paginates an IQueryable and returns a complete pagination result; page is 1-based, 0 page size skips pagination. The paginated result is mapped to a new type
        /// </summary>
        /// <returns></returns>
        public static async Task<PaginatedResult<U>> GetPaginatedResultAsync<T, U>(
            this IQueryable<T> queryable,
            int pageSize,
            int page,
            Func<T, U> mapper,
            Func<IQueryable<T>, Task<int>> counterMaterializer = null,
            Func<IQueryable<T>, Task<List<T>>> materializer = null
            )
        {

            counterMaterializer ??= e => e.CountAsync();

            materializer ??= e => e.ToListAsync();

            var data = Paginate(queryable, pageSize, page);
            var count = await counterMaterializer(queryable);

            var materializedData = await materializer(data);

            return new PaginatedResult<U>()
            {
                Count = count,
                Data = materializedData.Select(mapper).ToList(),
                Page = page,
                PageSize = pageSize
            };
        }


        public static PaginatedResult<T> GetPaginatedResult<T>(
            this IEnumerable<T> itemList,
            Pagination pagination
            )
        {
            IEnumerable<T> data;

            if (pagination.PageSize < 0)
            {
                throw new ArgumentOutOfRangeException("Cannot paginate with negative page size");
            }
            else if (pagination.PageSize == 0)
            {
                data = itemList;
            }
            if (pagination.Page < 1)
            {
                throw new ArgumentOutOfRangeException("Cannot get a page below 1");
            }
            data = itemList.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize);

            return new PaginatedResult<T>()
            {
                Count = itemList.Count(),
                Data = data.ToList(),
                Page = pagination.Page,
                PageSize = pagination.PageSize
            };
        }

        public static Task<PaginatedResult<U>> GetPaginatedResult<T, U>(
            this IQueryable<T> queryable,
            Pagination pagination,
            Func<T, U> mapper,
            Func<IQueryable<T>, Task<int>> counterMaterializer = null,
            Func<IQueryable<T>, Task<List<T>>> materializer = null
            )
        {
            if (pagination == null)
            {
                throw new ArgumentOutOfRangeException("Cannot paginate without pagination");
            }
            return GetPaginatedResultAsync(queryable, pagination.PageSize, pagination.Page, mapper, counterMaterializer, materializer);
        }

        /// <summary>
        /// Paginates an IQueryable and returns a complete pagination result; page is 1-based, 0 page size skips pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">Source to paginate</param>
        /// <param name="pageSize">Size of the page; if 0 skips pagination</param>
        /// <param name="page">Page to take; 1-based</param>
        /// <returns></returns>
        public static Task<PaginatedResult<T>> GetPaginatedResult<T>(
            this IQueryable<T> queryable,
            int pageSize,
            int page,
            Func<IQueryable<T>, Task<int>> counterMaterializer = null,
            Func<IQueryable<T>, Task<List<T>>> materializer = null
            ) => GetPaginatedResultAsync(queryable, pageSize, page, e => e, counterMaterializer, materializer);
        
        /// <summary>
        /// Paginates an IQueryable and returns a complete pagination result; page is 1-based, 0 page size skips pagination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">Source to paginate</param>
        /// <param name="pagination">Pagination information</param>
        /// <returns></returns>
        public static Task<PaginatedResult<T>> GetPaginatedResult<T>(
            this IQueryable<T> queryable,
            Pagination pagination,
            Func<IQueryable<T>, Task<int>> counterMaterializer = null,
            Func<IQueryable<T>, Task<List<T>>> materializer = null
            ) => GetPaginatedResult(queryable, pagination, e => e, counterMaterializer, materializer);

    }
}
