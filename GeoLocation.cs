using System;

class GeoLocation
{
    public static GeoCoord FindPointAtDistanceFrom(GeoCoord startPoint, double initialBearingRadians, double distanceKilometres)
    {
        const double radiusEarthKilometres = 6371.01;
        var distRatio = distanceKilometres / radiusEarthKilometres;
        var distRatioSine = Math.Sin(distRatio);
        var distRatioCosine = Math.Cos(distRatio);

        var startLatRad = DegreesToRadians(startPoint.Latitude);
        var startLonRad = DegreesToRadians(startPoint.Longitude);

        var startLatCos = Math.Cos(startLatRad);
        var startLatSin = Math.Sin(startLatRad);

        var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

        var endLonRads = startLonRad
            + Math.Atan2(
                Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                distRatioCosine - startLatSin * Math.Sin(endLatRads));

        return new GeoCoord
        {
            Latitude = RadiansToDegrees(endLatRads),
            Longitude = RadiansToDegrees(endLonRads)
        };
    }

    public struct GeoCoord
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public static double DegreeBearing(double lat1, double lon1,double lat2, double lon2)
    {
        var dLon = DegreesToRadians(lon2 - lon1);
        var dPhi = Math.Log(
            Math.Tan(DegreesToRadians(lat2) / 2 + Math.PI / 4) / Math.Tan(DegreesToRadians(lat1) / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI)
            dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
        return ToBearing(Math.Atan2(dLon, dPhi));
    }

    public static double BearingRelative(double x1, double y1, double heading, double x2, double y2)
    {
        double bearing_abs;
        double bearing_rel;
        bearing_abs = DegreeBearing(y1, x1, y2, x2);
        bearing_rel = bearing_abs - heading;
        if (bearing_rel < 0)
        {
            bearing_rel = bearing_rel + 360;
        }
        if (bearing_rel > 360)
        {
            bearing_rel = bearing_rel - 360;
        }
        return bearing_rel;
    }

    public static double ToBearing(double radians)
    {
        return (RadiansToDegrees(radians) + 360) % 360;
    }

    public static double DegreesToRadians(double degrees)
    {
        const double degToRadFactor = Math.PI / 180;
        return degrees * degToRadFactor;
    }

    public static double RadiansToDegrees(double radians)
    {
        const double radToDegFactor = 180 / Math.PI;
        return radians * radToDegFactor;
    }

    public static double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
        if ((lat1 == lat2) && (lon1 == lon2))
        {
            return 0;
        }
        else
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(DegreesToRadians(lat1)) * Math.Sin(DegreesToRadians(lat2)) + Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) * Math.Cos(DegreesToRadians(theta));
            dist = Math.Acos(dist);
            dist = RadiansToDegrees(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
    }

    public static Boolean IsDegreeDestAtLeftCartesian(double src, double dest)
    {
        Boolean ReturnValue;
        double back = DegreeBack(src);
        if (back > src)
        {
            ReturnValue = (dest > src) && (dest < back);
        }
        else
        {
            ReturnValue = ((dest > src) && (dest <= 360.0)) || ((dest >= 0.0) && (dest < back));
        }
        return ReturnValue;
    }

    public static double DegreeBack(double degreeOrg)
    {
        double ReturnValue;
        ReturnValue = degreeOrg + 180.0;
        while (ReturnValue > 360.0)
        {
            ReturnValue = ReturnValue - 360.0;
        }
        return ReturnValue;
    }

    public static double ValidateDegree(double degreeOrg)
    {
        double deg;
        deg = degreeOrg - (Math.Truncate(degreeOrg / 360.0) * 360.0);
        if (deg < 0.0)
        {
            deg = 360.0 - deg;
        }
        return deg;
    }
}

