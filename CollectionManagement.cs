// --------------------------------------------------------------------------------
// ExoScan module
//
// Description:	
//
// Environment:  Windows 10 executable, 32 and 64 bit
//
// Usage:        TBD
//
// Author:		(REM) Rick McAlister, rrskybox@yahoo.com
//
// Edit Log:     Rev 1.0 Initial Version
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 
// ---------------------------------------------------------------------------------
//

using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ExoScan
{
    internal class CollectionManagement
    {
        public static void CreateCollection(string collectionName)
        {
            //If the collection isn't already initialized, create a new collection file structure
            Configuration cfg = new Configuration();
            cfg.CollectionFolderPath = cfg.ExoScanFolderPath + "\\" + collectionName;
            if (!Directory.Exists(cfg.CollectionFolderPath))
            {
                Directory.CreateDirectory(cfg.CollectionFolderPath);
                cfg.TargetListPath = cfg.CollectionFolderPath + "\\" + "ExoScanList.xml";
                cfg.ColorListPath = cfg.CollectionFolderPath + "\\" + "ColorList.xml";
                cfg.ImageBankFolder = cfg.CollectionFolderPath + "\\" + "Image Bank";
                Directory.CreateDirectory(cfg.ImageBankFolder);
                cfg.StarchiveFilePath = cfg.CollectionFolderPath + "\\" + "Starchive.xml";
                cfg.LogFolder = cfg.CollectionFolderPath + "\\" + "Logs";
                Directory.CreateDirectory(cfg.LogFolder);
            }
            else
            {
                DialogResult dr = MessageBox.Show("Overwrite existing target list?", "", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    File.Delete(cfg.TargetListPath);
                }
            }
            return;
        }

        public static string OpenCollection(string collectionName)
        {
            //Change collection to an existing collection
            //  Return the path to the collection's target list)
            Configuration cfg = new Configuration();
            cfg.CollectionFolderPath = cfg.ExoScanFolderPath + "\\" + collectionName;
            cfg.TargetListPath = cfg.CollectionFolderPath + "\\" + "ExoScanList.xml";
            cfg.ColorListPath = cfg.CollectionFolderPath + "\\" + "ColorList.xml";
            cfg.ImageBankFolder = cfg.CollectionFolderPath + "\\" + "Image Bank";
            cfg.StarchiveFilePath = cfg.CollectionFolderPath + "\\" + "Starchive.xml";
            cfg.LogFolder = cfg.CollectionFolderPath + "\\" + "Logs";
            if (!Directory.Exists(cfg.CollectionFolderPath))
                CreateCollection(collectionName);
            return cfg.TargetListPath;
        }

        public static List<string> ListCollections()
        {
            //Produces a list of collection names (not paths)
            Configuration cfg = new Configuration();
            List<string> cList = new List<string>();
            string basepath = cfg.ExoScanFolderPath;
            foreach (string fd in Directory.EnumerateDirectories(basepath))
            {
                Path.GetFileName(fd);
                cList.Add(Path.GetFileName(fd));
            }
            return cList;
        }

        public static bool HasCollection(string collectionName)
        {
            //returns true if the collection exists, that is, there is a directory for it
            //  false otherwise
            List<string> cList = ListCollections();
            if (cList.Contains(collectionName))
                return true;
            else
                return false;
        }

        public static string ActiveCollection()
        {
            //Returns file name of currently configured collection
            Configuration cfg = new Configuration();
            return Path.GetFileName(cfg.CollectionFolderPath);
        }
    }
}

