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
    private InputField _InputField;
    
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
        _InputField = transform.Find("Input").GetComponent<InputField>();
        _Response = transform.Find("Image/Response").GetComponent<Text>();
        
        // Pop Logic Manager
        _LogicManager = new UILogicManager();

        // Bind EventHandler
        BindEventHandler();
    }

    #endregion

    #region <Methods>

    /// <summary>
    /// Bind Event Handler at LogicManager to UnityUI callbacks
    /// </summary>
    private void BindEventHandler()
    {
        var handler =  new InputField.SubmitEvent(); 
        handler.AddListener(_LogicManager.OnEndInputUrl);
        _InputField.onEndEdit = handler;
    }

    /// <summary>
    /// Set Response Message
    /// </summary>
    /// <param name="p_Message"></param>
    public void SetResponseMessage(string p_Message)
    {
        _Response.text = p_Message;
    }

    #endregion
}
