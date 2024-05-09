using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplicationMVC.Code;

//Třída použitá jako služba pro běh nějaké funkcionality (definované v metodě ExecuteAsync) na pozadí
public class MujBackgroundService : BackgroundService
{
    //Spustí se při startu webové aplikace, měly by být zde inicializační úkoly, které mají být hotovi před ExecuteAsync
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Zacinam z BW ze start");
        return base.StartAsync(cancellationToken);
    }

    //Spustí se jednou při startu webové aplikace po metodě StartAsync
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //Skoro nekonečná smyčka
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Píši z BW");
            await Task.Delay(500); 
        }
                
        Console.WriteLine("Končím z BW");
    }

    // Pokud ukončím webovou aplikaci korektně (například ctrl+c v terminálu), tak se mi zavolá metoda StopAsync
    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Končím z BW ze stop");
        return base.StopAsync(cancellationToken);
    }
}