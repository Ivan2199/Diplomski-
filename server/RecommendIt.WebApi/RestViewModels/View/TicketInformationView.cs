using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.View
{
    public class TicketInformationView
    {
        public Guid Id { get; set; }
        public decimal? Price { get; set; }
        public string PriceCurrency { get; set; }
        public string Seller { get; set; }
        public string Url { get; set; }

        public EventView Event { get; set; }
    }
}