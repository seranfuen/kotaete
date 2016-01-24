using KotaeteMVC.Context;
using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Service
{
    public abstract class ServiceBase
    {
        protected KotaeteDbContext _context;
        private int _pageSize;

        public ServiceBase(KotaeteDbContext context, int pageSize)
        {
            _context = context;
            _pageSize = pageSize;
        }

        protected IEnumerable<T> GetPageFor<T>(IQueryable<T> query, int page)
        {
            return query.Skip(page * (_pageSize - 1)).Take(_pageSize);
        }
    }
}