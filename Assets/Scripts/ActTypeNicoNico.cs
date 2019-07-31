using System;
using System.Collections.Generic;
using System.Text;
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
    public static ActTypeNicoNico GetInstance { get; }

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
        var partialTextGroup = p_Text.Split(new string[]{ "sm" }, StringSplitOptions.None);
        foreach (var partialTextLine in partialTextGroup)
        {
            Debug.Log(partialTextLine);
            try
            {
                var parsedId = int.Parse(partialTextLine.Substring(0, IdLength));
                if (!_VideoIdCollection.ContainsKey(parsedId))
                {
                    _VideoIdCollection.Add(parsedId, NicoNicoUrlHeader + parsedId + '\n');
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
