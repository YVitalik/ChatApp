using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Administration
{
    public class DbContextIdentity : IdentityDbContext<IdentityUser>
    {
        public DbContextIdentity(DbContextOptions<DbContextIdentity> options) : base(options)
        {

        }
    }
}
