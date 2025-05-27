# DragAndDropAreaControl

WPFアプリケーション用のドラッグ＆ドロップエリアコントロールです。

## 機能

- ファイルのドラッグ＆ドロップ
- ファイル選択ダイアログによるファイル選択
- 複数ファイルのサポート
- ファイル拡張子の制限
- ファイルサイズの制限
- フォルダのドラッグ＆ドロップ制御
- エラーメッセージの表示

## 使用方法

### XAMLでの使用例

```xaml
<local:DragAndDropArea
    AllowMultipleFiles="True"
    AllowedExtensions=".txt,.pdf,.doc,.docx"
    MaxFileSize="10485760"
    AllowFolders="False"
    FilesDropped="OnFilesDropped"/>
```

### コードビハインドでの使用例

```csharp
private void OnFilesDropped(string[] files)
{
    foreach (var file in files)
    {
        // ファイルの処理
    }
}
```

## プロパティ

- `AllowMultipleFiles`: 複数ファイルのドロップを許可するかどうか
- `AllowedExtensions`: 許可するファイル拡張子の配列
- `MaxFileSize`: 最大ファイルサイズ（バイト単位）
- `AllowFolders`: フォルダのドロップを許可するかどうか
- `AllowFiles`: ファイルのドロップを許可するかどうか
- `ErrorMessage`: エラーメッセージ
- `DroppedFiles`: ドロップされたファイルのパスの配列
- `IsDraggingOver`: ドラッグオーバー状態

## イベント

- `FilesDropped`: ファイルがドロップされた時に発生するイベント

## コマンド

- `ApplicationCommands.Open`: ファイル選択ダイアログを開く
- `ClearCommand`: ドロップされたファイルをクリアする

## 必要条件

- .NET 6.0以上
- WPFアプリケーション