using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using System.Threading.Tasks;

namespace CaseManagement.Forms
{
    public partial class frmGoogleDriveListFilesInAssignedFolder : BaseForm
    {
        public frmGoogleDriveListFilesInAssignedFolder()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //this.circularProgress.Visible = true;
            //this.circularProgress.IsRunning = true;
            this.dgvData.Rows.Clear();

            List<Google.Apis.Drive.v2.Data.File> FilterFiles = new List<Google.Apis.Drive.v2.Data.File>();
            //Task task = new Task(() =>
            //{
                FilterFiles = MainClass.AuthorizeAndListDirectoryFiles(this.txtDirectory.Text.Trim());
            //});
            //task.ContinueWith((x) =>
            //{
                foreach (Google.Apis.Drive.v2.Data.File File in FilterFiles)
                {
                    List<object> source = new List<object>();

                    source.Add(File.Title);
                    source.Add(File.AlternateLink);

                    int idx = this.dgvData.Rows.Add(source.ToArray());
                }
                //this.circularProgress.Visible = false;
                //this.circularProgress.IsRunning = false;
            //}, System.Threading.CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext()).ContinueWith((y) =>
            //{
            //    this.circularProgress.Visible = false;
            //    this.circularProgress.IsRunning = false;
            //}, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());                
        }
    }
}
