using GeoTagMap.Models;
using GeoTagMap.Service.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GeoTagMap.Models.Common;
using System.Transactions;
using System.Configuration;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/datainsert")]
    [Authorize]
    public class ApiEventDataInsertController : ApiController
    {
        private static readonly string JambaseApiKey = ConfigurationManager.AppSettings["JambaseApiKey"];
        private readonly IEventService _eventService;
        private readonly ILocationService _locationService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly IPerformerService _performerService;
        private readonly ITicketInformationService _ticketInformationService;
        private readonly IEventPerformerService _eventPerformerService;

        public ApiEventDataInsertController(IEventService eventService, ILocationService locationService, IGeoLocationService geoLocationService, IPerformerService performerService, ITicketInformationService ticketInformationService, IEventPerformerService eventPerformerService)
        {
            _eventService = eventService;
            _locationService = locationService;
            _geoLocationService = geoLocationService;
            _performerService = performerService;
            _ticketInformationService = ticketInformationService;
            _eventPerformerService = eventPerformerService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("jambase")]
        public async Task<IHttpActionResult> GetJambaseEventsAsync()
        {
            try
            {
                var currentPage = 1;
                var totalEvents = new List<(EventModel, TicketInformationModel, List<PerformerModel>, LocationModel, GeoLocation)>();

                while (true) 
                {
                    var jambaseApiUrl = $"https://www.jambase.com/jb-api/v1/events?apikey={JambaseApiKey}&page={currentPage}";

                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(jambaseApiUrl);
                        response.EnsureSuccessStatusCode();

                        var responseBody = await response.Content.ReadAsStringAsync();
                        var jambaseEvents = JsonConvert.DeserializeObject<JObject>(responseBody);

                        var events = ExtractEvents(jambaseEvents);
                        totalEvents.AddRange(events); 
                        if (!HasNextPage(jambaseEvents))
                            break;

                        currentPage++; 
                    }
                }


                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    foreach (var (eventModel, ticketInformation, performers, location, geoLocation) in totalEvents)
                    {
                        var existingEvent = await _eventService.GetEventByJambaseIdentifierAsync(eventModel.JambaseIdentifier);
                        if (existingEvent == null)
                        {
                            var existingLocation = await _locationService.GetLocationByJambaseIdentifierAsync(location.JambaseIdentifier);

                            if (existingLocation == null)
                            {
                                await _locationService.AddLocationAsync(location);
                            }

                            var addedLocation = existingLocation ?? location;

                            geoLocation.LocationId = addedLocation.Id;
                            eventModel.LocationId = addedLocation.Id;

                            await _geoLocationService.AddGeoLocationAsync(geoLocation);
                            await _ticketInformationService.AddTicketInformationAsync(ticketInformation);
                            await _eventService.AddEventAsync(eventModel);

                            foreach (var performer in performers)
                            {
                                IPerformerModel existingPerformer = await _performerService.GetPerformerByJambaseIdentifierAsync(performer.JambaseIdentifier);

                                IPerformerModel addedPerformer;

                                if (existingPerformer == null)
                                {
                                    await _performerService.AddPerformerAsync(performer);
                                    addedPerformer = performer;
                                }
                                else
                                {
                                    addedPerformer = existingPerformer;
                                }

                                var eventPerformer = new EventPerformerModel
                                {
                                    Id = Guid.NewGuid(),
                                    PerformerId = addedPerformer.Id,
                                    EventId = eventModel.Id,
                                    DateCreated = DateTime.Now,
                                    DateUpdated = DateTime.Now,
                                    IsActive = true
                                };

                                await _eventPerformerService.AddEventPerformerAsync(eventPerformer);
                            }
                        }
                    }

                    scope.Complete();
                }

                return Ok(totalEvents);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private bool HasNextPage(JObject jambaseEvents)
        {
            if (jambaseEvents.TryGetValue("pagination", out var paginationToken) && paginationToken is JObject pagination)
            {
                if (pagination.TryGetValue("totalPages", out var totalPagesToken) && totalPagesToken is JValue totalPagesValue)
                {
                    if (pagination.TryGetValue("page", out var currentPageToken) && currentPageToken is JValue currentPageValue)
                    {
                        var currentPage = currentPageValue.Value<int>();

                        return currentPage < 10;
                    }
                }
            }

            return false;
        }





        private List<(EventModel, TicketInformationModel, List<PerformerModel>, LocationModel, GeoLocation)> ExtractEvents(JObject jambaseEvents)
        {
            var eventsList = new List<(EventModel, TicketInformationModel, List<PerformerModel>, LocationModel, GeoLocation)>();

            if (jambaseEvents.TryGetValue("events", out var eventsToken) && eventsToken is JArray eventsArray)
            {
                foreach (var eventToken in eventsArray)
                {
                    var eventData = eventToken.ToString();
                    var (eventModel, ticketInformation, performers, location, geoLocation) = MapEventDataToEventModel(eventData);
                    eventsList.Add((eventModel, ticketInformation, performers, location, geoLocation));
                }
            }

            return eventsList;
        }

        private (EventModel, TicketInformationModel, List<PerformerModel>, LocationModel, GeoLocation) MapEventDataToEventModel(string eventData)
        {
            var jEvent = JsonConvert.DeserializeObject<JObject>(eventData);

            var locationData = jEvent.SelectToken("$.location");
            var location = new LocationModel
            {
                Id = Guid.NewGuid(),
                Country = locationData?.SelectToken("address.addressCountry.name")?.ToString() ?? null,
                City = locationData?.SelectToken("address.addressLocality")?.ToString() ?? null,
                Village = locationData?.SelectToken("address.x-streetAddress2")?.ToString() ?? null,
                Address = locationData?.SelectToken("address.streetAddress")?.ToString() ?? null,
                NameOfPlace = locationData?.SelectToken("name")?.ToString() ?? null,
                JambaseIdentifier = locationData?.SelectToken("identifier")?.ToString() ?? null,
                DateCreated = DateTime.Now, 
                DateUpdated = DateTime.Now,
                IsActive = true,
            };

            var geoCoordinatesData = jEvent.SelectToken("$.location.geo");
            var geoLocation = new GeoLocation
            {
                Id = Guid.NewGuid(),
                LocationId = location.Id,
                Latitude = (float)(geoCoordinatesData?.SelectToken("latitude")?.ToObject<double>() ?? 0.0),
                Longitude = (float)(geoCoordinatesData?.SelectToken("longitude")?.ToObject<double>() ?? 0.0),
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true,
            };


            var performerTokens = jEvent.SelectTokens("$.performer[*]");
            var performers = new List<PerformerModel>();

            foreach (var performerData in performerTokens)
            {
                var performer = new PerformerModel
                {
                    Id = Guid.NewGuid(),
                    Name = performerData?["name"]?.ToString() ?? null,
                    Image = performerData?["image"]?.ToString() ?? null,
                    BandOrMusician = performerData?["x-bandOrMusician"]?.ToString() ?? null,
                    NumOfUpcomingEvents = performerData?["x-numUpcomingEvents"]?.ToObject<int>() ?? null,
                    PerformanceDate = performerData?["x-performanceDate"]?.ToObject<DateTime>() ?? null,
                    DateIsConfirmed = performerData?["x-dateIsConfirmed"]?.ToObject<bool?>(),
                    JambaseIdentifier = performerData?["identifier"]?.ToString() ?? null,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    IsActive = true,
                };
                performers.Add(performer);
            }


            var ticketInformation = new TicketInformationModel
            {
                Id = Guid.NewGuid(),
                Price = jEvent.SelectToken("offers[0].priceSpecification.price")?.ToObject<decimal>() ?? null,
                PriceCurrency = jEvent.SelectToken("offers[0].priceSpecification.priceCurrency")?.ToString() ?? null,
                Seller = jEvent.SelectToken("offers[0].seller.name")?.ToString() ?? null,
                Url = jEvent.SelectToken("offers[0].url")?.ToString() ?? null,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true,
            };

            var eventModel = new EventModel
            {
                Id = Guid.NewGuid(),
                Name = jEvent.GetValue("name")?.ToString() ?? null,
                Url = jEvent.GetValue("url")?.ToString() ?? null,
                IsActive = true,
                Image = jEvent.GetValue("image")?.ToString() ?? null,
                EventStatus = jEvent.GetValue("eventStatus")?.ToString() ?? null,
                StartDate = jEvent.GetValue("startDate")?.ToObject<DateTime>() ?? null,
                EndDate = jEvent.GetValue("endDate")?.ToObject<DateTime>() ?? null,
                IsAccessibleForFree = jEvent.GetValue("isAccessibleForFree")?.ToObject<bool>() ?? false,
                Type = jEvent.GetValue("@type")?.ToString() ?? null,
                DateCreated = DateTime.Now, 
                DateUpdated = DateTime.Now,
                TicketInformationId = ticketInformation.Id,
                LocationId = location.Id,
                JambaseIdentifier = jEvent.GetValue("identifier")?.ToString() ?? null
            };

            return (eventModel, ticketInformation, performers, location, geoLocation); 
        }
    }
}