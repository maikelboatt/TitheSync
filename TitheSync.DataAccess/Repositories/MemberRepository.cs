using Microsoft.Extensions.Logging;
using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.DTO;
using TitheSync.Domain.Models;
using TitheSync.Domain.Repositories;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Repository for managing Member entities in the data store.
    /// </summary>
    public class MemberRepository:IMemberRepository
    {
        private readonly ISqlDataAccess _dataAccess;
        private readonly ILogger<MemberRepository> _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberRepository" /> class.
        /// </summary>
        /// <param name="dataAccess" >The data access layer for executing SQL commands.</param>
        /// <param name="logger" >The logger instance for logging repository operations.</param>
        /// <exception cref="ArgumentNullException" >Thrown when <paramref name="dataAccess" /> is null.</exception>
        public MemberRepository( ISqlDataAccess dataAccess, ILogger<MemberRepository> logger )
        {
            // Initialize the data access layer
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _logger = logger;
        }

        /// <summary>
        ///     Retrieves all members from the data store.
        /// </summary>
        /// <returns>A collection of <see cref="Member" /> objects.</returns>
        /// <exception cref="Exception" >Thrown when an error occurs during data retrieval.</exception>
        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            try
            {
                IEnumerable<MemberDto> memberDtos = await _dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetAll", new { });
                return memberDtos.Select(MapToMember);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving members");
                throw;
            }
        }

        /// <summary>
        ///     Retrieves a member by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member.</param>
        /// <returns>The <see cref="Member" /> object if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException" >Thrown when the <paramref name="id" /> is invalid.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data retrieval.</exception>
        public async Task<Member?> GetMemberByIdAsync( int id )
        {
            if (id <= 0) throw new ArgumentException("Invalid member ID.", nameof(id));

            try
            {
                IEnumerable<MemberDto> memberDtos = await _dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetById", new { MemberId = id });
                return memberDtos.Select(MapToMember).FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error retrieving member by ID: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Adds a new member to the data store.
        /// </summary>
        /// <param name="member" >The <see cref="Member" /> object to add.</param>
        /// <exception cref="ArgumentNullException" >Thrown when <paramref name="member" /> is null.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data insertion.</exception>
        public async Task AddMemberAsync( Member member )
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            try
            {
                MemberDto record = MapToMemberDto(member);
                await _dataAccess.CommandAsync(
                    "sp.Member_Add",
                    new
                    {
                        record.FirstName,
                        record.LastName,
                        record.Contact,
                        record.IsLeader,
                        record.Address,
                        record.Organization,
                        record.BibleClass
                    });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error adding member: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Updates an existing member in the data store.
        /// </summary>
        /// <param name="member" >The <see cref="Member" /> object to update.</param>
        /// <exception cref="ArgumentNullException" >Thrown when <paramref name="member" /> is null.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data update.</exception>
        public async Task UpdateMemberAsync( Member member )
        {
            ArgumentNullException.ThrowIfNull(member);

            try
            {
                MemberDto record = MapToMemberDto(member);
                await _dataAccess.CommandAsync("sp.Member_Update", record);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error updating member: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Deletes a member from the data store by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member to delete.</param>
        /// <exception cref="ArgumentException" >Thrown when the <paramref name="id" /> is invalid.</exception>
        /// <exception cref="Exception" >Thrown when an error occurs during data deletion.</exception>
        public async Task DeleteMemberAsync( int id )
        {
            if (id <= 0) throw new ArgumentException("Invalid member ID.", nameof(id));

            try
            {
                await _dataAccess.CommandAsync("sp.Member_Delete", new { MemberId = id });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError("Error deleting member: {ExMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        ///     Maps a <see cref="MemberDto" /> object to a <see cref="Member" /> object.
        /// </summary>
        /// <param name="dto" >The <see cref="MemberDto" /> object to map.</param>
        /// <returns>The mapped <see cref="Member" /> object.</returns>
        private static Member MapToMember( MemberDto dto ) => new(dto.MemberId, dto.FirstName, dto.LastName, dto.Contact, dto.IsLeader, dto.Address, dto.Organization, dto.BibleClass);

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
            member.IsLeader,
            member.Address,
            member.Organization,
            member.BibleClass);
    }
}
