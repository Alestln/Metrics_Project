using Metrics_Project.Entities;
using Metrics_Project.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Metrics_Project.Contexts;

public class DataDbContext(DbContextOptions<DataDbContext> options) : DbContext(options)
{
    public DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PersonConfiguration());
    }
}