using Microsoft.Extensions.Logging;

namespace TitheSync.Infrastructure.Services
{
    /// <summary>
    ///     Service for converting between <see cref="DateTime" /> and <see cref="DateOnly" />.
    /// </summary>
    public class DateConverterService( ILogger<DateConverterService> logger ):IDateConverterService
    {
        /// <summary>
        ///     Converts a <see cref="DateTime" /> to a <see cref="DateOnly" />.
        /// </summary>
        /// <param name="dateTime" >The <see cref="DateTime" /> to convert.</param>
        /// <returns>A <see cref="DateOnly" /> representing the date part of the input.</returns>
        public DateOnly ConvertToDateOnly( DateTime dateTime )
        {
            DateOnly date = new(dateTime.Year, dateTime.Month, dateTime.Day);
            return date;
        }

        /// <summary>
        ///     Converts a <see cref="DateOnly" /> to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateOnly" >The <see cref="DateOnly" /> to convert.</param>
        /// <returns>A <see cref="DateTime" /> representing the input date at midnight.</returns>
        public DateTime ConvertToDateTime( DateOnly dateOnly )
        {
            DateTime date = new(dateOnly.Year, dateOnly.Month, dateOnly.Day);
            return date;
        }
    }
}
