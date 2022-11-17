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
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }

        private AppDbContext dbx;

        public UnitOfWork(AppDbContext _dbx)
        {
            this.dbx = _dbx;
            Category = new CategoryRepository(dbx);
            CoverType = new CoverTypeRepository(dbx);
            Product = new ProductRepository(dbx);
            Company = new CompanyRepository(dbx);
            ShoppingCart = new ShoppingCartRepository(dbx);
            ApplicationUser = new ApplicationUserRepository(dbx);
            OrderHeader = new OrderHeaderRepository(dbx);
            OrderDetail = new OrderDetailRepository(dbx);
        }

        public void Save()
        {
            dbx.SaveChanges();
        }
    }
}
