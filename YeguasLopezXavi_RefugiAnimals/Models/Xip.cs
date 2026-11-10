using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;

[Index(nameof(Codi), IsUnique = true)] 
public class Xip
{
    [Required]
    public int XipID { get; set; }
    
    [Required]
    [MaxLength(15)]
    [MinLength(15)]
    public string Codi { get; set; }
    
    [Required]
    public DateTime DataImplantacio { get; set; }
    
    public virtual Animal Animal { get; set; }
    public override string ToString()
    {
        return $"Xip {{XipID = {XipID},Codi = {Codi}, DataImplantacio = {DataImplantacio}, Animal = {Animal}}}";
    }
    
    public Xip()
    {
        DataImplantacio = DateTime.Today;
    }
}