using System;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.UI;

public class GitHubUI : MonoBehaviour
{
    public GitHubManager githubManager;
    public InputField issue_title;
    public InputField issue_body;

    public InputField send_title;
    public InputField send_head;
    public InputField send_base;
    public InputField send_body;

    public InputField confirm_num;
    public InputField confirm_head;
    public InputField confirm_body;

    public InputField branch_name;
    public InputField branch_sha;

    public InputField branch_name_;


    public void OnCreateIssueButtonClick()
    {
        string title = issue_title.text;
        string body = issue_body.text;
        githubManager.CreateIssue(title, body);
    }
    public void OnSendClick()
    {
        string title = send_title.text;
        string head = send_head.text;
        string base_ = send_base.text;
        string body = send_body.text;
        githubManager.CreatePullRequest(title, head, base_, body);
    }
    public void OnConfirmClick()
    {
        int num = Convert.ToInt32(confirm_num.text);
        string head = confirm_head.text;
        string body = confirm_body.text;
        githubManager.MergePullRequest(num, head, body);
    }
    public void OnCreateBranchClick()
    {
        string name = branch_name.text;
        string sha = branch_sha.text;
        string baseSha = githubManager.GetBranchSha(sha);
        Debug.Log(baseSha);
        if (!string.IsNullOrEmpty(baseSha))
        {
            githubManager.CreateBranch(name, baseSha);
        }
        else
        {
            Debug.LogError("Failed to get base branch SHA.");
        }
    }
    public void OnDeleteBranchClick()
    {
        string name = branch_name_.text;
        githubManager.DeleteBranch(name);
    }
    public void OnGetCommitHistoryClick()
    {
        githubManager.GetCommitHistory();
    }
}
