using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;


namespace SportsStore.Domain.Concrete
{
    public class EFProductRepository: IProductRepository
    {
        private readonly EFDbContext _context = new EFDbContext();

        public IQueryable<Product> Products
        {
            get { return _context.Products; }
        }

    }
}
