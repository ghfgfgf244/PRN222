using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class UserDAO
    {
        private readonly AppointmentsDbContext _context;

        public UserDAO(AppointmentsDbContext context)
        {
            _context = context;
        }
        // Lấy danh sách người dùng có phân trang và tìm kiếm
        public async Task<(List<User> Users, int TotalPages)> GetPagedUsersAsync(string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => EF.Functions.Like(u.FullName, $"%{searchTerm}%"));
            }

            int total = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalPages);
        }
        // Tìm User theo ID
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task CreateUserAsync(User user)
        {
            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, user.Password); // Hash trước khi lưu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, user.Password);

                _context.Attach(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExistsAsync(user.UserId))
                    return false;
                throw;
            }
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Đổi trạng thái kích hoạt (toggle status)
        public async Task<bool?> ToggleUserStatusAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
            return user.IsActive;
        }

        // Kiểm tra tồn tại User
        public async Task<bool> UserExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(u => u.UserId == id);
        }

        // Lấy danh sách user theo Role
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role != null && u.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
        }


    }
}
