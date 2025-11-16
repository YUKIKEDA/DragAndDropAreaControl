using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace DragAndDropAreaControl
{
    /// <summary>
    /// ファイル／フォルダのドラッグ＆ドロップと、
    /// ファイル／フォルダ選択ダイアログによるアップロードを行うためのカスタムコントロールです。
    /// </summary>
    public class DragAndDropArea : Control
    {
        public DragAndDropArea()
        {
            AllowDrop = true;
            _fileItemClearHandler = OnFileItemClearClick;
        }

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
        /// 複数フォルダのドロップ／選択を許可するかどうかを示します。
        /// </summary>
        public bool AllowMultipleFolders
        {
            get => (bool)GetValue(AllowMultipleFoldersProperty);
            set => SetValue(AllowMultipleFoldersProperty, value);
        }

        public static readonly DependencyProperty AllowMultipleFoldersProperty =
            DependencyProperty.Register(
                nameof(AllowMultipleFolders),
                typeof(bool),
                typeof(DragAndDropArea),
                // 既存仕様と互換性を保つため、デフォルトは false（フォルダは合計 1 つのみ）
                new FrameworkPropertyMetadata(false));

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

        /// <summary>
        /// ドラッグオーバー状態かどうかを示します。
        /// </summary>
        public bool IsDragOver
        {
            get => (bool)GetValue(IsDragOverProperty);
            internal set => SetValue(IsDragOverProperty, value);
        }

        public static readonly DependencyProperty IsDragOverProperty =
            DependencyProperty.Register(
                nameof(IsDragOver),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// ドロップ完了状態かどうかを示します。
        /// </summary>
        public bool IsDropped
        {
            get => (bool)GetValue(IsDroppedProperty);
            internal set => SetValue(IsDroppedProperty, value);
        }

        public static readonly DependencyProperty IsDroppedProperty =
            DependencyProperty.Register(
                nameof(IsDropped),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(false));

        /// <summary>
        /// エラー状態かどうかを示します。
        /// </summary>
        public bool HasError
        {
            get => (bool)GetValue(HasErrorProperty);
            internal set => SetValue(HasErrorProperty, value);
        }

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register(
                nameof(HasError),
                typeof(bool),
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(false));


        #endregion

        #region テンプレート適用・ボタンイベント

        private const string PART_FileButton = "PART_FileButton";
        private const string PART_FolderButton = "PART_FolderButton";
        private const string PART_ClearButton = "PART_ClearButton";
        private const string PART_FileList = "PART_FileList";

        private Button? _fileButton;
        private Button? _folderButton;
        private Button? _clearButton;
        private ItemsControl? _fileList;
        private readonly RoutedEventHandler _fileItemClearHandler;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_fileButton != null)
            {
                _fileButton.Click -= OnFileButtonClick;
            }

            if (_folderButton != null)
            {
                _folderButton.Click -= OnFolderButtonClick;
            }

            if (_clearButton != null)
            {
                _clearButton.Click -= OnClearButtonClick;
            }

            if (_fileList != null)
            {
                _fileList.RemoveHandler(Button.ClickEvent, _fileItemClearHandler);
            }

            _fileButton = GetTemplateChild(PART_FileButton) as Button;
            _folderButton = GetTemplateChild(PART_FolderButton) as Button;
            _clearButton = GetTemplateChild(PART_ClearButton) as Button;
            _fileList = GetTemplateChild(PART_FileList) as ItemsControl;

            if (_fileButton != null)
            {
                _fileButton.Click += OnFileButtonClick;
            }

            if (_folderButton != null)
            {
                _folderButton.Click += OnFolderButtonClick;
            }

            if (_clearButton != null)
            {
                _clearButton.Click += OnClearButtonClick;
            }

            if (_fileList != null)
            {
                _fileList.AddHandler(Button.ClickEvent, _fileItemClearHandler, true);
            }
        }

        private void OnFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = AllowMultipleFiles,
                Filter = BuildOpenFileDialogFilter()
            };

            if (dialog.ShowDialog() == true)
            {
                HandleSelectedPaths(dialog.FileNames);
            }
        }

        private void OnFolderButtonClick(object? sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                // AllowMultipleFolders に応じてダイアログ側の複数選択可否を切り替える
                Multiselect = AllowMultipleFolders,
                Title = "フォルダを選択してください"
            };

            if (dialog.ShowDialog() == true)
            {
                string[]? paths = null;

                if (AllowMultipleFolders && dialog.FolderNames is { Length: > 0 } multi)
                {
                    // 複数フォルダ選択時: すべてを対象とする
                    paths = multi;
                }
                else if (!string.IsNullOrWhiteSpace(dialog.FolderName))
                {
                    // 単一フォルダ選択時
                    paths = [dialog.FolderName];
                }

                if (paths is null || paths.Length == 0)
                {
                    return;
                }

                if (!ValidatePathsForDrop(paths, out var error))
                {
                    HasError = true;
                    ErrorMessage = error;
                    IsDropped = false;
                    return;
                }

                HandleSelectedPaths(paths);
                IsDropped = true;
                HasError = false;
                ErrorMessage = string.Empty;
            }
        }

        private void OnClearButtonClick(object? sender, RoutedEventArgs e)
        {
            DroppedFiles = [];
            ErrorMessage = string.Empty;
            IsDropped = false;
            HasError = false;
        }

        private void OnFileItemClearClick(object? sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Button button)
            {
                return;
            }

            if (button.DataContext is not string path || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (DroppedFiles is null || DroppedFiles.Length == 0)
            {
                return;
            }

            var newFiles = DroppedFiles.Where(p => !string.Equals(p, path, StringComparison.OrdinalIgnoreCase)).ToArray();
            DroppedFiles = newFiles;

            if (newFiles.Length == 0)
            {
                IsDropped = false;
                HasError = false;
                ErrorMessage = string.Empty;
            }
        }

        #endregion

        #region ドラッグ＆ドロップ処理

        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            IsDragOver = true;
            HasError = false;
            ErrorMessage = string.Empty;
            ValidateDragData(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            ValidateDragData(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            IsDragOver = false;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            IsDragOver = false;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (!ValidatePathsForDrop(paths, out var error))
            {
                // すでに有効なファイル／フォルダが設定されている場合は、
                // 一旦それらをクリアしたうえでエラー状態として扱う。
                if (DroppedFiles is { Length: > 0 })
                {
                    DroppedFiles = [];
                }

                HasError = true;
                ErrorMessage = error;
                IsDropped = false;
                e.Effects = DragDropEffects.None;
                return;
            }

            HandleSelectedPaths(paths);
            IsDropped = true;
            HasError = false;
            ErrorMessage = string.Empty;
            e.Effects = DragDropEffects.Copy;
        }

        private void ValidateDragData(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var paths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (ValidatePathsForDrop(paths, out _))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private bool ValidatePathsForDrop(string[] paths, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (paths.Length == 0)
            {
                errorMessage = "ファイルまたはフォルダが見つかりません。";
                return false;
            }

            if (!AllowMultipleFiles && paths.Length > 1)
            {
                errorMessage = "複数のファイル／フォルダは選択できません。";
                return false;
            }

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    if (!AllowFile)
                    {
                        errorMessage = "ファイルの選択は許可されていません。";
                        return false;
                    }

                    if (!IsExtensionAllowed(path))
                    {
                        errorMessage = "許可されていない拡張子のファイルが含まれています。";
                        return false;
                    }
                }
                else if (Directory.Exists(path))
                {
                    if (!AllowFolder)
                    {
                        errorMessage = "フォルダの選択は許可されていません。";
                        return false;
                    }
                }
                else
                {
                    errorMessage = "存在しないファイルまたはフォルダが含まれています。";
                    return false;
                }
            }

            return true;
        }

        private bool IsExtensionAllowed(string filePath)
        {
            if (string.IsNullOrWhiteSpace(AllowedExtensions))
            {
                // 制限なし
                return true;
            }

            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }

            var patterns = AllowedExtensions
                .Split([';'], StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToArray();

            if (patterns.Length == 0)
            {
                return true;
            }

            foreach (var pattern in patterns)
            {
                if (pattern == "*" || pattern == "*.*")
                {
                    return true;
                }

                var normalized = pattern;
                if (normalized.StartsWith("*.", StringComparison.Ordinal))
                {
                    normalized = normalized.Substring(1); // "*.png" -> ".png"
                }
                else if (!normalized.StartsWith('.'))
                {
                    normalized = "." + normalized;
                }

                if (string.Equals(normalized, ext, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void HandleSelectedPaths(string[] paths)
        {
            if (paths is null || paths.Length == 0)
            {
                return;
            }

            var current = DroppedFiles ?? Array.Empty<string>();

            // 現在のファイル／フォルダを分解
            var currentFiles   = current.Where(File.Exists);
            var currentFolders = current.Where(Directory.Exists).ToArray();

            // 今回追加されたファイル／フォルダを分解
            var newFiles   = paths.Where(File.Exists).ToArray();
            var newFolders = paths.Where(Directory.Exists).ToArray();

            // ---------- ファイルのマージ ----------
            IEnumerable<string> nextFiles;

            if (AllowMultipleFiles)
            {
                // 既存 + 新規をマージ（重複は除外）
                nextFiles = currentFiles
                    .Concat(newFiles)
                    .Distinct(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                // 単一ファイルのみ許可: 今回のファイルがあればその1件で上書き。
                // （今回のドロップにファイルが含まれない場合は既存のファイルを維持）
                var fileToKeep = newFiles.FirstOrDefault() ?? currentFiles.FirstOrDefault();
                nextFiles = string.IsNullOrEmpty(fileToKeep)
                    ? Enumerable.Empty<string>()
                    : new[] { fileToKeep };
            }

            // ---------- フォルダのマージ ----------
            IEnumerable<string> nextFolders;

            if (AllowMultipleFolders)
            {
                // フォルダも複数許可: 既存 + 新規をマージ（重複は除外）
                nextFolders = currentFolders
                    .Concat(newFolders)
                    .Distinct(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                // フォルダは常に合計 1 つだけ:
                // 今回のフォルダがあればそれで上書き、なければ既存を維持。
                var folderToKeep = newFolders.LastOrDefault() ?? currentFolders.LastOrDefault();
                nextFolders = string.IsNullOrEmpty(folderToKeep)
                    ? Enumerable.Empty<string>()
                    : new[] { folderToKeep };
            }

            // ファイル + フォルダを結合して DroppedFiles に反映
            DroppedFiles = nextFiles
                .Concat(nextFolders)
                .ToArray();
        }

        private string BuildOpenFileDialogFilter()
        {
            if (string.IsNullOrWhiteSpace(AllowedExtensions))
            {
                return "すべてのファイル|*.*";
            }

            return $"許可されたファイル|{AllowedExtensions}";
        }

        #endregion
    }
}