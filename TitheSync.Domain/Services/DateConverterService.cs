namespace TitheSync.Domain.Services
{
    /// <summary>
    ///     Service for converting between <see cref="DateTime" /> and <see cref="DateOnly" />.
    /// </summary>
    public class DateConverterService:IDateConverterService
    {
        /// <summary>
        ///     Converts a <see cref="DateTime" /> to a <see cref="DateOnly" />.
        /// </summary>
        /// <param name="dateTime" >The <see cref="DateTime" /> to convert.</param>
        /// <returns>A <see cref="DateOnly" /> representing the date part of the input.</returns>
        public DateOnly ConvertToDateOnly( DateTime dateTime ) => new(dateTime.Year, dateTime.Month, dateTime.Day);

        /// <summary>
        ///     Converts a <see cref="DateOnly" /> to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateOnly" >The <see cref="DateOnly" /> to convert.</param>
        /// <returns>A <see cref="DateTime" /> representing the input date at midnight.</returns>
        public DateTime ConvertToDateTime( DateOnly dateOnly ) => new(dateOnly.Year, dateOnly.Month, dateOnly.Day);
    }
}
