namespace PSA_Application
{
    partial class FormIOInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.BT_CLOSE = new System.Windows.Forms.Button();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.tabOutputModule0 = new System.Windows.Forms.TabPage();
            this.GV_OutputModule = new System.Windows.Forms.DataGridView();
            this.tabInputModule0 = new System.Windows.Forms.TabPage();
            this.GV_InputModule_0 = new System.Windows.Forms.DataGridView();
            this.TC_IOList = new System.Windows.Forms.TabControl();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Module = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewImageColumn4 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabOutputModule0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GV_OutputModule)).BeginInit();
            this.tabInputModule0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GV_InputModule_0)).BeginInit();
            this.TC_IOList.SuspendLayout();
            this.SuspendLayout();
            // 
            // BT_CLOSE
            // 
            this.BT_CLOSE.Font = new System.Drawing.Font("Arial Black", 16F, System.Drawing.FontStyle.Bold);
            this.BT_CLOSE.Location = new System.Drawing.Point(7, 826);
            this.BT_CLOSE.Name = "BT_CLOSE";
            this.BT_CLOSE.Size = new System.Drawing.Size(599, 52);
            this.BT_CLOSE.TabIndex = 3;
            this.BT_CLOSE.Text = "Close";
            this.BT_CLOSE.UseVisualStyleBackColor = true;
            this.BT_CLOSE.Click += new System.EventHandler(this.Cotrol_Click);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 300;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // tabOutputModule0
            // 
            this.tabOutputModule0.Controls.Add(this.GV_OutputModule);
            this.tabOutputModule0.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabOutputModule0.Location = new System.Drawing.Point(4, 23);
            this.tabOutputModule0.Name = "tabOutputModule0";
            this.tabOutputModule0.Padding = new System.Windows.Forms.Padding(3);
            this.tabOutputModule0.Size = new System.Drawing.Size(595, 795);
            this.tabOutputModule0.TabIndex = 3;
            this.tabOutputModule0.Text = "Output Module #0";
            this.tabOutputModule0.UseVisualStyleBackColor = true;
            // 
            // GV_OutputModule
            // 
            this.GV_OutputModule.AllowUserToAddRows = false;
            this.GV_OutputModule.AllowUserToDeleteRows = false;
            this.GV_OutputModule.AllowUserToResizeColumns = false;
            this.GV_OutputModule.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.GV_OutputModule.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.GV_OutputModule.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GV_OutputModule.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GV_OutputModule.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.GV_OutputModule.ColumnHeadersHeight = 24;
            this.GV_OutputModule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GV_OutputModule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn9,
            this.dataGridViewTextBoxColumn10,
            this.dataGridViewTextBoxColumn11,
            this.dataGridViewImageColumn4});
            this.GV_OutputModule.Location = new System.Drawing.Point(6, 6);
            this.GV_OutputModule.MultiSelect = false;
            this.GV_OutputModule.Name = "GV_OutputModule";
            this.GV_OutputModule.ReadOnly = true;
            this.GV_OutputModule.RowHeadersVisible = false;
            this.GV_OutputModule.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.GV_OutputModule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GV_OutputModule.Size = new System.Drawing.Size(583, 783);
            this.GV_OutputModule.TabIndex = 6;
            // 
            // tabInputModule0
            // 
            this.tabInputModule0.Controls.Add(this.GV_InputModule_0);
            this.tabInputModule0.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabInputModule0.Location = new System.Drawing.Point(4, 23);
            this.tabInputModule0.Name = "tabInputModule0";
            this.tabInputModule0.Padding = new System.Windows.Forms.Padding(3);
            this.tabInputModule0.Size = new System.Drawing.Size(595, 795);
            this.tabInputModule0.TabIndex = 0;
            this.tabInputModule0.Text = "Input Module #0";
            this.tabInputModule0.UseVisualStyleBackColor = true;
            // 
            // GV_InputModule_0
            // 
            this.GV_InputModule_0.AllowUserToAddRows = false;
            this.GV_InputModule_0.AllowUserToDeleteRows = false;
            this.GV_InputModule_0.AllowUserToResizeColumns = false;
            this.GV_InputModule_0.AllowUserToResizeRows = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Format = "N0";
            dataGridViewCellStyle6.NullValue = null;
            this.GV_InputModule_0.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.GV_InputModule_0.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.GV_InputModule_0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.GV_InputModule_0.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.GV_InputModule_0.ColumnHeadersHeight = 24;
            this.GV_InputModule_0.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.GV_InputModule_0.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Module,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewImageColumn1});
            this.GV_InputModule_0.Location = new System.Drawing.Point(6, 6);
            this.GV_InputModule_0.MultiSelect = false;
            this.GV_InputModule_0.Name = "GV_InputModule_0";
            this.GV_InputModule_0.ReadOnly = true;
            this.GV_InputModule_0.RowHeadersVisible = false;
            this.GV_InputModule_0.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.GV_InputModule_0.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.GV_InputModule_0.Size = new System.Drawing.Size(583, 783);
            this.GV_InputModule_0.TabIndex = 3;
            // 
            // TC_IOList
            // 
            this.TC_IOList.Controls.Add(this.tabInputModule0);
            this.TC_IOList.Controls.Add(this.tabOutputModule0);
            this.TC_IOList.ItemSize = new System.Drawing.Size(87, 19);
            this.TC_IOList.Location = new System.Drawing.Point(5, 1);
            this.TC_IOList.Name = "TC_IOList";
            this.TC_IOList.SelectedIndex = 0;
            this.TC_IOList.Size = new System.Drawing.Size(603, 822);
            this.TC_IOList.TabIndex = 4;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.NullValue = null;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 250;
            // 
            // Module
            // 
            this.Module.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Module.DefaultCellStyle = dataGridViewCellStyle9;
            this.Module.HeaderText = "Seg";
            this.Module.Name = "Module";
            this.Module.ReadOnly = true;
            this.Module.Width = 140;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.NullValue = null;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn2.HeaderText = "Num";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 140;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn1.HeaderText = "Status";
            this.dataGridViewImageColumn1.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
            this.dataGridViewImageColumn1.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = null;
            this.dataGridViewTextBoxColumn9.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn9.HeaderText = "Name";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.ReadOnly = true;
            this.dataGridViewTextBoxColumn9.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn9.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn9.Width = 250;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn10.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn10.HeaderText = "Seg";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.ReadOnly = true;
            this.dataGridViewTextBoxColumn10.Width = 140;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.NullValue = null;
            this.dataGridViewTextBoxColumn11.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn11.HeaderText = "Num";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.ReadOnly = true;
            this.dataGridViewTextBoxColumn11.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn11.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn11.Width = 140;
            // 
            // dataGridViewImageColumn4
            // 
            this.dataGridViewImageColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn4.HeaderText = "Status";
            this.dataGridViewImageColumn4.Image = global::PSA_Application.Properties.Resources.YellowLED_OFF;
            this.dataGridViewImageColumn4.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.dataGridViewImageColumn4.Name = "dataGridViewImageColumn4";
            this.dataGridViewImageColumn4.ReadOnly = true;
            this.dataGridViewImageColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn4.Width = 50;
            // 
            // FormIOInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 882);
            this.ControlBox = false;
            this.Controls.Add(this.TC_IOList);
            this.Controls.Add(this.BT_CLOSE);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormIOInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Display IO Status";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormIOInfo_Load);
            this.Shown += new System.EventHandler(this.FormIOInfo_Shown);
            this.tabOutputModule0.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GV_OutputModule)).EndInit();
            this.tabInputModule0.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.GV_InputModule_0)).EndInit();
            this.TC_IOList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BT_CLOSE;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.TabPage tabOutputModule0;
        private System.Windows.Forms.DataGridView GV_OutputModule;
        private System.Windows.Forms.TabPage tabInputModule0;
        private System.Windows.Forms.DataGridView GV_InputModule_0;
        private System.Windows.Forms.TabControl TC_IOList;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Module;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
    }
}