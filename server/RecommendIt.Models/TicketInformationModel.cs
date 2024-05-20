using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class TicketInformationModel : ITicketInformationModel 
    {
        public Guid Id { get; set; }
        public decimal? Price { get; set; }
        public string PriceCurrency { get; set; }   
        public string Seller { get; set; }
        public string Url { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public IEventModel Event { get; set; }
    }
}
