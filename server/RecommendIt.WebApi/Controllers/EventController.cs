using GeoTagMap.RestViewModels;
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
using GeoTagMap.Models;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Intercom.Data;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.WebApi.RestViewModels.View;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using System.Xml.Linq;
using GeoTagMap.Common;

namespace GeoTagMap.WebApi.Controllers
{

    [RoutePrefix("api/event")]
    [Authorize]
    public class EventController : ApiController
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "Name",
            string sortOrder = "ASC",
            string name = "",
            string eventStatus = null,
            DateTime? startDate = null,
            string type = "",
            bool? isAccessibleForFree = null,
            string searchKeyword = ""

            )
        {
            Paging paging = new Paging(pageNumber, pageSize);
            Sorting sort = new Sorting(orderBy, sortOrder);
            EventFiltering filtering = new EventFiltering(name, eventStatus, startDate, type, isAccessibleForFree, searchKeyword);

            List<EventView> eventViews = new List<EventView>();
            try
            {
                var eventPagingInfo = await _eventService.GetAllEventsAsync(paging, sort, filtering);
                if (eventPagingInfo.List.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach(var currentEvent in eventPagingInfo.List)
                {
                    eventViews.Add(MapEventView(currentEvent));
                }

                PagingInfo<EventView> eventViewPagingInfo = new PagingInfo<EventView>()
                {
                    List = eventViews,
                    RRP = eventPagingInfo.RRP,
                    PageNumber = eventPagingInfo.PageNumber,
                    TotalSize = eventPagingInfo.TotalSize,
                };

                return Request.CreateResponse(HttpStatusCode.OK, eventViewPagingInfo);
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
                var eventModel = await _eventService.GetEventAsync(id);
                if (eventModel is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                EventView eventView = MapEventView(eventModel);

                return Request.CreateResponse(HttpStatusCode.OK, eventView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] EventRest eventRest)
        {
            try
            {
                if (eventRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                IEventModel eventModel = MapEvent(eventRest);
                await _eventService.AddEventAsync(eventModel);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] EventRest eventRest)
        {
            try
            {
                if (eventRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                IEventModel eventModel = MapEvent(eventRest);
                await _eventService.UpdateEventAsync(id, eventModel);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeleteEventAsync(Guid id)
        {
            try
            {
                if (await _eventService.GetEventAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No event with that id was found");
                }

                await _eventService.DeleteEventAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Event has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IEventModel MapEvent(EventRest eventRest)
        {
            return new EventModel
            {
                Id = Guid.NewGuid(),
                Name = eventRest.Name,
                Url = eventRest.Url,
                EventStatus = eventRest.EventStatus,
                Image = eventRest.Image,
                StartDate = eventRest.StartDate,
                EndDate = eventRest.EndDate,
                IsAccessibleForFree = eventRest.IsAccessibleForFree,
                Type = eventRest.Type,
                TicketInformationId = eventRest.TicketInformationId,
                LocationId = eventRest.LocationId,
                IsActive = true,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
            };
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

                TicketInformation = MapTicketInformationView(eventModel.TicketInformation),
                Comments = MapCommentsView(eventModel.Comments),
                EventPerformers = MapEventPerformersView(eventModel.EventPerformers),
                Location = MapLocationView(eventModel.Location)
            };
        }

        private TicketInformationView MapTicketInformationView(ITicketInformationModel ticketInformationModel)
        {
            if(ticketInformationModel == null)
            {
                return null;
            } 
            return new TicketInformationView
            {
                Id = ticketInformationModel.Id,
                Price = ticketInformationModel.Price,
                Seller = ticketInformationModel.Seller,
                PriceCurrency = ticketInformationModel.PriceCurrency,
                Url = ticketInformationModel.Url,
            };
        }
        private List<CommentView> MapCommentsView(List<ICommentModel> comments)
        {
            if(comments == null)
            {
                return null;
            }
            List<CommentView> commentsView = new List<CommentView>();
            foreach(var  comment in comments)
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
                CreatedBy = comment.CreatedBy,
                DateCreated = comment.DateCreated,
                DateUpdated = comment.DateUpdated,
            };
        }

        private List<EventPerformerView> MapEventPerformersView(List<IEventPerformerModel> eventPerformers)
        {
            if(eventPerformers == null)
            {
                return null;
            }
            List<EventPerformerView> eventPerformersView = new List<EventPerformerView>();
            foreach (var eventPerformer in eventPerformers)
            {
                eventPerformersView.Add(MapEventPerformerView(eventPerformer));
            }
            return eventPerformersView;
        }
        private EventPerformerView MapEventPerformerView(IEventPerformerModel eventPerformer)
        {
            return new EventPerformerView
            {
                Id = eventPerformer.Id,
                EventId = eventPerformer.Id,
                PerformerId = eventPerformer.Id,
            };
        }

        private LocationView MapLocationView(ILocationModel location)
        {
            if (location == null)
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
    }
}