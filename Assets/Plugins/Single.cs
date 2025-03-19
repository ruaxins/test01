using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Single : MonoBehaviour
{
    private void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "token.txt");

        // 检查文件是否存在
        if (File.Exists(filePath))
        {
            // 读取文件内容
            Value.Instance.Token = File.ReadAllText(filePath);
            Debug.Log(Value.Instance.Token);
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}
public class Value
{
    //private string path = "D:\\Study\\Unity\\Programs\\Puzzle";
    private string path = "D:\\Study\\Unity\\Programs\\Card";
    public string Path { get => path; set => path = value; }

    private string username = "ruaxins";
    public string Username { get => username; set => username = value; }

    //private string reponame = "test-repo";
    private string reponame = "test01";
    public string Reponame { get => reponame; set => reponame = value; }

    private string token;
    public string Token { get => token; set => token = value; }

    private string email = "2602656638@qq.com";
    public string Email { get => email; set => email = value; }


    private static Value value;
    private Value() { }
    public static Value Instance
    {
        get
        {
            if (value == null)
            {
                value = new Value();
            }
            return value;
        }
    }
}
