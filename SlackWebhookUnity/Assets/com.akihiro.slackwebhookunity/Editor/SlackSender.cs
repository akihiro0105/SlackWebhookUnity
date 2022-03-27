#if UNITY_EDITOR
using System;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Slackのメッセージ作成、プレビューツール
/// https://app.slack.com/block-kit-builder/
/// </summary>
public class SlackSender : EditorWindow
{
    private IEnumerator coroutine;
    private string messageText = "";
    private string statusText = "";

    [MenuItem("Slack/Send Message")]
    private static void ShowImportWindow() => EditorWindow.GetWindow(typeof(SlackSender), false, "Send Message");

    private void OnGUI()
    {
        messageText = GUILayout.TextField(messageText);
        foreach (var item in ScriptableObjectControl.GetScriptableObject())
        {
            if (GUILayout.Button(item.sendName + "に送信")) SendMessage(item.url);
        }
        GUILayout.Label(statusText);
    }

    private void SendMessage(string url)
    {
        coroutine = Post(url);
        EditorApplication.update += SendProgress;
    }

    private void SendProgress()
    {
        if (!coroutine.MoveNext()) EditorApplication.update -= SendProgress;
    }

    #region Send Message
    private IEnumerator Post(string url)
    {
        var message = new PostData(messageText);
        var json = JsonUtility.ToJson(message);
        var postBytes = Encoding.UTF8.GetBytes(json);
        var www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(postBytes);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            statusText = $"Post Failure {www.result}";
        }
        else
        {
            statusText = "Post Success";
        }
    }

    [Serializable]
    private class PostData
    {
        public Block[] blocks = new Block[1];
        public PostData(string message)
        {
            blocks[0] = new Block();
            blocks[0].text.text = message;
        }
    }

    [Serializable]
    private class Block
    {
        public string type = "section";
        public Text text = new Text();
    }

    [Serializable]
    private class Text
    {
        public string type = "mrkdwn";
        public string text;
    }
    #endregion
}
#endif
