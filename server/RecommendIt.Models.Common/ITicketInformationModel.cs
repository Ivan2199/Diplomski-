using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ITicketInformationModel
    {
        Guid Id { get; set; }
        decimal? Price { get; set; }
        string PriceCurrency { get; set; }
        string Seller { get; set; }
        string Url { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        IEventModel Event { get; set; }
    }
}
