using Microsoft.EntityFrameworkCore;
using Second_Step.Models;

namespace Second_Step.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
