using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;

public class AnimalService
{
    private RefugiContext _context;

    public AnimalService(RefugiContext context)
    {
        _context = context;
    }

    // Nivell 1: el Program no consulta, ho demana al servei
    public List<Animal> GetTots()
    {
        List<Animal> llista = _context.Animal
            .OrderBy(a => a.AnimalID)
            .ToList();
        return llista;
    }

    // Nivell 2 · a
    public List<Animal> GetDisponibles(Especie especie)
    {
        List<Animal> llista = _context.Animal
            .Where(a => a.Especie == especie && !a.EstaAdoptat)
            .OrderBy(a => a.DataEntrada)        // la data més antiga primer: els que fa més que esperen
            .ToList();
        return llista;
    }

    // Nivell 2 · b, amb la millora del nivell 3 al principi
    public Animal? CercaPerXip(string codiXip)
    {
        // Nivell 3: comprovem el format ABANS d'anar a la BD
        if (codiXip.Length != 15 || !codiXip.All(char.IsDigit))
        {
            Console.WriteLine($"El codi {codiXip} no és vàlid: han de ser 15 dígits");
            return null;
        }

        // a.Xip.Codi dins el FirstOrDefault no és lazy loading: EF ho tradueix a un JOIN
        Animal? animal = _context.Animal.FirstOrDefault(a => a.Xip.Codi == codiXip);
        if (animal == null)
        {
            Console.WriteLine($"No hi ha cap animal amb el xip {codiXip}");
            return null;
        }
        return animal;
    }

    // Nivell 3
    public List<Animal> CercaPerNom(string text)
    {
        // Així no: a SQLite, Contains es tradueix a instr(), que distingeix majúscules.
        //   _context.Animal.Where(a => a.Nom.Contains(text))           "lu" troba la Lluna però no la Luna
        // Així tampoc: ToList() porta TOTS els animals a memòria i després filtra.
        //   _context.Animal.ToList().Where(a => a.Nom.Contains(text, StringComparison.OrdinalIgnoreCase))

        // Així sí: el lower() el fa la BD i només arriben els que coincideixen
        string cerca = text.ToLower();
        List<Animal> llista = _context.Animal
            .Where(a => a.Nom.ToLower().Contains(cerca))
            .ToList();
        return llista;

        // També val: .Where(a => EF.Functions.Like(a.Nom, $"%{text}%"))
        // (el LIKE de SQLite no distingeix majúscules). Cap dels dos funciona amb accents: À ≠ à.
    }

    public bool CanviarMare(string codiXipCria, string? codiXipMare)
    {
        Animal cria = CercaPerXip(codiXipCria);
        if (cria == null)
        {
            return false;
        }
        if (codiXipMare == null)
        {
            cria.Mare = null;
            _context.SaveChanges();
            return true;
        }
        else
        {
            
            Animal mare = CercaPerXip(codiXipMare);
            
            if (mare == null || !mare.EsFemella)
            {
                return false;
            }
            if (cria.Mare == mare)
            {
                return false;
            }
            else
            {
                cria.Mare = mare;
                _context.SaveChanges();
                return true;
            }
        }
        
    }

    public bool RegistrarCria(string codiXipMare, Animal cria)
    {
        Animal mare = CercaPerXip(codiXipMare);
        if (mare == null || !mare.EsFemella)
        {
            return false;
        }

        if (mare.Especie != cria.Especie)
        {
            return false;
        }
        else
        {
            if (mare.DataNaixement < cria.DataNaixement)
            {
                cria.Mare = mare;
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    public List<Animal> Descendents(string codiXip)
    {
        // La llista es crea UN sol cop, aquí, i es passa a l'auxiliar perquè l'ompli.
        // Error típic: crear-la dins l'auxiliar. Cada crida en fa una de nova, el resultat es perd i surt buida
        List<Animal> descendents = new List<Animal>();

        Animal? animal = _context.Animal.FirstOrDefault(a => a.Xip.Codi == codiXip);
        if (animal == null)
        {
            Console.WriteLine($"No hi ha cap animal amb el xip {codiXip}");
            return descendents;     // buida, no null: el foreach del Program no peta
        }

        AfegirDescendents(animal, descendents);
        return descendents;
    }

    // L'auxiliar RECURSIVA: rep UN animal (no una llista d'animals) i la llista on va afegint.
    // private: és una peça interna del servei, el Program no la crida
    private void AfegirDescendents(Animal animal, List<Animal> descendents)
    {
        // Cas base: si l'animal no té cries, el foreach no fa cap volta i la recursió s'acaba sola
        foreach (Animal cria in animal.Cries)       // animal.Cries el carrega el lazy loading
        {
            descendents.Add(cria);                  // primer la cria...
            AfegirDescendents(cria, descendents);   // ...i després les seves cries: aquí és on es crida a si mateixa
        }
    }
    
    public void CalcularDespesaMes()
    {
        Animal animalm = null;
        decimal ra = 0;
        decimal total = 0;
        foreach (Animal a in GetTots()) 
        {
            decimal r = 0;
            decimal rb= 0;
            if (a.Especie == Especie.GOS)
            {
                r = 60;
            }
            else if (a.Especie == Especie.GAT)
            {
                r = 40;
            }
            else if (a.Especie == Especie.CONILL)
            {
                r = 15;
            }
            else if (a.Especie == Especie.AU)
            {
                r = 10;
            }
            else if (a.Especie == Especie.ALTRES)
            {
                r = 20;
            }
                if (a.Voluntaris == null)
                {
                    r *= 0.9m;
                }
                if (a.DataNaixement.Year - DateTime.Today.Year >= 8)
                {
                    r *= 1.3m;
                }

                if (a.Cries.Count != 0)
                {
                    decimal valor = 1.1m;
                    foreach (Animal c in a.Cries)
                    {
                        if (!c.EstaAdoptat)
                        {
                            valor+=0.1m;
                        }
                    } 
                    r *= valor;
                }
                Console.WriteLine($"Nom: {a.Nom}");
                Console.WriteLine($"Despesa prèvia: {a.DespesaAcumulada}€");
                Console.WriteLine($"Despesa del mes: {r}€");
                a.DespesaAcumulada += r;
                _context.SaveChanges();
                Console.WriteLine($"Despesa actual: {a.DespesaAcumulada}€");
                total += a.DespesaAcumulada;

                
                if(animalm == null || ra < r)
                {
                    ra = r;
                    animalm = a;
                }
        }
        Console.WriteLine($"Despesa total: {total}€");
        Console.WriteLine($"Animal amb més despesa del mes: {ra} - {animalm}");
    }
}