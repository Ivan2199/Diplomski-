using GeoTagMap.Common;
using GeoTagMap.Common.Filtering;
using GeoTagMap.Common.Paging;
using GeoTagMap.Common.Sorting;
using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<PagingInfo<IEventModel>> GetAllEventsAsync(Paging paging, Sorting sort, EventFiltering filtering)
        {
            return await _eventRepository.GetAllEventsAsync(paging, sort, filtering);
        }

        public async Task<IEventModel> GetEventAsync(Guid id)
        {
            return await _eventRepository.GetEventAsync(id);
        }
        public async Task AddEventAsync(IEventModel eventModel)
        {
            eventModel.CreatedBy = GetUserId();
            eventModel.UpdatedBy = GetUserId();
            await _eventRepository.AddEventAsync(eventModel);
        }
        public async Task UpdateEventAsync(Guid id, IEventModel eventData)
        {
            eventData.UpdatedBy = GetUserId();
            await _eventRepository.UpdateEventAsync(id, eventData);
        }
        public async Task DeleteEventAsync(Guid id)
        {
            await _eventRepository.DeleteEventAsync(id);
        }
        public async Task<IEventModel> GetEventByJambaseIdentifierAsync(string jambaseIdentifier)
        {
            return await _eventRepository.GetEventByJambaseIdentifierAsync(jambaseIdentifier);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
