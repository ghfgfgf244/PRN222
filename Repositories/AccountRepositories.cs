using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccountRepositories : IAccountRepositories
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepositories(AccountDAO context)
        {
            _accountDAO = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _accountDAO.GetUserByEmailAsync(email);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _accountDAO.UserExistsByEmailAsync(email);
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await _accountDAO.SignOutAsync(httpContext);
        }

        public async Task<User?> AuthenticateUserAsync(string email, string password) => await _accountDAO.AuthenticateUserAsync(email, password);
    }
}
