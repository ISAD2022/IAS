using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS;
using AIS.Models;
using Microsoft.EntityFrameworkCore;

namespace AIS.Models
{
    public class DBContext:DbContext
    {
       

        public DBContext(DbContextOptions<DBContext> options)
            : base(options) 
        {
            Database.EnsureCreated();
        } 
        public DbSet<UserModel> userModels { get; set; }
        
    }
}
