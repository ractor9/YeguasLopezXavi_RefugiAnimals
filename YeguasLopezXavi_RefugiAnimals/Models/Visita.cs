using System.ComponentModel.DataAnnotations;

namespace RefugiAnimals;

public class Visita
{
    [Required]
    public int VisitaID { get; set; }
    public DateTime Data { get; set; }
    
    public Motiu Motiu { get; set; }
    
    public int Cost { get; set; }
    
    public virtual Animal Animal { get; set; }
    
    public override string ToString()
    {
        return $"Visita {{Data = {Data}, Motiu = {Motiu}, Cost = {Cost}, Animal = {Animal.Nom}}}";
    }
}