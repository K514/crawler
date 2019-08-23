using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ActTypeNicoNico
{
    #region <Consts>

    /// <summary>
    /// NicoNico ID Format = smXXXXXXXX
    /// </summary>
    public const int IdLength = 8;

    /// <summary>
    /// NicoNico URL format Header
    /// </summary>
    public const string NicoNicoUrlHeader = "https://www.nicovideo.jp/watch/sm";
    public const string NicoNicoUrlRegix = "^" + NicoNicoUrlHeader + "[0-9]{0,9}$";
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
    /// <param name="p_Text"></param>
    public void FormatToNicoNicoUrl(string p_Text)
    {
        _VideoIdCollection.Clear();
        var partialTextGroup = p_Text.Split(new string[]{ "sm" }, StringSplitOptions.None);
        foreach (var partialTextLine in partialTextGroup)
        {
            Debug.Log(partialTextLine);
            try
            {
                var parsedId = int.Parse(partialTextLine.Substring(0, IdLength));
                var vidUrl = NicoNicoUrlHeader + parsedId;
                if (IsNicoNicoVidUrl(vidUrl))
                {
                    if (!ReadDB.cachedVideoUrlGroup.Contains(vidUrl))
                    {
                        ReadDB.cachedVideoUrlGroup.Add(vidUrl);
                        if (!_VideoIdCollection.ContainsKey(parsedId))
                        {
                            _VideoIdCollection.Add(parsedId, vidUrl + '\n');
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // do sth
            }
        }
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
