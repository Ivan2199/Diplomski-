using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.RestViewModels.Rest
{
    public class TicketInformationRest
    {
        public decimal? Price { get; set; }
        public string PriceCurrency { get; set; }
        public string Seller { get; set; }
        public string Url { get; set; }
    }
}