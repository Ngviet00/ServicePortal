namespace ServicePortals.Domain.Enums
{
    public enum StatusApplicationFormEnum
    {
        PENDING = 1,
        IN_PROCESS = 2,
        COMPLETE = 3,
        WAIT_HR = 4,
        REJECT = 5,
        ORG_UNIT_ID_HR_LEAVE_RQ = -10
    }

    public enum StatusLeaveRequestStepEnum
    {
        PENDING = 1,
        APPROVAL = 2,
        REJECT = 3
    }
}
