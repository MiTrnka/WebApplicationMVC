using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApplicationMVC.Code;

//Vytvoøí builder aplikace, který použiji pro její nakonfigurování
var builder = WebApplication.CreateBuilder(args);

//Registrace služby bìžící paralelnì s hlavní aplikací
builder.Services.AddHostedService<MujBackgroundService>();

//Instance MojeSluzba bude vytvoøena až v okamžiku, kdy bude požadována.
//Singleton(1 pro celou app), Scoped(1 pro scope/http pozadavek), Transient (1 pro kazdou instanci)
builder.Services.AddSingleton<IMojeSluzbaSingleton, MojeSluzba>();
builder.Services.AddScoped<IMojeSluzbaScoped, MojeSluzba>();
builder.Services.AddTransient<IMojeSluzbaTransient, MojeSluzba>();
/* Alternativa viz níže by vytvoøila instanci MojeSluzba hned
builder.Services.AddSingleton<IMojeSluzba>(new MojeSluzba());
*/

//Zaregistrovani moji tridy vytvorene pro validaci uzivatelského formátu parametru, v tomto pripade ip adresy
builder.Services.AddRouting(options => {
    options.ConstraintMap.Add("ip", typeof(IpRouteConstraint));
});

//Zaregistruje sluzbu, ktera bude schopna dle urcitych pravidel najit v projektu spravne View ke spravnemu controlleru a dalsi veci
builder.Services.AddControllersWithViews();
var app = builder.Build();

//Nastavení jednotlivých rout pro minimalistického routování
app.MapGet("/R1", ([FromQuery] string? s) => "Routa /R1 s nepovinnym stringovym query parametrem (/R1?s=hodnota): " + s);
app.MapGet("/R2/{p:int:range(2,8)?}", (int? p) => "Routa /R2 s nepovinnym int (s range 2 až 8) parametrem (/R2/4): " + p.ToString());
app.MapGet("/IP/{ip:ip?}", (string? ip) => $"Zadal jsi ip adresu: {ip}"); //validuje parametr dle vlastní tøídy IpRouteConstraint
app.MapGet("/redirect/{adresa}", (string adresa) => Results.Redirect($"https://{adresa}"));

//Nastavení MVC routování, prohledá projekt a najde všechny tøídy kontrolerù, které jsou umístìné v jakémkoli adresáøi (nejen v Controllers) a vytvoøí pro nì odpovídající routy 
app.MapControllers();

/*Ziskani instance z DI kontejneru pomoci tovarny, reflektuje Singleton/ScopedTransient
Tento øádek kódu získává instanci IServiceScopeFactory z globálního poskytovatele služeb, který je souèástí aplikace (app). 
IServiceScopeFactory je služba, která umožòuje vytváøení nových DI scope, což jsou izolované kontejnery služeb.
Metoda GetRequiredService<T> vyhledá službu daného typu (IServiceScopeFactory v tomto pøípadì) a vrátí ji. */
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

/* Tato metoda vytváøí nový DI scope. Scope je kontext, ve kterém jsou služby získávány a jejich životní cyklus je spravován.
Takže pokud byla služba registrovaná jako scoped, tak volání metody GetRequiredService na stejném scope vrátí stejnou instanci, u Transient jinou... */
using (var scope = scopeFactory.CreateScope())
{
    var s1 = scope.ServiceProvider.GetRequiredService<IMojeSluzbaScoped>();
    var s2 = scope.ServiceProvider.GetRequiredService<IMojeSluzbaScoped>();
    Console.WriteLine(s1.ZiskejZpravu());
    Console.WriteLine(s2.ZiskejZpravu());
    Console.WriteLine("Jsou instance MojeSluzba identicke?: "+(s1 == s2).ToString());
}

// Registrace meho middleware, který registruji bez pouziti nepovinného parametru prefix
app.UseMiddleware<MujLogovaciMiddleware>();

// Registrace meho middleware, který registruji s nepovinným parametrem prefix
app.UseMiddleware<MujLogovaciMiddleware>("Logovaci prefix: ");

/* Aby mi fungovalo naèítání do HttpClient i stránek s kódováním windows-1250, jako má napøíklad idnes.
- pøidat nuget balíèek System.Text.Encoding.CodePages
- spustit v powershellu dotnet add package System.Text.Encoding.CodePages --version 5.0.0
- Zaregistrovat provider viz øádek níže */
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

//Spustí již kompletnì nakonfigurovanou webovou aplikaci
app.Run();
