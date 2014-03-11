using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using FISCA.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using K12.Data;
using System.Xml.Linq;
using System.Text;

namespace CaseManagement.Forms
{
    public partial class frmQueryTeacherCase : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private List<UDT.CaseUsage> CasesUsages;
        private List<dynamic> Subjects;
        private List<dynamic> Teachers;
        private List<dynamic> CaseUsageSource;
        private List<UDT.Case> Cases;
        private List<UDT.Case> Cases_EnglishName;

        private bool form_loaded;

        public frmQueryTeacherCase()
        {
            InitializeComponent();

            this.Subjects = new List<dynamic>();
            this.Teachers = new List<dynamic>();
            this.Cases = new List<UDT.Case>();
            this.Cases_EnglishName = new List<UDT.Case>();            
            this.CaseUsageSource = new List<dynamic>();

            this.Load += new EventHandler(frmQueryTeacherCase_Load);

            Access = new AccessHelper();
            Query = new QueryHelper();
        }

        private void frmQueryTeacherCase_Load(object sender, EventArgs e)
        {
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;

            this.form_loaded = false;

            Task task = Task.Factory.StartNew(() =>
            {
                this.CasesUsages = (new List<UDT.CaseUsage>(){ new UDT.CaseUsage() }).Union(Access.Select<UDT.CaseUsage>()).ToList();
                this.Cases = (new List<UDT.Case>() { new UDT.Case() }).Union(Access.Select<UDT.Case>().Where(x=>!string.IsNullOrEmpty(x.Name))).ToList();
                this.Cases_EnglishName = (new List<UDT.Case>() { new UDT.Case() }).Union(Access.Select<UDT.Case>().Where(x=>!string.IsNullOrEmpty(x.EnglishName))).ToList();

                Dictionary<string, string> dicCourse = new Dictionary<string, string>();
                Dictionary<string, UDT.Case> dicCases = new Dictionary<string, UDT.Case>();
                Dictionary<string, CourseRecord> dicCourses = new Dictionary<string, CourseRecord>();
                Dictionary<string, string> dicTeachers = new Dictionary<string, string>();

                this.Cases.ForEach((x) =>
                {
                    if (!string.IsNullOrEmpty(x.UID) && !dicCases.ContainsKey(x.UID))
                        dicCases.Add(x.UID, x);
                });

                this.Cases_EnglishName.ForEach((x) =>
                {
                    if (!string.IsNullOrEmpty(x.UID) && !dicCases.ContainsKey(x.UID))
                        dicCases.Add(x.UID, x);
                });

                List<CourseRecord> Courses = Course.SelectAll();
                if (Courses.Count > 0)
                    dicCourses = Courses.ToDictionary(x => x.ID);

                string SQL = string.Format(@"SELECT  distinct teacher.id as teacher_id, teacher.teacher_name FROM course LEFT JOIN $ischool.emba.course_instructor ON $ischool.emba.course_instructor.ref_course_id = course.id LEFT JOIN teacher ON teacher.id = $ischool.emba.course_instructor.ref_teacher_id LEFT JOIN tag_teacher ON tag_teacher.ref_teacher_id = teacher.id LEFT JOIN tag ON tag.id = tag_teacher.ref_tag_id WHERE tag.category = 'Teacher' AND tag.prefix = '教師' Order by teacher_name");

                DataTable dataTable = Query.Select(SQL);
                this.Teachers.Add(new { ID = "", Name = "" });
                foreach (DataRow row in dataTable.Rows)
                {
                    this.Teachers.Add(new { ID = row["teacher_id"] + "", Name = row["teacher_name"] + "" });
                    if (!dicTeachers.ContainsKey(row["teacher_id"] + ""))
                        dicTeachers.Add(row["teacher_id"] + "", row["teacher_name"] + "");
                }

                SQL = string.Format(@"select subject.uid as subject_id, course.course_name, course.id as course_id from course join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id join $ischool.emba.subject as subject on subject.uid=ce.ref_subject_id");

                dataTable = Query.Select(SQL);
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!dicCourse.ContainsKey(row["course_id"] + ""))
                        dicCourse.Add(row["course_id"] + "", row["subject_id"] + "");
                }

                SQL = string.Format(@"select distinct subject.uid as subject_id, subject.name as subject_name from course join $ischool.emba.course_ext as ce on ce.ref_course_id=course.id join $ischool.emba.subject as subject on subject.uid=ce.ref_subject_id");
                
                dataTable = Query.Select(SQL);
                this.Subjects.Add(new { UID = "", Name = "" });
                foreach (DataRow row in dataTable.Rows)
                    this.Subjects.Add(new { UID = row["subject_id"] + "", Name = row["subject_name"] + "" });

                foreach (UDT.CaseUsage CaseUsage in this.CasesUsages)
                {
                    dynamic o = new ExpandoObject();

                    if (!dicCourses.ContainsKey(CaseUsage.CourseID.ToString()))
                        continue;

                    if (!dicTeachers.ContainsKey(CaseUsage.TeacherID.ToString()))
                        continue;

                    if (!dicCases.ContainsKey(CaseUsage.CaseID.ToString()))
                        continue;

                    if (!dicCourse.ContainsKey(CaseUsage.CourseID.ToString()))
                        continue;

                    o.SchoolYear = dicCourses[CaseUsage.CourseID.ToString()].SchoolYear;
                    o.Semester = dicCourses[CaseUsage.CourseID.ToString()].Semester;
                    o.CourseName = dicCourses[CaseUsage.CourseID.ToString()].Name;
                    o.CourseID = CaseUsage.CourseID.ToString();
                    o.SubjectID = dicCourse[CaseUsage.CourseID.ToString()];
                    o.TeacherName = dicTeachers[CaseUsage.TeacherID.ToString()];
                    o.TeacherID = CaseUsage.TeacherID.ToString();
                    o.CaseEnglishName = dicCases[CaseUsage.CaseID.ToString()].EnglishName;
                    o.CaseName = dicCases[CaseUsage.CaseID.ToString()].Name;
                    o.CaseID = CaseUsage.CaseID.ToString();
                    o.UrlList = dicCases[CaseUsage.CaseID.ToString()].UrlList;

                    this.CaseUsageSource.Add(o);
                }
            });
            task.ContinueWith((x) =>
            {
                this.circularProgress.Visible = false;
                this.circularProgress.IsRunning = false;
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                    return;
                }
                this.BindDataRowSource();
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            this.InitSchoolYear();
            this.InitSemester();

            this.form_loaded = true;
        }

        private void InitSchoolYear()
        {
            int DefaultSchoolYear;
            if (int.TryParse(K12.Data.School.DefaultSchoolYear, out DefaultSchoolYear))
            {
                this.nudSchoolYear.Value = decimal.Parse(DefaultSchoolYear.ToString());
                this.nudSchoolYear1.Value = decimal.Parse(DefaultSchoolYear.ToString());
            }
            else
            {
                this.nudSchoolYear.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
                this.nudSchoolYear1.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
            }
        }

        private void InitSemester()
        {
            this.cboSemester.DataSource = DataItems.SemesterItem.GetSemesterList();
            this.cboSemester.ValueMember = "Value";
            this.cboSemester.DisplayMember = "Name";

            this.cboSemester1.DataSource = DataItems.SemesterItem.GetSemesterList();
            this.cboSemester1.ValueMember = "Value";
            this.cboSemester1.DisplayMember = "Name";

            this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
            this.cboSemester1.SelectedValue = K12.Data.School.DefaultSemester;
        }

        private void BindDataRowSource()
        {
            this.cboSubject.DataSource = this.Subjects;
            this.cboSubject.ValueMember = "UID";
            this.cboSubject.DisplayMember = "Name";
            this.cboSubject.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboSubject.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.cboTeacher.DataSource = this.Teachers;
            this.cboTeacher.ValueMember = "ID";
            this.cboTeacher.DisplayMember = "Name";
            this.cboTeacher.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboTeacher.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.cboCaseName.DataSource = this.Cases;
            this.cboCaseName.ValueMember = "UID";
            this.cboCaseName.DisplayMember = "Name";
            this.cboCaseName.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboCaseName.AutoCompleteSource = AutoCompleteSource.ListItems;

            this.cboCaseEnglishName.DataSource = this.Cases_EnglishName;
            this.cboCaseEnglishName.ValueMember = "UID";
            this.cboCaseEnglishName.DisplayMember = "EnglishName";
            this.cboCaseEnglishName.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.cboCaseEnglishName.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void btnQuery_Click(object sender, EventArgs e)
        {            
            this.dgvData.Rows.Clear();

            List<string> DGV_Keys = new List<string>();

            int school_year = int.Parse(this.nudSchoolYear.Value + "");
            int school_year1 = int.Parse(this.nudSchoolYear1.Value + "");
            int semester = int.Parse((this.cboSemester.SelectedItem as DataItems.SemesterItem).Value);
            int semester1 = int.Parse((this.cboSemester1.SelectedItem as DataItems.SemesterItem).Value);
            string subject_id = this.cboSubject.SelectedValue + "";
            string teacher_id = this.cboTeacher.SelectedValue + "";
            string case_engname_id = this.cboCaseEnglishName.SelectedValue + "";
            string case_name_id = this.cboCaseName.SelectedValue + "";

            List<dynamic> DataGridViewSource = new List<dynamic>();
            foreach (dynamic o in this.CaseUsageSource)
            {
                bool wanted = false;
                int oSchoolYear = int.Parse(o.SchoolYear + "");
                int oSemester = int.Parse(o.Semester + "");
                string oSubjectID = o.SubjectID + "";
                string oTeacherID = o.TeacherID + "";
                string oCaseEnglishName_ID = o.CaseID + "";
                string oCaseName_ID = o.CaseID + "";

                if (oSchoolYear < school_year || oSchoolYear > school_year1)
                    continue;
                if (oSchoolYear == school_year && oSemester < semester)
                    continue;
                if (oSchoolYear == school_year1 && oSemester > semester1)
                    continue;

                if (oSubjectID == subject_id)
                    wanted = true;
                if (oTeacherID == teacher_id)
                    wanted = true;
                if (oCaseEnglishName_ID == case_engname_id)
                    wanted = true;
                if (oCaseName_ID == case_name_id)
                    wanted = true;

                if (string.IsNullOrEmpty(this.cboSubject.Text) && string.IsNullOrEmpty(this.cboTeacher.Text) && string.IsNullOrEmpty(this.cboCaseEnglishName.Text) && string.IsNullOrEmpty(this.cboCaseName.Text))
                    wanted = true;

                if (wanted)
                    DataGridViewSource.Add(o);
            }            
            //  <CaseDocument FileName='{0}' URL='{1}' Memo='{2}' />
            if (DataGridViewSource.Count == 0)
            {
                MsgBox.Show("查無資料。");
                return;
            }
            int max_column_count = DataGridViewSource.Where(x => !string.IsNullOrWhiteSpace(x.UrlList)).Select(x => ((XDocument.Parse(x.UrlList.Replace("&", "&#038;")).Descendants("CaseDocument")) as IEnumerable<XElement>).Count()).Max();

            this.SyncColumns(max_column_count);

            foreach (dynamic o in DataGridViewSource)
            {
                List<object> source = new List<object>();
                List<string> urls = new List<string>();

                int oSchoolYear = int.Parse(o.SchoolYear + "");
                int oSemester = int.Parse(o.Semester + "");
                string oCourseName = o.CourseName + "";
                string oTeacherName = o.TeacherName + "";
                string oCaseEnglishName = o.CaseEnglishName + "";
                string oCaseName = o.CaseName + "";

                source.Add(oSchoolYear);
                source.Add(oSemester);
                source.Add(oCourseName);
                source.Add(oTeacherName);
                source.Add(oCaseEnglishName);
                source.Add(oCaseName);

                if (string.IsNullOrEmpty(o.UrlList))
                {
                    for (int i = 0; i < max_column_count; i++)
                    {
                        source.Add(string.Empty);
                        urls.Add(string.Empty);
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Parse(o.UrlList.Replace("&", "&#038;"));
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
                    this.dgvData.Rows[idx].Cells[i + 6].Tag = urls.ElementAt(i);
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            this.dgvData.Columns.Cast<DataGridViewColumn>().ToList().ForEach(x => dataTable.Columns.Add(x.HeaderText));
            this.dgvData.Rows.Cast<DataGridViewRow>().ToList().ForEach(x =>
            {
                DataRow row = dataTable.NewRow();
                foreach (DataGridViewColumn Column in this.dgvData.Columns)
                    row[Column.HeaderText] = x.Cells[Column.Name].Value + "";

                dataTable.Rows.Add(row);
            });

            Workbook wb = new Workbook();
            foreach (Worksheet sheet in wb.Worksheets.Cast<Worksheet>().ToList())
                wb.Worksheets.RemoveAt(sheet.Name);

            int sheet_index = wb.Worksheets.Add();
            wb.Worksheets[sheet_index].Cells.ImportDataTable(dataTable, true, "A1");

            wb.Worksheets.Cast<Worksheet>().ToList().ForEach(y => y.AutoFitColumns());
            SaveFileDialog sd = new SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = "教師採用個案資料.xls";
            sd.Filter = "Excel 2003 相容檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Save(sd.FileName, FileFormatType.Excel2003);
                    System.Diagnostics.Process.Start(sd.FileName);
                }
                catch
                {
                    MessageBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
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
    }
}