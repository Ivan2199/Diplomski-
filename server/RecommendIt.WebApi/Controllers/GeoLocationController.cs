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

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/geolocation")]
    [Authorize]
    public class GeoLocationController : ApiController
    {
        private readonly IGeoLocationService _geoLocationService;
        public GeoLocationController(IGeoLocationService geoLocationService)
        {
            _geoLocationService = geoLocationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<GeoLocationView> geoLocationViews = new List<GeoLocationView>();
            try
            {
                var geoLocations = await _geoLocationService.GetAllGeoLocationsAsync();
                if (geoLocations.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var geoLocation in geoLocations)
                {
                    geoLocationViews.Add(MapGeoLocationView(geoLocation));
                }

                return Request.CreateResponse(HttpStatusCode.OK, geoLocationViews);
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
                var geoLocation = await _geoLocationService.GetGeoLocationAsync(id);
                if (geoLocation is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                GeoLocationView geoLocationView = MapGeoLocationView(geoLocation);

                return Request.CreateResponse(HttpStatusCode.OK, geoLocationView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] GeoLocationRest geoLocationRest)
        {
            try
            {
                if (geoLocationRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                IGeoLocationModel geoLocation = MapGeoLocation(geoLocationRest);
                await _geoLocationService.AddGeoLocationAsync(geoLocation);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] GeoLocationRest geoLocationRest)
        {
            try
            {
                if (geoLocationRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                IGeoLocationModel geoLocation = MapGeoLocation(geoLocationRest);
                await _geoLocationService.UpdateGeoLocationAsync(id, geoLocation);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IGeoLocationModel MapGeoLocation(GeoLocationRest geoLocationRest)
        {
            return new GeoLocation
            {
                Id = Guid.NewGuid(),
                Latitude = geoLocationRest.Latitude,
                Longitude = geoLocationRest.Longitude,
                LocationId = geoLocationRest.LocationId,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
        }

        private GeoLocationView MapGeoLocationView(IGeoLocationModel geoLocation)
        {
            return new GeoLocationView
            {
                Id = geoLocation.Id,
                Latitude = geoLocation.Latitude,
                Longitude = geoLocation.Longitude,
                Location = MapLocationView(geoLocation.Location),
            };
        }
        private LocationView MapLocationView(ILocationModel location)
        {
            if(location == null) return null;

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
    }
}