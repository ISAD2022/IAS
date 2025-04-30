using Microsoft.EntityFrameworkCore;

namespace AIS.Models
    {
    public class DBContext : DbContext
        {


        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
            {
            Database.EnsureCreated();
            }
        public DbSet<UserModel> userModels { get; set; }

        }
    }
