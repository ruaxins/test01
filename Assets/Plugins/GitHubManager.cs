using UnityEngine;
using System.Collections;
using System.Net;
using System.Text;
using System.IO;
using UnityEngine.UI;

public class GitHubManager : MonoBehaviour
{
    private string token;
    private string repoOwner;
    private string repoName;
    private void Start()
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        token = Value.Instance.Token; // 你的GitHub个人访问令牌
        repoOwner = Value.Instance.Username; // 你的GitHub用户名
        repoName = Value.Instance.Reponame; // 你的GitHub仓库名

    }



    //创建issue
    public void CreateIssue(string title, string body)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/issues";
        string json = $"{{\"title\": \"{title}\", \"body\": \"{body}\"}}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        byte[] data = Encoding.UTF8.GetBytes(json);
        request.ContentLength = data.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("Issue created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("Error creating issue: " + ex.Message);
        }
    }
    //创建拉取请求:title_请求标题，head_当前分支，baseBranch_目标分支，body_注释
    //创建请求
    public void CreatePullRequest(string title, string head, string baseBranch, string body)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/pulls";
        string json = $"{{\"title\": \"{title}\", \"head\": \"{head}\", \"base\": \"{baseBranch}\", \"body\": \"{body}\"}}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        byte[] data = Encoding.UTF8.GetBytes(json);
        request.ContentLength = data.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("Pull request created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string responseText = reader.ReadToEnd();
                Debug.LogError("Error creating pull request: " + responseText);
            }
        }
    }
    //合并拉取请求:pullNumber_拉取请求的编号，commitTitle_合并标题，commitMessage_合并注释
    //同意请求
    public void MergePullRequest(int pullNumber, string commitTitle, string commitMessage)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/pulls/{pullNumber}/merge";
        string json = $"{{\"commit_title\": \"{commitTitle}\", \"commit_message\": \"{commitMessage}\"}}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "PUT";
        request.ContentType = "application/json";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        byte[] data = Encoding.UTF8.GetBytes(json);
        request.ContentLength = data.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("Pull request merged: " + responseText);
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("Error merging pull request: " + ex.Message);
        }
    }
    //branchName_新分支名称，baseSha_原分支的哈希值
    //创建分支
    public void CreateBranch(string branchName, string baseSha)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/git/refs";
        string json = $"{{\"ref\": \"refs/heads/{branchName}\", \"sha\": \"{baseSha}\"}}";
        
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.ContentType = "application/json";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        byte[] data = Encoding.UTF8.GetBytes(json);
        request.ContentLength = data.Length;

        using (Stream stream = request.GetRequestStream())
        {
            stream.Write(data, 0, data.Length);
        }

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("Branch created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("Error creating branch: " + ex.Message);
        }
    }
    //branchName_获取哈希值的分支名称
    //获取哈希值
    public string GetBranchSha(string branchName)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/git/ref/heads/{branchName}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("API Response: " + responseText);

                // 解析JSON
                GitHubRefResponse refResponse = JsonUtility.FromJson<GitHubRefResponse>(responseText);
                if (refResponse != null && refResponse.@object != null)
                {
                    return refResponse.@object.sha; // 返回分支的哈希值
                }
                else
                {
                    Debug.LogError("Failed to parse branch SHA.");
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Debug.LogError("API Error: " + reader.ReadToEnd());
            }
            return null;
        }
    }

    [System.Serializable]
    public class GitHubRefResponse
    {
        public string @ref;
        public string node_id;
        public string url;
        public GitHubObject @object;
    }

    [System.Serializable]
    public class GitHubObject
    {
        public string sha;
        public string type;
        public string url;
    }
    //branchName_删除的分支名称
    //删除分支
    public void DeleteBranch(string branchName)
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/git/refs/heads/{branchName}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "DELETE";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.Log("Branch deleted: " + branchName);
        }
        catch (WebException ex)
        {
            Debug.LogError("Error deleting branch: " + ex.Message);
        }
    }
    //获取仓库的提交历史
    public void GetCommitHistory()
    {
        string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/commits";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.Accept = "application/vnd.github.v3+json";
        request.Headers.Add("Authorization", $"token {token}");
        request.UserAgent = "UnityGitHubPlugin";

        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseText = reader.ReadToEnd();
                Debug.Log("Commit history: " + responseText);
                // 解析JSON并显示提交历史
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("Error fetching commit history: " + ex.Message);
        }
    }


}