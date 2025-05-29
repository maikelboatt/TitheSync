using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Members
{
    public interface IMemberService
    {
        /// <summary>
        ///     Retrieves all members asynchronously.
        /// </summary>
        /// <returns>A collection of members.</returns>
        Task<IEnumerable<Member>> GetMembersAsync();

        /// <summary>
        ///     Retrieves a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to retrieve.</param>
        /// <returns>The member with the specified ID.</returns>
        Task<Member> GetMemberByIdAsync( int id );

        /// <summary>
        ///     Adds a new member asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        Task AddMemberAsync( Member member );

        /// <summary>
        ///     Updates an existing member asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        Task UpdateMemberAsync( Member member );

        /// <summary>
        ///     Deletes a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        Task DeleteMemberAsync( int id );
    }
}
