using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore2.API.Models;

namespace SportsStore2.API.Services
{
    public interface IBrandsService
    {
        Task<List<Brand>> GetBrands();
        Task<Brand> GetBrandById(int id);
        Task<bool> AddBrand(Brand brand);
        bool UpdateBrand(Brand brand);
        void DeleteBrand(Brand brand);
    }
}