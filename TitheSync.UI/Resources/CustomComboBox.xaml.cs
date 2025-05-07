using MvvmCross.Platforms.Wpf.Views;
using System.Windows;

namespace TitheSync.UI.Resources
{
    /// <summary>
    ///     A custom WPF control that extends `MvxWpfView` and provides additional properties
    ///     for data binding and validation.
    /// </summary>
    public partial class CustomComboBox:MvxWpfView
    {
        /// <summary>
        ///     DependencyProperty for the data to be displayed in the ComboBox.
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(object),
            typeof(CustomComboBox),
            new PropertyMetadata(default(object)));

        /// <summary>
        ///     DependencyProperty for the placeholder text displayed when no item is selected.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(CustomComboBox),
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     DependencyProperty for the currently selected value in the ComboBox.
        /// </summary>
        public static readonly DependencyProperty ChosenValueProperty = DependencyProperty.Register(
            nameof(ChosenValue),
            typeof(object),
            typeof(CustomComboBox),
            new PropertyMetadata(default(object)));

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomComboBox" /> class.
        /// </summary>
        public CustomComboBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the currently selected value in the ComboBox.
        /// </summary>
        public object ChosenValue
        {
            get => GetValue(ChosenValueProperty);
            set => SetValue(ChosenValueProperty, value);
        }

        /// <summary>
        ///     Gets or sets the placeholder text displayed when no item is selected.
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        ///     Gets or sets the data to be displayed in the ComboBox.
        /// </summary>
        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}
