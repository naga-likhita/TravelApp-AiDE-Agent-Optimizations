using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TravelApp.Entities;
using TravelApp.Mock;

namespace TravelApp;

public class TravelDbContext : DbContext
{
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {

    }
    public static string DbPath => $"Data Source=D:\\TravelDb_FatBrain.db";

    public static TravelDbContext Instance
    {
        get
        {
            var optionsBuilder = new DbContextOptionsBuilder<TravelDbContext>();
            optionsBuilder.UseSqlite(DbPath);
            return new TravelDbContext(optionsBuilder.Options);
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasConversion<string>();

        // Index configurations
        modelBuilder.Entity<Flight>()
            .HasIndex(f => new { f.Departure, f.Destination });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Country);

        // PhoneNumber index without included columns due to EF Core version constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber);

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.UserId);

        base.OnModelCreating(modelBuilder);
    }
    public void MockDynamic()
    {
        var isNewlyCreated = Database.EnsureCreated();
        if (isNewlyCreated is false)
            throw new Exception("DB Already mocked!!");

        var users = JsonSerializer.Deserialize<User[]>(File.ReadAllText("Mock/Users.json"));
        Users.AddRange(users);
        SaveChanges();

        Mocker.MockFlights(this);
        Mocker.MockBookings(this);
    }
    public void Import()
    {
        var isNewlyCreated = Database.EnsureCreated();
        if (isNewlyCreated is false)
            throw new Exception("DB Already mocked!!");

        var users = JsonSerializer.Deserialize<User[]>(File.ReadAllText("Mock/Users.json"));
        var flights = JsonSerializer.Deserialize<Flight[]>(File.ReadAllText("Mock/Flights.json"));
        var bookings = JsonSerializer.Deserialize<Booking[]>(File.ReadAllText("Mock/Bookings.json"));

        Users.AddRange(users);
        Flights.AddRange(flights);
        Bookings.AddRange(bookings);

        SaveChanges();
    }

    public void Export()
    {
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        File.WriteAllText("Mock/Users.json", JsonSerializer.Serialize(Users.ToList(), jsonOptions));
        File.WriteAllText("Mock/Flights.json", JsonSerializer.Serialize(Flights.ToList(), jsonOptions));
        File.WriteAllText("Mock/Bookings.json", JsonSerializer.Serialize(Bookings.ToList(), jsonOptions));
    }
}
