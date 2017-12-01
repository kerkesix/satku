namespace Web.Logic
{
    using System.Linq;

    using Web.QueryModels;

    public interface IAttendeeSpeedCalculator
    {
        AttendeeSpeed Calculate(IOrderedEnumerable<DashboardCheckpoint> checkpoints,
            string personId);
    }
}
