using System;
using Reactive.Bindings;

namespace DragAndDropAreaControl.Example
{
    /// <summary>
    /// <see cref="MainWindow"/> 用のサンプル ViewModel。
    /// すべての公開プロパティを ReactiveProperty でラップし、
    /// XAML からバインディングして利用します。
    /// </summary>
    public class MainWindowViewModel
    {
        public ReactiveProperty<string[]> DroppedFiles { get; }

        public ReactiveProperty<string> AllowedExtensions { get; }

        public ReactiveProperty<bool> AllowFile { get; }

        public ReactiveProperty<bool> AllowFolder { get; }

        public ReactiveProperty<bool> AllowMultipleFiles { get; }

        public ReactiveProperty<bool> AllowMultipleFolders { get; }

        public ReactiveProperty<string> HeaderText { get; }

        public ReactiveProperty<string> FileDropText { get; }

        public ReactiveProperty<string> FolderDropText { get; }

        public ReactiveProperty<string> DragOverlayColor { get; }

        public ReactiveProperty<string> AfterDropColor { get; }

        public MainWindowViewModel()
        {
            DroppedFiles = new ReactiveProperty<string[]>(Array.Empty<string>());

            // 拡張子は PNG / JPG を許可するサンプル
            AllowedExtensions = new ReactiveProperty<string>("*.png;*.jpg");

            // 各種許可フラグ（初期値はコントロールのデフォルトと同等）
            AllowFile = new ReactiveProperty<bool>(true);
            AllowFolder = new ReactiveProperty<bool>(true);
            AllowMultipleFiles = new ReactiveProperty<bool>(true);
            AllowMultipleFolders = new ReactiveProperty<bool>(true);

            // 表示テキスト
            HeaderText = new ReactiveProperty<string>("ReactiveProperty + MaterialDesign IconPack サンプル");
            FileDropText = new ReactiveProperty<string>("ファイルを選択（PNG/JPG）");
            FolderDropText = new ReactiveProperty<string>("フォルダを選択");

            // オーバーレイカラー（ARGB／RGB 系文字列）
            DragOverlayColor = new ReactiveProperty<string>("#1400FF00");
            AfterDropColor = new ReactiveProperty<string>("#14000000");
        }
    }
}


