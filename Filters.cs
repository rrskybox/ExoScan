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
using TheSky64Lib;

namespace ExoScan
{
    public partial class Filters
    {
        public class ActiveFilter
        {
            //public ColorIndexing.StandardColors JcAssign { get; set; }
            public string FilterName { get; set; }
            public int FilterIndex { get; set; }
        }

        public static string IniLookUpFilterName(int filterIndex)
        {
            //Ini file lookup equivalent of tsx szFilterName(filterIndex) method  
            string[] iFilters = IniFilterNameSet();
            return iFilters[filterIndex];
        }

        public static string[] IniFilterNameSet()
        {
            //Figure out the filter mapping
            //from the AppSetting.ini -> ImageProfile.ini file in use
            // and read it in
            List<string> iniFilters = new List<string>();
            string profilePath = ExoScanFileManager.GetImagingProfileFilePath();
            if (profilePath == null)
                return null;
            IniReadWrite iniDataFile = new IniReadWrite(profilePath);
            for (int filterIndex = 0; filterIndex < 120; filterIndex++)
            {
                string iniDataName = "\\Filter" + filterIndex.ToString("0");
                string iniFilterName = iniDataFile.GetData(iniDataName);
                if (iniFilterName == null)
                    break;
                iniFilters.Add(iniFilterName);
            }
            return iniFilters.ToArray();
        }

        public static int? IniLookUpFilterIndex(string filterName)
        {
            string[] fnl = IniFilterNameSet();
            if (fnl == null)
                return null;
            for (int i = 0; i < fnl.Length; i++)
                if (fnl[i].Contains(filterName))
                    return i;
            return null;
        }


        //Filter methods that use TSX ccdSoftCamera methods for the filter wheel
        public static string[] FilterNameSet()
        {
            //Figure out the filter mapping
            //Find the filter name for the filter filter Number
            ccdsoftCamera tsxc = new ccdsoftCamera();
            try { tsxc.Connect(); }
            catch { return null; }
            int filterCount = tsxc.lNumberFilters;
            string[] TSXFilterList = new string[filterCount];
            for (int f = 0; f < filterCount; f++)
                TSXFilterList[f] = (tsxc.szFilterName(f));
            return TSXFilterList;
        }

        public static string LookUpFilterName(int filterIndex)
        {
            ccdsoftCamera tsxc = new ccdsoftCamera();
            return (tsxc.szFilterName(filterIndex));
        }

        public static int? LookUpFilterIndex(string filterName)
        {
            string[] fnl = IniFilterNameSet();
            if (fnl == null)
                return null;
            for (int i = 0; i < fnl.Length; i++)
                if (fnl[i].Contains(filterName))
                    return i;
            return null;
        }


    }
}
