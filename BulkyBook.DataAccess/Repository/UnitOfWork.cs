using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public IProductRepository Product { get; private set; }

        private AppDbContext dbx;

        public UnitOfWork(AppDbContext _dbx)
        {
            this.dbx = _dbx;
            Category = new CategoryRepository(dbx);
            CoverType = new CoverTypeRepository(dbx);
            Product = new ProductRepository(dbx);
        }

        public void Save()
        {
            dbx.SaveChanges();
        }
    }
}
