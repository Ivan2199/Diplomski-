using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<List<ICategoryModel>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }
        public async Task<ICategoryModel> GetCategoryAsync(Guid id)
        {
            return await _categoryRepository.GetCategoryAsync(id);
        }
        public async Task AddCategoryAsync(ICategoryModel categoryModel)
        {
            categoryModel.CreatedBy = GetUserId();
            categoryModel.UpdatedBy = GetUserId();
            await _categoryRepository.AddCategoryAsync(categoryModel);
        }
        public async Task UpdateCategoryAsync(Guid id, ICategoryModel categoryData)
        {
            categoryData.UpdatedBy = GetUserId();
            await _categoryRepository.UpdateCategoryAsync(id, categoryData);
        }
        public async Task<ICategoryModel> GetCategoryByFsqIdAsync(string id)
        {
            return await _categoryRepository.GetCategoryByFsqIdAsync(id);
        }
        public async Task DeleteCategoryAsync(Guid id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
