using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class ReadDB : MonoBehaviour
{
    #region <Consts>

    private static string Storage_DB_Directory;
    private const string DB_FileName = "/BD415.TXT";
    public static List<string> cachedVideoUrlGroup;

    public static void UpdateDB()
    {
        var fileLoc = Storage_DB_Directory + DB_FileName;
        using (FileStream fs = File.Create(fileLoc))
        {
            foreach (var videoUrl in cachedVideoUrlGroup)
            {
                Byte[] serializedResult = new UTF8Encoding(true).GetBytes(videoUrl + '\n');
                fs.Write(serializedResult, 0, serializedResult.Length);
            }
        }
    }

    #endregion

    #region <Callback>

    private void Awake()
    {
        cachedVideoUrlGroup = new List<string>();
        Storage_DB_Directory = Application.dataPath + "/../DB";

        if (!Directory.Exists(Storage_DB_Directory))
        {
            Directory.CreateDirectory(Storage_DB_Directory);
        }

        var fileLoc = Storage_DB_Directory + DB_FileName;
        if (!File.Exists(fileLoc))
        {
            File.Create(fileLoc);
        }

    }

    private void Start()
    {        
        var fileLoc = Storage_DB_Directory + DB_FileName;
        string[] records = File.ReadAllLines(fileLoc);
        foreach (var record in records)
        {
            if (!cachedVideoUrlGroup.Contains(record) && ActTypeNicoNico.IsNicoNicoVidUrl(record))
            {
                cachedVideoUrlGroup.Add(record);
            }
        }
        CachingExportedVideoUrl();
    }

    #endregion

    #region <Methods>

    private void CachingExportedVideoUrl()
    {
        var exportDirectory = UIViewManager.Instance._LogicManager.GetExportFileName();
        if (Directory.Exists(exportDirectory))
        {
            var fileNameSet = Directory.GetFiles(exportDirectory);
            foreach (var fileName in fileNameSet)
            {
                var formattedDirectoryStr = fileName.Replace('\\', '/');
                string[] records = File.ReadAllLines(formattedDirectoryStr);
                foreach (var record in records)
                {
                    if (!cachedVideoUrlGroup.Contains(record) && ActTypeNicoNico.IsNicoNicoVidUrl(record))
                    {
                        cachedVideoUrlGroup.Add(record);
                    }
                }
            }
        }

        UpdateDB();
    }

    #endregion
}
