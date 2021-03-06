﻿using System;
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
	public partial class CenterRight_Initial : UserControl
	{
		public CenterRight_Initial()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
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
		void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
                if (mc.para.ETC.unloaderControl.value == 1)
                {
                    BT_PUSHER.Visible = true;
                    BT_MAGAZINE.Visible = true;
                    BT_StandBy.Location = new System.Drawing.Point(206, 505);
                    BT_All.Location = new System.Drawing.Point(345, 392);
                    BT_Vision.Location = new System.Drawing.Point(74, 392);
                }
                else
                {
                    BT_PUSHER.Visible = false;
                    BT_MAGAZINE.Visible = false;
                    BT_StandBy.Location = new System.Drawing.Point(206, 392);
                    BT_All.Location = new System.Drawing.Point(345, 285);
                    BT_Vision.Location = new System.Drawing.Point(74, 285);
                }

				if (mc.hd.reqMode == REQMODE.HOMING) BT_HD.Image = Properties.Resources.Refresh;
				else if (mc.init.success.HD) BT_HD.Image = Properties.Resources.Complete;
				else BT_HD.Image = Properties.Resources.Fail;

				if (mc.pd.reqMode == REQMODE.HOMING) BT_PD.Image = Properties.Resources.Refresh;
				else if (mc.init.success.PD) BT_PD.Image = Properties.Resources.Complete;
				else BT_PD.Image = Properties.Resources.Fail;

				if (mc.sf.reqMode == REQMODE.HOMING) BT_SF.Image = Properties.Resources.Refresh;
				else if (mc.init.success.SF) BT_SF.Image = Properties.Resources.Complete;
				else BT_SF.Image = Properties.Resources.Fail;

				if (mc.cv.reqMode == REQMODE.HOMING) BT_CV.Image = Properties.Resources.Refresh;
				else if (mc.init.success.CV) BT_CV.Image = Properties.Resources.Complete;
				else BT_CV.Image = Properties.Resources.Fail;

				if (mc.hdc.reqMode == REQMODE.HOMING || mc.ulc.reqMode == REQMODE.HOMING) BT_Vision.Image = Properties.Resources.Refresh;
				else if (mc.init.success.HDC && mc.init.success.ULC) BT_Vision.Image = Properties.Resources.Complete;
				else BT_Vision.Image = Properties.Resources.Fail;

                if (mc.ps.reqMode == REQMODE.HOMING) BT_PUSHER.Image = Properties.Resources.Refresh;
                else if (mc.init.success.PS) BT_PUSHER.Image = Properties.Resources.Complete;
                else BT_PUSHER.Image = Properties.Resources.Fail;

                if (mc.unloader.reqMode == REQMODE.HOMING) BT_MAGAZINE.Image = Properties.Resources.Refresh;
                else if (mc.init.success.MG) BT_MAGAZINE.Image = Properties.Resources.Complete;
                else BT_MAGAZINE.Image = Properties.Resources.Fail;

				if (mc.init.RUNING) BT_All.Image = Properties.Resources.Refresh;
				else if (mc.init.success.ALL) BT_All.Image = Properties.Resources.Complete;
				else BT_All.Image = Properties.Resources.Fail;

				if (mc.hd.reqMode == REQMODE.HOMING) BT_StandBy.Image = Properties.Resources.Refresh;
				else if (mc.init.success.HD) BT_StandBy.Image = Properties.Resources.Complete;
				else BT_StandBy.Image = Properties.Resources.Fail;

				//LB_.Focus();
				//LB_.BringToFront();
			}
		}
		#endregion

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_INITIAL(sender)) return;
			mc.check.push(sender, true);
			timer.Enabled = true;
			//EVENT.mainFormPanelMode(SPLITTER_MODE.EXPAND, SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT);
			EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT);
			#region Initial
			if (sender.Equals(BT_Vision))
			{
                //Derek 수정예정
                // jhlim : 임시
                mc.hdc.req = true; mc.hdc.reqMode = REQMODE.HOMING;
                mc.ulc.req = true; mc.ulc.reqMode = REQMODE.HOMING;
                //mc.init.success.HDC = true;
                //mc.init.success.ULC = true;
			}
			if (sender.Equals(BT_HD))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.HOMING;
			}
			if (sender.Equals(BT_PD))
			{
                //Derek 수정예정
				mc.pd.req = true; mc.pd.reqMode = REQMODE.HOMING;
                //mc.init.success.PD = true;
			}
			if (sender.Equals(BT_SF))
			{
                //Derek 수정예정
				mc.sf.req = true; mc.sf.reqMode = REQMODE.HOMING;
                //mc.init.success.SF = true;
			}
			if (sender.Equals(BT_CV))
			{
                //Derek 수정예정
                // conveyor상에 board가 있는지 확인
                bool[] bdstate = new bool[4];
                RetValue retval;
                mc.IN.CV.BD_IN(out bdstate[0], out retval.message);
                mc.IN.CV.BD_BUF(out bdstate[1], out retval.message);
                mc.IN.CV.BD_NEAR(out bdstate[2], out retval.message);
                mc.IN.CV.BD_OUT(out bdstate[3], out retval.message);
                if (bdstate[0] == true || bdstate[1] == true || bdstate[2] == true || bdstate[3] == true)
                {
                    string bdmsg = bdstate[0] ? textResource.CV_INPUT_AREA : "";
                    bdmsg += bdstate[1] ? textResource.CV_INPUT_BUFFER : "";
                    bdmsg += bdstate[2] ? textResource.CV_WORK_AREA : "";
                    bdmsg += bdstate[3] ? textResource.CV_OUTPUT_AREA : "";
                    if (mc.para.CV.homingSkip.value == 0)
                    {
                        MessageBox.Show(String.Format(textResource.MB_CV_CANNOT_INITIALIZE, bdmsg));
                        goto INIT_EXIT;
                    }
                }
                mc.cv.req = true; mc.cv.reqMode = REQMODE.HOMING;
                //mc.init.success.CV = true;
			}
            if (sender.Equals(BT_PUSHER))
            {
                mc.ps.req = true; mc.ps.reqMode = REQMODE.HOMING;
            }

            if (sender.Equals(BT_MAGAZINE))
            {
                mc.unloader.req = true; mc.unloader.reqMode = REQMODE.HOMING;
            }
			if (sender.Equals(BT_All))
			{
				mc.init.req = true;
			}
            if (sender.Equals(BT_StandBy))
            {
                mc.swcontrol.readconfig();			// Swcontrol 를 읽는 부분이 초기화 부분밖에 없어서 추가함. 사실 이 폼에는 필요없는 값들이지만, INI Update 하기 위한 공용 폼으로 사용함.
                UtilityControl.readGraphConfig();
                UtilityControl.readForceConfig();
                RetValue retval;
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    mc.hd.tool.jogMove(i, mc.hd.tool.tPos.z[i].XY_MOVING, out retval.message); if (retval.message != RetMessage.OK) { mc.message.alarmMotion(retval.message); goto INIT_EXIT; }
                }
                double posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                double posT2 = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD2].ZERO;
                mc.hd.tool.jogMove(mc.para.CAL.standbyPosition.x.value, mc.para.CAL.standbyPosition.y.value, posT, posT2, out retval.message); if (retval.message != RetMessage.OK) { mc.message.alarmMotion(retval.message); goto INIT_EXIT; }
                mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.OFF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.OFF]);		// 동작이 끝난 후 조명을 끈다.
                mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.OFF], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.OFF]);
            }

			#endregion
			RetValue ret;
			mc.OUT.MAIN.IONIZER(true, out ret.message); // 초기화할때도 Ionizer는 ON
			mc.main.Thread_Polling();
		INIT_EXIT:
			EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.CURRENT, SPLITTER_MODE.CURRENT);
			timer.Enabled = false;
			mc.check.push(sender, false);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}

        private void CenterRight_Initial_Load(object sender, EventArgs e)
        {
            if (mc.para.ETC.unloaderControl.value == 1)
            {
                BT_PUSHER.Visible = true;
                BT_MAGAZINE.Visible = true;
                BT_StandBy.Location = new System.Drawing.Point(206, 505);
                BT_All.Location = new System.Drawing.Point(345, 392);
                BT_Vision.Location = new System.Drawing.Point(74, 392);
            }
            else
            {
                BT_PUSHER.Visible = false;
                BT_MAGAZINE.Visible = false;
                BT_StandBy.Location = new System.Drawing.Point(206, 392);
                BT_All.Location = new System.Drawing.Point(345, 285);
                BT_Vision.Location = new System.Drawing.Point(74, 285);
            }
        }
	}
}
