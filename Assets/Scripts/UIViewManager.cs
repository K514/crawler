using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIViewManager : MonoBehaviour
{
    #region <Consts>

    /// <summary>
    /// Singleton
    /// </summary>
    public static UIViewManager Instance { get; private set; }

    #endregion
    
    #region <Fields>

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
        _KeywordInputField = transform.Find("KeyWord").GetComponent<InputField>();
        _UrlInputField = transform.Find("Url").GetComponent<InputField>();
        _ParsingSymbolField = transform.Find("ParsingTerminateSymbol").GetComponent<InputField>();
        _Response = transform.Find("Image/Response").GetComponent<Text>();
        
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
        var KeywordHandler =  new InputField.SubmitEvent(); 
        KeywordHandler.AddListener(_LogicManager.OnEndInputKeyword);
        _KeywordInputField.onEndEdit = KeywordHandler;
        
        var UrlHandler =  new InputField.SubmitEvent(); 
        UrlHandler.AddListener(_LogicManager.OnEndInputUrl);
        _UrlInputField.onEndEdit = UrlHandler;
        
        var ParsingHandler =  new InputField.SubmitEvent(); 
        ParsingHandler.AddListener(_LogicManager.OnEndInputParsingTerminateSymbol);
        _ParsingSymbolField.onEndEdit = ParsingHandler;
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
