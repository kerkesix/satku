namespace KsxEventTracker.Domain.Aggregates.Registration
{
    public enum ConfirmResult
    {
        Success = 0,
        AlreadyConfirmed,
        NotFound,
        Failed
    }
}