using MvvmCross.Platforms.Wpf.Views;
using System.Windows;

namespace TitheSync.UI.Resources
{
    /// <summary>
    ///     A custom WPF control that extends `MvxWpfView` and provides additional properties
    ///     for data binding and validation.
    /// </summary>
    public partial class CustomIntegerBox:MvxWpfView
    {
        /// <summary>
        ///     Dependency property for binding a string value to the control.
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(string),
            typeof(CustomIntegerBox),
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     Dependency property for specifying the largest integer value allowed.
        /// </summary>
        public static readonly DependencyProperty BiggestProperty = DependencyProperty.Register(
            nameof(Biggest),
            typeof(int),
            typeof(CustomIntegerBox),
            new PropertyMetadata(default(int)));

        /// <summary>
        ///     Dependency property for specifying the smallest integer value allowed.
        /// </summary>
        public static readonly DependencyProperty SmallestProperty = DependencyProperty.Register(
            nameof(Smallest),
            typeof(int),
            typeof(CustomIntegerBox),
            new PropertyMetadata(default(int)));

        /// <summary>
        ///     Dependency property for binding a description to the control.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(CustomIntegerBox),
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomIntegerBox" /> class.
        /// </summary>
        public CustomIntegerBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the largest integer value allowed.
        /// </summary>
        public int Biggest
        {
            get => (int)GetValue(BiggestProperty);
            set => SetValue(BiggestProperty, value);
        }

        /// <summary>
        ///     Gets or sets the smallest integer value allowed.
        /// </summary>
        public int Smallest
        {
            get => (int)GetValue(SmallestProperty);
            set => SetValue(SmallestProperty, value);
        }

        /// <summary>
        ///     Gets or sets the string data bound to the control.
        /// </summary>
        public string Data
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        ///     Gets or sets the description bound to the control.
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
    }
}
