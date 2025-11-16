# DragAndDropAreaControl

WPF 向けの **ファイル／フォルダ用ドラッグ＆ドロップ + 選択ダイアログ** カスタムコントロールです。  
単一／複数のファイル・フォルダ選択、拡張子フィルタ、エラー表示などを 1 つのコントロールで扱えます。

## 特長

- **ドラッグ＆ドロップ対応**: エクスプローラーからのファイル／フォルダ D&D をサポート
- **ダイアログ選択**: ファイル選択ダイアログ・フォルダ選択ダイアログからの指定に対応
- **柔軟な制限設定**:
  - ファイル／フォルダの許可・禁止
  - 単一／複数選択の切り替え（ファイル・フォルダで個別設定）
  - 拡張子のホワイトリスト（例: `*.png;*.jpg;*.csv`）
- **状態フラグとエラー表示**: ドラッグ中・ドロップ済み・エラー状態を依存関係プロパティとして提供
- **テンプレートカスタマイズ可能**: `Generic.xaml` を差し替えることで UI を自由に変更可能

## 対応環境

- .NET / WPF アプリケーション  
- 対応フレームワークはプロジェクト設定に依存します（例: .NET 6 WPF など）

## インストール

ソリューション内の `DragAndDropAreaControl` プロジェクトを、利用したい WPF アプリから参照してください。

1. WPF アプリケーションのプロジェクトを右クリックして **[参照の追加]** を選択
2. **[プロジェクト]** から `DragAndDropAreaControl` を選択
3. 再ビルド

### XAML での名前空間宣言

利用側の XAML のルート要素に次の名前空間を追加します:

```xml
xmlns:dd="clr-namespace:DragAndDropAreaControl;assembly=DragAndDropAreaControl"
```

## 基本的な使い方

最小構成の例:

```xml
<dd:DragAndDropArea
    AllowedExtensions="*.png;*.jpg"
    AllowFile="True"
    AllowFolder="False"
    DroppedFiles="{Binding SelectedPaths, Mode=TwoWay}" />
```

`DroppedFiles` には、ドロップ／ダイアログで選択されたファイルまたはフォルダのパス配列が格納されます。  
ViewModel 側では `string[]` として受け取り、アップロード処理などに利用できます。

### すべてのプロパティを使用したサンプル

`DragAndDropAreaControl.Example` プロジェクトに、ReactiveProperty と MaterialDesign IconPack を用いたサンプル画面が含まれています。  
プロジェクトをスタートアップに設定して実行すると、以下のような画面が動作します。

```xml
<dd:DragAndDropArea
    AfterDropColor="{Binding AfterDropColor.Value}"
    AllowFile="{Binding AllowFile.Value}"
    AllowFolder="{Binding AllowFolder.Value}"
    AllowMultipleFiles="{Binding AllowMultipleFiles.Value}"
    AllowMultipleFolders="{Binding AllowMultipleFolders.Value}"
    AllowedExtensions="{Binding AllowedExtensions.Value}"
    DragOverlayColor="{Binding DragOverlayColor.Value}"
    DroppedFiles="{Binding DroppedFiles.Value, Mode=TwoWay}"
    FileDropText="{Binding FileDropText.Value}"
    FolderDropText="{Binding FolderDropText.Value}"
    HeaderText="{Binding HeaderText.Value}">
    <!-- アイコンは Content として自由に差し替え可能 -->
</dd:DragAndDropArea>
```

## 主な依存関係プロパティ

- **`DroppedFiles` (`string[]`)**:  
  ドロップ／ダイアログで選択されたパス一覧。バインディングの基本となるプロパティです。

- **`AllowedExtensions` (`string`, 既定値: `""`)**:  
  許可する拡張子のリストをセミコロン区切りで指定します（例: `*.png;*.jpg;*.csv`）。空文字列の場合は制限なし。

- **`AllowFile` (`bool`, 既定値: `true`)**:  
  ファイルのドロップ／選択を許可するかどうか。

- **`AllowFolder` (`bool`, 既定値: `true`)**:  
  フォルダのドロップ／選択を許可するかどうか。

- **`AllowMultipleFiles` (`bool`, 既定値: `true`)**:  
  複数ファイルの選択を許可するかどうか。`false` の場合、最後に選択された 1 ファイルだけが保持されます。

- **`AllowMultipleFolders` (`bool`, 既定値: `false`)**:  
  複数フォルダの選択を許可するかどうか。`false` の場合、最後に選択された 1 フォルダのみ保持されます。

- **`FileDropText` (`string`, 既定値: `"ファイルを選択してください"`)**:  
  ファイル選択ボタンに表示するテキスト。

- **`FolderDropText` (`string`, 既定値: `"フォルダを選択してください"`)**:  
  フォルダ選択ボタンに表示するテキスト。

- **`HeaderText` (`string`)**:  
  コントロール上部に表示するヘッダーテキスト。

- **`FileDropIcon` / `FolderDropIcon` (`object`)**:  
  各ボタンに表示するアイコンコンテンツ。`PackIcon` や `Path` など任意の要素を指定可能です。

- **`ErrorMessage` (`string`)**:  
  バリデーションエラー発生時に表示されるメッセージ。

- **`DragOverlayColor` (`string`, 既定値: `"#40FFFFFF"`)**:  
  ドラッグオーバー中に表示するオーバーレイの色（ARGB 形式文字列）。

- **`AfterDropColor` (`string`, 既定値: `"#4000FF00"`)**:  
  ドロップ完了後に表示するオーバーレイの色。

- **`IsDragOver` (`bool`)**:  
  現在ドラッグオーバー中かどうか（読み取り専用目的）。

- **`IsDropped` (`bool`)**:  
  有効なドロップが完了しているかどうか。

- **`HasError` (`bool`)**:  
  エラー状態かどうか。

## IconButtonControl について

同一ソリューションには `IconButtonControl` プロジェクトも含まれており、アイコン付きボタン (`IconButton`) のカスタムコントロールを提供します。  
必要に応じてこちらも参照設定を追加して利用してください。

## ライセンス

本ライブラリは MIT License の下で提供されます。詳細は `LICENSE.txt` を参照してください。

