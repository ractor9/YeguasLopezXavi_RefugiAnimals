namespace RefugiAnimals;

public class VisitaService(RefugiContext context)
{
    
    private RefugiContext _context;
    public void GenerarVisites(int quantes)
    {
        Random rnd = new Random();
        

        List<Visita> visites = new List<Visita>();
        for (int i = 0; i < quantes; i++)
        {
            List<Animal> animals = _context.Animal.ToList();
            Animal a = animals[rnd.Next(animals.Count)];
            DateTime data = DateTime.Today.AddDays(-rnd.Next(0, 365));
            int c = rnd.Next(20, 201);
            Motiu m = Enum.GetValues<Motiu>()[Enum.GetValues<Motiu>().Length];
            Visita visita = new Visita()
            {
                Animal = a, Data = data, Cost = c, Motiu = m
            };
            visites.Add(visita);
            
        }
        _context.Visita.AddRange(visites);
        _context.SaveChanges();
    }

    public void EstadistiquesPerEspecie()
    {
        
        var llista = _context.Visita 
            .GroupBy(v => v.Animal.Especie)
            .Select(g => new
            {
                Animals = g.Count(),
                Adoptats = g.Count(v => v.Animal.EstaAdoptat),
                Disponibles = g.Count(v => !v.Animal.EstaAdoptat),
                Pes = g.Average(v => v.Animal.Pes)
            })
            .OrderByDescending(v => v.Animals)
            .ToList();
        foreach (var a in llista)
        {
            Console.WriteLine($"Animals: {a.Animals}, Adoptats: {a.Adoptats}, Disponibles: {a.Disponibles}, Pes: {a.Pes}");
        }
        
    }
}