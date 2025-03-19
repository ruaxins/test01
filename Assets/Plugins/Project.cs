using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Project : MonoBehaviour
{
    private string token = Value.Instance.Token; // ���GitHub���˷�������
    private string repoOwner = Value.Instance.Username; // ���GitHub�û���
    private string repoName = Value.Instance.Reponame; // ���GitHub�ֿ���
    // Git�ֿ��·��
    private string repoPath = Value.Instance.Path;
    // Զ�ֿ̲�URL
    private string remoteUrl = "https://github.com/{repoOwner}/{repoName}";
    private string uesename = Value.Instance.Username;
    private string email = Value.Instance.Email;

    public void OnCreateClick()
    {
        // ��ʼ���ֿ�
        InitializeRepository();

        // ��Ӳ��ύ��Ŀ�ļ�
        AddAndCommitFiles("Initial commit");

        // ���͵�Զ�ֿ̲�
        PushToRemote("main");
    }

    public void InitializeRepository()
    {
        // ��ʼ��Git�ֿ�
        RunGitCommand("init");

        // ����Զ�ֿ̲�
        RunGitCommand($"remote add origin {remoteUrl}");
    }

    public void AddAndCommitFiles(string commitMessage)
    {
        RunGitCommand($"config --global user.name \"{uesename}\"");
        RunGitCommand($"config --global user.email \"{email}\"");

        // ��������ļ�
        RunGitCommand("add .");

        // �ύ����
        RunGitCommand($"commit -m \"{commitMessage}\"");
    }

    public void PushToRemote(string branchName)
    {

        RunGitCommand($"branch -M {branchName}");
        // ���͵�Զ�ֿ̲�
        RunGitCommand($"push -u origin {branchName}");
    }

    private void RunGitCommand(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // ����Git����
        startInfo.FileName = "git";
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = repoPath;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;
        process.Start();

        // ��ȡ���
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        // ��ӡ���
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
