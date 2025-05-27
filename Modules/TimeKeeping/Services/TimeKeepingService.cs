using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Modules.TimeKeeping.Services.Interfaces;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Modules.TimeKeeping.Services
{
    public class TimeKeepingService : ITimeKeepingService
    {
        private readonly ApplicationDbContext _context;

        public TimeKeepingService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ManagementTimeKeepingResponseDto> GetManagementTimeKeeping(GetManagementTimeKeepingDto request)
        {
            ManagementTimeKeepingResponseDto results = new ManagementTimeKeepingResponseDto();

            List<Holiday> holidays = new List<Holiday>
            {
                new Holiday {
                    Date = $"2025-05-04",
                    Type = "sunday"
                },
                new Holiday {
                    Date = $"2025-05-11",
                    Type = "sunday"
                },
                new Holiday {
                    Date = $"2025-05-18",
                    Type = "sunday"
                },
                new Holiday {
                    Date = $"2025-05-25",
                    Type = "sunday"
                },

                new Holiday {
                    Date = $"2025-05-16",
                    Type = "special_holiday"
                }
            };

            var userCodeTimeKeeping = await _context.ManageUserTimeKeepings
                .Where(e => e.UserCodeManage == request.UserCode)
                .Select(x => x.UserCode)
                .Distinct()
                .ToListAsync();
            
            List<UserDataTimeKeeping> userDataTimeKeeping = new List<UserDataTimeKeeping>();

            for (int i = 1; i <= userCodeTimeKeeping.Count(); i++)
            {
                var userData = new UserDataTimeKeeping
                {
                    UserCode = $"UserCode_{i}",
                    Name = $"Nguyen_Van_A_{i}"
                };

                List<Attendance> attendances = new List<Attendance>();

                for (int j = 1; j <= 31; j++)
                {
                    var attendance = new Attendance();

                    attendance.Date = $"2025-05-{(j < 10 ? $"0{j}" : j)}";

                    if (i == 2 && j == 14)
                    {
                        attendance.Status = "O";
                    }
                    else if (i == 1 && j == 27)
                    {
                        attendance.Status = "S";
                    }
                    else if (i == 4 && j == 7)
                    {
                        attendance.Status = "ND";
                    }
                    else if (i == 2 && j == 1)
                    {
                        attendance.Status = "AL";
                    }
                    else if (i == 3 && j == 16)
                    {
                        attendance.Status = "SH";
                    }
                    else
                    {
                        attendance.Status = "X";
                    }

                    if (j == 4 || j == 11 || j == 18 || j == 25)
                    {
                        attendance.Status = "CN";
                    }

                    attendances.Add(attendance);
                }

                userData.Attendances = attendances;

                userDataTimeKeeping.Add(userData);
            }

            results.Holidays = holidays;
            results.UserData = userDataTimeKeeping;

            return results;
        }

        public async Task<List<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request)
        {
            List<object> list = new List<object>();

            for (int i = 1; i < 5; i++)
            {
                list.Add(new
                {
                    Date = $"1{i}/05/2025",
                    Day = $"{i+1}",
                    From = "08:00:00",
                    To = "17:25:05",
                    DayTimeWork = "480",
                    NightTimeWork = "--",
                    DayOTWork = "90",
                    NightOTWork = "0",
                    Late = "0",
                    Early = "0",
                    GoOut = "0",
                    Note = "NPL",
                });
            }

            return list;
        }

        public async Task<object> ConfirmTimeKeeping(GetManagementTimeKeepingDto request)
        {
            var a = new object();

            return a;
        }

        public async Task<PagedResults<UserResponseDto>> GetListUserToChooseManageTimeKeeping(GetUserManageTimeKeepingDto request)
        {
            int? position = request.Position;

            string currentUserCode = request?.UserCode ?? "";

            double pageSize = request.PageSize;
            double page = request.Page;

            string name = request.Name ?? "";

            var approvalFlows = await _context.ApprovalFlows
                .FromSqlRaw(@"
                     WITH RecursiveCTE AS (
                        SELECT * FROM approval_flows WHERE ToPosition = {0}

                        UNION ALL

                        SELECT af.* FROM approval_flows af
                        JOIN RecursiveCTE cte ON af.ToPosition = cte.FromPosition
                        WHERE af.DepartmentId = cte.DepartmentId
                      )
                      SELECT * FROM RecursiveCTE
                ", position)
                .ToListAsync();

            var positionIds = approvalFlows
                .Select(x => x.FromPosition)
                .Distinct()
                .ToList();

            var query = _context.Users.AsQueryable();

            query = query.Where(e => e.UserCode != "0");

            query = query.Where(u => u.PositionId != null && positionIds.Contains(u.PositionId.Value));

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => (u.UserCode != null && u.UserCode.Contains(name)));
            }

            var users = query.Where(u => positionIds.Contains(u.PositionId));

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var usersWithDetails = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var selectedUserCodes = await _context.ManageUserTimeKeepings
                .Where(x => x.UserCodeManage == currentUserCode)
                .Select(x => x.UserCode)
                .ToListAsync();

            var resultData = UserMapper.ToDtoList(usersWithDetails);

            foreach (var user in resultData)
            {
                user.IsCheckedHaveManageUserTimeKeeping = selectedUserCodes.Contains(user.UserCode);
            }

            var result = new PagedResults<UserResponseDto>
            {
                Data = resultData,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<object> SaveManageTimeKeeping(SaveManageTimeKeepingDto request)
        {
            var userDataTimeKeeping = await _context.ManageUserTimeKeepings.Where(e => e.UserCodeManage == request.UserCodeManage).ToListAsync();

            _context.ManageUserTimeKeepings.RemoveRange(userDataTimeKeeping);

            var newData = request?.UserCodes?.Select(item => new ManageUserTimeKeeping
            {
                UserCodeManage = request.UserCodeManage,
                UserCode = item
            }).ToList();

            _context.ManageUserTimeKeepings.AddRange(newData ?? []);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
