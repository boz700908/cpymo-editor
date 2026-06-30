namespace CpymoEditor.Tests;

public sealed class RepositoryTextEncodingTests
{
    private static readonly string[] MojibakeFragments =
    [
        "鏃佺櫧",
        "娆㈣繋",
        "鑳屾櫙",
        "瀵硅瘽",
        "閫夋嫨",
        "绱犳潗",
        "閰嶇疆",
        "宸ュ叿",
        "婧愮爜",
        "缂栬瘧",
        "鎵撳寘",
        "涓婁竴",
        "涓嬩竴",
        "璺宠浆"
    ];

    [Fact]
    public void RepositoryText_DoesNotContainKnownMojibakeFragments()
    {
        string root = FindRepositoryRoot();
        string[] files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(IsCheckedTextFile)
            .Where(path => !IsIgnoredPath(root, path))
            .ToArray();

        var matches = new List<string>();
        foreach (string file in files)
        {
            string text = File.ReadAllText(file);
            foreach (string fragment in MojibakeFragments)
            {
                if (text.Contains(fragment, StringComparison.Ordinal))
                {
                    matches.Add(Path.GetRelativePath(root, file) + ": " + fragment);
                }
            }
        }

        Assert.Empty(matches);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "src", "CpymoEditor.Core")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root was not found.");
    }

    private static bool IsCheckedTextFile(string path)
    {
        string extension = Path.GetExtension(path);
        return extension is ".cs" or ".xaml" or ".md";
    }

    private static bool IsIgnoredPath(string root, string path)
    {
        string relative = Path.GetRelativePath(root, path).Replace('\\', '/');
        return relative.Contains("/bin/", StringComparison.Ordinal)
            || relative.Contains("/obj/", StringComparison.Ordinal)
            || relative.StartsWith("external/", StringComparison.Ordinal)
            || relative.StartsWith("pymo_v1_2_0_tools/", StringComparison.Ordinal)
            || relative.EndsWith("RepositoryTextEncodingTests.cs", StringComparison.Ordinal);
    }
}
