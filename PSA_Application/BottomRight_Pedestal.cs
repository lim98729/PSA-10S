using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;

namespace PSA_Application
{
	public partial class BottomRight_Pedestal : UserControl
	{
		public BottomRight_Pedestal()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			#endregion
		}
		#region EVENT용 delegate 함수
		delegate void mainFormPanelMode_Call(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom);
		void mainFormPanelMode(SPLITTER_MODE up, SPLITTER_MODE center, SPLITTER_MODE bottom)
		{
			if (this.InvokeRequired)
			{
				mainFormPanelMode_Call d = new mainFormPanelMode_Call(mainFormPanelMode);
				this.BeginInvoke(d, new object[] { up, center, bottom });
			}
			else
			{
				refresh();
			}
		}
		delegate void refresh_Call();
		Image image;
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				#region IN
				mc.IN.PD.VAC_CHK(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_VAC.Image = image;

                mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_UP_SENSOR.Image = image;

                mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_DOWN_SENSOR.Image = image;
 				#endregion

				#region OUT
                mc.OUT.PD.SUC(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_SUCTION.Image = image;

                mc.OUT.PD.BLW(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_BLOW.Image = image;

                mc.OUT.PD.UP(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_UP.Image = image;

                mc.OUT.PD.DOWN(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_DOWN.Image = image;
				#endregion

				padCountCheck();
			}
		}
		#endregion
		RetValue ret;
		private void BT_OUT_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;

			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_SUCTION))
			{
                mc.OUT.PD.SUC(out ret.b, out ret.message);
                if (ret.message == RetMessage.OK)  mc.OUT.PD.SUC(!ret.b, out ret.message);
      		}
			if (sender.Equals(BT_OUT_BLOW))
			{
                mc.OUT.PD.BLW(out ret.b, out ret.message);
                if (ret.message == RetMessage.OK)  mc.OUT.PD.BLW(!ret.b, out ret.message);
			}

            if (sender.Equals(BT_OUT_UP))
            {
                mc.OUT.PD.UP(out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.PD.UP(!ret.b, out ret.message);
            }

            if (sender.Equals(BT_OUT_DOWN))
            {
                mc.OUT.PD.DOWN(out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.PD.DOWN(!ret.b, out ret.message);
            }

			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}


		int padIndexX;
		int padIndexY;
		int padCountX;
		int padCountY;
		double posX, posY, posZ;
		void padCountCheck()
		{
			if (padCountX == (int)mc.para.MT.padCount.x.value && padCountY == (int)mc.para.MT.padCount.y.value) return;
			padCountX = (int)mc.para.MT.padCount.x.value;
			padCountY = (int)mc.para.MT.padCount.y.value;
			CbB_PadIX.Items.Clear();
			CbB_PadIY.Items.Clear();
			for (int i = 0; i < padCountX; i++)
			{
				CbB_PadIX.Items.Add(i + 1);
			}
			for (int i = 0; i < padCountY; i++)
			{
				CbB_PadIY.Items.Add(i + 1);
			}
			CbB_PadIX.SelectedIndex = 0;
			CbB_PadIY.SelectedIndex = 0;
		}

		private void BT_Position_MoveToUp_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region MoveToUp
			padIndexX = CbB_PadIX.SelectedIndex;
			padIndexY = CbB_PadIY.SelectedIndex;
			mc.pd.PD_Mode(PedestalMode.PD_UP, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_MoveToDown_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
            mc.OUT.PD.SUC(false, out ret.message);
            mc.pd.PD_Mode(PedestalMode.PD_DOWN, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); }
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}

        private void BottomRight_Pedestal_Load(object sender, EventArgs e)
        {
            LB_IN_UP_SENSOR.Visible = true;
            LB_IN_DOWN_SENSOR.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
//            mc.hdc.cam.grab(out ret.message, out ret.s);
  //          mc.log.debug.write(mc.log.CODE.INFO, "Grab Time : " + mc.hdc.cam.grabTime);
            double a = mc.loadCell.getData(1);
            mc.log.debug.write(mc.log.CODE.FORCE, a.ToString());
            //mc.UnloaderControl.MG_Status[(int)MG_NUM.MG3, (int)SLOT_NUM.SLOT7] = (int)MG_STATUS.SKIP;
            //EVENT.refreshEditMagazine((int)MG_NUM.MG3, (int)SLOT_NUM.SLOT7);
            //mc.Magazinecontrol.MG_MAP_DATA;
            //mc.hd.tool.triggerHDC.output(false, out ret.message);
            //mc.hd.tool.triggerULC.output(false, out ret.message);
            //double forceHeight = mc.hd.tool.F.kilogram2Height(3, out ret.message);
            //double force = mc.hd.tool.F.TopLDToBottomLD(0, 5.5, out ret.message);
           // mc.hd.tool.jogMoveXY(mc.hd.tool.tPos.x[1].PICK(0), mc.hd.tool.tPos.y[1].PICK(0), out ret.message);
            //double T2TX = mc.hd.tool.tPos.x[1].ULC - mc.hd.tool.tPos.x[0].ULC;
            //double T2TY = mc.hd.tool.tPos.y[1].ULC - mc.hd.tool.tPos.y[0].ULC;
            //double a = mc.hd.tool.tPos.y[0].ULC;
            //double b = mc.hd.tool.tPos.y[1].ULC;
            //mc.idle(300);
            //mc.hd.tool.triggerULC.output(true, out ret.message);
            //mc.ulc.cam.grab(out ret.message, out ret.s);
            //mc.hd.tool.triggerULC.output(false, out ret.message);
            //mc.idle(300);
            //mc.ulc.cam.writeGrabImage("aaa");
            //a = a - b;
            //mc.hd.tool.jogMoveXY(mc.hd.tool.tPos.x[1].PICK(0), mc.hd.tool.tPos.y[1].PICK(0), out ret.message);
            //mc.hd.tool.Z[0].move(mc.hd.tool.tPos.z[0].XY_MOVING, mc.speed.slow, out ret.message);
            //while(false)
            //{
            //    mc.idle(1);
            //    mc.OUT.CV.FD_MTR1(true, out ret.message);
            //    mc.idle(1);
            //    mc.OUT.CV.FD_MTR1(false, out ret.message);
            //}
            //mc.ulc.SettriggerMode();
            //mc.ulc.reqMode = REQMODE.GRAB;
            //mc.ulc.req = true;
            //mc.main.Thread_Polling();
            //mc.pd.jogMode = (int)PD_JOGMODE.DOWN_MODE;
            //mc.pd.jogMovePlus(0,10000, out ret.message);
        }
	}
}
