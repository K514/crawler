using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UILogicManager : MonoBehaviour
{
    #region <Fields>

    private string _TargetUrl;
    private string _doc;
    
    #endregion

    #region <Callbacks>

    public void OnEndInputUrl(string p_Url)
    {
        _TargetUrl = p_Url;
        UIViewManager.Instance.OnNewTaskOccured();
        StartCoroutine(DownloadWebDoc());
    }

    #endregion

    #region <Methods>

    private IEnumerator DownloadWebDoc()
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
                Debug.Log(_doc);
            }
        }
    }

    #endregion
}
