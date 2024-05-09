using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class Osoba
    {
        [Display(Name = "Jméno")]
        [Required(ErrorMessage = "Zadejte prosím jméno")]        
        public string Jmeno { get; set; } = "";

        [Display(Name = "Věk")]
        [Required(ErrorMessage = "Zadejte prosím věk")]
        [DisplayFormat(DataFormatString = "{0:D3}")]
        [Range(0, 130, ErrorMessage = "Zadejte prosím číslo od 0 do 130.")]
        public int Vek {  get; set; }
        public Lazy<DateTime> AktualniDatum { get; set; }
        
        public Osoba()
        {
            // Instance třídy Lazy se vytvoří hned, ale ten DateTime se vytvoří až tehdy, když se k té instanci Lazy přistoupí přes vlastnost Value.
            //V souboru Akce1.cshtml je vidět toto použití v cache @Model.AktualniDatum.Value
            AktualniDatum = new Lazy<DateTime>(()=>GetCurrentDateTime());
        }
        
        //Díky obalení DateTime třídou Lazy se tato metoda skutečně volá jen při prvním požadavku a pak vždy jednou po vypršení cache a pak zase ne
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}
