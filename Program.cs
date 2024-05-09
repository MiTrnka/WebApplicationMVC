using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApplicationMVC.Code;

//Vytvo�� builder aplikace, kter� pou�iji pro jej� nakonfigurov�n�
var builder = WebApplication.CreateBuilder(args);

//Registrace slu�by b��c� paraleln� s hlavn� aplikac�
builder.Services.AddHostedService<MujBackgroundService>();

//Instance MojeSluzba bude vytvo�ena a� v okam�iku, kdy bude po�adov�na.
//Singleton(1 pro celou app), Scoped(1 pro scope/http pozadavek), Transient (1 pro kazdou instanci)
builder.Services.AddSingleton<IMojeSluzbaSingleton, MojeSluzba>();
builder.Services.AddScoped<IMojeSluzbaScoped, MojeSluzba>();
builder.Services.AddTransient<IMojeSluzbaTransient, MojeSluzba>();
/* Alternativa viz n�e by vytvo�ila instanci MojeSluzba hned
builder.Services.AddSingleton<IMojeSluzba>(new MojeSluzba());
*/

//Zaregistrovani moji tridy vytvorene pro validaci uzivatelsk�ho form�tu parametru, v tomto pripade ip adresy
builder.Services.AddRouting(options => {
    options.ConstraintMap.Add("ip", typeof(IpRouteConstraint));
});

//Zaregistruje sluzbu, ktera bude schopna dle urcitych pravidel najit v projektu spravne View ke spravnemu controlleru a dalsi veci
builder.Services.AddControllersWithViews();
var app = builder.Build();

//Nastaven� jednotliv�ch rout pro minimalistick�ho routov�n�
app.MapGet("/R1", ([FromQuery] string? s) => "Routa /R1 s nepovinnym stringovym query parametrem (/R1?s=hodnota): " + s);
app.MapGet("/R2/{p:int:range(2,8)?}", (int? p) => "Routa /R2 s nepovinnym int (s range 2 a� 8) parametrem (/R2/4): " + p.ToString());
app.MapGet("/IP/{ip:ip?}", (string? ip) => $"Zadal jsi ip adresu: {ip}"); //validuje parametr dle vlastn� t��dy IpRouteConstraint
app.MapGet("/redirect/{adresa}", (string adresa) => Results.Redirect($"https://{adresa}"));

//Nastaven� MVC routov�n�, prohled� projekt a najde v�echny t��dy kontroler�, kter� jsou um�st�n� v jak�mkoli adres��i (nejen v Controllers) a vytvo�� pro n� odpov�daj�c� routy 
app.MapControllers();

/*Ziskani instance z DI kontejneru pomoci tovarny, reflektuje Singleton/ScopedTransient
Tento ��dek k�du z�sk�v� instanci IServiceScopeFactory z glob�ln�ho poskytovatele slu�eb, kter� je sou��st� aplikace (app). 
IServiceScopeFactory je slu�ba, kter� umo��uje vytv��en� nov�ch DI scope, co� jsou izolovan� kontejnery slu�eb.
Metoda GetRequiredService<T> vyhled� slu�bu dan�ho typu (IServiceScopeFactory v tomto p��pad�) a vr�t� ji. */
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

/* Tato metoda vytv��� nov� DI scope. Scope je kontext, ve kter�m jsou slu�by z�sk�v�ny a jejich �ivotn� cyklus je spravov�n.
Tak�e pokud byla slu�ba registrovan� jako scoped, tak vol�n� metody GetRequiredService na stejn�m scope vr�t� stejnou instanci, u Transient jinou... */
using (var scope = scopeFactory.CreateScope())
{
    var s1 = scope.ServiceProvider.GetRequiredService<IMojeSluzbaScoped>();
    var s2 = scope.ServiceProvider.GetRequiredService<IMojeSluzbaScoped>();
    Console.WriteLine(s1.ZiskejZpravu());
    Console.WriteLine(s2.ZiskejZpravu());
    Console.WriteLine("Jsou instance MojeSluzba identicke?: "+(s1 == s2).ToString());
}

// Registrace meho middleware, kter� registruji bez pouziti nepovinn�ho parametru prefix
app.UseMiddleware<MujLogovaciMiddleware>();

// Registrace meho middleware, kter� registruji s nepovinn�m parametrem prefix
app.UseMiddleware<MujLogovaciMiddleware>("Logovaci prefix: ");

/* Aby mi fungovalo na��t�n� do HttpClient i str�nek s k�dov�n�m windows-1250, jako m� nap��klad idnes.
- p�idat nuget bal��ek System.Text.Encoding.CodePages
- spustit v powershellu dotnet add package System.Text.Encoding.CodePages --version 5.0.0
- Zaregistrovat provider viz ��dek n�e */
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

//Spust� ji� kompletn� nakonfigurovanou webovou aplikaci
app.Run();
