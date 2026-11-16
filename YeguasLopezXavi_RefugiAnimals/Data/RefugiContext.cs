using Microsoft.EntityFrameworkCore;

namespace RefugiAnimals;

public class RefugiContext:DbContext
{
    public RefugiContext(){}
    //Mapegem els DBsets com mapejaven les taules a hibernate
    public DbSet<Animal> Animal { get; set; }
    public DbSet<Xip> Xip { get; set; }
    public DbSet<Adoptant> Adoptant { get; set; }
    public DbSet<Voluntari> Voluntari { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite("Data Source=refugi.db").UseLazyLoadingProxies();
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Xip)
            .WithOne(x => x.Animal)
            .HasForeignKey<Animal>("XipID")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Animal>()
            .Property(a => a.Especie)
            .HasConversion<string>();
        
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Adoptant)
            .WithMany(ad => ad.Animals)
            .HasForeignKey("AdoptantID")
            .IsRequired(false);
        
        modelBuilder.Entity<Voluntari>()
            .HasMany(v => v.Animals)
            .WithMany(a => a.Voluntaris)
            .UsingEntity(j => j.ToTable("Cura"));

        // Com l'Especie: el torn surt com a text a la BD
        modelBuilder.Entity<Voluntari>()
            .Property(v => v.Torn)
            .HasConversion<string>();

        // [Range] i el mínim de [StringLength] EF no els comprova en desar.
        // Amb un CHECK ho comprova la BD: SQLite sí que el respecta i, si no es compleix, SaveChanges peta.
        modelBuilder.Entity<Animal>()
            .ToTable(t => t.HasCheckConstraint("CK_Animal_Pes", "\"Pes\" >= 0.1 AND \"Pes\" <= 100"));

        // Exactament 15 caràcters i tots dígits (GLOB '*[^0-9]*' = "conté algun caràcter que no és un dígit")
        modelBuilder.Entity<Xip>()
            .ToTable(t => t.HasCheckConstraint("CK_Xip_Codi", "length(\"Codi\") = 15 AND \"Codi\" NOT GLOB '*[^0-9]*'"));
        
        modelBuilder.Entity<Animal>()
            .HasOne(a => a.Mare)
            .WithMany(a => a.Cries)
            .HasForeignKey("MareID")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

    }
}