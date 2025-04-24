namespace ServicePortal.Domain.Enums
{
    public enum StatusLeaveRequestEnum
    {
        PENDING = 1,
        IN_PROCESS = 2,
        WAIT_HR_APPROVAL = 3,
        COMPLETE = 4,
        REJECT = 5
    }

    public enum StatusLeaveRequestStepEnum
    {
        PENDING = 1,
        APPROVAL = 2,
        REJECT = 3
    }
}
