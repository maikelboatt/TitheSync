namespace TitheSync.DataAccess.DTO
{
    /// <summary>
    ///     Represents a data transfer object for a member.
    /// </summary>
    /// <param name="MemberId" >The unique identifier of the member.</param>
    /// <param name="FirstName" >The first name of the member.</param>
    /// <param name="LastName" >The last name of the member.</param>
    /// <param name="Contact" >The contact information of the member.</param>
    /// <param name="IsLeader" >Indicates whether the member is a leader.</param>
    /// <param name="Address" >The address of the member.</param>
    /// <param name="Organization" >The organization the member belongs to.</param>
    /// <param name="BibleClass" >The Bible class the member attends.</param>
    public record MemberDto(
        int MemberId,
        string FirstName,
        string LastName,
        string Contact,
        bool IsLeader,
        string Address,
        string Organization,
        string BibleClass );
}
