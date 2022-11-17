using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private AppDbContext dbx;

        public OrderDetailRepository(AppDbContext _dbx) : base(_dbx)
        {
            this.dbx = _dbx;
        }

        public void Update(OrderDetail orderDetail)
        {
            dbx.OrderDetail.Update(orderDetail);
        }
    }
}
