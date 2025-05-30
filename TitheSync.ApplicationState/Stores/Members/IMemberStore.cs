using TitheSync.Domain.Models;

namespace TitheSync.ApplicationState.Stores.Members
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
        ///     Loads the provided collection of members into the store.
        /// </summary>
        /// <param name="members" >The collection of members to load.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        void GetMembers( IEnumerable<Member> members, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Retrieves a member by their unique identifier.
        /// </summary>
        /// <param name="memberId" >The ID of the member to retrieve.</param>
        /// <returns>The member with the specified ID, or null if not found.</returns>
        Member? GetMemberById( int memberId );

        /// <summary>
        ///     Adds a new member to the collection asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        void AddMember( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Updates an existing member in the collection asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        void UpdateMember( Member member, CancellationToken cancellationToken = default );

        /// <summary>
        ///     Deletes a member from the collection asynchronously by their ID.
        /// </summary>
        /// <param name="memberId" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to cancel the operation.</param>
        void DeleteMember( int memberId, CancellationToken cancellationToken = default );

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
