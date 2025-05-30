using MvvmCross.ViewModels;
using TitheSync.ApplicationState.Stores;
using TitheSync.Core.Parameters;
using TitheSync.Domain.Models;

namespace TitheSync.Core.Controls
{
    /// <summary>
    ///     Represents a control for managing modal navigation.
    /// </summary>
    /// <param name="modalNavigationStore" >The store that manages the current modal view model.</param>
    /// <param name="viewModelFactory" >A factory function to create view models.</param>
    public class ModalNavigationControl( IModalNavigationStore modalNavigationStore, Func<Type, object, MvxViewModel> viewModelFactory ):IModalNavigationControl
    {
        /// <summary>
        ///     Displays a modal popup with the specified view model type and an optional parameter.
        /// </summary>
        /// <typeparam name="TViewModel" >The type of the view model to display.</typeparam>
        /// <param name="parameter" >An optional parameter to pass to the view model.</param>
        public void PopUp<TViewModel>( int? parameter = null ) where TViewModel : IMvxViewModel
        {
            MvxViewModel viewModel = viewModelFactory(typeof(TViewModel), parameter!);
            modalNavigationStore.CurrentModalViewModel = viewModel;
        }

        /// <summary>
        ///     Displays a modal popup with the specified view model type and a list of selected members.
        /// </summary>
        /// <typeparam name="TViewModel" >The type of the view model to display.</typeparam>
        /// <param name="selectedMembers" >A list of members to pass to the view model.</param>
        public void PopUp<TViewModel>( List<Member> selectedMembers ) where TViewModel : IMvxViewModel
        {
            MvxViewModel viewModel = viewModelFactory(typeof(TViewModel), selectedMembers);
            modalNavigationStore.CurrentModalViewModel = viewModel;
        }

        /// <summary>
        ///     Displays a modal popup with the specified view model type and a navigation parameter.
        /// </summary>
        /// <param name="navigationParameter" > The parameter to pass to the view model.</param>
        /// <typeparam name="TViewModel" >The type of the view model to display.</typeparam>
        public void PopUp<TViewModel>( NavigationParameter navigationParameter ) where TViewModel : IMvxViewModel
        {
            MvxViewModel viewModel = viewModelFactory(typeof(TViewModel), navigationParameter);
            modalNavigationStore.CurrentModalViewModel = viewModel;
        }
    }
}
