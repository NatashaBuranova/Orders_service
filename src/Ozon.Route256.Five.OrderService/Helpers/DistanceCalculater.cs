namespace Ozon.Route256.Five.OrderService.Helpers;

public static class DistanceCalculator
{
    private const double EarthRadiusKm = 6371;

    public static double CalculateDistance(Point point1, Point city2)
    {
        var dLat = DegreesToRadians(city2.Latitude - point1.Latitude);
        var dLon = DegreesToRadians(city2.Longitude - point1.Longitude);

        var lat1 = DegreesToRadians(point1.Latitude);
        var lat2 = DegreesToRadians(city2.Latitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}


