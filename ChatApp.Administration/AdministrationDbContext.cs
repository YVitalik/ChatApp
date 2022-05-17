using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Administration
{
    public class AdministrationDbContext : IdentityDbContext<IdentityUser>
    {
        public AdministrationDbContext(DbContextOptions<AdministrationDbContext> options) : base(options)
        {

        }
    }
}
