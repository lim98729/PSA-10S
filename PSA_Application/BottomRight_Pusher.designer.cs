namespace PSA_Application
{
    partial class BottomRight_Pusher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BottomRight_Pusher));
            this.LB_ = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.TS_Position = new System.Windows.Forms.ToolStrip();
            this.LB_Position = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Position_MoveToUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_Position_MoveToDown = new System.Windows.Forms.ToolStripButton();
            this.TS_IN = new System.Windows.Forms.ToolStrip();
            this.LB_IN = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_JAM = new System.Windows.Forms.ToolStripLabel();
            this.TS_OUT = new System.Windows.Forms.ToolStrip();
            this.LB_OUT2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.BT_OUT_PS_UPDOWN = new System.Windows.Forms.ToolStripButton();
            this.LB_IN_READY = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_END = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_DOWN = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.LB_IN_UP = new System.Windows.Forms.ToolStripLabel();
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
            this.LB_.Text = "PUSHER";
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
            this.toolStripSeparator3,
            this.BT_Position_MoveToUp,
            this.toolStripSeparator8,
            this.BT_Position_MoveToDown});
            this.TS_Position.Location = new System.Drawing.Point(3, 137);
            this.TS_Position.Name = "TS_Position";
            this.TS_Position.Size = new System.Drawing.Size(324, 33);
            this.TS_Position.TabIndex = 80;
            this.TS_Position.Text = "toolStrip11";
            // 
            // LB_Position
            // 
            this.LB_Position.AutoSize = false;
            this.LB_Position.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LB_Position.Name = "LB_Position";
            this.LB_Position.Size = new System.Drawing.Size(80, 30);
            this.LB_Position.Text = "PUSHER";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_Position_MoveToUp
            // 
            this.BT_Position_MoveToUp.AutoSize = false;
            this.BT_Position_MoveToUp.Image = ((System.Drawing.Image)(resources.GetObject("BT_Position_MoveToUp.Image")));
            this.BT_Position_MoveToUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Position_MoveToUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_Position_MoveToUp.Name = "BT_Position_MoveToUp";
            this.BT_Position_MoveToUp.Size = new System.Drawing.Size(110, 30);
            this.BT_Position_MoveToUp.Text = "Move to ready";
            this.BT_Position_MoveToUp.Click += new System.EventHandler(this.PUSHER_READY_MOVE);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 33);
            // 
            // BT_Position_MoveToDown
            // 
            this.BT_Position_MoveToDown.AutoSize = false;
            this.BT_Position_MoveToDown.Image = ((System.Drawing.Image)(resources.GetObject("BT_Position_MoveToDown.Image")));
            this.BT_Position_MoveToDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BT_Position_MoveToDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BT_Position_MoveToDown.Name = "BT_Position_MoveToDown";
            this.BT_Position_MoveToDown.Size = new System.Drawing.Size(110, 30);
            this.BT_Position_MoveToDown.Text = "Move to push";
            this.BT_Position_MoveToDown.Click += new System.EventHandler(this.PUSHER_PSUH_MOVE);
            // 
            // TS_IN
            // 
            this.TS_IN.BackColor = System.Drawing.Color.Transparent;
            this.TS_IN.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_IN.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_IN.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_IN,
            this.toolStripSeparator6,
            this.LB_IN_UP,
            this.toolStripSeparator4,
            this.LB_IN_DOWN,
            this.toolStripSeparator5,
            this.LB_IN_JAM,
            this.toolStripSeparator2,
            this.LB_IN_READY,
            this.toolStripSeparator1,
            this.LB_IN_END});
            this.TS_IN.Location = new System.Drawing.Point(3, 57);
            this.TS_IN.Name = "TS_IN";
            this.TS_IN.Size = new System.Drawing.Size(603, 35);
            this.TS_IN.TabIndex = 82;
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
            // LB_IN_JAM
            // 
            this.LB_IN_JAM.AutoSize = false;
            this.LB_IN_JAM.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_JAM.Image")));
            this.LB_IN_JAM.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_JAM.Name = "LB_IN_JAM";
            this.LB_IN_JAM.Size = new System.Drawing.Size(90, 32);
            this.LB_IN_JAM.Text = "JAM";
            // 
            // TS_OUT
            // 
            this.TS_OUT.BackColor = System.Drawing.Color.Transparent;
            this.TS_OUT.Dock = System.Windows.Forms.DockStyle.None;
            this.TS_OUT.Font = new System.Drawing.Font("Arial", 9F);
            this.TS_OUT.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LB_OUT2,
            this.toolStripSeparator16,
            this.BT_OUT_PS_UPDOWN});
            this.TS_OUT.Location = new System.Drawing.Point(3, 97);
            this.TS_OUT.Name = "TS_OUT";
            this.TS_OUT.Size = new System.Drawing.Size(202, 35);
            this.TS_OUT.TabIndex = 83;
            this.TS_OUT.Text = "toolStrip11";
            // 
            // LB_OUT2
            // 
            this.LB_OUT2.AutoSize = false;
            this.LB_OUT2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.LB_OUT2.Name = "LB_OUT2";
            this.LB_OUT2.Size = new System.Drawing.Size(80, 22);
            this.LB_OUT2.Text = "Output";
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.AutoSize = false;
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 35);
            // 
            // BT_OUT_PS_UPDOWN
            // 
            this.BT_OUT_PS_UPDOWN.AutoToolTip = false;
            this.BT_OUT_PS_UPDOWN.Image = global::PSA_Application.Properties.Resources.gray_ball1;
            this.BT_OUT_PS_UPDOWN.Name = "BT_OUT_PS_UPDOWN";
            this.BT_OUT_PS_UPDOWN.Size = new System.Drawing.Size(104, 32);
            this.BT_OUT_PS_UPDOWN.Text = "PS UP/DOWN";
            this.BT_OUT_PS_UPDOWN.Click += new System.EventHandler(this.Out_Click);
            // 
            // LB_IN_READY
            // 
            this.LB_IN_READY.AutoSize = false;
            this.LB_IN_READY.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_READY.Image")));
            this.LB_IN_READY.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_READY.Name = "LB_IN_READY";
            this.LB_IN_READY.Size = new System.Drawing.Size(90, 32);
            this.LB_IN_READY.Text = "READY";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // LB_IN_END
            // 
            this.LB_IN_END.AutoSize = false;
            this.LB_IN_END.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_END.Image")));
            this.LB_IN_END.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_END.Name = "LB_IN_END";
            this.LB_IN_END.Size = new System.Drawing.Size(90, 32);
            this.LB_IN_END.Text = "END";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 35);
            // 
            // LB_IN_DOWN
            // 
            this.LB_IN_DOWN.AutoSize = false;
            this.LB_IN_DOWN.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_DOWN.Image")));
            this.LB_IN_DOWN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_DOWN.Name = "LB_IN_DOWN";
            this.LB_IN_DOWN.Size = new System.Drawing.Size(90, 32);
            this.LB_IN_DOWN.Text = "PS DOWN";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 35);
            // 
            // LB_IN_UP
            // 
            this.LB_IN_UP.AutoSize = false;
            this.LB_IN_UP.Image = ((System.Drawing.Image)(resources.GetObject("LB_IN_UP.Image")));
            this.LB_IN_UP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LB_IN_UP.Name = "LB_IN_UP";
            this.LB_IN_UP.Size = new System.Drawing.Size(90, 32);
            this.LB_IN_UP.Text = "PS UP";
            // 
            // BottomRight_Pusher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.TS_OUT);
            this.Controls.Add(this.TS_IN);
            this.Controls.Add(this.TS_Position);
            this.Controls.Add(this.LB_);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "BottomRight_Pusher";
            this.Size = new System.Drawing.Size(665, 293);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton BT_Position_MoveToUp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripButton BT_Position_MoveToDown;
        private System.Windows.Forms.ToolStrip TS_IN;
        private System.Windows.Forms.ToolStripLabel LB_IN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel LB_IN_JAM;
        private System.Windows.Forms.ToolStrip TS_OUT;
        private System.Windows.Forms.ToolStripLabel LB_OUT2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripButton BT_OUT_PS_UPDOWN;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel LB_IN_READY;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel LB_IN_END;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel LB_IN_UP;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel LB_IN_DOWN;
    }
}
