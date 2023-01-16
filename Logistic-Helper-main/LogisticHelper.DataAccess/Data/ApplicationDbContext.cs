using LogisticHelper.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticHelper.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }


        public DbSet<Terc> Tercs { get; set; }
        public DbSet<Simc> Simcs { get; set; }
        public DbSet<Ulic> Ulics { get; set; }
  

    }

}
