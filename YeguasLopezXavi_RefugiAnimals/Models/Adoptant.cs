using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;


[Index(nameof(DNI), IsUnique = true)] 
public class Adoptant
{
    [Required]
    public int AdoptantID { get; set; }
    
    [Required]
    [MaxLength(9)]
    public string DNI { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Nom { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Cognoms { get; set; }
    
    [Required]
    public bool TeJardi { get; set; }
    
    [Required]
    public DateTime DataRegistre { get; set; }
    
    public virtual ICollection<Animal> Animals { get; set; }
    
    public override string ToString()
    {
        return $"Professor {{AdoptantID = {AdoptantID},DNI = {DNI}, Nom = {Nom}, Cognoms = {Cognoms}, TeJardi = {TeJardi}, DataRegistre = {DataRegistre}}}";
    }

    public Adoptant()
    {
        DataRegistre = DateTime.Today;
        Animals = new HashSet<Animal>();
    }
}