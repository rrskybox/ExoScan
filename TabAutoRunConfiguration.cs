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
using System.Windows.Forms;

namespace ExoScan
{
    //public partial class FormAutoRun
    public partial class FormSessionManager
    {
        private DateTime? dusk;
        private DateTime? dawn;

        //public FormAutoRun()
        public void InitializeAutoRunTab()
        {
            //When an instance of the autostart form is created, 
            // the text boxes -- start up filepath, shutdown filepath, start up time
            // are filled in from the AGNSurvey configuration file.  That//s it.

            //InitializeComponent();
            //if (sddt > StartingDateTimePicker.Value) ShutdownDateTimePicker.Value = sddt;
            //else ShutdownDateTimePicker.Value = sddt.AddDays(1);
            //Get dusk and dawn times
            (dusk, dawn) = TSX_Resources.GetTwilight();
            ResyncSchedule();
        }

        public void UpdateAutoRunEntries(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            StagingEnabled.Checked = Convert.ToBoolean(cfg.StageSystemOn);
            StageSystemFilePathBox.Text = cfg.StageSystemPath;
            StagingDateTimePicker.Value = Convert.ToDateTime(cfg.StageSystemTime);

            StartupEnabled.Checked = Convert.ToBoolean(cfg.StartUpOn);
            StartUpFilePathBox.Text = cfg.StartUpPath;
            StartingDateTimePicker.Value = Convert.ToDateTime(cfg.StartUpTime);

            ShutDownEnabled.Checked = Convert.ToBoolean(cfg.ShutDownOn);
            ShutDownFilePathBox.Text = cfg.ShutDownPath;
            ShutdownDateTimePicker.Value = Convert.ToDateTime(cfg.ShutDownTime);
        }

        private void StageSystemBrowseButton_Click(object sender, EventArgs e)
        {
            //Upon clicking the Browse button on the Stage System filename box,
            //  A file selection dialog is run to pick up a filepath for the
            //  system staging file.  The result, if chosen, is entered in the stage System filename box
            //  in the form, and the AGNSurvey configuration file updated accordingly.

            DialogResult stageSystemPathDiag = StageSystemFileDialog.ShowDialog();
            if (stageSystemPathDiag == System.Windows.Forms.DialogResult.OK)
            {
                Configuration cfg = new Configuration();
                cfg.StageSystemPath = StageSystemFileDialog.FileName;
                StageSystemFilePathBox.Text = StageSystemFileDialog.FileName;
                return;
            }
        }

        private void StartUpBrowseButton_Click(object sender, EventArgs e)
        {
            //Upon clicking the Browse button on the Start Up filename box,
            //  A file selection dialog is run to pick up a filepath for the
            //  start up file.  The result, if chosen, is entered in the start up filename box
            //  in the form, and the AGNSurvey configuration file updated accordingly.

            DialogResult startUpPathDiag = StartUpFileDialog.ShowDialog();
            if (startUpPathDiag == System.Windows.Forms.DialogResult.OK)
            {
                Configuration cfg = new Configuration();
                cfg.StartUpPath = StartUpFileDialog.FileName;
                StartUpFilePathBox.Text = StartUpFileDialog.FileName;
                return;
            }
        }

        private void ShutDownBrowseButton_Click(object sender, EventArgs e)
        {
            //Upon clicking the Browse button on the Shutdown filename box,
            //  A file selection dialog is run to pick up a filepath for the
            //  shutdown file.  The result, if chosen, is entered in the shutdown filename box
            //  in the form, and the AGNSurvey configuration file updated accordingly.

            DialogResult shutDownPathDiag = ShutDownFileDialog.ShowDialog();
            if (shutDownPathDiag == System.Windows.Forms.DialogResult.OK)
            {
                Configuration cfg = new Configuration();
                cfg.ShutDownPath = ShutDownFileDialog.FileName;
                ShutDownFilePathBox.Text = ShutDownFileDialog.FileName;
                return;
            }
        }

        private void ResyncSchedule()
        {
            //Values for stage, start and shutdown times are reset to the current datetime, if
            //their current values precede the current datetime.
            //Otherwise, it is assumpted that the operator knows what he/she is doing

            Configuration cfg = new Configuration();
            //Store Program times for each
            //  Get the time from the form
            //Acquire the three launch times, stage, start, end
            DateTime sst = Convert.ToDateTime(cfg.StageSystemTime);
            DateTime sut = Convert.ToDateTime(cfg.StartUpTime);
            DateTime sdt = Convert.ToDateTime(cfg.ShutDownTime);

            //if stage launch time is earlier than current time then set the launch time to now
            if (sst < DateTime.Now)
                sst = DateTime.Now;
            //if start launch time is earlier than current time then set the launch time to the later of now or dusk
            if (sut < DateTime.Now)
            {
                if (dusk != null && (DateTime)dusk > DateTime.Now)
                    sut = (DateTime)dusk;
                else
                    sut = DateTime.Now;
            }

            //if the end time is earlier than startup time then set the end time to dawn
            if ((dawn != null) && ((sdt <= sut) || sdt > (DateTime)dawn))
                sdt = (DateTime)dawn;
            ////figure out how long this session is going to be.  If greater than a day, cut the
            //// shut down time by one day.
            //while ((sdt - sut).Days >= 1) sdt = sdt.AddDays(-1);
            ////Save the entered datetimes to the configuration.xml file and close out
            StagingDateTimePicker.Value = sst;
            StartingDateTimePicker.Value = sut;
            ShutdownDateTimePicker.Value = sdt;
            return;
        }

        private void StagingDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.StageSystemTime = StagingDateTimePicker.Value.ToString("yyyy/MM/dd HH:mm:ss");
            return;
        }

        private void StartingDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.StartUpTime = StartingDateTimePicker.Value.ToString("yyyy/MM/dd HH:mm:ss");
            return;
        }

        private void ShutdownDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.ShutDownTime = ShutdownDateTimePicker.Value.ToString("yyyy/MM/dd HH:mm:ss");
            return;
        }

        private void StagingEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.StageSystemOn = StagingEnabled.Checked.ToString();
        }

        private void StartupEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.StartUpOn = StartupEnabled.Checked.ToString();
        }

        private void ShutDownEnabled_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.ShutDownOn = ShutDownEnabled.Checked.ToString();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            //Reset the times to defaults:
            //  Staging = current time
            //  StartUp = current time or dusk
            //  ShutDown = dawn
            Configuration cfg = new Configuration();
            cfg.StageSystemTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            cfg.StartUpTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            cfg.ShutDownTime = ((DateTime)dawn).ToString("yyyy/MM/dd HH:mm:ss");
            ResyncSchedule();
        }
    }
}

