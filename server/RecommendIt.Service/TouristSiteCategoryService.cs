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
    public class TouristSiteCategoryService : ITouristSiteCategoryService
    {
        private readonly ITouristSiteCategoryRepository _touristSiteCategoryRepository;

        public TouristSiteCategoryService(ITouristSiteCategoryRepository touristSiteCategoryRepository)
        {
            _touristSiteCategoryRepository = touristSiteCategoryRepository;
        }

        public async Task AddTouristSiteCategoryAsync(ITouristSiteCategoryModel touristSiteCategoryModel)
        {
            touristSiteCategoryModel.CreatedBy = GetUserId();   
            touristSiteCategoryModel.UpdatedBy = GetUserId();
            await _touristSiteCategoryRepository.AddTouristSiteCategoryAsync(touristSiteCategoryModel);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
