using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.RestViewModels;
using GeoTagMap.Service;
using GeoTagMap.Service.Common;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GeoTagMap.WebApi.Controllers
{
    public class PhotoController : ApiController
    {
        private readonly IPhotoService _photoService;
        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<PhotoView> photoViews = new List<PhotoView>();
            try
            {
                var photos = await _photoService.GetPhotosAsync();
                if (photos.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var photo in photos)
                {
                    photoViews.Add(MapPhotoView(photo));
                }

                return Request.CreateResponse(HttpStatusCode.OK, photoViews);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                var photo = await _photoService.GetPhotoAsync(id);
                if (photo is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                PhotoView photoView = MapPhotoView(photo);

                return Request.CreateResponse(HttpStatusCode.OK, photoView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostAsync([FromBody] PhotoRest photoRest)
        {
            try
            {
                if (photoRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                IPhotoModel photo = MapPhoto(photoRest);
                await _photoService.AddPhotoAsync(photo);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpPut]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] PhotoRest photoRest)
        {
            try
            {
                if (photoRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No data has been entered");
                }
                IPhotoModel photo = MapPhoto(photoRest);
                await _photoService.UpdatePhotoAsync(id, photo);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadGateway, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin,")]
        public async Task<HttpResponseMessage> DeleteCommentAsync(Guid id)
        {
            try
            {
                if (await _photoService.GetPhotoAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No comment with that id was found");
                }

                await _photoService.DeletePhotoAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Comment has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        private IPhotoModel MapPhoto(PhotoRest photoRest)
        {
            return new PhotoModel
            {
                Id = Guid.NewGuid(),
                ImagePrefix = photoRest.ImagePrefix,
                ImageSuffix = photoRest.ImageSuffix,
                TouristSiteId = photoRest.TouristSiteId ?? null,
                StoryId = photoRest.StoryId ?? null,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
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

                TouristSite = MapTouristSiteView(photo.TouristSite),
                Story = MapStoryView(photo.Story),
                User = MapUserView(photo.User)
            };
        }

        private TouristSiteView MapTouristSiteView(ITouristSitesModel touristSite)
        {
            if (touristSite == null)
            {
                return null;
            }
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
            };
        }
        private StoryView MapStoryView(IStoryModel story)
        {
            if (story == null)
            {
                return null;
            }
            return new StoryView
            {
                Id = story.Id,
                Text = story.Text,
                DateTime = story.DateTime,
            };
        }
        private UserModelView MapUserView(IUserModel user)
        {
            return new UserModelView
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Image = user.Image,
                IsActive = user.IsActive,
            };
        }
    }
}