using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.DTO;
using TitheSync.Domain.Models;
using TitheSync.Domain.Repositories;

namespace TitheSync.DataAccess.Repositories
{
    /// <summary>
    ///     Repository for managing member-related data access operations.
    /// </summary>
    /// <param name="dataAccess" >The SQL data access service.</param>
    public class MemberRepository( ISqlDataAccess dataAccess ):IMemberRepository
    {
        /// <summary>
        ///     Retrieves all members from the database.
        /// </summary>
        /// <returns>A collection of members.</returns>
        public async Task<IEnumerable<Member>> GetMembersAsync()
        {
            // Executes a query to retrieve all members.
            IEnumerable<MemberDto> enumerable = await dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetAll", new { });

            // Converts a collection of MemberDto objects to a collection of Member objects.
            return enumerable.Select(
                x => new Member(
                    x.MemberId,
                    x.FirstName,
                    x.LastName,
                    x.Contact,
                    x.IsLeader,
                    x.Address,
                    x.Organization,
                    x.BibleClass
                ));
        }

        /// <summary>
        ///     Retrieves a member by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member.</param>
        /// <returns>The member if found; otherwise, null.</returns>
        public async Task<Member?> GetMemberByIdAsync( int id )
        {
            // Executes a query to retrieve a member by their unique identifier.
            IEnumerable<MemberDto> enumerable = await dataAccess.QueryAsync<MemberDto, dynamic>("sp.Member_GetById", new { MemberId = id });

            // Retrieves the first member DTO from the enumerable, or null if none exist.
            MemberDto? memberDto = enumerable.FirstOrDefault();

            // Converts a MemberDto object to a Member object if the MemberDto is not null.
            return memberDto is not null
                ? new Member(
                    memberDto.MemberId,
                    memberDto.FirstName,
                    memberDto.LastName,
                    memberDto.Contact,
                    memberDto.IsLeader,
                    memberDto.Address,
                    memberDto.Organization,
                    memberDto.BibleClass
                )
                : null;
        }


        /// <summary>
        ///     Adds a new member to the database.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        public async Task AddMemberAsync( Member member )
        {
            // Creates a new instance of the MemberDto class using the provided Member object.
            MemberDto record = new(
                member.MemberId,
                member.FirstName,
                member.LastName,
                member.Contact,
                member.IsLeader,
                member.Address,
                member.Organization,
                member.BibleClass
            );

            await dataAccess.CommandAsync("sp.Member_Add", record);
        }

        /// <summary>
        ///     Updates an existing member in the database.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        public async Task UpdateMemberAsync( Member member )
        {
            // Creates a new instance of the MemberDto class using the provided Member object.
            MemberDto record = new(
                member.MemberId,
                member.FirstName,
                member.LastName,
                member.Contact,
                member.IsLeader,
                member.Address,
                member.Organization,
                member.BibleClass
            );

            await dataAccess.CommandAsync("sp.Member_Update", record);
        }

        /// <summary>
        ///     Deletes a member from the database by their unique identifier.
        /// </summary>
        /// <param name="id" >The unique identifier of the member to delete.</param>
        public async Task DeleteMemberAsync( int id )
        {
            await dataAccess.CommandAsync("sp.Member_Delete", new { MemberId = id });
        }
    }
}
