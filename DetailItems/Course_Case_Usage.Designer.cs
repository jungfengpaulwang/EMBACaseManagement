namespace CaseManagement.DetailItems
{
    partial class Course_Case_Usage
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvData = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.CaseEnglishName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Teacher = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.txtFocus = new DevComponents.DotNetBar.Controls.TextBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.BackgroundColor = System.Drawing.Color.White;
            this.dgvData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CaseEnglishName,
            this.Teacher});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvData.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvData.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvData.Location = new System.Drawing.Point(30, 26);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersWidth = 25;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.Size = new System.Drawing.Size(490, 251);
            this.dgvData.TabIndex = 12;
            // 
            // CaseEnglishName
            // 
            this.CaseEnglishName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CaseEnglishName.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.CaseEnglishName.HeaderText = "個案名稱";
            this.CaseEnglishName.Name = "CaseEnglishName";
            this.CaseEnglishName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CaseEnglishName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Teacher
            // 
            this.Teacher.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Teacher.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Teacher.HeaderText = "教師";
            this.Teacher.Name = "Teacher";
            this.Teacher.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Teacher.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Teacher.Width = 59;
            // 
            // txtFocus
            // 
            // 
            // 
            // 
            this.txtFocus.Border.Class = "TextBoxBorder";
            this.txtFocus.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtFocus.Location = new System.Drawing.Point(528, 256);
            this.txtFocus.Name = "txtFocus";
            this.txtFocus.Size = new System.Drawing.Size(1, 25);
            this.txtFocus.TabIndex = 13;
            this.txtFocus.Visible = false;
            // 
            // Course_Case_Usage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFocus);
            this.Controls.Add(this.dgvData);
            this.Name = "Course_Case_Usage";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(550, 300);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgvData;
        private System.Windows.Forms.DataGridViewComboBoxColumn CaseEnglishName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Teacher;
        private DevComponents.DotNetBar.Controls.TextBoxX txtFocus;
    }
}
