using MvvmCross.Commands;
using MvvmCross.Platforms.Wpf.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TitheSync.UI.Customs
{
    public partial class NavigationItem:MvxWpfView
    {
        public NavigationItem()
        {
            InitializeComponent();
        }

        /// DependencyProperty for the Icon property
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(PathGeometry),
            typeof(NavigationItem),
            new PropertyMetadata(default(PathGeometry)));

        public PathGeometry Icon
        {
            get => (PathGeometry)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        // Dependency Property for the IconWidth property
        public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register(
            nameof(IconWidth),
            typeof(int),
            typeof(NavigationItem),
            new PropertyMetadata(default(int)));

        public int IconWidth
        {
            get => (int)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }
        
        public SolidColorBrush IndicatorBrush
        {
            get => (SolidColorBrush)GetValue(IndicatorBrushProperty);
            set => SetValue(IndicatorBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for IndicatorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorBrushProperty =
            DependencyProperty.Register(nameof(IndicatorBrush), typeof(SolidColorBrush), typeof(NavigationItem));


        public int IndicatorIndicatorCornerRadius
        {
            get => (int)GetValue(IndicatorIndicatorCornerRadiusProperty);
            set => SetValue(IndicatorIndicatorCornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for IndicatorIndicatorCornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorIndicatorCornerRadiusProperty =
            DependencyProperty.Register(nameof(IndicatorIndicatorCornerRadius), typeof(int), typeof(NavigationItem));


        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(NavigationItem));


        public new Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Padding.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(NavigationItem));


        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(NavigationItem));


        public string GroupName
        {
            get => (string)GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(NavigationItem));


        public static readonly DependencyProperty CommandNameProperty = DependencyProperty.Register(
            nameof(CommandName),
            typeof(MvxCommand),
            typeof(NavigationItem));

        public MvxCommand CommandName
        {
            get => (MvxCommand)GetValue(CommandNameProperty);
            set => SetValue(CommandNameProperty, value);
        }
    }
}
