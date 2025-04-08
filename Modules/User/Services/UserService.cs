using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.User.Responses;

namespace ServicePortal.Modules.User.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            var userResponse = _mapper.Map<List<UserResponse>>(users);

            return userResponse;
        }

        public async Task<UserResponse?> GetById(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                return _mapper.Map<UserResponse>(user);
            }

            return null;
        }

        public async Task<UserResponse> Update(UserResponse userResponse)
        {
            var user = _context.Users.Find(userResponse.Id) ?? throw new NotFoundException("User not found!");

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return userResponse;
        }

        public async Task Delete(Guid id)
        {
            var user = _context.Users.Find(id) ?? throw new NotFoundException("User not found!");

            user.DeletedAt = DateTime.UtcNow;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task ForceDelete(Guid id)
        {
            var user = _context.Users.Find(id) ?? throw new NotFoundException("User not found!");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}
