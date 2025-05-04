using TitheSync.Domain.Enums;

namespace TitheSync.DataAccess.DTO
{
    /// <summary>
    ///     Represents a data transfer object for a member.
    /// </summary>
    public record MemberDto
    {
        // Constructor for materialization
        public MemberDto() { }

        /// <summary>
        ///     Primary constructor
        /// </summary>
        /// <param name="memberId" >The unique identifier of the member.</param>
        /// <param name="firstName" >The first name of the member.</param>
        /// <param name="lastName" >The last name of the member.</param>
        /// <param name="contact" >The contact information of the member.</param>
        /// <param name="isLeader" >Indicates whether the member is a leader.</param>
        /// <param name="address" >The address of the member.</param>
        /// <param name="organization" >The organization the member belongs to.</param>
        /// <param name="bibleClass" >The Bible class the member attends.</param>
        public MemberDto( int memberId, string firstName, string lastName, string contact, bool isLeader, string address, OrganizationEnum organization, BibleClassEnum bibleClass )
        {
            MemberId = memberId;
            FirstName = firstName;
            LastName = lastName;
            Contact = contact;
            IsLeader = isLeader;
            Address = address;
            Organization = organization;
            BibleClass = bibleClass;
        }

        public int MemberId { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Contact { get; init; }
        public bool IsLeader { get; init; }
        public string Address { get; init; }
        public OrganizationEnum Organization { get; init; }
        public BibleClassEnum BibleClass { get; init; }
    }
}
