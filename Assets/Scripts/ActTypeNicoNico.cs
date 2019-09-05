using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ActTypeNicoNico
{
    #region <Consts>

    /// <summary>
    /// NicoNico URL format Header
    /// </summary>
    public const string NicoNicoUrlHeader = "https://www.nicovideo.jp/watch/";
    public const string NicoNicoUrlRegix = "^" + NicoNicoUrlHeader + "(sm|nm)[0-9]{0,9}$";
    public static ActTypeNicoNico GetInstance { get; }
    public static Regex _niconicoUrlRegex;

    public static bool IsNicoNicoVidUrl(string p_Url) => _niconicoUrlRegex.IsMatch(p_Url);
    
    #endregion

    #region <Fields>

    /// <summary>
    /// [video_Id][video_URL] collection
    /// </summary>
    private Dictionary<int, string> _VideoIdCollection;

    
    #endregion

    #region <Constructors>

    static ActTypeNicoNico ()
    {
        GetInstance = new ActTypeNicoNico();
        _niconicoUrlRegex = new Regex(@NicoNicoUrlRegix);        
    }

    private ActTypeNicoNico()
    {
        _VideoIdCollection = new Dictionary<int, string>();
    }
    
    #endregion

    #region <Methods>

    /// <summary>
    /// Extract ID from Text and form NicoNico format URL
    /// </summary>
    public void FormatToNicoNicoUrl(string p_Text)
    {
        Debug.Log(p_Text);
        
        var tryCount = 0;
        var successCount = 0;
        _VideoIdCollection.Clear();
        var partialTextGroup = p_Text.Split(new string[]{ NicoNicoUrlHeader }, StringSplitOptions.None);
        foreach (var partialTextLine in partialTextGroup)
        {
            try
            {
                Debug.Log("pasredText : " + partialTextLine);
                var vidUrl = NicoNicoUrlHeader + partialTextLine;
                tryCount++;
                if (IsNicoNicoVidUrl(vidUrl))
                {
                    vidUrl = vidUrl.Substring(0, vidUrl.Length - 1);
                    if (!ReadDB.cachedVideoUrlGroup.Contains(vidUrl))
                    {
                        ReadDB.cachedVideoUrlGroup.Add(vidUrl);
                        var parsedId = Convert.ToInt32(vidUrl.Split('m')[1]);
                        Debug.Log(parsedId);
                        if (!_VideoIdCollection.ContainsKey(parsedId))
                        {
                            successCount++;
                            _VideoIdCollection.Add(parsedId, vidUrl + "\n");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // do sth
            }
        }
        
        Debug.Log($"{tryCount} / {successCount}");
    }

    /// <summary>
    /// Return NicoNico format Manifest
    /// </summary>
    /// <returns></returns>
    public string GetFormattedString()
    {
        StringBuilder builder = new StringBuilder();
        foreach (var videoIdUrlPair in _VideoIdCollection)
        {
            builder.Append(videoIdUrlPair.Value);
        }

        return builder.ToString();
    }

    #endregion
}
