using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;
using ServicePortal.Modules.TimeKeeping.Services.Interfaces;

namespace ServicePortal.Modules.TimeKeeping.Services
{
    public class TimeKeepingService : ITimeKeepingService
    {
        public async Task<object> ConfirmTimeKeeping(GetManagementTimeKeepingDto request)
        {
            var a = new object();

            return a;
        }

        public async Task<List<ManagementTimeKeepingResponseDto>> GetManagementTimeKeeping(GetManagementTimeKeepingDto request)
        {
            List<ManagementTimeKeepingResponseDto> results = new List<ManagementTimeKeepingResponseDto>();

            for (int i = 1; i <= 4; i++)
            {
                var item = new ManagementTimeKeepingResponseDto();

                item.UserCode = $"UserCode_{i}";
                item.Name = $"Nguyen_Van_A_{i}";

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

                    attendances.Add(attendance);
                }

                item.Attendances = attendances;

                results.Add(item);
            }

            return results;
        }

        public async Task<List<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request)
        {
            //query where usercode, fromdate and todate
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
    }
}
