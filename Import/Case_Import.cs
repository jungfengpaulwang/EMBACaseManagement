using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using EMBA.DocumentValidator;
using EMBA.Import;
using EMBA.Validator;
using FISCA.Data;
using FISCA.UDT;
using K12.Data;
using K12.Data.Configuration;
using System;
using System.Text;

namespace CaseManagement.Import
{
    class Case_Import : ImportWizard
    {
        ImportOption mOption;
        string keyField;
        AccessHelper Access;
        QueryHelper queryHelper;
        Dictionary<string, string> dicGoogleDocs;

        public Case_Import()
        {
            this.IsSplit = false;
            this.ShowAdvancedForm = false;
            this.ValidateRuleFormater = XDocument.Parse(Properties.Resources.format);
            //this.CustomValidate = null;
            //this.SplitThreadCount = 5;
            //this.SplitSize = 3000;

            keyField = string.Empty;
            dicGoogleDocs = new Dictionary<string,string>();

            Access = new AccessHelper();
            queryHelper = new QueryHelper();
            
            this.CustomValidate = (Rows, Messages) =>
            {
                CustomValidator(Rows, Messages);
            };
        }

        public void CustomValidator(List<IRowStream> Rows, RowMessages Messages)
        {
            #  region 驗證流程
            if (this.SelectedKeyFields.Contains("個案英文名稱"))
                keyField = "個案英文名稱";
            else
                keyField = "個案英文名稱";

            ConfigData config = K12.Data.School.Configuration["台大EMBA個案文件雲端資料夾"];
            string strCloudFolder = config["CloudFolder"];

            if (string.IsNullOrEmpty(strCloudFolder))
                throw new Exception("未設定雲端資料夾，無法比對文件連結！");

            List<UDT.oAuthAccount> oAuthAccounts = Access.Select<UDT.oAuthAccount>();
            if (oAuthAccounts.Count == 0)
            {
                throw new Exception("未設定「oAuth 帳號」。");
            }

            Google.Apis.Helper.ClientCredentials.CLIENT_ID = oAuthAccounts.ElementAt(0).ClientID;
            Google.Apis.Helper.ClientCredentials.CLIENT_SECRET = oAuthAccounts.ElementAt(0).Secret;

            List<Google.Apis.Drive.v2.Data.File> FilterFiles = new List<Google.Apis.Drive.v2.Data.File>();
            FilterFiles = MainClass.AuthorizeAndListDirectoryFiles(strCloudFolder);
            if (FilterFiles.Count == 0)
            {
                MainClass.DeleteCachedRefreshToken();
                throw new Exception("設定之「雲端資料夾」沒有檔案，請確認您的帳號可存取該資料夾。");
            }
            foreach (Google.Apis.Drive.v2.Data.File File in FilterFiles)
            {
                if (!dicGoogleDocs.ContainsKey(File.Title))
                    dicGoogleDocs.Add(File.Title, File.AlternateLink);
            }

            #endregion
        }

        public override XDocument GetValidateRule()
        {
            return XDocument.Parse(Properties.Resources.Case_Import);
        }

        public override ImportAction GetSupportActions()
        {
            return ImportAction.InsertOrUpdate;
        }

        public override void Prepare(ImportOption Option)
        {
            mOption = Option;
        }

        public override string Import(List<IRowStream> Rows)
        {
            List<UDT.Case> Cases = Access.Select<UDT.Case>();
            Dictionary<string, UDT.Case> dicCases = new Dictionary<string, UDT.Case>();
            foreach (UDT.Case cazz in Cases)
            {
                if (!string.IsNullOrEmpty(cazz.EnglishName))
                {
                    if (!dicCases.ContainsKey(cazz.EnglishName))
                        dicCases.Add(cazz.EnglishName, cazz);
                }
                else
                {
                    if (!dicCases.ContainsKey(cazz.Name))
                        dicCases.Add(cazz.Name, cazz);
                }
            }
            foreach (IRowStream row in Rows)
            {
                string case_english = row.GetValue("學年度").Trim().ToLower();
                UDT.Case c = new UDT.Case();

                if (dicCases.ContainsKey((c.EnglishName + "").Trim().ToLower())) 
                    c = dicCases[(c.EnglishName + "").Trim().ToLower()];
                else if (dicCases.ContainsKey((c.Name + "").Trim().ToLower()))
                    c = dicCases[(c.Name + "").Trim().ToLower()];

                //  個案中文名稱  個案編號    作者  出版學校    備註  文件名稱
                c.EnglishName = row.GetValue("個案英文名稱").Trim();

                if (mOption.SelectedFields.Contains("個案中文名稱") && !string.IsNullOrWhiteSpace(row.GetValue("個案中文名稱")))
                {
                    c.Name = row.GetValue("個案中文名稱").Trim();
                }
                if (mOption.SelectedFields.Contains("個案編號") && !string.IsNullOrWhiteSpace(row.GetValue("個案編號")))
                {
                    c.Name = row.GetValue("個案編號").Trim();
                }
                if (mOption.SelectedFields.Contains("作者") && !string.IsNullOrWhiteSpace(row.GetValue("作者")))
                {
                    c.Name = row.GetValue("作者").Trim();
                }
                if (mOption.SelectedFields.Contains("出版學校") && !string.IsNullOrWhiteSpace(row.GetValue("出版學校")))
                {
                    c.Name = row.GetValue("出版學校").Trim();
                }
                if (mOption.SelectedFields.Contains("備註") && !string.IsNullOrWhiteSpace(row.GetValue("備註")))
                {
                    c.Name = row.GetValue("備註").Trim();
                }
                List<string> document_list = new List<string>();
                if (mOption.SelectedFields.Contains("文件名稱:1") && !string.IsNullOrWhiteSpace(row.GetValue("文件名稱:1")))
                    document_list.Add(row.GetValue("文件名稱:1").Trim());
                if (mOption.SelectedFields.Contains("文件名稱:2") && !string.IsNullOrWhiteSpace(row.GetValue("文件名稱:2")))
                    document_list.Add(row.GetValue("文件名稱:2").Trim());
                if (mOption.SelectedFields.Contains("文件名稱:3") && !string.IsNullOrWhiteSpace(row.GetValue("文件名稱:3")))
                    document_list.Add(row.GetValue("文件名稱:3").Trim());
                if (mOption.SelectedFields.Contains("文件名稱:4") && !string.IsNullOrWhiteSpace(row.GetValue("文件名稱:4")))
                    document_list.Add(row.GetValue("文件名稱:4").Trim());
                if (mOption.SelectedFields.Contains("文件名稱:5") && !string.IsNullOrWhiteSpace(row.GetValue("文件名稱:5")))
                    document_list.Add(row.GetValue("文件名稱:5").Trim());
                
                Dictionary<string, string> dicDocumentList = new Dictionary<string, string>();
                //  先記錄原有之文件名稱及連結
                if (c.RecordStatus != RecordStatus.Insert)
                {
                    XDocument xDocument = XDocument.Parse(c.UrlList.Replace("&", "&#038;").Replace("\"", "&#034;").Replace("\'", "&#039;").Replace("<", "&#060;").Replace(">", "&#062;"));
                    IEnumerable<XElement> xElements = xDocument.Descendants("CaseDocument");
                    foreach (XElement xElement in xElements)
                    {
                        string Url = xElement.Attribute("URL").Value;
                        string Url_Decode = string.Empty;

                        if (!string.IsNullOrWhiteSpace(Url))
                        {
                            byte[] utf8Bytes = Convert.FromBase64String(Url);
                            Url_Decode = Encoding.UTF8.GetString(utf8Bytes);
                        }
                        if (!dicDocumentList.ContainsKey(xElement.Attribute("FileName").Value.Trim()))
                            dicDocumentList.Add(xElement.Attribute("FileName").Value.Trim(), Url_Decode);
                    }
                }

                //  再記錄匯入之文件名稱及連結
                document_list.ForEach((x) =>
                {
                    if (!dicDocumentList.ContainsKey(x))
                    {
                        if (this.dicGoogleDocs.ContainsKey(x))
                            dicDocumentList.Add(x, this.dicGoogleDocs[x]);
                        else
                            dicDocumentList.Add(x, string.Empty);
                    }
                });

                //  接下來整理成xml格式
                StringBuilder sb = new StringBuilder("<CaseDocuments>");
                foreach (string key in dicDocumentList.Keys)
                {
                    string Url = dicDocumentList[key];
                    byte[] array = Encoding.UTF8.GetBytes(Url);
                    string Url_Encode = Convert.ToBase64String(array);
                    sb.Append(string.Format(@"<CaseDocument FileName=""{0}"" URL=""{1}"" Memo=""{2}"" />", key, Url_Encode, string.Empty));
                }
                sb.Append("</CaseDocuments>");
                c.UrlList = sb.ToString();

                if (c.RecordStatus == RecordStatus.Insert)
                    Cases.Add(c);
            }

            Cases.SaveAll();

            return string.Empty;
        }
    }
}
