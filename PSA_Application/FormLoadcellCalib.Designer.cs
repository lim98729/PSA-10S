namespace PSA_Application
{
	partial class FormLoadcellCalib
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLoadcellCalib));
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.TC_Loadcell_T = new System.Windows.Forms.TabControl();
            this.TC_Loadcell_1 = new System.Windows.Forms.TabPage();
            this.UD_TargetForce = new System.Windows.Forms.DomainUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.BT_AutoZPos = new System.Windows.Forms.Button();
            this.LB_HeadLC = new System.Windows.Forms.Label();
            this.LB_BottomLC = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LB_Force_FactorX = new System.Windows.Forms.Label();
            this.TC_Loadcell_2 = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.TB_RepeatCheckSpeed = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.BT_HeadPressDryRun = new System.Windows.Forms.Button();
            this.TB_RepeatCheckCount = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TB_RepeatCheckForce = new System.Windows.Forms.TextBox();
            this.BT_CheckRepeatForce = new System.Windows.Forms.Button();
            this.BT_Check2LoadcellForce = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.LB_CurForce = new System.Windows.Forms.Label();
            this.LB_CurDist = new System.Windows.Forms.Label();
            this.LB_OutVolt = new System.Windows.Forms.Label();
            this.TB_CheckDelay = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BT_StopCalib = new System.Windows.Forms.Button();
            this.CB_CheckDistance = new System.Windows.Forms.ComboBox();
            this.CB_VoltageInterval = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BT_AutoCalib = new System.Windows.Forms.Button();
            this.BT_Close = new System.Windows.Forms.Button();
            this.cb_selectedTool = new System.Windows.Forms.ComboBox();
            this.label = new System.Windows.Forms.Label();
            this.TC_Loadcell_T.SuspendLayout();
            this.TC_Loadcell_1.SuspendLayout();
            this.TC_Loadcell_2.SuspendLayout();
            this.SuspendLayout();
            // 
            // refreshTimer
            // 
            this.refreshTimer.Interval = 200;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // TC_Loadcell_T
            // 
            this.TC_Loadcell_T.Controls.Add(this.TC_Loadcell_1);
            this.TC_Loadcell_T.Controls.Add(this.TC_Loadcell_2);
            this.TC_Loadcell_T.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TC_Loadcell_T.Location = new System.Drawing.Point(12, 74);
            this.TC_Loadcell_T.Name = "TC_Loadcell_T";
            this.TC_Loadcell_T.SelectedIndex = 0;
            this.TC_Loadcell_T.Size = new System.Drawing.Size(635, 381);
            this.TC_Loadcell_T.TabIndex = 0;
            // 
            // TC_Loadcell_1
            // 
            this.TC_Loadcell_1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.TC_Loadcell_1.Controls.Add(this.UD_TargetForce);
            this.TC_Loadcell_1.Controls.Add(this.label3);
            this.TC_Loadcell_1.Controls.Add(this.BT_AutoZPos);
            this.TC_Loadcell_1.Controls.Add(this.LB_HeadLC);
            this.TC_Loadcell_1.Controls.Add(this.LB_BottomLC);
            this.TC_Loadcell_1.Controls.Add(this.label1);
            this.TC_Loadcell_1.Controls.Add(this.LB_Force_FactorX);
            this.TC_Loadcell_1.Location = new System.Drawing.Point(4, 24);
            this.TC_Loadcell_1.Name = "TC_Loadcell_1";
            this.TC_Loadcell_1.Padding = new System.Windows.Forms.Padding(3);
            this.TC_Loadcell_1.Size = new System.Drawing.Size(627, 353);
            this.TC_Loadcell_1.TabIndex = 0;
            this.TC_Loadcell_1.Text = "Bottom-Head Calib";
            // 
            // UD_TargetForce
            // 
            this.UD_TargetForce.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UD_TargetForce.Location = new System.Drawing.Point(250, 129);
            this.UD_TargetForce.Name = "UD_TargetForce";
            this.UD_TargetForce.Size = new System.Drawing.Size(93, 39);
            this.UD_TargetForce.TabIndex = 214;
            this.UD_TargetForce.Text = "2";
            this.UD_TargetForce.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(140, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 213;
            this.label3.Text = "Target Force";
            // 
            // BT_AutoZPos
            // 
            this.BT_AutoZPos.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_AutoZPos.BackgroundImage")));
            this.BT_AutoZPos.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_AutoZPos.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_AutoZPos.FlatAppearance.BorderSize = 0;
            this.BT_AutoZPos.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_AutoZPos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_AutoZPos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_AutoZPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_AutoZPos.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_AutoZPos.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_AutoZPos.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_AutoZPos.Location = new System.Drawing.Point(381, 120);
            this.BT_AutoZPos.Name = "BT_AutoZPos";
            this.BT_AutoZPos.Size = new System.Drawing.Size(136, 54);
            this.BT_AutoZPos.TabIndex = 212;
            this.BT_AutoZPos.TabStop = false;
            this.BT_AutoZPos.Text = "Find Z Pos";
            this.BT_AutoZPos.UseVisualStyleBackColor = true;
            this.BT_AutoZPos.Click += new System.EventHandler(this.Control_Click);
            // 
            // LB_HeadLC
            // 
            this.LB_HeadLC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_HeadLC.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_HeadLC.ForeColor = System.Drawing.Color.Black;
            this.LB_HeadLC.Location = new System.Drawing.Point(250, 57);
            this.LB_HeadLC.Name = "LB_HeadLC";
            this.LB_HeadLC.Size = new System.Drawing.Size(93, 18);
            this.LB_HeadLC.TabIndex = 211;
            this.LB_HeadLC.Text = "0.00";
            this.LB_HeadLC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LB_BottomLC
            // 
            this.LB_BottomLC.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_BottomLC.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_BottomLC.ForeColor = System.Drawing.Color.Black;
            this.LB_BottomLC.Location = new System.Drawing.Point(250, 90);
            this.LB_BottomLC.Name = "LB_BottomLC";
            this.LB_BottomLC.Size = new System.Drawing.Size(93, 18);
            this.LB_BottomLC.TabIndex = 210;
            this.LB_BottomLC.Text = "0.00";
            this.LB_BottomLC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(126, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 209;
            this.label1.Text = "Head Loadcell";
            // 
            // LB_Force_FactorX
            // 
            this.LB_Force_FactorX.AutoSize = true;
            this.LB_Force_FactorX.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Force_FactorX.ForeColor = System.Drawing.Color.Black;
            this.LB_Force_FactorX.Location = new System.Drawing.Point(112, 91);
            this.LB_Force_FactorX.Name = "LB_Force_FactorX";
            this.LB_Force_FactorX.Size = new System.Drawing.Size(112, 16);
            this.LB_Force_FactorX.TabIndex = 208;
            this.LB_Force_FactorX.Text = "Bottom Loadcell";
            // 
            // TC_Loadcell_2
            // 
            this.TC_Loadcell_2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.TC_Loadcell_2.Controls.Add(this.label14);
            this.TC_Loadcell_2.Controls.Add(this.label13);
            this.TC_Loadcell_2.Controls.Add(this.label12);
            this.TC_Loadcell_2.Controls.Add(this.TB_RepeatCheckSpeed);
            this.TC_Loadcell_2.Controls.Add(this.label11);
            this.TC_Loadcell_2.Controls.Add(this.BT_HeadPressDryRun);
            this.TC_Loadcell_2.Controls.Add(this.TB_RepeatCheckCount);
            this.TC_Loadcell_2.Controls.Add(this.label10);
            this.TC_Loadcell_2.Controls.Add(this.label9);
            this.TC_Loadcell_2.Controls.Add(this.TB_RepeatCheckForce);
            this.TC_Loadcell_2.Controls.Add(this.BT_CheckRepeatForce);
            this.TC_Loadcell_2.Controls.Add(this.BT_Check2LoadcellForce);
            this.TC_Loadcell_2.Controls.Add(this.label8);
            this.TC_Loadcell_2.Controls.Add(this.label7);
            this.TC_Loadcell_2.Controls.Add(this.label6);
            this.TC_Loadcell_2.Controls.Add(this.LB_CurForce);
            this.TC_Loadcell_2.Controls.Add(this.LB_CurDist);
            this.TC_Loadcell_2.Controls.Add(this.LB_OutVolt);
            this.TC_Loadcell_2.Controls.Add(this.TB_CheckDelay);
            this.TC_Loadcell_2.Controls.Add(this.label5);
            this.TC_Loadcell_2.Controls.Add(this.BT_StopCalib);
            this.TC_Loadcell_2.Controls.Add(this.CB_CheckDistance);
            this.TC_Loadcell_2.Controls.Add(this.CB_VoltageInterval);
            this.TC_Loadcell_2.Controls.Add(this.label4);
            this.TC_Loadcell_2.Controls.Add(this.label2);
            this.TC_Loadcell_2.Controls.Add(this.BT_AutoCalib);
            this.TC_Loadcell_2.Location = new System.Drawing.Point(4, 24);
            this.TC_Loadcell_2.Name = "TC_Loadcell_2";
            this.TC_Loadcell_2.Padding = new System.Windows.Forms.Padding(3);
            this.TC_Loadcell_2.Size = new System.Drawing.Size(627, 353);
            this.TC_Loadcell_2.TabIndex = 1;
            this.TC_Loadcell_2.Text = "Head Force Calib";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(178, 69);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 15);
            this.label14.TabIndex = 238;
            this.label14.Text = "[count]";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(178, 19);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 15);
            this.label13.TabIndex = 237;
            this.label13.Text = "[kg]";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(178, 44);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(47, 15);
            this.label12.TabIndex = 236;
            this.label12.Text = "[mm/s]";
            // 
            // TB_RepeatCheckSpeed
            // 
            this.TB_RepeatCheckSpeed.Location = new System.Drawing.Point(115, 41);
            this.TB_RepeatCheckSpeed.Name = "TB_RepeatCheckSpeed";
            this.TB_RepeatCheckSpeed.Size = new System.Drawing.Size(53, 21);
            this.TB_RepeatCheckSpeed.TabIndex = 235;
            this.TB_RepeatCheckSpeed.Text = "0.8";
            this.TB_RepeatCheckSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 15);
            this.label11.TabIndex = 234;
            this.label11.Text = "Check Speed";
            // 
            // BT_HeadPressDryRun
            // 
            this.BT_HeadPressDryRun.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_HeadPressDryRun.BackgroundImage")));
            this.BT_HeadPressDryRun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_HeadPressDryRun.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_HeadPressDryRun.FlatAppearance.BorderSize = 0;
            this.BT_HeadPressDryRun.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_HeadPressDryRun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_HeadPressDryRun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_HeadPressDryRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_HeadPressDryRun.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_HeadPressDryRun.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_HeadPressDryRun.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_HeadPressDryRun.Location = new System.Drawing.Point(37, 249);
            this.BT_HeadPressDryRun.Margin = new System.Windows.Forms.Padding(0);
            this.BT_HeadPressDryRun.Name = "BT_HeadPressDryRun";
            this.BT_HeadPressDryRun.Size = new System.Drawing.Size(152, 81);
            this.BT_HeadPressDryRun.TabIndex = 233;
            this.BT_HeadPressDryRun.TabStop = false;
            this.BT_HeadPressDryRun.Text = "Head Press DryRun";
            this.BT_HeadPressDryRun.UseVisualStyleBackColor = true;
            this.BT_HeadPressDryRun.Visible = false;
            this.BT_HeadPressDryRun.Click += new System.EventHandler(this.Control_Click);
            // 
            // TB_RepeatCheckCount
            // 
            this.TB_RepeatCheckCount.Location = new System.Drawing.Point(115, 67);
            this.TB_RepeatCheckCount.Name = "TB_RepeatCheckCount";
            this.TB_RepeatCheckCount.Size = new System.Drawing.Size(53, 21);
            this.TB_RepeatCheckCount.TabIndex = 232;
            this.TB_RepeatCheckCount.Text = "100";
            this.TB_RepeatCheckCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 69);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 15);
            this.label10.TabIndex = 231;
            this.label10.Text = "Check Num";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 15);
            this.label9.TabIndex = 230;
            this.label9.Text = "Check Force";
            // 
            // TB_RepeatCheckForce
            // 
            this.TB_RepeatCheckForce.Location = new System.Drawing.Point(115, 17);
            this.TB_RepeatCheckForce.Name = "TB_RepeatCheckForce";
            this.TB_RepeatCheckForce.Size = new System.Drawing.Size(53, 21);
            this.TB_RepeatCheckForce.TabIndex = 229;
            this.TB_RepeatCheckForce.Text = "0.5";
            this.TB_RepeatCheckForce.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BT_CheckRepeatForce
            // 
            this.BT_CheckRepeatForce.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_CheckRepeatForce.BackgroundImage")));
            this.BT_CheckRepeatForce.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_CheckRepeatForce.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_CheckRepeatForce.FlatAppearance.BorderSize = 0;
            this.BT_CheckRepeatForce.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_CheckRepeatForce.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_CheckRepeatForce.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_CheckRepeatForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_CheckRepeatForce.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_CheckRepeatForce.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_CheckRepeatForce.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_CheckRepeatForce.Location = new System.Drawing.Point(303, 11);
            this.BT_CheckRepeatForce.Margin = new System.Windows.Forms.Padding(0);
            this.BT_CheckRepeatForce.Name = "BT_CheckRepeatForce";
            this.BT_CheckRepeatForce.Size = new System.Drawing.Size(152, 81);
            this.BT_CheckRepeatForce.TabIndex = 228;
            this.BT_CheckRepeatForce.TabStop = false;
            this.BT_CheckRepeatForce.Text = "Check Repeat Force";
            this.BT_CheckRepeatForce.UseVisualStyleBackColor = true;
            this.BT_CheckRepeatForce.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Check2LoadcellForce
            // 
            this.BT_Check2LoadcellForce.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Check2LoadcellForce.BackgroundImage")));
            this.BT_Check2LoadcellForce.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Check2LoadcellForce.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Check2LoadcellForce.FlatAppearance.BorderSize = 0;
            this.BT_Check2LoadcellForce.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Check2LoadcellForce.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Check2LoadcellForce.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Check2LoadcellForce.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Check2LoadcellForce.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Check2LoadcellForce.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Check2LoadcellForce.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Check2LoadcellForce.Location = new System.Drawing.Point(37, 127);
            this.BT_Check2LoadcellForce.Margin = new System.Windows.Forms.Padding(0);
            this.BT_Check2LoadcellForce.Name = "BT_Check2LoadcellForce";
            this.BT_Check2LoadcellForce.Size = new System.Drawing.Size(152, 81);
            this.BT_Check2LoadcellForce.TabIndex = 227;
            this.BT_Check2LoadcellForce.TabStop = false;
            this.BT_Check2LoadcellForce.Text = "Check 2Loadcell Force";
            this.BT_Check2LoadcellForce.UseVisualStyleBackColor = true;
            this.BT_Check2LoadcellForce.Visible = false;
            this.BT_Check2LoadcellForce.Click += new System.EventHandler(this.Control_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(501, 212);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 16);
            this.label8.TabIndex = 226;
            this.label8.Text = "Force";
            this.label8.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(391, 212);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 16);
            this.label7.TabIndex = 225;
            this.label7.Text = "Dist";
            this.label7.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(280, 212);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 16);
            this.label6.TabIndex = 224;
            this.label6.Text = "Volt";
            this.label6.Visible = false;
            // 
            // LB_CurForce
            // 
            this.LB_CurForce.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_CurForce.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_CurForce.ForeColor = System.Drawing.Color.Black;
            this.LB_CurForce.Location = new System.Drawing.Point(552, 210);
            this.LB_CurForce.Name = "LB_CurForce";
            this.LB_CurForce.Size = new System.Drawing.Size(70, 18);
            this.LB_CurForce.TabIndex = 223;
            this.LB_CurForce.Text = "0";
            this.LB_CurForce.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_CurForce.Visible = false;
            // 
            // LB_CurDist
            // 
            this.LB_CurDist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_CurDist.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_CurDist.ForeColor = System.Drawing.Color.Black;
            this.LB_CurDist.Location = new System.Drawing.Point(434, 210);
            this.LB_CurDist.Name = "LB_CurDist";
            this.LB_CurDist.Size = new System.Drawing.Size(59, 18);
            this.LB_CurDist.TabIndex = 222;
            this.LB_CurDist.Text = "0";
            this.LB_CurDist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_CurDist.Visible = false;
            // 
            // LB_OutVolt
            // 
            this.LB_OutVolt.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LB_OutVolt.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_OutVolt.ForeColor = System.Drawing.Color.Black;
            this.LB_OutVolt.Location = new System.Drawing.Point(325, 210);
            this.LB_OutVolt.Name = "LB_OutVolt";
            this.LB_OutVolt.Size = new System.Drawing.Size(59, 18);
            this.LB_OutVolt.TabIndex = 221;
            this.LB_OutVolt.Text = "0";
            this.LB_OutVolt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LB_OutVolt.Visible = false;
            // 
            // TB_CheckDelay
            // 
            this.TB_CheckDelay.Location = new System.Drawing.Point(173, 71);
            this.TB_CheckDelay.Name = "TB_CheckDelay";
            this.TB_CheckDelay.Size = new System.Drawing.Size(93, 21);
            this.TB_CheckDelay.TabIndex = 220;
            this.TB_CheckDelay.Text = "1000";
            this.TB_CheckDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TB_CheckDelay.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(34, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 16);
            this.label5.TabIndex = 219;
            this.label5.Text = "Check Delay[ms]";
            this.label5.Visible = false;
            // 
            // BT_StopCalib
            // 
            this.BT_StopCalib.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_StopCalib.BackgroundImage")));
            this.BT_StopCalib.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_StopCalib.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_StopCalib.FlatAppearance.BorderSize = 0;
            this.BT_StopCalib.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_StopCalib.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_StopCalib.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_StopCalib.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_StopCalib.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_StopCalib.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_StopCalib.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_StopCalib.Location = new System.Drawing.Point(466, 11);
            this.BT_StopCalib.Margin = new System.Windows.Forms.Padding(0);
            this.BT_StopCalib.Name = "BT_StopCalib";
            this.BT_StopCalib.Size = new System.Drawing.Size(152, 81);
            this.BT_StopCalib.TabIndex = 218;
            this.BT_StopCalib.TabStop = false;
            this.BT_StopCalib.Text = "Stop";
            this.BT_StopCalib.UseVisualStyleBackColor = true;
            this.BT_StopCalib.Click += new System.EventHandler(this.Control_Click);
            // 
            // CB_CheckDistance
            // 
            this.CB_CheckDistance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_CheckDistance.FormattingEnabled = true;
            this.CB_CheckDistance.Items.AddRange(new object[] {
            "50",
            "100",
            "200"});
            this.CB_CheckDistance.Location = new System.Drawing.Point(173, 43);
            this.CB_CheckDistance.Name = "CB_CheckDistance";
            this.CB_CheckDistance.Size = new System.Drawing.Size(93, 23);
            this.CB_CheckDistance.TabIndex = 217;
            this.CB_CheckDistance.Visible = false;
            // 
            // CB_VoltageInterval
            // 
            this.CB_VoltageInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_VoltageInterval.FormattingEnabled = true;
            this.CB_VoltageInterval.Items.AddRange(new object[] {
            "0.5",
            "1.0",
            "2.0"});
            this.CB_VoltageInterval.Location = new System.Drawing.Point(173, 16);
            this.CB_VoltageInterval.Name = "CB_VoltageInterval";
            this.CB_VoltageInterval.Size = new System.Drawing.Size(93, 23);
            this.CB_VoltageInterval.TabIndex = 216;
            this.CB_VoltageInterval.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(10, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(133, 16);
            this.label4.TabIndex = 215;
            this.label4.Text = "Check Distance[um]";
            this.label4.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(20, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 16);
            this.label2.TabIndex = 214;
            this.label2.Text = "Voltage Interval[V]";
            this.label2.Visible = false;
            // 
            // BT_AutoCalib
            // 
            this.BT_AutoCalib.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_AutoCalib.BackgroundImage")));
            this.BT_AutoCalib.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_AutoCalib.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_AutoCalib.FlatAppearance.BorderSize = 0;
            this.BT_AutoCalib.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_AutoCalib.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_AutoCalib.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_AutoCalib.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_AutoCalib.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_AutoCalib.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_AutoCalib.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_AutoCalib.Location = new System.Drawing.Point(307, 150);
            this.BT_AutoCalib.Margin = new System.Windows.Forms.Padding(0);
            this.BT_AutoCalib.Name = "BT_AutoCalib";
            this.BT_AutoCalib.Size = new System.Drawing.Size(152, 54);
            this.BT_AutoCalib.TabIndex = 213;
            this.BT_AutoCalib.TabStop = false;
            this.BT_AutoCalib.Text = "Start Calibration";
            this.BT_AutoCalib.UseVisualStyleBackColor = true;
            this.BT_AutoCalib.Visible = false;
            this.BT_AutoCalib.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Close
            // 
            this.BT_Close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Close.BackgroundImage")));
            this.BT_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Close.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Close.FlatAppearance.BorderSize = 0;
            this.BT_Close.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Close.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Close.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Close.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Close.Location = new System.Drawing.Point(203, 461);
            this.BT_Close.Name = "BT_Close";
            this.BT_Close.Size = new System.Drawing.Size(247, 57);
            this.BT_Close.TabIndex = 215;
            this.BT_Close.TabStop = false;
            this.BT_Close.Text = "Close";
            this.BT_Close.UseVisualStyleBackColor = true;
            this.BT_Close.Click += new System.EventHandler(this.Control_Click);
            // 
            // cb_selectedTool
            // 
            this.cb_selectedTool.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.cb_selectedTool.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_selectedTool.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cb_selectedTool.Font = new System.Drawing.Font("Arial", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_selectedTool.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cb_selectedTool.FormattingEnabled = true;
            this.cb_selectedTool.Items.AddRange(new object[] {
            "TOOL 1",
            "TOOL 2"});
            this.cb_selectedTool.Location = new System.Drawing.Point(218, 10);
            this.cb_selectedTool.Name = "cb_selectedTool";
            this.cb_selectedTool.Size = new System.Drawing.Size(121, 36);
            this.cb_selectedTool.TabIndex = 382;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Font = new System.Drawing.Font("Arial Black", 20F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label.Location = new System.Drawing.Point(12, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(186, 38);
            this.label.TabIndex = 381;
            this.label.Text = "Select Tool";
            // 
            // FormLoadcellCalib
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(656, 530);
            this.ControlBox = false;
            this.Controls.Add(this.cb_selectedTool);
            this.Controls.Add(this.label);
            this.Controls.Add(this.BT_Close);
            this.Controls.Add(this.TC_Loadcell_T);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormLoadcellCalib";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Loadcell Calibration";
            this.Load += new System.EventHandler(this.FormLoadcellCalib_Load);
            this.TC_Loadcell_T.ResumeLayout(false);
            this.TC_Loadcell_1.ResumeLayout(false);
            this.TC_Loadcell_1.PerformLayout();
            this.TC_Loadcell_2.ResumeLayout(false);
            this.TC_Loadcell_2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer refreshTimer;
		private System.Windows.Forms.TabControl TC_Loadcell_T;
		private System.Windows.Forms.TabPage TC_Loadcell_1;
		private System.Windows.Forms.TabPage TC_Loadcell_2;
		private System.Windows.Forms.Label LB_HeadLC;
		private System.Windows.Forms.Label LB_BottomLC;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label LB_Force_FactorX;
		private System.Windows.Forms.DomainUpDown UD_TargetForce;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button BT_AutoZPos;
		private System.Windows.Forms.Button BT_Close;
		private System.Windows.Forms.Button BT_AutoCalib;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox CB_VoltageInterval;
		private System.Windows.Forms.ComboBox CB_CheckDistance;
		private System.Windows.Forms.Button BT_StopCalib;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox TB_CheckDelay;
		private System.Windows.Forms.Label LB_OutVolt;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label LB_CurForce;
        private System.Windows.Forms.Label LB_CurDist;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox TB_RepeatCheckForce;
		private System.Windows.Forms.Button BT_CheckRepeatForce;
		private System.Windows.Forms.Button BT_Check2LoadcellForce;
		private System.Windows.Forms.TextBox TB_RepeatCheckCount;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button BT_HeadPressDryRun;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox TB_RepeatCheckSpeed;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cb_selectedTool;
        private System.Windows.Forms.Label label;
	}
}