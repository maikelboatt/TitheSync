using MvvmCross.ViewModels;
using TitheSync.Core.ViewModels;

namespace TitheSync.Core
{
    public class App:MvxApplication
    {
        public override void Initialize()
        {
            base.Initialize(); 
            
            RegisterAppStart<ShellViewModel>();
        }
    }
}
