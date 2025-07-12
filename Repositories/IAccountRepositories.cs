using BusinessObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAccountRepositories
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsByEmailAsync(string email);
        Task SignOutAsync(HttpContext httpContext);
        Task<User?> AuthenticateUserAsync(string email, string password);
    }
}
