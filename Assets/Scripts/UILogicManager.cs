using UnityEngine;

public class UILogicManager
{
    #region <Fields>

    private string _TargetUrl;

    #endregion

    #region <Callbacks>

    public void OnEndInputUrl(string p_Url)
    {
        _TargetUrl = p_Url;
        Debug.Log(_TargetUrl);
    }

    #endregion
}
