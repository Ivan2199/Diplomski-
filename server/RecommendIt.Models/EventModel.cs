using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class EventModel : IEventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool? IsActive { get; set; }
        public string EventStatus { get; set; }
        public string Image { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAccessibleForFree { get; set; }
        public string Type { get; set; }
        public string JambaseIdentifier { get; set; }

        public Guid? TicketInformationId { get; set; }
        public Guid? LocationId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public ITicketInformationModel TicketInformation { get; set; }
        public List<IEventPerformerModel> EventPerformers { get; set; }
        public ILocationModel Location { get; set; }
        public List<ICommentModel> Comments { get; set; }
    }
}
