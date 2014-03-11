using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Campus.Windows;
using FISCA.Permission;
using K12.Data;
using FISCA.UDT;
using CaseManagement.DataItems;
using FISCA.Data;
using System.Xml.Linq;

namespace CaseManagement.DetailItems
{
    [AccessControl("ischool.EMBA.Course.Case_Usage", "使用個案", "課程>資料項目")]
    public partial class Course_Case_Usage : DetailContentImproved
    {
        private AccessHelper Access;
        private QueryHelper Query;
        private List<dynamic> TeacherRowSource = new List<dynamic>();
        private List<dynamic> CaseEnglishNameRowSource = new List<dynamic>();
        private List<UDT.CaseUsage> Records = new List<UDT.CaseUsage>();
        private ErrorProvider error_privider = new ErrorProvider();

        public Course_Case_Usage()
        {
            InitializeComponent();
            this.Group = "使用個案";

            this.Load += new System.EventHandler(this.Course_Case_Usage_Load);
           

            Access = new AccessHelper();
            Query = new QueryHelper();
        }

        private void dgvData_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvData.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Rows[e.RowIndex].Selected = true;
        }

        private void dgvData_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvData.CurrentCell = null;
            dgvData.Columns[e.ColumnIndex].Selected = true;
        }

        private void dgvData_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridView.HitTestInfo hit = dgv.HitTest(e.X, e.Y);

            if (hit.Type == DataGridViewHitTestType.TopLeftHeader)
            {
                dgvData.CurrentCell = null;
                dgvData.SelectAll();
                return;
            }
        }

        private void dgvData_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvData.SelectedCells.Count == 1)
            {
                dgvData.BeginEdit(true);
                if (dgvData.CurrentCell != null && dgvData.CurrentCell.GetType().ToString() == "System.Windows.Forms.DataGridViewComboBoxCell" && e.ColumnIndex == 1)
                    (dgvData.EditingControl as ComboBox).DroppedDown = true;  //自動拉下清單
            }
        }

        private void dgvData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is DataGridViewComboBoxEditingControl)
            {
                ComboBox comboBox = e.Control as ComboBox;

                if (this.dgvData.CurrentCell.ColumnIndex == 0)
                {
                    comboBox.DropDownStyle = ComboBoxStyle.DropDown;
                    comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                    comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
                if (this.dgvData.CurrentCell.ColumnIndex == 1)
                    comboBox.DropDownClosed += new EventHandler(comboBox_DropDownClosed);
            }
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            (sender as ComboBox).DropDownClosed -= new EventHandler(comboBox_DropDownClosed);
            this.dgvData.CurrentCell.Selected = false;
        }

        private void Course_Case_Usage_Load(object sender, EventArgs e)
        {
            this.dgvData.CellEnter += new DataGridViewCellEventHandler(dgvData_CellEnter);
            this.dgvData.CurrentCellDirtyStateChanged += new EventHandler(dgvData_CurrentCellDirtyStateChanged);
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_ColumnHeaderMouseClick);
            this.dgvData.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_RowHeaderMouseClick);
            this.dgvData.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dgvData_MouseClick);
            this.dgvData.DataError += new DataGridViewDataErrorEventHandler(dgvData_DataError);
            this.dgvData.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgvData_EditingControlShowing);
            
            //this.dgvData.UserDeletingRow += new DataGridViewRowCancelEventHandler(dgvData_UserDeletingRow);
            
            this.WatchChange(new DataGridViewSource(this.dgvData));
        }

        private void dgvData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        //public void SubjectSemesterScore_AfterUpdate(object sender, UDT.Case e)
        //{
        //    LoadSubjectSemesterScores(null);
        //    DataGridView_DataBinding();
        //}

        protected override void OnInitializeAsync()
        {  
            LoadCaseEnglishNameRowSource();
        }

        private void LoadTeacherRowSource()
        {
            TeacherRowSource.Clear();
            TeacherRowSource.Add(new { Name = "", ID = "" });

            DataTable dataTable = Query.Select(string.Format(@"SELECT teacher.id, teacher.teacher_name, teacher.status FROM course
            LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id
            LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id
            LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id
            LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id
            WHERE tag.category = 'Teacher' AND tag.prefix = '教師' AND course.id={0}
			Group by teacher.id, teacher.teacher_name, teacher.status", PrimaryKey));

            foreach (DataRow row in dataTable.Rows)
                TeacherRowSource.Add(new { Name = row["teacher_name"] + "", ID = row["id"] + "" });
            
            this.Teacher.DataSource = this.TeacherRowSource;
            this.Teacher.ValueMember = "ID";
            this.Teacher.DisplayMember = "Name";
        }

        private void LoadCaseEnglishNameRowSource()
        {
            CaseEnglishNameRowSource.Clear();
            CaseEnglishNameRowSource.Add(new { Name = "", ID = "" });

            DataTable dataTable = Query.Select(string.Format(@"select english_name, name, uid from $ischool.emba.case_management.case"));

            foreach (DataRow row in dataTable.Rows)
            {
                string case_name = string.IsNullOrEmpty(row["english_name"] + "") ? row["name"] + "" : row["english_name"] + "";
                CaseEnglishNameRowSource.Add(new { Name = case_name, ID = row["uid"] + "" });
            }
        }

        protected override void OnInitializeComplete(Exception error)
        {            
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            this.CaseEnglishName.DataSource = this.CaseEnglishNameRowSource;
            this.CaseEnglishName.ValueMember = "ID";
            this.CaseEnglishName.DisplayMember = "Name";
        }

        private void LoadCaseUsage()
        {
            this.Records = Access.Select<UDT.CaseUsage>(string.Format("ref_course_id = {0}", PrimaryKey));
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            LoadCaseUsage();
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            LoadTeacherRowSource();
            DataGridView_DataBinding();
        }

        private void DataGridView_DataBinding()
        {
            ErrorTip.Clear();

            this.dgvData.Rows.Clear();

            foreach (UDT.CaseUsage CaseUsage in this.Records)
            {
                List<object> sources = new List<object>();

                sources.Add(CaseUsage.CaseID.ToString());
                sources.Add(CaseUsage.TeacherID.ToString());

                int idx = this.dgvData.Rows.Add(sources.ToArray());
                this.dgvData.Rows[idx].Tag = CaseUsage;
            }
            this.dgvData.CurrentCell = null;
            ResetDirtyStatus();
        }

        protected override void OnDirtyStatusChanged(ChangeEventArgs e)
        {
            if (UserAcl.Current[this.GetType()].Editable)
                SaveButtonVisible = e.Status == ValueStatus.Dirty;
            else
                this.SaveButtonVisible = false;

            CancelButtonVisible = e.Status == ValueStatus.Dirty;
        }

        private bool ValidateData()
        {
            bool is_valid = true;
            //確定使用者修改的值都更新到控制項裡了(預防點選checkbox 後直接點選儲存，這時抓到的值仍是前一個值)。
            this.dgvData.EndEdit();
            this.dgvData.CurrentCell = null;
            IEnumerable<DataGridViewRow> Rows = this.dgvData.Rows.Cast<DataGridViewRow>();
            Dictionary<string, UDT.CaseUsage> dicCaseUsages = new Dictionary<string, UDT.CaseUsage>();
            this.error_privider.Clear();
            if (this.Records.Count > 0)
                dicCaseUsages = this.Records.ToDictionary(x => x.UID);
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;
                
                //1. 「個案名稱」為必填
                if (string.IsNullOrEmpty(row.Cells[0].Value + ""))
                {
                    row.Cells[0].ErrorText = "必填。";
                    is_valid = false;
                }
                else
                    row.Cells[0].ErrorText = string.Empty;

                //2. 「教師」為必填
                if (string.IsNullOrEmpty(row.Cells[1].Value + ""))
                {
                    row.Cells[1].ErrorText = "必填。";
                    is_valid = false;
                }
                else
                    row.Cells[1].ErrorText = string.Empty;

                if (!string.IsNullOrEmpty(row.Cells[0].Value + "") && !string.IsNullOrEmpty(row.Cells[1].Value + ""))
                {
                    if (Rows.Where(x => (x.Cells[0].Value + "") == (row.Cells[0].Value + "") && (x.Cells[1].Value + "") == (row.Cells[1].Value + "")).Count() > 1)
                    {
                        row.Cells[0].ErrorText = "個案名稱重覆。";
                        row.Cells[1].ErrorText = "教師重覆。";
                        is_valid = false;
                    }
                    else
                    {
                        row.Cells[0].ErrorText = string.Empty;
                        row.Cells[1].ErrorText = string.Empty;
                    }
                }
                UDT.CaseUsage CaseUsage = row.Tag as UDT.CaseUsage;
                if (CaseUsage != null)
                {
                    if (dicCaseUsages.ContainsKey(CaseUsage.UID))
                        dicCaseUsages.Remove(CaseUsage.UID);
                }
            }
            List<KeyValuePair<string, string>> TeacherCaseReplys = new List<KeyValuePair<string, string>>();
            if (dicCaseUsages.Keys.Count > 0)
            {
                /// <Answers>
                ///     <Question QuestionID="">
                ///         <Answer CaseID="" Score="3">尚可</Answer>
                ///         <Answer CaseID="123" Score="5">很滿意</Answer>
                ///     </Question>
                /// </Answers>
                List<UDT.Reply> Replys = Access.Select<UDT.Reply>(string.Format("ref_course_id = {0}", PrimaryKey));
                foreach (UDT.CaseUsage CaseUsage in dicCaseUsages.Values)
                {
                    foreach (UDT.Reply Reply in Replys)
                    {
                        if (CaseUsage.TeacherID != Reply.TeacherID)
                            continue;

                        XDocument xDocument = XDocument.Parse(Reply.Answer);
                        IEnumerable<XElement> xElements = xDocument.Descendants("Answer");
                        foreach (XElement xElement in xElements)
                        {
                            if (!string.IsNullOrEmpty(xElement.Attribute("CaseID").Value))
                                TeacherCaseReplys.Add(new KeyValuePair<string, string>(xElement.Attribute("CaseID").Value, Reply.TeacherID.ToString()));
                        }
                    }
                }
            }
            if (TeacherCaseReplys.Count > 0)
            {
                foreach(KeyValuePair<string, string> kv in TeacherCaseReplys)
                {
                    foreach (DataGridViewRow row in this.dgvData.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        if ((row.Cells[0].Value + "") == kv.Key && (row.Cells[1].Value + "") == kv.Value)
                        {
                            this.error_privider.SetError(this.dgvData, "教學評鑑包含待刪除記錄，不得刪除。");
                            is_valid = false;
                        }
                        else
                            this.error_privider.SetError(this.dgvData, "");
                    }
                }
            }
            return is_valid;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            base.OnCancelButtonClick(e);
            this.error_privider.Clear();
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].ErrorText = string.Empty;
                row.Cells[1].ErrorText = string.Empty;
            }
        }

        protected override void OnSaveData()
        {
            if (!this.ValidateData())
                return;

            this.Records.ForEach(x => x.Deleted = true);
            this.Records.SaveAll();

            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.IsNewRow)
                    continue;

                UDT.CaseUsage CaseUsage = new UDT.CaseUsage();

                CaseUsage.CourseID = int.Parse(PrimaryKey);
                CaseUsage.CaseID = int.Parse(row.Cells[0].Value + "");
                CaseUsage.TeacherID = int.Parse(row.Cells[1].Value + "");

                this.Records.Add(CaseUsage);
            }
            this.Records.SaveAll();

            this.OnPrimaryKeyChanged(EventArgs.Empty);

            ResetDirtyStatus();
        }

        private void dgvData_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.IsNewRow) return;

            if (MessageBox.Show("確定移除紀錄？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }
    }
}
