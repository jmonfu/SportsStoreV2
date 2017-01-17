using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsStore2.API.Models;

namespace SportsStore2.API.Repository
{
    public interface IBrandsRepository
    {
        Task<IActionResult> GetBrands();
        Task<IActionResult> GetBrandById(int id);
        void AddBrand(Brand brand);
        IActionResult UpdateBrand(Brand brand);
        IActionResult DeleteBrand(long id);
    }
}