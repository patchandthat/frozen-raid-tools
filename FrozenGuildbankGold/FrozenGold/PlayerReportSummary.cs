namespace FrozenGold
{
    public class PlayerReportSummary
    {
        public PlayerReportSummary(PlayerPaymentStatus status, int weeksDifference)
        {
            Status = status;
            WeeksDifference = weeksDifference;
        }

        public PlayerPaymentStatus Status { get; }
        public int WeeksDifference { get; }
    }
}