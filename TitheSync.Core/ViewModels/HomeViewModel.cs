using MvvmCross.ViewModels;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels.Members;

namespace TitheSync.Core.ViewModels
{
    public class HomeViewModel( IViewModelFactory viewModelFactory ):MvxViewModel, IHomeViewModel
    {
        public MemberListingViewModel? MemberListingViewModel => CreateViewModel();

        private MemberListingViewModel? CreateViewModel()
        {
            MemberListingViewModel? viewModel = viewModelFactory.CreateViewModel<MemberListingViewModel>();
            viewModel?.Initialize();
            return viewModel;
        }
    }
}
