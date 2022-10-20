using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private AppDbContext dbx;

        public CategoryRepository(AppDbContext _dbx) : base(_dbx)
        {
            this.dbx = _dbx;
        }

        public void Update(Category category)
        {
            dbx.Categories.Update(category);
        }
    }
}
