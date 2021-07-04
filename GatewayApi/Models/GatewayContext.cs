using Microsoft.EntityFrameworkCore;

namespace GatewayApi.Models
{
    public class GatewayContext : DbContext
    {
        
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<Device> Devices { get; set; }
        public GatewayContext(DbContextOptions<GatewayContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {         
        }
    }
}
