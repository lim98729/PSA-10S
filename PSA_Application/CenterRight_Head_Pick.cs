﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using AccessoryLibrary;

namespace PSA_Application
{
	public partial class CenterRight_Head_Pick : UserControl
	{
		public CenterRight_Head_Pick()
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
		#endregion
		RetValue ret;
		UnitCodeSF slectOffsetSF = UnitCodeSF.SF1;
		QueryTimer dwell = new QueryTimer();

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			#region search
			if (sender.Equals(BT_Search1st_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.search.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Search1st_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.search.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Search1st_Level)) mc.para.setting(mc.para.HD.pick.search.level, out mc.para.HD.pick.search.level);
			if (sender.Equals(TB_Search1st_Speed)) mc.para.setting(mc.para.HD.pick.search.vel, out mc.para.HD.pick.search.vel);
			if (sender.Equals(TB_Search1st_Delay)) mc.para.setting(mc.para.HD.pick.search.delay, out mc.para.HD.pick.search.delay);
			#endregion
			#region search2
			if (sender.Equals(BT_Search2nd_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.search2.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Search2nd_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.search2.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Search2nd_Level)) mc.para.setting(mc.para.HD.pick.search2.level, out mc.para.HD.pick.search2.level);
			if (sender.Equals(TB_Search2nd_Speed)) mc.para.setting(mc.para.HD.pick.search2.vel, out mc.para.HD.pick.search2.vel);
			if (sender.Equals(TB_Search2nd_Delay)) mc.para.setting(mc.para.HD.pick.search2.delay, out mc.para.HD.pick.search2.delay);
			#endregion
			#region delay
			if (sender.Equals(TB_Delay)) mc.para.setting(mc.para.HD.pick.delay, out mc.para.HD.pick.delay);
			if (sender.Equals(TB_Force)) mc.para.setting(mc.para.HD.pick.force, out mc.para.HD.pick.force);
			#endregion
			#region driver
			if (sender.Equals(BT_Drive1st_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.driver.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Drive1st_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.driver.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Drive1st_Level)) mc.para.setting(mc.para.HD.pick.driver.level, out mc.para.HD.pick.driver.level);
			if (sender.Equals(TB_Drive1st_Speed)) mc.para.setting(mc.para.HD.pick.driver.vel, out mc.para.HD.pick.driver.vel);
			if (sender.Equals(TB_Drive1st_Delay)) mc.para.setting(mc.para.HD.pick.driver.delay, out mc.para.HD.pick.driver.delay);
			#endregion
			#region driver2
			if (sender.Equals(BT_Drive2nd_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.driver2.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Drive2nd_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.driver2.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Drive2nd_Level)) mc.para.setting(mc.para.HD.pick.driver2.level, out mc.para.HD.pick.driver2.level);
			if (sender.Equals(TB_Drive2nd_Speed)) mc.para.setting(mc.para.HD.pick.driver2.vel, out mc.para.HD.pick.driver2.vel);
			if (sender.Equals(TB_Drive2nd_Delay)) mc.para.setting(mc.para.HD.pick.driver2.delay, out mc.para.HD.pick.driver2.delay);
			#endregion
			#region offset
			if (sender.Equals(BT_PositionOffset_SelectSF1)) slectOffsetSF = UnitCodeSF.SF1;
			if (sender.Equals(BT_PositionOffset_SelectSF2)) slectOffsetSF = UnitCodeSF.SF2;
			if (sender.Equals(BT_PositionOffset_SelectSF3))
			{
				if (mc.swcontrol.mechanicalRevision == 0) slectOffsetSF = UnitCodeSF.SF3;
				else slectOffsetSF = UnitCodeSF.SF5;
			}
			if (sender.Equals(BT_PositionOffset_SelectSF4))
			{
				if (mc.swcontrol.mechanicalRevision == 0) slectOffsetSF = UnitCodeSF.SF4;
				else slectOffsetSF = UnitCodeSF.SF6;
			}
			if (sender.Equals(BT_PositionOffset_SelectSF5)) slectOffsetSF = UnitCodeSF.SF5;
			if (sender.Equals(BT_PositionOffset_SelectSF6)) slectOffsetSF = UnitCodeSF.SF6;
			if (sender.Equals(BT_PositionOffset_SelectSF7)) slectOffsetSF = UnitCodeSF.SF7;
			if (sender.Equals(BT_PositionOffset_SelectSF8)) slectOffsetSF = UnitCodeSF.SF8;
			if (sender.Equals(TB_PositionOffset_X)) mc.para.setting(mc.para.HD.pick.offset[(int)slectOffsetSF].x, out mc.para.HD.pick.offset[(int)slectOffsetSF].x);
			if (sender.Equals(TB_PositionOffset_Y)) mc.para.setting(mc.para.HD.pick.offset[(int)slectOffsetSF].y, out mc.para.HD.pick.offset[(int)slectOffsetSF].y);
			//if (sender.Equals(TB_PositionOffset_Z)) mc.para.setting(mc.para.HD.pick.offset[(int)slectOffsetSF].z, out mc.para.HD.pick.offset[(int)slectOffsetSF].z);
			if (sender.Equals(BT_PositionOffset_Jog))
			{
				double posX, posY, posZ, posT;
				#region moving
				posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].PICK(slectOffsetSF);
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].PICK(slectOffsetSF);
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].PICK(slectOffsetSF);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPadXYZ ff = new FormJogPadXYZ();
				#region jogMode
				if (slectOffsetSF == UnitCodeSF.SF1) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF1;
				else if (slectOffsetSF == UnitCodeSF.SF2) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF2;
				else if (slectOffsetSF == UnitCodeSF.SF3) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF3;
				else if (slectOffsetSF == UnitCodeSF.SF4) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF4;
				else if (slectOffsetSF == UnitCodeSF.SF5) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF5;
				else if (slectOffsetSF == UnitCodeSF.SF6) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF6;
				else if (slectOffsetSF == UnitCodeSF.SF7) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF7;
				else if (slectOffsetSF == UnitCodeSF.SF8) ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF8;
				else ff.jogMode = JOGXYZ_MODE.HD_PICK_OFFSET_SF1;
				#endregion
				ff.dataX = mc.para.HD.pick.offset[(int)slectOffsetSF].x;
				ff.dataY = mc.para.HD.pick.offset[(int)slectOffsetSF].y;
				ff.dataZ = mc.para.HD.pick.offset[(int)slectOffsetSF].z;
				ff.ShowDialog();
				mc.para.setting(ref mc.para.HD.pick.offset[(int)slectOffsetSF].x, ff.dataX.value);
				mc.para.setting(ref mc.para.HD.pick.offset[(int)slectOffsetSF].y, ff.dataY.value);
				mc.para.setting(ref mc.para.HD.pick.offset[(int)slectOffsetSF].z, ff.dataZ.value);
				#region moving
				posX = mc.hd.tool.cPos.x.REF0;
				posY = mc.hd.tool.cPos.y.REF0; ;
				posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].XY_MOVING;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			if (sender.Equals(BT_PositionOffset_AllClear))
			{
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_PICK_INIT_OFFSET_XYZ);
				//mc.message.OkCancel("모든 Pick Offset X,Y,Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
				for (int i = 0; i < 8; i++)
				{
					mc.para.setting(ref mc.para.HD.pick.offset[i].x, 0);
					mc.para.setting(ref mc.para.HD.pick.offset[i].y, 0);
					mc.para.setting(ref mc.para.HD.pick.offset[i].z, 0);
				}
			}
			if (sender.Equals(BT_AutoPickCompenClear))
			{
				FormUserMessage ff = new FormUserMessage();
                ff.SetDisplayItems(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.WARNING, textResource.MB_HD_PICK_INIT_AUTO_TRACK);
				ff.ShowDialog();

				ret.usrDialog = FormUserMessage.diagResult;
				if (ret.usrDialog == DIAG_RESULT.Yes)
				{
					for (int i = 0; i < 8; i++)
					{
						mc.para.HD.pick.pickPosComp[i].x.value = 0;
						mc.para.HD.pick.pickPosComp[i].y.value = 0;
					}
                    ff.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_HD_PICK_INIT_FINISH);
					ff.ShowDialog();
				}
			}
			if (sender.Equals(BT_PickOffsetZCal))
			{
				mc.log.debug.write(mc.log.CODE.CAL, "Pick Z Offset Calibration START");
				double posX, posY, posZ, posT;
				int moveCase;
				double startZPos, sensor1Pos, sensor2Pos;
				bool pos1Done;
				bool pos2Done;
				bool teachDone = false;
				double[] tempval = new double[5];
				int stackFeedNum = 8;
				int sfTemp = 0;
				if (mc.swcontrol.mechanicalRevision == 1) stackFeedNum = 4;
				for (int i = 0; i < stackFeedNum; i++)
				{
					if (mc.swcontrol.mechanicalRevision == 1 && i >= 2) sfTemp = i + 2;
					mc.log.debug.write(mc.log.CODE.CAL, String.Format("Stack Feeder Move to {0} position", sfTemp + 1));
					for (int k = 0; k < 5; k++)
					{
						#region move stack feeder
						mc.sf.req = true; mc.sf.reqMode = REQMODE.READY;
						mc.sf.reqTubeNumber = (UnitCodeSF)sfTemp;
						mc.main.Thread_Polling();	// make stack feeder to be ready for picking
						if (mc.sf.ERROR)
						{
							mc.log.debug.write(mc.log.CODE.ERROR, "CANNOT run calibration process. Stack Feeder Error");
							goto EXIT;
						}
						#endregion

						#region move to pick position
                        posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].PICK((UnitCodeSF)sfTemp);
                        posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].PICK((UnitCodeSF)sfTemp);
                        posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].RAWPICK;
                        posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;

						mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						mc.idle(100);
						#endregion

						#region sensor check
                        //mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message);
                        //mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message);

						if (ret.b1 == false && ret.b2 == true) moveCase = 0;		// touch가 안되어 있거나 살짝 눌린 상태
						else if (ret.b1 == true && ret.b2 == true) moveCase = 1;	// 200~350um 눌린 상태
						else if (ret.b1 == true && ret.b2 == false) moveCase = 2;	// 350um이상 눌린 상태
						else
						{
							moveCase = 3;  // 몰라...이런 상태는 없어...
							mc.message.alarm("Load Sensor Error : Sensor ALL OFF"); goto EXIT;
						}
						#endregion

						if (moveCase > 0)
						{
							FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.FAILURE, "Pick Position이 너무 낮습니다.\nPick Position값을 먼저 설정해 주세요.");
							//mc.message.alarm("Pick Position is too LOW. Please change pick position."); goto EXIT;
						}

                        mc.hd.tool.Z[(int)UnitCodeHead.HD1].actualPosition(out startZPos, out ret.message);
						teachDone = false;

						for (int j = 0; j < 3; j++)
						{
							if (teachDone) break;

                            mc.hd.tool.Z[(int)UnitCodeHead.HD1].move(posZ - 600 * (j + 1), mc.speed.checkSpeed, out ret.message);

							dwell.Reset();
							sensor1Pos = 0;
							sensor2Pos = 0;
							pos1Done = false;
							pos2Done = false;
							while (true)
							{
// 								mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message);
// 								mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message);
								if (ret.b1 && !pos1Done)
								{
                                    mc.hd.tool.Z[(int)UnitCodeHead.HD1].actualPosition(out sensor1Pos, out ret.message);
									pos1Done = true;
								}
								if (ret.b1 && !ret.b2 && !pos2Done)
								{
                                    mc.hd.tool.Z[(int)UnitCodeHead.HD1].actualPosition(out sensor2Pos, out ret.message);
									pos2Done = true;
								}
                                mc.hd.tool.Z[(int)UnitCodeHead.HD1].AT_TARGET(out ret.b, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
								if (ret.b) break;
								if (dwell.Elapsed > 20000) { ret.message = RetMessage.TIMEOUT; mc.message.alarmMotion(ret.message); goto EXIT; }
							}
							if (sensor1Pos == 0)
							{
								mc.log.debug.write(mc.log.CODE.TRACE, "Cannot find in search " + (j + 1).ToString());
							}
							else
							{
								mc.log.debug.write(mc.log.CODE.TRACE, "1st Pos : " + Math.Round((startZPos - sensor1Pos), 3).ToString("f3") + ", 2nd Pos : " + Math.Round((startZPos - sensor2Pos), 3).ToString("f3"));
							}
							dwell.Reset();
							while (true)
							{
								if (dwell.Elapsed > 500) { ret.message = RetMessage.TIMEOUT; mc.message.alarmMotion(ret.message); goto EXIT; }
                                mc.hd.tool.Z[(int)UnitCodeHead.HD1].AT_DONE(out ret.b, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
								if (ret.b) break;
							}
							if (sensor1Pos != 0)
							{
								double offset = Math.Round(sensor1Pos - startZPos) - mc.para.CAL.z.sensor1.value;
								mc.log.debug.write(mc.log.CODE.CAL, "SF" + (sfTemp + 1).ToString() + " Offset: " + offset.ToString() + "[um]");
								tempval[k] = offset;
								teachDone = true;
								break;
							}
							else
							{
								mc.idle(100);
							}
						}
                        posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].XY_MOVING;
						mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

						double curSFZPos;
						mc.sf.Z.actualPosition(out curSFZPos, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						mc.sf.jogMoveZ(curSFZPos - 300, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					}
					mc.log.debug.write(mc.log.CODE.TRACE, "1st[" + tempval[0].ToString() + "], 2nd[" + tempval[1].ToString() + "], 3rd[" + tempval[2].ToString() + "], 4th[" + tempval[3].ToString() + "], 5th[" + tempval[4].ToString() + "]");
					double sum = tempval[0] + tempval[1] + tempval[2] + tempval[3] + tempval[4];
					mc.log.debug.write(mc.log.CODE.CAL, "SF" + (sfTemp + 1).ToString() + " Offset RESULT : " + Math.Round(sum / 5).ToString());

					mc.para.setting(ref mc.para.HD.pick.offset[sfTemp].z, Math.Round(sum / 5));
				}
				mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
				mc.sf.reqTubeNumber = UnitCodeSF.SF1;
				mc.main.Thread_Polling();
				mc.log.debug.write(mc.log.CODE.CAL, "Pick Z Offset Calibration END");
			}
			if (sender.Equals(TB_PickOffsetZ1)) mc.para.setting(mc.para.HD.pick.offset[0].z, out mc.para.HD.pick.offset[0].z);
			if (sender.Equals(TB_PickOffsetZ2)) mc.para.setting(mc.para.HD.pick.offset[1].z, out mc.para.HD.pick.offset[1].z);
			if (sender.Equals(TB_PickOffsetZ3)) mc.para.setting(mc.para.HD.pick.offset[2].z, out mc.para.HD.pick.offset[2].z);
			if (sender.Equals(TB_PickOffsetZ4)) mc.para.setting(mc.para.HD.pick.offset[3].z, out mc.para.HD.pick.offset[3].z);
			if (sender.Equals(TB_PickOffsetZ5)) mc.para.setting(mc.para.HD.pick.offset[4].z, out mc.para.HD.pick.offset[4].z);
			if (sender.Equals(TB_PickOffsetZ6)) mc.para.setting(mc.para.HD.pick.offset[5].z, out mc.para.HD.pick.offset[5].z);
			if (sender.Equals(TB_PickOffsetZ7)) mc.para.setting(mc.para.HD.pick.offset[6].z, out mc.para.HD.pick.offset[6].z);
			if (sender.Equals(TB_PickOffsetZ8)) mc.para.setting(mc.para.HD.pick.offset[7].z, out mc.para.HD.pick.offset[7].z);
			#endregion
			#region suction
			if (sender.Equals(BT_SuctionMode_Select_MovingLevelOn)) mc.para.setting(ref mc.para.HD.pick.suction.mode, (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON);
			if (sender.Equals(BT_SuctionMode_Select_SearchLevelOn)) mc.para.setting(ref mc.para.HD.pick.suction.mode, (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON);
			if (sender.Equals(BT_SuctionMode_Select_PickLevelOn)) mc.para.setting(ref mc.para.HD.pick.suction.mode, (int)PICK_SUCTION_MODE.PICK_LEVEL_ON);
			if (sender.Equals(TB_SuctionMode_Level)) mc.para.setting(mc.para.HD.pick.suction.level, out mc.para.HD.pick.suction.level);
			if (sender.Equals(BT_SuctionCheck_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.suction.check, (int)ON_OFF.ON);
			if (sender.Equals(BT_SuctionCheck_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.suction.check, (int)ON_OFF.OFF);
            if (sender.Equals(TB_SuctionCheckLimit_HD1)) mc.para.setting(mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD1], out mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD1]);
            if (sender.Equals(TB_SuctionCheckLimit_HD2)) mc.para.setting(mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD2], out mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD2]);
            if (sender.Equals(TB_SuctionCheck_Time)) mc.para.setting(mc.para.HD.pick.suction.checkLimitTime, out mc.para.HD.pick.suction.checkLimitTime);
			#endregion
			#region missCheck
			if (sender.Equals(BT_MissCheck_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.pick.missCheck.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_MissCheck_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.pick.missCheck.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_MissCheck_Retry)) mc.para.setting(mc.para.HD.pick.missCheck.retry, out mc.para.HD.pick.missCheck.retry);
			#endregion
		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
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
				#region search
				if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON)
				{
					LB_Search1st_Level.Visible = true; TB_Search1st_Level.Visible = true;
					LB_Search1st_Speed.Visible = true; TB_Search1st_Speed.Visible = true;
					LB_Search1st_Delay.Visible = true; TB_Search1st_Delay.Visible = true;
					BT_Search1st_SelectOnOff.Text = BT_Search1st_SelectOnOff_On.Text;
					BT_Search1st_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Search1st_Level.Text = mc.para.HD.pick.search.level.value.ToString();
					TB_Search1st_Speed.Text = mc.para.HD.pick.search.vel.value.ToString();
					TB_Search1st_Delay.Text = mc.para.HD.pick.search.delay.value.ToString();
				}
				if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.OFF)
				{
					LB_Search1st_Level.Visible = false; TB_Search1st_Level.Visible = false;
					LB_Search1st_Speed.Visible = false; TB_Search1st_Speed.Visible = false;
					LB_Search1st_Delay.Visible = false; TB_Search1st_Delay.Visible = false;
					BT_Search1st_SelectOnOff.Text = BT_Search1st_SelectOnOff_Off.Text;
					BT_Search1st_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region search2
				if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON)
				{
					LB_Search2nd_Level.Visible = true; TB_Search2nd_Level.Visible = true;
					LB_Search2nd_Speed.Visible = true; TB_Search2nd_Speed.Visible = true;
					LB_Search2nd_Delay.Visible = true; TB_Search2nd_Delay.Visible = true;
					BT_Search2nd_SelectOnOff.Text = BT_Search2nd_SelectOnOff_On.Text;
					BT_Search2nd_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Search2nd_Level.Text = mc.para.HD.pick.search2.level.value.ToString();
					TB_Search2nd_Speed.Text = mc.para.HD.pick.search2.vel.value.ToString();
					TB_Search2nd_Delay.Text = mc.para.HD.pick.search2.delay.value.ToString();
				}
				if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.OFF)
				{
					LB_Search2nd_Level.Visible = false; TB_Search2nd_Level.Visible = false;
					LB_Search2nd_Speed.Visible = false; TB_Search2nd_Speed.Visible = false;
					LB_Search2nd_Delay.Visible = false; TB_Search2nd_Delay.Visible = false;
					BT_Search2nd_SelectOnOff.Text = BT_Search2nd_SelectOnOff_Off.Text;
					BT_Search2nd_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				TB_Delay.Text = mc.para.HD.pick.delay.value.ToString();
				TB_Force.Text = mc.para.HD.pick.force.value.ToString();

				#region driver
				if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON)
				{
					LB_Drive1st_Level.Visible = true; TB_Drive1st_Level.Visible = true;
					LB_Drive1st_Speed.Visible = true; TB_Drive1st_Speed.Visible = true;
					LB_Drive1st_Delay.Visible = true; TB_Drive1st_Delay.Visible = true;
					BT_Drive1st_SelectOnOff.Text = BT_Drive1st_SelectOnOff_On.Text;
					BT_Drive1st_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Drive1st_Level.Text = mc.para.HD.pick.driver.level.value.ToString();
					TB_Drive1st_Speed.Text = mc.para.HD.pick.driver.vel.value.ToString();
					TB_Drive1st_Delay.Text = mc.para.HD.pick.driver.delay.value.ToString();
				}
				if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.OFF)
				{
					LB_Drive1st_Level.Visible = false; TB_Drive1st_Level.Visible = false;
					LB_Drive1st_Speed.Visible = false; TB_Drive1st_Speed.Visible = false;
					LB_Drive1st_Delay.Visible = false; TB_Drive1st_Delay.Visible = false;
					BT_Drive1st_SelectOnOff.Text = BT_Drive1st_SelectOnOff_Off.Text;
					BT_Drive1st_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region driver2
				if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON)
				{
					LB_Drive2nd_Level.Visible = true; TB_Drive2nd_Level.Visible = true;
					LB_Drive2nd_Speed.Visible = true; TB_Drive2nd_Speed.Visible = true;
					LB_Drive2nd_Delay.Visible = true; TB_Drive2nd_Delay.Visible = true;
					BT_Drive2nd_SelectOnOff.Text = BT_Drive2nd_SelectOnOff_On.Text;
					BT_Drive2nd_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Drive2nd_Level.Text = mc.para.HD.pick.driver2.level.value.ToString();
					TB_Drive2nd_Speed.Text = mc.para.HD.pick.driver2.vel.value.ToString();
					TB_Drive2nd_Delay.Text = mc.para.HD.pick.driver2.delay.value.ToString();
				}
				if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.OFF)
				{
					LB_Drive2nd_Level.Visible = false; TB_Drive2nd_Level.Visible = false;
					LB_Drive2nd_Speed.Visible = false; TB_Drive2nd_Speed.Visible = false;
					LB_Drive2nd_Delay.Visible = false; TB_Drive2nd_Delay.Visible = false;
					BT_Drive2nd_SelectOnOff.Text = BT_Drive2nd_SelectOnOff_Off.Text;
					BT_Drive2nd_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region offset
				if(mc.swcontrol.mechanicalRevision == 0) BT_PositionOffset_SelectSF.Text = slectOffsetSF.ToString();
				else
				{
					if (slectOffsetSF == UnitCodeSF.SF5) BT_PositionOffset_SelectSF.Text = UnitCodeSF.SF3.ToString();
					else if (slectOffsetSF == UnitCodeSF.SF6) BT_PositionOffset_SelectSF.Text = UnitCodeSF.SF4.ToString();
					else BT_PositionOffset_SelectSF.Text = slectOffsetSF.ToString();
				}

				TB_PositionOffset_X.Text = mc.para.HD.pick.offset[(int)slectOffsetSF].x.value.ToString();
				TB_PositionOffset_Y.Text = mc.para.HD.pick.offset[(int)slectOffsetSF].y.value.ToString();
				//TB_PositionOffset_Z.Text = mc.para.HD.pick.offset[(int)slectOffsetSF].z.value.ToString();
				TB_PickOffsetZ1.Text = mc.para.HD.pick.offset[0].z.value.ToString();
				TB_PickOffsetZ2.Text = mc.para.HD.pick.offset[1].z.value.ToString();
				TB_PickOffsetZ3.Text = mc.para.HD.pick.offset[2].z.value.ToString();
				TB_PickOffsetZ4.Text = mc.para.HD.pick.offset[3].z.value.ToString();
				TB_PickOffsetZ5.Text = mc.para.HD.pick.offset[4].z.value.ToString();
				TB_PickOffsetZ6.Text = mc.para.HD.pick.offset[5].z.value.ToString();
				TB_PickOffsetZ7.Text = mc.para.HD.pick.offset[6].z.value.ToString();
				TB_PickOffsetZ8.Text = mc.para.HD.pick.offset[7].z.value.ToString();
				#endregion

				#region suction
				if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_MovingLevelOn.Text;
					LB_SuctionMode_Level.Visible = false; TB_SuctionMode_Level.Visible = false; 
				}
				if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_SearchLevelOn.Text;
					LB_SuctionMode_Level.Visible = true; TB_SuctionMode_Level.Visible = true;
					TB_SuctionMode_Level.Text = mc.para.HD.pick.suction.level.value.ToString();
				}
				if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.PICK_LEVEL_ON)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_PickLevelOn.Text;
					LB_SuctionMode_Level.Visible = false; TB_SuctionMode_Level.Visible = false; 
				}
				if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.ON)
				{
					BT_SuctionCheck_SelectOnOff.Text = BT_SuctionCheck_SelectOnOff_On.Text;
					BT_SuctionCheck_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_SuctionCheck_Time.Text = mc.para.HD.pick.suction.checkLimitTime.value.ToString();
					LB_SuctionCheck_Limit.Visible = true; TB_SuctionCheck_Time.Visible = true;
                    TB_SuctionCheckLimit_HD1.Text = mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD1].value.ToString();
                    TB_SuctionCheckLimit_HD2.Text = mc.para.HD.pick.pickupVacLimit[(int)UnitCodeHead.HD2].value.ToString();
                    LB_SuctionCheckLimit_HD1.Visible = true; TB_SuctionCheckLimit_HD1.Visible = true; 
                    LB_SuctionCheckLimit_HD2.Visible = true; TB_SuctionCheckLimit_HD2.Visible = true; 
                    toolStripSeparator4.Visible = true; toolStripSeparator11.Visible = true; toolStripSeparator35.Visible = true;
				}
				if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.OFF)
				{
					BT_SuctionCheck_SelectOnOff.Text = BT_SuctionCheck_SelectOnOff_Off.Text;
					BT_SuctionCheck_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
					LB_SuctionCheck_Limit.Visible = false; TB_SuctionCheck_Time.Visible = false;
                    LB_SuctionCheckLimit_HD1.Visible = false; TB_SuctionCheckLimit_HD1.Visible = false;
                    LB_SuctionCheckLimit_HD2.Visible = false; TB_SuctionCheckLimit_HD2.Visible = false;
                    toolStripSeparator4.Visible = false; toolStripSeparator11.Visible = false; toolStripSeparator35.Visible = false;
				}
				#endregion

				#region missCheck
				if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.ON)
				{
					BT_MissCheck_SelectOnOff.Text = BT_MissCheck_SelectOnOff_On.Text;
					BT_MissCheck_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_MissCheck_Retry.Text = mc.para.HD.pick.missCheck.retry.value.ToString();
					LB_MissCheck_Retry.Visible = true; TB_MissCheck_Retry.Visible = true;
				}
				if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.OFF)
				{
					BT_MissCheck_SelectOnOff.Text = BT_MissCheck_SelectOnOff_Off.Text;
					BT_MissCheck_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
					LB_MissCheck_Retry.Visible = false; TB_MissCheck_Retry.Visible = false;
				}
				#endregion

				#region shake motion
				if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.ON)
				{
					BT_ShakeUse_OnOff.Text = BT_ShakeUse_OnOff_On.Text;
					BT_ShakeUse_OnOff.Image = Properties.Resources.Yellow_LED;
					TB_ShakeCount.Text = mc.para.HD.pick.shake.count.value.ToString();
					TB_ShakeDist.Text = mc.para.HD.pick.shake.level.value.ToString();
					TB_ShakeSpeed.Text = mc.para.HD.pick.shake.speed.value.ToString();
					TB_ShakeDelay.Text = mc.para.HD.pick.shake.delay.value.ToString();
					TB_ShakeCount.Visible = true; TB_ShakeDist.Visible = true;
					LB_ShakeCount.Visible = true; LB_ShakeDist.Visible = true;
					TB_ShakeSpeed.Visible = true; TB_ShakeDelay.Visible = true;
					LB_ShakeSpeed.Visible = true; LB_ShakeDelay.Visible = true;
				}
				if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.OFF)
				{
					BT_ShakeUse_OnOff.Text = BT_ShakeUse_OnOff_Off.Text;
					BT_ShakeUse_OnOff.Image = Properties.Resources.YellowLED_OFF;
					TB_ShakeCount.Visible = false; TB_ShakeDist.Visible = false;
					LB_ShakeCount.Visible = false; LB_ShakeDist.Visible = false;
					TB_ShakeSpeed.Visible = false; TB_ShakeDelay.Visible = false;
					LB_ShakeSpeed.Visible = false; LB_ShakeDelay.Visible = false;
				}
				#endregion

				TB_WasteDelay.Text = mc.para.HD.pick.wasteDelay.value.ToString();

				LB_.Focus();
			}
		}

		private void CenterRight_Head_Pick_Load(object sender, EventArgs e)
		{
			if (mc.swcontrol.mechanicalRevision == 1)
			{
				LB_PickOffsetZ1.Text = "SF1";
				LB_PickOffsetZ2.Text = "SF2";
				LB_PickOffsetZ5.Text = "SF3";
				LB_PickOffsetZ6.Text = "SF4";

				LB_PickOffsetZ3.Visible = false;
				LB_PickOffsetZ4.Visible = false;
				LB_PickOffsetZ7.Visible = false;
				LB_PickOffsetZ8.Visible = false;

				TB_PickOffsetZ1.Size = new System.Drawing.Size(50, 25);
				TB_PickOffsetZ2.Size = new System.Drawing.Size(50, 25);
				TB_PickOffsetZ5.Size = new System.Drawing.Size(50, 25);
				TB_PickOffsetZ6.Size = new System.Drawing.Size(50, 25);

				TB_PickOffsetZ3.Visible = false;
				TB_PickOffsetZ4.Visible = false;
				TB_PickOffsetZ7.Visible = false;
				TB_PickOffsetZ8.Visible = false;

				SL_PickOffsetZ3.Visible = false;
				SL_PickOffsetZ4.Visible = false;
				SL_PickOffsetZ7.Visible = false;
				SL_PickOffsetZ8.Visible = false;

				if (mc.swcontrol.mechanicalRevision == 1)
				{
					BT_PositionOffset_SelectSF.DropDownItems.Remove(BT_PositionOffset_SelectSF5);
					BT_PositionOffset_SelectSF.DropDownItems.Remove(BT_PositionOffset_SelectSF6);
					BT_PositionOffset_SelectSF.DropDownItems.Remove(BT_PositionOffset_SelectSF7);
					BT_PositionOffset_SelectSF.DropDownItems.Remove(BT_PositionOffset_SelectSF8);
				}
				else
				{
					BT_PositionOffset_SelectSF.DropDownItems.Add(BT_PositionOffset_SelectSF5);
					BT_PositionOffset_SelectSF.DropDownItems.Add(BT_PositionOffset_SelectSF6);
					BT_PositionOffset_SelectSF.DropDownItems.Add(BT_PositionOffset_SelectSF7);
					BT_PositionOffset_SelectSF.DropDownItems.Add(BT_PositionOffset_SelectSF8);
				}
			}
		}
	}
}
