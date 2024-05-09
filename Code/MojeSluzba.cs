namespace WebApplicationMVC.Code;

//Níže uvedené interface pak používám v aplikaci k odlišení toho, jakým způsobem (Singleton, Scoped, Transient) zaregistrovanou službu jsem použil pro získání její instance.
public interface IMojeSluzbaSingleton
{
    string ZiskejZpravu();
}
public interface IMojeSluzbaScoped
{
    string ZiskejZpravu();
}
public interface IMojeSluzbaTransient
{
    string ZiskejZpravu();
}


// Ukazkova trida pouzita jako sluzba pro IoC/DI kontejner. Při vytvoření instance si vygeneruje náhodné číslo, které pak nabízí pomocí metody ZiskejZpravu()
public class MojeSluzba : IMojeSluzbaSingleton, IMojeSluzbaScoped, IMojeSluzbaTransient
{
    private string zprava;
    public MojeSluzba()
    {
        Random random = new Random();
        zprava = "Nahodne cislo: " + random.Next(0, 100);
    }
    public string ZiskejZpravu()
    {
        return zprava;
    }
}
