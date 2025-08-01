using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AccountServices : IAccountServices
    {
        private readonly IAccountRepositories iAccountRepositories;

        public AccountServices(IAccountRepositories context)
        {
            iAccountRepositories = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await iAccountRepositories.GetUserByEmailAsync(email);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await iAccountRepositories.UserExistsByEmailAsync(email);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await iAccountRepositories.SignOutAsync(httpContext);
        }
        public async Task<User?> AuthenticateUserAsync(string email, string password) => await iAccountRepositories.AuthenticateUserAsync(email, password);

    }
}
