using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class GitCommandEditor
{
    [MenuItem("Git/Pull", false, 0)]
    public static void Pull()
    {
        string fullPath = GetShellPath("GitPull");
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Dont exist file {fullPath}");
            return;
        }

        string fullTempPath = fullPath.Replace(".sh", "_Temp.sh");
        StringBuilder sb = new StringBuilder(File.ReadAllText(fullPath));
        sb.Replace("#pull_branch#", "main");
        File.WriteAllText(fullTempPath, sb.ToString());

        Process p = StartProcess(fullTempPath);
        p?.WaitForExit();
        File.Delete(fullTempPath);
    }

    [MenuItem("Git/Push Package", false, 1)]
    public static void PushPackage()
    {
        string fullPath = GetShellPath("GitSubtree");
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Dont exist file {fullPath}");
            return;
        }

        string fullTempPath = fullPath.Replace(".sh", "_Temp.sh");
        StringBuilder sb = new StringBuilder(File.ReadAllText(fullPath));
        sb.Replace("#split_path#", "Assets/Package");
        sb.Replace("#split_branch#", "upm");
        File.WriteAllText(fullTempPath, sb.ToString());

        Process p = StartProcess(fullTempPath);
        p?.WaitForExit();
        File.Delete(fullTempPath);
    }

    [MenuItem("Git/Push Samples", false, 2)]
    public static void PushSamples()
    {
        CopySamples();
        string fullPath = GetShellPath("GitPush");
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Dont exist file {fullPath}");
            return;
        }

        string fullTempPath = fullPath.Replace(".sh", "_Temp.sh");
        StringBuilder sb = new StringBuilder(File.ReadAllText(fullPath));
        sb.Replace("#dir_path#", "Assets/Package/Samples~");
        sb.Replace("#cmt_message#", "update samples");
        sb.Replace("#push_branch#", "main");
        File.WriteAllText(fullTempPath, sb.ToString());

        Process p = StartProcess(fullTempPath);
        p?.WaitForExit();
        File.Delete(fullTempPath);
        PushPackage();
    }

    [MenuItem("Git/Clear History", false, 3)]
    public static void ClearHistory()
    {
        CopySamples();
        string fullPath = GetShellPath("GitClearHistory");
        if (!File.Exists(fullPath))
        {
            Debug.LogError($"Dont exist file {fullPath}");
            return;
        }

        string fullTempPath = fullPath.Replace(".sh", "_Temp.sh");
        StringBuilder sb = new StringBuilder(File.ReadAllText(fullPath));
        sb.Replace("#cmt_message#", "clear history");
        sb.Replace("#clear_branch#", "main");
        File.WriteAllText(fullTempPath, sb.ToString());

        Process p = StartProcess(fullTempPath);
        p?.WaitForExit();
        File.Delete(fullTempPath);
    }

    private static string GetShellPath(string name)
    {
        return $"{Application.dataPath}/{GetPackagePah()}/Editor/Shells/{name}.sh";
    }

    private static string GetPackagePah()
    {
        string package = Path.GetFullPath("Packages/com.cetejs.gitcommand");
        if (Directory.Exists(package))
        {
            return package;
        }

        return Path.GetFullPath("Package");
    }

    private static Process StartProcess(string fileName)
    {
        if (SystemInfo.operatingSystem.Contains("Windows"))
        {
            return Process.Start(fileName);
        }

        return Process.Start("/bin/bash", fileName);
    }

    private static void CopySamples()
    {
        string srcDirName = Path.Combine(Application.dataPath, "Samples");
        string desDirName = Path.Combine(Application.dataPath, "Package/Samples~");
        string[] filesPath = Directory.GetFiles(srcDirName, "*", SearchOption.AllDirectories);
        if (filesPath.Length <= 0)
        {
            return;
        }

        if (!Directory.Exists(desDirName))
        {
            Directory.CreateDirectory(desDirName);
        }

        foreach (string filePath in filesPath)
        {
            string desFilePath = filePath.Replace("Samples", "Package/Samples~");
            SafeCopyFile(filePath, desFilePath, true);
        }
    }

    private static void SafeCopyFile(string srcPath, string desPath, bool isOverwrite = false)
    {
        if (!File.Exists(srcPath))
        {
            return;
        }

        FileInfo fileInfo = new FileInfo(desPath);
        if (!Directory.Exists(fileInfo.DirectoryName))
        {
            Directory.CreateDirectory(fileInfo.DirectoryName);
        }

        File.Copy(srcPath, desPath, isOverwrite);
    }
}