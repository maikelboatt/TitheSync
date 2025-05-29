using Microsoft.Extensions.Logging;
using TitheSync.Business.Services.Members;
using TitheSync.Domain.Models;

namespace TitheSync.Core.Stores
{
    /// <summary>
    ///     Represents a store for managing members, providing thread-safe access and operations.
    /// </summary>
    public class MemberStore:IMemberStore
    {
        private readonly ReaderWriterLockSlim _lock = new();
        private readonly ILogger<MemberStore> _logger;
        private readonly List<Member> _members = [];
        private readonly IMemberService _memberService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberStore" /> class.
        /// </summary>
        /// <param name="memberService" >The service used to manage members.</param>
        /// <param name="logger" >The logger instance for logging store operations.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when the <paramref name="memberService" /> and or
        ///     <seealso cref="logger" /> is null.
        /// </exception>
        public MemberStore( IMemberService memberService, ILogger<MemberStore> logger )
        {
            // Validate the member service
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            // Initialize the logger
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Gets the list of members in a thread-safe manner.
        /// </summary>
        public IEnumerable<Member> Members
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _members.ToList();
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Event triggered when members are changed.
        /// </summary>
        public event Action<Member>? OnMembersChanged;

        /// <summary>
        ///     Event triggered when a member is added.
        /// </summary>
        public event Action<Member>? OnMemberAdded;

        /// <summary>
        ///     Event triggered when a member is updated.
        /// </summary>
        public event Action<Member>? OnMemberUpdated;

        /// <summary>
        ///     Event triggered when a member is deleted.
        /// </summary>
        public event Action<Member>? OnMemberDeleted;

        /// <summary>
        ///     Event triggered when members are loaded.
        /// </summary>
        public event Action? OnMembersLoaded;

        /// <summary>
        ///     Loads members asynchronously from the member service.
        /// </summary>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task LoadMemberAsync( CancellationToken cancellationToken = default )
        {
            try
            {
                IEnumerable<Member> members = await _memberService.GetMembersAsync();

                _lock.EnterWriteLock();
                try
                {
                    _members.Clear();
                    _members.AddRange(members);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

                OnMembersLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading members: {ExMessage}, {ExInnerException}", ex.Message, ex.InnerException);
                throw;
            }
        }

        /// <summary>
        ///     Adds a new member asynchronously.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task AddMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            await _memberService.AddMemberAsync(member);

            _lock.EnterWriteLock();
            try
            {
                _members.Add(member);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            OnMemberAdded?.Invoke(member);
        }

        /// <summary>
        ///     Updates an existing member asynchronously.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task UpdateMemberAsync( Member member, CancellationToken cancellationToken = default )
        {
            await _memberService.UpdateMemberAsync(member);

            _lock.EnterWriteLock();
            try
            {
                int index = _members.FindIndex(m => m.MemberId == member.MemberId);
                if (index != -1)
                {
                    _members[index] = member;
                    OnMemberUpdated?.Invoke(member);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Deletes a member asynchronously by their ID.
        /// </summary>
        /// <param name="memberId" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public async Task DeleteMemberAsync( int memberId, CancellationToken cancellationToken = default )
        {
            await _memberService.DeleteMemberAsync(memberId);

            _lock.EnterWriteLock();
            try
            {
                Member? member = _members.FirstOrDefault(m => m.MemberId == memberId);
                if (member != null)
                {
                    _members.Remove(member);
                    OnMemberDeleted?.Invoke(member);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
