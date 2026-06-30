namespace Sudan_Train.Data.Helpers
{
    /// <summary>
    /// Per-segment seat overlap: [b1, a1] and [b2, a2] overlap iff b1 &lt; a2 &amp;&amp; b2 &lt; a1.
    /// Stop orders are route positions (origin=0, destination=max+1).
    /// </summary>
    public static class SegmentOverlapHelper
    {
        public static bool RangesOverlap(int boardingOrder, int alightingOrder, int otherBoarding, int otherAlighting) =>
            boardingOrder < otherAlighting && otherBoarding < alightingOrder;
    }
}
