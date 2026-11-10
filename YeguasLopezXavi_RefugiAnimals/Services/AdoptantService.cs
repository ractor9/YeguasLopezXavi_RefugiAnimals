namespace RefugiAnimals;

public class AdoptantService
{
    private RefugiContext _context;

    public AdoptantService(RefugiContext context)
    {
        _context = context;
    }
    public bool Adoptar(string dni, string codiXip)
    {
        if (codiXip.Length != 15 || !codiXip.All(char.IsDigit))
        {
            Console.WriteLine($"El codi {codiXip} no és vàlid: han de ser 15 dígits");
            return false;
        }

        
        Animal? animal = _context.Animal.FirstOrDefault(a => a.Xip.Codi == codiXip);
        if (animal == null)
        {
            Console.WriteLine($"No hi ha cap animal amb el xip {codiXip}");
            return false;
        }

        if (animal.EstaAdoptat)
        {
            Console.WriteLine($"Ja esta adoptat el animal amb el xip {codiXip}");
            return false;
        }
        Adoptant? adoptant = _context.Adoptant.FirstOrDefault(a => a.DNI == dni);
        if (adoptant == null)
        {
            Console.WriteLine($"No hi ha cap adoptant amb el dni {dni}");
            return false;
        }
        if (animal.Especie.Equals(Especie.GOS))
        {
            if (!adoptant.TeJardi)
            {
                Console.WriteLine($"El adoptant ha de tenir jardi");
                return false;
            }
            
        }

        if (adoptant.Animals.Count == 3)
        {
            Console.WriteLine($"El adoptant ja te 3 animals adoptats");
            return false;
        }
        animal.Adoptant = adoptant;
        animal.EstaAdoptat = true;
        animal.DataAdopcio = DateTime.Today;
        _context.SaveChanges();
        Console.WriteLine($"{animal.Nom} adoptat per {adoptant.Nom}");
        return true;
    }

    public bool Retornar(string codiXip)
    {
        if (codiXip.Length != 15 || !codiXip.All(char.IsDigit))
        {
            Console.WriteLine($"El codi {codiXip} no és vàlid: han de ser 15 dígits");
            return false;
        }

        
        Animal? animal = _context.Animal.FirstOrDefault(a => a.Xip.Codi == codiXip);
        if (animal == null)
        {
            Console.WriteLine($"No hi ha cap animal amb el xip {codiXip}");
            return false;
        }

        if (!animal.EstaAdoptat)
        {
            Console.WriteLine($"No esta adoptat el animal amb el xip {codiXip}");
            return false;
        }
        animal.Adoptant = null;
        animal.EstaAdoptat = false;
        animal.DataAdopcio = null;
        animal.DataEntrada = DateTime.Today;
        _context.SaveChanges();
        Console.WriteLine($"{animal.Nom} retornat");
        Console.WriteLine($"{animal.Nom} retornat");
        return true;
    }
}