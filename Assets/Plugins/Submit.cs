using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class Submit : MonoBehaviour
{
    // Git�ֿ��·��
    public string repoPath = "D:\\Study\\Unity\\Programs\\Card";
    public string branchName;

    //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
    public string token; // ���GitHub���˷�������
    private string repoOwner = "ruaxins"; // ���GitHub�û���
    private string repoName = "test01"; // ���GitHub�ֿ���

    // Git�ֿ��URL
    private string repoUrl = "https://github.com/rauxins/test01.git";
    // ����·��
    private string downloadPath = "D:\\Study\\Unity\\Programs";

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
