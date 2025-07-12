using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IUserRepositories
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(User user);
        Task CreateUserAsync(User user);
        Task<(List<User> Users, int TotalPages)> GetPagedUsersAsync(string? searchTerm, int pageNumber, int pageSize);
        Task<bool> DeleteUserAsync(int id);
        Task<bool?> ToggleUserStatusAsync(int id);
        Task<bool> UserExistsAsync(int id);
    }
}
