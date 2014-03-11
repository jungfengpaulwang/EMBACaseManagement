using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using FISCA.Data;
using DevComponents.Editors;
using System.Threading.Tasks;
using EMBA.Export;
using Aspose.Cells;

namespace CaseManagement.Export
{
    public partial class CSAttend : EMBA.Export.ExportProxyForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private bool form_loaded;

        public CSAttend()
        {
            InitializeComponent();

            this.Access = new AccessHelper();
            this.Query = new QueryHelper();

            InitializeData();

            this.Load += new EventHandler(CSAttendResult_Load);
        }

        private void CSAttendResult_Load(object sender, EventArgs e)
        {
            form_loaded = false;

            //base.OnLoad(e);
            Access = new AccessHelper();

            this.ResolveField();
            form_loaded = true;
        }

        private void InitializeData()
        {
            this.AutoSaveFile = false;
            this.AutoSaveLog = false;   //  Log 要用新的寫法
            //this.KeyField = "";
            //this.InvisibleFields = null;

            //this.ReplaceFields = null;
            this.QuerySQL = SetQueryString();
            this.TitleText = "匯出個案資料";
            this.Text = "匯出個案資料";
            base.TitleText = "匯出個案資料";
            base.Text = "匯出個案資料";
        }

        private string SetQueryString()
        {
            string querySQL = string.Format(@"select english_name as 個案英文名稱, name as 個案中文名稱, no as 個案編號, author as 作者, publish_school as 出版學校, memo as 備註, url_list as 個案文件連結 from $ischool.emba.case_management.case");

            return querySQL;
        }

        protected override void OnExportButtonClick(object sender, EventArgs e)
        {            
            this.btnExport.Enabled = false;
            this.circularProgress.IsRunning = true;
            this.circularProgress.Visible = true;
            Workbook wb = new Workbook();
            Task task = Task.Factory.StartNew(() =>
            {
                DataTable dataTable = Query.Select(this.QuerySQL);

                //foreach (DataRow row in dataTable.Rows)
                //{
                //    row["授課教師"] = (dicCourseTeachers.ContainsKey(row["課程系統編號"] + "") ? dicCourseTeachers[row["課程系統編號"] + ""] : "");
                //}
                wb = dataTable.ToWorkbook(true, this.SelectedFields);
            });
            task.ContinueWith((x) =>
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "另存新檔";
                sfd.FileName = "匯出個案資料.xls";
                sfd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                DialogResult dr = sfd.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.OK)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.None;
                    return;
                }

                try
                {
                    wb.Save(sfd.FileName);
                    if (System.IO.File.Exists(sfd.FileName))
                        System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "錯誤");
                }

                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;

            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }
    }
}
