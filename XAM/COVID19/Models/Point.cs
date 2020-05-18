namespace Perimeter.Models
{
    public class Point
    {
        public Location Location { get; set; }
        public int Count { get; set; } = 1;
        public Xamarin.Forms.Color Heat { get; set; }

        public string Summary
        {
            get { return string.Format("{0} Lat:{1} Lon:{2}", Location.Id, Location.Latitude, Location.Longitude); }
            
        }
    }
}
