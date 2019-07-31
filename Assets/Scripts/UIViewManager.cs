using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIViewManager : MonoBehaviour
{
    #region <Consts>

    private const string DefaultHeader = "https://www.nicovideo.jp";
    private const string DefaultParsing = "\"";
    private const string DefaultKeyword = "/watch/sm";
    
    /// <summary>
    /// Singleton
    /// </summary>
    public static UIViewManager Instance { get; private set; }

    #endregion
    
    #region <Fields>

    /// <summary>
    /// Header Input UI
    /// </summary>
    private InputField _AddHeaderInputField;
    
    /// <summary>
    /// Url Input UI
    /// </summary>
    private InputField _KeywordInputField;
    
    /// <summary>
    /// Url Input UI
    /// </summary>
    private InputField _UrlInputField;
        
    /// <summary>
    /// Url Input UI
    /// </summary>
    private InputField _ExportInputField;
    
    /// <summary>
    /// Url Input UI
    /// </summary>
    private InputField _ParsingSymbolField;

    /// <summary>
    /// Response Message display here
    /// </summary>
    private Text _Response;
    
    /// <summary>
    /// Logic Manager
    /// </summary>
    private UILogicManager _LogicManager;
    
    #endregion

    #region <Callbacks>

    private void Awake()
    {
        // Singleton Initialize
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        // Find UI
        _AddHeaderInputField = transform.Find("AdditiveHeader").GetComponent<InputField>();
        _KeywordInputField = transform.Find("KeyWord").GetComponent<InputField>();
        _UrlInputField = transform.Find("Url").GetComponent<InputField>();
        _ParsingSymbolField = transform.Find("ParsingTerminateSymbol").GetComponent<InputField>();
        _Response = transform.Find("Image/Response").GetComponent<Text>();
        _ExportInputField = transform.Find("ExportTo").GetComponent<InputField>();
        
        // Pop Logic Manager
        _LogicManager = gameObject.AddComponent<UILogicManager>();

        // Bind EventHandler
        BindEventHandler();
    }

    /// <summary>
    /// Initialize View Manage State when new task begin
    /// </summary>
    public void OnNewTaskOccured()
    {
        ClearResponseMessage();
    }

    #endregion

    #region <Methods>

    /// <summary>
    /// Bind Event Handler at LogicManager to UnityUI callbacks
    /// </summary>
    private void BindEventHandler()
    {
        SetHandler_And_InitDefaultString(_AddHeaderInputField, _LogicManager.OnEndInputHeader, DefaultHeader);
        SetHandler_And_InitDefaultString(_KeywordInputField, _LogicManager.OnEndInputKeyword, DefaultKeyword);
        SetHandler_And_InitDefaultString(_ParsingSymbolField, _LogicManager.OnEndInputParsingTerminateSymbol, DefaultParsing);
        SetHandler_And_InitDefaultString(_UrlInputField, _LogicManager.OnEndInputUrl);
        _ExportInputField.text = UILogicManager.ExportDirectory;
    }

    /// <summary>
    /// Set Action and Default value to specify InputField
    /// </summary>
    private void SetHandler_And_InitDefaultString(InputField p_InputField, UnityAction<string> p_BindingAction, string p_DefautString = null)
    {
        var handlerWrapper =  new InputField.SubmitEvent(); 
        handlerWrapper.AddListener(p_BindingAction);
        p_InputField.onEndEdit = handlerWrapper;
        if (p_DefautString != null)
        {
            p_InputField.text = p_DefautString;
            p_InputField.onEndEdit.Invoke(p_InputField.text);
        }
    }

    /// <summary>
    /// Set Response Message
    /// </summary>
    /// <param name="p_Message"></param>
    public void SetResponseMessage(string p_Message)
    {
        _Response.text += $"{p_Message}\n" ;
    }

    /// <summary>
    /// Clear Response Message
    /// </summary>
    public void ClearResponseMessage()
    {
        _Response.text = null;
    }

    #endregion
}
