using System.Globalization;
using ServicePortal.Domain.Entities;
using ServicePortal.Modules.LeaveRequest.DTO;

namespace ServicePortal.Common.Mappers
{
    public class LeaveRequestMapper
    {
        public static LeaveRequestDto ToDto(LeaveRequest entity)
        {
            return new LeaveRequestDto
            {
                Id = entity.Id,
                //UserCode = entity.UserCode,
                //Name = entity.Name,
                //UserCodeRegister = entity.UserCodeRegister,
                //NameRegister = entity.NameRegister,
                //Deparment = entity.Deparment,
                //Position = entity.Position,
                //FromDate = entity.FromDate.ToString(),
                //ToDate = entity.ToDate.ToString(),
                //Reason = entity.Reason,
                //TimeLeave = entity.TimeLeave,
                //TypeLeave = entity.TypeLeave,
                //Image = entity.Image,
                //HaveSalary = entity.HaveSalary,
                //Status = entity.Status,
                //Note = entity.Note,
                //DepartmentId = entity.DepartmentId,
                //CreatedAt = entity.CreatedAt,
                //UpdatedAt = entity.UpdatedAt,
                //DeletedAt = entity.DeletedAt,
            };
        }

        public static LeaveRequest ToEntity(LeaveRequestDto dto)
        {
            return new LeaveRequest
            {
                Id = dto.Id,
                //UserCode = dto.UserCode,
                //Name = dto.Name,
                //UserCodeRegister = dto.UserCodeRegister,
                //NameRegister = dto.NameRegister,
                //Deparment = dto.Deparment,
                //Position = dto.Position,
                //FromDate = DateTime.ParseExact(dto?.FromDate ?? "", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                //ToDate = DateTime.ParseExact(dto?.ToDate ?? "", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                //Reason = dto?.Reason ?? "",
                //TimeLeave = dto?.TimeLeave,
                //TypeLeave = dto?.TypeLeave,
                //Image = dto?.Image,
                //HaveSalary = dto?.HaveSalary,
                //Status = dto?.Status,
                //Note = dto?.Note ?? "",
                //DepartmentId = dto?.DepartmentId,
                //CreatedAt = dto?.CreatedAt,
                //UpdatedAt = dto?.UpdatedAt,
                //DeletedAt = dto?.DeletedAt,
            };
        }

        public static List<LeaveRequestDto> ToDtoList(List<LeaveRequest> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<LeaveRequest> ToEntityList(List<LeaveRequestDto> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
