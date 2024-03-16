using Microsoft.EntityFrameworkCore;

namespace WebApp;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        
    }
}