using KotaeteMVC.Context;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Service
{
    public abstract class ServiceBase
    {
        protected KotaeteDbContext _context;
        protected int _pageSize;

        public ServiceBase(KotaeteDbContext context, int pageSize)
        {
            _context = context;
            _pageSize = pageSize;
        }

        protected IEnumerable<T> GetPageFor<T>(IQueryable<T> query, int page)
        {
            return GetPageFor(query, page, _pageSize);
        }

        protected IEnumerable<T> GetPageFor<T>(IQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}