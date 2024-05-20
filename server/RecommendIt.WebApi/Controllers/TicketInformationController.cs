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
    [RoutePrefix("api/ticketinformation")]
    [Authorize]
    public class TicketInformationController : ApiController
    {
        private readonly ITicketInformationService _ticketInformationService;
        public TicketInformationController(ITicketInformationService ticketInformationService)
        {
            _ticketInformationService = ticketInformationService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync()
        {
            List<TicketInformationView> ticketInformationViews = new List<TicketInformationView>();
            try
            {
                var ticketInformations = await _ticketInformationService.GetTicketInformationsAsync();
                if (ticketInformations.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var ticket in ticketInformations)
                {
                    ticketInformationViews.Add(MapTicketInformationView(ticket));
                }

                return Request.CreateResponse(HttpStatusCode.OK, ticketInformationViews);
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
                var ticketInformation = await _ticketInformationService.GetTicketInformationAsync(id);
                if (ticketInformation is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                TicketInformationView ticketInformationView = MapTicketInformationView(ticketInformation);
                return Request.CreateResponse(HttpStatusCode.OK, ticketInformationView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] TicketInformationRest ticketInformationRest)
        {
            try
            {
                if (ticketInformationRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                ITicketInformationModel ticketInformation = MapTicketInformation(ticketInformationRest);
                await _ticketInformationService.AddTicketInformationAsync(ticketInformation);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] TicketInformationRest ticketInformationRest)
        {
            try
            {
                if (ticketInformationRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                ITicketInformationModel ticketInformation = MapTicketInformation(ticketInformationRest);
                await _ticketInformationService.UpdateTicketInformationAsync(id, ticketInformation);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private ITicketInformationModel MapTicketInformation(TicketInformationRest ticketInformationRest)
        {
            return new TicketInformationModel
            {
                Id = Guid.NewGuid(),
                Price = ticketInformationRest.Price,
                PriceCurrency = ticketInformationRest.PriceCurrency,
                Seller = ticketInformationRest.Seller,
                Url = ticketInformationRest.Url,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
        }

        private TicketInformationView MapTicketInformationView(ITicketInformationModel ticketInformation)
        {
            return new TicketInformationView
            {
                Id = ticketInformation.Id,
                Price = ticketInformation.Price,
                PriceCurrency = ticketInformation.PriceCurrency,
                Seller = ticketInformation.Seller,
                Url = ticketInformation.Url,
                Event = MapEventView(ticketInformation.Event),
            };
        }

        private EventView MapEventView(IEventModel eventModel)
        {
            if(eventModel == null)
            {
                return null;
            }
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
    }
}