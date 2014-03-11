namespace CaseManagement.DetailItems
{
    partial class Teacher_CaseHistories
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
            this.SchoolYear = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.Semester = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CourseName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CaseEnglishName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.CaseName = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToOrderColumns = true;
            this.dgvData.BackgroundColor = System.Drawing.Color.White;
            this.dgvData.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SchoolYear,
            this.Semester,
            this.CourseName,
            this.CaseEnglishName,
            this.CaseName});
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
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersWidth = 25;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.Size = new System.Drawing.Size(490, 250);
            this.dgvData.TabIndex = 12;
            // 
            // SchoolYear
            // 
            this.SchoolYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.SchoolYear.HeaderText = "學年度";
            this.SchoolYear.Name = "SchoolYear";
            this.SchoolYear.ReadOnly = true;
            this.SchoolYear.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SchoolYear.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SchoolYear.Width = 72;
            // 
            // Semester
            // 
            this.Semester.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Semester.HeaderText = "學期";
            this.Semester.Name = "Semester";
            this.Semester.ReadOnly = true;
            this.Semester.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Semester.TextAlignment = System.Drawing.StringAlignment.Center;
            this.Semester.Width = 40;
            // 
            // CourseName
            // 
            this.CourseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CourseName.HeaderText = "開課";
            this.CourseName.Name = "CourseName";
            this.CourseName.ReadOnly = true;
            this.CourseName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CourseName.Width = 40;
            // 
            // CaseEnglishName
            // 
            this.CaseEnglishName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CaseEnglishName.HeaderText = "個案英文名稱";
            this.CaseEnglishName.Name = "CaseEnglishName";
            this.CaseEnglishName.ReadOnly = true;
            this.CaseEnglishName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CaseEnglishName.TextAlignment = System.Drawing.StringAlignment.Center;
            this.CaseEnglishName.Width = 92;
            // 
            // CaseName
            // 
            this.CaseName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CaseName.HeaderText = "個案中文名稱";
            this.CaseName.Name = "CaseName";
            this.CaseName.ReadOnly = true;
            this.CaseName.Width = 92;
            // 
            // Teacher_CaseHistories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvData);
            this.Name = "Teacher_CaseHistories";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(550, 300);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dgvData;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn SchoolYear;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn Semester;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CourseName;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CaseEnglishName;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn CaseName;
    }
}
