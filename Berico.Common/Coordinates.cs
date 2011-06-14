//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

namespace Berico.Common
{
    /// <summary>
    /// Represents a geo-spatial coordianate
    /// </summary>
    public class GeoCoordinate
    {
        /// <summary>
        /// Represents a specific corrdinate value (such as Longitude
        /// or Latitude) as Degrees, Minutes and Seconds.  It also 
        /// includes the cardinal direction.
        /// </summary>
        public class DMS
        {
            public int Degrees { get; set; }
            public int Minutes { get; set; }
            public int Seconds { get; set; }
            public char Direction { get; set; }

            /// <summary>
            /// Creates a new GeoCoordinate.DMS instance
            /// </summary>
            /// <param name="degrees">The degrees</param>
            /// <param name="minutes">The minutes</param>
            /// <param name="seconds">The seconds</param>
            /// <param name="direction">The cardinal direction</param>
            public DMS(int degrees, int minutes, int seconds, char direction)
            {
                this.Degrees = degrees;
                this.Minutes = minutes;
                this.Seconds = seconds;
                this.Direction = direction;
            }

            /// <summary>
            /// Returns the decimal representation for this DMS instance
            /// </summary>
            /// <returns>the decimal value for this DMS instance</returns>
            public double? ToDecimal()
            {
                // Convert the degrees, minutes and seconds to a decimal value
                double result = ((double)this.Degrees % 360) + ((double)this.Minutes / 60) + ((double)this.Seconds / 3600);

                // We need a negative value if we are South or West
                if (this.Direction == 'S' || this.Direction == 's' || this.Direction == 'W' || this.Direction == 'w')
                    result = result * -1;

                return result;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return String.Format("{0}°{1}’{2}”{3}", this.Degrees.ToString(), this.Minutes.ToString(), this.Seconds.ToString(), this.Direction);
            }

        }

        /// <summary>
        /// Creates a new instance of the GeoCoordinate class using
        /// the two provided DMS instances (which represent latitude
        /// and longitude)
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        public GeoCoordinate(DMS lat, DMS lng)
        {
            this.Latitude = lat;
            this.Longitude = lng;
        }

        /// <summary>
        /// Gets or sets the Latitude
        /// </summary>
        public DMS Latitude { get; set; }

        /// <summary>
        /// Gets or sets the Longitude
        /// </summary>
        public DMS Longitude { get; set; }

        /// <summary>
        /// Attempts to parse a latitude and longitude string into a 
        /// new GeoCoordinate instance
        /// </summary>
        /// <param name="input">The longitude and latitude string</param>
        /// <param name="output">A newly created GeoCoordinate instance</param>
        /// <returns>true if the parsing was succesful; otherwsie false</returns>
        public static bool TryParse(string input, out GeoCoordinate output)
        {

            // Validate input parameter
            if (string.IsNullOrEmpty(input))
            {
                output = null;
                return false;
            }

            // TODO: SUPPORT MGRS IN THE FUTURE
            
            // The regex below parses Longitude and Latitude values and supports
            // the following different formats:
            // - 38° 47’ 44” N 077° 36’ 50” W
            // - 38°47’44”N077°36’50”W
            // - 38°47’44”N,077°36’50”W
            // - 384744N 0773650W
            // - 38°47’44”N 077°36’50”W
            // - 38 47 44 N 77 36 50 W
            // - 38 47 44.23 N 77 36 50.77 W
            string longlatParseString = "^(?<lat_degrees>\\d{2})[°]?\\s*(?<lat_minutes>\\d{2})[’']?\\s*(?<lat_seconds>[\\d\\.]+)[”\\\"]?\\s*(?<lat_direction>[NnSs]{1})[\\s,_-]?(?<lng_degrees>\\d{2,3})[°]?\\s*(?<lng_minutes>\\d{2})[’']?\\s*(?<lng_seconds>[\\d\\.]+)[”\\\"]?\\s*(?<lng_direction>[EeWw]{1})$";
            Regex regex = new Regex(longlatParseString);

            // Check if the regex matches the provided value
            if (regex.IsMatch(input))
            {
                Match match = regex.Match(input);

                //TODO: VALIDATE THE GROUPS

                // Create a DMS instance for Latitude and Longitude
                DMS Latitude = new DMS(int.Parse(match.Groups["lat_degrees"].Value), int.Parse(match.Groups["lat_minutes"].Value), int.Parse(match.Groups["lat_seconds"].Value), (char)match.Groups["lat_direction"].Value[0]);
                DMS Longitude = new DMS(int.Parse(match.Groups["lng_degrees"].Value), int.Parse(match.Groups["lng_minutes"].Value), int.Parse(match.Groups["lng_seconds"].Value), (char)match.Groups["lng_direction"].Value[0]);

                output = new GeoCoordinate(Latitude, Longitude);
                return true;
            }
            else
            {
                // The regex does not match so the provided input
                // cannot be parsed as a longitude and latitude
                output = null;
                return false;
            }

        }

        public static bool IsValid(string value)
        {
            GeoCoordinate coordinate = null;

            if (GeoCoordinate.TryParse(value, out coordinate))
                return true;
            else
                return false;

        }

        /// <summary>
        /// Calculates the distance between two GeoCoordinate values.  This method
        /// uses the algorithm described at http://www.movable-type.co.uk/scripts/latlong.html.
        /// </summary>
        /// <param name="otherCoordinate">An instance of the GeoCoordinate class
        /// for comparison</param>
        /// <returns>the distance between this GeoCorrdinate instance and the 
        /// provided instance</returns>
        public double? GetDistanceTo(GeoCoordinate otherCoordinate)
        {
            // Ensure we have valid longitude and latitude values for the source
            if (this.Latitude == null || this.Longitude == null)
                return null;

            // Ensure we have valid longitude and latitude values for the target
            if (otherCoordinate.Latitude == null || otherCoordinate.Longitude == null)
                return null;

            double R = 6371;
            
            // Caclculate the latitude and longitude changes
            double deltaLat = (this.Latitude.ToDecimal().Value - otherCoordinate.Latitude.ToDecimal().Value).ToRad();
            double deltaLng = (this.Longitude.ToDecimal().Value - otherCoordinate.Longitude.ToDecimal().Value).ToRad();

            // Calculate the angle
            double angle = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(this.Latitude.ToDecimal().Value.ToRad()) * Math.Cos(otherCoordinate.Latitude.ToDecimal().Value.ToRad()) *
                Math.Sin(deltaLng / 2) * Math.Sin(deltaLng / 2);

            // Calculate the circumference
            double c = 2 * Math.Atan2(Math.Sqrt(angle), Math.Sqrt(1 - angle));
            
            return R * c;

        }
    }
}