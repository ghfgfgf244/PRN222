using BusinessObjects;
using DataAccessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class UserRepositories : IUserRepositories
    {
        private readonly UserDAO _userDAO;

        public UserRepositories(AppointmentsDbContext context)
        {
            _userDAO = new UserDAO(context);
        }

        public async Task CreateUserAsync(User user)
        {
            await _userDAO.CreateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userDAO.DeleteUserAsync(id);
        }

        public async Task<(List<User> Users, int TotalPages)> GetPagedUsersAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            return await _userDAO.GetPagedUsersAsync(searchTerm, pageNumber, pageSize);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userDAO.GetUserByIdAsync(id);
        }

        public async Task<bool?> ToggleUserStatusAsync(int id)
        {
            return await _userDAO.ToggleUserStatusAsync(id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userDAO.UpdateUserAsync(user);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userDAO.UserExistsAsync(id);
        }

    }
}
