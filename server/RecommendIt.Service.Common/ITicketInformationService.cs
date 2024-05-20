using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface ITicketInformationService
    {
        Task<List<ITicketInformationModel>> GetTicketInformationsAsync();
        Task<ITicketInformationModel> GetTicketInformationAsync(Guid id);
        Task AddTicketInformationAsync(ITicketInformationModel ticketInformation);
        Task UpdateTicketInformationAsync(Guid id, ITicketInformationModel ticketInformationData);
    }
}
