using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace DragAndDropAreaControl
{
    public class DragAndDropArea : Control
    {
        static DragAndDropArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DragAndDropArea),
                new FrameworkPropertyMetadata(typeof(DragAndDropArea)));
        }

        public static readonly RoutedCommand ClearCommand = new();

        public DragAndDropArea()
        {
            this.AllowDrop = true;
            this.Drop += DragAndDropArea_Drop;
            this.DragEnter += DragAndDropArea_DragEnter;
            this.DragLeave += DragAndDropArea_DragLeave;
            
            CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Open,
                OpenFileDialog_Execute));
            CommandBindings.Add(new CommandBinding(
                ClearCommand,
                ClearFiles_Execute,
                CanExecuteClearFiles));
        }

        private void DragAndDropArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                if (!AllowMultipleFiles && files.Length > 1)
                {
                    ErrorMessage = "複数のファイルを同時に選択することはできません。";
                    IsDraggingOver = false;
                    return;
                }

                string[] validFiles = files.Where(IsValidFile).ToArray();
                if (validFiles.Length > 0)
                {
                    OnFilesDropped(validFiles);
                }
            }
            IsDraggingOver = false;
        }

        private void DragAndDropArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                IsDraggingOver = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DragAndDropArea_DragLeave(object sender, DragEventArgs e)
        {
            IsDraggingOver = false;
        }

        private void OpenFileDialog_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = AllowMultipleFiles
            };

            if (AllowedExtensions.Length > 0)
            {
                dialog.Filter = string.Join("|", AllowedExtensions
                    .Select(ext => $"{ext.TrimStart('.')}ファイル (*{ext})|*{ext}"));
            }

            if (dialog.ShowDialog() == true)
            {
                if (!AllowMultipleFiles && dialog.FileNames.Length > 1)
                {
                    ErrorMessage = "複数のファイルを同時に選択することはできません。";
                    return;
                }

                OnFilesDropped(dialog.FileNames);
            }
        }

        private void ClearFiles_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DroppedFiles = [];
        }

        private void CanExecuteClearFiles(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DroppedFiles.Length > 0;
        }

        // ファイルがドロップされた時のイベント
        public delegate void FileDroppedEventHandler(string[] files);
        public event FileDroppedEventHandler? FilesDropped;

        public static readonly DependencyProperty DroppedFilesProperty =
            DependencyProperty.Register(
                nameof(DroppedFiles),
                typeof(string[]),
                typeof(DragAndDropArea),
                new PropertyMetadata(Array.Empty<string>()));

        public string[] DroppedFiles
        {
            get => (string[])GetValue(DroppedFilesProperty);
            private set => SetValue(DroppedFilesProperty, value);
        }

        protected virtual void OnFilesDropped(string[] files)
        {
            DroppedFiles = files;  // プロパティを更新
            FilesDropped?.Invoke(files);
        }

        public static readonly DependencyProperty AllowedExtensionsProperty =
            DependencyProperty.Register(
                nameof(AllowedExtensions),
                typeof(string[]),
                typeof(DragAndDropArea),
                new PropertyMetadata(Array.Empty<string>()));

        public static readonly DependencyProperty AllowFoldersProperty =
            DependencyProperty.Register(
                nameof(AllowFolders),
                typeof(bool),
                typeof(DragAndDropArea),
                new PropertyMetadata(true));

        public static readonly DependencyProperty AllowFilesProperty =
            DependencyProperty.Register(
                nameof(AllowFiles),
                typeof(bool),
                typeof(DragAndDropArea),
                new PropertyMetadata(true));

        public string[] AllowedExtensions
        {
            get => (string[])GetValue(AllowedExtensionsProperty);
            set => SetValue(AllowedExtensionsProperty, value);
        }

        public bool AllowFolders
        {
            get => (bool)GetValue(AllowFoldersProperty);
            set => SetValue(AllowFoldersProperty, value);
        }

        public bool AllowFiles
        {
            get => (bool)GetValue(AllowFilesProperty);
            set => SetValue(AllowFilesProperty, value);
        }

        public static readonly DependencyProperty MaxFileSizeProperty =
            DependencyProperty.Register(
                nameof(MaxFileSize),
                typeof(long),
                typeof(DragAndDropArea),
                new PropertyMetadata(long.MaxValue));

        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register(
                nameof(ErrorMessage),
                typeof(string),
                typeof(DragAndDropArea),
                new PropertyMetadata(string.Empty));

        public long MaxFileSize
        {
            get => (long)GetValue(MaxFileSizeProperty);
            set => SetValue(MaxFileSizeProperty, value);
        }

        public string ErrorMessage
        {
            get => (string)GetValue(ErrorMessageProperty);
            private set => SetValue(ErrorMessageProperty, value);
        }

        public static readonly DependencyProperty AllowMultipleFilesProperty =
            DependencyProperty.Register(
                nameof(AllowMultipleFiles),
                typeof(bool),
                typeof(DragAndDropArea),
                new PropertyMetadata(true));

        public bool AllowMultipleFiles
        {
            get => (bool)GetValue(AllowMultipleFilesProperty);
            set => SetValue(AllowMultipleFilesProperty, value);
        }

        private bool IsValidPath(string path)
        {
            bool isFile = File.Exists(path);
            bool isDirectory = Directory.Exists(path);

            if (isFile && !AllowFiles) return false;
            if (isDirectory && !AllowFolders) return false;

            if (isFile && AllowedExtensions.Length > 0)
            {
                string extension = Path.GetExtension(path).ToLower();
                return AllowedExtensions.Contains(extension.ToLower());
            }

            return true;
        }

        private bool IsValidFile(string path)
        {
            if (!IsValidPath(path)) 
            {
                ErrorMessage = "指定された拡張子のファイルのみ許可されています。";
                return false;
            }

            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Length > MaxFileSize)
                {
                    ErrorMessage = $"ファイルサイズが制限（{MaxFileSize / 1024 / 1024}MB）を超えています。";
                    return false;
                }
            }

            ErrorMessage = string.Empty;
            return true;
        }

        public static readonly DependencyProperty IsDraggingOverProperty =
            DependencyProperty.Register(
                nameof(IsDraggingOver),
                typeof(bool),
                typeof(DragAndDropArea),
                new PropertyMetadata(false));

        public bool IsDraggingOver
        {
            get => (bool)GetValue(IsDraggingOverProperty);
            private set => SetValue(IsDraggingOverProperty, value);
        }
    }
}
