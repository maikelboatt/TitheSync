using TitheSync.ApplicationState.Stores.Members;
using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Members
{
    /// <summary>
    ///     Service for managing members, providing methods to retrieve, add, update, and delete member records.
    /// </summary>
    public class MemberService:IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMemberStore _memberStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberService" /> class.
        /// </summary>
        /// <param name="memberRepository" >The repository for member data access.</param>
        /// <param name="memberStore" >The store for managing member state.</param>
        public MemberService( IMemberRepository memberRepository, IMemberStore memberStore )
        {
            _memberRepository = memberRepository;
            _memberStore = memberStore;
        }

        /// <summary>
        ///     Retrieves all members asynchronously and updates the member store.
        /// </summary>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task GetMembersAsync( CancellationToken cancellationToken = default )
        {
            IEnumerable<Member> members = await _memberRepository.GetMembersAsync();
            _memberStore.GetMembers(members, cancellationToken);
        }

        /// <summary>
        ///     Retrieves a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to retrieve.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the member with the specified ID.
        /// </returns>
        public Member? GetMemberById( int id ) => _memberStore.GetMemberById(id);

        /// <summary>
        ///     Adds a new member asynchronously and updates the member store.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            await _memberRepository.AddMemberAsync(member);
            _memberStore.AddMember(member, cancellationToken);
        }

        /// <summary>
        ///     Updates an existing member asynchronously and updates the member store.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            await _memberRepository.UpdateMemberAsync(member);
            _memberStore.UpdateMember(member, cancellationToken);
        }

        /// <summary>
        ///     Deletes a member by their ID asynchronously and updates the member store.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteMemberAsync( int id, CancellationToken cancellationToken = default )
        {
            await _memberRepository.DeleteMemberAsync(id);
            _memberStore.DeleteMember(id, cancellationToken);
        }
    }
}
