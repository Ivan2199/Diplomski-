using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GeoTagMap.Models
{
    public class PerformerModel : IPerformerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string BandOrMusician { get; set; }
        public int? NumOfUpcomingEvents { get; set; }
        public DateTime? PerformanceDate { get; set; }
        public bool? DateIsConfirmed { get; set; }
        public string JambaseIdentifier { get; set; }


        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public List<IEventPerformerModel> EventPerformers { get; set; }

    }
}
