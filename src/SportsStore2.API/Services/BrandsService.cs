using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore2.API.Models;
using SportsStore2.API.Repository;

namespace SportsStore2.API.Services
{
    public class BrandsService : IBrandsService
    {
        private readonly IGenericRepository<Brand> _brandsGenericRepository;

        public BrandsService(IGenericRepository<Brand> brandsGenericRepository)
        {
            _brandsGenericRepository = brandsGenericRepository;
        }
        public async Task<List<Brand>> GetBrands()
        {
            return await _brandsGenericRepository.GetAll(null, "Image");
        }

        public async Task<Brand> GetBrandById(int id)
        {
            //return await _brandsRepository.GetBrandById(id);
            return await _brandsGenericRepository.Get<Brand>(m => m.Id == id, "Image");
        }

        public async Task<bool> AddBrand(Brand brand)
        {
            var existingBrand = await _brandsGenericRepository.Get<Brand>(m => m.Name == brand.Name, null);

            if (existingBrand != null) return false;

            _brandsGenericRepository.Add(brand);
            return true;
        }

        public bool UpdateBrand(Brand brand)
        {
            _brandsGenericRepository.Update(brand);
            return true;
        }

        public void DeleteBrand(Brand brand)
        {
            _brandsGenericRepository.Delete(brand);
        }
    }
}
