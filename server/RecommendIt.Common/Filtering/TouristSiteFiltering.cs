using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Common.Filtering
{
    public class TouristSiteFiltering
    {
        public string Name { get; set; }
        public double? Popularity { get; set; }
        public double? Rating { get; set; }
        public string SearchKeyword { get; set; }

        public TouristSiteFiltering(string name, double? popularity, double? rating, string searchKeyword)
        {
            Name = name;
            Popularity = popularity;
            Rating = rating;
            SearchKeyword = searchKeyword;
        }
    }
}
