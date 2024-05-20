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
using GeoTagMap.WebApi.RestViewModels.View;
using GeoTagMap.Models.Common;
using GeoTagMap.WebApi.RestViewModels.Rest;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Common;

namespace GeoTagMap.WebApi.Controllers
{
    [RoutePrefix("api/performer")]
    [Authorize]
    public class PerformerController : ApiController
    {
        private readonly IPerformerService _performerService;

        public PerformerController(IPerformerService performerService)
        {
            _performerService = performerService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> GetAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string orderBy = "PerformanceDate",
            string sortOrder = "ASC",
            string name = "",
            string bandOrMusician = "",
            DateTime? performanceDate = null,
            string searchKeyword = ""
            )
        {
            Paging paging = new Paging(pageNumber, pageSize);
            Sorting sort = new Sorting(orderBy, sortOrder);
            PerformerFiltering filtering = new PerformerFiltering(name, bandOrMusician, performanceDate, searchKeyword);

            List<PerformerView> performerViews = new List<PerformerView>();
            try
            {
                var performersPagingInfo = await _performerService.GetAllPerformersAsync(paging, sort, filtering);
                if (performersPagingInfo.List.Count() == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                foreach (var performer in performersPagingInfo.List)
                {
                    performerViews.Add(MapPerformerView(performer));
                }
                PagingInfo<PerformerView> performerViewsPagingInfo = new PagingInfo<PerformerView>()
                {
                    List = performerViews,
                    RRP = performersPagingInfo.RRP,
                    PageNumber = performersPagingInfo.PageNumber,
                    TotalSize = performersPagingInfo.TotalSize,
                };

                return Request.CreateResponse(HttpStatusCode.OK, performerViewsPagingInfo);
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
                var performer = await _performerService.GetPerformerAsync(id);
                if (performer is null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No user with that Id");
                }
                PerformerView performerView = MapPerformerView(performer);
                return Request.CreateResponse(HttpStatusCode.OK, performerView);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] PerformerRest performerRest)
        {
            try
            {
                if (performerRest is null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }
                IPerformerModel performer = MapPerformer(performerRest);
                await _performerService.AddPerformerAsync(performer);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] PerformerRest performerRest)
        {
            try
            {
                if (performerRest == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }
                IPerformerModel performer = MapPerformer(performerRest);
                await _performerService.UpdatePerformerAsync(id, performer);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<HttpResponseMessage> DeletePerformerAsync(Guid id)
        {
            try
            {
                if (await _performerService.GetPerformerAsync(id) == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No performer with that id was found");
                }

                await _performerService.DeletePerformerAsync(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Performer has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private IPerformerModel MapPerformer(PerformerRest performer)
        {
            return new PerformerModel
            {
                Id = Guid.NewGuid(),
                Name = performer.Name,
                Image = performer.Image,
                BandOrMusician = performer.BandOrMusician,
                NumOfUpcomingEvents = performer.NumOfUpcomingEvents,
                PerformanceDate = performer.PerformanceDate,
                DateIsConfirmed = performer.DateIsConfirmed,

                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                IsActive = true
            };
        }

        private PerformerView MapPerformerView(IPerformerModel performer)
        {
            return new PerformerView
            {
                Id = performer.Id,
                Name = performer.Name,
                Image = performer.Image,
                BandOrMusician = performer.BandOrMusician,
                NumOfUpcomingEvents = performer.NumOfUpcomingEvents,
                PerformanceDate = performer.PerformanceDate,
                DateIsConfirmed = performer.DateIsConfirmed,
                EventPerformers = MapEventPerformerViews(performer.EventPerformers),
            };
        }

        private List<EventPerformerView> MapEventPerformerViews(List<IEventPerformerModel> eventPerformers)
        {
            if(eventPerformers == null)
            {
                return null;
            }
            List<EventPerformerView> eventPerformerViews = new List<EventPerformerView>();
            foreach (var eventPerformer in eventPerformers)
            {
                eventPerformerViews.Add(MapEventPerformerView(eventPerformer));
            }
            return eventPerformerViews;
        }

        private EventPerformerView MapEventPerformerView(IEventPerformerModel eventPerformer)
        {
            return new EventPerformerView
            {
                Id = eventPerformer.Id,
                EventId = eventPerformer.EventId,
                PerformerId = eventPerformer.PerformerId,
            };
        }
    }
}