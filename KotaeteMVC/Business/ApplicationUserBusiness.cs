using KotaeteMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KotaeteMVC.Business
{
    public class ApplicationUserBusiness : IDisposable
    {
        private ApplicationDbContext _db;

        public void Dispose()
        {
            _db.Dispose();
        }

    }
}