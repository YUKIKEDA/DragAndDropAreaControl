using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using DragAndDropAreaControl;

namespace DragAndDropAreaControl.Tests;

public class DragAndDropAreaTests
{
    #region ヘルパーメソッド（共通）

    /// <summary>
    /// WPF コントロールのインスタンス生成などを行う処理を STA スレッド上で実行する。
    /// </summary>
    private static void RunInSta(Action action)
    {
        ExceptionDispatchInfo? captured = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                captured = ExceptionDispatchInfo.Capture(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        captured?.Throw();
    }

    #region リフレクション用ヘルパー

    private static bool InvokeValidatePathsForDrop(DragAndDropArea target, string[] paths, out string errorMessage)
    {
        var method = typeof(DragAndDropArea).GetMethod(
            "ValidatePathsForDrop",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method);

        object?[] args = { paths, string.Empty };
        var result = (bool)method.Invoke(target, args)!;

        errorMessage = (string)args[1]!;
        return result;
    }

    private static void InvokeHandleSelectedPaths(DragAndDropArea target, string[] paths)
    {
        var method = typeof(DragAndDropArea).GetMethod(
            "HandleSelectedPaths",
            BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(method);

        method.Invoke(target, new object?[] { paths });
    }

    private static string CreateTempFile(string extension = ".tmp")
    {
        var dir = Path.Combine(Path.GetTempPath(), "DragAndDropAreaControlTests");
        Directory.CreateDirectory(dir);

        var path = Path.Combine(dir, $"{Guid.NewGuid():N}{extension}");
        File.WriteAllText(path, "test");
        return path;
    }

    private static string CreateTempDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), "DragAndDropAreaControlTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    #endregion

    #region プロパティのデフォルト値

    /// <summary>
    /// 依存関係プロパティの既定値がクラスコメントの仕様どおりになっていることを確認する。
    /// </summary>
    [Fact]
    public void 既定値_プロパティが期待通りであること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea();

            Assert.True(control.AllowFile);
            Assert.True(control.AllowFolder);
            Assert.True(control.AllowMultipleFiles);
            Assert.False(control.AllowMultipleFolders);

            Assert.Equal(string.Empty, control.AllowedExtensions);
            Assert.Equal("ファイルを選択してください", control.FileDropText);
            Assert.Equal("フォルダを選択してください", control.FolderDropText);
            Assert.Equal(string.Empty, control.HeaderText);
            Assert.Equal(string.Empty, control.ErrorMessage);
            Assert.Equal("#40FFFFFF", control.DragOverlayColor);
            Assert.Equal("#4000FF00", control.AfterDropColor);

            Assert.False(control.IsDragOver);
            Assert.False(control.IsDropped);
            Assert.False(control.HasError);
        });
    }

    #endregion

    #region ValidatePathsForDrop（ドロップ可能パス検証）のテスト

    /// <summary>
    /// パスが空配列の場合にエラーメッセージを返し、false が返ること。
    /// </summary>
    [Fact]
    public void ドロップ検証_パスが空配列の場合はエラーになること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea();

            var result = InvokeValidatePathsForDrop(control, Array.Empty<string>(), out var error);

            Assert.False(result);
            Assert.Equal("ファイルまたはフォルダが見つかりません。", error);
        });
    }

    /// <summary>
    /// AllowMultipleFiles = false の場合に、複数ファイル／フォルダを指定するとエラーになること。
    /// </summary>
    [Fact]
    public void ドロップ検証_複数指定不可設定で複数パスはエラーになること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowMultipleFiles = false
            };

            var file1 = CreateTempFile(".txt");
            var file2 = CreateTempFile(".txt");

            var result = InvokeValidatePathsForDrop(control, new[] { file1, file2 }, out var error);

            Assert.False(result);
            Assert.Equal("複数のファイル／フォルダは選択できません。", error);
        });
    }

    /// <summary>
    /// AllowFile = false の場合に、ファイルを指定するとエラーになること。
    /// </summary>
    [Fact]
    public void ドロップ検証_ファイル禁止設定でファイルはエラーになること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowFile = false
            };

            var file = CreateTempFile(".txt");

            var result = InvokeValidatePathsForDrop(control, new[] { file }, out var error);

            Assert.False(result);
            Assert.Equal("ファイルの選択は許可されていません。", error);
        });
    }

    /// <summary>
    /// AllowFolder = false の場合に、フォルダを指定するとエラーになること。
    /// </summary>
    [Fact]
    public void ドロップ検証_フォルダ禁止設定でフォルダはエラーになること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowFolder = false
            };

            var dir = CreateTempDirectory();

            var result = InvokeValidatePathsForDrop(control, new[] { dir }, out var error);

            Assert.False(result);
            Assert.Equal("フォルダの選択は許可されていません。", error);
        });
    }

    /// <summary>
    /// 実在しないパスが含まれている場合にエラーになること。
    /// </summary>
    [Fact]
    public void ドロップ検証_存在しないパスが含まれる場合はエラーになること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea();

            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "noexist.txt");

            var result = InvokeValidatePathsForDrop(control, new[] { path }, out var error);

            Assert.False(result);
            Assert.Equal("存在しないファイルまたはフォルダが含まれています。", error);
        });
    }

    /// <summary>
    /// AllowedExtensions で許可された拡張子は通過し、許可されていない拡張子はエラーになること。
    /// </summary>
    [Fact]
    public void ドロップ検証_許可された拡張子のみが有効になること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowedExtensions = "*.txt;*.csv"
            };

            var txtFile = CreateTempFile(".txt");
            var csvFile = CreateTempFile(".csv");
            var pngFile = CreateTempFile(".png");

            // 許可された拡張子は OK
            Assert.True(InvokeValidatePathsForDrop(control, new[] { txtFile }, out var error1));
            Assert.Equal(string.Empty, error1);

            Assert.True(InvokeValidatePathsForDrop(control, new[] { csvFile }, out var error2));
            Assert.Equal(string.Empty, error2);

            // 許可されていない拡張子は NG
            Assert.False(InvokeValidatePathsForDrop(control, new[] { pngFile }, out var error3));
            Assert.Equal("許可されていない拡張子のファイルが含まれています。", error3);
        });
    }

    #endregion

    #region HandleSelectedPaths（ドロップ済パスのマージ）のテスト

    /// <summary>
    /// AllowMultipleFiles = true, AllowMultipleFolders = false の場合に、
    /// ファイルは既存＋新規でマージされ、フォルダは最後の 1 件だけ保持されること。
    /// </summary>
    [Fact]
    public void パスマージ_複数ファイル許可かつ単一フォルダ許可で期待通りにマージされること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowMultipleFiles = true,
                AllowMultipleFolders = false
            };

            var file1 = CreateTempFile(".txt");
            var folder1 = CreateTempDirectory();

            control.DroppedFiles = new[] { file1, folder1 };

            var file2 = CreateTempFile(".txt");
            var folder2 = CreateTempDirectory();

            InvokeHandleSelectedPaths(control, new[] { file2, folder2 });

            Assert.NotNull(control.DroppedFiles);

            var files = control.DroppedFiles!.Where(File.Exists).ToArray();
            var folders = control.DroppedFiles!.Where(Directory.Exists).ToArray();

            // ファイル: 既存 + 新規がマージ（重複なし）
            Assert.Equal(2, files.Length);
            Assert.Contains(file1, files);
            Assert.Contains(file2, files);

            // フォルダ: AllowMultipleFolders = false のため、最後のフォルダ 1 件のみ
            Assert.Single(folders);
            Assert.Equal(folder2, folders[0]);
        });
    }

    /// <summary>
    /// AllowMultipleFiles = false の場合に、最後に選択された 1 ファイルのみ保持されること。
    /// </summary>
    [Fact]
    public void パスマージ_複数ファイル禁止設定で最後のファイルのみ保持されること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowMultipleFiles = false,
                AllowMultipleFolders = true
            };

            var file1 = CreateTempFile(".txt");
            var file2 = CreateTempFile(".txt");

            control.DroppedFiles = new[] { file1 };

            InvokeHandleSelectedPaths(control, new[] { file2 });

            Assert.NotNull(control.DroppedFiles);

            var files = control.DroppedFiles!.Where(File.Exists).ToArray();

            // AllowMultipleFiles = false のため、単一ファイルのみ保持される
            Assert.Single(files);
            Assert.Equal(file2, files[0]);
        });
    }

    /// <summary>
    /// AllowMultipleFolders = true の場合に、フォルダが既存＋新規でマージされること。
    /// </summary>
    [Fact]
    public void パスマージ_複数フォルダ許可設定で既存と新規がマージされること()
    {
        RunInSta(() =>
        {
            var control = new DragAndDropArea
            {
                AllowMultipleFiles = false,
                AllowMultipleFolders = true
            };

            var folder1 = CreateTempDirectory();
            var folder2 = CreateTempDirectory();

            control.DroppedFiles = new[] { folder1 };

            InvokeHandleSelectedPaths(control, new[] { folder2 });

            Assert.NotNull(control.DroppedFiles);

            var folders = control.DroppedFiles!.Where(Directory.Exists).ToArray();

            // AllowMultipleFolders = true のため、既存 + 新規がマージされる
            Assert.Equal(2, folders.Length);
            Assert.Contains(folder1, folders);
            Assert.Contains(folder2, folders);
        });
    }

    #endregion
}


#endregion