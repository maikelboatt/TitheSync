using TitheSync.Domain.Models;

namespace TitheSync.Domain.Repositories
{
    /// <summary>
    ///     Interface for managing member-related data operations.
    /// </summary>
    public interface IMemberRepository
    {
        /// <summary>
        ///     Retrieves all members asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of members.</returns>
        Task<IEnumerable<Member>> GetMembersAsync();

        /// <summary>
        ///     Retrieves a member by their unique identifier asynchronously.
        /// </summary>
        /// <param name="id" >The unique identifier of the member.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the member with the specified ID.</returns>
        Task<Member> GetMemberByIdAsync( int id );

        /// <summary>
        ///     Adds a new member asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddMemberAsync( Member member );

        /// <summary>
        ///     Updates an existing member asynchronously.
        /// </summary>
        /// <param name="member" >The member with updated information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task UpdateMemberAsync( Member member );

        /// <summary>
        ///     Deletes a member by their unique identifier asynchronously.
        /// </summary>
        /// <param name="id" >The unique identifier of the member to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteMemberAsync( int id );
    }
}
