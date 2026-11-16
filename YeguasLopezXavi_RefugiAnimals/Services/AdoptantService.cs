namespace RefugiAnimals;

public class AdoptantService
{
    private RefugiContext _context;

    public AdoptantService(RefugiContext context)
    {
        _context = context;
    }
    public List<Adoptant> GetTots()
    {
        List<Adoptant> llista = _context.Adoptant
            .OrderBy(a => a.AdoptantID)
            .ToList();
        return llista;
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
    
    
    public void LlistatAdoptants(int minAnimals)
    {
        // UNA sola consulta: el filtre i l'ordre els fa la BD amb el recompte de la col·lecció
        // (EF ho tradueix a un SELECT COUNT(*) FROM Animal WHERE AdoptantID = ...).
        // De MÉS a menys: OrderByDescending. I no .ToList() abans del Where: això portaria tots els adoptants a memòria
        List<Adoptant> llista = _context.Adoptant
            .Where(a => a.Animals.Count >= minAnimals)
            .OrderByDescending(a => a.Animals.Count)
            .ToList();

        // Si vols que el recompte també surti de la mateixa consulta (sense lazy loading en el foreach):
        //   .Select(a => new { NomComplet = a.Nom + " " + a.Cognoms, NumAnimals = a.Animals.Count })

        if (llista.Count == 0)
        {
            Console.WriteLine($"Cap adoptant té {minAnimals} animals o més");
            return;
        }
        // La funció és void: ha d'escriure ella per consola (Console, no Debug.WriteLine, que no surt a la consola)
        foreach (Adoptant a in llista)
        {
            Console.WriteLine($"{a.Nom} {a.Cognoms}: {a.Animals.Count} animals");
        }
    }
}