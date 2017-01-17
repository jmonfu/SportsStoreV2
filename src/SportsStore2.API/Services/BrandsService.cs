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
        private readonly IBrandsRepository _brandsRepository;

        public BrandsService(IBrandsRepository brandsRepository)
        {
            _brandsRepository = brandsRepository;
        }
        public async Task<IActionResult> GetBrands()
        {
            return await _brandsRepository.GetBrands();
        }

        public async Task<IActionResult> GetBrandById(int id)
        {
            return await _brandsRepository.GetBrandById(id);
        }

        public void AddBrand(Brand brand)
        {
            _brandsRepository.AddBrand(brand);
        }

        public IActionResult UpdateBrand(Brand brand)
        {
            return _brandsRepository.UpdateBrand(brand);
        }

        public IActionResult DeleteBrand(long id)
        {
            return _brandsRepository.DeleteBrand(id);
        }
    }
}
