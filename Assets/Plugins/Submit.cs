using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class Submit : MonoBehaviour
{
    // Git�ֿ��·��
    private string repoPath;
    public string branchName;

    private string repoOwner;
    private string repoName;

    // Git�ֿ��URL
    private string repoUrl;
    // ����·��
    private string downloadPath;
    private void Start()
    {
        repoPath = Value.Instance.Path;
        repoOwner = Value.Instance.Username; // ���GitHub�û���
        repoName = Value.Instance.Reponame; // ���GitHub�ֿ���
        repoUrl = "https://github.com/{repoOwner}/{repoName}.git";
        downloadPath = Value.Instance.Path;
    }
    public void CommitChanges(string commitMessage)
    {
        // ��������޸ĵ��ļ�
        RunGitCommand("add .");

        // �ύ�޸�
        RunGitCommand($"commit -m \"{commitMessage}\"");

        // �����ύ��Զ�ֿ̲�
        RunGitCommand("push origin " + branchName);
    }

    public void DownloadBranch(string branchName)
    {
        // ȷ������·������
        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }

        // ��¡�ֿ���ض���֧
        RunGitCommand($"clone --branch {branchName} --single-branch {repoUrl} {downloadPath}");
    }

    private void RunGitCommand(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();

        // ����Git����
        startInfo.FileName = "git";
        startInfo.Arguments = command;
        startInfo.WorkingDirectory = repoPath; // ����Git�ֿ�·��
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        process.StartInfo = startInfo;

        // ��������
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
