using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UILogicManager : MonoBehaviour
{
    #region <Consts>

    private const string ExportFormatPref = "ExportFormatPref";
    public static string ExportDirectory;
    
    #endregion
    
    #region <Fields>

    private string _TargetUrl;
    private string _TargetKeyword;
    private string _TargetHeader;
    private char _ParsingTerminateSymbol;
    private string _doc;
    private string _result;
    private List<Action> _TaskQueue;
    private int _TaskQueueIndex;
    private Phase _CurrentPhase;
    private ActType _ActType;
    
    #endregion

    #region <Enums>

    private enum Phase
    {
        None, DownloadWebDoc, ParsingWebDoc, ExtraParsingByActType, ExportToText
    }

    private enum ActType
    {
        None, NicoNicoDouge
    }

    #endregion
    
    #region <Callbacks>

    private void Awake()
    {
        ExportDirectory = Application.dataPath + "/../Export";
        _TaskQueue = new List<Action>();
        _TaskQueue.Add(() => { StartCoroutine(DownloadWebDoc()); });
        _TaskQueue.Add(() => { StartCoroutine(ParseWebDoc()); });
        _TaskQueue.Add(() => { StartCoroutine(ExtraParsingByActType()); });
        _TaskQueue.Add(() => { StartCoroutine(ExporToText()); });
        _TaskQueue.Add(() => { Initialize(); });
    }

    public void OnEndInputHeader(string p_Header)
    {
        if (_CurrentPhase != Phase.None) return;
        _TargetHeader = p_Header;
        UIViewManager.Instance.SetResponseMessage($"Header set . . . [{ _TargetHeader }]");
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

    public void OnNicoNicoToggleSwitched(bool p_Flag)
    {
        if (_ActType == ActType.NicoNicoDouge && !p_Flag)
        {
            _ActType = ActType.None;
        }

        if (_ActType != ActType.NicoNicoDouge && p_Flag)
        {
            _ActType = ActType.NicoNicoDouge;
        }
    }

    #endregion

    #region <Methods>

    private void Initialize()
    {
        _TaskQueueIndex = 0;
        _result = "";
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
                if (iterateTargetChar == _ParsingTerminateSymbol) break;
                stringAppend.Append(iterateTargetChar);
            }
            _result += $"{_TargetHeader + stringAppend}\n";
        }
        
        EntryNextTask();
    }

    private IEnumerator ExtraParsingByActType()
    {
        yield return null;
        _CurrentPhase = Phase.ExtraParsingByActType;
        UIViewManager.Instance.SetResponseMessage($"Acting . . .  [{ _CurrentPhase }]");

        switch (_ActType)
        {
            case ActType.None :
                break;
            case ActType.NicoNicoDouge :
                ActTypeNicoNico.GetInstance.FormatToNicoNicoUrl(_result);
                _result = ActTypeNicoNico.GetInstance.GetFormattedString();
                break;
        }
        
        EntryNextTask();
    }
    
    private IEnumerator ExporToText()
    {
        yield return null;
        _CurrentPhase = Phase.ExportToText;
        UIViewManager.Instance.SetResponseMessage($"Exporting . . .");

        var exportId = PlayerPrefs.GetInt(ExportFormatPref) + 1;
        PlayerPrefs.SetInt(ExportFormatPref, exportId);
        PlayerPrefs.Save();

        if(!Directory.Exists(ExportDirectory))
        {
            Directory.CreateDirectory(ExportDirectory);
        }
        
        using (FileStream fileStream = File.Create(ExportDirectory + $"/{exportId}.txt"))
        {
            Byte[] serializedResult = new UTF8Encoding(true).GetBytes(_result);
            fileStream.Write(serializedResult, 0, serializedResult.Length);
        }
        ReadDB.UpdateDB();
        UIViewManager.Instance.SetResponseMessage($"Complete ! ! !");
        EntryNextTask();
    }

    public string GetExportFileName() => ExportDirectory;

    #endregion
}
