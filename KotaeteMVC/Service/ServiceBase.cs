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
            return query.Skip((page - 1) * _pageSize).Take(_pageSize);
        }
    }
}