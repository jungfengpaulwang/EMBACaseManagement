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
using DevComponents.Editors;
using FISCA.Data;
using System.Threading.Tasks;

namespace CaseManagement.Forms
{
    public partial class frimQueryCase : BaseForm
    {
        private AccessHelper Access;
        private QueryHelper Query;

        private bool form_loaded;

        public frimQueryCase()
        {
            InitializeComponent();
            this.Load += new EventHandler(frmAttendNoneCourse_Load);
        }

        private void frmAttendNoneCourse_Load(object sender, EventArgs e)
        {
            form_loaded = false;

            Access = new AccessHelper();
            Query = new QueryHelper();

            this.InitSchoolYear();
            this.InitSemester();

            form_loaded = true;
            this.DGV_DataBinding();
        }

        private void InitSchoolYear()
        {
            int DefaultSchoolYear;
            //if (int.TryParse(K12.Data.School.DefaultSchoolYear, out DefaultSchoolYear))
            //{
            //    this.nudSchoolYear.Value = decimal.Parse(DefaultSchoolYear.ToString());
            //}
            //else
            //{
            //    this.nudSchoolYear.Value = decimal.Parse((DateTime.Today.Year - 1911).ToString());
            //}
        }

        private void InitSemester()
        {
            //this.cboSemester.DataSource = CourseSelection.DataItems.SemesterItem.GetSemesterList();
            //this.cboSemester.ValueMember = "Value";
            //this.cboSemester.DisplayMember = "Name";

            //this.cboSemester.SelectedValue = K12.Data.School.DefaultSemester;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nudSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            this.DGV_DataBinding();
        }

        private void cboSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.DGV_DataBinding();
        }

        private void DGV_DataBinding()
        {
            if (!this.form_loaded)
                return;

            this.dgvData.Rows.Clear();
            //this.dgvDataNonConfirm.Rows.Clear();

            //int school_year = int.Parse(this.nudSchoolYear.Value + "");
            //int semester = int.Parse((this.cboSemester.SelectedItem as CourseSelection.DataItems.SemesterItem).Value);

            DataTable dataTable_Confirmed = new DataTable();
            DataTable dataTable_NonConfirmed = new DataTable();
            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            Task task = Task.Factory.StartNew(() =>
            {
//                #region  已確認學生
//                dataTable_Confirmed = Query.Select(string.Format(@"select student.id as student_id, sb.grade_year, sb.enroll_year, dg.name as dept_name, dg.uid as dept_id, class.id as class_id, class_name, student_number, 
//    student.name as student_name from student
//    join $ischool.emba.registration_confirm as ca on ca.ref_student_id=student.id
//    left join $ischool.emba.student_brief2 as sb on sb.ref_student_id=student.id
//    left join $ischool.emba.department_group as dg on dg.uid=sb.ref_department_group_id
//    left join class on class.id=student.ref_class_id
//	where ca.confirm=true and ca.school_year={0} and ca.semester={1} order by sb.grade_year, sb.enroll_year, dg.name, class_name, student_number", school_year, semester));
//                #endregion

//                #region 未確認學生
//                dataTable_NonConfirmed = Query.Select(string.Format(@"select student.id as student_id, sb.grade_year, sb.enroll_year, dg.name as dept_name, dg.uid as dept_id, class.id as class_id, class_name, student_number,  student.name as student_name from student left join $ischool.emba.student_brief2 as sb on sb.ref_student_id=student.id left join $ischool.emba.department_group as dg on dg.uid=sb.ref_department_group_id left join class on class.id=student.ref_class_id where student.status in (1, 4) and student.id not in (select ref_student_id from $ischool.emba.registration_confirm as rc	where rc.school_year={0} and rc.semester={1} and rc.confirm=true)", school_year, semester));
//                #endregion
            });

            task.ContinueWith((x) =>
            {
                foreach (DataRow row in dataTable_Confirmed.Rows)
                {
                    List<object> source = new List<object>();

                    source.Add(row["grade_year"] + "");
                    source.Add(row["enroll_year"] + "");
                    source.Add(row["dept_name"] + "");
                    source.Add(row["class_name"] + "");
                    source.Add(row["student_number"] + "");
                    source.Add(row["student_name"] + "");

                    int idx = this.dgvData.Rows.Add(source.ToArray());
                    this.dgvData.Rows[idx].Tag = row["student_id"] + "";
                }

                foreach (DataRow row in dataTable_NonConfirmed.Rows)
                {
                    List<object> source = new List<object>();

                    source.Add(row["grade_year"] + "");
                    source.Add(row["enroll_year"] + "");
                    source.Add(row["dept_name"] + "");
                    source.Add(row["class_name"] + "");
                    source.Add(row["student_number"] + "");
                    source.Add(row["student_name"] + "");

                    //int idx = this.dgvDataNonConfirm.Rows.Add(source.ToArray());
                    //this.dgvDataNonConfirm.Rows[idx].Tag = row["student_id"] + "";
                }
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1)
                return;

            if (this.dgvData.Columns[e.ColumnIndex].Name == "Cancel")
            {
                if (e.RowIndex == -1)
                    return;

                string ref_student_id = this.dgvData.Rows[e.RowIndex].Tag + "";
                //int school_year = int.Parse(this.nudSchoolYear.Value + "");
                //int semester = int.Parse((this.cboSemester.SelectedItem as CourseSelection.DataItems.SemesterItem).Value);

                //List<UDT.RegistrationConfirm> RC = Access.Select<UDT.RegistrationConfirm>(string.Format("ref_student_id={0} and school_year={1} and semester={2}", ref_student_id, school_year, semester));
                //RC.ForEach((x) => x.Confirm = false);
                //RC.SaveAll();

                this.DGV_DataBinding();
            }
        }
    }
}
