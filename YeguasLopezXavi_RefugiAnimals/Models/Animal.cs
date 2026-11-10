using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;

[Index(nameof(XipID), IsUnique = true)] 
public class Animal
{
    [Required]
    public int AnimalID { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Nom { get; set; }
    
    [Required]
    public Especie Especie { get; set; }
    
    [Required]
    public bool EsFemella { get; set; }
    
    [Required]
    public DateTime DataNaixement { get; set; }

    [Required] 
    public DateTime DataEntrada { get; set; }
    
    [Range(0.1, 100)]
    public double Pes { get; set; }
    
    public bool EstaAdoptat { get; set; }
    
    public DateTime? DataAdopcio { get; set; }

    [Precision(8, 2)] 
    public decimal QuotaAdopcio { get; set; }
    
    [Required] 
    public int XipID { get; set; }
    
    public virtual Xip Xip { get; set; }
    
    public virtual Adoptant? Adoptant { get; set; }

    public override string ToString()
    {
        return $"Animal {{AnimalID = {AnimalID}, Nom = {Nom}, Especie = {Especie}, EsFemella = {EsFemella}, DataNaixement = {DataNaixement}, DataEntrada = {DataEntrada}, Pes = {Pes}, EstaAdoptat = {EstaAdoptat}, DataAdopcio = {DataAdopcio}, QuotaAdopcio = {QuotaAdopcio}, XipID = {XipID}}}";
    }
    
    public Animal()
    {
        DataEntrada = DateTime.Today;
        EstaAdoptat = false;
        DataAdopcio = null;
        QuotaAdopcio = 0;
    }
}