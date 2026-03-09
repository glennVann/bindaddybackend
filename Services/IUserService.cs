using BinDaddy.Backend.DTOs;
using BinDaddy.Backend.Models;

namespace BinDaddy.Backend.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return users.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                return user != null ? MapToDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {id}");
                throw;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user != null ? MapToDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by email {email}");
                throw;
            }
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            try
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException($"User with email {dto.Email} already exists");

                var user = new User
                {
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Phone = dto.Phone,
                    Address = dto.Address
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create cart for user
                var cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    throw new KeyNotFoundException($"User {id} not found");

                if (!string.IsNullOrEmpty(dto.FirstName))
                    user.FirstName = dto.FirstName;
                if (!string.IsNullOrEmpty(dto.LastName))
                    user.LastName = dto.LastName;
                if (!string.IsNullOrEmpty(dto.Phone))
                    user.Phone = dto.Phone;
                if (!string.IsNullOrEmpty(dto.Address))
                    user.Address = dto.Address;

                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return MapToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {id}");
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {id}");
                throw;
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address
            };
        }
    }
}
