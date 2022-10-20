using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private AppDbContext dbx;

        public CoverTypeRepository(AppDbContext _dbx) : base(_dbx)
        {
            this.dbx = _dbx;
        }

        public void Update(CoverType coverType)
        {
            dbx.CoverTypes.Update(coverType);
        }
    }
}
