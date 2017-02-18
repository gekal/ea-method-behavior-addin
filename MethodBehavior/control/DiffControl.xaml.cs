using GitSharp;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MethodBehavior.control
{
    /// <summary>
    /// DiffControl.xaml の相互作用ロジック
    /// </summary>
    public partial class DiffControl : UserControl
    {
        #region "変数"
        /// <summary>
        /// 差分詳細
        /// </summary>
        public Diff Diff { get; private set; }
        #endregion

        #region "プロパティ"
        /// <summary>
        /// 左側のテキスト
        /// </summary>
        public string LeftText
        {
            get { return (string)GetValue(LeftTextProperty); }
            set { SetValue(LeftTextProperty, value); }
        }

        /// <summary>
        /// 依存プロパティ
        /// </summary>
        public static readonly DependencyProperty LeftTextProperty =
            DependencyProperty.Register("LeftText", typeof(string), typeof(DiffControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, LeftChanged));

        /// <summary>
        /// テキストチェンジイベント
        /// </summary>
        /// <param name="depObj">依存オブジェクト</param>
        /// <param name="e">e</param>
        public static void LeftChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DiffControl diffControl = (DiffControl)depObj;
            var left = (string)e.NewValue ?? string.Empty;
            var right = diffControl.RightText ?? string.Empty;
            // dv.Clear();
            diffControl.Init(new Diff(left, right));
        }

        /// <summary>
        /// 右側のテキスト
        /// </summary>
        public string RightText
        {
            get { return (string)GetValue(RightTextProperty); }
            set { SetValue(RightTextProperty, value); }
        }

        /// <summary>
        /// 依存プロパティ
        /// </summary>
        public static readonly DependencyProperty RightTextProperty =
            DependencyProperty.Register("RightText", typeof(string), typeof(DiffControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender, RightChanged));

        /// <summary>
        /// テキストチェンジイベント
        /// </summary>
        /// <param name="depObj">依存オブジェクト</param>
        /// <param name="e">e</param>
        public static void RightChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DiffControl diffControl = (DiffControl)depObj;
            var right = (string)e.NewValue ?? string.Empty;
            var left = diffControl.LeftText ?? string.Empty;
            diffControl.Init(new Diff(left, right));
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="diff">差分データ</param>
        public void Init(Diff diff)
        {
            Diff = diff;
            ListLeft.ItemsSource = diff.Sections;
            ListRight.ItemsSource = diff.Sections;
        }
        #endregion

        #region "コンストラクタ"
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DiffControl()
        {
            InitializeComponent();
        }
        #endregion

        #region "スクロールの同期化"
        /// <summary>
        /// スクロールの同期化
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == ScrollViewerLeft)
            {
                ScrollViewerRight.ScrollToVerticalOffset(ScrollViewerLeft.VerticalOffset);
                ScrollViewerRight.ScrollToHorizontalOffset(ScrollViewerLeft.HorizontalOffset);
            }
            else if (sender == ScrollViewerRight)
            {
                ScrollViewerLeft.ScrollToVerticalOffset(ScrollViewerRight.VerticalOffset);
                ScrollViewerLeft.ScrollToHorizontalOffset(ScrollViewerRight.HorizontalOffset);
            }
        }
        #endregion

        #region "水平位置の表示"
        /// <summary>
        /// 水平位置の表示
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StretchDataTemplate(object sender, RoutedEventArgs e)
        {
            var t = sender as FrameworkElement;
            if (t == null)
                return;
            var p = VisualTreeHelper.GetParent(t) as ContentPresenter;
            if (p == null)
                return;

            p.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        #endregion
    }

    #region "テキストのコンバーター"
    /// <summary>
    /// テキストのコンバーター
    /// </summary>
    public class BlockTextConverterLeft : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var section = value as Diff.Section;
            if (section == null)
                return 0;
            var a_lines = section.EndA - section.BeginA;
            var b_lines = section.EndB - section.BeginB;
            var line_difference = Math.Max(a_lines, b_lines) - a_lines;
            var s = new StringBuilder(Regex.Replace(section.TextA, "\r?\n$", ""));
            if (a_lines == 0)
                line_difference -= 1;
            for (var i = 0; i < line_difference; i++)
                s.AppendLine();
            return s.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// テキストのコンバーター
    /// </summary>
    public class BlockTextConverterRight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var section = value as Diff.Section;
            if (section == null)
                return 0;
            var a_lines = section.EndA - section.BeginA;
            var b_lines = section.EndB - section.BeginB;
            var line_difference = Math.Max(a_lines, b_lines) - b_lines;
            var s = new StringBuilder(Regex.Replace(section.TextB, "\r?\n$", ""));
            if (b_lines == 0)
                line_difference -= 1;
            for (var i = 0; i < line_difference; i++)
                s.AppendLine();
            return s.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region "ツールチップのコンバーター"
    /// <summary>
    /// ツールチップのコンバーター
    /// </summary>
    public class ToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var section = value as Diff.Section;
            if (section == null)
                return "異常";

            var toolTip = string.Empty;
            switch (section.EditWithRespectToA)
            {
                case Diff.EditType.Deleted:
                    return "削除";
                case Diff.EditType.Replaced:
                    return "変更";
                case Diff.EditType.Inserted:
                    return "追加";
                case Diff.EditType.Unchanged:
                default:
                    return "同じ";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region "背景色のコンバーター"
    /// <summary>
    /// 背景色のコンバーター
    /// </summary>
    public class BlockColorConverterLeft : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var section = value as Diff.Section;
            if (section == null)
                return Brushes.Pink;
            switch (section.EditWithRespectToA)
            {
                case Diff.EditType.Deleted:
                    return Brushes.LightSkyBlue;
                case Diff.EditType.Replaced:
                    return Brushes.LightSalmon;
                case Diff.EditType.Inserted:
                    return Brushes.DarkGray;
                case Diff.EditType.Unchanged:
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 背景色のコンバーター
    /// </summary>
    public class BlockColorConverterRight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var section = value as Diff.Section;
            if (section == null)
                return Brushes.Pink;
            switch (section.EditWithRespectToA)
            {
                case Diff.EditType.Deleted:
                    return Brushes.DarkGray;
                case Diff.EditType.Replaced:
                    return Brushes.LightSalmon;
                case Diff.EditType.Inserted:
                    return Brushes.LightGreen;
                case Diff.EditType.Unchanged:
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
