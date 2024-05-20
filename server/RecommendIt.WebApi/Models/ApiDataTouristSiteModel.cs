using System;
using System.Collections.Generic;

namespace GeoTagMap.WebApi.Models
{
    public class VenueResponse
    {
        public List<Venue> Results { get; set; }
    }

    public class Venue
    {
        public string Fsq_Id { get; set; }
        public List<Category> Categories { get; set; }
        public Geocodes Geocodes { get; set; }
        public string Link { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public double Popularity { get; set; }
        public int Price { get; set; }
        public double Rating { get; set; }
        public Hours Hours { get; set; }
        public string Email { get; set; }
        public SocialMedia Social_Media { get; set; }
        public List<Tip> Tips { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public List<Photo> Photos { get; set; }
    }

    public class Photo
    {
        public string Id { get; set; }
        public string Prefix { get; set; } 
        public string Suffix { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Short_Name { get; set; }
        public string Plural_Name { get; set; }
        public Icon Icon { get; set; }
    }

    public class Icon
    {
        public string Prefix { get; set; }
        public string Suffix { get; set; }
    }

    public class Geocodes
    {
        public Main Main { get; set; }
        public Roof Roof { get; set; }
    }

    public class Main
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Roof
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Location
    {
        public string Address { get; set; }
        public string Census_Block { get; set; }
        public string Country { get; set; }
        public string Dma { get; set; }
        public string Formatted_Address { get; set; }
        public string Locality { get; set; }
        public string Po_Box { get; set; }
        public string Postcode { get; set; }
        public string Region { get; set; }
    }

    public class Hours
    {
        public string Display { get; set; }
        public bool Is_Local_Holiday { get; set; }
        public bool Open_Now { get; set; }
        public List<Regular> Regular { get; set; }
    }

    public class Regular
    {
        public string Close { get; set; }
        public int Day { get; set; }
        public string Open { get; set; }
    }

    public class SocialMedia
    {
        public string Facebook_Id { get; set; }
        public string Instagram { get; set; }
        public string Twitter { get; set; }

    }

    public class Tip
    {
        public string Created_At { get; set; }
        public string Text { get; set; }
    }
}
