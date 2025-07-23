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
        public async Task<(List<User> Users, int TotalPages)> GetPagedUsersAsync(string? role, string? searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            // Lọc theo Role nếu có
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role != null && u.Role.Trim().ToLower() == role.Trim().ToLower());
            }

            // Tìm kiếm theo FullName, Email hoặc PhoneNumber nếu có searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))
                );
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

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
                // Lấy dữ liệu hiện tại từ DB
                var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == user.UserId);

                if (existingUser == null)
                    return false;

                // Nếu password mới null hoặc rỗng, giữ nguyên password cũ
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = existingUser.Password;
                }
                else
                {
                    // Có nhập password mới → hash lại
                    var hasher = new PasswordHasher<User>();
                    user.Password = hasher.HashPassword(user, user.Password);
                }

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
        public async Task VerifyEmailAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new Exception("User not found");

            user.IsActive = true;
            await _context.SaveChangesAsync();
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

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phone);
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return false;

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, newPassword);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
           .Where(u => u.Role == "Patient")
           .ToListAsync();
        }
    }
}
