namespace ServicePortal.Applications.Modules.HRManagement.DTO.Requests
{
    public class SaveHRManagementRequest
    {
        public List<DataHRManagement>? ManageTimekeeping { get; set; }
        public List<DataHRManagement>? ManageTraining { get; set; }
        public List<DataHRManagement>? ManageRecruitment { get; set; }
    }

    public class DataHRManagement
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
    }
}
