using TitheSync.ApplicationState.Stores.Members;
using TitheSync.Business.Services.Errors;
using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Members
{
    /// <summary>
    ///     Service for managing members, providing methods to retrieve, add, update, and delete member records.
    /// </summary>
    public class MemberService( IMemberRepository memberRepository, IMemberStore memberStore, IDatabaseErrorHandlerService databaseErrorHandlerService ):IMemberService
    {
        /// <summary>
        ///     Retrieves all members asynchronously and updates the member store.
        /// </summary>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GetMembersAsync( CancellationToken cancellationToken = default )
        {
            IEnumerable<Member> members = await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                memberRepository.GetMembersAsync,
                "Retrieving members"
            ) ?? [];
            memberStore.GetMembers(members, cancellationToken);
        }

        /// <summary>
        ///     Retrieves a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to retrieve.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the member with the specified ID.
        /// </returns>
        public Member? GetMemberById( int id ) => memberStore.GetMemberById(id);

        /// <summary>
        ///     Adds a new member asynchronously and updates the member store.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            int? uniqueId = await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => memberRepository.AddMemberAsync(member),
                "Adding members"
            );
            {
                Member memberWithCorrectId = memberStore.CreateMemberWithCorrectMemberId(uniqueId.Value, member);
                memberStore.AddMember(memberWithCorrectId, cancellationToken);
            }
        }

        /// <summary>
        ///     Updates an existing member asynchronously and updates the member store.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => memberRepository.UpdateMemberAsync(member),
                "Updating members"
            );
            memberStore.UpdateMember(member, cancellationToken);
        }

        /// <summary>
        ///     Deletes a member by their ID asynchronously and updates the member store.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteMemberAsync( int id, CancellationToken cancellationToken = default )
        {
            await databaseErrorHandlerService.HandleDatabaseOperationAsync(
                () => memberRepository.DeleteMemberAsync(id),
                "Deleting members"
            );
            memberStore.DeleteMember(id, cancellationToken);
        }
    }
}
