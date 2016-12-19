namespace PSA_Application
{
    partial class FormHalconModelTeach
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHalconModelTeach));
            this.TB_Result = new System.Windows.Forms.TextBox();
            this.BT_SpeedT = new System.Windows.Forms.Button();
            this.BT_JogT_CW = new System.Windows.Forms.Button();
            this.BT_AutoTeach = new System.Windows.Forms.Button();
            this.BT_JogY_Inside = new System.Windows.Forms.Button();
            this.BT_JogY_Outside = new System.Windows.Forms.Button();
            this.BT_SpeedXY = new System.Windows.Forms.Button();
            this.BT_JogX_Left = new System.Windows.Forms.Button();
            this.BT_JogX_Right = new System.Windows.Forms.Button();
            this.BT_ESC = new System.Windows.Forms.Button();
            this.BT_Teach = new System.Windows.Forms.Button();
            this.BT_Test = new System.Windows.Forms.Button();
            this.BT_JogT_CCW = new System.Windows.Forms.Button();
            this.bt_create_rect = new System.Windows.Forms.Button();
            this.BT_MOVE_PATTERN = new System.Windows.Forms.Button();
            this.BT_MOVE_CORNER = new System.Windows.Forms.Button();
            this.LB_ExposureValue = new System.Windows.Forms.Label();
            this.LB_Exposure = new System.Windows.Forms.Label();
            this.SB_Exposure = new System.Windows.Forms.HScrollBar();
            this.LB_Channel1Value = new System.Windows.Forms.Label();
            this.LB_Channel2Value = new System.Windows.Forms.Label();
            this.LB_Channel2 = new System.Windows.Forms.Label();
            this.LB_Channel1 = new System.Windows.Forms.Label();
            this.SB_Channel2 = new System.Windows.Forms.HScrollBar();
            this.SB_Channel1 = new System.Windows.Forms.HScrollBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TB_Result
            // 
            this.TB_Result.Font = new System.Drawing.Font("Arial", 8.25F);
            this.TB_Result.Location = new System.Drawing.Point(12, 248);
            this.TB_Result.Multiline = true;
            this.TB_Result.Name = "TB_Result";
            this.TB_Result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Result.Size = new System.Drawing.Size(232, 104);
            this.TB_Result.TabIndex = 257;
            // 
            // BT_SpeedT
            // 
            this.BT_SpeedT.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_SpeedT.BackgroundImage")));
            this.BT_SpeedT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_SpeedT.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_SpeedT.FlatAppearance.BorderSize = 0;
            this.BT_SpeedT.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_SpeedT.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_SpeedT.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_SpeedT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_SpeedT.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_SpeedT.ForeColor = System.Drawing.Color.White;
            this.BT_SpeedT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_SpeedT.Location = new System.Drawing.Point(337, 8);
            this.BT_SpeedT.Name = "BT_SpeedT";
            this.BT_SpeedT.Size = new System.Drawing.Size(70, 70);
            this.BT_SpeedT.TabIndex = 261;
            this.BT_SpeedT.TabStop = false;
            this.BT_SpeedT.Text = "±1";
            this.BT_SpeedT.UseVisualStyleBackColor = true;
            this.BT_SpeedT.Visible = false;
            this.BT_SpeedT.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_JogT_CW
            // 
            this.BT_JogT_CW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogT_CW.BackgroundImage")));
            this.BT_JogT_CW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogT_CW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogT_CW.FlatAppearance.BorderSize = 0;
            this.BT_JogT_CW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogT_CW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogT_CW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogT_CW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogT_CW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogT_CW.ForeColor = System.Drawing.Color.White;
            this.BT_JogT_CW.Image = global::PSA_Application.Properties.Resources.CW_Arrow1;
            this.BT_JogT_CW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogT_CW.Location = new System.Drawing.Point(412, 8);
            this.BT_JogT_CW.Name = "BT_JogT_CW";
            this.BT_JogT_CW.Size = new System.Drawing.Size(70, 70);
            this.BT_JogT_CW.TabIndex = 259;
            this.BT_JogT_CW.TabStop = false;
            this.BT_JogT_CW.UseVisualStyleBackColor = true;
            this.BT_JogT_CW.Visible = false;
            this.BT_JogT_CW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogT_CW.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogT_CW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_AutoTeach
            // 
            this.BT_AutoTeach.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_AutoTeach.BackgroundImage")));
            this.BT_AutoTeach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_AutoTeach.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_AutoTeach.FlatAppearance.BorderSize = 0;
            this.BT_AutoTeach.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_AutoTeach.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_AutoTeach.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_AutoTeach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_AutoTeach.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_AutoTeach.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_AutoTeach.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_AutoTeach.Location = new System.Drawing.Point(262, 248);
            this.BT_AutoTeach.Name = "BT_AutoTeach";
            this.BT_AutoTeach.Size = new System.Drawing.Size(220, 77);
            this.BT_AutoTeach.TabIndex = 258;
            this.BT_AutoTeach.TabStop = false;
            this.BT_AutoTeach.Text = "Auto Teach";
            this.BT_AutoTeach.UseVisualStyleBackColor = true;
            this.BT_AutoTeach.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_JogY_Inside
            // 
            this.BT_JogY_Inside.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogY_Inside.BackgroundImage")));
            this.BT_JogY_Inside.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogY_Inside.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogY_Inside.FlatAppearance.BorderSize = 0;
            this.BT_JogY_Inside.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogY_Inside.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogY_Inside.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogY_Inside.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogY_Inside.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogY_Inside.ForeColor = System.Drawing.Color.White;
            this.BT_JogY_Inside.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogY_Inside.Location = new System.Drawing.Point(97, 8);
            this.BT_JogY_Inside.Name = "BT_JogY_Inside";
            this.BT_JogY_Inside.Size = new System.Drawing.Size(70, 70);
            this.BT_JogY_Inside.TabIndex = 125;
            this.BT_JogY_Inside.TabStop = false;
            this.BT_JogY_Inside.Text = "▲";
            this.BT_JogY_Inside.UseVisualStyleBackColor = true;
            this.BT_JogY_Inside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogY_Inside.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogY_Inside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_JogY_Outside
            // 
            this.BT_JogY_Outside.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogY_Outside.BackgroundImage")));
            this.BT_JogY_Outside.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogY_Outside.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogY_Outside.FlatAppearance.BorderSize = 0;
            this.BT_JogY_Outside.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogY_Outside.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogY_Outside.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogY_Outside.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogY_Outside.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogY_Outside.ForeColor = System.Drawing.Color.White;
            this.BT_JogY_Outside.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogY_Outside.Location = new System.Drawing.Point(97, 150);
            this.BT_JogY_Outside.Name = "BT_JogY_Outside";
            this.BT_JogY_Outside.Size = new System.Drawing.Size(70, 70);
            this.BT_JogY_Outside.TabIndex = 126;
            this.BT_JogY_Outside.TabStop = false;
            this.BT_JogY_Outside.Text = "▼";
            this.BT_JogY_Outside.UseVisualStyleBackColor = true;
            this.BT_JogY_Outside.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogY_Outside.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogY_Outside.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_SpeedXY
            // 
            this.BT_SpeedXY.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_SpeedXY.BackgroundImage")));
            this.BT_SpeedXY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_SpeedXY.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_SpeedXY.FlatAppearance.BorderSize = 0;
            this.BT_SpeedXY.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_SpeedXY.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_SpeedXY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_SpeedXY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_SpeedXY.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BT_SpeedXY.ForeColor = System.Drawing.Color.White;
            this.BT_SpeedXY.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_SpeedXY.Location = new System.Drawing.Point(97, 79);
            this.BT_SpeedXY.Name = "BT_SpeedXY";
            this.BT_SpeedXY.Size = new System.Drawing.Size(70, 70);
            this.BT_SpeedXY.TabIndex = 124;
            this.BT_SpeedXY.TabStop = false;
            this.BT_SpeedXY.Text = "±1";
            this.BT_SpeedXY.UseVisualStyleBackColor = true;
            this.BT_SpeedXY.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_JogX_Left
            // 
            this.BT_JogX_Left.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogX_Left.BackgroundImage")));
            this.BT_JogX_Left.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogX_Left.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogX_Left.FlatAppearance.BorderSize = 0;
            this.BT_JogX_Left.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogX_Left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogX_Left.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogX_Left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogX_Left.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogX_Left.ForeColor = System.Drawing.Color.White;
            this.BT_JogX_Left.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogX_Left.Location = new System.Drawing.Point(21, 79);
            this.BT_JogX_Left.Name = "BT_JogX_Left";
            this.BT_JogX_Left.Size = new System.Drawing.Size(70, 70);
            this.BT_JogX_Left.TabIndex = 123;
            this.BT_JogX_Left.TabStop = false;
            this.BT_JogX_Left.Text = "◀";
            this.BT_JogX_Left.UseVisualStyleBackColor = true;
            this.BT_JogX_Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogX_Left.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogX_Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_JogX_Right
            // 
            this.BT_JogX_Right.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogX_Right.BackgroundImage")));
            this.BT_JogX_Right.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogX_Right.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogX_Right.FlatAppearance.BorderSize = 0;
            this.BT_JogX_Right.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogX_Right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogX_Right.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogX_Right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogX_Right.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogX_Right.ForeColor = System.Drawing.Color.White;
            this.BT_JogX_Right.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogX_Right.Location = new System.Drawing.Point(174, 79);
            this.BT_JogX_Right.Name = "BT_JogX_Right";
            this.BT_JogX_Right.Size = new System.Drawing.Size(70, 70);
            this.BT_JogX_Right.TabIndex = 122;
            this.BT_JogX_Right.TabStop = false;
            this.BT_JogX_Right.Text = "▶";
            this.BT_JogX_Right.UseVisualStyleBackColor = true;
            this.BT_JogX_Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogX_Right.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogX_Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // BT_ESC
            // 
            this.BT_ESC.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_ESC.BackgroundImage")));
            this.BT_ESC.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_ESC.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_ESC.FlatAppearance.BorderSize = 0;
            this.BT_ESC.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_ESC.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_ESC.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_ESC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_ESC.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_ESC.ForeColor = System.Drawing.Color.White;
            this.BT_ESC.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_ESC.Location = new System.Drawing.Point(374, 356);
            this.BT_ESC.Name = "BT_ESC";
            this.BT_ESC.Size = new System.Drawing.Size(108, 59);
            this.BT_ESC.TabIndex = 117;
            this.BT_ESC.TabStop = false;
            this.BT_ESC.Text = "ESC";
            this.BT_ESC.UseVisualStyleBackColor = true;
            this.BT_ESC.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Teach
            // 
            this.BT_Teach.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Teach.BackgroundImage")));
            this.BT_Teach.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Teach.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Teach.FlatAppearance.BorderSize = 0;
            this.BT_Teach.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Teach.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Teach.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Teach.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Teach.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Teach.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Teach.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Teach.Location = new System.Drawing.Point(262, 356);
            this.BT_Teach.Name = "BT_Teach";
            this.BT_Teach.Size = new System.Drawing.Size(108, 59);
            this.BT_Teach.TabIndex = 115;
            this.BT_Teach.TabStop = false;
            this.BT_Teach.Text = "Teach";
            this.BT_Teach.UseVisualStyleBackColor = true;
            this.BT_Teach.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_Test
            // 
            this.BT_Test.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_Test.BackgroundImage")));
            this.BT_Test.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_Test.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_Test.FlatAppearance.BorderSize = 0;
            this.BT_Test.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_Test.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_Test.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_Test.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_Test.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_Test.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_Test.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_Test.Location = new System.Drawing.Point(12, 356);
            this.BT_Test.Name = "BT_Test";
            this.BT_Test.Size = new System.Drawing.Size(232, 59);
            this.BT_Test.TabIndex = 262;
            this.BT_Test.TabStop = false;
            this.BT_Test.Text = "Test";
            this.BT_Test.UseVisualStyleBackColor = true;
            this.BT_Test.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_JogT_CCW
            // 
            this.BT_JogT_CCW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_JogT_CCW.BackgroundImage")));
            this.BT_JogT_CCW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_JogT_CCW.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_JogT_CCW.FlatAppearance.BorderSize = 0;
            this.BT_JogT_CCW.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_JogT_CCW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.BT_JogT_CCW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_JogT_CCW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_JogT_CCW.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_JogT_CCW.ForeColor = System.Drawing.Color.White;
            this.BT_JogT_CCW.Image = global::PSA_Application.Properties.Resources.CCW_Arrow1;
            this.BT_JogT_CCW.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_JogT_CCW.Location = new System.Drawing.Point(262, 8);
            this.BT_JogT_CCW.Name = "BT_JogT_CCW";
            this.BT_JogT_CCW.Size = new System.Drawing.Size(70, 70);
            this.BT_JogT_CCW.TabIndex = 260;
            this.BT_JogT_CCW.TabStop = false;
            this.BT_JogT_CCW.UseVisualStyleBackColor = true;
            this.BT_JogT_CCW.Visible = false;
            this.BT_JogT_CCW.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Control_MouseDown);
            this.BT_JogT_CCW.MouseLeave += new System.EventHandler(this.Control_MouseLeave);
            this.BT_JogT_CCW.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Control_MouseUp);
            // 
            // bt_create_rect
            // 
            this.bt_create_rect.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bt_create_rect.BackgroundImage")));
            this.bt_create_rect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_create_rect.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.bt_create_rect.FlatAppearance.BorderSize = 0;
            this.bt_create_rect.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.bt_create_rect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.bt_create_rect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.bt_create_rect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bt_create_rect.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.bt_create_rect.ForeColor = System.Drawing.Color.DodgerBlue;
            this.bt_create_rect.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.bt_create_rect.Location = new System.Drawing.Point(262, 117);
            this.bt_create_rect.Name = "bt_create_rect";
            this.bt_create_rect.Size = new System.Drawing.Size(220, 32);
            this.bt_create_rect.TabIndex = 265;
            this.bt_create_rect.TabStop = false;
            this.bt_create_rect.Text = "Teach";
            this.bt_create_rect.UseVisualStyleBackColor = true;
            this.bt_create_rect.Click += new System.EventHandler(this.Control_Click);
            // 
            // BT_MOVE_PATTERN
            // 
            this.BT_MOVE_PATTERN.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_PATTERN.BackgroundImage")));
            this.BT_MOVE_PATTERN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_PATTERN.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_PATTERN.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_PATTERN.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_PATTERN.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_PATTERN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_PATTERN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_PATTERN.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_PATTERN.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_PATTERN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_PATTERN.Location = new System.Drawing.Point(262, 155);
            this.BT_MOVE_PATTERN.Name = "BT_MOVE_PATTERN";
            this.BT_MOVE_PATTERN.Size = new System.Drawing.Size(108, 87);
            this.BT_MOVE_PATTERN.TabIndex = 266;
            this.BT_MOVE_PATTERN.TabStop = false;
            this.BT_MOVE_PATTERN.Text = "Move Pattern Postion";
            this.BT_MOVE_PATTERN.UseVisualStyleBackColor = true;
            this.BT_MOVE_PATTERN.Click += new System.EventHandler(this.Move_Pos);
            // 
            // BT_MOVE_CORNER
            // 
            this.BT_MOVE_CORNER.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("BT_MOVE_CORNER.BackgroundImage")));
            this.BT_MOVE_CORNER.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BT_MOVE_CORNER.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.BT_MOVE_CORNER.FlatAppearance.BorderSize = 0;
            this.BT_MOVE_CORNER.FlatAppearance.CheckedBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_CORNER.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Orange;
            this.BT_MOVE_CORNER.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Black;
            this.BT_MOVE_CORNER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BT_MOVE_CORNER.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.BT_MOVE_CORNER.ForeColor = System.Drawing.Color.DodgerBlue;
            this.BT_MOVE_CORNER.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BT_MOVE_CORNER.Location = new System.Drawing.Point(374, 155);
            this.BT_MOVE_CORNER.Name = "BT_MOVE_CORNER";
            this.BT_MOVE_CORNER.Size = new System.Drawing.Size(108, 87);
            this.BT_MOVE_CORNER.TabIndex = 267;
            this.BT_MOVE_CORNER.TabStop = false;
            this.BT_MOVE_CORNER.Text = "Move Corner Position";
            this.BT_MOVE_CORNER.UseVisualStyleBackColor = true;
            this.BT_MOVE_CORNER.Click += new System.EventHandler(this.Move_Pos);
            // 
            // LB_ExposureValue
            // 
            this.LB_ExposureValue.AutoSize = true;
            this.LB_ExposureValue.Location = new System.Drawing.Point(121, 154);
            this.LB_ExposureValue.Name = "LB_ExposureValue";
            this.LB_ExposureValue.Size = new System.Drawing.Size(13, 12);
            this.LB_ExposureValue.TabIndex = 276;
            this.LB_ExposureValue.Text = "0";
            // 
            // LB_Exposure
            // 
            this.LB_Exposure.Location = new System.Drawing.Point(6, 131);
            this.LB_Exposure.Name = "LB_Exposure";
            this.LB_Exposure.Size = new System.Drawing.Size(80, 35);
            this.LB_Exposure.TabIndex = 275;
            this.LB_Exposure.Text = "Exposure Time (us)";
            this.LB_Exposure.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SB_Exposure
            // 
            this.SB_Exposure.Location = new System.Drawing.Point(10, 168);
            this.SB_Exposure.Name = "SB_Exposure";
            this.SB_Exposure.Size = new System.Drawing.Size(160, 22);
            this.SB_Exposure.TabIndex = 274;
            this.SB_Exposure.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scrol);
            // 
            // LB_Channel1Value
            // 
            this.LB_Channel1Value.AutoSize = true;
            this.LB_Channel1Value.Location = new System.Drawing.Point(121, 28);
            this.LB_Channel1Value.Name = "LB_Channel1Value";
            this.LB_Channel1Value.Size = new System.Drawing.Size(13, 12);
            this.LB_Channel1Value.TabIndex = 273;
            this.LB_Channel1Value.Text = "0";
            // 
            // LB_Channel2Value
            // 
            this.LB_Channel2Value.AutoSize = true;
            this.LB_Channel2Value.Location = new System.Drawing.Point(121, 91);
            this.LB_Channel2Value.Name = "LB_Channel2Value";
            this.LB_Channel2Value.Size = new System.Drawing.Size(13, 12);
            this.LB_Channel2Value.TabIndex = 272;
            this.LB_Channel2Value.Text = "0";
            // 
            // LB_Channel2
            // 
            this.LB_Channel2.Location = new System.Drawing.Point(8, 91);
            this.LB_Channel2.Name = "LB_Channel2";
            this.LB_Channel2.Size = new System.Drawing.Size(80, 14);
            this.LB_Channel2.TabIndex = 271;
            this.LB_Channel2.Text = "Channel 2";
            // 
            // LB_Channel1
            // 
            this.LB_Channel1.Location = new System.Drawing.Point(6, 28);
            this.LB_Channel1.Name = "LB_Channel1";
            this.LB_Channel1.Size = new System.Drawing.Size(80, 14);
            this.LB_Channel1.TabIndex = 270;
            this.LB_Channel1.Text = "Channel 1";
            // 
            // SB_Channel2
            // 
            this.SB_Channel2.Location = new System.Drawing.Point(10, 105);
            this.SB_Channel2.Name = "SB_Channel2";
            this.SB_Channel2.Size = new System.Drawing.Size(160, 22);
            this.SB_Channel2.TabIndex = 269;
            this.SB_Channel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scrol);
            // 
            // SB_Channel1
            // 
            this.SB_Channel1.Location = new System.Drawing.Point(10, 42);
            this.SB_Channel1.Name = "SB_Channel1";
            this.SB_Channel1.Size = new System.Drawing.Size(160, 22);
            this.SB_Channel1.TabIndex = 268;
            this.SB_Channel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.SB_Scrol);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.LB_Channel1);
            this.groupBox1.Controls.Add(this.LB_ExposureValue);
            this.groupBox1.Controls.Add(this.SB_Channel1);
            this.groupBox1.Controls.Add(this.LB_Exposure);
            this.groupBox1.Controls.Add(this.SB_Channel2);
            this.groupBox1.Controls.Add(this.SB_Exposure);
            this.groupBox1.Controls.Add(this.LB_Channel2);
            this.groupBox1.Controls.Add(this.LB_Channel1Value);
            this.groupBox1.Controls.Add(this.LB_Channel2Value);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(491, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(182, 208);
            this.groupBox1.TabIndex = 277;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lighting Control";
            // 
            // FormHalconModelTeach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(685, 431);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.BT_MOVE_CORNER);
            this.Controls.Add(this.BT_MOVE_PATTERN);
            this.Controls.Add(this.bt_create_rect);
            this.Controls.Add(this.BT_Test);
            this.Controls.Add(this.BT_SpeedT);
            this.Controls.Add(this.BT_JogT_CCW);
            this.Controls.Add(this.BT_JogT_CW);
            this.Controls.Add(this.BT_AutoTeach);
            this.Controls.Add(this.TB_Result);
            this.Controls.Add(this.BT_JogY_Inside);
            this.Controls.Add(this.BT_JogY_Outside);
            this.Controls.Add(this.BT_SpeedXY);
            this.Controls.Add(this.BT_JogX_Left);
            this.Controls.Add(this.BT_JogX_Right);
            this.Controls.Add(this.BT_ESC);
            this.Controls.Add(this.BT_Teach);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormHalconModelTeach";
            this.Text = "Image Processing Model Teach";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormHalconModelTeach_FormClosed);
            this.Load += new System.EventHandler(this.FormHalconModelTeach_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BT_Teach;
        private System.Windows.Forms.Button BT_ESC;
        private System.Windows.Forms.Button BT_JogY_Inside;
        private System.Windows.Forms.Button BT_JogY_Outside;
        private System.Windows.Forms.Button BT_SpeedXY;
        private System.Windows.Forms.Button BT_JogX_Left;
        private System.Windows.Forms.Button BT_JogX_Right;
        private System.Windows.Forms.TextBox TB_Result;
        private System.Windows.Forms.Button BT_AutoTeach;
        private System.Windows.Forms.Button BT_SpeedT;
        private System.Windows.Forms.Button BT_JogT_CW;
        private System.Windows.Forms.Button BT_Test;
        private System.Windows.Forms.Button BT_JogT_CCW;
        private System.Windows.Forms.Button bt_create_rect;
        private System.Windows.Forms.Button BT_MOVE_PATTERN;
        private System.Windows.Forms.Button BT_MOVE_CORNER;
        private System.Windows.Forms.Label LB_ExposureValue;
        private System.Windows.Forms.Label LB_Exposure;
        private System.Windows.Forms.HScrollBar SB_Exposure;
        private System.Windows.Forms.Label LB_Channel1Value;
        private System.Windows.Forms.Label LB_Channel2Value;
        private System.Windows.Forms.Label LB_Channel2;
        private System.Windows.Forms.Label LB_Channel1;
        private System.Windows.Forms.HScrollBar SB_Channel2;
        private System.Windows.Forms.HScrollBar SB_Channel1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
