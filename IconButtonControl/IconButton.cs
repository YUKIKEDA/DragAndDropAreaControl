using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IconButtonControl
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IconButtonControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:IconButtonControl;assembly=IconButtonControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class IconButton : Button
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        #region 依存関係プロパティ

        /// <summary>
        /// ボタンに表示するベクターアイコンを取得・設定します。
        /// Path の Data として使用される Geometry を想定しています。
        /// </summary>
        public Geometry Icon
        {
            get => (Geometry)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(
                nameof(Icon),
                typeof(Geometry),
                typeof(IconButton),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// アイコンの表示サイズを取得・設定します。
        /// </summary>
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(double),
                typeof(IconButton),
                new FrameworkPropertyMetadata(32.0));

        /// <summary>
        /// ボタンに表示するテキストを取得・設定します。
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(IconButton),
                new FrameworkPropertyMetadata(string.Empty));

        #endregion
    }
}