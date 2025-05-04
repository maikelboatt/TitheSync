using Microsoft.Extensions.Logging;
using MvvmCross.ViewModels;
using TitheSync.Domain.Enums;
using TitheSync.Domain.Models;
using TitheSync.Domain.Services;

namespace TitheSync.Core.ViewModels.Members
{
    public class MemberListingViewModel( IMemberService memberService, ILogger<MemberListingViewModel> logger ):MvxViewModel
    {
        private IEnumerable<Member> members;

        public IEnumerable<Member> Members
        {
            get => members;
            set => SetProperty(ref members, value);
        }

        public int MemberCount => Members?.Count() ?? 0;

        public override async Task Initialize()
        {
            await base.Initialize();
            // await CreateMemberAsync();
            await LoadMembersAsync();
        }

        private async Task CreateMemberAsync()
        {
            Member newMember = new(
                0,
                "Nessa",
                "Boatt",
                "0541047330",
                false,
                "Methodist Street",
                OrganizationEnum.Choir,
                BibleClassEnum.EmeliaAkrofi);

            await memberService.AddMemberAsync(newMember);
            logger.LogInformation("Member created: {Member}", newMember);
        }

        private async Task LoadMembersAsync()
        {
            Members = await memberService.GetMembersAsync();
            logger.LogInformation("Members loaded: {MemberCount}, {Members}", MemberCount, Members);
        }
    }
}
