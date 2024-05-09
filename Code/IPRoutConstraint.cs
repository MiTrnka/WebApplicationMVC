namespace WebApplicationMVC.Code
{
    /// <summary>
    /// Třída pro validaci parametru, že je to formát IP adresy
    /// </summary>
    public class IpRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var value = values[routeKey] as string;
            return System.Net.IPAddress.TryParse(value, out _);
        }
    }
}
