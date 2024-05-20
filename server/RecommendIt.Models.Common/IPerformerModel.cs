using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface IPerformerModel
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Image { get; set; }
        string BandOrMusician { get; set; }
        int? NumOfUpcomingEvents { get; set; }
        DateTime? PerformanceDate { get; set; }
        bool? DateIsConfirmed { get; set; }
        string JambaseIdentifier { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        List<IEventPerformerModel> EventPerformers { get; set; }
    }
}
