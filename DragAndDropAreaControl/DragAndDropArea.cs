using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.IO;
using System.Linq;

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
                string[] validFiles = files.Where(IsValidPath).ToArray();
                if (validFiles.Length > 0)
                {
                    OnFilesDropped(validFiles);
                }
            }
        }

        private void DragAndDropArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void DragAndDropArea_DragLeave(object sender, DragEventArgs e)
        {
            // ドラッグが領域から出た時の処理
        }

        private void OpenFileDialog_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (AllowedExtensions.Length > 0)
            {
                dialog.Filter = string.Join("|", AllowedExtensions
                    .Select(ext => $"{ext.TrimStart('.')}ファイル (*{ext})|*{ext}"));
            }

            if (dialog.ShowDialog() == true)
            {
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
    }
}
