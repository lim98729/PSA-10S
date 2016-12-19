namespace PSA_Application
{
    partial class FormMagazineEdit
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
            this.BT_SKIP = new System.Windows.Forms.Button();
            this.BT_READY = new System.Windows.Forms.Button();
            this.BT_DONE = new System.Windows.Forms.Button();
            this.BT_Cancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.BT_Apply = new System.Windows.Forms.Button();
            this.BT_MGStatus = new System.Windows.Forms.Button();
            this.CB_SELECTALL = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BT_SKIP
            // 
            this.BT_SKIP.BackColor = System.Drawing.Color.Black;
            this.BT_SKIP.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BT_SKIP.ForeColor = System.Drawing.Color.White;
            this.BT_SKIP.Location = new System.Drawing.Point(527, 35);
            this.BT_SKIP.Name = "BT_SKIP";
            this.BT_SKIP.Size = new System.Drawing.Size(76, 50);
            this.BT_SKIP.TabIndex = 2;
            this.BT_SKIP.Text = "Skip";
            this.BT_SKIP.UseVisualStyleBackColor = false;
            this.BT_SKIP.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_READY
            // 
            this.BT_READY.BackColor = System.Drawing.Color.Yellow;
            this.BT_READY.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BT_READY.Location = new System.Drawing.Point(363, 35);
            this.BT_READY.Name = "BT_READY";
            this.BT_READY.Size = new System.Drawing.Size(76, 50);
            this.BT_READY.TabIndex = 3;
            this.BT_READY.Text = "Ready";
            this.BT_READY.UseVisualStyleBackColor = false;
            this.BT_READY.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_DONE
            // 
            this.BT_DONE.BackColor = System.Drawing.Color.LimeGreen;
            this.BT_DONE.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.BT_DONE.Location = new System.Drawing.Point(445, 34);
            this.BT_DONE.Name = "BT_DONE";
            this.BT_DONE.Size = new System.Drawing.Size(76, 50);
            this.BT_DONE.TabIndex = 4;
            this.BT_DONE.Text = "DONE";
            this.BT_DONE.UseVisualStyleBackColor = false;
            this.BT_DONE.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Cancel
            // 
            this.BT_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Cancel.Location = new System.Drawing.Point(513, 276);
            this.BT_Cancel.Name = "BT_Cancel";
            this.BT_Cancel.Size = new System.Drawing.Size(90, 71);
            this.BT_Cancel.TabIndex = 7;
            this.BT_Cancel.Text = "Cancel";
            this.BT_Cancel.UseVisualStyleBackColor = true;
            this.BT_Cancel.Click += new System.EventHandler(this.Control_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(12, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(108, 312);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(126, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(108, 312);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(240, 35);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(108, 312);
            this.panel3.TabIndex = 9;
            // 
            // BT_Apply
            // 
            this.BT_Apply.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_Apply.Location = new System.Drawing.Point(363, 276);
            this.BT_Apply.Name = "BT_Apply";
            this.BT_Apply.Size = new System.Drawing.Size(90, 71);
            this.BT_Apply.TabIndex = 10;
            this.BT_Apply.Text = "Apply";
            this.BT_Apply.UseVisualStyleBackColor = true;
            this.BT_Apply.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_MGStatus
            // 
            this.BT_MGStatus.BackColor = System.Drawing.Color.White;
            this.BT_MGStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_MGStatus.ForeColor = System.Drawing.Color.Black;
            this.BT_MGStatus.Location = new System.Drawing.Point(445, 120);
            this.BT_MGStatus.Name = "BT_MGStatus";
            this.BT_MGStatus.Size = new System.Drawing.Size(76, 45);
            this.BT_MGStatus.TabIndex = 299;
            this.BT_MGStatus.Text = "None";
            this.BT_MGStatus.UseVisualStyleBackColor = false;
            this.BT_MGStatus.Click += new System.EventHandler(this.Control_Click);
            // 
            // CB_SELECTALL
            // 
            this.CB_SELECTALL.AutoSize = true;
            this.CB_SELECTALL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CB_SELECTALL.Location = new System.Drawing.Point(527, 133);
            this.CB_SELECTALL.Name = "CB_SELECTALL";
            this.CB_SELECTALL.Size = new System.Drawing.Size(45, 20);
            this.CB_SELECTALL.TabIndex = 298;
            this.CB_SELECTALL.Text = "All";
            this.CB_SELECTALL.UseVisualStyleBackColor = true;
            this.CB_SELECTALL.CheckedChanged += new System.EventHandler(this.Control_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("HY견고딕", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(27, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 19);
            this.label1.TabIndex = 300;
            this.label1.Text = "MG #1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("HY견고딕", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(141, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 19);
            this.label2.TabIndex = 301;
            this.label2.Text = "MG #2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("HY견고딕", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(255, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 19);
            this.label3.TabIndex = 302;
            this.label3.Text = "MG #3";
            // 
            // FormMagazineEdit
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(613, 363);
            this.ControlBox = false;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BT_MGStatus);
            this.Controls.Add(this.CB_SELECTALL);
            this.Controls.Add(this.BT_Apply);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BT_Cancel);
            this.Controls.Add(this.BT_DONE);
            this.Controls.Add(this.BT_READY);
            this.Controls.Add(this.BT_SKIP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMagazineEdit";
            this.ShowIcon = false;
            this.Text = "FormMagazineEdit";
            this.Load += new System.EventHandler(this.FormMagazineEdit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_SKIP;
        private System.Windows.Forms.Button BT_READY;
        private System.Windows.Forms.Button BT_DONE;
        private System.Windows.Forms.Button BT_Cancel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button BT_Apply;
		private System.Windows.Forms.Button BT_MGStatus;
        private System.Windows.Forms.CheckBox CB_SELECTALL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;


    }
}