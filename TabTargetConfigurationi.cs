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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ExoScan
{
    //public partial class FormCreateTargetList : Form
    public partial class FormSessionManager
    {

        public List<string> tgtZeroBasedFilters;
        public List<string> fwZeroBasedFilters;

        public SearchSwarthmore swarthmoreSearch = null;

        // public FormCreateTargetList()
        public void InitializeTargetTab()
        {
            //InitializeComponent();
            //fill Collection list
            Configuration cfg = new Configuration();
            InitializeTargetCollection();
            string currentCollection = CollectionManagement.ActiveCollection();
            PopulateCollectionBox(currentCollection);
            return;
        }

        private void PopulateCollectionBox(string collectionName)
        {
            //List the existing collections in the Collction Box
            //  Reset the selected index to the currnt collection
            int? cdx = null;
            ExoTargetsBox.Items.Clear();
            List<string> cList = CollectionManagement.ListCollections();
            for (int c = 0; c < cList.Count; c++)
            {
                ExoTargetsBox.Items.Add(cList[c]);
                if (cList[c] == collectionName)
                    cdx = c;
            }
            if (cdx != null)
                ExoTargetsBox.SelectedIndex = (int)cdx;
            return;
        }

        private void InitializeTargetCollection()
        {
            Configuration cfg = new Configuration();
            if (CollectionManagement.HasCollection(CollectionManagement.ActiveCollection()))
                ActiveCollectionBox.Text = CollectionManagement.ActiveCollection();
            else
                ActiveCollectionBox.Text = "No Collection";
            if (File.Exists(cfg.TargetListPath))
            {
                //file exists so populate window accordingly
                TargetXList tList = new TargetXList();
                List<TargetXList.TargetXDescriptor> tXList = tList.GetTargetXList();
                TargetListDropDown.Items.Clear();
                foreach (TargetXList.TargetXDescriptor tX in tXList)
                    TargetListDropDown.Items.Add(tX.Name);
                if (TargetListDropDown.Items.Count > 0)
                    TargetListDropDown.Text = TargetListDropDown.Items[0].ToString();
                //Load current filter wheel list into filter selection box
                ColorIndexing colorIndex = new ColorIndexing();
                FilterListBox.Items.Clear();
                fwZeroBasedFilters = Filters.IniFilterNameSet().ToList();
                FilterListBox.Items.AddRange(fwZeroBasedFilters.ToArray());
                //Mark currently assigned filters in filter selection box
                tgtZeroBasedFilters = colorIndex.GetSessionFilters();
                if (tgtZeroBasedFilters.Count > 0 && fwZeroBasedFilters.Count > 0)
                {
                    for (int t = 0; t < tgtZeroBasedFilters.Count; t++)
                        for (int f = 0; f < fwZeroBasedFilters.Count; f++)
                            if (fwZeroBasedFilters[f].Contains(tgtZeroBasedFilters[t]))
                                FilterListBox.SetItemChecked(f, true);
                }
            }
            else                //New target selection
            {
                TargetListDropDown.Items.Clear();
                FilterListBox.Items.Clear();
                fwZeroBasedFilters = Filters.IniFilterNameSet().ToList();
                //Add to list box
                if (fwZeroBasedFilters.Count > 0)
                    FilterListBox.Items.AddRange(fwZeroBasedFilters.ToArray());
            }
        }

        private void AddTSXTargetButton_Click(object sender, EventArgs e)
        {
            //Add contents of target list field to target list
            //Read in from field
            string newTgtName = TargetListDropDown.Text;
            //Look up target from TSX and get Ra and Dec
            double ra; double dec;
            if (!CollectionManagement.HasCollection(newTgtName))
            {
                //Create collection
                CollectionManagement.CreateCollection(newTgtName);
                //Open collection
                string tgtListPath = CollectionManagement.OpenCollection(newTgtName);
                try { (ra, dec) = TSX_Resources.FindTarget(newTgtName); }
                catch
                {
                    MessageBox.Show("Look up of target failed");
                    return;
                }
                //Add new target
                TargetXList.AddToTargetXList(newTgtName, ra, dec, DateTime.Now);
                //Redraw collection window
                InitializeCollection();
                PopulateCollectionBox(newTgtName);
                return;
            }
            else
                return;
        }

        private void AddSwarthmoreTargetButton_Click(object sender, EventArgs e)
        {
            //Add new target to collections
            //  If the target already exists, then we're done
            //  If not it must have been found in the swarthmore search or it wouldn't be in the list
            //      so create a new collection
            string newTgtName = TargetListDropDown.Text;
            //string newTgtName = TargetListDropDown.SelectedItem.ToString();
            double ra; double dec;
            DateTime strans, etrans;
            if (!CollectionManagement.HasCollection(newTgtName))
            {
                //Create collection
                CollectionManagement.CreateCollection(newTgtName);
                //Open collection
            }
            else //already has collection -- updating data
            {
                string tgtListPath = CollectionManagement.OpenCollection(newTgtName);
                if (swarthmoreSearch == null)
                {
                    swarthmoreSearch = new SearchSwarthmore();
                    bool hasTargets = swarthmoreSearch.GetAndSet();
                }
            }
            (ra, dec) = swarthmoreSearch.FindCoordinates(newTgtName);
            (strans, etrans) = swarthmoreSearch.FindTransitTimes(newTgtName);
            //Delete old target data, if any
            TargetXList.DeleteFromTargetXList(newTgtName);
            //Add new target
            TargetXList.AddToTargetXList(newTgtName, ra, dec, DateTime.Now, strans, etrans);
            //Redraw collection window
            InitializeCollection();
            PopulateCollectionBox(newTgtName);
            return;

        }

        private void ExoTargetsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            TargetListDropDown.Items.Clear();
            TargetListDropDown.Items.Add(ExoTargetsBox.Text);
            TargetListDropDown.SelectedIndex = 0;
        }

        private void FilterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Save filter configurations
            Configuration cfg = new Configuration();
            ColorIndexing clist = new ColorIndexing();
            List<Filters.ActiveFilter> afList = new List<Filters.ActiveFilter>();
            for (int i = 0; i < FilterListBox.CheckedItems.Count; i++)
            {
                afList.Add(new Filters.ActiveFilter()
                {
                    FilterName = FilterListBox.CheckedItems[i].ToString(),
                    FilterIndex = (int)Filters.IniLookUpFilterIndex(FilterListBox.CheckedItems[i].ToString())
                });
            }
            if (afList.Count > 0)
                clist.SaveActiveFilters(afList);
            return;
        }

        private void SwarthmoreButton_Click(object sender, EventArgs e)
        {
            //string swarthmoreURL = @"https://astro.swarthmore.edu/transits/transits.cgi"
            System.Diagnostics.Process.Start("https://astro.swarthmore.edu/transits/transits.cgi");
            swarthmoreSearch = new SearchSwarthmore();
            bool hasTargets = swarthmoreSearch.GetAndSet();
            List<string> ptargs = swarthmoreSearch.PotentialTargetList();
            if (ptargs.Count > 0)
            {
                TargetListDropDown.Items.Clear();
                foreach (string tgt in ptargs)
                    TargetListDropDown.Items.Add(tgt);
            }
            //System.Diagnostics.Process.Start("https://astro.swarthmore.edu/transits/transits.cgi");
            TargetListDropDown.DroppedDown = true;
            Show();
        }

        private void TargetListDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TargetListDropDown.SelectedItem != null)
                CollectionManagement.OpenCollection(TargetListDropDown.SelectedItem.ToString());
            SwarthmoreButton.Enabled = true;
            TargetListDropDown.Items.Clear();
            FilterListBox.Items.Clear();
            fwZeroBasedFilters = Filters.IniFilterNameSet().ToList();
            //Add to list box
            if (fwZeroBasedFilters.Count > 0)
                FilterListBox.Items.AddRange(fwZeroBasedFilters.ToArray());

            //Load current filter wheel list into filter selection box
            ColorIndexing colorIndex = new ColorIndexing();
            FilterListBox.Items.Clear();
            fwZeroBasedFilters = Filters.IniFilterNameSet().ToList();
            FilterListBox.Items.AddRange(fwZeroBasedFilters.ToArray());
            //Mark currently assigned filters in filter selection box
            tgtZeroBasedFilters = colorIndex.GetSessionFilters();
            if (tgtZeroBasedFilters.Count > 0 && fwZeroBasedFilters.Count > 0)
            {
                for (int t = 0; t < tgtZeroBasedFilters.Count; t++)
                    for (int f = 0; f < fwZeroBasedFilters.Count; f++)
                        if (fwZeroBasedFilters[f].Contains(tgtZeroBasedFilters[t]))
                            FilterListBox.SetItemChecked(f, true);
            }

            return;
        }

    }

}

