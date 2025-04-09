using AutoMapper;
using ServicePortal.Domain.Entities;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.Responses;

namespace ServicePortal.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<User, UserDTO>();
            CreateMap<Deparment, DeparmentDTO>();
        }
    }
}
