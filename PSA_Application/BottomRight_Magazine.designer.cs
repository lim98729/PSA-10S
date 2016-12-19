namespace PSA_Application
{
    partial class BottomRight_Magazine
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BottomRight_Magazine));
            this.LB_ = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.TS_Position = new System.Windows.Forms.ToolStrip();
            this.LB_Position = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_PositionSelect_MG = new System.Windows.Forms.ToolStripDropDownButton();
            this.BT_MG1_SELECT = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_MG2_SELECT = new System.Windows.Forms.ToolStripMenuItem();
            this.BT_MG3_SELECT = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_PositionSelect_Slot = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_MG_MOVE = new System.Windows.Forms.ToolStripButton();
            this.TS_IN = new System.Windows.Forms.ToolStrip();
            this.LB_IN = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_AREA_SENSOR = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_MG_RESET = new System.Windows.Forms.ToolStripLabel();
            this.TS_OUT = new System.Windows.Forms.ToolStrip();
            this.LB_OUT = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_OUT_MG_RESET = new System.Windows.Forms.ToolStripButton();
            this.LB_IN_MG_IN = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TS_Position.SuspendLayout();
            this.TS_IN.SuspendLayout();
            this.TS_OUT.SuspendLayout();
            this.SuspendLayout();
            // 
            // LB_
            // 
            this.LB_.Dock = System.Windows.Forms.DockStyle.Top;
            this.LB_.Font = new System.Drawing.Font("Arial", 8.25F);
            this.LB_.Location = new System.Drawing.Point(0, 0);
            this.LB_.Name = "LB_";
            this.LB_.Size = new System.Drawing.Size(665, 23);
            this.LB_.TabIndex = 35;
            this.LB_.Text = "MAGAZINE";
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // TS_Position
            // 
            this.TS_Position.BackColor = System.Drawing.Color.Transparent;
            this.TS_Position.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_Position.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_Position.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_Position,
            this.toolStripSeparator9,
            this.BT_PositionSelect_MG,
            this.toolStripSeparator1,
            this.BT_PositionSelect_Slot,
            this.toolStripSeparator3,
            this.BT_MG_MOVE});
            this.TS_Position.Location = new System.Drawing.Point(0, 132);
            this.TS_Position.Name = "TS_Position";
            this.TS_Position.Size = new System.Drawing.Size(440, 33);
            this.TS_Position.TabIndex = 74;
            this.TS_Position.Text = "toolStrip11";
            // 
            // LB_Position
            // 
            this.LB_Position.AutoSize = false;
            this.LB_Position.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.LB_Position.Name = "LB_Position";
            this.LB_Position.Size = new System.Drawing.Size(80, 30);
            this.LB_Position.Text = "Manual Move";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_PositionSelect_MG
            // 
            this.BT_PositionSelect_MG.AutoSize = false;
            this.BT_PositionSelect_MG.AutoToolTip = false;
            this.BT_PositionSelect_MG.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BT_MG1_SELECT,
            this.BT_MG2_SELECT,
            this.BT_MG3_SELECT});
            this.BT_PositionSelect_MG.Image = global::PSA_Application.Properties.Resources.Checked;
            this.BT_PositionSelect_MG.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_PositionSelect_MG.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_PositionSelect_MG.Name = "BT_PositionSelect_MG";
            this.BT_PositionSelect_MG.Size = new System.Drawing.Size(110, 30);
            this.BT_PositionSelect_MG.Text = "MG1";
            // 
            // BT_MG1_SELECT
            // 
            this.BT_MG1_SELECT.Name = "BT_MG1_SELECT";
            this.BT_MG1_SELECT.Size = new System.Drawing.Size(99, 22);
            this.BT_MG1_SELECT.Text = "MG1";
            this.BT_MG1_SELECT.Click += new System.EventHandler(this.MG_Click);
            // 
            // BT_MG2_SELECT
            // 
            this.BT_MG2_SELECT.Name = "BT_MG2_SELECT";
            this.BT_MG2_SELECT.Size = new System.Drawing.Size(99, 22);
            this.BT_MG2_SELECT.Text = "MG2";
            this.BT_MG2_SELECT.Click += new System.EventHandler(this.MG_Click);
            // 
            // BT_MG3_SELECT
            // 
            this.BT_MG3_SELECT.Name = "BT_MG3_SELECT";
            this.BT_MG3_SELECT.Size = new System.Drawing.Size(99, 22);
            this.BT_MG3_SELECT.Text = "MG3";
            this.BT_MG3_SELECT.Click += new System.EventHandler(this.MG_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_PositionSelect_Slot
            // 
            this.BT_PositionSelect_Slot.AutoSize = false;
            this.BT_PositionSelect_Slot.AutoToolTip = false;
            this.BT_PositionSelect_Slot.Image = global::PSA_Application.Properties.Resources.Checked;
            this.BT_PositionSelect_Slot.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_PositionSelect_Slot.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_PositionSelect_Slot.Name = "BT_PositionSelect_Slot";
            this.BT_PositionSelect_Slot.Size = new System.Drawing.Size(110, 30);
            this.BT_PositionSelect_Slot.Text = "SLOT(#1)";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_MG_MOVE
            // 
            this.BT_MG_MOVE.AutoSize = false;
            this.BT_MG_MOVE.Image = ((System.Drawing.Image)(resources.GetObject("BT_MG_MOVE.Image")));
            this.BT_MG_MOVE.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_MG_MOVE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_MG_MOVE.Name = "BT_MG_MOVE";
            this.BT_MG_MOVE.Size = new System.Drawing.Size(110, 30);
            this.BT_MG_MOVE.Text = "Move";
            this.BT_MG_MOVE.Click += new System.EventHandler(this.BT_MG_MOVE_Click);
            // 
            // TS_IN
            // 
            this.TS_IN.BackColor = System.Drawing.Color.Transparent;
            this.TS_IN.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_IN.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_IN.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_IN,
            this.toolStripSeparator5,
            this.LB_IN_AREA_SENSOR,
            this.toolStripSeparator4,
            this.LB_IN_MG_RESET,
            this.toolStripSeparator2,
            this.LB_IN_MG_IN});
            this.TS_IN.Location = new System.Drawing.Point(0, 40);
            this.TS_IN.Name = "TS_IN";
            this.TS_IN.Size = new System.Drawing.Size(441, 35);
            this.TS_IN.TabIndex = 83;
            this.TS_IN.Text = "toolStrip11";
            // 
            // LB_IN
            // 
            this.LB_IN.AutoSize = false;
            this.LB_IN.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.LB_IN.Name = "LB_IN";
            this.LB_IN.Size = new System.Drawing.Size(80, 22);
            this.LB_IN.Text = "Input";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 35);
            // 
            // LB_IN_AREA_SENSOR
            // 
            this.LB_IN_AREA_SENSOR.AutoSize = false;
            this.LB_IN_AREA_SENSOR.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_AREA_SENSOR.Image")));
            this.LB_IN_AREA_SENSOR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_AREA_SENSOR.Name = "LB_IN_AREA_SENSOR";
            this.LB_IN_AREA_SENSOR.Size = new System.Drawing.Size(100, 32);
            this.LB_IN_AREA_SENSOR.Text = "Area Sensor";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 35);
            // 
            // LB_IN_MG_RESET
            // 
            this.LB_IN_MG_RESET.AutoSize = false;
            this.LB_IN_MG_RESET.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_MG_RESET.Image")));
            this.LB_IN_MG_RESET.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_MG_RESET.Name = "LB_IN_MG_RESET";
            this.LB_IN_MG_RESET.Size = new System.Drawing.Size(100, 32);
            this.LB_IN_MG_RESET.Text = "MG Reset";
            // 
            // TS_OUT
            // 
            this.TS_OUT.BackColor = System.Drawing.Color.Transparent;
            this.TS_OUT.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_OUT.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_OUT.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_OUT,
            this.toolStripSeparator6,
            this.BT_OUT_MG_RESET});
            this.TS_OUT.Location = new System.Drawing.Point(3, 87);
            this.TS_OUT.Name = "TS_OUT";
            this.TS_OUT.Size = new System.Drawing.Size(208, 33);
            this.TS_OUT.TabIndex = 84;
            this.TS_OUT.Text = "toolStrip11";
            this.TS_OUT.Visible = false;
            // 
            // LB_OUT
            // 
            this.LB_OUT.AutoSize = false;
            this.LB_OUT.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.LB_OUT.Name = "LB_OUT";
            this.LB_OUT.Size = new System.Drawing.Size(80, 30);
            this.LB_OUT.Text = "MG Reset";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_OUT_MG_RESET
            // 
            this.BT_OUT_MG_RESET.AutoSize = false;
            this.BT_OUT_MG_RESET.Image = ((System.Drawing.Image)(resources.GetObject("BT_OUT_MG_RESET.Image")));
            this.BT_OUT_MG_RESET.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_OUT_MG_RESET.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_OUT_MG_RESET.Name = "BT_OUT_MG_RESET";
            this.BT_OUT_MG_RESET.Size = new System.Drawing.Size(110, 30);
            this.BT_OUT_MG_RESET.Text = "MG Reset";
            this.BT_OUT_MG_RESET.Click += new System.EventHandler(this.SLOT_Click);
            // 
            // LB_IN_MG_IN
            // 
            this.LB_IN_MG_IN.AutoSize = false;
            this.LB_IN_MG_IN.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_MG_IN.Image")));
            this.LB_IN_MG_IN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_MG_IN.Name = "LB_IN_MG_IN";
            this.LB_IN_MG_IN.Size = new System.Drawing.Size(100, 32);
            this.LB_IN_MG_IN.Text = "Magazine In";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // BottomRight_Magazine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.TS_OUT);
            this.Controls.Add(this.TS_IN);
            this.Controls.Add(this.TS_Position);
            this.Controls.Add(this.LB_);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "BottomRight_Magazine";
            this.Size = new System.Drawing.Size(665, 293);
            this.Load += new System.EventHandler(this.BottomRight_Magazine_Load);
            this.TS_Position.ResumeLayout(false);
            this.TS_Position.PerformLayout();
            this.TS_IN.ResumeLayout(false);
            this.TS_IN.PerformLayout();
            this.TS_OUT.ResumeLayout(false);
            this.TS_OUT.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LB_;
        public System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStrip TS_Position;
        private System.Windows.Forms.ToolStripLabel LB_Position;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton BT_MG_MOVE;
        private System.Windows.Forms.ToolStripDropDownButton BT_PositionSelect_MG;
        private System.Windows.Forms.ToolStripMenuItem BT_MG1_SELECT;
        private System.Windows.Forms.ToolStripMenuItem BT_MG2_SELECT;
        private System.Windows.Forms.ToolStripMenuItem BT_MG3_SELECT;
        private System.Windows.Forms.ToolStripDropDownButton BT_PositionSelect_Slot;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStrip TS_IN;
		private System.Windows.Forms.ToolStripLabel LB_IN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripLabel LB_IN_MG_RESET;
		private System.Windows.Forms.ToolStrip TS_OUT;
		private System.Windows.Forms.ToolStripLabel LB_OUT;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton BT_OUT_MG_RESET;
		private System.Windows.Forms.ToolStripLabel LB_IN_AREA_SENSOR;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel LB_IN_MG_IN;
    }
}
