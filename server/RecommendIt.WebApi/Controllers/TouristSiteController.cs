using GeoTagMap.Models;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GeoTagMap.Service;
using GeoTagMap.Models.Common;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Common;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/touristsite")]
    [Authorize]
    public class TouristSiteController : ApiController
    {
        private readonly ITouristSiteService _touristSiteService;

        public TouristSiteController(ITouristSiteService touristSiteService)
        {
            _touristSiteService = touristSiteService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Popularity",
            string sortOrder = "ASC",
            string name = "",
            double? popularity = null,
            double? rating = null,
            string searchKeyword = ""
            )
        {

            Paging paging = new Paging(pageNumber, pageSize);
            Sorting sort = new Sorting(orderBy, sortOrder);
            TouristSiteFiltering filtering = new TouristSiteFiltering(name, popularity, rating, searchKeyword);

            List<TouristSiteView> touristSiteViews = new List<TouristSiteView>();
            try
            {
                var sitesPagingInfo = await _touristSiteService.GetAllSitesAsync(paging, sort, filtering);
                if (sitesPagingInfo.List.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var site in sitesPagingInfo.List)
                {
                    touristSiteViews.Add(MapTouristSiteView(site));
                }
                PagingInfo<TouristSiteView> siteViewsPagingInfo = new PagingInfo<TouristSiteView>()
                {
                    List = touristSiteViews,
                    RRP = sitesPagingInfo.RRP,
                    PageNumber = sitesPagingInfo.PageNumber,
                    TotalSize = sitesPagingInfo.TotalSize,
                };

                return Request.CreateResponse(HttpStatusCode.OK, siteViewsPagingInfo);
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
                var site = await _touristSiteService.GetTouristSiteAsync(id);
                if (site is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                TouristSiteView touristSiteView = MapTouristSiteView(site);
                return Request.CreateResponse(HttpStatusCode.OK, touristSiteView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync([FromBody] TouristSiteRest touristSiteRest)
        {
            try
            {
                if (touristSiteRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                ITouristSitesModel touristSite = MapTouristSite(touristSiteRest);
                await _touristSiteService.AddTouristSiteAsync(touristSite);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] TouristSiteRest touristSiteRest)
        {
            try
            {
                if (touristSiteRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                ITouristSitesModel touristSite = MapTouristSite(touristSiteRest);
                await _touristSiteService.UpdateTouristSiteAsync(id, touristSite);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteTouristSiteAsync(Guid id)
        {
            try
            {
                if (await _touristSiteService.GetTouristSiteAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No tourist site with that id was found");
                }

                await _touristSiteService.DeleteTouristSiteAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Tourist site has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private ITouristSitesModel MapTouristSite(TouristSiteRest touristSiteRest)
        {
            return new TouristSitesModel
            {
                Id = Guid.NewGuid(),
                Name = touristSiteRest.Name,
                Link = touristSiteRest.Link,
                LocationId = touristSiteRest.LocationId,
                Popularity = touristSiteRest.Popularity,
                Rating = touristSiteRest.Rating,
                Description = touristSiteRest.Description,
                WebsiteUrl = touristSiteRest.WebsiteUrl,
                Email = touristSiteRest.Email,
                HoursOpen = touristSiteRest.HoursOpen,
                FacebookId = touristSiteRest.FacebookId,
                Instagram = touristSiteRest.Instagram,
                Twitter = touristSiteRest.Twitter,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true,
            };
        }

        private TouristSiteView MapTouristSiteView(ITouristSitesModel touristSite)
        {
            return new TouristSiteView
            {
                Id = touristSite.Id,
                Name = touristSite.Name,
                Link = touristSite.Link,
                Popularity = touristSite.Popularity,
                Rating = touristSite.Rating,
                Description = touristSite.Description,
                WebsiteUrl = touristSite.WebsiteUrl,
                Email = touristSite.Email,
                HoursOpen = touristSite.HoursOpen,
                FacebookId = touristSite.FacebookId,
                Instagram = touristSite.Instagram,
                Twitter = touristSite.Twitter,

                Location = MapLocationView(touristSite.Location),
                Comments = MapCommentViews(touristSite.Comments),
                SiteCategories = MapTouristSiteCategoryViews(touristSite.SiteCategories),
                Photos = MapPhotoViews(touristSite.Photos),
            };
        }

        private LocationView MapLocationView(ILocationModel location)
        {
            if(location == null)
            {
                return null;
            }
            return new LocationView
            {
                Id = location.Id,
                Country = location.Country,
                City = location.City,
                Address = location.Address,
                NameOfPlace = location.NameOfPlace,
                Village = location.Village,
            };
        }

        private List<CommentView> MapCommentViews(List<ICommentModel> comments)
        {
            if(comments == null)
            {
                return null;
            }
            List<CommentView> commentViews = new List<CommentView>();
            foreach (var comment in comments)
            {
                commentViews.Add(MapCommentView(comment));
            }
            return commentViews;
        }
        private CommentView MapCommentView(ICommentModel comment)
        {
            return new CommentView
            {
                Id = comment.Id,
                Text = comment.Text,
                CreatedBy = comment.CreatedBy,
                DateCreated = comment.DateCreated,
                DateUpdated = comment.DateUpdated,
            };
        }

        private List<TouristSiteCategoryView> MapTouristSiteCategoryViews(List<ITouristSiteCategoryModel> siteCategories)
        {
            if(siteCategories == null)
            {
                return null;
            }
            List<TouristSiteCategoryView> touristSiteCategoryViews = new List<TouristSiteCategoryView>();
            foreach (var siteCategory in siteCategories)
            {
                touristSiteCategoryViews.Add(MapTouristSiteCategoryView(siteCategory));
            }
            return touristSiteCategoryViews;
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

        private List<PhotoView> MapPhotoViews(List<IPhotoModel> photos)
        {
            if (photos == null)
            {
                return null;
            }
            List<PhotoView> photoViews = new List<PhotoView>();
            foreach (var photo in photos)
            {
                photoViews.Add(MapPhotoView(photo));
            }
            return photoViews;
        }
        private PhotoView MapPhotoView(IPhotoModel photo)
        {
            return new PhotoView
            {
                Id = photo.Id,
                ImagePrefix = photo.ImagePrefix,
                ImageSuffix = photo.ImageSuffix,
                DateCreated = photo.DateCreated,
                DateUpdated = photo.DateUpdated,
                CreatedBy = photo.CreatedBy,
            };
        }
    }
}