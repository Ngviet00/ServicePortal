using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.Interfaces;

namespace ServicePortal.Modules.User.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAll()
        {
            var users = await _context.Users.ToListAsync();

            List<UserDTO> userDTO = _mapper.Map<List<UserDTO>>(users);

            return userDTO;
        }

        public async Task<UserDTO> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("Code không được để trống");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Code == code) ?? throw new NotFoundException("User not found!");

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetById(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id không được để trống");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            return _mapper.Map<UserDTO>(user);
        }

        //public async Task<UserDTO> Update(Guid id, UpdateUserRequest request)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

        //    throw new NotImplementedException();
        //}

        public async Task<UserDTO> Delete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id không được để trống");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            user.DeletedAt = DateTime.Now;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> ForceDelete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id không được để trống");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDTO>(user);
        }
    }
}
