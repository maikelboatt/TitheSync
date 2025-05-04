using TitheSync.Domain.Models;

namespace TitheSync.Core.Stores
{
    /// <summary>
    ///     Interface for managing members in the system.
    /// </summary>
    public interface IMemberStore
    {
        /// <summary>
        ///     Gets the collection of members.
        /// </summary>
        IEnumerable<Member> Members { get; }

        /// <summary>
        ///     Loads the collection of members asynchronously.
        /// </summary>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task LoadMemberAsync( CancellationToken cancellationToken = default );

        /// <summary>
        ///     Adds a new member to the collection asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task AddMemberAsync( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing member in the collection asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task UpdateMemberAsync( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a member from the collection asynchronously by their ID.
        /// </summary>
        /// <param name="memberId" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        Task DeleteMemberAsync( int memberId, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Event triggered when members are changed.
        /// </summary>
        event Action<Member> OnMembersChanged;

        /// <summary>
        ///     Event triggered when a member is added.
        /// </summary>
        event Action<Member> OnMemberAdded;

        /// <summary>
        ///     Event triggered when a member is updated.
        /// </summary>
        event Action<Member> OnMemberUpdated;

        /// <summary>
        ///     Event triggered when a member is deleted.
        /// </summary>
        event Action<Member> OnMemberDeleted;

        /// <summary>
        ///     Event triggered when members are loaded.
        /// </summary>
        event Action OnMembersLoaded;
    }
}
