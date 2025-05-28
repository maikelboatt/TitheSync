namespace TitheSync.Infrastructure.Services
{
    /// <summary>
    ///     Provides methods to convert between <see cref="DateTime" /> and <see cref="DateOnly" />.
    /// </summary>
    public interface IDateConverterService
    {
        /// <summary>
        ///     Converts a <see cref="DateTime" /> to a <see cref="DateOnly" />.
        /// </summary>
        /// <param name="dateTime" >The <see cref="DateTime" /> to convert.</param>
        /// <returns>The converted <see cref="DateOnly" />.</returns>
        DateOnly ConvertToDateOnly( DateTime dateTime );

        /// <summary>
        ///     Converts a <see cref="DateOnly" /> to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="dateOnly" >The <see cref="DateOnly" /> to convert.</param>
        /// <returns>The converted <see cref="DateTime" />.</returns>
        DateTime ConvertToDateTime( DateOnly dateOnly );
    }
}
