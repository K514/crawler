using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UILogicManager : MonoBehaviour
{
    #region <Fields>

    private string _TargetUrl;
    private string _TargetKeyword;
    private char _ParsingTerminateSymbol;
    private string _doc;
    private string _result;
    private List<Action> _TaskQueue;
    private int _TaskQueueIndex;
    private Phase _CurrentPhase;
    
    #endregion

    #region <Enums>

    private enum Phase
    {
        None, DownloadWebDoc, ParsingWebDoc
    }

    #endregion
    
    #region <Callbacks>

    private void Awake()
    {
        _TaskQueue = new List<Action>();
        _TaskQueue.Add(() => { StartCoroutine(DownloadWebDoc()); });
        _TaskQueue.Add(() => { StartCoroutine(ParseWebDoc()); });
    }

    public void OnEndInputKeyword(string p_Keyword)
    {
        if (_CurrentPhase != Phase.None) return;
        _TargetKeyword = p_Keyword;
        UIViewManager.Instance.SetResponseMessage($"Keyword set . . . [{ _TargetKeyword }]");
    }
    
    public void OnEndInputUrl(string p_Url)
    {
        if (_CurrentPhase != Phase.None) return;
        _TargetUrl = p_Url;
        UIViewManager.Instance.OnNewTaskOccured();
        Initialize();
        EntryNextTask();
    }

    public void OnEndInputParsingTerminateSymbol(string p_Symbol)
    {
        if (_CurrentPhase != Phase.None) return;
        _ParsingTerminateSymbol = p_Symbol[0];
        UIViewManager.Instance.SetResponseMessage($"Parsing Terminate Symbol set . . . [{ _ParsingTerminateSymbol }]");
    }
    
    #endregion

    #region <Methods>

    private void Initialize()
    {
        _TaskQueueIndex = 0;
        _CurrentPhase = Phase.None;
    }

    private void EntryNextTask()
    {
        _TaskQueue[_TaskQueueIndex++%_TaskQueue.Count]();
    }

    private IEnumerator DownloadWebDoc()
    {
        yield return null;
        _CurrentPhase = Phase.DownloadWebDoc;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_TargetUrl))
        {
            UIViewManager.Instance.SetResponseMessage($"Requesting . . . [{ _TargetUrl }]");
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                UIViewManager.Instance.SetResponseMessage($"Network Fail . . . [{ _TargetUrl }]");
                Initialize();
            }
            else
            {
                UIViewManager.Instance.SetResponseMessage($"Received  . . . [{ _TargetUrl }]\n");
                _doc = webRequest.downloadHandler.text;
                EntryNextTask();
            }
        }
    }

    private IEnumerator ParseWebDoc()
    {
        yield return null;
        _CurrentPhase = Phase.ParsingWebDoc;
        UIViewManager.Instance.SetResponseMessage($"Parcing . . .  start from [{ _TargetKeyword }] to [{ _ParsingTerminateSymbol }]");
        var stringAppend = new StringBuilder();
        var partialDoc = _doc.Split(new string[]{_TargetKeyword}, StringSplitOptions.None);
        foreach (var parsingLine in partialDoc)
        {
            stringAppend.Clear();
            stringAppend.Append(_TargetKeyword);
            var deadLine = parsingLine.Length;
            for (var i = 0; i < deadLine; i++)
            {
                var iterateTargetChar = parsingLine[i];
                stringAppend.Append(iterateTargetChar);
                if (iterateTargetChar == _ParsingTerminateSymbol) break;
            }
            _result += $"{stringAppend}\n";
        }
        
        Debug.Log(_result);
    }

    #endregion
}
