using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
namespace PSA_Application
{
	public partial class PedestalFlatness : Form
	{
		public PedestalFlatness(int a, int b)
		{
			InitializeComponent();
			x = a;
			y = b;
		}

		int x, y;
		double posX, posY;
		RetValue ret;
		double[] height = new double[5];        //          1
                                                //      2   0   3       0 : 기준점
                                                //          4
        double[] tmp = new double[5];

        double distanceX, distanceY;

		delegate void refresh_Call();
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				for (int i = 0; i < 5; i++) tmp[i] = Math.Round(height[i], 3);

				TB_LT.Text = tmp[1].ToString();
				TB_LB.Text = tmp[2].ToString();
				//TB_CENTER.Text = tmp[0].ToString();
				TB_RT.Text = tmp[3].ToString();
				TB_RB.Text = tmp[4].ToString();

				TB_JIGSIZE_X.Text = mc.para.CAL.JigSize.x.value.ToString();
				TB_JIGSIZE_Y.Text = mc.para.CAL.JigSize.y.value.ToString();
				TB_JIG_OFFSET.Text = mc.para.CAL.JigOffset.value.ToString();

				BT_ESC.Focus();
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
            distanceX = mc.para.CAL.JigSize.x.value / 2 - mc.para.CAL.JigOffset.value;
            distanceY = mc.para.CAL.JigSize.y.value / 2 - mc.para.CAL.JigOffset.value;

            int mode = 0;
            if (sender.Equals(BT_LT)) { mode = 3; }
            if (sender.Equals(BT_LB)) { mode = 2; }
            //if (sender.Equals(BT_CENTER)) { distanceX = 0; distanceY = 0; }
            if (sender.Equals(BT_RT)) { mode = 0; }
            if (sender.Equals(BT_RB)) { mode = 1; }

            int padX, padY;

            #region xyz moving
            if (mode == 0)       // RT
            {
                padX = Convert.ToInt32(mc.para.MT.padCount.x.value - 1);
                padY = Convert.ToInt32(mc.para.MT.padCount.y.value - 1);
                posX = mc.hd.tool.lPos.x.PAD(padX) + distanceX;
                posY = mc.hd.tool.lPos.y.PAD(padY) + distanceY;
            }
            else if (mode == 1)     // RB
            {
                padX = Convert.ToInt32(mc.para.MT.padCount.x.value) - 1;
                padY = 0;
                posX = mc.hd.tool.lPos.x.PAD(padX) + distanceX;
                posY = mc.hd.tool.lPos.y.PAD(padY) - distanceY;
            }
            else if (mode == 2)     // LB
            {
                padX = 0;
                padY = 0;
                posX = mc.hd.tool.lPos.x.PAD(padX) - distanceX;
                posY = mc.hd.tool.lPos.y.PAD(padY) - distanceY;
            }
            else
            {
                padX = 0;
                padY = Convert.ToInt32(mc.para.MT.padCount.y.value - 1);
                posX = mc.hd.tool.lPos.x.PAD(padX) - distanceX;
                posY = mc.hd.tool.lPos.y.PAD(padY) + distanceX;
            }
            
            mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            #endregion
            mc.idle(300);

			ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;

            if (sender.Equals(BT_LT)) { height[1] = ret.d; }
            if (sender.Equals(BT_LB)) { height[2] = ret.d; }
            //if (sender.Equals(BT_CENTER)) { height[0] = ret.d; }
            if (sender.Equals(BT_RT)) { height[3] = ret.d; }
            if (sender.Equals(BT_RB)) { height[4] = ret.d; }

			refresh();
		}

		private void BT_AutoCalibration_Click(object sender, EventArgs e)
		{
            distanceX = mc.para.CAL.JigSize.x.value / 2 - mc.para.CAL.JigOffset.value;
            distanceY = mc.para.CAL.JigSize.y.value / 2 - mc.para.CAL.JigOffset.value;

            mc.hd.tool.jogMove(0, posX, posY, mc.hd.tool.tPos.z[0].XY_MOVING, mc.hd.tool.tPos.t[0].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
			mc.idle(300);
			ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
			height[0] = ret.d;

            //if (sender.Equals(BT_MEASURE_AUTO))
            //{
            //    GetVerticalHeight();
            //    GetHorizentalHeight();
            //}
			
            refresh();
		}

		private void BT_ESC_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void PedestalFlatness_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

            posX = mc.hd.tool.lPos.x.PAD(x);
            posY = mc.hd.tool.lPos.y.PAD(y);

			refresh();
		}

        private void JigSize_Click(object sender, EventArgs e)
        {
            if(sender.Equals(TB_JIGSIZE_X)) mc.para.setting( mc.para.CAL.JigSize.x, out  mc.para.CAL.JigSize.x);
            else if (sender.Equals(TB_JIGSIZE_Y)) mc.para.setting(mc.para.CAL.JigSize.y, out  mc.para.CAL.JigSize.y);
            else if (sender.Equals(TB_JIG_OFFSET)) mc.para.setting(mc.para.CAL.JigOffset, out  mc.para.CAL.JigOffset);

			refresh();
		}

        public void GetVerticalHeight()
        {
            mc.hd.tool.jogMove(0, posX, posY + distanceY, mc.hd.tool.tPos.z[0].XY_MOVING, mc.hd.tool.tPos.t[0].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            mc.idle(300);
            ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
			height[1] = ret.d - height[0];

            mc.hd.tool.jogMove(0, posX, posY - distanceY, mc.hd.tool.tPos.z[0].XY_MOVING, mc.hd.tool.tPos.t[0].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            mc.idle(300);
            ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
			height[4] = ret.d - height[0];

        }

        public void GetHorizentalHeight()
        {
            mc.hd.tool.jogMove(0, posX - distanceX, posY, mc.hd.tool.tPos.z[0].XY_MOVING, mc.hd.tool.tPos.t[0].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            mc.idle(300);
			ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
			height[2] = ret.d - height[0];

            mc.hd.tool.jogMove(0, posX + distanceX, posY, mc.hd.tool.tPos.z[0].XY_MOVING, mc.hd.tool.tPos.t[0].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); return; }
            mc.idle(300);
			ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
			height[3] = ret.d - height[0];
        }
	}
}
