using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class Create : MonoBehaviour
{
    // GitHub Token
    private string token = Value.Instance.Token;

    public void OnCreateClick()
    {
        // ´´½¨²Ö¿â
        CreateRepository("test-repo", "This is a test repository", false);
    }

    public void CreateRepository(string name, string description, bool isPrivate)
    {
        string url = "https://api.github.com/user/repos";
        string json = $"{{\"name\": \"{name}\", \"description\": \"{description}\", \"private\": {isPrivate.ToString().ToLower()}}}";

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
                Debug.Log("Repository created: " + responseText);
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                Debug.LogError("Error creating repository: " + reader.ReadToEnd());
            }
        }
    }


}