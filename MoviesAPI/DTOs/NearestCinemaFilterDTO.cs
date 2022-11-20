using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class NearestCinemaFilterDTO
    {
        [Range(-90,90)]
        public double Latitude { get; set; }
        [Range(-180, 180)]
        public double Longitude { get; set; }
        private double distanceInKm = 10;
        private int maxDistanceInKm = 20;
        public int DistanceInKm
        {
            get
            {
                return (int)distanceInKm;
            }
            set
            {
                distanceInKm = (value>maxDistanceInKm)?maxDistanceInKm:value;
            }
        }


    }
}
