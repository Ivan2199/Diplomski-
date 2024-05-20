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
    public class EventPerformerService : IEventPerformerService
    {
        private readonly IEventPerformerRepository _eventPerformerRepository;
        public EventPerformerService(IEventPerformerRepository eventPerformerRepository)
        {
            _eventPerformerRepository = eventPerformerRepository;
        }
        public async Task AddEventPerformerAsync(IEventPerformerModel eventPerformer)
        {
            eventPerformer.CreatedBy = GetUserId();
            eventPerformer.UpdatedBy = GetUserId();
            await _eventPerformerRepository.AddEventPerformerAsync(eventPerformer);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
