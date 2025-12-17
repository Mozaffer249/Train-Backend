namespace Sudan_Train.Service.Models
{
    public static class EmailSendingStrategyExtensions
    {
        /// <summary>
        /// Converts EmailSendingStrategy enum to its integer value for API requests
        /// </summary>
        /// <param name="strategy">The email sending strategy</param>
        /// <returns>Integer value: 0=Direct, 1=Queued, 2=Fallback</returns>
        public static int ToIntValue(this EmailSendingStrategy strategy)
        {
            return (int)strategy;
        }
    }
}
