using GeoTagMap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GeoTagMap.Service.Common;
using GeoTagMap.Service;
using Newtonsoft.Json;
using GeoTagMap.Models.Common;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;
using System.Transactions;
using GeoTagMap.WebApi.Models;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;
using GeoTagMap.RestViewModels;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/story")]
    [Authorize]
    public class StoryController : ApiController
    {
        private readonly IStoryService _storyService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly ILocationService _locationService;
        private readonly HttpClient _httpClient;

        public StoryController(IStoryService storyService, IGeoLocationService geoLocationService, ILocationService locationService)
        {
            _storyService = storyService;
            _httpClient = new HttpClient();
            _geoLocationService = geoLocationService;
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<StoryView> storiesView = new List<StoryView>();
            try
            {
                var stories = await _storyService.GetAllStoriesAsync();
                if (stories.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var story in stories)
                {
                    storiesView.Add(MapStoryViews(story));
                }

                return Request.CreateResponse(HttpStatusCode.OK, storiesView);
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
                var story = await _storyService.GetStoryAsync(id);
                if (story is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                StoryView storyView = MapStoryViews(story);
                return Request.CreateResponse(HttpStatusCode.OK, storyView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync(double longitude, double latitude, [FromBody] StoryRest storyRest)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (storyRest is null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                    }
                   

                    var locationInfo = await GetLocationInfo(latitude, longitude);

                    if (locationInfo == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Unable to retrieve location information");
                    }

                    ILocationModel location = await _locationService.GetLocationByAddressAsync(locationInfo.Address.FormattedAddress);
                    if (location == null || locationInfo.Address.FormattedAddress == "")
                    {
                        string country = locationInfo.Address.CountryRegion;
                        string city = locationInfo.Address.Locality;
                        string village = locationInfo.Address.AdminDistrict;
                        string address = locationInfo.Address.FormattedAddress;
                        string nameOfPlace = locationInfo.Name;

                        ILocationModel currentLocation = new LocationModel
                        {
                            Id = Guid.NewGuid(),
                            Country = country,
                            City = city,
                            Village = village,
                            Address = address,
                            NameOfPlace = nameOfPlace,
                            IsActive = true,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            JambaseIdentifier = null

                        };
                        await _locationService.AddLocationAsync(currentLocation);
                        location = currentLocation;
                    }

                    IGeoLocationModel geoLocation = new GeoLocation
                    {
                        Id = Guid.NewGuid(),
                        Latitude = latitude,
                        Longitude = longitude,
                        LocationId = location.Id,
                        IsActive = true,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                    };

                    await _geoLocationService.AddGeoLocationAsync(geoLocation);


                    storyRest.LocationId = location.Id;
                    IStoryModel story = MapStory(storyRest);

                    await _storyService.AddStoryAsync(story);

                    transactionScope.Complete();

                    return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] StoryRest storyRest)
        {
            try
            {
                if (storyRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                IStoryModel story = MapStory(storyRest);
                await _storyService.UpdateStoryAsync(id, story);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeleteStoryAsync(Guid id)
        {
            try
            {
                if (await _storyService.GetStoryAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No tourist site with that id was found");
                }

                await _storyService.DeleteStoryAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Tourist site has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IStoryModel MapStory(StoryRest storyRest)
        {
            return new StoryModel
            {
                Text = storyRest.Text,
                DateTime = storyRest.DateTime,
                LocationId = storyRest.LocationId,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true,
            };
        }
        private StoryView MapStoryViews(IStoryModel story)
        {
            return new StoryView
            {
                Id = story.Id,
                Text = story.Text,
                DateTime = story.DateTime,
                Location = MapLocationView(story.Location),
                Comment = MapCommentsView(story.Comment),
                User = MapUserView(story.User),
                Photos = MapPhotoViews(story.Photos),
            };
        }
        private LocationView MapLocationView(ILocationModel location)
        {
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
        private List<CommentView> MapCommentsView(List<ICommentModel> comments)
        {
            if(comments == null)
            {
                return null;
            }
            List<CommentView> commentsView = new List<CommentView>();
            foreach (CommentModel comment in comments)
            {
                commentsView.Add(MapCommentView(comment));
            }
            return commentsView;
        }
        private CommentView MapCommentView(ICommentModel comment)
        {
            return new CommentView
            {
                Id = comment.Id,
                Text = comment.Text,
                DateCreated = comment.DateCreated,
                DateUpdated = comment.DateUpdated,
                CreatedBy = comment.CreatedBy,
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
        private List<PhotoView> MapPhotoViews(List<IPhotoModel> photos)
        {
            if(photos == null)
            {
                return null;
            }
            List<PhotoView> photoViews = new List<PhotoView>();
            foreach(var photo in photos)
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

        private async Task<Resource> GetLocationInfo(double latitude, double longitude)
        {
            var bingMapsKey = "Ak7RjUuR0SWMgRxCj1DuS8JN9anU_dGfAxY_8DNjXlY0RH1w0h27XOSh3-2na9s-";
            string requestUri = $"http://dev.virtualearth.net/REST/v1/Locations/{latitude},{longitude}?o=json&key={bingMapsKey}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var locationInfo = JsonConvert.DeserializeObject<BingLocationInfo>(responseBody);

                var resource = locationInfo.ResourceSets.FirstOrDefault()?.Resources.FirstOrDefault();

                return resource;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}