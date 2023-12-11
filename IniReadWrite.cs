// --------------------------------------------------------------------------------
// ExoScan module
//
// Description:	Class for reading and writing a generic .ini file, but mainly TheSky
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
using System.Linq;

namespace ExoScan
{
    internal class IniReadWrite
    {
        List<string> iniContents = null;
        string iniPath = null;

        public IniReadWrite(string iniFilePath)
        {
            //Queues up an ini file for R/W
            //
            //Save iniFile name
            iniPath = iniFilePath;
            //Read in iniFile
            iniContents = File.ReadAllLines(iniFilePath).ToList();
        }

        public string GetData(string itemName)
        {
            //Retrieves content of an itemName file
            string entryData, entryName;
            string itemLine = iniContents.FirstOrDefault(a => a.Contains(itemName));
            if (itemLine != null)
                (entryName, entryData) = ParseLine(itemLine);
            else
                entryData = null;
            return entryData;
        }

        public void SetData(string itemName, string newData)
        {
            //Rewrites content of an itemName in the ini file
            var itemLine = iniContents.First(a => a.Contains(itemName));
            int index = iniContents.IndexOf(itemLine);
            (string entryName, string entryData) = ParseLine(itemLine);
            iniContents[index] = FormLine(entryName, newData);
            return;
        }

        public void SaveFile()
        {
            //Writes out ini file data
            File.WriteAllLines(iniPath, iniContents);
        }

        private (string, string) ParseLine(string line)
        {
            //returns entry name and entry data as separate strings
            string[] lineParts = line.Split('=');
            return (lineParts[0], lineParts[1]);
        }

        private string FormLine(string entryName, string entryData)
        {
            //returns entry name and entry data as combined strings
            return entryName + "=" + entryData;
        }
    }
}
