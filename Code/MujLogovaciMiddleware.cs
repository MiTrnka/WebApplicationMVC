using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace WebApplicationMVC.Code;

//Statická třída jen pro metody rozšíření
public static class Utils
{
    //Factory metoda pro vytvoření MujLogovaciMiddleware s prefixem
    public static IApplicationBuilder UseMujLogovaciMiddlewareSLogovacimPrefixem(this IApplicationBuilder builder, string prefix)
    {
        return builder.UseMiddleware<MujLogovaciMiddleware>(prefix);
    }

}

//Třída pro vlastní middleware, slouží pro logování s volitelným stringovým parametrem jako prefix logovací hlášky
public class MujLogovaciMiddleware
{
    private readonly RequestDelegate nextMiddleware;
    private readonly ILogger<MujLogovaciMiddleware> logger;
    private readonly string prefix;

    //Pomocí constructor dependenci získám instance RequestDelegate a ILogger, volitelnou hodnotu mnou vymyšleného parametru prefix si při registraci musím dodat sám.
    public MujLogovaciMiddleware(RequestDelegate nextMiddleware, ILogger<MujLogovaciMiddleware> logger, string prefix="")
    {
        this.nextMiddleware = nextMiddleware;
        this.logger = logger;
        this.prefix = prefix;
    }

    // Metoda se volá pro každý příchozí http požadavek
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation(prefix+$"Příchozí požadavek: {context.Request.Method} {context.Request.Path}");
        //Zavolá vykonání následujícího (dle pořadí registrace v Program.cs) middleware, mohu rozhodnout z nějakého důvodu, že ho nezavolám.
        //Když se požadavek vykoná a vrací se Response, tak se dokončují jednotlivé middleware logicky v opačném pořadí
        await nextMiddleware(context);
        logger.LogInformation(prefix + $"Odpověď: {context.Response.StatusCode}");
    }
}
