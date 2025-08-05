namespace Entities.Dtos
{
    public record StatsDto
    {
        public String? Key { get; init; }
        public int TotalCount { get; init; }
        public int ThisMonthsCount { get; init; }
        public int LastMonthsCount { get; init; }
        public int Difference => ThisMonthsCount - LastMonthsCount;
        public String SignedDifference => $"{(Difference >= 0 ?  "+" : "")}{Difference}";
        public int Percentage => LastMonthsCount == 0 ? ThisMonthsCount * 100 : (int) (((double)(Difference) / LastMonthsCount) * 100);
        public String SignedPercentage => $"{(Percentage >= 0 ? "+" : "")}{Percentage}";
    }
}
