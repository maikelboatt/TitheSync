using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Models;

namespace TitheSync.Business.Services.Members
{
    /// <summary>
    ///     Service for managing members.
    /// </summary>
    public class MemberService( IMemberRepository memberRepository ):IMemberService
    {
        /// <summary>
        ///     Retrieves all members asynchronously.
        /// </summary>
        /// <returns>A collection of members.</returns>
        public async Task<IEnumerable<Member>> GetMembersAsync() => await memberRepository.GetMembersAsync();

        /// <summary>
        ///     Retrieves a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to retrieve.</param>
        /// <returns>The member with the specified ID.</returns>
        public async Task<Member> GetMemberByIdAsync( int id ) => await memberRepository.GetMemberByIdAsync(id);

        /// <summary>
        ///     Adds a new member asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        public async Task AddMemberAsync( Member member )
        {
            await memberRepository.AddMemberAsync(member);
        }

        /// <summary>
        ///     Updates an existing member asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        public async Task UpdateMemberAsync( Member member )
        {
            await memberRepository.UpdateMemberAsync(member);
        }

        /// <summary>
        ///     Deletes a member by their ID asynchronously.
        /// </summary>
        /// <param name="id" >The ID of the member to delete.</param>
        public async Task DeleteMemberAsync( int id )
        {
            await memberRepository.DeleteMemberAsync(id);
        }
    }
}
