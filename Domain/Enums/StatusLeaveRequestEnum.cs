namespace ServicePortal.Domain.Enums
{
    public enum StatusLeaveRequestEnum
    {
        PENDING,
        IN_PROCESS,
        COMPLETED,
        REJECT,

        WAIT_HR = -10
    }

    public enum StatusLeaveRequestStepEnum
    {
        PENDING = 1,
        APPROVAL = 2,
        REJECT = 3
    }
}
