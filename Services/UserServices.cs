using BusinessObjects;
using DataAccessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepositories iUserRepositories;

        public UserServices(IUserRepositories context)
        {
            iUserRepositories = context;
        }
        public async Task CreateUserAsync(User user)
        {
            await iUserRepositories.CreateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await iUserRepositories.DeleteUserAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await iUserRepositories.EmailExistsAsync(email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await iUserRepositories.GetAllUsersAsync();
        }

        public async Task<(List<User> Users, int TotalPages)> GetPagedUsersAsync(string? role, string? searchTerm, int pageNumber, int pageSize)
        {
            return await iUserRepositories.GetPagedUsersAsync(role, searchTerm, pageNumber, pageSize);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await iUserRepositories.GetUserByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await iUserRepositories.GetUserByIdAsync(id);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await iUserRepositories.PhoneExistsAsync(phone);
        }

        public async Task<bool?> ToggleUserStatusAsync(int id)
        {
            return await iUserRepositories.ToggleUserStatusAsync(id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await iUserRepositories.UpdateUserAsync(user);
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            return await iUserRepositories.UpdateUserPasswordAsync(userId, newPassword);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await iUserRepositories.UserExistsAsync(id);
        }

        public async Task VerifyEmailAsync(int id)
        {
            await iUserRepositories.VerifyEmailAsync(id);
        }
    }
}
