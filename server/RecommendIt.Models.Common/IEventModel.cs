using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface IEventModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Url { get; set; }
        bool? IsActive { get; set; }
        string EventStatus { get; set; }
        string Image { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        bool? IsAccessibleForFree { get; set; }
        string Type { get; set; }
        string JambaseIdentifier { get; set; }

        Guid? TicketInformationId { get; set; }
        Guid? LocationId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }

        ITicketInformationModel TicketInformation { get; set; }
        List<IEventPerformerModel> EventPerformers { get; set; }
        ILocationModel Location { get; set; }
        List<ICommentModel> Comments { get; set; }
    }
}
