using System.Collections.Generic;
using System.Linq;

namespace KotaeteMVC.Helpers
{
    public class PaginationCreator<T>
    {
        public List<T> GetPage(IOrderedQueryable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<T> GetPage(IEnumerable<T> query, int page, int pageSize)
        {
            return query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}