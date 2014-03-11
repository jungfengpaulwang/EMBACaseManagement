using System;
using FISCA.UDT;

namespace CaseManagement.UDT
{
    /// <summary>
    /// oAuth 帳號管理
    /// </summary>
    [FISCA.UDT.TableName("ischool.emba.case_management.oauth_account")]
    public class oAuthAccount : ActiveRecord
    {
        /// <summary>
        /// ClientID
        /// </summary>
        [Field(Field = "ClientID", Indexed = false, Caption = "ClientID")]
        public string ClientID { get; set; }

        /// <summary>
        /// Secret
        /// </summary>
        [Field(Field = "secret", Indexed = false, Caption = "Secret")]
        public string Secret { get; set; }

        /// <summary>
        /// 淺層複製物件
        /// </summary>
        /// <returns></returns>
        public oAuthAccount Clone()
        {
            return this.MemberwiseClone() as oAuthAccount;
        }
    }
}
