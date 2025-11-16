using System.Windows;
using System.Windows.Controls;

namespace DragAndDropAreaControl
{
    /// <summary>
    /// ファイル／フォルダのドラッグ＆ドロップと、
    /// ファイル／フォルダ選択ダイアログによるアップロードを行うためのカスタムコントロールです。
    /// </summary>
    public class DragAndDropArea : Control
    {
        static DragAndDropArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragAndDropArea), new FrameworkPropertyMetadata(typeof(DragAndDropArea)));
        }

        #region 依存関係プロパティ

        /// <summary>
        /// ドロップもしくはダイアログで選択されたファイル／フォルダのパス一覧を取得・設定します。
        /// </summary>
        public string[] DroppedFiles
        {
            get => (string[])GetValue(DroppedFilesProperty);
            set => SetValue(DroppedFilesProperty, value);
        }

        public static readonly DependencyProperty DroppedFilesProperty =
            DependencyProperty.Register(
                nameof(DroppedFiles),
                typeof(string[]),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 許可する拡張子をセミコロン区切りで指定します。
        /// 例: "*.png;*.jpg;*.csv"
        /// </summary>
        public string AllowedExtensions
        {
            get => (string)GetValue(AllowedExtensionsProperty);
            set => SetValue(AllowedExtensionsProperty, value);
        }

        public static readonly DependencyProperty AllowedExtensionsProperty =
            DependencyProperty.Register(
                nameof(AllowedExtensions),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// ファイルのドロップ／選択を許可するかどうかを示します。
        /// </summary>
        public bool AllowFile
        {
            get => (bool)GetValue(AllowFileProperty);
            set => SetValue(AllowFileProperty, value);
        }

        public static readonly DependencyProperty AllowFileProperty =
            DependencyProperty.Register(
                nameof(AllowFile),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// フォルダのドロップ／選択を許可するかどうかを示します。
        /// </summary>
        public bool AllowFolder
        {
            get => (bool)GetValue(AllowFolderProperty);
            set => SetValue(AllowFolderProperty, value);
        }

        public static readonly DependencyProperty AllowFolderProperty =
            DependencyProperty.Register(
                nameof(AllowFolder),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 複数ファイルのドロップ／選択を許可するかどうかを示します。
        /// </summary>
        public bool AllowMultipleFiles
        {
            get => (bool)GetValue(AllowMultipleFilesProperty);
            set => SetValue(AllowMultipleFilesProperty, value);
        }

        public static readonly DependencyProperty AllowMultipleFilesProperty =
            DependencyProperty.Register(
                nameof(AllowMultipleFiles),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// ファイル選択ボタンに表示するテキストを取得・設定します。
        /// </summary>
        public string FileDropText
        {
            get => (string)GetValue(FileDropTextProperty);
            set => SetValue(FileDropTextProperty, value);
        }

        public static readonly DependencyProperty FileDropTextProperty =
            DependencyProperty.Register(
                nameof(FileDropText),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata("ファイルを選択してください"));

        /// <summary>
        /// フォルダ選択ボタンに表示するテキストを取得・設定します。
        /// </summary>
        public string FolderDropText
        {
            get => (string)GetValue(FolderDropTextProperty);
            set => SetValue(FolderDropTextProperty, value);
        }

        public static readonly DependencyProperty FolderDropTextProperty =
            DependencyProperty.Register(
                nameof(FolderDropText),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata("フォルダを選択してください"));

        /// <summary>
        /// コントロール上部に表示するヘッダーテキストを取得・設定します。
        /// </summary>
        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register(
                nameof(HeaderText),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// ファイル選択ボタンに表示するアイコンを表す文字列を取得・設定します。
        /// 利用側で Path アイコン名やフォントアイコン文字列など、扱いやすい形式を自由に使用できます。
        /// </summary>
        public string FileDropIcon
        {
            get => (string)GetValue(FileDropIconProperty);
            set => SetValue(FileDropIconProperty, value);
        }

        public static readonly DependencyProperty FileDropIconProperty =
            DependencyProperty.Register(
                nameof(FileDropIcon),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// フォルダ選択ボタンに表示するアイコンを表す文字列を取得・設定します。
        /// </summary>
        public string FolderDropIcon
        {
            get => (string)GetValue(FolderDropIconProperty);
            set => SetValue(FolderDropIconProperty, value);
        }

        public static readonly DependencyProperty FolderDropIconProperty =
            DependencyProperty.Register(
                nameof(FolderDropIcon),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// バリデーションエラーなどが発生した際に表示するエラーメッセージを取得・設定します。
        /// </summary>
        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ErrorMessage),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// ファイル／フォルダがドラッグオーバーされている間に表示するオーバーレイの色を表す文字列を取得・設定します。
        /// 例: "#80FFFFFF"（ARGB 形式など）
        /// </summary>
        public string DragOverlayColor
        {
            get => (string)GetValue(DragOverlayColorProperty);
            set => SetValue(DragOverlayColorProperty, value);
        }

        public static readonly DependencyProperty DragOverlayColorProperty =
            DependencyProperty.Register(
                nameof(DragOverlayColor),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata("#40FFFFFF"));

        /// <summary>
        /// ドロップ完了後に表示するオーバーレイの色を表す文字列を取得・設定します。
        /// </summary>
        public string AfterDropColor
        {
            get => (string)GetValue(AfterDropColorProperty);
            set => SetValue(AfterDropColorProperty, value);
        }

        public static readonly DependencyProperty AfterDropColorProperty =
            DependencyProperty.Register(
                nameof(AfterDropColor),
                typeof(string),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata("#4000FF00"));


        #endregion
    }
}