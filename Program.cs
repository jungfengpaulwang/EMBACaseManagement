using FISCA;
using FISCA.UDT;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Presentation;

namespace CaseManagement
{
    /// <summary>
    /// 自訂資料欄位
    /// </summary>
    public class Program
    {
        [MainMethod("CaseManagement")]
        public static void Main()
        {
            SyncDBSchema();
            InitDetailContent();
        }

        public static void SyncDBSchema()
        {
            #region 模組啟用先同步Schema

            SchemaManager Manager = new SchemaManager(FISCA.Authentication.DSAServices.DefaultConnection);

            Manager.SyncSchema(new UDT.Case());
            Manager.SyncSchema(new UDT.CaseUsage());
            Manager.SyncSchema(new UDT.oAuthAccount());
            Manager.SyncSchema(new UDT.Reply());

            #endregion
        }

        public static void InitDetailContent()
        {
            #region 資料項目

            /*  註冊權限  */

            Catalog detail = RoleAclSource.Instance["課程"]["資料項目"];

            detail.Add(new DetailItemFeature(typeof(DetailItems.Course_Case_Usage)));

            if (UserAcl.Current[typeof(DetailItems.Course_Case_Usage)].Viewable)
                NLDPanels.Course.AddDetailBulider<DetailItems.Course_Case_Usage>();
            
            detail.Add(new DetailItemFeature(typeof(DetailItems.Teacher_CaseHistories)));

            if (UserAcl.Current[typeof(DetailItems.Teacher_CaseHistories)].Viewable)
                NLDPanels.Teacher.AddDetailBulider<DetailItems.Teacher_CaseHistories>();

            #endregion

            #region 課程>個案>管理>個案管理

            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("Button_CaseManagement_CaseDetailManagement", "個案資料管理"));

            MotherForm.RibbonBarItems["課程", "個案"]["管理"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["課程", "個案"]["管理"].Image = Properties.Resources.network_lock_64;
            MotherForm.RibbonBarItems["課程", "個案"]["管理"]["個案資料"].Enable = UserAcl.Current["Button_CaseManagement_CaseDetailManagement"].Executable;
            MotherForm.RibbonBarItems["課程", "個案"]["管理"]["個案資料"].Click += delegate
            {
                (new Forms.frmCase()).ShowDialog();
            };
            #endregion

            //#region 課程>個案>匯出>個案資料
            //RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("Button_Case_Export", "匯出個案資料"));

            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"].Image = Properties.Resources.Export_Image;
            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"]["個案資料"].Enable = true;// UserAcl.Current["Button_Case_Export"].Executable;
            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"]["個案資料"].Click += delegate
            //{
            //    (new Export.CSAttend()).ShowDialog();
            //};
            //#endregion

            //#region 課程>個案>GoogleDrive
            //RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("Button_Case_Export", "匯出個案資料"));

            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"].Image = Properties.Resources.Export_Image;
            //MotherForm.RibbonBarItems["課程", "個案"]["匯出"]["個案資料"].Enable = true;// UserAcl.Current["Button_Case_Export"].Executable;
            //MotherForm.RibbonBarItems["課程", "個案"]["GoogleDrive"].Click += delegate
            //{
            //    (new Forms.frmGoogleDriveListFilesInAssignedFolder()).ShowDialog();
            //};
            //#endregion

            #region 課程>個案>設定>oAuth 帳號
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("Button_CaseManagement_oAuthAccount_Set", "設定 oAuth 帳號"));

            var templateManager = MotherForm.RibbonBarItems["課程", "個案"]["設定"];
            templateManager.Size = RibbonBarButton.MenuButtonSize.Large;
            templateManager.Image = Properties.Resources.sandglass_unlock_64;
            templateManager["oAuth 帳號"].Enable = UserAcl.Current["Button_CaseManagement_oAuthAccount_Set"].Executable;
            templateManager["oAuth 帳號"].Click += delegate
            {
                (new Forms.frmOAuth_Management()).ShowDialog();
            };
            #endregion

            #region 課程>個案>查詢>複合查詢教師採用個案資料
            RoleAclSource.Instance["課程"]["功能按鈕"].Add(new RibbonFeature("Button_CaseManagement_TeacherCase_Query", "複合查詢教師採用個案資料"));

            MotherForm.RibbonBarItems["課程", "個案"]["查詢"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["課程", "個案"]["查詢"].Image = Properties.Resources.searchHistory;
            MotherForm.RibbonBarItems["課程", "個案"]["查詢"]["教師採用個案資料"].Enable = UserAcl.Current["Button_CaseManagement_TeacherCase_Query"].Executable;
            MotherForm.RibbonBarItems["課程", "個案"]["查詢"]["教師採用個案資料"].Click += delegate
            {
                (new Forms.frmQueryTeacherCase()).ShowDialog();
            };
            #endregion


            #region 匯入
            MotherForm.RibbonBarItems["課程", "個案"]["匯入"].Image = Properties.Resources.Import_Image;
            MotherForm.RibbonBarItems["課程", "個案"]["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;

                #region  個案基本資料
            Catalog button_importStudent = RoleAclSource.Instance["課程"]["功能按鈕"];
            button_importStudent.Add(new RibbonFeature("Course_Button_ImportCase", "匯入個案基本資料"));
            MotherForm.RibbonBarItems["課程", "個案"]["匯入"]["匯入個案基本資料"].Enable = UserAcl.Current["Course_Button_ImportCase"].Executable;

            MotherForm.RibbonBarItems["課程", "個案"]["匯入"]["匯入個案基本資料"].Click += delegate
                {
                    new Import.Case_Import().Execute();
                };
                #endregion

            #endregion
        }
    }
}
