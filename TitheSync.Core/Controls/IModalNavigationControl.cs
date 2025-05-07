using MvvmCross.ViewModels;
using TitheSync.Domain.Models;

namespace TitheSync.Core.Controls
{
    public interface IModalNavigationControl
    {
        /// <summary>
        ///     Displays a modal popup with the specified view model type and an optional parameter.
        /// </summary>
        /// <typeparam name="TViewModel" >The type of the view model to display.</typeparam>
        /// <param name="parameter" >An optional parameter to pass to the view model.</param>
        void PopUp<TViewModel>( int? parameter = null ) where TViewModel : IMvxViewModel;

        /// <summary>
        ///     Displays a modal popup with the specified view model type and a list of selected members.
        /// </summary>
        /// <typeparam name="TViewModel" >The type of the view model to display.</typeparam>
        /// <param name="selectedMembers" >A list of members to pass to the view model.</param>
        void PopUp<TViewModel>( List<Member> selectedMembers ) where TViewModel : IMvxViewModel;
    }
}
