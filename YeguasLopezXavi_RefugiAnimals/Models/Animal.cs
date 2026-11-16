using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;

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
    
    public virtual Xip Xip { get; set; }
    
    public virtual Adoptant? Adoptant { get; set; }
    
    public virtual ICollection<Voluntari> Voluntaris { get; set; }
    
    public virtual Animal Mare { get; set; }
    
    public virtual ICollection<Animal> Cries { get; set; }

    public override string ToString()
    {
        string texto = $"Animal {{AnimalID = {AnimalID}, Nom = {Nom}, Especie = {Especie}, EsFemella = {EsFemella}, DataNaixement = {DataNaixement}, DataEntrada = {DataEntrada}, Pes = {Pes}, EstaAdoptat = {EstaAdoptat}, DataAdopcio = {DataAdopcio}, QuotaAdopcio = {QuotaAdopcio}, Adoptant = {Adoptant}, Voluntaris = {Voluntaris}";
        if (Mare != null)
        {
            string texto1 = $", Mare = {Mare.Nom}";
            if (Cries != null)
            {
                string texto2 = $", Cries = {Cries.Count}}}";
                return texto + texto1 + texto2;
            }
            else
            {
                string texto2 = $", Cries = 0}}";
                return texto + texto1 + texto2 +"}}";
            }
        }
        else
        {
            if (Cries != null)
            {
                string texto2 = $", Cries = {Cries.Count}}}";
                return texto + texto2;
            }
            else
            {
                string texto2 = $", Cries = 0}}";
                return texto + texto2+"}}";
            }
        }
        
    }
    
    public Animal()
    {
        DataEntrada = DateTime.Today;
        EstaAdoptat = false;
        DataAdopcio = null;
        QuotaAdopcio = 0;
    }
}