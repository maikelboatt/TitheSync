using MvvmCross;
using MvvmCross.Exceptions;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System.Reflection;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels;

namespace TitheSync.Core
{
    /// <summary>
    ///     Represents the main application class for the TitheSync.Core project.
    ///     Inherits from MvxApplication and initializes the application.
    /// </summary>
    public class App:MvxApplication
    {
        /// <summary>
        ///     Initializes the application by registering ViewModels and services.
        /// </summary>
        public override void Initialize()
        {
            // Get the assembly containing the current type
            Assembly[] assemblies =
            [
                GetType().Assembly
            ];

            // Register all types ending with "ViewModel" in the assembly as dynamic types
            foreach (Assembly assembly in assemblies)
            {
                CreatableTypes(assembly)
                    .EndingWith("ViewModel")
                    .AsTypes()
                    .RegisterAsDynamic();
            }

            // Register the IViewModelFactory implementation
            Mvx.IoCProvider?.RegisterSingleton<IViewModelFactory>(new ViewModelFactory());

            // Register a factory function for creating ViewModel instances with parameters
            Mvx.IoCProvider?.RegisterSingleton<Func<Type, object, MvxViewModel>>(
                () => ( viewModelType, parameter ) =>
                {
                    // Resolve the ViewModel instance from the IoC container
                    MvxViewModel viewModel = (MvxViewModel)(Mvx.IoCProvider.Resolve(viewModelType)
                                                            ?? throw new MvxIoCResolveException($"Failed to resolve ViewModel of type: {viewModelType.FullName}"));

                    // Invoke the "Prepare" method on the ViewModel if it exists
                    viewModelType.GetMethod("Prepare", new[] { parameter.GetType() })
                                 ?.Invoke(viewModel, new[] { parameter });

                    // Initialize the ViewModel
                    viewModel.Initialize();
                    return viewModel;
                });

            // Call the base class's Initialize method
            base.Initialize();

            // Register the starting ViewModel for the application
            RegisterAppStart<ShellViewModel>();
        }
    }
}
