using TitheSync.Domain.Enums;

namespace TitheSync.Domain.Models
{
    /// <summary>
    ///     Represents a member with personal and organizational details.
    /// </summary>
    /// <param name="memberId" >The unique identifier for the member.</param>
    /// <param name="firstName" >The first name of the member.</param>
    /// <param name="lastName" >The last name of the member.</param>
    /// <param name="contact" >The contact information of the member.</param>
    /// <param name="gender" >The gender of the member.</param>
    /// <param name="isLeader" >Indicates whether the member is a leader.</param>
    /// <param name="address" >The address of the member.</param>
    /// <param name="organization" >The organization the member belongs to.</param>
    /// <param name="bibleClass" >The Bible class the member is part of.</param>
    public class Member(
        int memberId,
        string firstName,
        string lastName,
        string contact,
        string gender,
        bool isLeader,
        string address,
        OrganizationEnum organization,
        BibleClassEnum bibleClass )
    {
        /// <summary>
        ///     Gets the unique identifier for the member.
        /// </summary>
        public int MemberId { get; init; } = memberId;

        /// <summary>
        ///     Gets the first name of the member.
        /// </summary>
        public string FirstName { get; init; } = firstName;

        /// <summary>
        ///     Gets the last name of the member.
        /// </summary>
        public string LastName { get; init; } = lastName;

        /// <summary>
        ///     Gets the contact information of the member.
        /// </summary>
        public string Contact { get; init; } = contact;

        /// <summary>
        ///     Gets the gender of the member
        /// </summary>
        public string Gender { get; init; } = gender;

        /// <summary>
        ///     Gets a value indicating whether the member is a leader.
        /// </summary>
        public bool IsLeader { get; init; } = isLeader;

        /// <summary>
        ///     Gets the address of the member.
        /// </summary>
        public string Address { get; init; } = address;

        /// <summary>
        ///     Gets the organization the member belongs to.
        /// </summary>
        public OrganizationEnum Organization { get; init; } = organization;

        /// <summary>
        ///     Gets the Bible class the member is part of.
        /// </summary>
        public BibleClassEnum BibleClass { get; init; } = bibleClass;

        /// <summary>
        ///     Gets or sets a value indicating whether the member is selected.
        /// </summary>
        public bool IsSelected { get; set; } = false;

        public override string ToString() =>
            $"MemberId: {MemberId}, Firstname: {FirstName}, Lastname: {LastName}, Contact: {Contact}, IsLeader: {IsLeader}, Address: {Address}, Organization: {Organization}, BibleClass: {BibleClass}";
    }
}
