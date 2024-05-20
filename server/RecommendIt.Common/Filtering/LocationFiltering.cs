using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Common.Filtering
{
    public class LocationFiltering
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Village { get; set; }
        public string Address { get; set; }
        public string NameOfPlace { get; set; }
        public string SearchKeyword { get; set; }

        public LocationFiltering(string country, string city, string village, string address, string nameOfPlace, string searchKeyword)
        {
            Country = country;
            City = city;
            Village = village;
            Address = address;
            NameOfPlace = nameOfPlace;
            SearchKeyword = searchKeyword;
        }
    }
}
