using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private AppDbContext dbx;

        public CompanyRepository(AppDbContext _dbx) : base(_dbx)
        {
            this.dbx = _dbx;
        }

        public void Update(Company company)
        {
            dbx.Companies.Update(company);
        }
    }
}
