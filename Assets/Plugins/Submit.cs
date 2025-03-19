using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class Submit : MonoBehaviour
{
    // Git仓库的路径
    private string repoPath;
    public string branchName;

    private string repoOwner;
    private string repoName;

    // Git仓库的URL
    private string repoUrl;
    // 下载路径
    private string downloadPath;
    private void Start()
    {
        repoPath = Value.Instance.Path;
        repoOwner = Value.Instance.Username; // 你的GitHub用户名
        repoName = Value.Instance.Reponame; // 你的GitHub仓库名
        repoUrl = "https://github.com/{repoOwner}/{repoName}.git";
        downloadPath = Value.Instance.Path;
    }
    public void CommitChanges(string commitMessage)
    {
        // 添加所有修改的文件
        RunGitCommand("add .");

        // 提交修改
        RunGitCommand($"commit -m \"{commitMessage}\"");

        // 推送提交到远程仓库
        RunGitCommand("push origin " + branchName);
    }

    public void DownloadBranch(string branchName)
    {
        // 确保下载路径存在
        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }

        // 克隆仓库的特定分支
        RunGitCommand($"clone --branch {branchName} --single-branch {repoUrl} {downloadPath}");
    }

    private void RunGitCommand(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // 设置Git命令
        startInfo.FileName = "git";
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = repoPath; // 设置Git仓库路径
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;

        // 启动进程
        process.Start();

        // 读取输出
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        // 打印输出
        if (!string.IsNullOrEmpty(output))
        {
            UnityEngine.Debug.Log("Git Output: " + output);
        }
        if (!string.IsNullOrEmpty(error))
        {
            UnityEngine.Debug.LogError("Git Error: " + error);
        }
    }
}
