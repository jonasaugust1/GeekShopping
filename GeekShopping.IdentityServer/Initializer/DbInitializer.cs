using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekShopping.IdentityServer.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly MySQLContext _context;
        private readonly UserManager<ApplicationUser> _user;
        private readonly RoleManager<IdentityRole> _role;

        public DbInitializer(
            MySQLContext context, 
            UserManager<ApplicationUser> user, 
            RoleManager<IdentityRole> role)
        {
            _context = context;
            _user = user;
            _role = role;
        }

        public void Initialize()
        {
            if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;

            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
            _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

            ApplicationUser admin = new()
            {
                UserName = "JonasAdmin",
                Email = "jonasaugusto99@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (79) 99933-9044",
                FirstName = "Jonas",
                LastName = "Araujo"
            };

            _user.CreateAsync(admin, "Jonas123$").GetAwaiter().GetResult();
            _user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();
            IdentityResult adminClaims = _user.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, admin.FirstName),
                new Claim(JwtClaimTypes.FamilyName, admin.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
            }).Result; 
            
            ApplicationUser user = new()
            {
                UserName = "JonasClient",
                Email = "jonasaugusto777@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+55 (79) 99933-9044",
                FirstName = "Jonas",
                LastName = "Araujo"
            };

            _user.CreateAsync(user, "Jonas123$").GetAwaiter().GetResult();
            _user.AddToRoleAsync(user, IdentityConfiguration.Client).GetAwaiter().GetResult();
            IdentityResult userClaims = _user.AddClaimsAsync(user, new Claim[]
            {
                new Claim(JwtClaimTypes.Name, $"{user.FirstName} {admin.LastName}"),
                new Claim(JwtClaimTypes.GivenName, user.FirstName),
                new Claim(JwtClaimTypes.FamilyName, user.LastName),
                new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
            }).Result;
        }
    }
}
