namespace VemQueCabe.Application.Extensions;

/// <summary>
/// Provides strongly-typed cache key generators for various entities in the application,
/// such as User, Driver, Passenger, RideRequest, and Ride. Each nested static class
/// contains methods to generate cache keys for specific use cases, ensuring consistency
/// and reducing the risk of key collisions.
/// </summary>
public static class CacheKeys
{
    public static class User
    {
        public static string ById(int id) => $"user:{id}";
    }
    
    public static class Driver
    {
        public static string ById(int id) => $"driver:{id}";
        public static string List() => "driver:list";
        public static string Available() => "driver:list:available";
    }
    
    public static class Passenger
    {
        public static string ById(int id) => $"passenger:{id}";
        public static string List() => "passenger:list";
    }
    
    public static class RideRequest
    {
        public static string ById(int id) => $"riderequest:{id}";
        public static string List() => "riderequest:list";
        public static string ActiveByPassengerId(int id) => $"riderequest:list:active:passenger:{id}";
        public static string ActiveByStatus(string status) => $"riderequest:list:active:status:{status.ToLower()}";
    }
    
    public static class Ride
    {
        public static string ById(int id) => $"ride:{id}";
        public static string List() => "ride:list";
        public static string ActiveByDriverId(int id) => $"ride:list:active:driver:{id}";
    }
}