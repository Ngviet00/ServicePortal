namespace ServicePortal.Modules.TimeKeeping.DTO.Requests
{
    public class SaveManageTimeKeepingDto
    {
        public string? UserCodeManage {  get; set; }
        public List<string>? UserCodes { get; set; }
    }
}
