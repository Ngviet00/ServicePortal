namespace ServicePortal.Domain.Enums
{
    public enum StatusLeaveRequestEnum
    {
        PENDING = 1,
        IN_PROCESS = 2,
        COMPLETE = 3,
        REJECT = 4,
        WAIT_HR = 5
    }

    public enum StatusLeaveRequestStepEnum
    {
        PENDING = 1,
        APPROVAL = 2,
        REJECT = 3
    }
}
