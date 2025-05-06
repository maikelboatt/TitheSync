using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Exceptions;
using MvvmCross.IoC;
using MvvmCross.ViewModels;
using System.Reflection;
using TitheSync.Core.Factory;
using TitheSync.Core.ViewModels;
using TitheSync.DataAccess.DatabaseAccess;
using TitheSync.DataAccess.Repositories;
using TitheSync.Domain.Repositories;

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
            // Load appsettings.json configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                               .SetBasePath(AppContext.BaseDirectory)
                                               .AddJsonFile("appsettings.json", false, true)
                                               .Build();

            // Register the IConfiguration instance
            Mvx.IoCProvider?.RegisterSingleton(configuration);

            // Register ISqlDataAccess with the connection string
            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<ISqlDataAccess>(() =>
            {
                ILogger<SqlDataAccess> logger = Mvx.IoCProvider.Resolve<ILogger<SqlDataAccess>>();
                return new SqlDataAccess(configuration.GetConnectionString("DefaultConnection"), logger);
            });

            // Get the assembly containing the current type
            Assembly[] assemblies =
            {
                GetType().Assembly,
                typeof(IMemberRepository).Assembly,
                typeof(MemberRepository).Assembly
            };

            // Register all types ending with "ViewModel" in the assembly as dynamic types
            foreach (Assembly assembly in assemblies)
            {
                // Dynamically registers all types in the specified assembly that have names ending with "Services".
                // These types are registered as interfaces and as lazy singletons in the IoC container.
                CreatableTypes(assembly)
                    .EndingWith("Service")
                    .AsInterfaces()
                    .RegisterAsLazySingleton();

                // Dynamically registers all types in the specified assembly that have names ending with "Repository".
                // These types are registered as interfaces and as lazy singletons in the IoC container.
                CreatableTypes(assembly)
                    .EndingWith("Repository")
                    .AsInterfaces()
                    .RegisterAsLazySingleton();

                // Dynamically registers all types in the specified assembly that have names ending with "ViewModel".
                // These types are registered as dynamic types in the IoC container.
                CreatableTypes(assembly)
                    .EndingWith("ViewModel")
                    .AsTypes()
                    .RegisterAsDynamic();
            }

            // Register the IViewModelFactory implementation
            Mvx.IoCProvider?.RegisterSingleton<IViewModelFactory>(new ViewModelFactory());

            // Register a factory function for creating ViewModel instances with parameters
            Mvx.IoCProvider?.RegisterSingleton<Func<Type, object, MvxViewModel>>(() => ( viewModelType, parameter ) =>
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
