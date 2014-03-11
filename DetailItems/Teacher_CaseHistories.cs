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
using System.Dynamic;

namespace CaseManagement.DetailItems
{
    [AccessControl("ischool.EMBA.Teacher_CaseHistories", "採用個案歷程", "教師>資料項目")]
    public partial class Teacher_CaseHistories : DetailContentImproved
    {
        private List<dynamic> TeacherCaseUsages;
        private Dictionary<string, CourseRecord> dicCourses;
        private Dictionary<string, UDT.Case> dicCases;
        private AccessHelper Access;

        public Teacher_CaseHistories()
        {
            InitializeComponent();
            this.Group = "採用個案歷程";

            TeacherCaseUsages = new List<dynamic>();
            dicCourses = new Dictionary<string,CourseRecord>();
            dicCases = new Dictionary<string,UDT.Case>();
            Access = new AccessHelper();

            this.Load += new System.EventHandler(this.Teacher_CaseHistories_Load);
        }

        private void Teacher_CaseHistories_Load(object sender, EventArgs e)
        {
            this.dgvData.BackgroundColor = Color.White;
            
            //Teacher.AfterChange += (x, y) => ReInitialize();
            //UDT.SubjectSemesterScore.AfterUpdate += new EventHandler<UDT.ParameterEventArgs>(SubjectSemesterScore_AfterUpdate);
        }

        //public void SubjectSemesterScore_AfterUpdate(object sender, UDT.Case e)
        //{
        //    LoadSubjectSemesterScores(null);
        //    DataGridView_DataBinding();
        //}

        protected override void OnInitializeAsync()
        {
            List<CourseRecord> Courses = new List<CourseRecord>();
            List<UDT.Case> Cases = new List<UDT.Case>();
            Courses = Course.SelectAll();
            Cases = Access.Select<UDT.Case>();

            if (Courses.Count > 0)
                dicCourses = Courses.ToDictionary(x => x.ID);

            if (Cases.Count > 0)
                this.dicCases = Cases.ToDictionary(x => x.UID);
        }

        protected override void OnInitializeComplete(Exception error)
        {            
            if (error != null) //有錯就直接丟出去吧。
                throw error;
        }

        protected override void OnPrimaryKeyChangedAsync()
        {
            LoadTeacherCaseUsage();
        }

        protected override void OnPrimaryKeyChangedComplete(Exception error)
        {
            if (error != null) //有錯就直接丟出去吧。
                throw error;

            DataGridView_DataBinding();
        }

        private void DataGridView_DataBinding()
        {
            ErrorTip.Clear();

            this.dgvData.Rows.Clear();

            foreach (dynamic o in this.TeacherCaseUsages)
            {
                List<object> source = new List<object>();

                source.Add(o.SchoolYear);
                source.Add(o.Semester);
                source.Add(o.CourseName);
                source.Add(o.CaseEnglishName);
                source.Add(o.CaseName);

                int idx = this.dgvData.Rows.Add(source.ToArray());
            }
        }

        private void LoadTeacherCaseUsage()
        {
            this.TeacherCaseUsages.Clear();
            List<UDT.CaseUsage> CaseUsages = (new AccessHelper()).Select<UDT.CaseUsage>("ref_teacher_id=" + this.PrimaryKey);

            foreach (UDT.CaseUsage CaseUsage in CaseUsages)
            {
                if (!dicCourses.ContainsKey(CaseUsage.CourseID.ToString()))
                    continue;

                if (!dicCases.ContainsKey(CaseUsage.CaseID.ToString()))
                    continue;

                dynamic o = new ExpandoObject();

                o.SchoolYear = dicCourses[CaseUsage.CourseID.ToString()].SchoolYear;
                o.Semester = dicCourses[CaseUsage.CourseID.ToString()].Semester;
                o.CourseName = dicCourses[CaseUsage.CourseID.ToString()].Name;
                o.CaseEnglishName = dicCases[CaseUsage.CaseID.ToString()].EnglishName;
                o.CaseName = dicCases[CaseUsage.CaseID.ToString()].Name;

                this.TeacherCaseUsages.Add(o);                
            }
        }
    }
}
