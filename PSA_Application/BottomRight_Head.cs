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
	public partial class BottomRight_Head : UserControl
	{
		StringBuilder analogSb = new StringBuilder(100);
		public BottomRight_Head()
		{
			InitializeComponent();
			#region EVENT 등록
			EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
			#endregion
            addHeadSelectBox();
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
                mc.para.HD.headNumber.id = CbB_Head_Select.SelectedIndex;

				#region IN
                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_VAC1.Image = image;

                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_VAC2.Image = image;
				#endregion

				#region OUT
                mc.OUT.HD.SUC((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_SUCTION1.Image = image;

                mc.OUT.HD.BLW((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_BLOW1.Image = image;

                mc.OUT.HD.SUC((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_SUCTION2.Image = image;

                mc.OUT.HD.BLW((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_BLOW2.Image = image;			
				#endregion

                ret.d = mc.AIN.Laser(); if (Math.Abs(ret.d) >= 10) ret.d = -1;
                analogSb.Clear(); analogSb.Length = 0;
                analogSb.Append("LASER");
                if(ret.d != -1)
                {
                    image = Properties.Resources.Red_LED;
                    analogSb.AppendFormat(" : {0:f2} mm", ret.d);
                }
                else image = Properties.Resources.Red_LED_OFF;
                LB_AIN_LASER.Image = image;
                LB_AIN_LASER.Text = analogSb.ToString();


                ret.d = mc.AIN.HeadVac1(); if (Math.Abs(ret.d) >= 110) ret.d = -1;
                analogSb.Clear(); analogSb.Length = 0;
                analogSb.Append("VAC #1");
                if (ret.d != -1)
                {
                    image = Properties.Resources.Red_LED;
                    analogSb.AppendFormat(" : {0:f1} kPa", ret.d);
//                    analogSb.AppendFormat(" : {0:f2} v", ret.d);
                }
                else image = Properties.Resources.Red_LED_OFF;
                LB_AIN_VAC1.Image = image;
                analogSb.AppendFormat(" [{0:f1}]", mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD1].value.ToString());
                LB_AIN_VAC1.Text = analogSb.ToString();

                ret.d = mc.AIN.HeadVac2(); if (Math.Abs(ret.d) >= 110) ret.d = -1;
                analogSb.Clear(); analogSb.Length = 0;
                analogSb.Append("VAC #2");
                if (ret.d != -1)
                {
                    image = Properties.Resources.Red_LED;
                    analogSb.AppendFormat(" : {0:f1} kPa", ret.d);
                    //                    analogSb.AppendFormat(" : {0:f2} v", ret.d);
                }
                else image = Properties.Resources.Red_LED_OFF;
                LB_AIN_VAC2.Image = image;
                analogSb.AppendFormat(" [{0:f1}]", mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD2].value.ToString());
                LB_AIN_VAC2.Text = analogSb.ToString();

				padCountCheck();

				if (BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text || BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text)
				{
					if (BT_Position_CameraMove.Visible) BT_Position_CameraMove.Visible = false;
				}
				else
				{
					if (!BT_Position_CameraMove.Visible) BT_Position_CameraMove.Visible = true;
				}
			}
		}
		#endregion
		RetValue ret;
		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			refresh();
			timer.Enabled = true;
		}
		private void BT_OUT_Click(object sender, EventArgs e)
		{
			//if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region OUT
			if (sender.Equals(BT_OUT_SUCTION1))
			{
                mc.OUT.HD.SUC((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.HD.SUC((int)UnitCodeHead.HD1, !ret.b, out ret.message);
			}
            if (sender.Equals(BT_OUT_SUCTION2))
            {
                mc.OUT.HD.SUC((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.HD.SUC((int)UnitCodeHead.HD2, !ret.b, out ret.message);
            }
			if (sender.Equals(BT_OUT_BLOW1))
			{
                mc.OUT.HD.BLW((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.HD.BLW((int)UnitCodeHead.HD1, !ret.b, out ret.message);
			}
            if (sender.Equals(BT_OUT_BLOW2))
            {
                mc.OUT.HD.BLW((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.HD.BLW((int)UnitCodeHead.HD2, !ret.b, out ret.message);
            }

			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
	   

		private void Manual_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);

            if(mc.init.success.ALL)
            {
                mc.hd.clear(); 
                mc.hd.tool.clear();
                mc.cv.clear();
                mc.pd.clear();
                mc.sf.clear();
                mc.hdc.clear();
                mc.ulc.clear();
            }

			#region Manual
			if (sender.Equals(BT_Manual_StepCycle))
			{
				mc.hd.req = true; mc.hd.reqMode = REQMODE.STEP;
			}
			if (sender.Equals(BT_Manual_PickupCycle))
			{
                mc.hd.tool.singleCycleHead = CbB_Head_Select.SelectedIndex;
				mc.hd.req = true; mc.hd.reqMode = REQMODE.PICKUP;
			}
			if (sender.Equals(BT_Manual_WasteCycle))
			{
                mc.hd.tool.singleCycleHead = CbB_Head_Select.SelectedIndex;
				mc.hd.req = true; mc.hd.reqMode = REQMODE.WASTE;
			}
			if (sender.Equals(BT_Manual_SingleCycle))
			{
                mc.hd.tool.singleCycleHead = CbB_Head_Select.SelectedIndex;
				mc.hd.req = true; mc.hd.reqMode = REQMODE.SINGLE;
			}
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}	
	
		int padIndexX;
		int padIndexY;
		int padCountX;
		int padCountY;
		double posX, posY;	//, posZ, posT;
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
        void addHeadSelectBox()
        {
            CbB_Head_Select.Items.Clear();

            CbB_Head_Select.Items.Add("Head 1");
            CbB_Head_Select.Items.Add("Head 2");

            CbB_Head_Select.SelectedIndex = 0;
        }
		private void Position_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region BT_PositionSelect
			if (sender.Equals(BT_PositionSelect_Ref0)) BT_PositionSelect.Text = BT_PositionSelect_Ref0.Text;
			if (sender.Equals(BT_PositionSelect_Ref1_1)) BT_PositionSelect.Text = BT_PositionSelect_Ref1_1.Text;
			if (sender.Equals(BT_PositionSelect_Ref1_2)) BT_PositionSelect.Text = BT_PositionSelect_Ref1_1.Text;

			if (sender.Equals(BT_PositionSelect_Pick1)) BT_PositionSelect.Text = BT_PositionSelect_Pick1.Text;
			if (sender.Equals(BT_PositionSelect_Pick2)) BT_PositionSelect.Text = BT_PositionSelect_Pick2.Text;
			if (sender.Equals(BT_PositionSelect_Pick3)) BT_PositionSelect.Text = BT_PositionSelect_Pick3.Text;
			if (sender.Equals(BT_PositionSelect_Pick4)) BT_PositionSelect.Text = BT_PositionSelect_Pick4.Text;
			if (sender.Equals(BT_PositionSelect_Pick5)) BT_PositionSelect.Text = BT_PositionSelect_Pick5.Text;
			if (sender.Equals(BT_PositionSelect_Pick6)) BT_PositionSelect.Text = BT_PositionSelect_Pick6.Text;
			if (sender.Equals(BT_PositionSelect_Pick7)) BT_PositionSelect.Text = BT_PositionSelect_Pick7.Text;
			if (sender.Equals(BT_PositionSelect_Pick8)) BT_PositionSelect.Text = BT_PositionSelect_Pick8.Text;

			if (sender.Equals(BT_PositionSelect_ULC)) BT_PositionSelect.Text = BT_PositionSelect_ULC.Text;

			if (sender.Equals(BT_PositionSelect_BDEdge)) BT_PositionSelect.Text = BT_PositionSelect_BDEdge.Text;

		   
			if (sender.Equals(BT_PositionSelect_PadCenter))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadCenter.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC1))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC1.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC2))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC2.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC3))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC3.Text;
			}
			else if (sender.Equals(BT_PositionSelect_PadC4))
			{
				CbB_PadIX.Visible = true; CbB_PadIY.Visible = true;
				CbB_PadIX_Separator.Visible = true; CbB_PadIY_Separator.Visible = true;
				BT_PositionSelect.Text = BT_PositionSelect_PadC4.Text;
			}
			else
			{
				CbB_PadIX.Visible = false; CbB_PadIY.Visible = false;
				CbB_PadIX_Separator.Visible = false; CbB_PadIY_Separator.Visible = false;
			}

			if (sender.Equals(BT_PositionSelect_ATC1)) BT_PositionSelect.Text = BT_PositionSelect_ATC1.Text;
			if (sender.Equals(BT_PositionSelect_ATC2)) BT_PositionSelect.Text = BT_PositionSelect_ATC2.Text;
			if (sender.Equals(BT_PositionSelect_ATC3)) BT_PositionSelect.Text = BT_PositionSelect_ATC3.Text;
			if (sender.Equals(BT_PositionSelect_ATC4)) BT_PositionSelect.Text = BT_PositionSelect_ATC4.Text;

			if (sender.Equals(BT_PositionSelect_Loadcell)) BT_PositionSelect.Text = BT_PositionSelect_Loadcell.Text;
			if (sender.Equals(BT_PositionSelect_Touchprobe)) BT_PositionSelect.Text = BT_PositionSelect_Touchprobe.Text;

			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_CameraMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Cameara Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.cPos.x.REF0;
				posY = mc.hd.tool.cPos.y.REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
				posX = mc.hd.tool.cPos.x.REF1_1;
				posY = mc.hd.tool.cPos.y.REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
				posX = mc.hd.tool.cPos.x.REF1_2;
				posY = mc.hd.tool.cPos.y.REF1_2;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF1);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF2);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			{
				if (mc.swcontrol.mechanicalRevision == 0)
				{
					posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF3);
					posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF3);
				}
				else
				{
					posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
					posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
				}
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			{
				if (mc.swcontrol.mechanicalRevision == 0)
				{
					posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF4);
					posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF4);
				}
				else
				{
					posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
					posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
				}
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick5.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick6.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick7.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF7);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF7);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick8.Text)
			{
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF8);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF8);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
				posX = mc.hd.tool.cPos.x.ULC;
				posY = mc.hd.tool.cPos.y.ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
				posX = mc.hd.tool.cPos.x.BD_EDGE;
				posY = mc.hd.tool.cPos.y.BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PAD(padIndexX);
				posY = mc.hd.tool.cPos.y.PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
				posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
				posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
				posX = mc.hd.tool.cPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
				posY = mc.hd.tool.cPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
				posX = mc.hd.tool.cPos.x.LOADCELL;
				posY = mc.hd.tool.cPos.y.LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
				posX = mc.hd.tool.cPos.x.TOUCHPROBE;
				posY = mc.hd.tool.cPos.y.TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.hdc.LIVE = true; mc.idle(300); mc.hdc.LIVE = false;
		EXIT:
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BT_Position_ToolMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Tool Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.tPos.x[0].REF0;
                posY = mc.hd.tool.tPos.y[0].REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
                posX = mc.hd.tool.tPos.x[0].REF1_1;
                posY = mc.hd.tool.tPos.y[0].REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
                posX = mc.hd.tool.tPos.x[0].REF1_2;
                posY = mc.hd.tool.tPos.y[0].REF1_2;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			{
                posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF1);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			{
				posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF2);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			{
				if (mc.swcontrol.mechanicalRevision == 0)
				{
                    posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF3);
                    posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF3);
				}
				else
				{
                    posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF5);
                    posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF5);
				}
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			{
				if (mc.swcontrol.mechanicalRevision == 0)
				{
                    posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF4);
                    posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF4);
				}
				else
				{
                    posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF6);
                    posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF6);
				}
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick5.Text)
			{
                posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF5);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF5);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick6.Text)
			{
                posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF6);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF6);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick7.Text)
			{
                posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF7);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF7);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Pick8.Text)
			{
                posX = mc.hd.tool.tPos.x[0].PICK(UnitCodeSF.SF8);
                posY = mc.hd.tool.tPos.y[0].PICK(UnitCodeSF.SF8);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
                posX = mc.hd.tool.tPos.x[0].ULC;
                posY = mc.hd.tool.tPos.y[0].ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
                posX = mc.hd.tool.tPos.x[0].BD_EDGE;
                posY = mc.hd.tool.tPos.y[0].BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
                posX = mc.hd.tool.tPos.x[0].PAD(padIndexX);
                posY = mc.hd.tool.tPos.y[0].PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
                posX = mc.hd.tool.tPos.x[0].PADC1(padIndexX);
                posY = mc.hd.tool.tPos.y[0].PADC1(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
                posX = mc.hd.tool.tPos.x[0].PADC2(padIndexX);
                posY = mc.hd.tool.tPos.y[0].PADC2(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
                posX = mc.hd.tool.tPos.x[0].PADC3(padIndexX);
                posY = mc.hd.tool.tPos.y[0].PADC3(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
                posX = mc.hd.tool.tPos.x[0].PADC4(padIndexX);
                posY = mc.hd.tool.tPos.y[0].PADC4(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
                posX = mc.hd.tool.tPos.x[0].TOOL_CHANGER(UnitCodeToolChanger.T1);
                posY = mc.hd.tool.tPos.y[0].TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
                posX = mc.hd.tool.tPos.x[0].TOOL_CHANGER(UnitCodeToolChanger.T2);
                posY = mc.hd.tool.tPos.y[0].TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC3.Text)
			{
                posX = mc.hd.tool.tPos.x[0].TOOL_CHANGER(UnitCodeToolChanger.T3);
                posY = mc.hd.tool.tPos.y[0].TOOL_CHANGER(UnitCodeToolChanger.T3);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC4.Text)
			{
                posX = mc.hd.tool.tPos.x[0].TOOL_CHANGER(UnitCodeToolChanger.T4);
                posY = mc.hd.tool.tPos.y[0].TOOL_CHANGER(UnitCodeToolChanger.T4);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
                posX = mc.hd.tool.tPos.x[0].LOADCELL;
                posY = mc.hd.tool.tPos.y[0].LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
                posX = mc.hd.tool.tPos.x[0].TOUCHPROBE;
                posY = mc.hd.tool.tPos.y[0].TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
		EXIT:
			#endregion
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}

		private void BottomRight_Head_Load(object sender, EventArgs e)
		{
			if (mc.swcontrol.mechanicalRevision == 1)
			{
				BT_PositionSelect_Pick5.Visible = false;
				BT_PositionSelect_Pick6.Visible = false;
				BT_PositionSelect_Pick7.Visible = false;
				BT_PositionSelect_Pick8.Visible = false;
			}
		}

		private void BT_Position_LaserMove_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);
			#region Position Laser Move
			if (BT_PositionSelect.Text == BT_PositionSelect_Ref0.Text)
			{
				posX = mc.hd.tool.lPos.x.REF0;
				posY = mc.hd.tool.lPos.y.REF0;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_1.Text)
			{
				posX = mc.hd.tool.lPos.x.REF1_1;
				posY = mc.hd.tool.lPos.y.REF1_1;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Ref1_2.Text)
			{
				posX = mc.hd.tool.lPos.x.REF1_2;
				posY = mc.hd.tool.lPos.y.REF1_2;
			}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick1.Text)
			//{
			//    posX = mc.hd.tool.lPos.x.PICK(UnitCodeSF.SF1);
			//    posY = mc.hd.tool.lPos.y.PICK(UnitCodeSF.SF1);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick2.Text)
			//{
			//    posX = mc.hd.tool.lPos.x.PICK(UnitCodeSF.SF2);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF2);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick3.Text)
			//{
			//    if (mc.swcontrol.mechanicalRevision == 0)
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF3);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF3);
			//    }
			//    else
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
			//    }
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick4.Text)
			//{
			//    if (mc.swcontrol.mechanicalRevision == 0)
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF4);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF4);
			//    }
			//    else
			//    {
			//        posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
			//        posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
			//    }
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick5.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF5);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF5);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick6.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF6);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF6);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick7.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF7);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF7);
			//}
			//else if (BT_PositionSelect.Text == BT_PositionSelect_Pick8.Text)
			//{
			//    posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF8);
			//    posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF8);
			//}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ULC.Text)
			{
				posX = mc.hd.tool.lPos.x.ULC;
				posY = mc.hd.tool.lPos.y.ULC;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_BDEdge.Text)
			{
				posX = mc.hd.tool.lPos.x.BD_EDGE;
				posY = mc.hd.tool.lPos.y.BD_EDGE;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadCenter.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PAD(padIndexX);
				posY = mc.hd.tool.lPos.y.PAD(padIndexY);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC1.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC1(padIndexX) - 500;
				posY = mc.hd.tool.lPos.y.PADC1(padIndexY) - 500;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC2.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC2(padIndexX) - 500;
				posY = mc.hd.tool.lPos.y.PADC2(padIndexY) + 500;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC3.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC3(padIndexX) + 500;
				posY = mc.hd.tool.lPos.y.PADC3(padIndexY) + 500;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_PadC4.Text)
			{
				padIndexX = CbB_PadIX.SelectedIndex;
				padIndexY = CbB_PadIY.SelectedIndex;
				posX = mc.hd.tool.lPos.x.PADC4(padIndexX) + 500;
				posY = mc.hd.tool.lPos.y.PADC4(padIndexY) - 500;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC1.Text)
			{
				posX = mc.hd.tool.lPos.x.TOOL_CHANGER(UnitCodeToolChanger.T1);
				posY = mc.hd.tool.lPos.y.TOOL_CHANGER(UnitCodeToolChanger.T1);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_ATC2.Text)
			{
				posX = mc.hd.tool.lPos.x.TOOL_CHANGER(UnitCodeToolChanger.T2);
				posY = mc.hd.tool.lPos.y.TOOL_CHANGER(UnitCodeToolChanger.T2);
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Loadcell.Text)
			{
				posX = mc.hd.tool.lPos.x.LOADCELL;
				posY = mc.hd.tool.lPos.y.LOADCELL;
			}
			else if (BT_PositionSelect.Text == BT_PositionSelect_Touchprobe.Text)
			{
				posX = mc.hd.tool.lPos.x.TOUCHPROBE;
				posY = mc.hd.tool.lPos.y.TOUCHPROBE;
			}
			else
			{
				goto EXIT;
			}
			mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			/*mc.hdc.LIVE = true; */
			mc.idle(300); mc.hdc.LIVE = false;
		EXIT:
			#endregion

			mc.OUT.HD.LS.ON(out ret.b, out ret.message);
			if (ret.message == RetMessage.OK) if(!ret.b) mc.OUT.HD.LS.ON(true, out ret.message);

			mc.main.Thread_Polling();
			mc.check.push(sender, false);
		}
	}
}
