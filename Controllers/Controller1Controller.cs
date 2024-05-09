using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMVC.Controllers;

[Route("")]
public class Controller1Controller : Controller
{
    [Route("")] //Uvodni stranka
    public IActionResult Akce1()
    {
        return View();
    }

    [Route("Akce2/{p?}")]
    public string Akce2(string? p)
    {
        return "Parametr z url " + p;
    }
    [Route("Akce3")]
    public string Akce3([FromQuery] string? p)
    {
        return "Parametr z query " + p;
    }

    //Zjisti asynchronne pocet znaku na zadane html strance, url je bez uvodniho https
    [Route("Async/{*url}")]
    public async Task<string> Akce4(string url)
    {
        if (url.Length < 3)
            return "Byla zadaná chybná url adresa";
        string pocet="";
        if (!url.StartsWith("http"))
            url = $"https://{url}";
        var validUri = new Uri(url);
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(validUri);
                if (response.IsSuccessStatusCode)
                {
                    //Zde je to samotné asynchronní volání, kvůli kterému je i celá akce async
                    var htmlContent = await response.Content.ReadAsStringAsync();
                    pocet = $"Počet znaků na stránce: {htmlContent.Length}";
                }
                else
                {
                    pocet = "Nepodařilo se načíst stránku.";
                }
            }
        }
        catch (Exception ex)
        {
            pocet = $"Pro stránku s url {validUri} došlo k chybě: {ex.Message}";
        }
        return pocet;

    }

}