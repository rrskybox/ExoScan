/*
* SearchEXO Class
*
* Class for downloading and parsing Exo Planet database query results
* 
* This class serves as method template for conversions from all 
*  catalog sources
* 
* Author:           Rick McAlister
* Date:             4/23/21
* Current Version:  1.0
* Developed in:     Visual Studio 2019
* Coded in:         C# 8.0
* App Envioronment: Windows 10 Pro, .Net 4.8, TSX 5.0 Build 12978
* 
* Support documents
* 
* Table Access Protocol: https://exoplanetarchive.ipac.caltech.edu/docs/TAP/usingTAP.html
* 
* Change Log:
* 
* 4/23/21 Rev 1.0  Release
* 
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ExoScan
{
    public class SearchSwarthmore
    {
        const string URL_Swarthmore_search = "https://astro.swarthmore.edu/transits/print_transits.cgi?";

        const string SCsingle_object = "single_object";
        const string SCra = "ra";
        const string SCdec = "dec";
        const string SCepoch = "epoch";
        const string SCperiod = "period";
        const string SCduration = "duration";
        const string SCdepth = "depth";
        const string SCtarget = "target";
        const string SCobservatory_string = "observatory_string";
        const string SCuse_utc = "use_utc";
        const string SCobservatory_latitude = "observatory_latitude";
        const string SCobservatory_longitude = "observatory_longitude";
        const string SCtimezone = "timezone";
        const string SCstart_date = "start_date";
        const string SCdays_to_print = "days_to_print";
        const string SCdays_in_past = "days_in_past";
        const string SCminimum_start_elevation = "minimum_start_elevation";
        const string SCand_vs_or = "and_vs_or";
        const string SCminimum_end_elevation = "minimum_end_elevation";
        const string SCminimum_ha = "minimum_ha";
        const string SCmaximum_ha = "maximum_ha";
        const string SCbaseline_hrs = "baseline_hrs";
        const string SCshow_unc = "show_unc";
        const string SCminimum_depth = "minimum_depth";
        const string SCmaximum_V_mag = "maximum_V_mag";
        const string SCtarget_string = "target_string";
        const string SCprint_html = "print_html";
        const string SCtwilight = "twilight";
        const string SCmax_airmass = "max_airmass";
        const string SCfovWidth = "fovWidth";
        const string SCfovHeight = "fovHeight";
        const string SCfovPA = "fovPA";

        const string TLRootX = "Swarthmore_Targets";
        const string TLTargetDataX = "SD_Target";
        const string TH_NameX = "Name";

        private string swarthmoreResults;
        private XElement swarthmoreXResults;

        public bool GetAndSet()
        {

            //Import Exo VOTable query and convert to an SDB XML database
            swarthmoreResults = ServerQueryToResultsCSV();
            if (swarthmoreResults == null)
                return false;
            swarthmoreXResults = CSVToXML(swarthmoreResults);
            //Check then cull results of any entry that has no planetary period
            return true;
        }

        public List<string> PotentialTargetList()
        {
            //return list of swarthmore targets
            return swarthmoreXResults.Elements(TLTargetDataX).Descendants(TH_NameX).Select(e => e.Value).ToList();
        }

        private string ServerQueryToResultsCSV()
        {
            // url of exo APR
            // 
            string exoResults = "";
            WebClient client = new WebClient();

            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            ExoPlanetSurvey exops = new ExoPlanetSurvey();
            string exoURLquery = URL_Swarthmore_search + MakeSearchQuery(exops);
            int t = exoURLquery.CompareTo(exoURLquery);

            try
            {
                exoResults = client.DownloadString(exoURLquery);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download Error: " + ex.Message);
                return null;
            };

            //EXO parse
            return exoResults;
        }

        private XElement CSVToXML(string dataCSV)
        {
            //Convert a CSV string to XML
            char[] hcharsToTrim = { '"', '\\', ')' };
            char[] dcharsToTrim = { '"', '\\' };
            using StringReader sb = new StringReader(dataCSV);
            //Read in first line as set up for XML record names
            List<string> header = sb.ReadLine().Split(new char[] { ',' }).ToList();
            XElement dataXML = new XElement(TLRootX);
            while (sb.Peek() != -1)
            {
                XElement lineXML = new XElement(TLTargetDataX);
                List<string> lineData = sb.ReadLine().Split(new char[] { ',' }).ToList();
                //clean up the listing so it can be used in xml

                for (int i = 0; i < lineData.Count; i++)
                {
                    string hdr = header[i].Trim(hcharsToTrim).Replace(" ", "_").Replace("(", "_");
                    string entry = lineData[i].Trim(dcharsToTrim);
                    lineXML.Add(new XElement(hdr, entry));
                }
                dataXML.Add(lineXML);
            }
            return dataXML;
        }

        private string MakeSearchQuery(ExoPlanetSurvey exops)
        {
            //Returns a url string for querying the Exo website
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString[SCsingle_object] = exops.EPsingle_object.ToString();
            queryString[SCra] = exops.EPra.ToString().ToString();
            queryString[SCdec] = exops.EPdec.ToString();
            queryString[SCepoch] = exops.EPepoch.ToString();
            queryString[SCperiod] = exops.EPperiod.ToString();
            queryString[SCduration] = exops.EPduration.ToString();
            queryString[SCdepth] = exops.EPdepth.ToString();
            queryString[SCtarget] = exops.EPtarget.ToString();
            queryString[SCobservatory_string] = exops.EPobservatory_string.ToString();
            queryString[SCuse_utc] = exops.EPuse_utc.ToString();
            queryString[SCobservatory_latitude] = exops.EPobservatory_latitude.ToString();
            queryString[SCobservatory_longitude] = exops.EPobservatory_longitude.ToString();
            queryString[SCtimezone] = exops.EPtimezone.ToString();
            queryString[SCstart_date] = exops.EPstart_date.ToString();
            queryString[SCdays_to_print] = exops.EPdays_to_print.ToString();
            queryString[SCdays_in_past] = exops.EPdays_in_past.ToString();
            queryString[SCminimum_start_elevation] = exops.EPminimum_start_elevation.ToString();
            queryString[SCand_vs_or] = exops.EPand_vs_or.ToString();
            queryString[SCminimum_end_elevation] = exops.EPminimum_end_elevation.ToString();
            queryString[SCminimum_ha] = exops.EPminimum_ha.ToString();
            queryString[SCmaximum_ha] = exops.EPmaximum_ha.ToString();
            queryString[SCbaseline_hrs] = exops.EPbaseline_hrs.ToString();
            queryString[SCshow_unc] = exops.EPshow_unc.ToString();
            queryString[SCminimum_depth] = exops.EPminimum_depth.ToString();
            queryString[SCmaximum_V_mag] = exops.EPmaximum_V_mag.ToString();
            queryString[SCtarget_string] = exops.EPtarget_string.ToString();
            queryString[SCprint_html] = exops.EPprint_html.ToString();
            queryString[SCtwilight] = exops.EPtwilight.ToString();
            queryString[SCmax_airmass] = exops.EPmax_airmass.ToString();
            queryString[SCfovWidth] = exops.EPfovWidth.ToString();
            queryString[SCfovHeight] = exops.EPfovHeight.ToString();
            queryString[SCfovPA] = exops.EPfovPA.ToString();

            return queryString.ToString(); // Returns "key1=value1&key2=value2", all URL-encoded
        }

        public struct ExoPlanetSurvey
        {
            public string EPsingle_object = "0";
            public double EPra = 0;
            public double EPdec = 0;
            public string EPepoch = "";
            public double EPperiod = 0;
            public double EPduration = 0;
            public double EPdepth = 0;
            public string EPtarget = "";
            public string EPobservatory_string = "Specified_Lat_Long";
            public string EPuse_utc = "0";
            public double EPobservatory_latitude = 19.828333;
            public double EPobservatory_longitude = -155.478333;
            public string EPtimezone = "HST";
            public string EPstart_date = DateTime.Now.ToString("MM-dd-yyyy");
            public int EPdays_to_print = 1;
            public int EPdays_in_past = 0;
            public int EPminimum_start_elevation = 30;
            public int EPminimum_end_elevation = 30;
            public int EPminimum_ha = -12;
            public int EPmaximum_ha = 12;
            public string EPand_vs_or = "or";
            public int EPbaseline_hrs = 1;
            public int EPshow_unc = 1;
            public int EPminimum_depth = 20;
            public double EPmaximum_V_mag = 14;
            public string EPtarget_string = "";
            public string EPprint_html = "2";
            public int EPtwilight = -12;
            public double EPmax_airmass = 2.4;
            public string EPfovWidth = "";
            public string EPfovHeight = "";
            public string EPfovPA = "";

            public ExoPlanetSurvey()
            {
            }
        }

        public enum XPDataNum
        {
            Name = 0,
            V,
            start_time,
            mid_time,
            end_time,
            duration_hours,
            jd_start,
            jd_mid,
            jd_end,
            el_start,
            el_mid,
            el_end,
            az_start,
            az_mid,
            az_end,
            ha_start,
            ha_mid,
            ha_end,
            coords_J2000,
            depth_ppt,
            priority,
            period_days,
            period_unc_days0,
            T_0_BJD_TDB,
            T_0_unc,
            comments,
            percent_transit_observable,
            percent_baseline_observable,
            obs_start_time,
            obs_end_time,
            moon_percent,
            moon_dist_deg
        }

        public (double, double) FindCoordinates(string tgtName)
        {
            double ra = 0, dec = 0;
            var tgtX = swarthmoreXResults.Elements(TLTargetDataX).Where(e => e.Element("Name").Value == tgtName).First();
            string[] radec = tgtX.Element("coords_J2000").Value.Split(' ');

            ra = (double)Utility.ParseRADecString(radec[0]);
            dec = (double)Utility.ParseRADecString(radec[1]);
            return (ra, dec);
        }

        public (DateTime, DateTime) FindTransitTimes(string tgtName)
        {
            //Read in the exoplanet transit times
            var tgtX = swarthmoreXResults.Elements(TLTargetDataX).Where(e => e.Element("Name").Value == tgtName).First();
            DateTime startTime = Convert.ToDateTime(tgtX.Element("start_time").Value);
            DateTime endTime = Convert.ToDateTime(tgtX.Element("end_time").Value);
            return (startTime, endTime);
        }
    }
}
