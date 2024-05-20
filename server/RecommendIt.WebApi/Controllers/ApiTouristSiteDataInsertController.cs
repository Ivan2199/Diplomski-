using RestSharp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using GeoTagMap.WebApi.Models;
using System.Linq;
using GeoTagMap.Models.Common;
using GeoTagMap.Service.Common;
using GeoTagMap.Models;
using System.Collections.Generic;
using System.Transactions;
using System.Globalization;
using System.Configuration;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/touristsites")]
    [Authorize]
    public class ApiTouristSiteDataInsertController : ApiController
    {
        private static readonly string ForSquareApiKey = ConfigurationManager.AppSettings["ForsquareApiKey"];
        private readonly ITouristSiteService _touristSiteService;
        private readonly ILocationService _locationService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly ICategoryService _categoryService;
        private readonly ITouristSiteCategoryService _touristSiteCategoryService;
        private readonly IPhotoService _photoService;
        private static readonly Random random = new Random();


        public ApiTouristSiteDataInsertController(
            ITouristSiteService touristSiteService,
            ILocationService locationService,
            IGeoLocationService geoLocationService,
            ICategoryService categoryService,
            ITouristSiteCategoryService touristSiteCategoryService,
            IPhotoService photoService)
        {
            _touristSiteService = touristSiteService;
            _locationService = locationService;
            _geoLocationService = geoLocationService;
            _categoryService = categoryService;
            _touristSiteCategoryService = touristSiteCategoryService;
            _photoService = photoService;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetRandomPlaces()
        {
            int numberOfLocations = 50;
            int radius = 100000;
            string fields = "fsq_id,description,name,geocodes,photos,location,categories,link,rating,popularity,price,tips,email,website,hours,social_media";
            try
            {
                List<(string Latitude, string Longitude)> randomCoordinates = GenerateRandomCoordinates(numberOfLocations);

                foreach (var (lat, lon) in randomCoordinates)
                {
                    var apiUrl = $"https://api.foursquare.com/v3/places/search?ll={lat},{lon}&radius={radius}&fields={fields}&limit=50";
                    var client = new RestClient(apiUrl);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("accept", "application/json");
                    request.AddHeader("Authorization", ForSquareApiKey);

                    var response = await client.ExecuteTaskAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                        {
                            var resultObject = JsonConvert.DeserializeObject<VenueResponse>(response.Content);

                            foreach (var venue in resultObject.Results)
                            {
                                await ProcessTouristSite(venue);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Response content type is not JSON");
                        }
                    }
                    else
                    {
                        return Request.CreateResponse(response.StatusCode, response.Content);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, "Random places fetched and processed successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        private List<(string Latitude, string Longitude)> GenerateRandomCoordinates(int numberOfCoordinates)
        {
            return Enumerable.Range(1, numberOfCoordinates)
                .Select(_ => (random.Next(-90, 90).ToString(), random.Next(-180, 180).ToString()))
                .ToList();
        }


        private async Task ProcessTouristSite(Venue venue)
        {
            ITouristSitesModel existingTouristSite = await _touristSiteService.GetPerformerByOpenTripMapIdAsync(venue.Fsq_Id);
            if (existingTouristSite == null)
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    string fullCountryName = GetCountryFullName(venue.Location.Country);
                    ILocationModel location = new LocationModel
                    {
                        Id = Guid.NewGuid(),
                        NameOfPlace = string.IsNullOrEmpty(venue.Name) ? null : venue.Name,
                        Country = string.IsNullOrEmpty(venue.Location.Country) ? null : fullCountryName,
                        Village = string.IsNullOrEmpty(venue.Location.Region) ? null : venue.Location.Region,
                        City = string.IsNullOrEmpty(venue.Location.Locality) ? null : venue.Location.Locality,
                        Address = string.IsNullOrEmpty(venue.Location.Formatted_Address) ? null : venue.Location.Formatted_Address,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        IsActive = true,
                    };

                    IGeoLocationModel geoLocation = new GeoLocation
                    {
                        Id = Guid.NewGuid(),
                        Longitude = venue.Geocodes.Main.Longitude,
                        Latitude = venue.Geocodes.Main.Latitude,
                        LocationId = location.Id,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        IsActive = true
                    };

                    ITouristSitesModel touristSite = new TouristSitesModel
                    {
                        Id = Guid.NewGuid(),
                        Name = string.IsNullOrEmpty(venue.Name) ? null : venue.Name,
                        Link = venue.Link == null ? null : venue.Link,
                        LocationId = location.Id,
                        DateCreated = DateTime.Now,
                        DateUpdated = DateTime.Now,
                        IsActive = true,
                        Fsq_Id = venue.Fsq_Id ?? null,
                        Popularity = venue.Popularity,
                        Rating = venue.Rating,
                        Description = string.IsNullOrEmpty(venue.Description) ? null : venue.Description,
                        WebsiteUrl = string.IsNullOrEmpty(venue.Website) ? null : venue.Website,
                        Email = string.IsNullOrEmpty(venue.Email) ? null : venue.Email,
                        HoursOpen = venue.Hours?.Display ?? null,
                        FacebookId = venue.Social_Media?.Facebook_Id ?? null,
                        Instagram = venue.Social_Media?.Instagram ?? null,
                        Twitter = venue.Social_Media?.Twitter ?? null
                    };

                    await _locationService.AddLocationAsync(location);
                    await _geoLocationService.AddGeoLocationAsync(geoLocation);
                    await _touristSiteService.AddTouristSiteAsync(touristSite);

                    if (venue.Photos != null)
                    {
                        foreach (var currentPhoto in venue.Photos)
                        {
                            IPhotoModel photo = new PhotoModel
                            {
                                Id = Guid.NewGuid(),
                                ImagePrefix = currentPhoto.Prefix,
                                ImageSuffix = currentPhoto.Suffix,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                TouristSiteId = touristSite.Id,
                                IsActive = true
                            };
                            await _photoService.AddPhotoAsync(photo);
                        }
                    }
                    foreach (var categoryData in venue.Categories)
                    {
                        ICategoryModel existingCategory = await _categoryService.GetCategoryByFsqIdAsync(categoryData.Id.ToString());

                        ICategoryModel category;

                        if (existingCategory == null)
                        {
                            category = new CategoryModel
                            {
                                Id = Guid.NewGuid(),
                                Type = categoryData.Name ?? null,
                                Icon = (categoryData.Icon.Prefix + categoryData.Icon.Suffix) ?? null,
                                Fsq_CategoryId = categoryData.Id.ToString() ?? null,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now,
                                IsActive = true
                            };

                            await _categoryService.AddCategoryAsync(category);
                        }
                        else
                        {
                            category = existingCategory;
                        }
                        ITouristSiteCategoryModel touristSiteCategory = new TouristSiteCategoryModel
                        {
                            Id = Guid.NewGuid(),
                            CategoryId = category.Id,
                            TouristSiteId = touristSite.Id,
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                            IsActive = true
                        };

                        await _touristSiteCategoryService.AddTouristSiteCategoryAsync(touristSiteCategory);
                    }

                    scope.Complete();
                }
            }
        }

        private static string GetCountryFullName(string iso2CountryCode)
        {
            try
            {
                RegionInfo region = new RegionInfo(iso2CountryCode);
                return region.EnglishName;
            }
            catch (ArgumentException)
            {
                return "Invalid ISO2 country code";
            }
        }

    }
}
