using Microsoft.Extensions.Logging;
using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.DTO;
using TitheSync.Domain.Models;
using TitheSync.Infrastructure.Services;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Repository for managing Member entities in the data store.
    /// </summary>
    public class MemberRepository:IMemberRepository
    {
        private readonly ISqlDataAccess _dataAccess;
        private readonly IDatabaseExecutionExceptionHandlingService _databaseExecutionExceptionHandlingService;
        private readonly ILogger<MemberRepository> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberRepository" /> class.
        /// </summary>
        /// <param name="dataAccess" >The data access layer for executing SQL commands.</param>
        /// <param name="logger" >The logger instance for logging repository operations.</param>
        /// <param name="databaseExecutionExceptionHandlingService" ></param>
        /// <exception cref="ArgumentNullException" >Thrown when <paramref name="dataAccess" /> is null.</exception>
        public MemberRepository( ISqlDataAccess dataAccess, ILogger<MemberRepository> logger, IDatabaseExecutionExceptionHandlingService databaseExecutionExceptionHandlingService )
        {
            // Validate the data access layer
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            // Initialize the logger
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _databaseExecutionExceptionHandlingService = databaseExecutionExceptionHandlingService ?? throw new ArgumentNullException(nameof(databaseExecutionExceptionHandlingService));
        }

        /// <summary>
        ///     Retrieves all members from the data store.
        /// </summary>
        /// <returns>A collection of <see cref="Member" /> objects.</returns>
        /// <exception cref="Exception" >Thrown when an error occurs during data retrieval.</exception>
        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            IEnumerable<MemberDto>? result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                "sp.Member_GetAll",
                new { },
                async () => await _dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetAll", new { })
            );
            return result.Select(MapToMember);
        }

        /// <summary>
        ///     Retrieves a member by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member.</param>
        /// <returns>The <see cref="Member" /> object if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException" >Thrown when the provided ID is invalid.</exception>
        public async Task<Member?> GetMemberByIdAsync( int id )
        {
            if (id <= 0)
                throw new ArgumentException("Invalid member ID.", nameof(id));

            IEnumerable<MemberDto>? result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                "sp.Member_GetById",
                new { MemberId = id },
                async () => await _dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetById", new { MemberId = id })
            );
            return result.Select(MapToMember).FirstOrDefault();
        }


        /// <summary>
        ///     Adds a new member to the data store.
        /// </summary>
        /// <param name="member" >
        ///     The <see cref="Member" /> object to add.
        /// </param>
        /// <returns>
        ///     The unique identifier of the newly added member.
        /// </returns>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when the provided <paramref name="member" /> is null.
        /// </exception>
        public async Task<int> AddMemberAsync( Member member )
        {
            ArgumentNullException.ThrowIfNull(member);

            MemberDto record = MapToMemberDto(member);

            IEnumerable<int>? result = await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                "sp.Member_Add",
                null!, // No need to pass parameters here
                async () => await _dataAccess.QueryAsync<int, dynamic>(
                    "sp.Member_Add",
                    new
                    {
                        record.FirstName,
                        record.LastName,
                        record.Contact,
                        record.IsLeader,
                        record.Address,
                        record.Organization,
                        record.BibleClass,
                        record.Gender
                    })
            );

            return result.First();
        }

        /// <summary>
        ///     Updates an existing member in the data store.
        /// </summary>
        /// <param name="member" >The <see cref="Member" /> object with updated information.</param>
        /// <exception cref="ArgumentNullException" >Thrown when the provided member is null.</exception>
        public async Task UpdateMemberAsync( Member member )
        {
            ArgumentNullException.ThrowIfNull(member);

            await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                "sp.Member_Update",
                new { },
                async () =>
                {
                    MemberDto record = MapToMemberDto(member);
                    await _dataAccess.CommandAsync("sp.Member_Update", record);
                    return true; // Dummy return for Task<bool>
                }
            );

        }

        /// <summary>
        ///     Deletes a member from the data store by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member to delete.</param>
        /// <exception cref="ArgumentException" >Thrown when the provided ID is invalid.</exception>
        public async Task DeleteMemberAsync( int id )
        {
            if (id <= 0)
                throw new ArgumentException("Invalid member ID.", nameof(id));

            await _databaseExecutionExceptionHandlingService.ExecuteWithExceptionHandlingAsync(
                "sp.Member_Delete",
                new { MemberId = id },
                async () =>
                {
                    await _dataAccess.CommandAsync("sp.Member_Delete", new { MemberId = id });
                    return true; // Dummy return for Task<bool>
                }
            );
        }

        /// <summary>
        ///     Maps a <see cref="MemberDto" /> object to a <see cref="Member" /> object.
        /// </summary>
        /// <param name="dto" >The <see cref="MemberDto" /> object to map.</param>
        /// <returns>The mapped <see cref="Member" /> object.</returns>
        private static Member MapToMember( MemberDto dto ) => new(
            dto.MemberId,
            dto.FirstName,
            dto.LastName,
            dto.Contact,
            dto.Gender,
            dto.IsLeader,
            dto.Address,
            dto.Organization,
            dto.BibleClass);

        /// <summary>
        ///     Maps a <see cref="Member" /> object to a <see cref="MemberDto" /> object.
        /// </summary>
        /// <param name="member" >The <see cref="Member" /> object to map.</param>
        /// <returns>The mapped <see cref="MemberDto" /> object.</returns>
        private static MemberDto MapToMemberDto( Member member ) => new(
            member.MemberId,
            member.FirstName,
            member.LastName,
            member.Contact,
            member.Gender,
            member.IsLeader,
            member.Address,
            member.Organization,
            member.BibleClass);
    }
}
