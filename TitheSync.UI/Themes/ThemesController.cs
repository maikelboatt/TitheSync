using System.Windows;

namespace TitheSync.UI.Themes
{
    public class ThemesController
    {
        public static ThemeTypes CurrentTheme { get; set; }
        public static ResourceDictionary ThemeDictionary
        {
            get => Application.Current.Resources.MergedDictionaries[0];
            set => Application.Current.Resources.MergedDictionaries[0] = value;
        }

        private static void ChangeTheme( Uri uri )
        {
            ThemeDictionary = new ResourceDictionary { Source = uri };
        }

        public static void SetTheme( ThemeTypes theme )
        {
            string themeName = null!;
            CurrentTheme = theme;

            themeName = theme switch
            {
                ThemeTypes.Light => "LightTheme",
                ThemeTypes.Dark  => "DarkTheme",
                _                => themeName
            };

            try
            {
                if (!string.IsNullOrEmpty(themeName))
                {
                    ChangeTheme(new Uri($"Themes/{themeName}.xaml", UriKind.Relative));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
