using RefugiAnimals;
using Microsoft.EntityFrameworkCore;

using var db = new RefugiContext();
db.Database.EnsureDeleted();    // primer esborrar...
db.Database.EnsureCreated();    // ...i després crear, amb les taules noves de cada sessió

// ============ Nivell 1 · Seed ============
// Cada animal porta el seu xip: no cal fer db.Xip.Add, EF segueix la navegació.
// No posem DataImplantacio: surt la d'avui, del constructor.
List<Animal> animals = new List<Animal>()
{
    new Animal() { Nom = "Luna", Especie = Especie.GOS, EsFemella = true, Pes = 22.5,
        DataNaixement = new DateTime(2020, 3, 14), DataEntrada = new DateTime(2025, 2, 10),
        Xip = new Xip() { Codi = "724098100000001" } },
    new Animal() { Nom = "Thor", Especie = Especie.GOS, EsFemella = false, Pes = 31,
        DataNaixement = new DateTime(2019, 7, 2), DataEntrada = new DateTime(2024, 11, 5),
        Xip = new Xip() { Codi = "724098100000002" } },
    new Animal() { Nom = "Kira", Especie = Especie.GOS, EsFemella = true, Pes = 12.8,
        DataNaixement = new DateTime(2023, 1, 20), DataEntrada = new DateTime(2025, 9, 1),
        Xip = new Xip() { Codi = "724098100000003" } },
    // Ja adoptat: per veure que GetDisponibles el deixa fora
    new Animal() { Nom = "Bruc", Especie = Especie.GOS, EsFemella = false, Pes = 18,
        DataNaixement = new DateTime(2021, 5, 9), DataEntrada = new DateTime(2024, 6, 15),
        EstaAdoptat = true, DataAdopcio = new DateTime(2025, 10, 3),
        Xip = new Xip() { Codi = "724098100000004" } },
    new Animal() { Nom = "Mixa", Especie = Especie.GAT, EsFemella = true, Pes = 3.9,
        DataNaixement = new DateTime(2022, 4, 11), DataEntrada = new DateTime(2025, 3, 22),
        Xip = new Xip() { Codi = "724098100000005" } },
    new Animal() { Nom = "Lluna", Especie = Especie.GAT, EsFemella = true, Pes = 4.2,
        DataNaixement = new DateTime(2021, 12, 1), DataEntrada = new DateTime(2024, 12, 30),
        Xip = new Xip() { Codi = "724098100000006" } },
    new Animal() { Nom = "Pelut", Especie = Especie.GAT, EsFemella = false, Pes = 5.6,
        DataNaixement = new DateTime(2018, 8, 17), DataEntrada = new DateTime(2025, 7, 7),
        Xip = new Xip() { Codi = "724098100000007" } },
    new Animal() { Nom = "Floc", Especie = Especie.CONILL, EsFemella = false, Pes = 1.8,
        DataNaixement = new DateTime(2024, 2, 3), DataEntrada = new DateTime(2025, 5, 12),
        Xip = new Xip() { Codi = "724098100000008" } },
    new Animal() { Nom = "Piu", Especie = Especie.AU, EsFemella = true, Pes = 0.3,
        DataNaixement = new DateTime(2023, 9, 25), DataEntrada = new DateTime(2025, 8, 19),
        Xip = new Xip() { Codi = "724098100000009" } },
};
db.Animal.AddRange(animals);
db.SaveChanges();               // fins aquí no s'ha fet cap INSERT

AnimalService animalService = new AnimalService(db);

Console.WriteLine("--- Tots els animals ---");
foreach (Animal a in animalService.GetTots())
{
    Console.WriteLine(a);
}

// ============ Nivell 2 ============
Console.WriteLine("\n--- Gossos disponibles (el Bruc no hi surt) ---");
foreach (Animal a in animalService.GetDisponibles(Especie.GOS))
{
    Console.WriteLine(a);
}

Console.WriteLine("\n--- ALTRES disponibles ---");
List<Animal> altres = animalService.GetDisponibles(Especie.ALTRES);
if (altres.Count == 0)
{
    Console.WriteLine("No hi ha cap animal disponible d'aquesta espècie");
}

Console.WriteLine("\n--- Cerca per xip ---");
Animal? trobat = animalService.CercaPerXip("724098100000003");
if (trobat != null)
{
    Console.WriteLine(trobat);
}
animalService.CercaPerXip("724098100000999");   // format bo, però no existeix

// ============ Nivell 3 ============
Console.WriteLine("\n--- Cerca per xip, codis mal formats ---");
animalService.CercaPerXip("12345");             // massa curt
animalService.CercaPerXip("72409810000000A");   // 15 caràcters, però amb una lletra

Console.WriteLine("\n--- Cerca per nom ---");
List<string> textos = new List<string>() { "lu", "LU", "Lu" };
foreach (string text in textos)
{
    Console.WriteLine($"\"{text}\":");
    foreach (Animal a in animalService.CercaPerNom(text))
    {
        Console.WriteLine($"   {a}");
    }
}

// ============ Extra · CHECK a la BD ============
Console.WriteLine("\n--- Restriccions CHECK ---");
List<Animal> dolents = new List<Animal>()
{
    new Animal() { Nom = "Gegant", Especie = Especie.GOS, EsFemella = false, Pes = 150,     // pes > 100
        DataNaixement = new DateTime(2020, 1, 1), Xip = new Xip() { Codi = "724098100000010" } },
    new Animal() { Nom = "Curt", Especie = Especie.GAT, EsFemella = true, Pes = 4,          // codi de 5
        DataNaixement = new DateTime(2020, 1, 1), Xip = new Xip() { Codi = "12345" } },
    new Animal() { Nom = "Lletra", Especie = Especie.GAT, EsFemella = true, Pes = 4,        // 15, però amb una lletra
        DataNaixement = new DateTime(2020, 1, 1), Xip = new Xip() { Codi = "72409810000001A" } },
};
foreach (Animal dolent in dolents)
{
    try
    {
        db.Animal.Add(dolent);
        db.SaveChanges();
        Console.WriteLine($"{dolent.Nom}: s'ha desat (no hauria!)");
    }
    catch (DbUpdateException e)
    {
        // El missatge de SQLite és a la InnerException: "CHECK constraint failed: CK_..."
        Console.WriteLine($"{dolent.Nom}: no s'ha desat · {e.InnerException?.Message}");
        db.ChangeTracker.Clear();   // si no, l'animal dolent continua pendent i el següent SaveChanges tornaria a petar
    }
}

List<Adoptant> adoptants = new List<Adoptant>()
{
    new Adoptant() {DNI = "55076305K", Nom = "Xavi", Cognoms ="Yeguas Lopez", TeJardi = true},
    new Adoptant() {DNI = "55087305K", Nom = "Laia", Cognoms ="Yeguas Lopez", TeJardi = false},
};
db.Adoptant.AddRange(adoptants);
db.SaveChanges();

AdoptantService adoptantService = new AdoptantService(db);
Console.WriteLine("\n--- Adoptar ---");
adoptantService.Adoptar("55076305K", "724098100000001");
adoptantService.Adoptar("55087305K", "724098100000001");
adoptantService.Adoptar("55087305K", "724098100000002");
adoptantService.Adoptar("55076305K", "724098100000010ASDGDG");
adoptantService.Adoptar("55017305K", "724098100000001");
adoptantService.Adoptar("55076305K", "724098100000005");
adoptantService.Adoptar("55076305K", "724098100000006");
adoptantService.Adoptar("55076305K", "724098100000007");

void MostrarAdoptants()
{
    // Ho llegim de la BD amb el servei, no de les variables marta i pau
    foreach (Adoptant ad in adoptantService.GetTots())
    {
        Console.WriteLine(ad);
        if (ad.Animals.Count == 0)
        {
            Console.WriteLine("   (cap animal adoptat)");
        }
        foreach (Animal an in ad.Animals)      // cada adoptant AMB els seus animals
        {
            Console.WriteLine($"   {an}");
        }
    }
}
Console.WriteLine("\n--- Retornar ---");
adoptantService.Retornar("724098100000002");    // bé: la Marta retorna el Thor
adoptantService.Retornar("724098100000003");    // false: la Kira no està adoptada
adoptantService.Retornar("724098100000999");    // false: no hi ha cap animal amb aquest xip

Console.WriteLine("\n--- Adoptants després de retornar (la Marta ja no té el Thor) ---");
MostrarAdoptants();

// ============ Nivell 3 · Llistat ============
// Ara la Marta en té 2 (Bruc i Mixa) i el Pau 1 (Floc)
Console.WriteLine("\n--- Adoptants amb 1 animal o més ---");
adoptantService.LlistatAdoptants(1);            // Marta (2) i després Pau (1)
Console.WriteLine("\n--- Adoptants amb 3 animals o més ---");
adoptantService.LlistatAdoptants(3);            // cap


// ================================================================
// ============ SESSIÓ 3 · VOLUNTARIS =============================
// ================================================================

// ============ Nivell 1 · Dades ============
// Després de P2: Luna, Thor (retornat) i Kira són gossos no adoptats. La Lluna i el Pelut són gats no adoptats.
// Com amb el Bruc, els animals es LLEGEIXEN de la BD: les variables de la llista animals ja no les segueix EF
Animal luna = db.Animal.First(a => a.Xip.Codi == "724098100000001");
Animal kira = db.Animal.First(a => a.Xip.Codi == "724098100000003");
Animal lluna = db.Animal.First(a => a.Xip.Codi == "724098100000006");

// Variables normals del Program: res de public (això és per als atributs d'una classe, aquí no compila)
Voluntari aina = new Voluntari() { Nom = "Aina", Torn = Torn.MATI };
Voluntari joan = new Voluntari() { Nom = "Joan", Torn = Torn.TARDA };
Voluntari nuria = new Voluntari() { Nom = "Núria", Torn = Torn.CAP_DE_SETMANA };
aina.Animals.Add(luna);         // a les dades, la relació es posa afegint a la col·lecció d'un costat
aina.Animals.Add(kira);
joan.Animals.Add(lluna);

// Crear-los no n'hi ha prou: cal afegir-los al context i desar. I ABANS de cridar el servei, que els busca a la BD
db.Voluntari.AddRange(aina, joan, nuria);
db.SaveChanges();

VoluntariService voluntariService = new VoluntariService(db);

void MostrarVoluntaris()
{
    // Ho llegim de la BD amb el servei, no de les variables aina, joan i nuria
    foreach (Voluntari v in voluntariService.GetTots())
    {
        Console.WriteLine(v);
        if (v.Animals.Count == 0)
        {
            Console.WriteLine("   (no té cura de cap animal)");
        }
        foreach (Animal an in v.Animals)      // cada voluntari AMB els seus animals
        {
            Console.WriteLine($"   {an}");
        }
    }
}

Console.WriteLine("\n--- Voluntaris amb els seus animals ---");
MostrarVoluntaris();

// ============ Nivell 1 · AssignarAnimal ============
// Una assignació correcta i un cas per cada return false.
// Cada prova ha de fallar NOMÉS per la condició que vols provar: per al màxim de 4 cal un animal
// que no estigui adoptat ni assignat ja a aquell voluntari, si no fallaria abans
Console.WriteLine("\n--- Assignar animals ---");
voluntariService.AssignarAnimal("Núria", "724098100000002");    // bé: la Núria té cura del Thor
voluntariService.AssignarAnimal("Pere", "724098100000002");     // false: no hi ha cap voluntari amb aquest nom
voluntariService.AssignarAnimal("Aina", "724098100000999");     // false: no hi ha cap animal amb aquest xip
voluntariService.AssignarAnimal("Aina", "724098100000004");     // false: el Bruc està adoptat
voluntariService.AssignarAnimal("Aina", "724098100000001");     // false: l'Aina ja té cura de la Luna
voluntariService.AssignarAnimal("Aina", "724098100000007");     // bé: el Pelut, 3r animal de l'Aina
voluntariService.AssignarAnimal("Aina", "724098100000009");     // bé: el Piu, 4t animal de l'Aina
voluntariService.AssignarAnimal("Aina", "724098100000006");     // false: l'Aina ja en té 4 (la Lluna és del Joan, però pot tenir més d'un voluntari)

Console.WriteLine("\n--- Voluntaris després d'assignar ---");
MostrarVoluntaris();

// ============ Nivell 2 · DesassignarAnimal ============
Console.WriteLine("\n--- Desassignar animals ---");
voluntariService.DesassignarAnimal("Aina", "724098100000009");  // bé: l'Aina ja no té cura del Piu (en tenia 4 i en pot treure)
voluntariService.DesassignarAnimal("Pere", "724098100000009");  // false: no hi ha cap voluntari amb aquest nom
voluntariService.DesassignarAnimal("Aina", "724098100000999");  // false: no hi ha cap animal amb aquest xip
voluntariService.DesassignarAnimal("Núria", "724098100000001"); // false: la Núria no té cura de la Luna

// Desassignar no esborra l'animal: el Piu continua a la BD
Animal? piu = animalService.CercaPerXip("724098100000009");
if (piu != null)
{
    Console.WriteLine($"Continua al refugi: {piu}");
}

// ============ Nivell 2 · Adoptar treu els voluntaris ============
// La Marta té el Bruc i la Mixa i té jardí: pot adoptar la Kira, que és de l'Aina
Console.WriteLine("\n--- Adoptar un animal que tenia voluntaris ---");
adoptantService.Adoptar("12345678Z", "724098100000003");        // bé: la Marta adopta la Kira
Console.WriteLine($"Voluntaris de la Kira: {kira.Voluntaris.Count}");

Console.WriteLine("\n--- Voluntaris després de l'adopció (l'Aina ja no té la Kira) ---");
MostrarVoluntaris();

// ============ Nivell 3 · RegistrarPasseig ============
// Ara l'Aina té cura de la Luna (gos) i del Pelut (gat)
Console.WriteLine("\n--- Passejos ---");
voluntariService.RegistrarPasseig("Aina", "724098100000001", 30);   // bé: abans 0, ara 30
voluntariService.RegistrarPasseig("Aina", "724098100000001", 45);   // bé: abans 30, ara 75 (s'acumulen)
voluntariService.RegistrarPasseig("Joan", "724098100000006", 20);   // false: la Lluna és un gat
voluntariService.RegistrarPasseig("Núria", "724098100000001", 20);  // false: la Núria no té cura de la Luna

Console.WriteLine("\n--- Voluntaris al final ---");
MostrarVoluntaris();
Animal nala = new Animal()
{
    Nom = "Nala", Especie = Especie.GOS, EsFemella = true, Pes = 22.5,
    DataNaixement = DateTime.Today.AddYears(-9), DataEntrada = new DateTime(2025, 2, 10),
    Xip = new Xip() { Codi = "724098100000101" }
};
db.Animal.Add(nala);
db.SaveChanges();
Animal coco = new Animal()
{
    Nom = "Coco", Especie = Especie.GOS, EsFemella = true, Pes = 22.5,
    DataNaixement = DateTime.Today.AddYears(-5), DataEntrada = new DateTime(2025, 2, 10),
    Mare = nala, Xip = new Xip() { Codi = "724098100000102" }
};
db.Animal.Add(coco);
db.SaveChanges();
Animal rocky = new Animal()
{
    Nom = "Rocky", Especie = Especie.GOS, EsFemella = false, Pes = 22.5,
    DataNaixement = DateTime.Today.AddMonths(-4), DataEntrada = new DateTime(2025, 2, 10),
    Mare = coco, Xip = new Xip() { Codi = "724098100000103" }
};
db.Animal.Add(rocky);
db.SaveChanges();
Animal bimba = new Animal()
{
    Nom = "Bimba", Especie = Especie.GOS, EsFemella = true, Pes = 22.5,
    DataNaixement = DateTime.Today.AddMonths(-4), DataEntrada = new DateTime(2025, 2, 10),
    Mare = coco, Xip = new Xip() { Codi = "724098100000104" }
};
db.Animal.Add(bimba);
db.SaveChanges();


Console.WriteLine("--- Tots els animals ---");
foreach (Animal a in animalService.GetTots())
{
    Console.WriteLine(a);
}
Console.WriteLine(animalService.CanviarMare("724098100000103", "724098100000101"));
Console.WriteLine(animalService.RegistrarCria("724098100000101", rocky));

void MostrarFamilia()
{
    // Ho llegim de la BD amb el servei, no de les variables nala, coco...
    foreach (Animal a in animalService.GetTots())
    {
        // Mare pot ser null: a.Mare.Nom sense comprovar-ho peta (NullReferenceException) amb el primer animal sense mare.
        // També val: string mare = a.Mare?.Nom ?? "sense mare";
        string mare;
        if (a.Mare != null)
        {
            mare = $"mare {a.Mare.Nom}";
        }
        else
        {
            mare = "sense mare";
        }
        // a.Cries el carrega el lazy loading
        Console.WriteLine($"{a.AnimalID}. {a.Nom} · {mare} · {a.Cries.Count} cries");
    }
}
// ============ Nivell 2 · RegistrarCria ============
// La cria es crea aquí amb new i es passa a la funció: és el servei qui la desa, si tot és correcte.
// Cada cria dolenta només té malament el que es prova: és un gos, ha nascut fa poc i té un xip nou
Console.WriteLine("\n--- Registrar cries ---");
animalService.RegistrarCria("724098100000012", new Animal() { Nom = "Pipa", Especie = Especie.GOS, EsFemella = true, Pes = 2.3,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000015" } });   // bé: la Coco té una cria nova
animalService.RegistrarCria("724098100000013", new Animal() { Nom = "Max", Especie = Especie.GOS, EsFemella = false, Pes = 1.5,
    DataNaixement = DateTime.Today.AddDays(-10), Xip = new Xip() { Codi = "724098100000016" } });    // false: el Rocky és mascle
animalService.RegistrarCria("724098100000012", new Animal() { Nom = "Misha", Especie = Especie.GAT, EsFemella = true, Pes = 0.8,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000017" } });   // false: un gat no pot ser cria de la Coco
animalService.RegistrarCria("724098100000012", new Animal() { Nom = "Ringo", Especie = Especie.GOS, EsFemella = false, Pes = 2.6,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000011" } });   // false: aquest xip és el de la Nala
// No les demana l'enunciat, però són les altres condicions de la funció
animalService.RegistrarCria("724098100000012", new Animal() { Nom = "Duc", Especie = Especie.GOS, EsFemella = false, Pes = 20,
    DataNaixement = DateTime.Today.AddYears(-6), Xip = new Xip() { Codi = "724098100000018" } });    // false: el Duc és més gran que la Coco
animalService.RegistrarCria("724098100000999", new Animal() { Nom = "Lia", Especie = Especie.GOS, EsFemella = true, Pes = 2,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000019" } });   // false: no hi ha cap mare amb aquest xip

// La Coco ha de tenir 3 cries (Rocky, Bimba i Pipa) i cap de les dolentes ha d'haver arribat a la BD
Console.WriteLine("\n--- Animals després de registrar les cries ---");
MostrarFamilia();

// ============ Nivell 3 · Descendents ============
Console.WriteLine("\n--- Descendents ---");
Console.WriteLine("Descendents de la Nala:");
foreach (Animal a in animalService.Descendents("724098100000101"))      // Coco, Rocky, Bimba i Pipa
{
    Console.WriteLine($"   {a.Nom}");
}

List<Animal> descendentsRocky = animalService.Descendents("724098100000013");
if (descendentsRocky.Count == 0)
{
    Console.WriteLine("El Rocky no té descendents");                    // un animal sense cries: llista buida
}
animalService.CalcularDespesaMes();
animalService.CalcularDespesaMes();
coco.EstaAdoptat = true;
db.Animal.Update(coco);
db.SaveChanges();