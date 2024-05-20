namespace GeoTagMap.Common.Paging
{
    public class Paging
    {
        public int RRP { get; set; }
        public int PageNumber { get; set; }

        public Paging(int pagenumber, int rrp)
        { RRP = rrp; PageNumber = pagenumber; }
    }
}