using Microsoft.Extensions.Logging;
using TitheSync.Domain.Models;

namespace TitheSync.ApplicationState.Stores.Members
{
    /// <summary>
    ///     Represents a store for managing members, providing thread-safe access and operations.
    /// </summary>
    public class MemberStore:IMemberStore
    {
        /// <summary>
        ///     Lock for thread-safe read/write operations on the members list.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        ///     Logger instance for logging store operations.
        /// </summary>
        private readonly ILogger<MemberStore> _logger;

        /// <summary>
        ///     Internal list of members.
        /// </summary>
        private readonly List<Member> _members = [];

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberStore" /> class.
        /// </summary>
        /// <param name="logger" >The logger instance for logging store operations.</param>
        /// <exception cref="ArgumentNullException" >
        ///     Thrown when the <paramref name="logger" /> is null.
        /// </exception>
        public MemberStore( ILogger<MemberStore> logger )
        {
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
        ///     Loads a collection of members into the store, replacing any existing members.
        /// </summary>
        /// <param name="members" >The collection of members to load.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void GetMembers( IEnumerable<Member> members, CancellationToken cancellationToken = default )
        {
            try
            {
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
        ///     Retrieves a member by their unique ID in a thread-safe manner.
        /// </summary>
        /// <param name="memberId" >The unique identifier of the member to retrieve.</param>
        /// <returns>
        ///     The <see cref="Member" /> with the specified ID, or <c>null</c> if not found.
        /// </returns>
        public Member? GetMemberById( int memberId )
        {
            _lock.EnterReadLock();
            try
            {
                return _members.FirstOrDefault(m => m.MemberId == memberId);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        ///     Adds a new member to the store in a thread-safe manner.
        /// </summary>
        /// <param name="member" >The member to add.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void AddMember( Member member, CancellationToken cancellationToken = default )
        {
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
        ///     Updates an existing member in the store in a thread-safe manner.
        /// </summary>
        /// <param name="member" >The member to update.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void UpdateMember( Member member, CancellationToken cancellationToken = default )
        {
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
        ///     Deletes a member from the store by their ID in a thread-safe manner.
        /// </summary>
        /// <param name="memberId" >The ID of the member to delete.</param>
        /// <param name="cancellationToken" >A token to monitor for cancellation requests.</param>
        public void DeleteMember( int memberId, CancellationToken cancellationToken = default )
        {
            _lock.EnterWriteLock();
            try
            {
                Member? member = _members.FirstOrDefault(m => m.MemberId == memberId);
                if (member == null) return;
                _members.Remove(member);
                OnMemberDeleted?.Invoke(member);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
