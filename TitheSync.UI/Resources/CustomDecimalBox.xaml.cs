using MvvmCross.Platforms.Wpf.Views;
using System.Windows;

namespace TitheSync.UI.Resources
{
    /// <summary>
    ///     A custom WPF control for decimal input with validation for minimum and maximum values.
    /// </summary>
    public partial class CustomDecimalBox:MvxWpfView
    {
        /// <summary>
        ///     Identifies the Data dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(string),
            typeof(CustomDecimalBox),
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(CustomDecimalBox),
            new PropertyMetadata(default(string)));

        /// <summary>
        ///     Identifies the Smallest dependency property, representing the minimum allowed value.
        /// </summary>
        public static readonly DependencyProperty SmallestProperty = DependencyProperty.Register(
            nameof(Smallest),
            typeof(int),
            typeof(CustomDecimalBox),
            new PropertyMetadata(0));

        /// <summary>
        ///     Identifies the Biggest dependency property, representing the maximum allowed value.
        /// </summary>
        public static readonly DependencyProperty BiggestProperty = DependencyProperty.Register(
            nameof(Biggest),
            typeof(int),
            typeof(CustomDecimalBox),
            new PropertyMetadata(0));

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomDecimalBox" /> class.
        /// </summary>
        public CustomDecimalBox()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Gets or sets the decimal data as a string.
        /// </summary>
        public string Data
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        /// <summary>
        ///     Gets or sets the description for the decimal box.
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        /// <summary>
        ///     Gets or sets the minimum allowed value.
        /// </summary>
        public int Smallest
        {
            get => (int)GetValue(SmallestProperty);
            set => SetValue(SmallestProperty, value);
        }

        /// <summary>
        ///     Gets or sets the maximum allowed value.
        /// </summary>
        public int Biggest
        {
            get => (int)GetValue(BiggestProperty);
            set => SetValue(BiggestProperty, value);
        }
    }
}
