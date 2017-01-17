using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore2.API.Models;

namespace SportsStore2.API.Repository
{
    public class BrandsRepository : IBrandsRepository
    {
        private readonly SportsStore2Context _context;

        public BrandsRepository(SportsStore2Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands.Include(b => b.Image).ToListAsync();

            if (brands == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(brands); 
        }

        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _context.Brands.Include(b => b.Image).SingleOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return new NotFoundResult();
            }

            return new JsonResult(brand);
        }

        public void AddBrand(Brand brand)
        {
            if (!_context.Brands.Any(x => x.Name == brand.Name))
            {
                _context.Brands.AddRange(brand);
                _context.SaveChanges();
            }
        }

        public IActionResult UpdateBrand(Brand brand)
        {
            var updateBrand = _context.Brands.AsNoTracking().FirstOrDefault(x => x.Id == brand.Id);

            if (updateBrand == null)
                return new NotFoundResult();

            _context.Brands.Update(brand);
            _context.SaveChanges();
            return new NoContentResult();
        }

        public IActionResult DeleteBrand(long id)
        {
            //var brand = _context.Brands.AsNoTracking().FirstOrDefault(x => x.Id == id);
            var brand = _context.Brands.Find(id);

            if (brand == null)
                return new NotFoundResult();

            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return new NoContentResult();
        }
    }
}
