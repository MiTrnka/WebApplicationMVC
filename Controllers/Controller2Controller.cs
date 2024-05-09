using Microsoft.AspNetCore.Mvc;
using WebApplicationMVC.Code;
using WebApplicationMVC.Models;

namespace WebApplicationMVC.Controllers
{
    [Route("Controller2")]
    public class Controller2Controller : Controller
    {
        public IMojeSluzbaSingleton mojeSluzbaSingleton1,mojeSluzbaSingleton2;
        public IMojeSluzbaScoped mojeSluzbaScoped1, mojeSluzbaScoped2;
        public IMojeSluzbaTransient mojeSluzbaTransient1, mojeSluzbaTransient2;
        
        //Konstruktor si nechava pomoci DI vstriknout 6 instanci MojeSluzba (2 Singleton, 2 Scoped a 2 Transient) z DI kontejneru
        public Controller2Controller(
            IMojeSluzbaSingleton mojeSluzbaSingleton1, IMojeSluzbaSingleton mojeSluzbaSingleton2, 
            IMojeSluzbaScoped mojeSluzbaScoped1, IMojeSluzbaScoped mojeSluzbaScoped2, 
            IMojeSluzbaTransient mojeSluzbaTransient1, IMojeSluzbaTransient mojeSluzbaTransient2)
        {
            this.mojeSluzbaSingleton1 = mojeSluzbaSingleton1;
            this.mojeSluzbaSingleton2 = mojeSluzbaSingleton2;
            this.mojeSluzbaScoped1 = mojeSluzbaScoped1;
            this.mojeSluzbaScoped2 = mojeSluzbaScoped2;
            this.mojeSluzbaTransient1 = mojeSluzbaTransient1;
            this.mojeSluzbaTransient2 = mojeSluzbaTransient2;
        }

        [Route("Akce1")]
        public IActionResult Akce1()
        {
            Osoba osoba = new Osoba();
            osoba.Jmeno = "NEVYPLNĚNO";
            osoba.Vek = -1;
            return View(osoba);
        }
        [HttpPost("Akce1")]//pokud je požadavek http, tak má přednost před Route
        public IActionResult Akce1(Osoba osoba)
        {
            osoba.Jmeno += " zestarl o rok";
            osoba.Vek += 1;
            // Aktualizuje ModelState, aby se hodnota Jmeno ve formuláři zobrazila správně
            ModelState.Remove("Jmeno");
            // Aktualizuje ModelState, aby se hodnota Vek ve formuláři zobrazila správně
            ModelState.Remove("Vek");
            ViewBag.hodnota = "Hodnota z ViewBag"; //Krome hodnot v modelu mohu poslat jeste data ve ViewBag/ViewModel
            return View(osoba); //View("NepovinnyNazevView",NepovinnaInstanceModelu);
        }

        //ignoruje diky "/" routovani u controlleru a jde z rootu bez použití routy u controlleru
        //Ukazka pouziti injektovanych Singleton sluzeb
        [Route("/MojeSluzbaSingleton")] 
        public string Akce2()
        {
            return "Singleton: obě instance budou mít po celou dobu života aplikace stejnou hodnotu \r\n 1. injektovaná instance MojeSluzba: " + mojeSluzbaSingleton1.ZiskejZpravu() + "\r\n 2. injektovaná instance MojeSluzba: " + mojeSluzbaSingleton2.ZiskejZpravu();
        }
        //Ukazka pouziti injektovanych Singleton sluzeb
        [Route("/MojeSluzbaScoped")]
        public string Akce3()
        {
            return "Scoped: obě instance budou mít po dobu jednoho http požadavku stejnou hodnotu \r\n 1. injektovaná instance MojeSluzba: " + mojeSluzbaScoped1.ZiskejZpravu() + "\r\n 2. injektovaná instance MojeSluzba: " + mojeSluzbaScoped2.ZiskejZpravu();
        }
        //Ukazka pouziti injektovanych Singleton sluzeb
        [Route("/MojeSluzbaTransient")]
        public string Akce4()
        {
            return "Transient: obě instance budou mít velmi pravděpodobně různé hodnoty \r\n 1. injektovaná instance MojeSluzba: " + mojeSluzbaTransient1.ZiskejZpravu() + "\r\n 2. injektovaná instance MojeSluzba: " + mojeSluzbaTransient2.ZiskejZpravu();
        }
    }
}
