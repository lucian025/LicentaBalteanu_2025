using LicentaBalteanu.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LicentaBalteanu.Components.Account
{
    internal sealed class IdentityUserAccessor
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly IdentityRedirectManager _redirectManager;

        public IdentityUserAccessor(
            IDbContextFactory<ApplicationDbContext> factory,
            IdentityRedirectManager redirectManager)
        {
            _factory = factory;
            _redirectManager = redirectManager;
        }

        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var userId = context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _redirectManager.RedirectToWithStatus("Account/InvalidUser", "Error: User not authenticated.", context);
                return null!;
            }

            using var db = _factory.CreateDbContext();
            var user = await db.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userId}'.", context);
                return null!;
            }

            return user;
        }
    }
}
