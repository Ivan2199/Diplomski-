using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoTagMap.WebApi.Models
{
    public class Address
    {
        public string AddressLine { get; set; }
        public string AdminDistrict { get; set; }
        public string CountryRegion { get; set; }
        public string FormattedAddress { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
    }

    public class Point
    {
        public string Type { get; set; }
        public double[] Coordinates { get; set; }
    }

    public class Resource
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public Point Point { get; set; }
    }

    public class BingLocationResponse
    {
        public Resource[] Resources { get; set; }
    }

    public class BingLocationInfo
    {
        public BingLocationResponse[] ResourceSets { get; set; }
    }
}