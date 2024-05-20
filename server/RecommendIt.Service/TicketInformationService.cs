using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class TicketInformationService : ITicketInformationService
    {
        private readonly ITicketInformationRepository _ticketInformationRepository;
        public TicketInformationService(ITicketInformationRepository ticketInformationRepository)
        {
            _ticketInformationRepository = ticketInformationRepository;
        }
        public async Task<List<ITicketInformationModel>> GetTicketInformationsAsync()
        {
            return await _ticketInformationRepository.GetTicketInformationsAsync();
        }
        public async Task<ITicketInformationModel> GetTicketInformationAsync(Guid id)
        {
            return await _ticketInformationRepository.GetTicketInformationAsync(id);
        }
        public async Task AddTicketInformationAsync(ITicketInformationModel ticketInformation)
        {
            ticketInformation.CreatedBy = GetUserId();
            ticketInformation.UpdatedBy = GetUserId();
            await _ticketInformationRepository.AddTicketInformationAsync(ticketInformation);
        }
        public async Task UpdateTicketInformationAsync(Guid id, ITicketInformationModel ticketInformationData)
        {
            ticketInformationData.UpdatedBy = GetUserId();
            await _ticketInformationRepository.UpdateTicketInformationAsync(id, ticketInformationData); 
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
