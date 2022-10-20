using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private AppDbContext dbx;

        public ProductRepository(AppDbContext _dbx) : base(_dbx)
        {
            this.dbx = _dbx;
        }

        public void Update(Product product)
        {
            dbx.Products.Update(product);
        }
    }
}
