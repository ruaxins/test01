using UnityEditor;
using UnityEngine;

public class MyCustomWindow : EditorWindow
{
    string new_branchName = "";
    string original_branchName;
    string delete_branchName;
    string now_branchName;
    string merge_title;
    string merge_branchName;
    string merged_branchName;
    string merge_body;

    // ����һ���˵�����ڴ򿪴���
    [MenuItem("Tools/Git")]
    public static void ShowWindow()
    {
        // ��ʾ�Զ��崰��
        GetWindow<MyCustomWindow>("Git");
    }

    // ���ڵ�GUI����
    private void OnGUI()
    {
        GUILayout.Label("������֧", EditorStyles.boldLabel);
        new_branchName = EditorGUILayout.TextField("������֧����:", new_branchName);
        original_branchName = EditorGUILayout.TextField("ԭ��֧����:", original_branchName);
        if (GUILayout.Button("����"))
        {
            bool isConfirmed = EditorUtility.DisplayDialog(
                "Confirmation", // ��������
                "Are you sure you want to submit: " + new_branchName + "?", // ������Ϣ
                "Yes", // ȷ�ϰ�ť�ı�
                "No" // ȡ����ť�ı�
            );
            if(!isConfirmed) return;
            if (!string.IsNullOrEmpty(new_branchName) && !string.IsNullOrEmpty(original_branchName))
            {
                try
                {
                    //������֧
                    Debug.Log("�����ɹ�");
                }
                catch
                {

                }
            }
        }

        GUILayout.Label("\nɾ����֧", EditorStyles.boldLabel);
        delete_branchName = EditorGUILayout.TextField("ɾ����֧����:", delete_branchName);
        if (GUILayout.Button("ɾ��"))
        {
            bool isConfirmed = EditorUtility.DisplayDialog(
                "Confirmation", // ��������
                "Are you sure you want to submit: " + new_branchName + "?", // ������Ϣ
                "Yes", // ȷ�ϰ�ť�ı�
                "No" // ȡ����ť�ı�
            );
            if (!isConfirmed) return;
            if (!string.IsNullOrEmpty(delete_branchName))
            {
                try
                {
                    //ɾ����֧
                    Debug.Log("ɾ���ɹ�");
                }
                catch
                {

                }
            }
        }

        GUILayout.Label("\n���·�֧", EditorStyles.boldLabel);
        now_branchName = EditorGUILayout.TextField("���·�֧����:", now_branchName);
        if (GUILayout.Button("����"))
        {
            if (!string.IsNullOrEmpty(now_branchName))
            {
                bool isConfirmed = EditorUtility.DisplayDialog(
                    "Confirmation", // ��������
                    "Are you sure you want to submit: " + new_branchName + "?", // ������Ϣ
                    "Yes", // ȷ�ϰ�ť�ı�
                    "No" // ȡ����ť�ı�
                );
                if (!isConfirmed) return;
                try
                {
                    //���·�֧
                    Debug.Log("���³ɹ�");
                }
                catch
                {

                }
            }
        }

        GUILayout.Label("\n�ϲ���֧", EditorStyles.boldLabel);
        merge_title = EditorGUILayout.TextField("�ϲ�����:", merge_title);
        merge_branchName = EditorGUILayout.TextField("�ϲ���֧����:", merge_branchName);
        merged_branchName = EditorGUILayout.TextField("�ϲ�����֧����:", merged_branchName);
        merge_body = EditorGUILayout.TextField("�ϲ���ע:", merge_body);
        if (GUILayout.Button("�ϲ�"))
        {
            if (!string.IsNullOrEmpty(merge_title) && !string.IsNullOrEmpty(merge_branchName) && 
                !string.IsNullOrEmpty(merged_branchName) && !string.IsNullOrEmpty(merge_body))
            {
                bool isConfirmed = EditorUtility.DisplayDialog(
                    "Confirmation", // ��������
                    "Are you sure you want to submit: " + new_branchName + "?", // ������Ϣ
                    "Yes", // ȷ�ϰ�ť�ı�
                    "No" // ȡ����ť�ı�
                );
                if (!isConfirmed) return;
                try
                {
                    //�ϲ���֧
                    Debug.Log("�ϲ��ɹ�");
                }
                catch
                {

                }
            }
        }
    }
}
