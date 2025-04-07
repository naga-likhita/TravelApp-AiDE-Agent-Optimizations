﻿using Microsoft.EntityFrameworkCore;
using MessagePack;
using System.Text.Json;
using System.Text.Json.Serialization;
using TravelApp.Entities;
using TravelApp.Mock;

namespace TravelApp;

public class TravelDbContext : DbContext
{
    public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options)
    {

    }
    public static string DbPath => $"Data Source=D:\\TravelOldDb.db";

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

        modelBuilder.Entity<Flight>()
            .HasIndex(f => new { f.Departure, f.Destination });

        modelBuilder.Entity<Flight>()
            .HasIndex(f => f.Id);

        modelBuilder.Entity<Booking>()
            .HasIndex(b => b.User.PhoneNumber);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Country);

        base.OnModelCreating(modelBuilder);
    }
    public void MockDynamic()
    {
        var isNewlyCreated = Database.EnsureCreated();
        if (isNewlyCreated is false)
            throw new Exception("DB Already mocked!!");

        var users = MessagePackSerializer.Deserialize<User[]>(File.ReadAllBytes("Mock/Users.json"));
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

        var users = MessagePackSerializer.Deserialize<User[]>(File.ReadAllBytes("Mock/Users.json"));
        var flights = JsonSerializer.Deserialize<Flight[]>(File.ReadAllText("Mock/Flights.json"));
        var bookings = JsonSerializer.Deserialize<Booking[]>(File.ReadAllText("Mock/Bookings.json"));

        Users.AddRange(users);
        Flights.AddRange(flights);
        Bookings.AddRange(bookings);

        SaveChanges();
    }

    public void Export()
    {
        var users = Users.ToList();
        var flights = Flights.ToList();
        var bookings = Bookings.ToList();

        File.WriteAllBytes("Mock/Users.json", MessagePackSerializer.Serialize(users));
        File.WriteAllText("Mock/Flights.json", JsonSerializer.Serialize(flights, new JsonSerializerOptions { WriteIndented = true }));
        File.WriteAllText("Mock/Bookings.json", JsonSerializer.Serialize(bookings, new JsonSerializerOptions { WriteIndented = true }));
    }
}
