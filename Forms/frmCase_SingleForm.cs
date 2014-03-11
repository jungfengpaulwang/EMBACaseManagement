using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using System.Dynamic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace CaseManagement.Forms
{
    public partial class frmCase_SingleForm : BaseForm
    {
        private UDT.Case Case;
        private ErrorProvider ErrorProvider;
        private AccessHelper Access;
        private List<DataItems.CloudFileUrlItem> GoogleDocsRowSource;

        public frmCase_SingleForm(UDT.Case Case, List<DataItems.CloudFileUrlItem> googleDocsRowSource)
        {
            InitializeComponent();
            this.Case = Case;
            this.GoogleDocsRowSource = googleDocsRowSource;
            Access = new AccessHelper();
            this.ErrorProvider = new ErrorProvider();
            this.dgvData.CellEnter += new DataGridViewCellEventHandler(dgvData_CellEnter);
            this.dgvData.CurrentCellDirtyStateChanged += new EventHandler(dgvData_CurrentCellDirtyStateChanged);
            this.dgvData.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgvData_EditingControlShowing);
            this.dgvData.DataError += new DataGridViewDataErrorEventHandler(dgvData_DataError);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);
            this.dgvData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_RowHeaderMouseClick);
            this.dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvData_MouseClick);
            this.Load += new EventHandler(frmCase_SingleForm_Load);
        }

        private void frmCase_SingleForm_Load(object sender, EventArgs e)
        {
            this.FileName.DataSource = this.GoogleDocsRowSource;
            this.FileName.ValueMember = "Url";
            this.FileName.DisplayMember = "Name";          
            if (this.Case == null)
            {
                this.Text = "新增個案";
            }
            else
            {
                this.txtEnglishName.Text = this.Case.EnglishName;
                this.txtName.Text = this.Case.Name;
                this.txtCaseNo.Text = this.Case.No;
                this.txtAuthor.Text = this.Case.Author;
                this.txtSchool.Text = this.Case.PublishSchool;
                this.txtMemo.Text = this.Case.Memo.Replace("\r\n", "\n").Replace("\n", "\r\n");
                this.Text = "修改個案";
                this.DGV_DataBinding();
            }  
        }

        private void DGV_DataBinding()
        {
            if (string.IsNullOrEmpty(this.Case.UrlList))
                return;

            XDocument xDocument = XDocument.Parse(this.Case.UrlList.Replace("&", "&#038;"));
            IEnumerable<XElement> xElements = xDocument.Descendants("CaseDocument");
            foreach (XElement xElement in xElements)
            {
                string Url = xElement.Attribute("URL").Value;
                string Memo = xElement.Attribute("Memo").Value;
                string FileName = xElement.Attribute("FileName").Value;
                bool finded = false;
                string Url_Decode = null;
                if (!string.IsNullOrWhiteSpace(Url))
                {
                    byte[] utf8Bytes = Convert.FromBase64String(Url);
                    Url_Decode = Encoding.UTF8.GetString(utf8Bytes);
                    foreach (DataItems.CloudFileUrlItem o in this.GoogleDocsRowSource)
                    {
                        if (o.Name == FileName && o.Url == Url_Decode)
                        {
                            finded = true;
                            break;
                        }
                    }
                    if (!finded)
                        this.GoogleDocsRowSource.Add(new DataItems.CloudFileUrlItem(FileName, Url_Decode));
                }
                List<object> source = new List<object>();

                source.Add(null);
                source.Add(Url_Decode);
                source.Add(Memo);

                int idx = this.dgvData.Rows.Add(source.ToArray());
                this.dgvData.Rows[idx].Cells[0].Value = Url_Decode;
            }
            this.dgvData.CurrentCell = null;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0 && dgvData.SelectedCells.Count == 1)
            {
                dgvData.BeginEdit(true);  //Raise EditingControlShowing Event !
                //if (dgvData.CurrentCell != null && dgvData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell")
                    //(dgvData.EditingControl as ComboBox).DroppedDown = true;  //自動拉下清單
            }
        }

        private void dgvData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dgvData.CurrentCell.ColumnIndex == 0)
            {
                if (e.Control is DataGridViewComboBoxEditingControl)
                {
                    ComboBox comboBox = e.Control as ComboBox;

                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                    comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                    comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
                }                
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            (sender as ComboBox).SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);

            if ((sender as ComboBox).SelectedItem == null)
                return;

            dgvData.CurrentRow.Cells[1].Value = ((sender as ComboBox).SelectedItem as DataItems.CloudFileUrlItem).Url;
        }

        private void dgvData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvData.CurrentCell = null;
                dgvData.SelectAll();
            }
        }

        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private new bool Validated()
        {
            this.ErrorProvider.Clear();
            bool validated = true;

            List<UDT.Case> Cases = Access.Select<UDT.Case>();
            if (string.IsNullOrWhiteSpace(this.txtName.Text) && string.IsNullOrWhiteSpace(this.txtEnglishName.Text))
            {
                this.ErrorProvider.SetError(this.txtName, "英文名稱與中文名稱不可皆為空白。");
                this.ErrorProvider.SetError(this.txtEnglishName, "英文名稱與中文名稱不可皆為空白。");
                validated = false;
            }
            if (!string.IsNullOrEmpty(this.txtEnglishName.Text))
            {
                if (this.Case == null)
                {
                    if (Cases.Where(x => (x.EnglishName.ToLower() == this.txtEnglishName.Text.Trim().ToLower())).Count() > 0)
                    {
                        this.ErrorProvider.SetError(this.txtEnglishName, "相同英文名稱之個案已存在。");
                        validated = false;
                    }

                }
                else
                {
                    if (Cases.Where(x => (x.EnglishName.Trim().ToLower() == this.txtEnglishName.Text.Trim().ToLower() && x.UID != this.Case.UID)).Count() > 0)
                    {
                        this.ErrorProvider.SetError(this.txtEnglishName, "相同英文名稱之個案已存在。");
                        validated = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(this.txtName.Text))
            {
                if (this.Case == null)
                {
                    if (Cases.Where(x => (x.Name.ToLower() == this.txtName.Text.Trim().ToLower())).Count() > 0)
                    {
                        this.ErrorProvider.SetError(this.txtName, "相同中文名稱之個案已存在。");
                        validated = false;
                    }
                }
                else
                {
                    if (Cases.Where(x => (x.Name.Trim().ToLower() == this.txtName.Text.Trim().ToLower() && x.UID != this.Case.UID)).Count() > 0)
                    {
                        this.ErrorProvider.SetError(this.txtName, "相同中文名稱之個案已存在。");
                        validated = false;
                    }
                }
            }
            List<string> FileNames = new List<string>();
            FileNames = this.dgvData.Rows.Cast<DataGridViewRow>().Where(x => !x.IsNewRow).Select(x => x.Cells[0].Value + "").ToList();
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (string.IsNullOrEmpty(row.Cells[0].Value + ""))
                {
                    this.ErrorProvider.SetError(this.dgvData, "文件名稱必填。");
                    validated = false;
                    break;
                }
                if (FileNames.Where(x => x == row.Cells[0].Value + "").Count() > 1)
                {
                    this.ErrorProvider.SetError(this.dgvData, "文件名稱重覆。");
                    validated = false;
                    break;
                }
            }

            return validated;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            if (!this.Validated())
            {
                MessageBox.Show("請修正錯誤再儲存。");
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }

            if (this.Case == null)
                this.Case = new UDT.Case();

            this.Case.EnglishName = this.txtEnglishName.Text.Trim();
            this.Case.Name = this.txtName.Text.Trim();
            this.Case.No = this.txtCaseNo.Text.Trim();
            this.Case.Author = this.txtAuthor.Text.Trim();
            this.Case.PublishSchool = this.txtSchool.Text.Trim();
            this.Case.Memo = this.txtMemo.Text.Trim();

            StringBuilder sb = new StringBuilder("<CaseDocuments>");
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string Url = row.Cells[1].Value + "";
                byte[] array = Encoding.UTF8.GetBytes(Url);
                string Url_Encode = Convert.ToBase64String(array);
                sb.Append(string.Format(@"<CaseDocument FileName=""{0}"" URL=""{1}"" Memo=""{2}"" />", row.Cells[0].FormattedValue + "", Url_Encode, row.Cells[2].Value + ""));
            }
            sb.Append("</CaseDocuments>");

            this.Case.UrlList = sb.ToString();
            
            this.Case.Save();
            this.Close();
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 1)
                return;

            if (string.IsNullOrEmpty(this.dgvData.Rows[e.RowIndex].Cells[1].Value + ""))
                return;

            try
            {
                System.Diagnostics.Process.Start(this.dgvData.Rows[e.RowIndex].Cells[1].Value + "");
            }
            catch {}
        }

        private void dgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;

            //if (e.ColumnIndex != 0)
            //    return;

            //MessageBox.Show(this.dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "");
        }

        private void dgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;

            //if (e.ColumnIndex != 0)
            //    return;

            //MessageBox.Show(this.dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "");
        }

        private void dgvData_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    this.dgvData.EndEdit();
            //    MessageBox.Show(this.dgvData.CurrentCell.Value + "");
            //}
        }
    }
}
