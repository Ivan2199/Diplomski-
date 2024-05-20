using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface ICategoryRepository
    {
        Task<List<ICategoryModel>> GetAllCategoriesAsync();
        Task<ICategoryModel> GetCategoryAsync(Guid id);
        Task AddCategoryAsync(ICategoryModel categoryModel);
        Task UpdateCategoryAsync(Guid id, ICategoryModel categoryData);
        Task<ICategoryModel> GetCategoryByFsqIdAsync(string id);
        Task DeleteCategoryAsync(Guid id);
    }
}
