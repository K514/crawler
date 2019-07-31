using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UILogicManager : MonoBehaviour
{
    #region <Fields>

    private string _TargetUrl;
    private string _TargetKeyword;
    private string _ParsingTerminateSymbol;
    private string _doc;
    
    #endregion

    #region <Callbacks>

    public void OnEndInputKeyword(string p_Keyword)
    {
        _TargetKeyword = p_Keyword;
        UIViewManager.Instance.SetResponseMessage($"Keyword set . . . [{ _TargetKeyword }]");
    }
    
    public void OnEndInputUrl(string p_Url)
    {
        _TargetUrl = p_Url;
        UIViewManager.Instance.OnNewTaskOccured();
        StartCoroutine(DownloadWebDoc());
    }

    public void OnEndInputParsingTerminateSymbol(string p_Symbol)
    {
        _ParsingTerminateSymbol = p_Symbol;
        UIViewManager.Instance.SetResponseMessage($"Parsing Terminate Symbol set . . . [{ _ParsingTerminateSymbol }]");
    }
    
    #endregion

    #region <Methods>

    private IEnumerator DownloadWebDoc(Action p_NextTask = null)
    {
        yield return null;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_TargetUrl))
        {
            UIViewManager.Instance.SetResponseMessage($"Requesting . . . [{ _TargetUrl }]");
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                UIViewManager.Instance.SetResponseMessage($"Network Fail . . . [{ _TargetUrl }]");
            }
            else
            {
                UIViewManager.Instance.SetResponseMessage($"Received  . . . [{ _TargetUrl }]\n");
                _doc = webRequest.downloadHandler.text;
                p_NextTask?.Invoke();
            }
        }
    }

    #endregion
}
