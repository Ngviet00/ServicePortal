using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.Services.Interfaces;

namespace ServicePortal.Modules.TimeKeeping.Services
{
    public class TimeKeepingService : ITimeKeepingService
    {
        public Task GetManagementTimeKeeping()
        {
            throw new NotImplementedException();
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
