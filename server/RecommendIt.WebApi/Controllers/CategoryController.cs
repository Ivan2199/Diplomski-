using GeoTagMap.Models;
using GeoTagMap.Service;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GeoTagMap.Models.Common;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/category")]
    [Authorize]
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<CategoryView> categoryViews = new List<CategoryView>();
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                if (categories.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var category in categories)
                {
                    categoryViews.Add(MapCatgoryView(category));
                }
                return Request.CreateResponse(HttpStatusCode.OK, categoryViews);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryAsync(id);
                if (category is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                CategoryView categoryView = MapCatgoryView(category);
                return Request.CreateResponse(HttpStatusCode.OK, categoryView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync([FromBody] CategoryRest categoryRest)
        {
            try
            {
                if (categoryRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                ICategoryModel category = MapCategory(categoryRest);
                await _categoryService.AddCategoryAsync(category);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] CategoryRest categoryRest)
        {
            try
            {
                if (categoryRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                ICategoryModel category = MapCategory(categoryRest);
                await _categoryService.UpdateCategoryAsync(id, category);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin,")]
        public async Task<HttpResponseMessage> DeleteCategoryAsync(Guid id)
        {
            try
            {
                if (await _categoryService.GetCategoryAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No category with that id was found");
                }

                await _categoryService.DeleteCategoryAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Category has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private ICategoryModel MapCategory(CategoryRest categoryRest)
        {
            return new CategoryModel
            {
                Id = Guid.NewGuid(),
                Type = categoryRest.Type,
                Icon = categoryRest.Icon,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
        }
        private CategoryView MapCatgoryView(ICategoryModel category)
        {
            return new CategoryView
            {
                Id = category.Id,
                Type = category.Type,
                Icon = category.Icon,
                SiteCategories = MapTouristSiteCategoryViews(category.SiteCategories),
            };
        }

        private List<TouristSiteCategoryView> MapTouristSiteCategoryViews(List<ITouristSiteCategoryModel> siteCategories)
        {
            List<TouristSiteCategoryView> siteCategoryViews = new List<TouristSiteCategoryView>();
            foreach (var siteCategory in siteCategories)
            {
                siteCategoryViews.Add(MapTouristSiteCategoryView(siteCategory));
            }
            return siteCategoryViews;
        }

        private TouristSiteCategoryView MapTouristSiteCategoryView(ITouristSiteCategoryModel siteCategory)
        {
            return new TouristSiteCategoryView
            {
                Id = siteCategory.Id,
                CategoryId = siteCategory.CategoryId,
                TouristSiteId = siteCategory.TouristSiteId,
            };
        }
    }
}