using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class Tool : MonoBehaviour
{
    public string branchName;
    private string token;
    private string repoOwner;
    private string repoName;
    private string repoPath;
    private string repoUrl;
    private string downloadPath;

    private void Start()
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        token = Value.Instance.Token; // 你的GitHub个人访问令牌
        repoOwner = Value.Instance.Username; // 你的GitHub用户名
        repoName = Value.Instance.Reponame; // 你的GitHub仓库名
        repoPath = Value.Instance.Path;
        repoUrl = "https://github.com/{repoOwner}/{repoName}.git";
        downloadPath = Value.Instance.Path;
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
                UnityEngine.Debug.Log("Pull request created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string responseText = reader.ReadToEnd();
                UnityEngine.Debug.LogError("Error creating pull request: " + responseText);
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
                UnityEngine.Debug.Log("Pull request merged: " + responseText);
            }
        }
        catch (WebException ex)
        {
            UnityEngine.Debug.LogError("Error merging pull request: " + ex.Message);
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
                UnityEngine.Debug.Log("Branch created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            UnityEngine.Debug.LogError("Error creating branch: " + ex.Message);
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
                UnityEngine.Debug.Log("API Response: " + responseText);

                // 解析JSON
                GitHubRefResponse refResponse = JsonUtility.FromJson<GitHubRefResponse>(responseText);
                if (refResponse != null && refResponse.@object != null)
                {
                    return refResponse.@object.sha; // 返回分支的哈希值
                }
                else
                {
                    UnityEngine.Debug.LogError("Failed to parse branch SHA.");
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                UnityEngine.Debug.LogError("API Error: " + reader.ReadToEnd());
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
            UnityEngine.Debug.Log("Branch deleted: " + branchName);
        }
        catch (WebException ex)
        {
            UnityEngine.Debug.LogError("Error deleting branch: " + ex.Message);
        }
    }
    //递交修改
    public void CommitChanges(string commitMessage)
    {
        // 添加所有修改的文件
        RunGitCommand("add .");

        // 提交修改
        RunGitCommand($"commit -m \"{commitMessage}\"");

        // 推送提交到远程仓库
        RunGitCommand("push origin " + branchName);
    }
    //获取分支
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
    //Git指令
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
public class Tools
{
    private string token;
    private string repoOwner;
    private string repoName;
    private string repoPath;
    private string repoUrl;
    private string downloadPath;

    public Tools()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "token.txt");

        // 检查文件是否存在
        if (File.Exists(filePath))
        {
            // 读取文件内容
            Value.Instance.Token = File.ReadAllText(filePath);
        }
        else
        {
            UnityEngine.Debug.LogError("File not found: " + filePath);
        }
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        token = Value.Instance.Token; // 你的GitHub个人访问令牌
        repoOwner = Value.Instance.Username; // 你的GitHub用户名
        repoName = Value.Instance.Reponame; // 你的GitHub仓库名
        repoPath = Value.Instance.Path;
        repoUrl = "https://github.com/{repoOwner}/{repoName}.git";
        downloadPath = Value.Instance.Path;
    }


    //创建拉取请求:title_请求标题，head_当前分支，baseBranch_目标分支，body_注释
    //创建请求
    public int CreatePullRequest(string title, string head, string baseBranch, string body)
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
                // 解析JSON响应，获取拉取请求编号
                return ParsePRNumber(responseText);
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string responseText = reader.ReadToEnd();
                return -1;
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
            }
        }
        catch
        {
        }
    }
    //获取拉取请求编号
    private int ParsePRNumber(string jsonResponse)
    {
        // 使用简单的字符串查找方法解析JSON
        int startIndex = jsonResponse.IndexOf("\"number\":") + 9;
        int endIndex = jsonResponse.IndexOf(",", startIndex);
        string numberStr = jsonResponse.Substring(startIndex, endIndex - startIndex);
        return int.Parse(numberStr);
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

            }
        }
        catch (WebException ex)
        {

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


                // 解析JSON
                GitHubRefResponse refResponse = JsonUtility.FromJson<GitHubRefResponse>(responseText);
                if (refResponse != null && refResponse.@object != null)
                {
                    return refResponse.@object.sha; // 返回分支的哈希值
                }
                else
                {
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                UnityEngine.Debug.LogError("API Error: " + reader.ReadToEnd());
            }
            //UnityEngine.Debug.Log(token);
            //UnityEngine.Debug.Log(ex);
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
        }
        catch
        {

        }
    }
    //递交修改
    public void CommitChanges(string branchName, string commitMessage)
    {
        // 添加所有修改的文件
        RunGitCommand("add .");

        // 提交修改
        RunGitCommand($"commit -m \"{commitMessage}\"");

        // 推送提交到远程仓库
        RunGitCommand("push origin " + branchName);
    }
    //获取分支
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
    //Git指令
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
