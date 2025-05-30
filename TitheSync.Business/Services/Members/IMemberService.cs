using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Members
{
    /// <summary>
    ///     Defines the contract for member-related operations, including retrieval,
    ///     addition, update, and deletion of members in the system.
    /// </summary>
    public interface IMemberService
    {
        /// <summary>
        ///     Retrieves all members asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation, containing a collection of members.</returns>
        Task GetMembersAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Retrieves a member by their ID asynchronously.
        /// </summary>
        /// <param name="memberId" >The ID of the member to retrieve.</param>
        /// <returns>The member with the specified ID, or null if not found.</returns>
        Member? GetMemberById( int memberId );

        /// <summary>
        ///     Adds a new member asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddMemberAsync( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing member asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateMemberAsync( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteMemberAsync( int id, CancellationToken cancellationToken = default );
    }
}
