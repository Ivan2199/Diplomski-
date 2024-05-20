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
using GeoTagMap.Models.Common;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using Intercom.Core;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Xml.Linq;
using GeoTagMap.Common;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/location")]
    [Authorize]
    public class LocationController : ApiController
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Country",
            string sortOrder = "ASC",
            string country = "",
            string city = "",
            string village = "",
            string address = "",
            string nameOfPlace = "",
            string searchKeyword = ""
            )
        {
            Paging paging = new Paging(pageNumber, pageSize);
            Sorting sort = new Sorting(orderBy, sortOrder);
            LocationFiltering filtering = new LocationFiltering(country, city, village, address, nameOfPlace, searchKeyword);

            List<LocationView> locationViews = new List<LocationView>();
            try
            {
                var locationsPagingInfo = await _locationService.GetAllLocationsAsync(paging, sort, filtering);
                if (locationsPagingInfo.List.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var location in locationsPagingInfo.List)
                {
                    locationViews.Add(MapLocationView(location));
                }

                PagingInfo<LocationView> locationViewsPagingInfo = new PagingInfo<LocationView>()
                {
                    List = locationViews,
                    RRP = locationsPagingInfo.RRP,
                    PageNumber = locationsPagingInfo.PageNumber,
                    TotalSize = locationsPagingInfo.TotalSize,
                };

                return Request.CreateResponse(HttpStatusCode.OK, locationViewsPagingInfo);
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
                var location = await _locationService.GetLocationAsync(id);
                if (location is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                LocationView locationView = MapLocationView(location);
                return Request.CreateResponse(HttpStatusCode.OK, locationView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] LocationRest locationRest)
        {
            try
            {
                if (locationRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                ILocationModel location = MapLocation(locationRest);
                await _locationService.AddLocationAsync(location);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] LocationRest locationRest)
        {
            try
            {
                if (locationRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                ILocationModel location = MapLocation(locationRest);
                await _locationService.UpdateLocationAsync(id, location);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private ILocationModel MapLocation(LocationRest locationRest)
        {
            return new LocationModel
            {
                Id = Guid.NewGuid(),
                Country = locationRest.Country,
                City = locationRest.City,
                Village = locationRest.Village,
                Address = locationRest.Address,
                NameOfPlace = locationRest.NameOfPlace,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
        }

        private LocationView MapLocationView(ILocationModel location)
        {
            return new LocationView
            {
                Id = location.Id,
                Country = location.Country,
                City = location.City,
                Village = location.Village,
                Address = location.Address,
                NameOfPlace = location.NameOfPlace,
                GeoLocations = MapGeoLocationViews(location.GeoLocations),
                Events = MapEventViews(location.Events),
                Sites = MapTouristSiteViews(location.Sites),
                Stories = MapStoryViews(location.Stories),
            };
        }

        private List<GeoLocationView> MapGeoLocationViews(List<IGeoLocationModel> geoLocations)
        {
            if(geoLocations == null)
            {
                return null;
            }
            List<GeoLocationView> geoLocationViews = new List<GeoLocationView>();
            foreach (var geoLocation in geoLocations)
            {
                geoLocationViews.Add(MapGeoLocationView(geoLocation));
            }
            return geoLocationViews;
        }

        private GeoLocationView MapGeoLocationView(IGeoLocationModel geoLocation)
        {
            return new GeoLocationView
            {
                Id = geoLocation.Id,
                Latitude = geoLocation.Latitude,
                Longitude = geoLocation.Longitude,
            };
        }

        private List<EventView> MapEventViews(List<IEventModel> events)
        {
            if(events == null)
            {
                return null;
            }
            List<EventView> eventViews = new List<EventView>();
            foreach (var eventModel in events)
            {
                eventViews.Add(MapEventView(eventModel));
            }
            return eventViews;
        }

        private EventView MapEventView(IEventModel eventModel)
        {
            return new EventView
            {
                Id = eventModel.Id,
                Name = eventModel.Name,
                Url = eventModel.Url,
                EventStatus = eventModel.EventStatus,
                Image = eventModel.Image,
                StartDate = eventModel.StartDate,
                EndDate = eventModel.EndDate,
                IsAccessibleForFree = eventModel.IsAccessibleForFree,
                Type = eventModel.Type,
            };
        }

        private List<TouristSiteView> MapTouristSiteViews(List<ITouristSitesModel> sites)
        {
            if(sites == null)
            {
                return null;
            }
            List<TouristSiteView> touristSiteViews = new List<TouristSiteView>();
            foreach (var site in sites)
            {
                touristSiteViews.Add(MapTouristSiteView(site));
            }
            return touristSiteViews;
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
                Twitter = touristSite.Twitter
            };
        }

        private List<StoryView> MapStoryViews(List<IStoryModel> stories)
        {
            if(stories == null)
            {
                return null;
            }
            List<StoryView> storyViews = new List<StoryView>();
            foreach (var story in stories)
            {
                storyViews.Add(MapStoryView(story));
            }
            return storyViews;
        }
        private StoryView MapStoryView(IStoryModel story)
        {
            return new StoryView
            {
                Id = story.Id,
                Text = story.Text,
                DateTime = story.DateTime,
            };
        }

    }
}