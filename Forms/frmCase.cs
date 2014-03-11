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
using FISCA.UDT;
using System.Dynamic;
using System.Xml.Linq;
using K12.Data.Configuration;

namespace CaseManagement.Forms
{
    public partial class frmCase : BaseForm
    {
        private AccessHelper Access;
        private List<DataItems.CloudFileUrlItem> GoogleDocsRowSource;
        private ConfigData config; 
        private BackgroundWorker BW;
        private List<UDT.Case> Cases;

        public frmCase()
        {
            InitializeComponent();
            GoogleDocsRowSource = new List<DataItems.CloudFileUrlItem>();
            BW = new BackgroundWorker();
            BW.DoWork += new DoWorkEventHandler(BW_DoWork);
            BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BW_RunWorkerCompleted);
            BW.WorkerSupportsCancellation = true;
            Access = new AccessHelper();
            Cases = new List<UDT.Case>();
            this.Load += new EventHandler(frmCase_Load);
            this.FormClosed += new FormClosedEventHandler(frmCase_FormClosed);
        }

        private void frmCase_Load(object sender, EventArgs e)
        {
            this.InitCase();

            config = K12.Data.School.Configuration["台大EMBA個案文件雲端資料夾"];

            if (config != null)
                this.txtDocCloudDirectory.Text = config["CloudFolder"];
        }

        private void frmCase_FormClosed(object sender, EventArgs e)
        {
            BW.CancelAsync();
        }

        private void InitCase()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                this.Cases = Access.Select<UDT.Case>().OrderBy(x => int.Parse(x.UID)).ToList();
            });
            task.ContinueWith((x) =>
            {
                this.FilterCase();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //建立 DataGrid 欄位
        private void SyncColumns(int DocumentCount)
        {
            int columnCount = this.dgvData.Columns.Count;
            for (int i = 6; i < columnCount; i++)
            {
                this.RemoveColumn(this.dgvData.Columns.Count - 1);
            }

            for (int i = 0; i < DocumentCount; i++)
            {
                int columnIndex = this.dgvData.Columns.Add(this.MakeColumn(i + 1));
                this.dgvData.Columns[columnIndex].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        private void RemoveColumn(int columnIndex)
        {
            this.dgvData.Columns.RemoveAt(columnIndex);
        }

        private DataGridViewColumn MakeColumn(int documentIndex)
        {
            DataGridViewColumn col = new DataGridViewLinkColumn();
            col.HeaderText = "文件名稱" + documentIndex;
            col.Name = "document" + documentIndex;
            col.Width = 60;
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            return col;
        }

        private void FilterCase(string search_string = "")
        {
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            this.dgvData.Rows.Clear();
            int max_column_count = 0;
            bool case_name_focused = this.txtCaseName.Focused;
            bool author_focused = this.txtAuthor.Focused;

            Task task = Task.Factory.StartNew(() =>
            {
                //  <CaseDocument FileName='{0}' URL='{1}' Memo='{2}' />
                if (this.Cases.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(search_string))
                        max_column_count = Cases.Where(x => !string.IsNullOrWhiteSpace(x.UrlList)).Select(x => (XDocument.Parse(x.UrlList.Replace("&", "&#038;")).Descendants("CaseDocument")).Count()).Max();
                    else
                    {
                        if (case_name_focused)
                            max_column_count = Cases.Where(x => (x.EnglishName.Trim().ToLower().Contains(search_string.Trim().ToLower()) || x.Name.Trim().ToLower().Contains(search_string.Trim().ToLower()))).Where(x => !string.IsNullOrWhiteSpace(x.UrlList)).Select(x => (XDocument.Parse(x.UrlList.Replace("&", "&#038;")).Descendants("CaseDocument")).Count()).Max();
                        if (author_focused)
                            max_column_count = Cases.Where(x => x.Author.Trim().ToLower().Contains(search_string.Trim().ToLower())).Where(x => !string.IsNullOrWhiteSpace(x.UrlList)).Select(x => (XDocument.Parse(x.UrlList.Replace("&", "&#038;")).Descendants("CaseDocument")).Count()).Max();
                    }
                }
            });
            task.ContinueWith((x) =>
            {
                if (this.Cases.Count == 0)
                {
                    goto TheEnd;
                }
                this.SyncColumns(max_column_count);
                Cases.ForEach((y) =>
                {
                    if (!string.IsNullOrWhiteSpace(search_string))
                    {
                        if (this.txtCaseName.Focused)
                        {
                            if (!y.EnglishName.Trim().ToLower().Contains(search_string.Trim().ToLower()) && !y.Name.Trim().ToLower().Contains(search_string.Trim().ToLower()))
                                goto Break;
                        }
                        if (this.txtAuthor.Focused)
                        {
                            if (!y.Author.Trim().ToLower().Contains(search_string.Trim().ToLower()))
                                goto Break;
                        }
                    }

                    List<object> source = new List<object>();
                    List<string> urls = new List<string>();

                    source.Add(y.EnglishName);
                    source.Add(y.Name);
                    source.Add(y.No);
                    source.Add(y.Author);
                    source.Add(y.PublishSchool);
                    source.Add(y.Memo);

                    if (string.IsNullOrEmpty(y.UrlList))
                    {
                        for (int i = 0; i < max_column_count; i++)
                        {
                            source.Add(string.Empty);
                            urls.Add(string.Empty);
                        }
                    }
                    else
                    {
                        XDocument xDocument = XDocument.Parse(y.UrlList.Replace("&", "&#038;"));
                        IEnumerable<XElement> xElements = xDocument.Descendants("CaseDocument");
                        int j = 0;
                        foreach (XElement xElement in xElements)
                        {
                            string Url = xElement.Attribute("URL").Value;
                            if (string.IsNullOrWhiteSpace(Url))
                                urls.Add(string.Empty);
                            else
                            {
                                byte[] utf8Bytes = Convert.FromBase64String(Url);
                                string Url_Decode = Encoding.UTF8.GetString(utf8Bytes);
                                urls.Add(Url_Decode);
                            }
                            source.Add(xElement.Attribute("FileName").Value);
                            j++;
                        }
                        for (int i = j; i < max_column_count; i++)
                        {
                            source.Add(string.Empty);
                            urls.Add(string.Empty);
                        }
                    }
                    int idx = this.dgvData.Rows.Add(source.ToArray());
                    for (int i = 0; i < max_column_count; i++)
                    {
                        this.dgvData.Rows[idx].Cells[i+6].Tag = urls.ElementAt(i);
                    }
                    this.dgvData.Rows[idx].Tag = y;
                Break:
                    ;
                });
                TheEnd:
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            if (this.GoogleDocsRowSource.Count == 0)
            {
                MessageBox.Show("請先取得文件連結。", "錯誤");
                return;
            }

            if ((new Forms.frmCase_SingleForm(null, this.GoogleDocsRowSource)).ShowDialog() == System.Windows.Forms.DialogResult.OK)
                this.InitCase();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (this.dgvData.SelectedRows.Count == 0)
            {
                MessageBox.Show("請單選項目。");
                return;
            }
            if (this.dgvData.SelectedRows.Count > 1)
            {
                MessageBox.Show("僅能單筆修改。");
                return;
            }
            if (this.GoogleDocsRowSource.Count == 0)
            {
                this.GoogleDocsRowSource.Add(new DataItems.CloudFileUrlItem(null, null));
            }
            if (this.dgvData.SelectedRows.Count == 1)
            {
                UDT.Case Case = this.dgvData.SelectedRows[0].Tag as UDT.Case;
                Forms.frmCase_SingleForm frm = new Forms.frmCase_SingleForm(Case, this.GoogleDocsRowSource);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    this.InitCase();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.dgvData.SelectedRows.Count == 0)
            {
                MessageBox.Show("請選取項目(可複選)。");
                return;
            }

            List<UDT.CaseUsage> CaseUsages = Access.Select<UDT.CaseUsage>();
            List<string> Usage_Case_IDs = new List<string>();
            if (CaseUsages.Count > 0)
                Usage_Case_IDs = CaseUsages.Select(x => x.CaseID.ToString()).Distinct().ToList();

            List<UDT.Case> Cases = new List<UDT.Case>();
            string Error_Message = "下列個案已有教師使用，不得刪除。\n\n";
            foreach (DataGridViewRow row in this.dgvData.SelectedRows)
            {
                UDT.Case Case = row.Tag as UDT.Case;
                if (Usage_Case_IDs.Contains(Case.UID))
                {
                    Error_Message += string.Format("個案英文名稱：{0}，個案中文名稱：{1}\n", row.Cells[0].Value + "", row.Cells[1].Value + "");
                }
                else
                {
                    Case.Deleted = true;
                    Cases.Add(Case);
                }
            }
            if (Error_Message == "下列個案已有教師使用，不得刪除。\n\n")
            {
                if (MessageBox.Show("確定刪除？", "警告", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                    return;

                Cases.SaveAll();
                this.InitCase();
            }
            else
                MessageBox.Show(Error_Message);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 6)
                return;

            string url = this.dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag + "";
            if (!string.IsNullOrWhiteSpace(url))
                System.Diagnostics.Process.Start(url);
        }

        private void btnGetCloudDocument_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtDocCloudDirectory.Text.Trim()))
            {
                MessageBox.Show("請先輸入「雲端資料夾名稱」。");
                return;
            }
            List<UDT.oAuthAccount> oAuthAccounts = Access.Select<UDT.oAuthAccount>();
            if (oAuthAccounts.Count == 0)
            {
                MessageBox.Show("請先設定「oAuth 帳號」。");
                return;
            }
            Google.Apis.Helper.ClientCredentials.CLIENT_ID = oAuthAccounts.ElementAt(0).ClientID;
            Google.Apis.Helper.ClientCredentials.CLIENT_SECRET = oAuthAccounts.ElementAt(0).Secret;
            this.GoogleDocsRowSource.Clear();
            this.GoogleDocsRowSource.Add(new DataItems.CloudFileUrlItem(null, null));

            this.circularProgress.IsRunning = true;
            this.circularProgress.Visible = true;
            this.btnGetCloudDocument.Enabled = false;
            this.btnAddNew.Enabled = false;
            BW.RunWorkerAsync(this.txtDocCloudDirectory.Text.Trim());
        }

        private void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Google.Apis.Drive.v2.Data.File> FilterFiles = new List<Google.Apis.Drive.v2.Data.File>();
            FilterFiles = MainClass.AuthorizeAndListDirectoryFiles(e.Argument + "");
            if (FilterFiles.Count == 0)
            {
                MainClass.DeleteCachedRefreshToken();
                throw new Exception("指定之「雲端資料夾」沒有檔案，請確認您的帳號可存取該資料夾。");
            }
            foreach (Google.Apis.Drive.v2.Data.File File in FilterFiles)
                this.GoogleDocsRowSource.Add(new DataItems.CloudFileUrlItem(File.Title, File.AlternateLink));
        }

        private void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);
            else
            {
                if (config != null)
                {
                    config["CloudFolder"] = this.txtDocCloudDirectory.Text.Trim();
                    config.Save();
                }
                MessageBox.Show("已成功取得文件連結。");
            }

            this.circularProgress.IsRunning = false;
            this.circularProgress.Visible = false;
            this.btnGetCloudDocument.Enabled = true;
            this.btnAddNew.Enabled = true;
        }

        private void txtCaseName_TextChanged(object sender, EventArgs e)
        {
            this.FilterCase(((TextBox)sender).Text.Trim());
        }

        private void txtAuthor_TextChanged(object sender, EventArgs e)
        {
            this.FilterCase(((TextBox)sender).Text.Trim());
        }
    }
}