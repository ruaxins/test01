using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Project : MonoBehaviour
{
    // Git仓库的路径
    private string repoPath = "D:\\Study\\Unity\\Programs\\Puzzle";
    // 远程仓库URL
    private string remoteUrl = "https://github.com/ruaxins/test-repo";
    private string uesename = "ruaxin";
    private string email = "2602656638@qq.com";

    public void OnCreateClick()
    {
        // 初始化仓库
        //InitializeRepository();

        // 添加并提交项目文件
        //AddAndCommitFiles("Initial commit");

        // 推送到远程仓库
        PushToRemote("main");
    }

    public void InitializeRepository()
    {
        // 初始化Git仓库
        RunGitCommand("init");

        // 关联远程仓库
        RunGitCommand($"remote add origin {remoteUrl}");
    }

    public void AddAndCommitFiles(string commitMessage)
    {
        // 添加所有文件
        RunGitCommand("add .");

        // 提交更改
        RunGitCommand($"commit -m \"{commitMessage}\"");
    }

    public void PushToRemote(string branchName)
    {
        RunGitCommand($"config --global user.name \"{uesename}\"");
        RunGitCommand($"config --global user.name \"{email}\"");
        RunGitCommand($"branch -M {branchName}");
        // 推送到远程仓库
        RunGitCommand($"push -u origin {branchName}");
    }

    private void RunGitCommand(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // 设置Git命令
        startInfo.FileName = "git";
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = repoPath;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;
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
