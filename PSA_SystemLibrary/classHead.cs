using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;
using System.IO;
using AccessoryLibrary;
using System.Diagnostics;

namespace PSA_SystemLibrary
{
	public class classHead : CONTROL
	{
		public classHeadTool tool = new classHeadTool();
        public classMultiHeadOrder order = new classMultiHeadOrder();

		public bool isActivate
		{
			get
			{
				return tool.isActivate;
			}
		}
		public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig[] z, axisConfig[] t, axtOut triggerHDC, axtOut triggerULC, out RetMessage retMessage)
		{
			tool.activate(x, y, y2, z, t, triggerHDC, triggerULC, out retMessage);
		}
        public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig[] z, axisConfig[] t, mpiNodeDigtalOut triggerHDC, mpiNodeDigtalOut triggerULC, out RetMessage retMessage)
        {
            tool.activate(x, y, y2, z, t, triggerHDC, triggerULC, out retMessage);
        }
		public void deactivate(out RetMessage retMessage)
		{
			tool.deactivate(out retMessage);
		}

        public int tempIndex;
		public void checkTMSFileRead(out bool result)
		{
			try
			{
				if (mc.board.working.tmsInfo.readOK.I > 0)
					result = true;
				else
					result = false;
			}
			catch
			{
				result = false;
			}
		}
		StringBuilder tempSb = new StringBuilder();
		public void checkNoExistPad(int ix, int iy, out bool result, out string msg)
		{
			msg = "";
			try
			{
				tempSb.Clear(); tempSb.Length = 0;

				mc.board.xy2index(tool.padX, tool.padY, out tempIndex, out ret.b);
				if (ret.b)
				{
					if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == (int)TMSCODE.READY)
					{
						if (WorkAreaControl.workArea[tool.padX, tool.padY] == 1) result = false;
						else
						{
							if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == 0)
							{
								tempSb.AppendFormat("Attach  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
								msg = tempSb.ToString();
								//msg = "Attach  SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
							}
							else
							{
								tempSb.AppendFormat("InspErr  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
								msg = tempSb.ToString();
								//msg = "InspErr SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
							}

							result = true;
						}
					}
					else
					{
						if (mc.board.working.tmsInfo.mapInfo[tempIndex].I == 0)
						{
							tempSb.AppendFormat("Attach  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
							msg = tempSb.ToString();
							//msg = "Attach  SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
						}
						else
						{
							tempSb.AppendFormat("InspErr  SKIP - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
							msg = tempSb.ToString();
							msg = "InspErr SKIP - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
						}
						
						//strmsg = "Skip Attach : (" + (ix + 1).ToString() + "," + (iy + 1).ToString() + "), MapInfo : " + mc.board.working.tmsInfo.mapInfo[tempIndex].I;
						result = true;
					}
				}
				else
				{
					result = false;
					tempSb.AppendFormat("Convert  ERR - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
					msg = tempSb.ToString();
					//msg = "Convert  ERR - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
				}
			}
			catch
			{
				result = false;
				tempSb.AppendFormat("Convert FAIL - X[{0}]], Y[{1}]", (ix + 1), (iy + 1));
				msg = tempSb.ToString();
				//msg = "Convert FAIL - X[" + (ix + 1).ToString() + "], Y[" + (iy + 1).ToString() + "]";
			}
		}

		public bool wastedonestop; // pick-up단계에서 retry도중 stop시키기 위한 flag
		bool traytypedisplay;
		bool prelaseron;

		public FormUserMessage userMessageBox  = new FormUserMessage();

		public bool stepCycleExit = false;
		public bool cycleMode = false;
		public int padCount = 0;
        public int pickedPosition = 0;
        public bool[] retValue1 = new bool[mc.activate.headCnt];
        public bool[] retValue2 = new bool[mc.activate.headCnt];

		public void control()
		{
			if (!req) return;
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF1, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF2, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF3, false, out ret.message);
					mc.OUT.SF.TUBE_BLOW(UnitCodeSF.SF4, false, out ret.message);
					sqc++; break;
				case 1:
					if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_GANTRY_NOT_READY); break; }
					//tool.F.kilogram(mc.para.HD.moving_force, out ret.message); if (ret.message != RetMessage.OK) { ioCheck(sqc, ret.message); break; }
					sqc++; break;
				case 2:
					cycleMode = false;
					mc.hd.tool.placeForceMean = 0;
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					if (reqMode == REQMODE.STEP) { cycleMode = true; sqc = SQC.STEP; break; }
					if (reqMode == REQMODE.PICKUP) { sqc = SQC.PICKUP; break; }
					if (reqMode == REQMODE.WASTE) { sqc = SQC.WASTE; break; }
					if (reqMode == REQMODE.SINGLE) { sqc = SQC.SINGLE; break; }
					if (reqMode == REQMODE.PRESS) { sqc = SQC.PRESS; break; }
					if (reqMode == REQMODE.AUTO) { sqc = SQC.AUTO; break; }
					//if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY; break; }
					if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY_TEST; break; }
					if (reqMode == REQMODE.JIG_PICKUP) { sqc = SQC.JIG_PICKUP; break; }
					if (reqMode == REQMODE.JIG_HOME) { sqc = SQC.JIG_HOME; break; }
					if (reqMode == REQMODE.JIG_PLACE) { sqc = SQC.JIG_PLACE; break; }
					if (reqMode == REQMODE.COMPEN_FORCE) { sqc = SQC.COMPEN_FORCE; break; }
					if (reqMode == REQMODE.COMPEN_FLAT) { sqc = SQC.COMPEN_FLAT; break; }
                    if (reqMode == REQMODE.COMPEN_FLAT_TEST) { sqc = SQC.COMPEN_FLAT + 1; break; }          // COMPEN_FLAT 에서는 ON/OFF 유무를 검사하기 때문에 테스트 모드를 따로 생성.
					if (reqMode == REQMODE.COMPEN_REF) { sqc = SQC.COMPEN_REF; break; }
                    if (reqMode == REQMODE.CHECK_ATTACH_TILT) { sqc = SQC.CHECK_ATTACH_TILT; break; }

					errorCheck(ERRORCODE.HD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_GANTRY_LIST_NONE); break;

				#region HOMING
                case SQC.HOMING:
                    if (dev.NotExistHW.ZMP) { mc.init.success.HD = true; sqc = SQC.STOP; break; }
                    mc.init.success.HD = false;
                    sqc++; break;
                case SQC.HOMING + 1:
                    tool.X.abort(out ret.message);
                    tool.Y.abort(out ret.message);
                    tool.Y2.abort(out ret.message);
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].abort(out ret.message);
                        tool.T[i].abort(out ret.message);
                    }

                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].setPosition(0, out ret.message);
                        tool.Z[i].setCommandPosition(0, out ret.message);
                        tool.T[i].setPosition(0, out ret.message);
                        tool.T[i].setCommandPosition(0, out ret.message);
                    }
                    sqc++; break;
                case SQC.HOMING + 2:
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.homingZ[i].req = true;
                    }
                    sqc++; break;
                case SQC.HOMING + 3:
                    if (tool.mRUNING(ref tool.homingZ)) break;
                    if (tool.mERROR(ref tool.homingZ)) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-Z Axis Homing Response Error"); break; }
                    tool.homingX.req = true;
                    tool.homingY.req = true;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.homingT[i].req = true;
                    }
                    sqc++; break;
                case SQC.HOMING + 4:
                    if (tool.mRUNING(ref tool.homingT)) break;
                    if (tool.mERROR(ref tool.homingT))
                    {
                        Esqc = sqc; sqc = SQC.HOMING_ERROR;
                        mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-T Axis Homing Response Error");
                        break;
                    }
                    sqc++; break;
                case SQC.HOMING + 5:
                    if (tool.homingX.RUNING || tool.homingY.RUNING) break;
                    if (tool.homingX.ERROR || tool.homingY.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> HEAD-X or Y Axis Homing Response Error"); break; }
                    dwell.Reset();
                    sqc++; break;
                case SQC.HOMING + 6:
                    if (dwell.Elapsed < 100) break;

                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].move(tool.tPos.z[i].XY_MOVING, mc.speed.slow, out ret.message);
                    }
                    tool.X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message);
                    tool.Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message);
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.T[i].move(tool.tPos.t[i].ZERO, mc.speed.slowRPM, out ret.message);
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.HOMING + 7:
                    if (dwell.Elapsed > 10000) { Esqc = sqc; sqc = SQC.HOMING_ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Home> Head Origin Moving Timeout Error"); break; }
                    tool.X.AT_IDLE(out ret.b1, out ret.message);
                    tool.Y.AT_IDLE(out ret.b2, out ret.message);
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].AT_IDLE(out ret.b3, out ret.message);
                        tool.T[i].AT_IDLE(out ret.b4, out ret.message);
                    }
                    if (!ret.b1 || !ret.b2 || !ret.b3 || !ret.b4) break;

                    mc.init.success.HD = true;
                    sqc = SQC.STOP; break;

                case SQC.HOMING_ERROR:
                    if (tool.homingX.RUNING || tool.homingY.RUNING || tool.mRUNING(ref tool.homingZ) || tool.mRUNING(ref tool.homingZ)) break;//tool.homingZ.RUNING || tool.homingT.RUNING) break;
                    tool.X.motorEnable(false, out ret.message);
                    tool.Y.motorEnable(false, out ret.message);
                    tool.Y2.motorEnable(false, out ret.message);
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].motorEnable(false, out ret.message);
                        tool.T[i].motorEnable(false, out ret.message);
                    }
                    sqc = SQC.ERROR; break;
				#endregion

				#region DUMY
				case SQC.DUMY:
                    tool.workingZ = 0;
                    tool.singleCycleHead = (int)UnitCodeHead.INVALID;
                    mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.EMPTY);
                    mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.EMPTY);
                            
                    if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD1)
                        mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.NO_DIE);
                    if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD2)
                        mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.NO_DIE);

					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
					if (!ret.b)
					{
						mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 8; break; }
						mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 8; break; }
						mc.board.working.tmsInfo.TrayType = (int)TRAY_TYPE.NOMAL_TRAY;
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Dry Run Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					sqc++; break;
				case SQC.DUMY + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.pd.req = true; 
					mc.pd.reqMode = REQMODE.AUTO;
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.DUMY + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //if (tool.remainCount <= mc.para.MT.padCount.y.value) mc.pd.singleUp = true;
					sqc++; break;
				case SQC.DUMY + 3:
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.DUMY + 4:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.DUMY + 5:
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 8; break; }
                    if (order.waste != (int)ORDER.EMPTY)
                    {
                        sqc = SQC.DUMY + 7;
                        break;
                    }
                    mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
					if (!ret.b) { sqc = SQC.DUMY + 8; break; }
					sqc++; break;
				case SQC.DUMY + 6:
					tool.place_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc -= 2; break;
                case SQC.DUMY + 7:
                    tool.place_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (order.bond_done != (int)ORDER.EMPTY) mc.hd.order.set(mc.hd.order.bond_done, (int)ORDER.NO_DIE);
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 8; break; }
                    mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
					if (!ret.b) { sqc = SQC.DUMY + 8; break; }
					sqc = SQC.DUMY + 3; break;
				case SQC.DUMY + 8:
                    tool.place_standby();		// 20140516 : place_home() -> place_standby()
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if (mc.para.ETC.useAutoTiltCheck.value == 1)
                    {
                        sqc = SQC.DUMY + 15;
                        break;
                    }
                    else
                    {
                        mc.pd.req = true;
                        mc.pd.reqMode = REQMODE.READY;
                        sqc = SQC.STOP; break;
                    }

				case SQC.DUMY + 10:
					mc.board.shift(BOARD_ZONE.LOADING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 8; break; }
					mc.board.shift(BOARD_ZONE.WORKING, out ret.b); if (!ret.b) { sqc = SQC.DUMY + 8; break; }
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b,  out tool.remainCount); if (!ret.b) { sqc = SQC.DUMY + 7; break; }
					sqc = SQC.DUMY + 6; break;

                case SQC.DUMY + 15:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.DUMY + 30; break; }
                    
                    // 여기부터 레이저 측정 시작
                    mc.board.padIndex(out tool.padX, out tool.padY, PAD_STATUS.ATTACH_DONE, out ret.b);
                    if (!ret.b) { sqc = SQC.DUMY + 30; break; }
                    else
                    {
                        sqc++; break;
                    }
                case SQC.DUMY + 16:
                    tool.check_PCB_Tilt();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc--;
                    break;
                
                case SQC.DUMY + 30:
                    tool.place_standby();		// 20140516 : place_home() -> place_standby()
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.pd.req = true;
                    mc.pd.reqMode = REQMODE.READY;
                    sqc = SQC.STOP; break;

				#endregion

				#region STEP
				case SQC.STEP:				
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.MAIN.T_BUZZER(true, out ret.message); if (ioCheck(sqc, ret.message)) break;

                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.FAILURE, textResource.MB_SF_TUBE_ERROR);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.OK) break;		// SF 활성화 시킬때까지 대기시키기 위한 스텝
						else sqc = SQC.STEP + 10;		// 종료
						break;
					}

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_LOAD_TMS);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Yes)
					{
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Step Cycle Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					else if (FormUserMessage.diagResult == DIAG_RESULT.No)
					{
						FormPadSelect padSelectDialog = new FormPadSelect((int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
						padSelectDialog.ShowDialog();

						tool.padX = padSelectDialog.retPoint.X;
						tool.padY = padSelectDialog.retPoint.Y;
					}
					else { sqc = SQC.STEP + 10; break; }

					sqc++; break;
				case SQC.STEP + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b)
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_REQ_SF_UP);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Next) sqc += 2;
						else if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
						break;
					}

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_WASTE);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Yes) { sqc++; break; }			// 자재 버리러 감
					else
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_NOWASTE);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.OK) { sqc = SQC.STEP	+ 7;break; }		// 안 버리고 ulc로 이동
						else if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) sqc = SQC.STEP + 10;				// 취소
						break;
					}
				case SQC.STEP + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_REQ_SF_UP);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 3:
					#region mc.sf.req
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.STEP + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					
					userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, String.Format(textResource.MB_HD_CYCLE_MOVE_PICK, mc.sf.workingTubeNumber));
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					// Move Gantry to Stack Feeder Position
					sqc++; break;
				case SQC.STEP + 5:
					tool.home_pickgantry();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_PICK_DOWN);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 6:
					// Step 구분을 해야 한다. Move Gantry
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.STEP + 7:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_ISP_ORIENTATION);
					userMessageBox.ShowDialog();
					if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					sqc++; break;
				case SQC.STEP + 8:
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

					if (!mc.hd.stepCycleExit)
					{
                        userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_ISP_TILT);
						userMessageBox.ShowDialog();
						if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { sqc = SQC.STEP + 10; break; };
					}
					else { sqc = SQC.STEP + 10; break; }

					sqc++; break;
				case SQC.STEP + 9:
					tool.place_laser();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					
					userMessageBox.SetDisplayItems(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_HD_CYCLE_MOVE_STANDBY);
					userMessageBox.ShowDialog();
					sqc++; break;
				case SQC.STEP + 10:
					tool.move_standby();		// 20150516 : move_waste() -> move_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region PICKUP
				case SQC.PICKUP:
                    for (int i = 0; i < mc.activate.headCnt; i++ )
                    {
                        if(i == tool.singleCycleHead) mc.hd.order.set(i, (int)ORDER.NO_DIE);
                        else mc.hd.order.set(i, (int)ORDER.EMPTY);
                    }

					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(tool.singleCycleHead, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b)
                    {
                        mc.hd.order.set(tool.singleCycleHead, (int)ORDER.PICK_SUCESS);
                    }
					sqc++; break;
				case SQC.PICKUP + 1:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 2:
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.PICKUP + 3:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 4:
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PICKUP + 5:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region WASTE
				case SQC.WASTE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.WASTE + 1:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region SINGLE
				case SQC.SINGLE:
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        if (i == tool.singleCycleHead) mc.hd.order.set(i, (int)ORDER.NO_DIE);
                        else mc.hd.order.set(i, (int)ORDER.EMPTY);
                    }

					if (mc.para.mmiOption.manualSingleMode == false)
					{
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Single Attach Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					else
					{
						tool.padX = mc.para.mmiOption.manualPadX;
						tool.padY = mc.para.mmiOption.manualPadY;
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.READY, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Sing Cycle Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
					}
					sqc++; break;
				case SQC.SINGLE + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK(tool.singleCycleHead, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b)
                    {
                        mc.hd.order.set(tool.singleCycleHead, (int)ORDER.PICK_SUCESS);
                    }
					sqc++; break;
				case SQC.SINGLE + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 3:
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.SINGLE + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.pd.req = true;
                    mc.pd.reqMode = REQMODE.AUTO;
					sqc++; break;
				case SQC.SINGLE + 5:
					tool.home_pick();
                    if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.SINGLE + 6:
                    tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
				case SQC.SINGLE + 7:
                    tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.SINGLE + 8:
                    tool.place_standby();    
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region PRESS
				case SQC.PRESS:
					tool.padX = mc.para.mmiOption.manualPadX;
					tool.padY = mc.para.mmiOption.manualPadY;
					sqc++; break;
				case SQC.PRESS + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc += 2; break; }
					sqc++; break;
				case SQC.PRESS + 2:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 3:
					//tool.home_press();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 4:
					tool.place_laser();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.PRESS + 5:
					tool.move_standby();	// 20140516 : move_waste() -> move_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

                #region CHECK_ATTACH_TILT
                case SQC.CHECK_ATTACH_TILT:
                    if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    tool.padX = mc.para.mmiOption.manualPadX;
                    tool.padY = mc.para.mmiOption.manualPadY;
                    mc.pd.req = true; mc.pd.reqMode = REQMODE.AUTO;
                    sqc++; break;
                case SQC.CHECK_ATTACH_TILT + 1:
                    if (mc.pd.RUNING) break;
                    if (mc.pd.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.CHECK_ATTACH_TILT + 2:
                    tool.check_Attach_flatness();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;
                #endregion

				#region AUTO
				case SQC.AUTO:
					tool.ulcfailcount = 0;
                    tool.singleCycleHead = (int)UnitCodeHead.INVALID;
                    tool.workingZ = 0;
					padCount = 0;
                    
					mc.cv.toWorking.checkTrayType(1, out ret.b);    // read from working conveyor information
					if (ret.b)  // cover tray
					{
						if (traytypedisplay != true)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "COVER Tray");
							traytypedisplay = true;
						}
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
						if (!ret.b) { sqc = SQC.AUTO + 15; traytypedisplay = false; break; }
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "AUto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
						break;
					}
					else	// Normal Tray, Normal End Tray
					{
						if (traytypedisplay != true)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, "WORK Tray");
							traytypedisplay = true;
						}
						// 결론적으로 생각해 보면 여기서 찍을 Point가 있는지 없는지 굳이 검사할 필요가 없다. TMS로부터 파일을 정상적으로 읽었는지 여부부터 검사를 진행해야 하므로 이 단계에서의 검사는 이미 의미를 상실했다.
						// Comment로 막았다가 풀음..한 Step씩 Shift가 발생함.
                        mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);		// Tuple에서 READY string을 찾는 함수. 이젠 여기서 Error를 발생시키면 안된다. Loading단계에서 이미 모두 현재 pad status를 update한 상태에서 들어온다.
																						// READY를 찾을 수 없다면 SKIP 상태이므로 배출을 해야 한다.
																						// 어차피 TMS 파일에서도 동일한 검사를 밑에서 수행하므로 굳이 여기서 할 필요는 없는데..만약 있다면 밑의 Sequence를 타지 않고, 
						traytypedisplay = false;
						sqc++;
					}
					break;
				case SQC.AUTO + 1:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; mc.log.debug.write(mc.log.CODE.ERROR, "Tool is RUNNING!"); break; } 
                    sqc += 2;
					break;		// 있으면 버리기

				case SQC.AUTO + 2:		// 최초 부품을 버리는 step은 나중에 필요에 의해 사용하도록 한다.
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.AUTO + 3:
					#region mc.sf.req
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTO + 12; break; }
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;			
                    mc.sf.req = true;
					#endregion
					sqc++; break;
				case SQC.AUTO + 4:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.AUTO + 20; break;
					
				case SQC.AUTO + 5:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTO + 11; break; }			// 집기 전에 정지하도록 추가
					checkTMSFileRead(out ret.b);
					if (ret.b)
					{
						// TMS File을 정상적으로 읽은 뒤에 현재 Point에 대해서 Skip Point인지 검사한다.
						checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
						if (ret.b1)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
                            mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
							if (!ret.b)
							{
								mc.commMPC.SVIDReport(); //20130624. kimsong.
								mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
								sqc = SQC.AUTO + 12; break;
							}
							else break;
						}
						else
						{
							mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 0);
                            mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.EMPTY);
                            mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.EMPTY);

                            if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD1)
                            {
                                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                                if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.PICK_SUCESS);
                                else mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.NO_DIE);
                            }
                            if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD2)
                            {
                                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                                if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.PICK_SUCESS);
                                else mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.NO_DIE);
                            }

                            mc.pd.req = true;
                            mc.pd.reqMode = REQMODE.AUTO;
                            sqc++;
                            
							break;
						}
					}
					else
					{
						if (mc.full.reqMode == REQMODE.AUTO && (mc.swcontrol.hwCheckSkip & 0x02) == 0)
						{
							errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_SG_TMS_READ_ERROR); break;
						}
						else
						{
                            mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.EMPTY);
                            mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.EMPTY);

                            if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD1)
                            {
                                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                                if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.PICK_SUCESS); 
                                else mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.NO_DIE);
                            }
                            if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD2)
                            {
                                mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                                if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.PICK_SUCESS);
                                else mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.NO_DIE);
                            }
                                
                            mc.pd.req = true;
                            mc.pd.reqMode = REQMODE.AUTO;
							sqc++;
							break;
						}
					}
				case SQC.AUTO + 6:
					wastedonestop = false;
					mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);					
					tool.home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
						Esqc = sqc; 
						sqc = SQC.ERROR; 
						break; 
					}
					if (wastedonestop) { sqc = SQC.AUTO + 12; break; }
					sqc++; break;
				case SQC.AUTO + 7:
					tool.pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
						Esqc = sqc; 
						sqc = SQC.ERROR; 
						break; 
					}
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTO + 11; break; }			// 집고 나서 정지 가능토록 추가
					sqc++; break;
				case SQC.AUTO + 8:
					mc.hd.tool.ulcfailchecked = false;
					tool.ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.AUTO + 11; break; }
					if (mc.hd.tool.ulcfailchecked)
					{
						if (mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
						{
							// move to waste position
							mc.hd.tool.ulcfailcount++;
							sqc = SQC.AUTO + 18; break;
						}
					}
					sqc++; break;
				case SQC.AUTO + 9:
					// 여기서 Remaining Point들을 계산한다.
					mc.board.padIndex(out tool.padX, out tool.padY, out ret.b, out tool.remainCount);
					if (!ret.b)
					{
						// 남아 있는 작업 Point가 없으면, 종료 신호를 보내고 Sequence를 정지한다.
						mc.commMPC.SVIDReport(); //20130624. kimsong.
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
						sqc = SQC.AUTO + 11; break;
					}
					else
					{
						// 가져온 Point에 대해서 Skip할 Point인지 검사한다.
						checkNoExistPad(tool.padX, tool.padY, out ret.b1, out ret.s1);
						if (ret.b1)
						{
							mc.log.debug.write(mc.log.CODE.TRACE, ret.s1);
							// 새로운 Point가 Skip Point인 경우, SKIP으로 표시한다.
                            mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
							break;  // 현재 Step을 계속 진행한다.
						}
						else
						{
							// 작업할 Point인 경우 다음 Step으로 이동한다.
							sqc++; break;
						}
					}
						
				case SQC.AUTO + 10:
					wastedonestop = false;

					// 혹시 모르니 한번 더 검사하고 꺼주고..
					mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);

					tool.place_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (wastedonestop) { sqc = SQC.AUTO + 12; break; }
                    //if (tool.remainCount <= mc.para.MT.padCount.y.value) mc.pd.singleUp = true;
                    //mc.pd.req = true; 
                    //mc.pd.reqMode = REQMODE.AUTO;
					sqc -= 3; break;
				case SQC.AUTO + 11:
					tool.place_standby();		// 20140516 : place_home() -> place_standby()
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.ATTACH_WORK, 1);
					sqc++; break;
				case SQC.AUTO + 12:
					sqc = SQC.STOP; break;
				case SQC.AUTO + 15:
					mc.cv.toWorking.checkTrayType(1, out ret.b);
					if (!ret.b)
					{
                        mc.board.padStatus(BOARD_ZONE.WORKING, tool.padX, tool.padY, PAD_STATUS.SKIP, out ret.b); if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "Auto Mode Error", ALARM_CODE.E_CONV_WORK_TRAY_DATA_SAVE_ERROR); break; }
						mc.board.padIndex(out tool.padX, out tool.padY, out ret.b);
						if (!ret.b)
						{
							mc.commMPC.SVIDReport(); //20130624. kimsong.
							mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
							sqc = SQC.AUTO + 11; break;
						}
						else break;
					}
					else // cover tray
					{
						mc.commMPC.SVIDReport(); //20130624. kimsong.
						mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_FINISHED);
						sqc = SQC.STOP; break;
					}
				case SQC.AUTO + 18:
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc=SQC.AUTO + 6; break;

				case SQC.AUTO + 20:
					if ((int)mc.para.CV.trayReverseUse.value == (int)ON_OFF.ON)
					{
						sqc++; break;
					}
					else
					{
						if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc = SQC.AUTO + 22;	// reverse check2로 이동
						else sqc = SQC.AUTO + 23; 		// ref check로 이동
						break;
					}
				case SQC.AUTO + 21:
					tool.check_trayreverse();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{ 
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
						break; 
					}
					if ((int)mc.para.CV.trayReverseUse2.value == (int)ON_OFF.ON) sqc++;	// reverse check2로 이동
					else sqc = SQC.AUTO + 23; 		// ref check로 이동
					break;
				case SQC.AUTO + 22:
					tool.check_trayreverse2();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_TRAY_REVERSE);
						break;
					}
					sqc++; break;
				case SQC.AUTO + 23:
					if ((int)mc.para.ETC.refCompenUse.value == (int)ON_OFF.ON && mc.full.boardCount % (int)mc.para.ETC.refCompenTrayNum.value == 0 && mc.para.runInfo.refCompenStartFlag == true)
					{
						tool.refcheckcount = 0;
						sqc++; break;
					}
					else
					{
						sqc = SQC.AUTO + 5; break;
					}
				case SQC.AUTO + 24:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR) 
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
						break; 
					}
					mc.para.runInfo.refCompenStartFlag = false;
					sqc = SQC.AUTO + 5; break;

				#endregion

				#region DUMY_TEST
				case SQC.DUMY_TEST:
					if ((int)mc.para.ETC.forceCompenUse.value == 1)
					{
						sqc++; break;
					}
					else
					{
						sqc += 2; break;
					}
				case SQC.DUMY_TEST + 1:
                    //tool.check_force();
                    //if (tool.RUNING) break;
                    //if (tool.ERROR)
                    //{
                    //    Esqc = sqc; sqc = SQC.ERROR;
                    //    //errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
                    //    break;
                    //}
					sqc++; break;
				case SQC.DUMY_TEST + 2:
					if ((int)mc.para.ETC.flatCompenUse.value == 1)
					{
						sqc++; break;
					}
					else
					{
						sqc += 3; break;
					}
				case SQC.DUMY_TEST + 3:
					tool.check_flatness();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						//errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
						break;
					}
					sqc++; break;
                case SQC.DUMY_TEST + 4:
                    tool.check_Pedestal_flatness();
                    if (tool.RUNING || mc.pd.RUNING) break;
                    if (tool.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        //errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
                    }
                    if (mc.pd.ERROR)
                    {
                        Esqc = sqc; sqc = SQC.ERROR;
                        //errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
                    }
                    sqc++; break;
				case SQC.DUMY_TEST + 5:
					if ((int)mc.para.ETC.refCompenUse.value == 1)
					{
						tool.refcheckcount = 0;
						sqc++; break;
					}
					else
					{
						sqc = SQC.DUMY; break;
					}
				case SQC.DUMY_TEST + 6:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR)
					{
						Esqc = sqc; sqc = SQC.ERROR;
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
						break;
					}
					sqc = SQC.DUMY; break;
				#endregion

				#region COMPEN_FORCE
				case SQC.COMPEN_FORCE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.para.ETC.forceCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.COMPEN_FORCE+1:
					//tool.check_force();
                    //if (tool.RUNING) break;
                    //if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.para.runInfo.forceCompenStartFlag = false;
					sqc = SQC.STOP; break;
				#endregion

				#region COMPEN_FLAT
				case SQC.COMPEN_FLAT:
					if (mc.para.ETC.flatCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
                    tool.singleCycleHead = (int)UnitCodeHead.INVALID;
                    mc.para.runInfo.flatCompenStartFlag = false;
					sqc++; break;
				case SQC.COMPEN_FLAT + 1:
                    mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.EMPTY);
                    mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.EMPTY);
                    if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD1)
                    {
                        mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message);
                        if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.PICK_FAIL);
                        else mc.hd.order.set((int)UnitCodeHead.HD1, (int)ORDER.NO_DIE);
                    }
                    if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX || mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD2)
                    {
                        mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD2, out ret.b, out ret.message);
                        if (ret.b) mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.PICK_FAIL);
                        else mc.hd.order.set((int)UnitCodeHead.HD2, (int)ORDER.NO_DIE);
                    }
                    sqc++; break;
				case SQC.COMPEN_FLAT + 2:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
					tool.move_waste();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.COMPEN_FLAT + 3:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    tool.check_flatness();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.COMPEN_FLAT + 4:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    tool.check_Pedestal_flatness();
                    if (tool.RUNING) break;
                    if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;

				#endregion

				#region COMPEN_REF
				case SQC.COMPEN_REF:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.para.ETC.refCompenUse.value == (int)ON_OFF.OFF) { sqc = SQC.STOP; break; }
					tool.refcheckcount = 0;
					sqc++; break;
				case SQC.COMPEN_REF + 1:
					tool.check_reference();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region JIG_PICKUP
				case SQC.JIG_PICKUP:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_PICKUP + 1:
					tool.jig_home_pick();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.JIG_PICKUP + 2:
					tool.jig_pick_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region JIG_HOME
				case SQC.JIG_HOME:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_HOME + 1:
					tool.jig_move_home();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion

				#region JIG_PLACE
				case SQC.JIG_PLACE:
					if (tool.RUNING) { Esqc = sqc; sqc = SQC.ERROR; break; }
					mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { sqc = SQC.STOP; break; }
					sqc++; break;
				case SQC.JIG_PLACE + 1:
					tool.jig_ulc_place();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case SQC.JIG_PLACE + 2:
					tool.jig_place_ulc();
					if (tool.RUNING) break;
					if (tool.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;
				#endregion


				case SQC.ERROR:
                    // Derek 수정예정

					//if (mc.pd.RUNING) break;
                    //if (mc.init.success.PD)
                    //{
                    //    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    //}
					//string str = "HD Esqc " + Esqc.ToString();
					//EVENT.statusDisplay(str);

                    mc.pd.SetZDown();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD Esqc {0}", Esqc));
					sqc = SQC.STOP; break;

				case SQC.STOP:
					if (mc.init.success.HD == false) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.STOP + 1:
					if (dwell.Elapsed > 5000) { mc.init.success.HD = false; sqc++; break; }
					tool.X.AT_IDLE(out ret.b1, out ret.message);
					tool.Y.AT_IDLE(out ret.b2, out ret.message);
                    if (!ret.b1 || !ret.b2) break;
                    sqc++; break;
                case SQC.STOP + 2:
                    if (dwell.Elapsed > 5000) { mc.init.success.HD = false; sqc++; break; }
                    
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        tool.Z[i].AT_IDLE(out retValue1[i], out ret.message);
                        tool.T[i].AT_IDLE(out retValue2[i], out ret.message);
                    }
                    if (!retValue1[0] || !retValue2[0] || !retValue1[1] || !retValue2[1]) break;
					sqc++; break;
				case SQC.STOP + 3:
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}
		}

		public void test1()
		{
			//tool.Z.move(tool.tPos.z.REF0 + 200, 0.2, 3, -0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.REF0, 0.01, 0.4, out ret.retMessage);
			//Thread.Sleep(10);
			//tool.Z.move(tool.tPos.z.XY_MOVING - 200, 0.2, 3, 0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.XY_MOVING, 0.01, 0.4, out ret.retMessage);
		}
		public void test2()
		{
			//tool.Z.move(tool.tPos.z.REF0 + 200, 0.3, 1.5, -0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.REF0, 0.01, 0.4, out ret.retMessage);
			//Thread.Sleep(10);
			//tool.Z.move(tool.tPos.z.XY_MOVING - 200, 0.2, 3, 0.01, out ret.retMessage);
			//tool.Z.move(tool.tPos.z.XY_MOVING, 0.01, 0.4, out ret.retMessage);
		}
		public void checkMovingZPos()
		{
			double curPos;
			RetMessage retMsg;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                tool.Z[i].actualPosition(out curPos, out retMsg);
                if ((tool.tPos.z[i].XY_MOVING - curPos) > 1000)
                {
                    mc.log.debug.write(mc.log.CODE.WARN, String.Format("move Z to SAFTY position : {0}[um] -> {1}", Math.Round(curPos), tool.tPos.z[i].XY_MOVING));
                    tool.jogMove(i, tool.tPos.z[i].XY_MOVING, mc.speed.slow, out retMsg);
                }
            }
		}
	}
	public class classHeadTool : TOOL_CONTROL
	{
		public mpiMotion X = new mpiMotion();
        public mpiMotion Y = new mpiMotion();
        public mpiMotion Y2 = new mpiMotion();
        public mpiMotion[] Z = new mpiMotion[mc.activate.headCnt];
        public mpiMotion[] T = new mpiMotion[mc.activate.headCnt];

        public double _X = new double();
        public double _Y = new double();
        public double _Z = new double();
        public double _T = new double();
		
        public classForce F = new classForce();

		public captureHoming homingX = new captureHoming();
		public gantryHoming homingY = new gantryHoming();
        public captureHoming[] homingZ = new captureHoming[mc.activate.headCnt];
        public captureHoming[] homingT = new captureHoming[mc.activate.headCnt];
		//public capturZPhaseHoming homingT = new capturZPhaseHoming();

		public cameraTrigger triggerHDC = new cameraTrigger();
		public cameraTrigger triggerULC = new cameraTrigger();

		public classHeadCamearPosition cPos = new classHeadCamearPosition();
		public classHeadToolPosition tPos = new classHeadToolPosition();
		public classHeadLaserPosition lPos = new classHeadLaserPosition();

        public int workingZ = 0;

        double comparePos = 3000;

		PAD_STATUS placeResult;

        public int remainCount;

        public classHeadTool()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i] = new mpiMotion();
                T[i] = new mpiMotion();
                homingZ[i] = new captureHoming();
                homingT[i] = new captureHoming();
            }
        }

		public bool isActivate
		{
			get
			{
				if (!X.isActivate) return false;
				if (!Y.isActivate) return false;
				if (!Y2.isActivate) return false;
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (!Z[i].isActivate) return false;
                    if (!T[i].isActivate) return false;
                }

				if (!homingX.isActivate) return false;
				if (!homingY.isActivate) return false;
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (!homingZ[i].isActivate) return false;
                    if (!homingT[i].isActivate) return false;
                }

				if (!triggerHDC.isActivate) return false;
				if (!triggerULC.isActivate) return false;
				return true;
			}
		}
		public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig[] z, axisConfig[] t, axtOut axtOutHDC, axtOut axtOutULC, out RetMessage retMessage)
		{
			if (!X.isActivate)
			{
				X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!Y.isActivate)
			{
				Y.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			if (!Y2.isActivate)
			{
				Y2.activate(y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y2, 0, retMessage)) return;
			}
            for (int i = 0; i < mc.activate.headCnt; i ++)
            {
                //수정예정
                if (!Z[i].isActivate)
                {
                    Z[i].activate(z[i], out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
                }
                if (!T[i].isActivate)
                {
                    T[i].activate(t[i], out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
                }
            }
			

			if (!homingX.isActivate)
			{
				homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
			}
			if (!homingY.isActivate)
			{
				homingY.activate(y, y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
			}
			//if (!homingY2.isActivate)
			//{
			//    homingY2.activate(x, out ret.retMessage); if (mpiCheck(MPIAxisCODE.Y2, ret.retMessage)) return;
			//}
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                if (!homingZ[i].isActivate)
                {
                    homingZ[i].activate(z[i], out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
                }
                if (!homingT[i].isActivate)
                {
                    homingT[i].activate(t[i], out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
                }
            }
			if (!triggerHDC.isActivate)
			{
				triggerHDC.activate(axtOutHDC);
			}
			if (!triggerULC.isActivate)
			{
				triggerULC.activate(axtOutULC);
			}
			retMessage = RetMessage.OK;
		}
        public void activate(axisConfig x, axisConfig y, axisConfig y2, axisConfig[] z, axisConfig[] t, mpiNodeDigtalOut gmsOutHDC, mpiNodeDigtalOut gmsOutULC, out RetMessage retMessage)
        {
            if (!X.isActivate)
            {
                X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
            }
            if (!Y.isActivate)
            {
                Y.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
            }
            if (!Y2.isActivate)
            {
                Y2.activate(y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y2, 0, retMessage)) return;
            }
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                //수정예정
                if (!Z[i].isActivate)
                {
                    Z[i].activate(z[i], out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
                }
                if (!T[i].isActivate)
                {
                    T[i].activate(t[i], out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
                }
            }


            if (!homingX.isActivate)
            {
                homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
            }
            if (!homingY.isActivate)
            {
                homingY.activate(y, y2, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
            }
            //if (!homingY2.isActivate)
            //{
            //    homingY2.activate(x, out ret.retMessage); if (mpiCheck(MPIAxisCODE.Y2, ret.retMessage)) return;
            //}
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                if (!homingZ[i].isActivate)
                {
                    homingZ[i].activate(z[i], out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage)) return;
                }
                if (!homingT[i].isActivate)
                {
                    homingT[i].activate(t[i], out retMessage); if (mpiCheck(UnitCodeAxis.T, 0, retMessage)) return;
                }
            }
            if (!triggerHDC.isActivate)
            {
                triggerHDC.activate(gmsOutHDC);
            }
            if (!triggerULC.isActivate)
            {
                triggerULC.activate(gmsOutULC);
            }
            retMessage = RetMessage.OK;
        }
		public void deactivate(out RetMessage retMessage)
		{
			X.deactivate(out retMessage);
			Y.deactivate(out retMessage);
			Y2.deactivate(out retMessage);
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].deactivate(out retMessage);
                T[i].deactivate(out retMessage);
            }

			homingX.deactivate(out retMessage);
			homingY.deactivate(out retMessage);
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                homingZ[i].deactivate(out retMessage);
                homingT[i].deactivate(out retMessage);
            }
		}

		public int padX, padY;
		#region jogMove ...
		public void jogMove(double posX, double posY, out RetMessage retMessage, bool saftyMode = true)
		{
            //x, y이므로 multiHead의 Z axis를 전부 Up
            if (saftyMode)
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    #region endcheck
                    dwell.Reset();
                    while (true)
                    {
                        mc.idle(10);
                        if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                        Z[i].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                        if (ret.b) break;
                    }
                    dwell.Reset();
                    while (true)
                    {
                        mc.idle(10);
                        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                        Z[i].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                        if (ret.b) break;
                    }
                    #endregion
                }
            }
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogFastMove(int headNum, double posX, double posY, out RetMessage retMessage)
		{
            //x, y이므로 multiHead의 Z axis를 전부 Up
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                #region endcheck
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                #endregion
            }
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogMove(int headNum, double posX, double posY, double posT, out RetMessage retMessage)
		{
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                #region endcheck
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                #endregion
            }
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			T[headNum].move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}

        public void jogMove(double posX, double posY, double posT1, double posT2, out RetMessage retMessage)
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                #region endcheck
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                #endregion
            }
            X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            T[(int)UnitCodeHead.HD1].move(posT1, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            T[(int)UnitCodeHead.HD2].move(posT2, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            #region endcheck
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[(int)UnitCodeHead.HD1].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[(int)UnitCodeHead.HD2].AT_TARGET(out ret.b4, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b1 && ret.b2 && ret.b3 && ret.b3) break;
            }
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[(int)UnitCodeHead.HD1].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[(int)UnitCodeHead.HD2].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b1 && ret.b2 && ret.b3) return;
            }
            #endregion
        FAIL:
            mc.init.success.HD = false;
        }
        public void jogMove(int headNum, double posX, double posY, double posZ, double posT, out RetMessage retMessage)
		{
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                #region endcheck
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z[i].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                #endregion
            }
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            T[headNum].move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			#endregion
            Z[headNum].move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogMove(int headNum, double posZ, axisMotionSpeed speed, out RetMessage retMessage)
		{
            Z[headNum].move(posZ, speed, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void checkZMoveEnd(int headNum, out RetMessage retMessage)
		{
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
		public void jogMove(int headNum, double posZ, out RetMessage retMessage)
		{
            Z[headNum].move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogMovePlus(int headNum, double posZ, out RetMessage retMessage)
        {
            Z[headNum].movePlus(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            #region endcheck
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b) break;
            }
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b) return;
            }
            #endregion
        FAIL:
            mc.init.success.HD = false;
        }
        public void jogMoveXY(double posX, double posY, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				X.AT_ERROR(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_ERROR(out ret.b4, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) break;
				if (ret.b3 || ret.b4)
				{
					X.eStop(out retMessage);
					Y.eStop(out retMessage);
					retMessage = RetMessage.AXIS_ERROR;
					goto FAIL;
				}
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				X.AT_ERROR(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_ERROR(out ret.b4, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2) return;
				if (ret.b3 || ret.b4)
				{
					X.eStop(out retMessage);
					Y.eStop(out retMessage);
					retMessage = RetMessage.AXIS_ERROR;
					goto FAIL;
				}
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogMoveXYZ(int headNum, double posX, double posY, double posZ, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            Z[headNum].move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;

			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                Z[headNum].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                Z[headNum].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogMoveXYT(int headNum, double posX, double posY, double posT, out RetMessage retMessage)
		{
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            T[headNum].move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}
        public void jogPlusMoveT(int headNum, double plusT, out RetMessage retMessage)
		{
            T[headNum].movePlus(plusT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                T[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                T[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}

        public void jogMoveZXYT(int headNum, double posX, double posY, double posZ, double posT, out RetMessage retMessage)
		{
            Z[headNum].move(posZ, mc.speed.homing, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                Z[headNum].AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b) break;
			}
			#endregion
			X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            T[headNum].move(posT, mc.speed.slowRPM, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
			#region endcheck
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 50000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) break;
			}
			dwell.Reset();
			while (true)
			{
				mc.idle(10);
				if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
				X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                T[headNum].AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
				if (ret.b1 && ret.b2 && ret.b3) return;
			}
			#endregion
		FAIL:
			mc.init.success.HD = false;
		}

		public bool ServoState
		{
			get
			{
				X.MOTOR_ENABLE(out ret.b1, out ret.message);
				Y.MOTOR_ENABLE(out ret.b2, out ret.message);
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    Z[i].MOTOR_ENABLE(out ret.b3, out ret.message); if (!ret.b3) break;
                    T[i].MOTOR_ENABLE(out ret.b4, out ret.message); if (!ret.b4) break;
                }
				
				if (ret.b1 == true && ret.b2 == true && ret.b3 == true && ret.b4 == true) return true;
				else return false;
			}
		}

		public void motorDisable(out RetMessage retMessage)
		{
			mc.init.success.HD = false;
			X.motorEnable(false, out retMessage);
			Y.motorEnable(false, out retMessage);
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].motorEnable(false, out retMessage);
                T[i].motorEnable(false, out retMessage);
            }

			X.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
                T[i].motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            }
		}
		public void motorEnable(out RetMessage retMessage)
		{
			X.reset(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.reset(out retMessage); if (retMessage != RetMessage.OK) return;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].reset(out retMessage); if (retMessage != RetMessage.OK) return;
                T[i].reset(out retMessage); if (retMessage != RetMessage.OK) return;
            }
			mc.idle(100);
			X.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
			Y.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
                T[i].clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
            }
			mc.idle(100);
			X.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
			Y.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
                T[i].motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
            }
			// also, check homing status
			//mc.init.success.HD = true;

		}
		public void motorAbort(out RetMessage retMessage)
        {
            mc.init.success.HD = false;
            X.abort(out retMessage);
            Y.abort(out retMessage);
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].abort(out retMessage);
                T[i].abort(out retMessage);
            }

            X.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            Y.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].abort(out retMessage); if (retMessage != RetMessage.OK) return;
                T[i].abort(out retMessage); if (retMessage != RetMessage.OK) return;
            }
        }

		public void actualPosition_AxisT(int headCnt, out double position, out RetMessage retMessage)
		{
			T[headCnt].actualPosition(out position, out retMessage);
		}
        public void actualPosition_AxisZ(int headCnt, out double position, out RetMessage retMessage)
		{
            Z[headCnt].actualPosition(out position, out retMessage);
		}
		#endregion

		#region tool control
		//public UnitCodeSF tubeNum = UnitCodeSF.SF1;
		public int tubePickCount = 0;
		public int tubePickMaxCount = 10;
		double posZ, posZ1;
		double levelS1, levelS2, levelD1, levelD2;
		double velS1, velS2, accS1, accS2, velD1, velD2, accD1, accD2;
		double delayS1, delayS2, delayD1, delayD2, delay;
		double rateX, rateY;
		double placeX, placeY, placeT;

        //public double clacULCX, clacULCY, clacULCT, clacULCW, clacULCH;
        //public double[] totalULCX = new double[2];
        //public double[] totalULCY = new double[2];
        //public double[] totalULCT = new double[2];

        public double[] calcULCX = new double[2];
        public double[] calcULCY = new double[2];
        public double[] calcULCT = new double[2];
        //public double[] calcULCW = new double[2];
        //public double[] calcULCH = new double[2];

        public double ulcP1X;// = new double[2];
        public double ulcP1Y;// = new double[2];
        public double ulcP1T;// = new double[2];

        public double ulcP2X;// = new double[2];
        public double ulcP2Y;// = new double[2];
        public double ulcP2T;// = new double[2];
        
        public double hdcX, hdcY, hdcT;
        public double hdcP1X, hdcP1Y, hdcP1T_1, hdcP1T_2, hdcPassScoreP1;
        public double hdcP2X, hdcP2Y, hdcP2T_1, hdcP2T_2, hdcPassScoreP2;

        public double hdcP1Score, hdcP2Score, hdcP1PassScore, hdcP2PassScore;

		double tmpX, tmpY, tmpT;
		public double ulcWDif, ulcHDif;

		public double hdcResult;
		public double fidPX, fidPY, fidPD;
		public double trayReversePX, trayReversePY, trayReversePT;
        private bool for_break;
		
		public void move_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0; 
					sqc++; break;
				case 1:
                    //수정예정
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 2:
					if (!Z_AT_TARGET_ALL()) break;
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					dwell.Reset();
					sqc++; break;
				case 3:
                    if (!Z_AT_DONE_ALL()) break;
					sqc++; break;
				case 4:
					Y.move(cPos.y.REF0, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.REF0, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 5:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 6:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc++; break;
				case 7:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD move_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void move_waste()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    if (singleCycleHead == (int)UnitCodeHead.INVALID) { if (mc.hd.order.waste == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; } }
                    else
                    {
                        mc.hd.order.set(singleCycleHead, (int)ORDER.PICK_FAIL);
                    }

					sqc = 10; break;

				case 10:
                    //수정예정
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
			        {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 11:
                    if (!Z_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL()) break;
					if (mc.para.ETC.useWasteCountLimit.value == 1 && mc.para.ETC.wasteCount.value >= mc.para.ETC.wasteCountLimit.value)
					{
						sqc = 20;
						break;
					}
					X.commandPosition(out ret.d1, out ret.message);if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(tPos.x[mc.hd.order.waste].WASTE - ret.d1) > 50000 || Math.Abs(tPos.y[mc.hd.order.waste].WASTE - ret.d2) > 50000)
					{
                        X.move(tPos.x[mc.hd.order.waste].WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.waste].WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
                        X.move(tPos.x[mc.hd.order.waste].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.waste].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.IN.HD.VAC_CHK(mc.hd.order.waste, out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.SUC(mc.hd.order.waste, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(mc.hd.order.waste, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (ret.b) mc.para.ETC.wasteCount.value += 1;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(mc.hd.order.waste, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.hd.order.set(mc.hd.order.waste, (int)ORDER.NO_DIE);
					sqc++; break;
				case 16:
                    //if (mc.pd.RUNING || mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
                    if (mc.pd.ERROR && ((mc.hd.reqMode != REQMODE.STEP && mc.hd.reqMode != REQMODE.PICKUP && mc.hd.reqMode != REQMODE.WASTE))) 
                    { sqc = SQC.ERROR; break; }
                    if (mc.hd.order.waste == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
                    else {sqc = 10; break;}

				case 20:
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(mc.para.CAL.standbyPosition.x.value - ret.d1) > 50000 || Math.Abs(mc.para.CAL.standbyPosition.y.value - ret.d2) > 50000)
					{
						X.move(mc.para.CAL.standbyPosition.x.value, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (mc.pd.RUNING) break; // || mc.hd.tool.F.RUNING) break;
					if (mc.pd.ERROR) { sqc = SQC.ERROR; break; } // || mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
					errorCheck(ERRORCODE.HD, sqc, "TrashBin is Full!");
					break;

				case SQC.ERROR:
					//string str = "HD move_waste Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_waste Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
        public void move_standby()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				case 10:
                    //수정예정
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
			        {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 11:
                    if (!Z_AT_TARGET_ALL()) break;
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, "Pedestal 이 Error 상태이기 때문에 준비동작을 완료할 수 없습니다.");
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL()) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(mc.para.CAL.standbyPosition.x.value - ret.d1) > 50000 || Math.Abs(mc.para.CAL.standbyPosition.y.value - ret.d2) > 50000)
					{
						X.move(mc.para.CAL.standbyPosition.x.value, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(mc.para.CAL.standbyPosition.x.value, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(mc.para.CAL.standbyPosition.y.value, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (mc.pd.RUNING) break; // || mc.hd.tool.F.RUNING) break;
					if (mc.pd.ERROR) { sqc = SQC.ERROR; break; } // || mc.hd.tool.F.ERROR) { sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD move_standby Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD move_standby Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		// Step 동작을 위해 home_pickgantry()추가..home_pick()에서 Gantry움직이는 구문을그대로 따서 쓴다. 이 함수는 단지 Step 동작을 구분하기 위해 Gantry를 Pick 영역으로 이동하기 위해 필요한 함수
        // Derek - Step 동작이므로 mc.hd.order.pick check후 전체 Multi Head의 Pick Up 완료 후 빠져나가게 하지 않음
        public void home_pickgantry()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    if (mc.hd.order.pick == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
					sqc = 10; break;


				case 10:
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
			        {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break= true;
                        T[i].move(tPos.t[i].ZERO, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 11:
                    if (!Z_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL()) break;
					sqc = 20; break;
				case 20:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    else mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
                    if (mc.hd.order.pick != (int)ORDER.EMPTY)
                    {
                        Y.move(tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(tPos.x[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET) break;
					if (!Y_AT_TARGET) break;
					sqc = SQC.STOP; break;
				case SQC.ERROR:
					//string str = "HD home_pickgantry Esqc " + Esqc.ToString();
					sqc = SQC.STOP; break;
				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		int pickretrycount;
		bool pickupFailDone;

		public void home_pick()
		{
			#region PICK_SUCTION_MODE.SEARCH_LEVEL_ON
			if (sqc > 30 && sqc < 40 && mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON)
			{
				mc.OUT.HD.SUC(mc.hd.order.pick, out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (!ret.b)
				{
                    Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.pick.suction.level.value)
					{
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    pickToPick = false;
                    if (mc.hd.order.pick == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
					sqc++; break;
				case 1:
					//if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.STEP || mc.hd.reqMode == REQMODE.DUMY) { mc.pd.req = true; mc.pd.reqMode = REQMODE.AUTO; }    // 20131022. Tray 첫 Point에서 underpress되는 현상.
					if (mc.hd.reqMode == REQMODE.SINGLE)
					{
						mc.pd.req = true;
						mc.pd.reqMode = REQMODE.AUTO;   // 20131022
						//if (dev.debug) mc.pd.reqMode = REQMODE.DUMY;
                    }
                    #region pos set
                    if (mc.hd.reqMode == REQMODE.DUMY)
                        posZ = tPos.z[mc.hd.order.pick].DRYRUNPICK(mc.sf.workingTubeNumber);
                    else
                        posZ = tPos.z[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);
                    if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON)
                    {
                        levelS1 = mc.para.HD.pick.search.level.value;
                        delayS1 = mc.para.HD.pick.search.delay.value;
                        velS1 = (mc.para.HD.pick.search.vel.value) / 1000.0;
                        accS1 = mc.para.HD.pick.search.acc.value;
                    }
                    else
                    {
                        levelS1 = 0;
                        delayS1 = 0;
                    }
                    if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON)
                    {
                        levelS2 = mc.para.HD.pick.search2.level.value;
                        delayS2 = mc.para.HD.pick.search2.delay.value;
                        velS2 = (mc.para.HD.pick.search2.vel.value) / 1000;
                        accS2 = mc.para.HD.pick.search2.acc.value;
                    }
                    else
                    {
                        levelS2 = 0;
                        delayS2 = 0;
                    }
                    delay = mc.para.HD.pick.delay.value;
                    pickretrycount = 0;
                    pickupFailDone = false;
                    mc.para.runInfo.startCycleTime();
                    #endregion
                    sqc = 10; break;

                #region case 10 Z.move.XY_MOVING
                case 10:
                    //mc.para.runInfo.startCycleTime();
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 0);
                    if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
                    {
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                        T[i].move(tPos.t[i].ZERO, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (!Z_AT_TARGET_ALL()) break;
                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!Z_AT_DONE_ALL()) break;
                    sqc = 20; break;
                #endregion

                #region case 20 XY.move.PICK
                case 20:
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    else mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;

                    mc.log.workHistory.write("---------------> Start Pick Up(#" + (int)mc.hd.order.pick + ")");
   					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;

                    double targetX = tPos.x[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);
                    double targetY = tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);

                    if (Math.Abs(targetX - ret.d1) > 50000)// || Math.Abs(targetY - ret.d2) > 50000)
                    {
                        //Y.move(targetY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(targetX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        //Y.move(targetY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(targetX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    if (Math.Abs(targetY - ret.d2) > 50000)
                    {
                        Y.move(targetY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Y.move(targetY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    }

                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (!X_AT_TARGET) break;
                    if (!Y_AT_TARGET) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 1);
                    sqc = 30; break;
                #endregion

                #region case 30 Z move down
                case 30:
                    if (mc.sf.RUNING) break;
                    if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if (mc.hd.reqMode == REQMODE.DUMY)
                        posZ = tPos.z[mc.hd.order.pick].DRYRUNPICK(mc.sf.workingTubeNumber);
                    else
                        posZ = tPos.z[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);
                    //mc.hd.tool.F.stackFeedNum = mc.sf.workingTubeNumber;
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICK;

                    mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 0);
                    if (levelS1 != 0)
                    {
                        Z[mc.hd.order.pick].moveCompare(posZ + levelS1 + levelS2, -velS1, Y.config, tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                        Z[mc.hd.order.pick].move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                        if (delayS1 == 0) { sqc += 3; break; }
                    }
                    else
                    {
                        Z[mc.hd.order.pick].moveCompare(posZ + levelS1 + levelS2, Y.config, tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                        sqc += 3; break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 31:
                    if (!Z_AT_TARGET(mc.hd.order.pick)) break;
                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (dwell.Elapsed < delayS1 - 3) break;
                    sqc++; break;
                case 33:
                    if (levelS2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.pick].move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                    if (levelD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 34:
                    if (!Z_AT_TARGET(mc.hd.order.pick)) break;
                    dwell.Reset();
                    sqc++; break;
                case 35:
                    if (dwell.Elapsed < delayS2 - 3) break;
                    sqc++; break;
                case 36:
                    dwell.Reset();
                    sqc++; break;
                case 37:
                    if (!Z_AT_TARGET(mc.hd.order.pick)) break;
                    dwell.Reset();
                    sqc++; break;
                case 38:
                    if (!Z_AT_DONE(mc.hd.order.pick)) break;

                    if (mc.hd.cycleMode)
                    {
                        mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_SUC_HS);
                        mc.hd.userMessageBox.ShowDialog();
                        if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                        mc.hd.stepCycleExit = false;
                    }

                    if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.PICK_LEVEL_ON)
                    {
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 39:
                    if (dwell.Elapsed < delay - 3) break;
                    dwell.Reset();
                    sqc = 40; break;
                #endregion

				#region case 40 suction.check
				case 40:
					mc.para.runInfo.writePickInfo(mc.sf.workingTubeNumber, PickCodeInfo.PICK);
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 켜준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, true, out ret.message);
					}
					if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.OFF) 
                    {
                        sqc = 50; break; 
                    }
					sqc++; break;
				case 41:
					// wait suction check time
					if (dwell.Elapsed > mc.para.HD.pick.suction.checkLimitTime.value)   // 공압 검사 ERROR
					{
						// 여기서 Suction을 OFF하는데, Waste Position으로 움직인 뒤에 Suction OFF해야 한다.
						if (mc.hd.reqMode != REQMODE.AUTO)
						{
                            Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
							errorCheck(ERRORCODE.HD, sqc, "Pick Suction Check Time Limit Error"); break;
						}
                        else
                        {
							if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.ON)
							{
								if ((pickretrycount+1) < (int)mc.para.HD.pick.missCheck.retry.value)
								{	// retry pick up
									// move to waste position
									sqc = 80;
									mc.sf.req = true;
									pickupFailDone = false;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail. FailCnt[{0}]", pickretrycount + 1));
								}
								else
								{
									// 버린 다음에 알람
									pickupFailDone = true;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail", pickretrycount + 1));
									sqc = 80;
									break;
								}
								pickretrycount++;
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							else
							{
								pickupFailDone = true;
                                mc.OUT.HD.SUC(mc.hd.order.pick, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
								mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail"));
								sqc = 80; 
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							break;
						}
					}
					mc.IN.HD.VAC_CHK(mc.hd.order.pick, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) break;
					sqc = 50; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc++; break;
				case 52:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_SUCESS);
                    if (mc.hd.order.pick != (int)ORDER.EMPTY) 
                    {
                        //if (mc.hd.order.pick != (int)ORDER.EMPTY)
                        //{
                            if (remainCount < mc.activate.headCnt)
                            {
                                mc.hd.order.set(mc.hd.order.pick, (int)ORDER.EMPTY);
                                sqc = SQC.STOP;
                            }
                            else
                            {
                                sqc = 100;
                            }
                            break;
                        //}
                    }

                    sqc = SQC.STOP; break;
				#endregion

				#region case 80 move to waste position
				case 80:
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
					}
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					// 쓰레기통으로 갈 때. 요놈도 문제를 발생할 가능성이 보인다.
					//mc.pd.req = true; mc.pd.reqMode = REQMODE.READY;
					dwell.Reset();
					sqc++; break;
				case 82:
                    if (!Z_AT_DONE(mc.hd.order.pick)) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(tPos.x[mc.hd.order.pick].WASTE - ret.d1) > 50000 || Math.Abs(tPos.y[mc.hd.order.pick].WASTE - ret.d2) > 50000)
					{
                        X.move(tPos.x[mc.hd.order.pick].WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.pick].WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
                        X.move(tPos.x[mc.hd.order.pick].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.pick].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.OUT.HD.SUC(mc.hd.order.pick, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 85:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(mc.hd.order.pick, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case 86:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						//mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;	//SF Z축은 Homing Sequence를 따르는데 Homing 도중 Error로 Stop이 걸리기 때문에 Amp만 Abort된 상태로 끝난다.
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						errorCheck(ERRORCODE.FULL, sqc, "", ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_EMPTY); break;
					}
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; mc.hd.wastedonestop = true; break; }
					if (pickupFailDone) { pickupFailDone = false; errorCheck(ERRORCODE.HD, sqc, "Pick up 할 때 흡착이 되지 않습니다! Tube의 HeatSlug 기울기, Pick Up Z축 높이 위치를 확인해주세요!"); break; }
					else sqc = 10; break;
				#endregion

                #region case 100, 110, 120 pick to pick
                    // Derek Multi Head를 위한 작업
                    // Tool안에 pick_pick형식으로 만드려고 했으나... 내부 컨트롤이 겁나 하드코딩!! 
                #region case 100 Z move up
                case 100:
                    //Derek status 확인해서 error 추가필요
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICK2M;
                    sqc++; break;
                case 101:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.pick_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    if (delayD1 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 102:
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 103:
                    if (dwell.Elapsed < delayD1) break;
                    sqc++; break;
                case 104:
                    if (levelD2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.pick_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 105:
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 106:
                    if (dwell.Elapsed < delayD2) break;

                    if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 꺼준다.
                    {
                        mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
                    }
                    sqc++; break;
                case 107:

                    if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.ON)
                    {
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_VIB);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        shakeCount = 0;
                        sqc = 110;
                    }
                    else sqc = 120;
                    break;
                #endregion

                #region case 110 Z Shaking Motion
                // 일단 아래방향으로 떤다.
                // DOUBLE_DET 대신 Blow Position값을 입력해야 할 필요가 생길 수도 있다.
                case 110:
                    Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 111:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 112:
                    if (mc.para.HD.pick.shake.level.value == 0) mc.para.HD.pick.shake.level.value = 1000;
                    if (mc.para.HD.pick.shake.speed.value == 0) mc.para.HD.pick.shake.speed.value = 0.5;
                    if (shakeCount % 2 == 0)
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET - mc.para.HD.pick.shake.level.value, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 113:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 114:
                    if (dwell.Elapsed < mc.para.HD.pick.shake.delay.value) break;
                    sqc++; break;
                case 115:
                    shakeCount++;
                    if (shakeCount < mc.para.HD.pick.shake.count.value) sqc = 112;
                    else sqc = 120;
                    break;
                #endregion

                #region case 120 Slug Double Check
                case 120:
                    Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    if (mc.para.HD.pick.doubleCheck.enable.value == (int)ON_OFF.ON)
                    {
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_DOUBLE_DET);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        sqc++;
                        dwell.Reset();
                    }
                    else
                    {
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_ULC);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        //Derek 20으로 가도 될것 같은데...
                        mc.log.workHistory.write("---------------> End Pick Up(#" + (int)mc.hd.order.pick_done + ")");
                        sqc = 10;
                    }
                    break;
                case 121:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 122:
                    sqc = 10;
                    break;
                #endregion

                #endregion



                case SQC.ERROR:
					//string str = "HD home_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD home_pick Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}
		public void pick_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    if (mc.hd.order.ulc == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
					sqc++; break;
				case 1:
					#region pos set
                    //pickUp Sucess한 Head가 내려가 있을 것이므로 mc.hd.order.ulc를 이용한다
					Z[mc.hd.order.ulc].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.pick.driver.level.value;
						delayD1 = mc.para.HD.pick.driver.delay.value;
						velD1 = (mc.para.HD.pick.driver.vel.value / 1000);
						accD1 = mc.para.HD.pick.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.pick.driver2.level.value;
						delayD2 = mc.para.HD.pick.driver2.delay.value;
						velD2 = (mc.para.HD.pick.driver2.vel.value / 1000);
						accD2 = mc.para.HD.pick.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
                    #endregion
                    sqc = 10; break;

                #region case 10 Z move up
                case 10:
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICK2M;
                    sqc++; break;
                case 11:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.ulc].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    if (delayD1 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
                    dwell.Reset();
                    sqc++; break;
                case 13:
                    if (dwell.Elapsed < delayD1) break;
                    sqc++; break;
                case 14:
                    if (levelD2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.ulc].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 15:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
                    dwell.Reset();
                    sqc++; break;
                case 16:
                    if (dwell.Elapsed < delayD2) break;
                    sqc++; break;
                case 17:
                    Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    sqc = 20; break;
                #endregion

                #region case 20 XY.move.REF0
                case 20:
                    Y.moveCompare(cPos.y.REF0, Z[mc.hd.order.ulc].config, tPos.z[mc.hd.order.ulc].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.moveCompare(cPos.x.REF0, Z[mc.hd.order.ulc].config, tPos.z[mc.hd.order.ulc].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    mc.log.workHistory.write("Compare Axis : " + (int)mc.hd.order.ulc);
                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 23:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    sqc++; break;
                case 24:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;
                #endregion

				case SQC.ERROR:
					//string str = "HD pick_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD pick_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		int shakeCount;
		double tmpPos;
        bool ULCToULC = false;
        int compareULCAxis = 0;
        public int singleCycleHead = 0;

		public void pick_ulc()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    ULCToULC = false;
                    if (mc.hd.order.ulc == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
					sqc++; break;
				case 1:
					//if (mc.hd.withoutPick) { mc.pd.req = true; mc.pd.reqMode = REQMODE.AUTO; sqc = 40;  break;}		// Home Pick 을 안 가는 경우..
					#region pos set
					Z[mc.hd.order.pick_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.pick.driver.level.value;
						delayD1 = mc.para.HD.pick.driver.delay.value;
						velD1 = (mc.para.HD.pick.driver.vel.value / 1000);
						accD1 = mc.para.HD.pick.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.pick.driver2.level.value;
						delayD2 = mc.para.HD.pick.driver2.delay.value;
						velD2 = (mc.para.HD.pick.driver2.vel.value / 1000);
						accD2 = mc.para.HD.pick.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.pick_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
					if (delayD1 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < delayD1) break;
					sqc++; break;
				case 14:
					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.pick_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < delayD2) break;

					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 꺼준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
					}
					sqc++; break;
				case 17:
					if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.ON)
					{
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_VIB);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						shakeCount = 0;
						sqc = 20;
					}
					else sqc = 30;
					break;
				#endregion

				#region case 20 Z Shaking Motion
				// 일단 아래방향으로 떤다.
                // DOUBLE_DET 대신 Blow Position값을 입력해야 할 필요가 생길 수도 있다.
                case 20:
                    Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 22:
                    if (mc.para.HD.pick.shake.level.value == 0) mc.para.HD.pick.shake.level.value = 1000;
					if (mc.para.HD.pick.shake.speed.value == 0) mc.para.HD.pick.shake.speed.value = 0.5;
					if (shakeCount % 2 == 0)
					{
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET - mc.para.HD.pick.shake.level.value, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					}
					else
					{
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 23:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < mc.para.HD.pick.shake.delay.value) break;
					sqc++; break;
				case 25:
					shakeCount++;
					if (shakeCount < mc.para.HD.pick.shake.count.value) sqc = 22;
					else sqc = 30;
					break;
				#endregion

				#region case 30 Slug Double Check
				case 30:
                    mc.log.workHistory.write("---------------> End Pick Up(#" + (int)mc.hd.order.pick_done + ")");
					if (mc.para.HD.pick.doubleCheck.enable.value == (int)ON_OFF.ON)
					{
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_DOUBLE_DET);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						sqc++;
						dwell.Reset();
					}
					else
					{
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
						if (mc.hd.cycleMode)
						{
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_ULC);
							mc.hd.userMessageBox.ShowDialog();
							if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
							mc.hd.stepCycleExit = false;
						}
						sqc = 40;
					}
					break;
				case 31:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
                    sqc = 40;
					break;
				#endregion

				#region case 40 XYZ.move.ULC_C1(LIDC3 or LIDC2)
				case 40:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
                    if (!ULCToULC) compareULCAxis = (int)mc.hd.order.pick_done;
                    else compareULCAxis = (int)mc.hd.order.ulc_done;

                    // 20160524. jhlim : 실제 이동 XY 위치는 1번 Tool로, Compare 는 2번 Z축으로.. 마지막에 내려온 건 2번 이니까..
                    tmpPos = tPos.z[compareULCAxis].XY_MOVING;
                    if (mc.para.ULC.detectDirection.value == 0)         // 1->3
                    {
                        Y.moveCompare(tPos.y[mc.hd.order.ulc].LIDC1, Z[compareULCAxis].config, tmpPos - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(tPos.x[mc.hd.order.ulc].LIDC1, Z[compareULCAxis].config, tmpPos - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        // 4->2
                        Y.moveCompare(tPos.y[mc.hd.order.ulc].LIDC4, Z[compareULCAxis].config, tmpPos - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(tPos.x[mc.hd.order.ulc].LIDC4, Z[compareULCAxis].config, tmpPos - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    mc.log.workHistory.write("Compare Axis : " + compareULCAxis);
                    mc.log.workHistory.write("---------------> Start Lid Align(#" + (int)mc.hd.order.ulc + ")");
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Y_AT_TARGET || !X_AT_TARGET) break;// || !Z_AT_DONE(mc.hd.order.pick_done)) break;
                    
                    if (mc.hd.reqMode != REQMODE.DUMY) 
                    {
                        if (ULCToULC && mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX)
                        {
                            mc.sf.req = true;
                        }
                        else if (mc.para.ETC.useHeadMode.value != (int)UnitCodeHead.HD_MAX)
                        {
                            mc.sf.req = true;
                        }
                    }
                    
					dwell.Reset();
					sqc++; break;
				case 42:
                    // 일단 돈 검사하고 가자
                    if (!Y_AT_DONE || !X_AT_DONE) break;
					sqc++; break;
                case 43:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
                    if (mc.ulc.RUNING) break;
                    // 160531. jhlim : 임시
                    if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if(ULCToULC)
                    {
                        #region ULC.Result
                        ulcP2X = -1;
                        ulcP2Y = -1;
                        ulcP2T = -1;
                        if (mc.hd.reqMode != REQMODE.DUMY)
                        {
                            try
                            {
                                // 160531. jhlim : 임시
                                ulcP2X = mc.ulc.cam.edgeIntersection.resultX;
                                ulcP2Y = mc.ulc.cam.edgeIntersection.resultY;
                                ulcP2T = mc.ulc.cam.edgeIntersection.resultAngleH;

                                mc.log.debug.write(mc.log.CODE.INFO, "ULC 코너2(#" + mc.hd.order.ulc_done.ToString() + ") -> X : " + ulcP2X.ToString() + ", Y : "
                                    + ulcP2Y.ToString() + ", T : " + ulcP2T.ToString());
                            }
                            catch
                            {
                                ulcP2X = -1;
                                ulcP2Y = -1;
                                ulcP2T = -1;
                                mc.log.debug.write(mc.log.CODE.FAIL, "ULC 코너2(#" + mc.hd.order.ulc_done.ToString() + ") -> Teaching Error!!");
                            }
                        }
                        #endregion

                        if (mc.hd.reqMode != REQMODE.DUMY && (ulcP2X == -1 || ulcP2Y == -1 || ulcP2T == -1)) // ulc Vision Result Error
                        {
                            //if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                            //{
                            //    tempSb.Clear(); tempSb.Length = 0;
                            //    tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                            //    //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                            //    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                            //    sqc = 120; break;
                            //}
                            //else
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
                            }
                        }

                        calcULCX[mc.hd.order.ulc] = (ulcP1X + ulcP2X) / 2;
                        calcULCY[mc.hd.order.ulc] = (ulcP1Y + ulcP2Y) / 2;
                        calcULCT[mc.hd.order.ulc] = (ulcP1T + ulcP2T) / 2;

                        mc.log.debug.write(mc.log.CODE.INFO, "ULC 보상(#" + mc.hd.order.ulc.ToString() + ") -> X : " + calcULCX[mc.hd.order.ulc].ToString() + ", Y : " + calcULCY[mc.hd.order.ulc].ToString() + ", T : " + calcULCT[mc.hd.order.ulc].ToString());
                    }
                    sqc++; break;
				case 44:
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
                        Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].ULC_FOCUS_WITH_MT, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						// 부품 높이만큼 띄워야 한다. 20140326
                        Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].ULC_FOCUS_WITH_MT /*+ mc.para.MT.lidSize.h.value * 1000*/, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					}
					#region ULC.req
                    ulcP1X = 0; ulcP1Y = 0; ulcP1T = 0;

                    if (mc.para.ULC.detectDirection.value == 0)     // 1->3
                    {
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.ulc.reqMode = REQMODE.GRAB;
                        else
                        {
                            if (mc.para.ULC.modelCorner1.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
                            }
                            else
                            {
                                mc.ulc.reqMode = REQMODE.GRAB;
                            }
                        }
                        mc.ulc.lighting_exposure(mc.para.ULC.modelCorner1.light, mc.para.ULC.modelCorner1.exposureTime);
                    }
                    else
                    {
                        // 4->2
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.ulc.reqMode = REQMODE.GRAB;
                        else
                        {
                            if (mc.para.ULC.modelCorner4.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
                            }
                            else
                            {
                                mc.ulc.reqMode = REQMODE.GRAB;
                            }
                        }
                        mc.ulc.lighting_exposure(mc.para.ULC.modelCorner4.light, mc.para.ULC.modelCorner4.exposureTime);
                    }
                    
                    // 160531. jhlim : 임시
                    //mc.ulc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 45:
					if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 46:
					if(!Z_AT_DONE(mc.hd.order.ulc)) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
					sqc = 50; break;
				#endregion

                #region case 50 trigger ULC_C1(LIDC3)
                case 50:
					//if (mc.ulc.req == false) { sqc = SQC.STOP; break; }
                    if (dev.NotExistHW.CAMERA) { sqc = 53; break; }
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 15) break; //  ulc camera delay
                    // 160606. jhlim
                    mc.ulc.visionEnd = false;
                    mc.ulc.req = true;
                    // 160601. jhlim 
                    //triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
                    //수정예정
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    // 160601. jhlim 
                    //triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 53:
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
// 					mc.hd.homepickdone = true;		// 집고 문제 없으니 true
                    sqc = 70; break;
                    //mc.hd.order.set(mc.hd.order.ulc, (int)ORDER.ULCI_SUCESS);
                    //if (mc.hd.order.ulc != (int)ORDER.EMPTY) { sqc = 100; break; }
					//sqc = SQC.STOP; break;
				#endregion

                #region case 70 XYZ.move.ULC_C2(LIDC1 or LIDC4)
                case 70:
                    if (!dev.NotExistHW.CAMERA && !mc.ulc.visionEnd) break;

                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
   					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    if (mc.para.ULC.detectDirection.value == 0)     // 1->3
                    {
                        Y.move(tPos.y[mc.hd.order.ulc].LIDC3, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(tPos.x[mc.hd.order.ulc].LIDC3, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        // 4->2
                        Y.move(tPos.y[mc.hd.order.ulc].LIDC2, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(tPos.x[mc.hd.order.ulc].LIDC2, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    //mc.ulc.cam.grabClear(out ret.message, out ret.s);	// clear grab image 20140829
                    dwell.Reset();
                    sqc++; break;
                case 71:
                    if (!Y_AT_TARGET || !X_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 72:
                    //if (!Y_AT_DONE || !X_AT_DONE) break;
                    // 20160522. jhlim : Done 검사 해야할까..??
                    //if (timeCheck(X.config.axisCode, sqc, 10)) break;
                    //X.actualPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    //Y.actualPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //if (Math.Abs(tPos.x[mc.hd.order.ulc].ULC - ret.d1) > 50000 || Math.Abs(tPos.y[mc.hd.order.ulc].ULC - ret.d2) > 50000) break;
                    sqc++; break;
                case 73:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
                    if (mc.ulc.RUNING) break;
                    // 160531. jhlim : 임시
                    if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    #region ULC.Result
                    ulcP1X = -1;
                    ulcP1Y = -1;
                    ulcP1T = -1;
                    if (mc.hd.reqMode != REQMODE.DUMY)
                    {
                        try
                        {
                            // 160531. jhlim : 임시
                            ulcP1X = mc.ulc.cam.edgeIntersection.resultX;
                            ulcP1Y = mc.ulc.cam.edgeIntersection.resultY;
                            ulcP1T = mc.ulc.cam.edgeIntersection.resultAngleH;

                            mc.log.debug.write(mc.log.CODE.INFO, "ULC 코너1(#" + mc.hd.order.ulc.ToString() + ") -> X : " + ulcP1X.ToString() + ", Y : "
                                + ulcP1Y.ToString() + ", T : " + ulcP1T.ToString());
                        }
                        catch
                        {
                            ulcP2X = -1;
                            ulcP2Y = -1;
                            ulcP2T = -1;
                            mc.log.debug.write(mc.log.CODE.FAIL, "ULC 코너1(#" + mc.hd.order.ulc.ToString() + ") -> Teaching Error!!");
                        }
                    }
                    #endregion
                    if (mc.hd.reqMode != REQMODE.DUMY && ulcP1X == -1 && ulcP1Y == -1 && ulcP1T == -1) // ulc Vision Result Error
					{
                        //if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                        //{
                        //    tempSb.Clear(); tempSb.Length = 0;
                        //    tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                        //    //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                        //    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                        //    sqc = 120; break;
                        //}
                        //else
						{
							tempSb.Clear(); tempSb.Length = 0;
							tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}], PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
							errorCheck(ERRORCODE.HD, sqc, tempSb.ToString()); break;
						}
					}
                    
                    #region ULC.req
                    ulcP2X = 0; ulcP2Y = 0; ulcP2T = 0;

                    if (mc.para.ULC.detectDirection.value == 0)     // 1->3
                    {
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.ulc.reqMode = REQMODE.GRAB;
                        else
                        {
                            if (mc.para.ULC.modelCorner3.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                            }
                            else
                            {
                                mc.ulc.reqMode = REQMODE.GRAB;
                            }
                        }
                        mc.ulc.lighting_exposure(mc.para.ULC.modelCorner3.light, mc.para.ULC.modelCorner3.exposureTime);
                    }
                    else
                    {
                        // 4->2
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.ulc.reqMode = REQMODE.GRAB;
                        else
                        {
                            if (mc.para.ULC.modelCorner2.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.ulc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                            }
                            else
                            {
                                mc.ulc.reqMode = REQMODE.GRAB;
                            }
                        }
                        mc.ulc.lighting_exposure(mc.para.ULC.modelCorner2.light, mc.para.ULC.modelCorner2.exposureTime);
                    }

                    // 160531. jhlim : 임시
                    //mc.ulc.req = true;
		            #endregion
                    dwell.Reset();
                    sqc++; break;
                case 74:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
                    dwell.Reset();
                    sqc++; break;
                case 75:
                    if (!Z_AT_DONE(mc.hd.order.ulc)) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
                    sqc = 90; break;
                #endregion

                #region case 90 trigger ULC_C2(LIDC1)
                case 90:
                    //if (mc.ulc.req == false) { sqc = SQC.STOP; break; }
                    if (dev.NotExistHW.CAMERA) { sqc = 93; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
                    dwell.Reset();
                    sqc++; break;
                case 91:
                    if (dwell.Elapsed < 15) break; //  ulc camera delay
                    // 160606. jhlim
                    mc.ulc.visionEnd = false;
                    mc.ulc.req = true;
                    // 160601. jhlim 
                    //triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 92:
                    //수정예정
                    if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    // 160601. jhlim 
                    //triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    sqc++; break;
                case 93:
                    if (!dev.NotExistHW.CAMERA && !mc.ulc.visionEnd) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
                    // 					mc.hd.homepickdone = true;		// 집고 문제 없으니 true

                    mc.log.workHistory.write("---------------> End Lid Align(#" + (int)mc.hd.order.ulc + ")");   
                    mc.hd.order.set(mc.hd.order.ulc, (int)ORDER.ULCI_SUCESS);
                    if (mc.hd.order.ulc != (int)ORDER.EMPTY) { sqc = 100; break; }
                    sqc = SQC.STOP; break;
                #endregion

                #region case 100 z move up
                case 100:
                    if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX) ULCToULC = true;
                    Z[mc.hd.order.ulc_done].move(tPos.z[mc.hd.order.ulc_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 101:       // 20160524. jhlim : 막아도 될거 같은데..
                    //if (!Z_AT_TARGET(mc.hd.order.ulc_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 102:
                    //if (!Z_AT_DONE(mc.hd.order.ulc_done)) break;
                    
                    sqc = 40; break;
                #endregion

                case SQC.ERROR:
					//string str = "HD pick_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD pick_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
// 					mc.hd.homepickdone = false;		// 집고 문제 있으니 false
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
        public int ulcfailcount;
		public bool ulcfailchecked;
		public int ulcchamferfail;
		public int hdcfailcount;
		
		public int fiducialfailcount;
		public bool fiducialfailchecked;
		public int pickfailcount;
		public bool pickfailchecked;
		//bool writedone;
		int attachError;	// 0:No Error, 1:Under Force Error, 2:Over Force Error
		int graphDisplayIndex;
		int graphDisplayCount;
		int graphDisplayPoint;
		double graphVPPMFilter;
		double graphLoadcellFilter;
        double feedbackForce;
		double sgaugeVolt, sgaugeForce;		// variable for strain gauge loadcell force data graph display
		QueryTimer loadTime = new QueryTimer();
		QueryTimer forceTime = new QueryTimer();
		double diffForce = 0;
		bool graphDispStart;
		Random rndSeed = new Random();
		double[] loadForceFilter = new double[1000];
		double[] sgaugeForceFilter = new double[1000];
		int meanFilterIndex;
		public double forceTargetZPos;
		bool contactPointSearchDone;
		bool forceStartPointSearchDone;
		bool linearAutoTrackStart;
		int forceStartPointCheckCount;			// 지정된 횟수동안 force가 형성되어야 start point로 인식한다.
		QueryTimer autoTrackDelayTime = new QueryTimer();		// 이 시간이 새로운 Place Delay Time이 된다.
		QueryTimer autoTrackCheckTime = new QueryTimer();		// 이 시간마다 Tracking 보상을 수행한다.
		double contactPos;
		QueryTimer placeForceErrorTime = new QueryTimer();		// Limit Force를 얼마의 시간동안 Over했는지 검사하는 시간으로 사용된다.
		bool placeForceOver, placeForceUnder;
		int placeSensorForceCheckCount;
		int placeForceCheckCount;

		// Place Force 평균 구하기 위한 용도
		public double placeForceMean;
		double placeForceSumCount;
		double placeForceMin;
		double placeForceMax;
		double placeForceSum;
		StringBuilder tempSb = new StringBuilder();
		double cosTheta, sinTheta;
		double tmpDistX, tmpDistY;
        bool placeToPlace = false;
        int compareAxis = 0;
        double[] tempValue = new double[4];

        bool useTopLoadcell = true;
        public double placeForce = 0;
        
        double trackingForce = 0;
        double trackingHeight = 0;
        double trackingSpeed = 0;
        double trackingAcc = 0;

        #region JogTeach 용 변수
        double p1X = 0;
        double p1Y = 0;
        double p2X = 0;
        double p2Y = 0;
        double totalP1X = 0;
        double totalP1Y = 0;
        double totalP2X = 0;
        double totalP2Y = 0;
        double refAngle = 0;
        double realAngle = 0;
        double totalAngle = 0;

        public jogTeachCornerMode JogTeachMode;
        public bool jogTeachCancel = false;
        public bool jogTeachIgnore = false;
        FormJogTeach jogTeach = new FormJogTeach();
        public bool attachSkip = false;
        public bool setJogTeach = false;
        #endregion

        bool usePatternPos = false;

		public void ulc_place()
		{
			#region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
			if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
			{
				mc.OUT.HD.SUC(mc.hd.order.bond, out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (ret.b)
				{
                    Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.place.suction.level.value)
					{
                        mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

            switch (sqc)
            {
                case 0:
                    mc.hdc.LIVE = false;
                    mc.hdc.visionEnd = false;
                    placeToPlace = false;
                    hdcfailcount = 0;
                    fiducialfailcount = 0;
                    fiducialfailchecked = false;
                    Esqc = 0;
                    if (mc.hd.order.ulc_done == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
                    graphDisplayCount = UtilityControl.graphDisplayFilter;
                    graphDisplayPoint = UtilityControl.graphStartPoint;
                    graphVPPMFilter = UtilityControl.graphControlDataFilter;
                    graphLoadcellFilter = UtilityControl.graphLoadcellDataFilter;
                    EVENT.clearLoadcellData();
                    if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON) sqc = 1;
                    else sqc = 10;
                    break;

                #region Check Ficucial Mark
                case 1:
                    // 수정예정... 지저분함...
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;


                    //mc.activate.headCnt 수정 필요... 두축 모두 확인해야 함...
                    if (mc.para.HDC.fiducialPos.value == 0)
                    {
                        Y.moveCompare(cPos.y.PADC1(padY), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(cPos.x.PADC1(padX), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else if (mc.para.HDC.fiducialPos.value == 1)
                    {
                        Y.moveCompare(cPos.y.PADC2(padY), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(cPos.x.PADC2(padX), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else if (mc.para.HDC.fiducialPos.value == 2)
                    {
                        Y.moveCompare(cPos.y.PADC3(padY), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(cPos.x.PADC3(padX), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Y.moveCompare(cPos.y.PADC4(padY), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(cPos.x.PADC4(padX), Z, tPos.z[mc.hd.order.ulc_done].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    mc.log.workHistory.write("Compare Axis : " + (int)mc.hd.order.ulc_done);
                    dwell.Reset();
                    sqc++; break;
                case 2:
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].AT_ERROR(out ret.b, out ret.message); if (ret.b) break;
                    }
                    if (ret.b)
                    {
                        X.eStop(out ret.message); Y.eStop(out ret.message);
                    }
                    if (!Z_AT_TARGET_ALL()) break;
                    #region HDC.PADC1.req
                    fidPX = 0;
                    fidPY = 0;
                    fidPD = 0;
                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
                            mc.hdc.reqPassScore = mc.para.HDC.modelPAD.passScore.value;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
                            mc.hdc.reqPassScore = mc.para.HDC.modelPAD.passScore.value;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                    {
                        if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
                        else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
                        else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
                        else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
                    }
                    else mc.hdc.reqMode = REQMODE.GRAB;
                    mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                    #endregion
                    dwell.Reset();
                    sqc++; break;
                case 3:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;  // 목표 위치로 이동 됐는지 신호로 판단 
                    dwell.Reset();
                    sqc++; break;
                case 4:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;   // 목표로 이동 했으니까,, 멈췄는지 검사 ..
                    sqc++; break;
                case 5:
                    if (mc.pd.RUNING) break;
                    if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    dwell.Reset();
                    sqc++; break;
                case 6:
                    if (dwell.Elapsed < 15) break; // head camera delay
                    if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 7:
                    if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    //if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
                    dwell.Reset();
                    sqc++; break;
                case 8:
                    if (mc.hdc.RUNING) break;
                    if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc.hdc.cam.refresh_req) break;
                    #region fiducial result
                    if (mc.hd.reqMode == REQMODE.DUMY) { }
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
                        {
                            fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
                            fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
                            fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
                        }
                    }
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
                        {
                            fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
                            fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
                            fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
                        }
                    }
                    else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
                    {
                        fidPX = mc.hdc.cam.circleCenter.resultX;
                        fidPY = mc.hdc.cam.circleCenter.resultY;
                        fidPD = mc.hdc.cam.circleCenter.resultRadius;
                    }
                    #endregion
                    if (fidPX == -1 && fidPY == -1 && fidPD == -1) // HDC Fiducial Result Error
                    {
                        tempSb.Clear(); tempSb.Length = 0;
                        tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                        fiducialfailcount++;
                        if (fiducialfailcount < mc.para.HDC.failretry.value)
                        {
                            tempSb.AppendFormat("Fiducial Check Fail ({0})", (fiducialfailcount + 1));
                            //str += "fiducial check fail (" + (fiducialfailcount + 1) + ")";
                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                            sqc = 2;
                        }
                        else
                        {
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_FIDUCIAL_FIND_FAIL);
                        }
                    }
                    else
                    {
                        sqc = 10;
                    }
                    break;
                #endregion

                #region case 10 xy pad c1 move
                case 10:
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 0);
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;

                    if (mc.hd.tool.singleCycleHead == (int)UnitCodeHead.INVALID)
                    {
                        if (mc.para.ETC.useHeadMode.value == (int)UnitCodeHead.HD_MAX)
                        {
                            if (!placeToPlace) compareAxis = (int)UnitCodeHead.HD2;
                            else
                            {
                                compareAxis = (int)UnitCodeHead.HD1;
                                mc.para.runInfo.checkCycleTime();
                                mc.para.runInfo.startCycleTime();
                            }
                        }
                        else compareAxis = Convert.ToInt32(mc.para.ETC.useHeadMode.value);
                    }
                    else compareAxis = mc.hd.tool.singleCycleHead;

                    if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON)
                    {
                        rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                        rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

                        if (mc.para.HDC.detectDirection.value == 0)
                        {
                            if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                            {
                                usePatternPos = false;
                            }
                            else
                            {
                                usePatternPos = true;
                            }

                            Y.move(cPos.y.PADC2(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.move(cPos.x.PADC2(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                        else
                        {
                            if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                            {
                                usePatternPos = false;
                            }
                            else
                            {
                                usePatternPos = true;
                            }

                            Y.move(cPos.y.PADC1(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            X.move(cPos.x.PADC1(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                    }
                    else
                    {
                        if (mc.para.HDC.detectDirection.value == 0)
                        {
                            if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                            {
                                usePatternPos = false;
                            }
                            else
                            {
                                usePatternPos = true;
                            }

                            double targetX = cPos.x.PADC2(padX, usePatternPos);
                            double targetY = cPos.y.PADC2(padY, usePatternPos);

                            X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            if (Math.Abs(targetX - ret.d1) > 50000 || Math.Abs(targetY - ret.d2) > 50000)
                            {
                                // 20160524. jhlim : 마지막에 검사한 거와 Compare 하자
                                Y.moveCompare(targetY, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                                X.moveCompare(targetX, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            }
                            else
                            {
                                Y.moveCompare(targetY, mc.speed.slow, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                                X.moveCompare(targetX, mc.speed.slow, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            }
                        }
                        else
                        {
                            if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                            {
                                usePatternPos = false;
                            }
                            else
                            {
                                usePatternPos = true;
                            }

                            double targetX = cPos.x.PADC1(padX, usePatternPos);
                            double targetY = cPos.y.PADC1(padY, usePatternPos);

                            X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                            if (Math.Abs(targetX - ret.d1) > 50000 || Math.Abs(targetY - ret.d2) > 50000)
                            {
                                // 20160524. jhlim : 마지막에 검사한 거와 Compare 하자
                                Y.moveCompare(targetY, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                                X.moveCompare(targetX, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            }
                            else
                            {
                                Y.moveCompare(targetY, mc.speed.slow, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                                X.moveCompare(targetX, mc.speed.slow, Z[compareAxis].config, tPos.z[compareAxis].XY_MOVING - comparePos + 1500, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                            }
                        }
                    }
                    mc.log.workHistory.write("Compare Axis : " + compareAxis);
                    mc.log.workHistory.write("---------------> Start PCB Align(#" + mc.hd.order.bond + ")");
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].AT_ERROR(out ret.b, out ret.message); if (ret.b) break;
                    }
                    if (ret.b)
                    {
                        X.eStop(out ret.message); Y.eStop(out ret.message);
                    }
                    if (!Z_AT_TARGET_ALL()) break;

                    if (mc.pd.RUNING) break;
                    if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    #region hdc.req
                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        #region HDC.PADC2.req
                        //Derek 하기 주석 삭제 예정
                        hdcP1X = -1;
                        hdcP1Y = -1;
                        hdcP1T_1 = -1;
                        hdcP1T_2 = -1;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC2.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC2.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_2;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }
                    else
                    {
                        #region HDC.PADC1.req
                        hdcP1X = 0;
                        hdcP1Y = 0;
                        hdcP1T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_1;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }

                    #endregion

                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 13:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;
                    sqc++; break;
                case 14:
                    //Derek 수정예정 - 하기 주석 삭제
                    if (mc.pd.RUNING) break;
                    if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 1);
                    sqc = 20; break;
                #endregion

                #region case 20 triggerHDC
                case 20:
                    if (dev.NotExistHW.CAMERA) { sqc = 30; break; }
                    if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 0);
                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (dwell.Elapsed < 15) break; // head camera delay
                    mc.hdc.visionEnd = false;
                    if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 22:
                    //Derek 수정예정 - 하기 주석
                    if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
                    dwell.Reset();
                    sqc++; break;
                case 23:
                    //if (dwell.Elapsed < 300) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 1);
                    sqc = 30; break;
                #endregion

                #region case 30 xy pad c3 move
                case 30:
                    if (!dev.NotExistHW.CAMERA && !mc.hdc.visionEnd) break;

                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 0);
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

                    #region hdc.req
                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC4(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC4(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC3(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC3(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    #endregion

                    dwell.Reset();
                    sqc++; break;
                case 31:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
                    if (!dev.NotExistHW.CAMERA && (!mc.hdc.visionEnd || !mc.ulc.visionEnd)) break;
                    if (mc.ulc.RUNING || mc.hdc.RUNING) break;
                    if (mc.hd.reqMode != REQMODE.DUMY && (mc.ulc.ERROR || mc.hdc.ERROR)) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    
                    #region ULC result
                    if (mc.hd.reqMode == REQMODE.DUMY) { }
                    else if (mc.para.ULC.modelCorner2.isCreate.value == (int)BOOL.TRUE)
                    {
                        try
                        {
                            // 160531. jhlim : 임시
                            ulcP2X = mc.ulc.cam.edgeIntersection.resultX;
                            ulcP2Y = mc.ulc.cam.edgeIntersection.resultY;
                            ulcP2T = mc.ulc.cam.edgeIntersection.resultAngleH;
                            mc.log.debug.write(mc.log.CODE.INFO, "ULC 코너2(#" + mc.hd.order.ulc_done.ToString() + ") -> X : " + ulcP2X.ToString() + ", Y : "
                                + ulcP2Y.ToString() + ", T : " + ulcP2T.ToString());

                            calcULCX[mc.hd.order.ulc_done] = (ulcP1X + ulcP2X) / 2;
                            calcULCY[mc.hd.order.ulc_done] = (ulcP1Y + ulcP2Y) / 2;
                            calcULCT[mc.hd.order.ulc_done] = (ulcP1T + ulcP2T) / 2;
                            mc.log.debug.write(mc.log.CODE.INFO, "ULC 보상(#" + mc.hd.order.ulc_done.ToString() + ") -> X : " + calcULCX[mc.hd.order.ulc_done].ToString() + ", Y : " + calcULCY[mc.hd.order.ulc_done].ToString() + ", T : " + calcULCT[mc.hd.order.ulc_done].ToString());
                        }
                        catch
                        {
                            ulcP2X = -1;
                            ulcP2Y = -1;
                            ulcP2T = -1;
                            mc.log.debug.write(mc.log.CODE.FAIL, "ULC 코너2(#" + mc.hd.order.ulc_done.ToString() + ") -> Teaching Error!!");

                            calcULCX[mc.hd.order.ulc_done] = -1;
                            calcULCY[mc.hd.order.ulc_done] = -1;
                            calcULCT[mc.hd.order.ulc_done] = -1;
                        }
                    }
                    // 160622. jhlim : 여기다가 ULC Score 넣자
                    if (mc.hd.reqMode != REQMODE.DUMY && calcULCX[mc.hd.order.ulc_done] == -1 && calcULCY[mc.hd.order.ulc_done] == -1 && calcULCT[mc.hd.order.ulc_done] == -1) // ULC Vision Result Error
                    {
                        mc.ulc.displayUserMessage("LID DETECTION FAIL");
                        if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                        {
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("LID Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                            //string str = "LID Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                            //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                            ulcfailchecked = true;
                            mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                            sqc = SQC.END; break;
                        }
                        else
                        {
                            mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("LID Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_VISION_PROCESS_FAIL); break;
                        }
                    }

                    #region check center
                    // Center Limit Check 
                    //if (Math.Abs(clacULCX) > mc.para.MT.lidCheckLimit.value)
                    //{
                    //    mc.ulc.displayUserMessage("X RESULT OVER FAIL");
                    //    tempSb.Clear(); tempSb.Length = 0;
                    //    tempSb.AppendFormat("LID Chk Fail(X Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(clacULCX), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);

                    //    //string str = "LID Chk Fail(X Limit-Rst[" + Math.Round(ulcX).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                    //    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());

                    //    if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                    //    {
                    //        //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                    //        ulcfailchecked = true;
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_X_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        sqc = SQC.END; break;
                    //    }
                    //    else
                    //    {
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_X_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(clacULCX));
                    //        //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcX).ToString() + "]";
                    //        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_X_RESULT_OVER); break;
                    //    }
                    //}
                    //if (Math.Abs(clacULCY) > mc.para.MT.lidCheckLimit.value)
                    //{
                    //    mc.ulc.displayUserMessage("Y RESULT OVER FAIL");
                    //    tempSb.Clear(); tempSb.Length = 0;
                    //    tempSb.AppendFormat("LID Chk Fail(Y Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(clacULCY), Math.Round(mc.para.MT.lidCheckLimit.value), (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                    //    //string str = "LID Chk Fail(Y Limit-Rst[" + Math.Round(ulcY).ToString() + "]Lmt[" + Math.Round(mc.para.MT.lidCheckLimit.value).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                    //    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
        
                    //    if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                    //    {
                    //        //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                    //        ulcfailchecked = true;
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_Y_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        sqc = SQC.END; break;
                    //    }
                    //    else
                    //    {
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_Y_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(clacULCY));
                    //        //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcY).ToString() + "]";
                    //        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_Y_RESULT_OVER); break;
                    //    }
                    //}
                    //if (Math.Abs(clacULCT) > 10) 
                    //{
                    //    mc.ulc.displayUserMessage("R RESULT OVER FAIL");
                    //    tempSb.Clear(); tempSb.Length = 0;
                    //    tempSb.AppendFormat("LID Chk Fail(T Limit-Rst[{0}]Lmt[{1}])-PadX[{2}],PadY[{3}],FailCnt[{4}]", Math.Round(clacULCT), 10.0, (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                    //    //string str = "LID Chk Fail(T Limit-Rst[" + Math.Round(ulcT).ToString() + "]Lmt[" + Math.Round(10.0).ToString() + "])-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                    //    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
                    //    if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                    //    {
                    //        //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                    //        ulcfailchecked = true;
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_T_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        sqc = SQC.END; break;
                    //    }
                    //    else
                    //    {
                    //        if (mc.para.ULC.imageSave.value == 1) mc.ulc.cam.writeLogGrapImage("ULC_T_Limit_Fail");
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.POSERR);
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(clacULCT));
                    //        //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(ulcT).ToString() + "]";
                    //        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_HEAT_SLUG_T_RESULT_OVER); break;
                    //    }
                    //}
                    #endregion

                    #region check Orientation
                    // 여기에 오리엔테이션 체크 추가해야함-------------------------------------------------------------------------------------
                    //if (mc.hd.reqMode == REQMODE.DUMY) { }
                    //else if (mc.para.ULC.orientationUse.value == 1 && mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
                    //{
                    //    if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
                    //    {
                    //        tmpX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
                    //        tmpY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
                    //        tmpT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
                    //    }
                    //    else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    //    {
                    //        tmpX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
                    //        tmpY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
                    //        tmpT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
                    //    }
                    //}
                    //if (mc.para.ULC.orientationUse.value == 1 && tmpX == -1 && tmpY == -1 && tmpT == -1) // ULC Vision Result Error
                    //{
                    //    mc.ulc.displayUserMessage("LID ORIENTATION CHECK FAIL");
                    //    if (mc.para.ULC.failretry.value > 0 && mc.hd.tool.ulcfailcount < mc.para.ULC.failretry.value)
                    //    {
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("LID Orientation Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                    //        //string str = "LID Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]";
                    //        mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                    //        //EVENT.statusDisplay("LID Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.ulcfailcount.ToString() + "]");
                    //        ulcfailchecked = true;
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                    //        sqc = SQC.END; break;
                    //    }
                    //    else
                    //    {
                    //        mc.para.runInfo.writePickInfo(PickCodeInfo.VISIONERR);
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("LID Orientation Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.ulcfailcount);
                    //        //string str = "PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "]";
                    //        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_ULC_VISION_PROCESS_FAIL); break;
                    //    }
                    //}
                    //-------------------------------------------------------------------------------------------------------------
                    #endregion

                    // 자동 보상을 해주기 위하여 검사 결과를 저장한다.
                    //if (!hdcfailchecked && mc.hd.reqMode != REQMODE.DUMY)
                    //{
                    //    if (Math.Abs(clacULCX) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].x.value += clacULCX;
                    //    if (Math.Abs(clacULCY) < 500) mc.para.HD.pick.pickPosComp[mc.hd.pickedPosition].y.value += clacULCY;
                    //}
                    #endregion

                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        #region HDC.PADC2.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
                                hdcP1Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultScore;
                                hdcP1PassScore = mc.para.HDC.modelPADC2.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
                                hdcP1Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultScore;
                                hdcP1PassScore = mc.para.HDC.modelPADC2.passScore.value;

                            }
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            // 160531. jhlim : 임시
                            hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                            hdcP1Score = 100;
                            hdcP1PassScore = 0;

                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            // 160531. jhlim : 임시
                            hdcP1X = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultX;
                            hdcP1Y = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultY;
                            hdcP1T_1 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultAngle;
                            hdcP1T_2 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultAngle2;
                            hdcP1Score = 100;
                            hdcP1PassScore = 0;

                            if (dev.debug)
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, "HDC 코너2(#" + mc.hd.order.bond.ToString() + ") -> X : " + Math.Round(hdcP1X, 2).ToString() + ", Y : "
                                    + Math.Round(hdcP1Y, 2).ToString() + ", T1 : " + Math.Round(hdcP1T_1, 2).ToString() + ", T2 : " + Math.Round(hdcP1T_2, 2).ToString());
                            }
                        }
                        if (mc.hd.reqMode != REQMODE.DUMY && (hdcP1X == -1 || hdcP1Y == -1 || hdcP1T_1 == -1
                            || hdcP1Score * 100 < hdcP1PassScore)) // HDC Vision Result Error
                        {
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner24;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
                                }
                            }
                        }
                        #endregion
                        #region HDC.PADC4.req
                        hdcP2X = -1;
                        hdcP2Y = -1;
                        hdcP2T_1 = -1;
                        hdcP2T_2 = -1;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_4;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }
                    else
                    {
                        #region HDC.PADC1.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
                                hdcP1Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultScore;
                                hdcP1PassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
                                hdcP1Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultScore;
                                hdcP1PassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                            hdcP1Score = 100;
                            hdcP1PassScore = 0;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            // 160531. jhlim : 임시
                            hdcP1X = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultX;
                            hdcP1Y = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultY;
                            hdcP1T_1 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultAngle;
                            hdcP1T_2 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultAngle2;
                            hdcP1Score = 100;
                            hdcP1PassScore = 0;

                            if (dev.debug)
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, "HDC 코너1(#" + mc.hd.order.bond.ToString() + ") -> X : " + Math.Round(hdcP1X, 2).ToString() + ", Y : "
                                    + Math.Round(hdcP1Y, 2).ToString() + ", T1 : " + Math.Round(hdcP1T_1, 2).ToString() + ", T2 : " + Math.Round(hdcP1T_2, 2).ToString());
                            }
                        }
                        if (mc.hd.reqMode != REQMODE.DUMY && (hdcP1X == -1 || hdcP1Y == -1 || hdcP1T_1 == -1
                            || hdcP1Score * 100 < hdcP1PassScore)) // HDC Vision Result Error
                        {
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}],FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1X) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1} um", hdcP1X));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1X));
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1Y) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1} um", hdcP1Y));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1Y));
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1T_1) > 5 || Math.Abs(hdcP1T_2) > 5)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1} degree", hdcP1T_1));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2}]", (padX + 1), (padY + 1), Math.Round(hdcP1T_1));
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
                                }
                            }
                        }
                        #endregion
                        #region HDC.PADC3.req
                        hdcP2X = 0;
                        hdcP2Y = 0;
                        hdcP2T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_3;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }

                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 33:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 1);
                    sqc = 40; break;
                #endregion

                #region case 40 triggerHDC
                case 40:
                    mc.log.workHistory.write("---------------> End PCB Align(#" + (int)mc.hd.order.bond + ")");
                    if (dev.NotExistHW.CAMERA) { sqc = 50; break; }
                    if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 0);
                    dwell.Reset();
                    sqc++; break;
                case 41:
                    if (dwell.Elapsed < 15) break; // head camera delay
                    mc.hdc.visionEnd = false;
                    if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 42:
                    //Derek 하기 주석 삭제 예정
                    if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
                    dwell.Reset();
                    sqc++; break;
                case 43:
                    //if (dwell.Elapsed < 300) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 1);
                    sqc = 50; break;
                #endregion

                #region case 50 xy pad move
                case 50:
                    placeX = tPos.x[mc.hd.order.bond].PAD(padX);
                    placeY = tPos.y[mc.hd.order.bond].PAD(padY);
                    placeT = tPos.t[mc.hd.order.bond].ZERO;
                    dwell.Reset();
                    sqc++; break;
                case 51:
                    if (dev.NotExistHW.CAMERA) { sqc++; break; }
                    if (!dev.NotExistHW.CAMERA && !mc.hdc.visionEnd) break;
                    if (mc.hdc.RUNING) break;
                    if (mc.hd.reqMode != REQMODE.DUMY && mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HDC.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HDC.detectDirection.value == 1))
                    {
                        #region HDC.PADC4.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
                                hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
                                hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
                                hdcP2Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultScore;
                                hdcP2PassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
                                hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
                                hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
                                hdcP2Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultScore;
                                hdcP2PassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            // 160531. jhlim : 임시
                            hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                            hdcP2Score = 100;
                            hdcP2PassScore = 0;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            // 160531. jhlim : 임시
                            hdcP2X = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultX;
                            hdcP2Y = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultY;
                            hdcP2T_1 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultAngle;
                            hdcP2T_2 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultAngle2;
                            hdcP2Score = 100;
                            hdcP2PassScore = 0;

                            if (dev.debug)
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, "HDC 코너4(#" + mc.hd.order.bond.ToString() + ") -> X : " + Math.Round(hdcP2X, 2).ToString() + ", Y : "
                                    + Math.Round(hdcP2Y, 2).ToString() + ", T1 : " + Math.Round(hdcP2T_1, 2).ToString() + ", T2 : " + Math.Round(hdcP2T_2, 2).ToString());
                            }
                        }
                        //cosTheta = Math.Cos(hdcT * Math.PI / 180);
                        //sinTheta = Math.Sin(hdcT * Math.PI / 180);
                        //hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
                        //hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
                        //EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
                        #endregion

                    }
                    else
                    {
                        #region HDC.PADC3.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
                                hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
                                hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
                                hdcP2Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultScore;
                                hdcP2PassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
                                hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
                                hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
                                hdcP2Score = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultScore;
                                hdcP2PassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                            hdcP2Score = 100;
                            hdcP2PassScore = 0;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            // 160531. jhlim : 임시
                            hdcP2X = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultX;
                            hdcP2Y = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultY;
                            hdcP2T_1 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultAngle;
                            hdcP2T_2 = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultAngle2;
                            hdcP2Score = 100;
                            hdcP2PassScore = 0;

                            if (dev.debug)
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, "HDC 코너3(#" + mc.hd.order.bond.ToString() + ") -> X : " + Math.Round(hdcP2X, 2).ToString() + ", Y : "
                                    + Math.Round(hdcP2Y, 2).ToString() + ", T1 : " + Math.Round(hdcP2T_1, 2).ToString() + ", T2 : " + Math.Round(hdcP2T_2, 2).ToString());
                            }
                        }
                        //cosTheta = Math.Cos(hdcT * Math.PI / 180);
                        //sinTheta = Math.Sin(hdcT * Math.PI / 180);
                        //hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
                        //hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
                        //EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
                        #endregion
                    }
                    //mc.log.debug.write(mc.log.CODE.INFO, "hdcP2X : " + hdcP2X + ", hdcP2Y : " + hdcP2Y);
                    #region C2.Result
                    if (mc.hd.reqMode != REQMODE.DUMY && (hdcP2X == -1 || hdcP2Y == -1 || hdcP2T_1 == -1
                        || (hdcP2Score * 100) < hdcP2PassScore)) // HDC Vision Result Error
                    {
                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                        {
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                            //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HDC.jogTeachUse.value == 1)
                            {
                                if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                else JogTeachMode = jogTeachCornerMode.Corner13;
                                sqc = 130; break;
                            }
                            else
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
                            }
                        }
                    }
                    if (Math.Abs(hdcP2X) > 5000)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1} um", hdcP2X));
                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                        {
                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HDC.jogTeachUse.value == 1)
                            {
                                if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                else JogTeachMode = jogTeachCornerMode.Corner13;
                                sqc = 130; break;
                            }
                            else
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_X_Limit");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
                            }
                        }
                    }
                    if (Math.Abs(hdcP2Y) > 5000)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1} um", hdcP2Y));
                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                        {
                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HDC.jogTeachUse.value == 1)
                            {
                                if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                else JogTeachMode = jogTeachCornerMode.Corner13;
                                sqc = 130; break;
                            }
                            else
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_Y_Limit");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
                            }
                        }
                    }
                    if (Math.Abs(hdcP2T_1) > 5 || Math.Abs(hdcP2T_2) > 5)
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1} degree", hdcP2T_1));
                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                        {
                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
                            sqc = 120; break;
                        }
                        else
                        {
                            if (mc.para.HDC.jogTeachUse.value == 1)
                            {
                                if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                else JogTeachMode = jogTeachCornerMode.Corner13;
                                sqc = 130; break;
                            }
                            else
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C3_T_Limit");
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T_1);
                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
                            }
                        }
                    }
                    #endregion

                    sqc++; break;

                case 52:
                    // 160629. jhlim : 프로젝션 일 경우에만 이렇게 하기
                    //{
                    //    hdcX = (hdcP1X + hdcP2X) / 2;
                    //    hdcY = (hdcP1Y + hdcP2Y) / 2;
                    //    hdcT = (hdcP1T_1 + hdcP2T_1) / 2;

                    //    // 각도를 신뢰할 수 없으니 최대 최소 빼고 평균을 내자.
                    //    tempValue[0] = hdcP1T_1;
                    //    tempValue[1] = hdcP1T_2;
                    //    tempValue[2] = hdcP2T_1;
                    //    tempValue[3] = hdcP2T_2;
                    //    hdcT = (tempValue.Sum() - (tempValue.Min() + tempValue.Max())) / 2;
                    //}

                    if (!setJogTeach)
                    {
                        if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            hdcT = hdcP2T_1;
                            #region Corner 1
                            double refT = Math.Atan2(-mc.para.HDC.modelPADC1.patternPos.y.value, -mc.para.HDC.modelPADC1.patternPos.x.value) + hdcT * Math.PI / 180;
                            double dist = Math.Sqrt(mc.para.HDC.modelPADC1.patternPos.x.value * mc.para.HDC.modelPADC1.patternPos.x.value
                                + mc.para.HDC.modelPADC1.patternPos.y.value * mc.para.HDC.modelPADC1.patternPos.y.value);
                            double cX = dist * Math.Cos(refT);
                            double cY = dist * Math.Sin(refT);
                            double diffX = cX + mc.para.HDC.modelPADC1.patternPos.x.value;
                            double diffY = cY + mc.para.HDC.modelPADC1.patternPos.y.value;
                            hdcP1X = hdcP1X + diffX;
                            hdcP1Y = hdcP1Y + diffY;
                            #endregion

                            #region Corner 3
                            refT = Math.Atan2(-mc.para.HDC.modelPADC3.patternPos.y.value, -mc.para.HDC.modelPADC3.patternPos.x.value) + hdcT * Math.PI / 180;
                            dist = Math.Sqrt(mc.para.HDC.modelPADC3.patternPos.x.value * mc.para.HDC.modelPADC3.patternPos.x.value
                                + mc.para.HDC.modelPADC3.patternPos.y.value * mc.para.HDC.modelPADC3.patternPos.y.value);
                            cX = dist * Math.Cos(refT);
                            cY = dist * Math.Sin(refT);
                            diffX = cX + mc.para.HDC.modelPADC3.patternPos.x.value;
                            diffY = cY + mc.para.HDC.modelPADC3.patternPos.y.value;
                            hdcP2X = hdcP2X + diffX;
                            hdcP2Y = hdcP2Y + diffY;
                            #endregion

                            hdcX = (hdcP1X + hdcP2X) / 2;
                            hdcY = (hdcP1Y + hdcP2Y) / 2;

                            //double tmpX = mc.hd.tool.cPos.x.PAD(padX) + hdcP1X;
                            //double tmpX2 = mc.hd.tool.cPos.x.PAD(padX) + hdcP2X;
                            //hdcX = tmpX2 - tmpX;

                            //double tmpY = mc.hd.tool.cPos.x.PAD(padY) + hdcP1Y;
                            //double tmpY2 = mc.hd.tool.cPos.x.PAD(padY) + hdcP2Y;
                            //hdcY = tmpY2 - tmpY;
                        }
                        else
                        {
                            hdcX = (hdcP1X + hdcP2X) / 2;
                            hdcY = (hdcP1Y + hdcP2Y) / 2;
                            hdcT = (hdcP1T_1 + hdcP2T_1) / 2;
                        }

                        #region PCB Position Error Check
                        tempSb.Clear(); tempSb.Length = 0;
                        tempSb.AppendFormat("HDC[{0},{1}] Package X,Y,T : {2}, {3}, {4}", padX, padY, Math.Round(hdcX), Math.Round(hdcY), Math.Round(hdcT));
                        mc.log.debug.write(mc.log.CODE.INFO, tempSb.ToString());
                        if (Math.Abs(hdcX) > mc.para.MT.padCheckCenterLimit.value * 2)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package X Position Limit Error : {0:F1} um", hdcX));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                    else JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_XPos_Over");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center X: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcX, mc.para.MT.padCheckCenterLimit.value);
                                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center X: " + Math.Round(hdcX, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_XRESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcY) > mc.para.MT.padCheckCenterLimit.value)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package Y Position Limit Error : {0:F1}um", hdcY));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    if (mc.para.HDC.detectDirection.value == 0) JogTeachMode = jogTeachCornerMode.Corner24;
                                    else JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_Packege_YPos_Over");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center Y: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcY, mc.para.MT.padCheckCenterLimit.value);
                                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center Y: " + Math.Round(hdcY, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_YRESULT_OVER); break;
                                }
                            }
                        }
                        #endregion

                        #region 이게 정답인거 같은데..
                        DPOINT ulc_ct, pt1, pt2;
                        ulc_ct.x = -mc.para.CAL.Tool_Con[mc.hd.order.bond].x.value;
                        ulc_ct.y = -mc.para.CAL.Tool_Con[mc.hd.order.bond].y.value;
                        pt1.x = calcULCX[mc.hd.order.bond];
                        pt1.y = calcULCY[mc.hd.order.bond];

                        Calc.rotate(-ulcP2T, ulc_ct, pt1, out pt2);

                        calcULCX[mc.hd.order.bond] = pt2.x;
                        calcULCY[mc.hd.order.bond] = pt2.y;

                        placeX -= calcULCX[mc.hd.order.bond];
                        placeY -= calcULCY[mc.hd.order.bond];
                        if (mc.hd.order.bond == (int)UnitCodeHead.HD2) placeT = tPos.t[mc.hd.order.bond].ZERO - calcULCT[mc.hd.order.bond] + hdcT + mc.para.HD.place.offset2.t.value + mc.swcontrol.placeOffset_HD2T;
                        else placeT = tPos.t[mc.hd.order.bond].ZERO - calcULCT[mc.hd.order.bond] + hdcT + mc.para.HD.place.offset.t.value + mc.swcontrol.placeOffset_HD1T;

                        placeX += hdcX;
                        placeY += hdcY;
                        #endregion
                    }
                    else
                    {
                        setJogTeach = false;

                        #region 이게 정답인거 같은데..
                        DPOINT ulc_ct, pt1, pt2;
                        ulc_ct.x = -mc.para.CAL.Tool_Con[mc.hd.order.bond].x.value;
                        ulc_ct.y = -mc.para.CAL.Tool_Con[mc.hd.order.bond].y.value;
                        pt1.x = calcULCX[mc.hd.order.bond];
                        pt1.y = calcULCY[mc.hd.order.bond];

                        Calc.rotate(-ulcP2T, ulc_ct, pt1, out pt2);

                        calcULCX[mc.hd.order.bond] = pt2.x;
                        calcULCY[mc.hd.order.bond] = pt2.y;

                        placeX -= calcULCX[mc.hd.order.bond];
                        placeY -= calcULCY[mc.hd.order.bond];
                        if (mc.hd.order.bond == (int)UnitCodeHead.HD2) placeT = tPos.t[mc.hd.order.bond].ZERO - calcULCT[mc.hd.order.bond] + hdcT + mc.para.HD.place.offset2.t.value + mc.swcontrol.placeOffset_HD2T;
                        else placeT = tPos.t[mc.hd.order.bond].ZERO - calcULCT[mc.hd.order.bond] + hdcT + mc.para.HD.place.offset.t.value + mc.swcontrol.placeOffset_HD1T;

                        placeX += hdcX;
                        placeY += hdcY;
                        #endregion
                    }

                    //double cosTheta, sinTheta;
                    //cosTheta = Math.Cos((-calcULCT[mc.hd.order.bond]) * Math.PI / 180);
                    //sinTheta = Math.Sin((-calcULCT[mc.hd.order.bond]) * Math.PI / 180);
                    //calcULCX[mc.hd.order.bond] = (cosTheta * calcULCX[mc.hd.order.bond]) - (sinTheta * calcULCY[mc.hd.order.bond]);
                    //calcULCY[mc.hd.order.bond] = (sinTheta * calcULCX[mc.hd.order.bond]) + (cosTheta * calcULCY[mc.hd.order.bond]);

                    #region Test1
                    //DPOINT ct, pt1, pt2;
                    //ct.x = mc.para.CAL.Tool_Con[mc.hd.order.bond].x.value;
                    //ct.y = mc.para.CAL.Tool_Con[mc.hd.order.bond].y.value;
                    //pt1.x = calcULCX[mc.hd.order.bond];
                    //pt1.y = calcULCY[mc.hd.order.bond];

                    //Calc.rotate(-ulcP2T, ct, pt1, out pt2);

                    //placeX -= pt2.x;
                    //placeY -= pt2.y;
                    ///////////////////////////////////////////////////////////////////
                    //pt1.x = hdcX;
                    //pt1.y = hdcY;

                    //Calc.rotate(hdcT, ct, pt1, out pt2);

                    //placeX += pt2.x;
                    //placeY += pt2.y;

                    //placeT = tPos.t[mc.hd.order.bond].ZERO - calcULCT[mc.hd.order.bond] + hdcT + mc.para.HD.place.offset.t.value;
                    #endregion

                    if (padX < 0 || padY < 0)
                    {
                        errorCheck(ERRORCODE.HD, sqc, "Array Index Error : X-" + padX.ToString() + " Y-" + padY.ToString()); break;
                    }
                    placeX += mc.para.CAL.place[padX, padY].x.value;
                    placeY += mc.para.CAL.place[padX, padY].y.value;

                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 0);

                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    Y.move(placeY, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    X.move(placeX, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    T[mc.hd.order.bond].move(placeT, out ret.message); if (mpiCheck(T[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 53:
                    if (timeCheck(UnitCodeAxis.X, sqc, 3)) break;
                    X.actualPosition(out ret.d, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(placeX - ret.d) > 3000) break;
                    dwell.Reset();
                    sqc++; break;
                case 54:
                    if (timeCheck(UnitCodeAxis.Y, sqc, 3)) break;
                    Y.actualPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(placeY - ret.d) > 3000) break;
                    dwell.Reset();
                    sqc++; break;
                case 55:
                    if (timeCheck(UnitCodeAxis.T, sqc, 3)) break;
                    T[mc.hd.order.bond].actualPosition(out ret.d, out ret.message); if (mpiCheck(T[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
                    //if (Math.Abs(placeT - ret.d) > 3) break;
                    dwell.Reset();
                    sqc++; break;
                case 56:
                    if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.bond)) break;
                    mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 1);
                    sqc = 60; break;
                #endregion

                #region case 60 z down
                case 60:
                    mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);
                    mc.log.workHistory.write("---------------> Start Attach(#" + (int)mc.hd.order.bond + ")");

                    mc.loadCell.setZero(mc.hd.order.bond);
                    // 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
                    if (mc.hd.reqMode == REQMODE.DUMY && mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON) posZ = tPos.z[mc.hd.order.bond].DRYRUNPLACE;
                    else posZ = tPos.z[mc.hd.order.bond].PLACE;

                    posZ -= mc.para.CAL.place[padX, padY].z.value;

                    forceTargetZPos = posZ;

                    contactPointSearchDone = false;
                    forceStartPointSearchDone = false;
                    forceStartPointCheckCount = 0;
                    linearAutoTrackStart = false;


                    // Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
                    if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
                    {
                        diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search2.force.value;
                        //if (graphDisplayPoint == 2)
                        //	diffForce = mc.para.HD.place.force.value;
                    }
                    else
                    {
                        diffForce = mc.para.HD.place.force.value - mc.para.HD.place.search.force.value;
                        //if (graphDisplayPoint == 2)
                        //	diffForce = mc.para.HD.place.force.value;
                    }

                    if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
                    {
                        diffForce = 0.001;
                    }

                    #region pos set
                    if (mc.hd.reqMode == REQMODE.DUMY && mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON) posZ = tPos.z[mc.hd.order.bond].DRYRUNPLACE;
                    else posZ = tPos.z[mc.hd.order.bond].PLACE;

                    // 최종 target force
                    posZ += mc.para.CAL.place[padX, padY].z.value;

                    mc.log.debug.write(mc.log.CODE.INFO, "Place Offset Value : " + mc.para.CAL.place[padX, padY].z.value.ToString());
                    if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
                    {
                        levelS1 = mc.para.HD.place.search.level.value;
                        delayS1 = mc.para.HD.place.search.delay.value;
                        velS1 = (mc.para.HD.place.search.vel.value) / 1000;
                        accS1 = mc.para.HD.place.search.acc.value;
                    }
                    else
                    {
                        levelS1 = 0;
                        delayS1 = 0;
                    }
                    if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
                    {
                        levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
                        delayS2 = mc.para.HD.place.search2.delay.value;
                        velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
                        accS2 = mc.para.HD.place.search2.acc.value;
                    }
                    else
                    {
                        levelS2 = 0;
                        delayS2 = 0;
                    }
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    {
                        delay = mc.para.HD.place.delay.value + mc.para.HD.place.suction.purse.value;
                    }
                    else
                    {
                        delay = mc.para.HD.place.delay.value;
                    }
                    #endregion
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

                    // clear loadcell graph data & time
                    //if (UtilityControl.graphDisplayEnabled == 1)
                    //{
                    //    graphDispStart = true;
                    //    EVENT.clearLoadcellData();
                    //}
                    //else graphDispStart = false;

                    loadTime.Reset();
                    graphDisplayIndex = 0;
                    meanFilterIndex = 0;

                    // initialize place-time force check variables...
                    placeForceCheckCount = 0;
                    placeForceOver = false;
                    placeForceUnder = false;
                    placeSensorForceCheckCount = 0;

                    placeForceSumCount = 0;
                    placeForceSum = 0;
                    placeForceMin = 0;
                    placeForceMax = 0;

                    if (levelS1 != 0)
                    {
                        Z[mc.hd.order.bond].move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
                        //Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
                        Z[mc.hd.order.bond].move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
                        if (delayS1 == 0) { sqc += 3; break; }
                    }
                    else
                    {
                        Z[mc.hd.order.bond].move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
                        sqc += 3; break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 61:
                    //DisplayGraph(0);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_TARGET(mc.hd.order.bond)) break;
                    //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint == 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
                    //if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
                    //{
                    //    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    //}
                    dwell.Reset();
                    sqc++; break;
                case 62:
                    //DisplayGraph(0);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
                    if (graphDisplayPoint == 0)
                    {
                        DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);
                    }
                    sqc++; break;
                case 63:
                    // clear loadcell graph data & time
                    if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
                    {
                        loadTime.Reset();
                        graphDisplayIndex = 0;
                    }

                    if (levelS2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.bond].move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;	// search2 move start
                    if (levelD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 64:
                    // Search2 구간에서 Contact이 발생한다.
                    Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
                    if (mc.hd.reqMode == REQMODE.DUMY && mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON) contactPos = tPos.z[mc.hd.order.bond].DRYCONTACTPOS;
                    else contactPos = tPos.z[mc.hd.order.bond].CONTACTPOS;
                    if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
                    {
                        if (graphDisplayPoint == 2) loadTime.Reset();
                        graphDisplayIndex = 0;
                        contactPointSearchDone = true;
                    }
                    //if (contactPointSearchDone) DisplayGraph(2);
                    //else DisplayGraph(1);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_TARGET(mc.hd.order.bond)) break;		// Search2 구간까지 완료된 경우.
                    
                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);

                    if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
                    {
                        loadTime.Reset();
                        graphDisplayIndex = 0;
                    }

                    dwell.Reset();
                    forceTime.Reset();
                    //mc.log.debug.write(mc.log.CODE.ETC, "start");
                    sqc++; break;
                case 65:
                    //DisplayGraph(3, false, false, true);
                    ////if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
                    //{
                    //    try
                    //    {
                    //        double slopeforce;
                    //        slopeforce = (forceTime.Elapsed * diffForce / delayS2) + mc.para.HD.place.search2.force.value;
                    //        if (slopeforce > 0)
                    //        {
                    //            if ((graphDisplayIndex % graphDisplayCount) == 0)
                    //            {
                    //                if(UtilityControl.forceTopLoadcellBaseForce ==0)
                    //                    mc.hd.tool.F.kilogram(slopeforce, out ret.message);// if (ioCheck(sqc, ret.message)) break;
                    //                else
                    //                    mc.hd.tool.F.kilogram(slopeforce, out ret.message, true);// if (ioCheck(sqc, ret.message)) break;
                    //                //mc.log.debug.write(mc.log.CODE.CAL, Math.Round(slopeforce, 3).ToString());
                    //            }
                    //        }
                    //    }
                    //    catch
                    //    {
                    //        mc.log.debug.write(mc.log.CODE.EVENT, "load calc2 strange.");
                    //    }
                    //}

                    if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
                    //mc.log.debug.write(mc.log.CODE.ETC, "end");
                    //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)	EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

                    //if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
                    //{
                    //    if (mc.hd.reqMode != REQMODE.DUMY && mc.para.ETC.usePlaceForceTracking.value == 1)
                    //    {
                    //        if (UtilityControl.forceTopLoadcellBaseForce == 0)
                    //        {
                    //            mc.hd.tool.F.kilogram(mc.para.HD.place.force.value + mc.para.HD.place.placeForceOffset.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    //        }
                    //        else
                    //        {
                    //            mc.hd.tool.F.kilogram(mc.para.HD.place.force.value + mc.para.HD.place.placeForceOffset.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
                    //        }

                    //    }
                    //    else
                    //    {
                    //        if (UtilityControl.forceTopLoadcellBaseForce == 0)
                    //        {

                    //            mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    //        }
                    //        else
                    //        {
                    //            mc.hd.tool.F.kilogram(mc.para.HD.place.force.value, out ret.message, true); if (ioCheck(sqc, ret.message)) break;
                    //        }
                    //    }
                    //}

                    dwell.Reset();
                    sqc++; break;
                case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
                    //DisplayGraph(3);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_TARGET(mc.hd.order.bond)) break;

                    dwell.Reset();
                    sqc++; break;
                case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
                    //DisplayGraph(3);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_DONE(mc.hd.order.bond)) break;

                    DisplayPoint(mc.hd.order.bond, useTopLoadcell);     // 컨텍 된 시점
                    
                    double placeForce = mc.para.HD.place.force.value;
                    
                    if (mc.hd.reqMode != REQMODE.DUMY && mc.para.ETC.usePlaceForceTracking.value == 1) placeForce += (mc.para.HD.place.placeForceOffset[mc.hd.order.bond].value) * 0.8;

                    double forceHeight = F.kilogram2Height(mc.hd.order.bond, placeForce, out ret.message, false);
                    mc.log.debug.write(mc.log.CODE.INFO, "Force Height : " + forceHeight.ToString());
                    if (ret.message == RetMessage.OK)
                    {
                        double posFinalZ = posZ - forceHeight;
                        Z[mc.hd.order.bond].move(posFinalZ, velS2, accS2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;	// search2 move start
                    }
                    dwell.Reset();
                    sqc++; break;

                case 68:
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_TARGET(mc.hd.order.bond)) break;

                    dwell.Reset();
                    sqc++; break;

                case 69:
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (!Z_AT_DONE(mc.hd.order.bond)) break;

                    DisplayPoint(mc.hd.order.bond, useTopLoadcell);     // Force 높이에 도착한 시점

                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
                    mc.OUT.HD.SUC(mc.hd.order.bond, out ret.b, out ret.message); ioCheck(sqc, ret.message);
                    if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
                    {
                        // Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
                        Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
                        mc.log.debug.write(mc.log.CODE.WARN, String.Format("Check Place Suction Mode-Cmd:{0:F0} ![<]Cur: {1:F0}", ret.d, posZ + mc.para.HD.place.suction.level.value));
                        mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
                    }
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
                    {
                        mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
                    }

                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);
                    dwell.Reset();
                    if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
                    sqc++; break;
                case 70:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
                    //DisplayGraph(3);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    // X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
                    //if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
                    mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);

                    if (mc.para.HD.place.forceTracking.enable.value > 0)
                    {
                        double trackingForce = mc.para.HD.place.force.value + mc.para.HD.place.forceTracking.force.value;
                        double trackingHeight = F.kilogram2Height(mc.hd.order.bond, trackingForce, out ret.message, false);
                        double trackingSpeed = (mc.para.HD.place.forceTracking.vel.value) / 1000;
                        double trackingAcc = accS2 / 15;

                        mc.log.debug.write(mc.log.CODE.INFO, "Tracking Height : " + trackingHeight.ToString() + ", Tracking Speed : " + trackingSpeed.ToString() + ", Tracking Acc : " + trackingAcc.ToString());
                        if (ret.message == RetMessage.OK)
                        {
                            double trackingZ = posZ - trackingHeight;
                            Z[mc.hd.order.bond].move(trackingZ, trackingSpeed, trackingAcc, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;	// search2 move start
                        }
                    }

                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
                    {
                        mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
                        sqc++;
                    }
                    else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
                    {
                        sqc += 2;
                    }
                    // PLACE_UP_OFF는 UP timing에 동작한다.
                    else
                    {
                        sqc = 74;
                    }
                    break;
                case 71:	// Blow Time 대기 시간..
                    //DisplayGraph(3);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
                    mc.OUT.HD.BLW(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);
                    //mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    //PreForce = ret.d1;
                    //writedone = false;
                    sqc++; break;
                case 72:	// suction off delay
                    //DisplayGraph(3, true, true);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    // target force를 형성하기 위한 feedback control을 시작한다.
                    //if (forceStartPointSearchDone)
                    //{
                        //if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
                        //if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
                        //if (autoTrackDelayTime.Elapsed < mc.para.HD.place.delay.value) break;
                    //}
                    //else
                    {
                        //if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
                        //if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                        if (dwell.Elapsed < mc.para.HD.place.delay.value) break;

                    }

                    //ret.d2 = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d2)) break;

                    mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);
                    sqc += 2; break;
                case 73:	// Blow delay
                    //DisplayGraph(3);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (forceStartPointSearchDone)
                    {
                        if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
                        mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow On {0:F0}", autoTrackDelayTime.Elapsed));
                    }
                    else
                    {
                        if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
                        mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow On {0:F0}", dwell.Elapsed));
                    }
                    mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);
                    sqc++; break;
                case 74:
                    //if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    //{
                    //    if (UtilityControl.graphEndPoint >= 1)
                    //    {
                    //        if (dwell.Elapsed < 500) DisplayGraph(4);
                    //        else DisplayGraph(4, true, true);
                    //    }
                    //    else
                    //    {
                    //        DisplayGraph(4, true, true, false, false);
                    //    }
                    //}
                    //else
                    //{
                    //    DisplayGraph(3, true, true);
                    //}
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond, useTopLoadcell);

                    if (dwell.Elapsed < delay - 3) break;

                    if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    {
                        mc.OUT.HD.BLW(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        mc.log.debug.write(mc.log.CODE.INFO, String.Format("COMP : Blow Off {0:F0}", dwell.Elapsed), false);
                    }

                    DisplayGraphPoint(mc.hd.order.bond, useTopLoadcell);

                    double LDValue = mc.loadCell.getData(mc.hd.order.bond);
                    if (LDValue == -9999)
                    {
                        LDValue = mc.loadCell.getData(mc.hd.order.bond);
                        if (LDValue == -9999)
                        {
                            LDValue = mc.loadCell.getData(mc.hd.order.bond);
                        }
                    }

                    placeForce = LDValue;
                    //placeForce = F.TopLDToBottomLD(mc.hd.order.bond, LDValue, out ret.message);

                    //mc.AIN.SG(out ret.d2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    //mc.hd.tool.F.voltage2kilogram(ret.d2, out ret.d3, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    // Load ON 정상..

                    //EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

                    attachError = 0;
                    // 					mc.hd.homepickdone = false;		// Attach 했으니 false;

                    if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
                    {
                        // Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
                        if (placeForce < mc.para.ETC.placeForceLowLimit.value)
                        {
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}[kg] : UNDER PRESS", (padX + 1), (padY + 1), placeForce);
                            mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
                            mc.para.HD.place.placeForceOffset[mc.hd.order.bond].value = 0;
                            attachError = 1;
                        }
                        else if (placeForce > mc.para.ETC.placeForceHighLimit.value)
                        {
                            tempSb.Clear(); tempSb.Length = 0;
                            tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}[kg] : OVER PRESS", (padX + 1), (padY + 1), placeForce);
                            mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
                            mc.para.HD.place.placeForceOffset[mc.hd.order.bond].value = 0;
                            attachError = 2;
                        }
                    }
                    if (attachError == 0)
                    {
                        tempSb.Clear(); tempSb.Length = 0;
                        tempSb.AppendFormat("Attach Done - X[{0}], Y[{1}], Force : {2:F2}[kg]", (padX + 1), (padY + 1), placeForce);
                        mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
                        mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
                        if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }
                        if (mc.hd.reqMode != REQMODE.DUMY && mc.para.ETC.usePlaceForceTracking.value == 1)
                        {
                            double forceDiff = Math.Round(mc.para.HD.place.force.value - placeForce, 2);
                            if(Math.Abs(forceDiff) > 0.05)
                            {
                                mc.para.HD.place.placeForceOffset[mc.hd.order.bond].value = forceDiff;
                                mc.log.debug.write(mc.log.CODE.INFO, "Force Auto Tracking : " + forceDiff.ToString() + "[kg]");
                            }
                            else
                            {
                                mc.log.debug.write(mc.log.CODE.INFO, "Ignore Force Auto Tracking : " + forceDiff.ToString() + "[kg]");
                            }
                        }
                    }

                    // SVID Send..
                    mc.commMPC.SVIDReport();

                    mc.board.write(BOARD_ZONE.WORKING, out ret.b);
                    if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

                    // 일단 Attach 끝난 다음 판단 하여 Map에 안 쓰는 경우를 방지해야 한다.
                    //if (attachError == 0 && mc.para.ETC.usePlaceForceTracking.value == 1 && mc.full.reqMode == REQMODE.AUTO)
                    //{
                    //    placeForceMean = Math.Round((placeForceSum - (placeForceMax + placeForceMin)) / (placeForceSumCount - 2), 3) + mc.swcontrol.forceMeanOffset;		// min, max 를 빼고 평균값을 구한다.

                    //    if (placeForceSum < 0.1 || placeForceMean < 0.1)
                    //    {
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("Force Check Value Error - Sum:{0}, Mean{1}", placeForceSum, placeForceMean);
                    //        placeForceMean = mc.para.HD.place.force.value;
                    //        mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                    //    }
                    //    if (Math.Abs(mc.para.HD.place.force.value - placeForceMean) >= 0.1)			// Offset 값이 0 아니며 0.1 kg 차이나면 진짜 에러(0일 경우는 보정 리셋인 경우이므로 무시)
                    //    {	// 0일 경우는 Under / Over Press 일 경우..
                    //        mc.para.HD.place.placeForceOffset.value = 0;
                    //        mc.log.debug.write(mc.log.CODE.FAIL, textResource.LOG_DEBUG_HD_FORCE_TRACKING_INIT, false);

                    //        mc.hd.order.set(mc.hd.order.bond, (int)ORDER.BOND_FAIL);

                    //        sqc = 150;
                    //        break;
                    //    }
                    //    if (Math.Abs((mc.para.HD.place.force.value - placeForceMean) * 0.5) >= 0.01)		// offset 값이 20g(10gx2) 이하로 차이날 경우에는 무시.
                    //    {
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        mc.para.HD.place.placeForceOffset.value += Math.Round((mc.para.HD.place.force.value - placeForceMean) / 2, 3);		// 차이의 절반을 기존값에 더한다.
                    //        tempSb.AppendFormat("Force Mean : {0}[kg] Force Offset : {0}[kg]", placeForceMean, mc.para.HD.place.placeForceOffset.value);
                    //        //mc.log.debug.write(mc.log.CODE.FORCE, "Force Mean : " + Math.Round(placeForceMean, 3) + " (kg)/" + "Force Offset : " + mc.para.HD.place.placeForceOffset.value + " (kg)");
                    //        mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString(), false);
                    //    }
                    //    else
                    //    {
                    //        tempSb.Clear(); tempSb.Length = 0;
                    //        tempSb.AppendFormat("Force Offset 값이 너무 작아서 무시 합니다. 현재값 : {0}[kg]", mc.para.HD.place.placeForceOffset.value);
                    //        mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString(), false);
                    //    }
                    //    //placeForceMean = 0;		// clear
                    //}
                    sqc++; break;
                case 75:
                    mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
                    mc.log.workHistory.write("---------------> End Attach(#" + (int)mc.hd.order.bond + ")");
                    if ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0)
                    {	// 에러인 경우 Up Position 으로 이동
                        mc.hd.order.set(mc.hd.order.bond, (int)ORDER.BOND_FAIL);
                        sqc++;
                    }
                    else
                    {
                        mc.hd.order.set(mc.hd.order.bond, (int)ORDER.BOND_SUCESS);
                        if (mc.hd.order.bond != (int)ORDER.EMPTY)
                        {
                            //double value = mc.para.MT.padCount.x.value / mc.activate.headCnt;
                            int tempIndex = padX + 2;// (int)Math.Ceiling(value);
                            if (tempIndex >= mc.para.MT.padCount.x.value)
                            {
                                mc.board.padIndex(out padX, out padY, out ret.b);
                                if (!ret.b)
                                {
                                    sqc = SQC.STOP;
                                    break;
                                }
                                else
                                {
                                    //if (remainCount <= mc.para.MT.padCount.y.value) mc.pd.singleUp = true;
                                    sqc = 200; break;
                                }
                            }
                            else
                            {
                                PAD_STATUS state = mc.board.padStatus(BOARD_ZONE.WORKING, tempIndex, padY);
                                //mc.board.padStatus(BOARD_ZONE.WORKING, tempIndex, padY, PAD_STATUS.READY, out ret.b);
                                if (state == PAD_STATUS.READY)
                                {
                                    padX = tempIndex;
                                }
                                else
                                {
                                    mc.board.padIndex(out padX, out padY, out ret.b);
                                    if (!ret.b)
                                    {
                                        sqc = SQC.STOP;
                                        break;
                                    }
                                    else
                                    {
                                        sqc = 200; break;
                                    }
                                }
                            }
                            sqc = 200; break;
                        }
                        sqc = SQC.STOP;
                    }
                    break;
                case 76:	// Move Z Up to Safety Position
                    mc.para.HD.place.placeForceOffset[mc.hd.order.bond_fail].value = 0;
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
                    Z[mc.hd.order.bond_fail].move(tPos.z[mc.hd.order.bond_fail].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_fail].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 77:
                    if (!Z_AT_TARGET(mc.hd.order.bond_fail)) break;
                    dwell.Reset();
                    sqc++; break;
                case 78:
                    if (!Z_AT_DONE(mc.hd.order.bond_fail)) break;
                    //mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
                    //string errmessage;
                    tempSb.Clear(); tempSb.Length = 0;
                    tempSb.AppendFormat("X[{0}],Y[{1}]", (padX + 1), (padY + 1));
                    //errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
                    if (attachError == 1)
                    {
                        placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
                        mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
                    }
                    else if (attachError == 2)
                    {
                        placeResult = PAD_STATUS.ATTACH_OVERPRESS;
                        mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
                    }

                    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);

                    sqc = SQC.STOP; break;

                #endregion

                #region case 80 xy pad c2 move(Retry Mode)
                case 80:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC1(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC1(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC2(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC2(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    sqc++; break;
                case 81:
                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        #region HDC.PADC1.req
                        hdcP1X = 0;
                        hdcP1Y = 0;
                        hdcP1T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC1.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }
                    else
                    {
                        #region HDC.PADC2.req
                        hdcP1X = 0;
                        hdcP1Y = 0;
                        hdcP1T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC2.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC2.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }

                    dwell.Reset();
                    sqc++; break;
                case 82:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 83:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;
                    sqc++; break;
                case 84:
                    sqc = 90; break;
                #endregion

                #region case 90 triggerHDC
                case 90:
                    if (mc.hdc.req == false) { sqc = 100; break; }
                    dwell.Reset();
                    sqc++; break;
                case 91:
                    if (dwell.Elapsed < 15) break; // head camera delay
                    if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 92:
                    if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
                    dwell.Reset();
                    sqc++; break;
                case 93:
                    if (dwell.Elapsed < 300) break;
                    sqc = 100; break;
                #endregion

                #region case 100 xy pad c4 move(Retry Mode)
                case 100:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC3(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC3(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER || mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            usePatternPos = false;
                        }
                        else
                        {
                            usePatternPos = true;
                        }

                        Y.move(cPos.y.PADC4(padY, usePatternPos), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.move(cPos.x.PADC4(padX, usePatternPos), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    }

                    sqc++; break;
                case 101:
                    if (mc.hdc.RUNING) break;
                    if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if (mc.para.HDC.detectDirection.value == 0)
                    {
                        #region HDC.PADC1.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
                            }
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
                            }
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                        if (mc.hd.reqMode != REQMODE.DUMY && hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
                        {
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
                                }
                            }
                        }

                        if (Math.Abs(hdcP1X) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_X_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1Y) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, "HDC P1-Y Compensation Amount Limit Error : " + Math.Round(hdcP1Y).ToString() + " um");
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_Y_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1T_1) > 5 || Math.Abs(hdcP1T_2) > 5)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner13;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C1_T_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
                                }
                            }
                        }
                        #endregion
                        #region HDC.PADC3.req
                        hdcP2X = 0;
                        hdcP2Y = 0;
                        hdcP2T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC3.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }
                    else
                    {
                        #region HDC.PADC2.result
                        if (mc.hd.reqMode == REQMODE.DUMY) { }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
                            }
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                            {
                                hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
                                hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
                                hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
                            }
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
                            hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
                            hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                        if (mc.hd.reqMode != REQMODE.DUMY && hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
                        {
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                tempSb.Clear(); tempSb.Length = 0;
                                tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
                                //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
                                mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner24;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1X) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner24;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_X_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1Y) > 5000)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_Y_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner24;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C21_Y_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
                                }
                            }
                        }
                        if (Math.Abs(hdcP1T_1) > 5 || Math.Abs(hdcP1T_2) > 5)
                        {
                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
                            {
                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
                                sqc = 120; break;
                            }
                            else
                            {
                                if (mc.para.HDC.jogTeachUse.value == 1)
                                {
                                    JogTeachMode = jogTeachCornerMode.Corner24;
                                    sqc = 130; break;
                                }
                                else
                                {
                                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrabImage("HDC_C2_T_Limit");
                                    tempSb.Clear(); tempSb.Length = 0;
                                    tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
                                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
                                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
                                }
                            }
                        }
                        #endregion
                        #region HDC.PADC4.req
                        hdcP2X = 0;
                        hdcP2Y = 0;
                        hdcP2T_1 = 0;
                        if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                            {
                                mc.hdc.reqMode = REQMODE.FIND_MODEL;
                                mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
                                mc.hdc.reqPassScore = mc.para.HDC.modelPADC4.passScore.value;
                            }
                            else mc.hdc.reqMode = REQMODE.GRAB;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
                        }
                        else mc.hdc.reqMode = REQMODE.GRAB;
                        mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
                        if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                        #endregion
                    }

                    dwell.Reset();
                    sqc++; break;
                case 102:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 103:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    sqc = 110; break;
                #endregion

                #region case 110 triggerHDC
                case 110:
                    if (mc.hdc.req == false) { sqc = 50; break; }
                    dwell.Reset();
                    sqc++; break;
                case 111:
                    if (dwell.Elapsed < 15) break; // head camera delay
                    if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 112:
                    if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
                    dwell.Reset();
                    sqc++; break;
                case 113:
                    if (dwell.Elapsed < 300) break;
                    sqc = 50; break;
                #endregion

                #region case 130, 120 ??
                case 130:
                    if (mc.hdc.RUNING) break;
                    mc.hdc.SetLive(true);
                    mc.hdc.reqMode = REQMODE.LIVE;
                    mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS; mc.hdc.req = true;
                    dwell.Reset();
                    sqc++; break;
                case 131:
                    if (dwell.Elapsed < 100) break;
                    mc.OUT.MAIN.UserBuzzerCtl(true);

                    for (int i = 0; i < 4; i++)
                    {
                        jogTeach.Offset[i].x.value = -1;
                        jogTeach.Offset[i].y.value = -1;
                    }

                    if (JogTeachMode == jogTeachCornerMode.Corner13)
                    {
                        jogTeach.Corner13Teach = true;
                    }
                    else
                    {
                        jogTeach.Corner13Teach = false;
                    }
                    jogTeach.ShowDialog();
                    mc.hdc.SetLive(false);
                    sqc++; break;
                case 132:
                    if (mc.hdc.RUNING) break;
                    if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                    if (jogTeachCancel) // 에러 띄우고 스탑. 
                    {
                        jogTeachCancel = false;
                        errorCheck(ERRORCODE.HDC, sqc, "PCB Align Error");
                        break;
                    }

                    if (jogTeachIgnore)  // 에러 안 띄우고 PCB ERROR 만들고 다음 검사
                    {
                        jogTeachIgnore = false;
                        mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.PCB_ERROR, out ret.b);
                        mc.board.padIndex(out padX, out padY, out ret.b);
                        if (!ret.b)
                        {
                            sqc = SQC.STOP;
                            break;
                        }
                        else
                        {
                            sqc = 10; break;
                        }
                    }

                    //if (JogTeachMode == jogTeachCornerMode.Corner13)
                    //{
                    //    hdcP1T_1 = Math.Acos(jogTeach.HDCP1X / Math.Pow(Math.Pow(jogTeach.HDCP1X, 2) + Math.Pow(jogTeach.HDCP1Y, 2), 0.5)) - Math.Acos(hdcP1X / Math.Pow(Math.Pow(hdcP1X, 2) + Math.Pow(hdcP1Y, 2), 0.5));
                    //    hdcP2T_1 = Math.Acos(jogTeach.HDCP2X / Math.Pow(Math.Pow(jogTeach.HDCP2X, 2) + Math.Pow(jogTeach.HDCP2Y, 2), 0.5)) - Math.Acos(hdcP2X / Math.Pow(Math.Pow(hdcP2X, 2) + Math.Pow(hdcP2Y, 2), 0.5));

                    //    hdcP1X = jogTeach.HDCP1X;
                    //    hdcP1Y = jogTeach.HDCP1Y;
                    //    hdcP2X = jogTeach.HDCP2X;
                    //    hdcP2Y = jogTeach.HDCP2Y;
                    //}
                    //else
                    //{
                    //    hdcP1T_1 = Math.Acos(jogTeach.HDCP1X / Math.Pow(Math.Pow(jogTeach.HDCP1X, 2) + Math.Pow(jogTeach.HDCP1Y, 2), 0.5)) - Math.Acos(hdcP1X / Math.Pow(Math.Pow(hdcP1X, 2) + Math.Pow(hdcP1Y, 2), 0.5));
                    //    hdcP2T_1 = Math.Acos(jogTeach.HDCP2X / Math.Pow(Math.Pow(jogTeach.HDCP2X, 2) + Math.Pow(jogTeach.HDCP2Y, 2), 0.5)) - Math.Acos(hdcP2X / Math.Pow(Math.Pow(hdcP2X, 2) + Math.Pow(hdcP2Y, 2), 0.5));

                    //    hdcP1X = jogTeach.HDCP1X;
                    //    hdcP1Y = jogTeach.HDCP1Y;
                    //    hdcP2X = jogTeach.HDCP2X;
                    //    hdcP2Y = jogTeach.HDCP2Y;
                    //}

                    //placeX = tPos.x[mc.hd.order.bond].PAD(padX);
                    //placeY = tPos.y[mc.hd.order.bond].PAD(padY);
                    //placeT = tPos.t[mc.hd.order.bond].ZERO;

                    //hdcX = (hdcP1X + hdcP2X) / 2;
                    //hdcY = (hdcP1Y + hdcP2Y) / 2;
                    //hdcT = (hdcP1T_1 + hdcP2T_1) / 2;


					#region JogTeach 용 변수
					p1X = 0;
					p1Y = 0;
					p2X = 0;
					p2Y = 0;
					totalP1X = 0;
					totalP1Y = 0;
					totalP2X = 0;
					totalP2Y = 0;
					refAngle = 0;
					realAngle = 0;
					totalAngle = 0;
					setJogTeach = false;
					#endregion

                    hdcP1X = jogTeach.HDCP1X;
					hdcP1Y = jogTeach.HDCP1Y;
					hdcP2X = jogTeach.HDCP2X;
					hdcP2Y = jogTeach.HDCP2Y;
                    
					if (JogTeachMode == jogTeachCornerMode.Corner13)
                    {
                        p1X = mc.hd.tool.cPos.x.PAD(padX) + (mc.para.MT.padSize.x.value * 1000 / 2);
                        p1Y = mc.hd.tool.cPos.y.PAD(padY) + (mc.para.MT.padSize.y.value * 1000 / 2);
                        p2X = mc.hd.tool.cPos.x.PAD(padX) - (mc.para.MT.padSize.x.value * 1000 / 2);
                        p2Y = mc.hd.tool.cPos.y.PAD(padY) - (mc.para.MT.padSize.y.value * 1000 / 2);
                    }
                    else
                    {
						p1X = mc.hd.tool.cPos.x.PAD(padX) + (mc.para.MT.padSize.x.value * 1000 / 2);
						p1Y = mc.hd.tool.cPos.y.PAD(padY) - (mc.para.MT.padSize.y.value * 1000 / 2);
                        p2X = mc.hd.tool.cPos.x.PAD(padX) - (mc.para.MT.padSize.x.value * 1000 / 2);
                        p2Y = mc.hd.tool.cPos.y.PAD(padY) + (mc.para.MT.padSize.y.value * 1000 / 2);
                    }

					totalP1X = p1X + jogTeach.HDCP1X;
					totalP1Y = p1Y + jogTeach.HDCP1Y;
					totalP2X = p2X + jogTeach.HDCP2X;
					totalP2Y = p2Y + jogTeach.HDCP2Y;

                    double resultX = totalP1X + (totalP2X - totalP1X) / 2;
                    double originX = mc.hd.tool.cPos.x.PAD(padX);

                    double resultY = totalP1Y + (totalP2Y - totalP1Y) / 2;
                    double originY = mc.hd.tool.cPos.y.PAD(padY);

                    double totalOffsetX = resultX - originX;
                    double totalOffsetY = resultY - originY;

					refAngle = Math.Atan2((p2Y - p1Y), (p2X - p1X)) * 180 / Math.PI;
					realAngle = Math.Atan2((totalP2Y - totalP1Y), (totalP2X - totalP1X)) * 180 / Math.PI;

                    hdcT = realAngle - refAngle;
                    hdcX = totalOffsetX;
                    hdcY = totalOffsetY;

                    setJogTeach = true;

                    sqc = 52; break;


                case 120:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 121:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    mc.log.debug.write(mc.log.CODE.EVENT, "PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + (mc.hd.tool.hdcfailcount + 1).ToString() + "]");
                    //EVENT.statusDisplay("PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]");
                    mc.hd.tool.hdcfailcount++;

                    if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
                    else sqc = 80;
                    break;
                #endregion

                #region case 150 Error 발생하여 Z축 Up 한 다음 알람 띄우기
                case 150:
                    mc.OUT.HD.SUC(mc.hd.order.bond_fail, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_fail, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 151:
                    if (dwell.Elapsed < 500) break;
                    Z[mc.hd.order.bond_fail].move(mc.hd.tool.tPos.z[mc.hd.order.bond_fail].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_fail].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 152:
                    if (!Z_AT_DONE(mc.hd.order.bond_fail)) break;
                    dwell.Reset();
                    sqc++; break;
                case 153:
                    if (!Z_AT_TARGET(mc.hd.order.bond_fail)) break;
                    errorCheck(ERRORCODE.HD, sqc, "Force 차이가 너무 큽니다. Head 부하량이 변경되었거나 Force Calibraion 을 확인 하세요. 현재 차이값 : " + (mc.para.HD.place.force.value - placeForceMean) + " (kg)");
                    break;

                //???
                #endregion

                #region case 200 place to place

                #region case 200 Z move up
                case 200:
                    #region pos set
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
                    Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
                    {
                        levelD1 = mc.para.HD.place.driver.level.value;
                        delayD1 = mc.para.HD.place.driver.delay.value;
                        if (delayD1 == 0) delayD1 = 1;
                        velD1 = (mc.para.HD.place.driver.vel.value / 1000);
                        accD1 = mc.para.HD.place.driver.acc.value;
                    }
                    else
                    {
                        levelD1 = 0;
                        delayD1 = 0;
                    }
                    if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
                    {
                        levelD2 = mc.para.HD.place.driver2.level.value;
                        delayD2 = mc.para.HD.place.driver2.delay.value;
                        velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
                        accD2 = mc.para.HD.place.driver2.acc.value;
                    }
                    else
                    {
                        levelD2 = 0;
                        delayD2 = 0;
                    }
                    #endregion
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
                    sqc++; break;
                case 201:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    //if (delayD1 == 0) { sqc += 3; break; }
                    if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
                    dwell.Reset();
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
                    {
                        sqc++;
                    }
                    else
                    {
                        sqc += 3;
                    }
                    break;
                case 202:	// suction off & blow on
                    //if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
                    sqc++; break;
                case 203:	// blow off
                    //if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
                    sqc++; break;
                case 204:
                    //if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    #region Z.AT_TARGET
                    if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
                    Z[mc.hd.order.bond_done].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (ret.b)
                    {
                        Z[mc.hd.order.bond_done].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                        break;
                    }
                    Z[mc.hd.order.bond_done].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (!ret.b) break;
                    #endregion
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
                    dwell.Reset();
                    sqc++; break;
                case 205:
                    //if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (dwell.Elapsed < delayD1) break;
                    if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
                    {
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
                    }
                    //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
                    sqc++; break;
                case 206:
                    //if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (levelD2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 207:
                    //if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    #region Z.AT_TARGET
                    if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
                    Z[mc.hd.order.bond_done].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (ret.b)
                    {
                        Z[mc.hd.order.bond_done].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                        break;
                    }
                    Z[mc.hd.order.bond_done].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    if (!ret.b) break;
                    #endregion
                    dwell.Reset();
                    sqc++; break;
                case 208:
                    //if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (dwell.Elapsed < delayD2) break;
                    //if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 2) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
                    sqc++; break;
                case 209:
                    Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    //sqc = 20; 
                    pickretrycount = 0;
                    // 160525. jhlim 
                    mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
                    mc.hd.order.set(mc.hd.order.bond_done, (int)ORDER.NO_DIE);
                    placeToPlace = true;
                    if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON) sqc = 1;
                    else sqc = 10;
                    //if (reqPedestal)
                    //{
                    //    mc.pd.req = true;
                    //    mc.pd.reqMode = REQMODE.AUTO;  // 20160524. jhlim
                    //}
                    break;
                #endregion

                #endregion

                case SQC.ERROR:
                    //string dspstr = "HD ulc_place Esqc " + Esqc.ToString();
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
                    //EVENT.statusDisplay(str);
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;


            }
		}

        void DisplayGraph(int num, bool useTopLoadcell)// = false)
        {
            if (dev.NotExistHW.LOADCELL)
            {
                mc.idle(100);
                Random random = new Random();
                int rndMin, rndMax;
                rndMin = (int)loadTime.Elapsed - 200;
                rndMax = (int)loadTime.Elapsed + 200;
                feedbackForce = random.Next(rndMin, rndMax) / 1000.0;
            }
            else feedbackForce = mc.loadCell.getData(num);

            if(feedbackForce != -9999)
            {
                if (!useTopLoadcell)
                {
                    feedbackForce = mc.hd.tool.F.TopLDToBottomLD(num, feedbackForce, out ret.message);
                }

                if (num == (int)UnitCodeHead.HD1)
                {
                    EVENT.addLoadcellData2(0, loadTime.Elapsed, feedbackForce);
                }
                else if (num == (int)UnitCodeHead.HD2)
                {
                    EVENT.addLoadcellData2(2, loadTime.Elapsed, feedbackForce);
                }
            }
        }

        void DisplayGraphPoint(int num, bool useTopLoadcell)// = false)
        {
            //if (dev.NotExistHW.LOADCELL)
            //{
            //    mc.idle(100);
            //    Random random = new Random();
            //    int rndMin, rndMax;
            //    rndMin = (int)loadTime.Elapsed - 200;
            //    rndMax = (int)loadTime.Elapsed + 200;
            //    feedbackForce = random.Next(rndMin, rndMax) / 1000.0;
            //}
            //else feedbackForce = mc.loadCell.getData(num);

            //if (!useTopLoadcell)
            //{
            //    feedbackForce = mc.hd.tool.F.TopLDToBottomLD(num, feedbackForce, out ret.message);
            //}

            //if (num == (int)UnitCodeHead.HD1)
            //{
            //    EVENT.addLoadcellData2(1, loadTime.Elapsed, feedbackForce);
            //}
            //else if (num == (int)UnitCodeHead.HD2)
            //{
            //    EVENT.addLoadcellData2(3, loadTime.Elapsed, feedbackForce);
            //}
        }

        void DisplayPoint(int num, bool useTopLoadcell)// = false)
        {
            if (dev.NotExistHW.LOADCELL)
            {
                mc.idle(100);
                Random random = new Random();
                int rndMin, rndMax;
                rndMin = (int)loadTime.Elapsed - 200;
                rndMax = (int)loadTime.Elapsed + 200;
                feedbackForce = random.Next(rndMin, rndMax) / 1000.0;
            }
            else feedbackForce = mc.loadCell.getData(num);

            if (!useTopLoadcell)
            {
                feedbackForce = mc.hd.tool.F.TopLDToBottomLD(num, feedbackForce, out ret.message);
            }

            if (num == (int)UnitCodeHead.HD1)
            {
                EVENT.addLoadcellData2(1, loadTime.Elapsed, feedbackForce);
            }
            else if (num == (int)UnitCodeHead.HD2)
            {
                EVENT.addLoadcellData2(3, loadTime.Elapsed, feedbackForce);
            }
        }

        bool DisplayGraph(int curPoint, bool useForceTracking = false, bool useForceCheck = false, bool useNoiseFilter = false, bool graphDisplay = true)
		{
            //if ((graphDisplayIndex % graphDisplayCount) == 0 && graphDisplayPoint <= curPoint)
            //{
            //    loadVolt = mc.AIN.VPPM(); if (ioCheck(sqc, loadVolt)) return false;
            //    mc.hd.tool.F.voltage2kilogram(loadVolt, out loadForce, out ret.message); if (ioCheck(sqc, ret.message)) return false;
            //    sgaugeVolt = mc.AIN.HeadLoadcell(); if (ioCheck(sqc, sgaugeVolt)) return false;
            //    if (UtilityControl.forceTopLoadcellBaseForce == 1)
            //    {
            //        sgaugeForce = sgaugeVolt;	// Top Loadcell값을 그대로 display한다.
            //    }
            //    else
            //    {
            //        mc.hd.tool.F.sgVoltage2kilogram(sgaugeVolt, out sgaugeForce, out ret.message); if (ioCheck(sqc, ret.message)) return false;
            //    }
            //    if (useNoiseFilter)
            //    {
            //        if (Math.Abs(loadForce - loadForcePrev) > graphVPPMFilter || Math.Abs(sgaugeForce - sgaugeForcePrev) > graphLoadcellFilter)
            //        {
            //            loadForce = loadForcePrev;
            //            sgaugeForce = sgaugeForcePrev;
            //            //return true;
            //        }
            //    }
            //    if ((meanFilterIndex + 1) % UtilityControl.graphMeanFilter == 0 || UtilityControl.graphMeanFilter < 3)
            //    {
            //        if (UtilityControl.graphMeanFilter < 3)
            //        {
            //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplay)
            //            {
            //                EVENT.addLoadcellData(0, loadTime.Elapsed, loadForce, sgaugeForce);
            //            }
            //            meanFilterIndex = 0;

            //            mc.OUT.HD.BLW(mc.hd.order.bond, out ret.b, out ret.message);
            //            if (useForceCheck && !ret.b)
            //            {
            //                if (mc.para.ETC.usePlaceForceTracking.value == 1) calcPlaceForce(sgaugeForce);
            //                checkOverUnderForce(sgaugeForce);
            //            }
            //        }
            //        else
            //        {
            //            // Mean값 만들고
            //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplay)
            //            {
            //                calcMean(loadForceFilter, sgaugeForceFilter, UtilityControl.graphMeanFilter, ref ret.d1, ref ret.d2);
            //                EVENT.addLoadcellData(0, loadTime.Elapsed, ret.d1, ret.d2);
            //            }
            //            meanFilterIndex = 0;

            //            mc.OUT.HD.BLW(mc.hd.order.bond, out ret.b, out ret.message);
            //            if (!ret.b)
            //            {
            //                if (useForceCheck) checkOverUnderForce(sgaugeForce);
            //                if (useForceTracking) if (mc.para.ETC.usePlaceForceTracking.value == 1) calcPlaceForce(sgaugeForce);
            //            }

            //        }
            //    }
            //    else
            //    {
            //        loadForceFilter[meanFilterIndex] = loadForce;
            //        sgaugeForceFilter[meanFilterIndex] = sgaugeForce;
            //        meanFilterIndex++;
            //    }
            //}
            //// 순간적으로 튀는 Noise를 제거하기 위해 이전값을 현재값과 비교하는데, 이전 상태값을 조절하기 위한 Filter를 생성하기 위해 현재값을 BackUp
            //loadForcePrev = loadForce;
            //sgaugeForcePrev = sgaugeForce;

            //graphDisplayIndex++;
			return true;
		}

		void checkOverUnderForce(double checkForce)
		{
			//QueryTimer placeForceErrorTime = new QueryTimer();		// Limit Force를 얼마의 시간동안 Over했는지 검사하는 시간으로 사용된다.
			//int placeForceCheckCount;
			//bool placeForceOver, placeForceUnder;
			if (placeForceCheckCount == 0) placeForceErrorTime.Reset();
			else
			{
				//if (checkForce > (mc.para.HD.place.force.value + mc.para.ETC.placeTimeForceCheckLimit.value))
				if (checkForce > mc.para.ETC.placeForceHighLimit.value)
				{ 
					if(placeForceErrorTime.Elapsed > mc.para.ETC.placeTimeForceErrorDuration.value)
					{
						placeForceOver = true;
					}
				}
				//else if(checkForce < (mc.para.HD.place.force.value - mc.para.ETC.placeTimeForceCheckLimit.value))
				else if (checkForce < mc.para.ETC.placeForceLowLimit.value)
				{
					if (placeForceErrorTime.Elapsed > mc.para.ETC.placeTimeForceErrorDuration.value)
					{
						placeForceUnder = true;
					}
				}
				else
				{
					placeForceErrorTime.Reset();
				}
			}
			placeForceCheckCount++;
		}

		void calcPlaceForce(double checkForce)
		{
			if (placeForceMin > checkForce) placeForceMin = checkForce;
			if (placeForceMax < checkForce) placeForceMax = checkForce;
			placeForceSum += checkForce;
			placeForceSumCount++;
		}

        public void calcMean(double[] val1, double[] val2, int filter, ref double out1, ref double out2)
		{
			double maxVal, minVal;
			int maxIndex, minIndex;
			double sumVal, meanVal;

			double maxValV, minValV;
			int maxIndexV, minIndexV;
			double sumValV, meanValV;

			maxVal = -100;
			minVal = 100;
			maxIndex = 0;
			minIndex = 0;

			for (int i = 0; i < filter; i++)
			{
				if (val1[i] > maxVal) { maxVal = val1[i]; maxIndex = i; }
				if (val1[i] < minVal) { minVal = val1[i]; minIndex = i; }
			}
			if (maxIndex == minIndex)
			{
				maxIndex = minIndex + 1;
			}
			sumVal = 0;
			for (int i = 0; i < filter; i++)
			{
				if (i == maxIndex || i == minIndex) continue;
				else
				{
					sumVal += val1[i];
				}
			}
			meanVal = sumVal / (filter - 2);

			maxValV = -100;
			minValV = 100;
			maxIndexV = 0;
			minIndexV = 0;

			for (int i = 0; i < filter; i++)
			{
				if (val2[i] > maxValV) { maxValV = val2[i]; maxIndexV = i; }
				if (val2[i] < minValV) { minValV = val2[i]; minIndexV = i; }
			}
			if (maxIndexV == minIndexV)
			{
				maxIndexV = minIndexV + 1;
			}
			sumValV = 0;
			for (int i = 0; i < filter; i++)
			{
				if (i == maxIndexV || i == minIndexV) continue;
				else
				{
					sumValV += val2[i];
				}
			}
			meanValV = sumValV / (filter - 2);

			out1 = meanVal;
			out2 = meanValV;
		}

        //public void home_press()
        //{
        //    #region PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF
        //    if (sqc > 60 && sqc < 70 && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
        //    {
        //        mc.OUT.HD.SUC(mc.hd.order.bond, out ret.b, out ret.message); ioCheck(sqc, ret.message);
        //        if (ret.b)
        //        {
        //            Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
        //            if (ret.d < posZ + mc.para.HD.place.suction.level.value)
        //            {
        //                mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); ioCheck(sqc, ret.message);
        //            }
        //        }
        //    }
        //    #endregion
        //    switch (sqc)
        //    {
        //        case 0:
        //            hdcfailcount = 0;
        //            fiducialfailcount = 0;
        //            fiducialfailchecked = false;
        //            Esqc = 0;
        //            graphDisplayCount = UtilityControl.graphDisplayFilter;
        //            graphDisplayPoint = UtilityControl.graphStartPoint;
        //            graphVPPMFilter = UtilityControl.graphControlDataFilter;
        //            graphLoadcellFilter = UtilityControl.graphLoadcellDataFilter;
        //            sqc++; break;
        //        case 1:
        //            // 이동할 Position을 먼저 입력한 뒤에 pedestal request를 call한다. 20131022
        //            mc.OUT.HD.LS.ON(true, out ret.message);
        //            mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            padX = mc.para.mmiOption.manualPadX;
        //            padY = mc.para.mmiOption.manualPadY;

        //            mc.pd.req = true;
        //            mc.pd.reqMode = REQMODE.AUTO;
        //            if (mc.para.HDC.fiducialUse.value == (int)ON_OFF.OFF) sqc = 10;
        //            else sqc++;
        //            break;

        //        #region Check Ficucial Mark
        //        case 2:
        //            for_break = false;
        //            for (int i = 0; i < mc.activate.headCnt; i++)
        //            {
        //                Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
        //            }
        //            if (for_break) break;
					
        //            if (mc.para.HDC.fiducialPos.value == 0)
        //            {
        //                Y.moveCompare(cPos.y.PADC1(padY, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.moveCompare(cPos.x.PADC1(padX, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            else if (mc.para.HDC.fiducialPos.value == 1)
        //            {
        //                Y.moveCompare(cPos.y.PADC2(padY), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.moveCompare(cPos.x.PADC2(padX), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            else if (mc.para.HDC.fiducialPos.value == 2)
        //            {
        //                Y.moveCompare(cPos.y.PADC3(padY, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.moveCompare(cPos.x.PADC3(padX, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            else
        //            {
        //                Y.moveCompare(cPos.y.PADC4(padY), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.moveCompare(cPos.x.PADC4(padX), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 3:
        //            for (int i = 0; i < mc.activate.headCnt; i++)
        //            {
        //                Z[i].AT_ERROR(out ret.b, out ret.message); if (ret.b) break;
        //            }
        //            if (ret.b)
        //            {
        //                X.eStop(out ret.message); Y.eStop(out ret.message);
        //            }
        //            if (!Z_AT_TARGET_ALL()) break;
        //            #region HDC.PADC1.req
        //            fidPX = 0;
        //            fidPY = 0;
        //            fidPD = 0;
        //            if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
        //            {
        //                if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                    mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //            }
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //            {
        //                if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                    mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //            }
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
        //            {
        //                if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
        //                else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
        //                else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
        //                else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
        //            }
        //            else mc.hdc.reqMode = REQMODE.GRAB;
        //            mc.hdc.lighting_exposure(mc.para.HDC.modelFiducial.light, mc.para.HDC.modelFiducial.exposureTime);
        //            if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //            #endregion
        //            dwell.Reset();
        //            sqc++; break;
        //        case 4:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 5:
        //            if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;
        //            sqc++; break;
        //        case 6:
        //            if (mc.pd.RUNING) break;
        //            if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 7:
        //            if (dwell.Elapsed < 15) break; // head camera delay
        //            if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
        //            triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 8:
        //            if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
        //            triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            //if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 9:
        //            if (mc.hdc.RUNING) break;
        //            if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
        //            if (mc.hdc.cam.refresh_req) break;
        //            #region fiducial result
        //            if (mc.hd.reqMode == REQMODE.DUMY) { }
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
        //            {
        //                if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
        //                {
        //                    fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
        //                    fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
        //                    fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
        //                }
        //            }
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //            {
        //                if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
        //                {
        //                    fidPX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
        //                    fidPY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
        //                    fidPD = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
        //                }
        //            }
        //            else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
        //            {
        //                fidPX = mc.hdc.cam.circleCenter.resultX;
        //                fidPY = mc.hdc.cam.circleCenter.resultY;
        //                fidPD = mc.hdc.cam.circleCenter.resultRadius;
        //            }
        //            #endregion
        //            if (fidPX == -1 && fidPY == -1 && fidPD == -1) // HDC Fiducial이 보이면 오히려 Error
        //            {
        //                sqc = 10;
        //            }
        //            else
        //            {
        //                tempSb.Clear(); tempSb.Length = 0;
        //                tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                fiducialfailcount++;
        //                if (fiducialfailcount < mc.para.HDC.failretry.value)
        //                {
        //                    tempSb.AppendFormat("fiducial checked ({0})", fiducialfailcount + 1);
        //                    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                    sqc = 3;
        //                }
        //                else
        //                {
        //                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_FIDUCIAL_CHECKED_FAIL); break;
        //                }
        //            }
        //            break;
        //        #endregion

        //        #region case 10 xy pad c1 move
        //        case 10:
        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 0);
        //            for_break = false;
        //            for (int i = 0; i < mc.activate.headCnt; i++)
        //            {
        //                Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
        //            }
        //            if (for_break) break;
        //            #region Gantry Move
        //            if (hdcfailchecked || (mc.para.HDC.fiducialUse.value == (int)ON_OFF.ON))
        //            {
        //                rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
        //                rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //                else
        //                {
        //                    Y.move(cPos.y.PADC1(padY, true), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC1(padX, true), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //            }
        //            else
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    Y.moveCompare(cPos.y.PADC2(padY), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.moveCompare(cPos.x.PADC2(padX), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //                else
        //                {
        //                    Y.moveCompare(cPos.y.PADC1(padY, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.moveCompare(cPos.x.PADC1(padX, true), Z, tPos.z[mc.hd.order.bond].XY_MOVING - comparePos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //            }
        //            #endregion
        //            dwell.Reset();
        //            sqc++; break;
        //        case 11:
        //            for (int i = 0; i < mc.activate.headCnt; i++)
        //            {
        //                Z[i].AT_ERROR(out ret.b, out ret.message); if (ret.b) break;
        //            }
        //            if (ret.b)
        //            {
        //                X.eStop(out ret.message); Y.eStop(out ret.message);
        //            }
        //            if (!Z_AT_TARGET_ALL()) break;
					
        //            #region 1st Corner Camera Request
        //            if (hdcfailchecked)
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    #region HDC.PADC1.req
        //                    hdcP1X = 0;
        //                    hdcP1Y = 0;
        //                    hdcP1T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion

        //                }
        //                else
        //                {
        //                    #region HDC.PADC2.req
        //                    hdcP1X = 0;
        //                    hdcP1Y = 0;
        //                    hdcP1T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion

        //                }
        //            }
        //            else
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    #region HDC.PADC2.req
        //                    hdcP1X = 0;
        //                    hdcP1Y = 0;
        //                    hdcP1T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region HDC.PADC1.req
        //                    hdcP1X = 0;
        //                    hdcP1Y = 0;
        //                    hdcP1T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion
        //                }

        //            }
        //            #endregion

        //            dwell.Reset();
        //            sqc++; break;
        //        case 12:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 13:
        //            if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;
        //            sqc++; break;
        //        case 14:
        //            if (mc.pd.RUNING) break;
        //            if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_1ST_FIDUCIAL_POS, 1);
        //            sqc = 20; break;
        //        #endregion

        //        #region case 20 triggerHDC
        //        case 20:
        //            if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 30; break; }
        //            mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 0);
        //            dwell.Reset();
        //            sqc++; break;
        //        case 21:
        //            if (dwell.Elapsed < 15) break; // head camera delay
        //            if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
        //            triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 22:
        //            if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
        //            triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 23:
        //            if (dwell.Elapsed < 300) break;
        //            mc.log.mcclog.write(mc.log.MCCCODE.SCAN_1ST_FIDUCIAL, 1);
        //            sqc = 30; break;
        //        #endregion

        //        #region case 30 xy pad c3 move
        //        case 30:
        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 0);
        //            #region gantry move
        //            rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
        //            rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);

        //            if (hdcfailchecked)
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //                else
        //                {
        //                    Y.move(cPos.y.PADC3(padY, true), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC3(padX, true), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //            }
        //            else
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //                else
        //                {
        //                    Y.move(cPos.y.PADC3(padY, true), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                    X.move(cPos.x.PADC3(padX, true), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //                }
        //            }
        //            #endregion
        //            sqc++; break;
        //        case 31:
        //            if (mc.hdc.RUNING) break;
        //            if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

        //            if (hdcfailchecked)
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    #region HDC.PADC1.result
        //                    if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                        hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                        hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                    }
        //                    if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                    {
        //                        mc.hdc.displayUserMessage("DETECTION FAIL");
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                            //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                        }
        //                    }
						
        //                    if (Math.Abs(hdcP1X) > 5000)
        //                    {
        //                        mc.hdc.displayUserMessage("X RESULT OVER FAIL");
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1Y) > 5000)
        //                    {
        //                        mc.hdc.displayUserMessage("Y RESULT OVER FAIL");
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1T_1) > 5 || Math.Abs(hdcP1T_2) > 5)
        //                    {
        //                        mc.hdc.displayUserMessage("R RESULT OVER FAIL");
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
        //                        }
        //                    }
        //                    #endregion

        //                    #region HDC.PADC3.req
        //                    hdcP2X = 0;
        //                    hdcP2Y = 0;
        //                    hdcP2T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region HDC.PADC2.result
        //                    if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                        hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                        hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                    }
        //                    if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                    {
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                            //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                        }
        //                    }
        //                    if (dev.debug)
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 10)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C21_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 5)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T_1).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    #endregion

        //                    #region HDC.PADC4.req
        //                    hdcP2X = -1;
        //                    hdcP2Y = -1;
        //                    hdcP2T_1 = -1;
        //                    hdcP2T_2 = -1;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion
        //                }

        //            }
        //            else
        //            {
        //                if (mc.para.HDC.detectDirection.value == 0)
        //                {
        //                    #region HDC.PADC2.result
        //                    if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                        hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                        hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                    }
        //                    if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                    {
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                            //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                        }
        //                    }
        //                    if (dev.debug)
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 10)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C21_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 5)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    #endregion

        //                    #region HDC.PADC4.req
        //                    hdcP2X = 0;
        //                    hdcP2Y = 0;
        //                    hdcP2T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion

        //                }
        //                else
        //                {
        //                    #region HDC.PADC1.result
        //                    if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
        //                            hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
        //                            hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
        //                        }
        //                    }
        //                    else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                        hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                        hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                    }
        //                    if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                    {
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                            //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                            mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                        }
        //                    }
        //                    if (dev.debug)
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 10)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T_1).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Math.Abs(hdcP1X) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1Y) > 5000)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
        //                            }
        //                        }
        //                        if (Math.Abs(hdcP1T_1) > 5)
        //                        {
        //                            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                            if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                                sqc = 120; break;
        //                            }
        //                            else
        //                            {
        //                                if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                                tempSb.Clear(); tempSb.Length = 0;
        //                                tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                                //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
        //                            }
        //                        }
        //                    }
        //                    #endregion

        //                    #region HDC.PADC3.req
        //                    hdcP2X = 0;
        //                    hdcP2Y = 0;
        //                    hdcP2T_1 = 0;
        //                    if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                    {
        //                        if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                    {
        //                        if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                        {
        //                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
        //                        }
        //                        else mc.hdc.reqMode = REQMODE.GRAB;
        //                    }
        //                    else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                    mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
        //                    if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                    #endregion
        //                }
        //            }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 32:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 33:
        //            if (!X_AT_DONE || !Y_AT_DONE) break;
        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_2ND_FIDUCIAL_POS, 1);
        //            sqc = 40; break;
        //        #endregion

        //        #region case 40 triggerHDC
        //        case 40:
        //            if (mc.swcontrol.useHwTriger == 1) if (mc.hdc.req == false) { sqc = 50; break; }
        //            mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 0);
        //            dwell.Reset();
        //            sqc++; break;
        //        case 41:
        //            if (dwell.Elapsed < 15) break; // head camera delay
        //            if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
        //            triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 42:
        //            if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
        //            triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 43:
        //            if (dwell.Elapsed < 300) break;
        //            mc.log.mcclog.write(mc.log.MCCCODE.SCAN_2ND_FIDUCIAL, 1);
        //            sqc = 50; break;
        //        #endregion

        //        #region case 50 xy pad move
        //        case 50:
        //            placeX = tPos.x[mc.hd.order.bond].PAD(padX);
        //            placeY = tPos.y[mc.hd.order.bond].PAD(padY);
        //            dwell.Reset();
        //            sqc++; break;
        //        case 51:
        //            if (mc.hdc.RUNING) break;
        //            if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
        //            //clacULCX = clacULCY = clacULCT = 0;
					
        //            if (((mc.hd.tool.hdcfailcount % 2) == 0 && mc.para.HDC.detectDirection.value == 0) || ((mc.hd.tool.hdcfailcount % 2) == 1 && mc.para.HDC.detectDirection.value == 1))
        //            {
        //                #region HDC.PADC4.result
        //                if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
        //                        hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
        //                        hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
        //                        hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
        //                        hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
        //                    hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
        //                    hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                }

        //                //cosTheta = Math.Cos(hdcT * Math.PI / 180);
        //                //sinTheta = Math.Sin(hdcT * Math.PI / 180);
        //                //hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
        //                //hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
        //                //EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
        //                #endregion
        //            }
        //            else
        //            {
        //                #region HDC.PADC3.result
        //                if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
        //                        hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
        //                        hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP2X = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
        //                        hdcP2Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
        //                        hdcP2T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
        //                    hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
        //                    hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                }
        //                //cosTheta = Math.Cos(hdcT * Math.PI / 180);
        //                //sinTheta = Math.Sin(hdcT * Math.PI / 180);
        //                //hdcX = (cosTheta * hdcX) - (sinTheta * hdcY);
        //                //hdcY = (sinTheta * hdcX) + (cosTheta * hdcY);
        //                //EVENT.statusDisplay("HDC : " + Math.Round(hdcX, 2).ToString() + "  " + Math.Round(hdcY, 2).ToString() + "  " + Math.Round(hdcT, 2).ToString());
        //                #endregion
        //            }

        //            if (hdcP2X == -1 && hdcP2Y == -1 && hdcP2T_1 == -1) // HDC Vision Result Error
        //            {
        //                if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                    //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                    mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                    sqc = 120; break;
        //                }
        //                else
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                    //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_VISION_PROCESS_FAIL); break;
        //                }
        //            }
        //            if (dev.debug)
        //            {
        //                if (Math.Abs(hdcP2X) > 5000)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP2X));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_X_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_X_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP2Y) > 5000)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP2Y));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_Y_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_Y_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP2T_1) > 10)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP2T_1));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T_1);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
        //                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
        //                    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
        //                    //EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_(C1-C3)_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_(C1-C3)_Limit");
        //                        //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (Math.Abs(hdcP2X) > 5000)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP2X));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_X_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_X_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2X);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2X).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP2Y) > 5000)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP2Y));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_Y_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_Y_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2Y);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2Y).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP2T_1) > 5)
        //                {
        //                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP2T_1));
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C3_T_Limit");
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP2T_1);
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP2T).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                    }
        //                }
        //                if (Math.Abs(hdcP1X - hdcP2X) > mc.para.MT.padCheckLimit.value || Math.Abs(hdcP1Y - hdcP2Y) > mc.para.MT.padCheckLimit.value)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - P1-P2 : {2:F2}, {3:F2}", (padX + 1), (padY + 1), hdcP1X - hdcP2X, hdcP1Y - hdcP2Y);
        //                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString();
        //                    mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
        //                    //EVENT.statusDisplay("HDC[" + padX.ToString() + "," + padY.ToString() + "] P1-P2 : " + Math.Round(hdcP1X - hdcP2X, 2).ToString() + "  " + Math.Round(hdcP1Y - hdcP2Y, 2).ToString());
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_(C1-C3)_Limit");
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_(C1-C3)_Limit");
        //                        //str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y - hdcP2T).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PAD_SIZE_OVER); break;
        //                    }
        //                }
        //            }
        //            hdcX = (hdcP1X + hdcP2X) / 2;
        //            hdcY = (hdcP1Y + hdcP2Y) / 2;
        //            hdcT = (hdcP1T_1 + hdcP2T_1) / 2;
        //            mc.log.debug.write(mc.log.CODE.INFO, String.Format("HDC[{0},{1}] Package X,Y,T : {2:F1}, {3:F1}, {4:F1}", padX + 1, padY + 1, hdcX, hdcY, hdcT));
        //            if (Math.Abs(hdcX) > mc.para.MT.padCheckCenterLimit.value * 2)
        //            {
        //                mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package X Position Limit Error : {0:F1}um", hdcX));
        //                if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                {
        //                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_Packege_XPos_Over");
        //                    sqc = 120; break;
        //                }
        //                else
        //                {
        //                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_Packege_XPos_Over");
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center X: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcX, mc.para.MT.padCheckCenterLimit.value);
        //                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center X: " + Math.Round(hdcX, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
        //                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_XRESULT_OVER); break;
        //                }
        //            }
        //            if (Math.Abs(hdcY) > mc.para.MT.padCheckCenterLimit.value * 2)
        //            {
        //                mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC Package Y Position Limit Error : {0:F1}um", hdcY));
        //                if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                {
        //                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_Packege_YPos_Over");
        //                    sqc = 120; break;
        //                }
        //                else
        //                {
        //                    if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_Packege_YPos_Over");
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("PadX[{0}],PadY[{1}] - Package Center Y: {2:F2}, Limit: {3:F2}", (padX + 1), (padY + 1), hdcY, mc.para.MT.padCheckCenterLimit.value);
        //                    //string str = "HDC[" + padX.ToString() + "," + padY.ToString() + "] Package Center Y: " + Math.Round(hdcY, 2).ToString() + ", Limit: " + Math.Round(mc.para.MT.padCheckCenterLimit.value, 2).ToString();
        //                    errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_PACKAGE_CENTER_YRESULT_OVER); break;
        //                }
        //            }

        //            //double cosTheta, sinTheta;
        //            //cosTheta = Math.Cos((-clacULCT) * Math.PI / 180);
        //            //sinTheta = Math.Sin((-clacULCT) * Math.PI / 180);
        //            //clacULCX = (cosTheta * clacULCX) - (sinTheta * clacULCY);
        //            //clacULCY = (sinTheta * clacULCX) + (cosTheta * clacULCY);
        //            //placeX -= clacULCX;
        //            //placeY -= clacULCY;
        //            //placeT = tPos.t[mc.hd.order.bond].ZERO + clacULCT - hdcT + mc.para.HD.place.offset.t.value;

        //            //placeX += hdcX;
        //            //placeY += hdcY;

        //            if (padX < 0 || padY < 0)
        //            {
        //                errorCheck(ERRORCODE.HD, sqc, String.Format("Array Index Error : X-{0} Y-{1}", padX, padY)); break;
        //            }
        //            placeX += mc.para.CAL.place[padX, padY].x.value;
        //            placeY += mc.para.CAL.place[padX, padY].y.value;

        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 0);

        //            rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
        //            Y.move(placeY, out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //            rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
        //            X.move(placeX, out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            T[mc.hd.order.bond].move(placeT, out ret.message); if (mpiCheck(T[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 52:
        //            if (timeCheck(UnitCodeAxis.X, sqc, 3)) break;
        //            X.actualPosition(out ret.d, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            if (Math.Abs(placeX - ret.d) > 3000) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 53:
        //            if (timeCheck(UnitCodeAxis.Y, sqc, 3)) break;
        //            Y.actualPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //            if (Math.Abs(placeY - ret.d) > 3000) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 54:
        //            if (timeCheck(UnitCodeAxis.T, sqc, 3)) break;
        //            T[mc.hd.order.bond].actualPosition(out ret.d, out ret.message); if (mpiCheck(T[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //            if (Math.Abs(placeT - ret.d) > 3) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 55:
        //            if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.bond)) break;
        //            mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_BOND_POS, 1);
        //            sqc = 60; break;
        //        #endregion

        //        #region case 60 z down
        //        case 60:
        //            mc.commMPC.EventReport((int)eEVENT_LIST.eEV_ATTACH_START);

        //            // 최종 target에 대한 point만 검사한다. Force task에서 이 값을 사용하기 위함.
        //            if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z[mc.hd.order.bond].DRYRUNPLACE;
        //            else posZ = tPos.z[mc.hd.order.bond].PLACE;
					
        //            posZ -= mc.para.CAL.place[padX, padY].z.value;

        //            forceTargetZPos = posZ;

        //            contactPointSearchDone = false;
        //            forceStartPointSearchDone = false;
        //            forceStartPointCheckCount = 0;
        //            linearAutoTrackStart = false;

        //            //mc.hd.tool.F.req = true;
        //            //if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
        //            //    mc.hd.tool.F.reqMode = REQMODE.F_M2PLACE;
        //            //else if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
        //            //    mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEREV;

        //            // Slope를 만들어 내기 위해 force에 대한 차이값을 만든다. air(low->high) mode에서 사용
        //            if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
        //            {
        //                diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search2.force.value;
        //                //if (graphDisplayPoint == 2)
        //                //	diffForce = mc.para.HD.place.force.value;
        //            }
        //            else
        //            {
        //                diffForce = mc.para.HD.press.force.value - mc.para.HD.place.search.force.value;
        //                //if (graphDisplayPoint == 2)
        //                //	diffForce = mc.para.HD.place.force.value;
        //            }

        //            if (diffForce == 0)		// 0으로 나뉘어지는 경우를 방지하기 위한 최소값을 입력
        //            {
        //                diffForce = 0.001;
        //            }
        //            #region pos set
        //            if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) posZ = tPos.z[mc.hd.order.bond].DRYRUNPLACE;
        //            else posZ = tPos.z[mc.hd.order.bond].PLACE;
					
        //            // 최종 target force
        //            posZ -= mc.para.CAL.place[padX, padY].z.value;

        //            if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
        //            {
        //                levelS1 = mc.para.HD.place.search.level.value;
        //                delayS1 = mc.para.HD.place.search.delay.value;
        //                velS1 = (mc.para.HD.place.search.vel.value) / 1000;
        //                accS1 = mc.para.HD.place.search.acc.value;
        //            }
        //            else
        //            {
        //                levelS1 = 0;
        //                delayS1 = 0;
        //            }
        //            if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
        //            {
        //                levelS2 = (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value);
        //                delayS2 = mc.para.HD.place.search2.delay.value;
        //                velS2 = (mc.para.HD.place.search2.vel.value) / 1000;
        //                accS2 = mc.para.HD.place.search2.acc.value;
        //            }
        //            else
        //            {
        //                levelS2 = 0;
        //                delayS2 = 0;
        //            }
        //            if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
        //            {
        //                delay = mc.para.HD.press.pressTime.value + mc.para.HD.place.suction.purse.value;
        //            }
        //            else
        //            {
        //                delay = mc.para.HD.press.pressTime.value;
        //            }
        //            #endregion
        //            mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 0);

        //            // clear loadcell graph data & time
        //            if (UtilityControl.graphDisplayEnabled == 1)
        //            {
        //                graphDispStart = true;
        //                EVENT.clearLoadcellData();
        //            }
        //            else graphDispStart = false;

        //            loadTime.Reset();
        //            graphDisplayIndex = 0;
        //            meanFilterIndex = 0;

        //            // initialize place-time force check variables...
        //            placeForceCheckCount = 0;
        //            placeForceOver = false;
        //            placeForceUnder = false;
        //            placeSensorForceCheckCount = 0;
        //            placeSensorForceOver = false;
        //            placeSensorForceUnder = false;

        //            if (levelS1 != 0)
        //            {
        //                //?? 왜2번?
        //                Z[mc.hd.order.bond].move(posZ + levelS1 + levelS2, -velS1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //                //Z.move(posZ + levelS1 + levelS2, -velS1, (int)mc.para.HD.place.forceMode.speed.value, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
        //                Z[mc.hd.order.bond].move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //                if (delayS1 == 0) { sqc += 3; break; }
        //            }
        //            else
        //            {
        //                Z[mc.hd.order.bond].move(posZ + levelS1 + levelS2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //                sqc += 3; break;
        //            }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 61:
        //            DisplayGraph(0);
        //            if (!Z_AT_TARGET(mc.hd.order.bond)) break;
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint == 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Moving Done
        //            //if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
        //            //{
        //            //    F.kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            //}
        //            dwell.Reset();
        //            sqc++; break;
        //        case 62:
        //            DisplayGraph(0);
        //            if (dwell.Elapsed < delayS1 - 3) break;		// Search1 Delay
        //            if (graphDisplayPoint == 0)
        //            {
        //                if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 1 Delay Done
        //            }
        //            sqc++; break;
        //        case 63:
        //            // clear loadcell graph data & time
        //            if (graphDisplayPoint == 1)		// Search2 구간부터 Display한다.
        //            {
        //                loadTime.Reset();
        //                graphDisplayIndex = 0;
        //            }

        //            if (levelS2 == 0) { sqc += 3; break; }
        //            Z[mc.hd.order.bond].move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;	// search2 move start
        //            if (levelD2 == 0) { sqc += 3; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 64:
        //            // Search2 구간에서 Contact이 발생한다.
        //            Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
        //            if (mc.hd.reqMode == REQMODE.DUMY && (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON || mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)) contactPos = tPos.z[mc.hd.order.bond].DRYCONTACTPOS;
        //            else contactPos = tPos.z[mc.hd.order.bond].CONTACTPOS;
        //            if (ret.d < (contactPos - mc.para.CAL.place[padX, padY].z.value + 20) && contactPointSearchDone == false)	// 10um Offset은 조금 더 주자. 실질적인 Force 파형은 늦게 나타나므로 사실 필요가 없을 수도 있다.
        //            {
        //                if (graphDisplayPoint == 2) loadTime.Reset();
        //                graphDisplayIndex = 0;
        //                contactPointSearchDone = true;
        //            }
        //            if (contactPointSearchDone) DisplayGraph(2);
        //            else DisplayGraph(1);

        //            if (!Z_AT_TARGET(mc.hd.order.bond)) break;		// Search2 구간까지 완료된 경우.
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && graphDisplayPoint <= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);

        //            if (graphDisplayPoint == 3)		// Search2 Delay 구간부터 Display한다.
        //            {
        //                loadTime.Reset();
        //                graphDisplayIndex = 0;
        //            }

        //            loadForcePrev = loadForce;
        //            sgaugeForcePrev = sgaugeForce;

        //            //if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE)
        //            //{
        //            //    F.kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            //}

        //            dwell.Reset();
        //            forceTime.Reset();
        //            //mc.log.debug.write(mc.log.CODE.ETC, "start");
        //            sqc++; break;
        //        case 65:
        //            DisplayGraph(3, false, true);

        //            if (dwell.Elapsed < delayS2 - 3) break;			// Search2 Delay 구간
        //            //mc.log.debug.write(mc.log.CODE.ETC, "end");
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Search 2 Delay Done

        //            dwell.Reset();
        //            sqc++; break;
        //        case 66:		// Search2를 사용하지 않거나, Search2 Delay가 0일때 Z축 Target Done Check
        //            DisplayGraph(3);

        //            if (!Z_AT_TARGET(mc.hd.order.bond)) break;

        //            dwell.Reset();
        //            sqc++; break;
        //        case 67:		// Z축 Motion Done이 발생했는지 확인하는 구간..여기서 모든 Z축의 동작이 완료된다.
        //            DisplayGraph(3);
	
        //            if (!Z_AT_DONE(mc.hd.order.bond)) break;
        //            mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_DOWN, 1);
        //            mc.OUT.HD.SUC(mc.hd.order.bond, out ret.b, out ret.message); ioCheck(sqc, ret.message);
        //            if (ret.b && mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
        //            {
        //                // Suction이 꺼져야 하는데, 안꺼졌어...뭔가 문제 있지...
        //                Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message);
        //                mc.log.debug.write(mc.log.CODE.WARN, "Check Place Suction Mode-Cmd:" + Math.Round(ret.d).ToString() + "![<]Cur:" + Math.Round(posZ + mc.para.HD.place.suction.level.value).ToString());
        //                mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
        //            }
        //            if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
        //            {
        //                mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
        //            }

        //            PreForce = loadForce;

        //            // 20140602
        //            mc.log.place.write("Pre Force : " + PreForce + "kg");

        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart)
        //            {
        //                EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Z Motion Done
        //            }
        //            dwell.Reset();
        //            if (forceStartPointSearchDone == true) autoTrackDelayTime.Reset();
        //            sqc++; break;
        //        case 68:		// X,Y,T의 Motion Done이 완료되었는지 확인하는 구간..이건 사실 필요가 없다. 왜냐하면 이 루틴이 앞으로 빠졌기 때문
        //            DisplayGraph(3);

        //            // X, Y, T 완료 루틴 제거..혹시나 timing을 깨버리는 요소로 동작할 가능성도 있어서..
        //            //if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE) break; 
        //            mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 0);
        //            if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF || mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
        //            {
        //                mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
        //                sqc++;
        //            }
        //            else if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)   // in the case of PLACE_END_OFF
        //            {
        //                sqc += 2;
        //            }
        //            // PLACE_UP_OFF는 UP timing에 동작한다.
        //            else
        //            {
        //                sqc = 72;
        //            }
        //            break;
        //        case 69:	// Blow Time 대기 시간..
        //            DisplayGraph(3);

        //            if (dwell.Elapsed < mc.para.HD.place.suction.purse.value) break;    //이거 Place Value가 아니라 Blow Time값이다.
        //            mc.OUT.HD.BLW(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow off
        //            //mc.hd.tool.F.voltage2kilogram(ret.d, out ret.d1, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            //PreForce = ret.d1;
        //            //writedone = false;
        //            sqc++; break;
        //        case 70:	// suction off delay
        //            DisplayGraph(3);

        //            if (forceStartPointSearchDone)
        //            {
        //                //if (autoTrackDelayTime.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
        //                //if (autoTrackDelayTime.Elapsed < mc.para.HD.place.suction.delay.value) break;
        //                if (autoTrackDelayTime.Elapsed < mc.para.HD.press.pressTime.value) break;
        //            }
        //            else
        //            {
        //                //if (dwell.Elapsed < (delay - (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value))) break;
        //                //if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
        //                if (dwell.Elapsed < mc.para.HD.press.pressTime.value) break;

        //            }
        //            mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// suction off
        //            sqc += 2; break;
        //        case 71:	// Blow delay
        //            DisplayGraph(3);

        //            if (forceStartPointSearchDone)
        //            {
        //                if (autoTrackDelayTime.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
        //                mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(autoTrackDelayTime.Elapsed));
        //            }
        //            else
        //            {
        //                if (dwell.Elapsed < (delay - mc.para.HD.place.suction.purse.value)) break;
        //                mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow On " + Math.Round(dwell.Elapsed));
        //            }
        //            mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);	// blow on
        //            sqc++; break;
        //        case 72:
        //            if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
        //            {
        //                if (UtilityControl.graphEndPoint >= 1)
        //                {
        //                    if (dwell.Elapsed < 200) DisplayGraph(4);
        //                    else DisplayGraph(4, true);
        //                }
        //            }
        //            else DisplayGraph(3);

        //            if (dwell.Elapsed < delay - 3) break;

        //            if (mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
        //            {
        //                mc.OUT.HD.BLW(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                mc.log.debug.write(mc.log.CODE.INFO, "COMP : Blow Off " + Math.Round(dwell.Elapsed));
        //            }

        //            if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint > 0) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done

        //            //ret.d2 = mc.AIN.VPPM(); if (ioCheck(sqc, ret.d2)) break;

        //            // Load ON 정상..
        //            if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
        //            {
        //                //mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                if (ret.b1 == false)
        //                {
        //                    placeSensorForceUnder = true;
        //                    placeSensorForceCheckCount++;
        //                    if (placeSensorForceCheckCount <= 3) break;
        //                }
        //                else
        //                {
        //                    placeSensorForceUnder = false;
        //                }
        //                if (mc.para.ETC.placeTimeSensorCheckMethod.value == 1 || mc.para.ETC.placeTimeSensorCheckMethod.value == 3)
        //                {
        //                    //mc.IN.HD.LOAD_CHK2(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
        //                    if (ret.b2 == false)
        //                    {
        //                        placeSensorForceOver = true;
        //                        placeSensorForceCheckCount++;
        //                        if (placeSensorForceCheckCount <= 3) break;
        //                    }
        //                    else
        //                    {
        //                        placeSensorForceOver = false;
        //                    }
        //                }
        //            }

        //            PostForce = ret.d3;

        //            // 20140602
        //            mc.log.place.write("Post Force : " + PostForce + "kg");


        //            //EVENT.controlLoadcellData(2, Math.Ceiling(loadTime.Elapsed / 1000) * 1000);

        //            attachError = 0;

        //            if (mc.para.ETC.placeTimeForceCheckUse.value == (int)ON_OFF.ON)
        //            {
        //                // Sensor 상태가 아니라 Force Feedback Data를 보고, Over Press/Under Press를 설정한다.
        //                if (placeForceUnder)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
        //                    mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
        //                    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
        //                    attachError = 1;
        //                }
        //                else if (placeForceOver)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
        //                    mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
        //                    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
        //                    attachError = 2;
        //                }
        //            }
        //            if (mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON && attachError == 0)
        //            {
        //                if (placeSensorForceUnder)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
        //                    mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
        //                    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_UNDERPRESS, out ret.b);
        //                    attachError = 3;
        //                }
        //                else if (placeSensorForceOver)
        //                {
        //                    tempSb.Clear(); tempSb.Length = 0;
        //                    tempSb.AppendFormat("Attach FAIL - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : OVER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
        //                    mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
        //                    mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, PAD_STATUS.ATTACH_OVERPRESS, out ret.b);
        //                    attachError = 4;
        //                }
        //            }
        //            if (attachError == 0)
        //            {
        //                tempSb.Clear(); tempSb.Length = 0;
        //                tempSb.AppendFormat("Attach Done - X[{0}], Y[{1}], Force : {2:F2}, {2:F2}[kg] + {4:F2} [V] : UNDER PRESS", (padX + 1), (padY + 1), PreForce, PostForce, ret.d2);
        //                mc.log.debug.write(mc.log.CODE.TRACE, tempSb.ToString());
        //                mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.ATTACH_DONE, out ret.b);
        //                if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus upload fail"); break; }
        //            }

        //            // SVID Send..
        //            mc.commMPC.SVIDReport();

        //            mc.board.write(BOARD_ZONE.WORKING, out ret.b);
        //            if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "board.padStatus update fail"); break; }

        //            sqc++; break;
        //        case 73:
        //            //if (mc.hd.tool.F.RUNING) break;
        //            //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
        //            mc.log.mcclog.write(mc.log.MCCCODE.START_BONDING, 1);
        //            if ((attachError > 2 && (int)mc.para.ETC.placeTimeSensorCheckMethod.value > 1) || ((attachError == 1 || attachError == 2) && (int)mc.para.ETC.placeTimeForceCheckMethod.value > 0))
        //            {
        //                sqc++;
        //            }
        //            else
        //                sqc = SQC.STOP;
        //            break;
        //        case 74:	// Move Z Up to Safety Position
        //            mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
        //            Z[mc.hd.order.bond].move(tPos.z[mc.hd.order.bond].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 75:
        //            if (!Z_AT_TARGET(mc.hd.order.bond)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 76:
        //            if (!Z_AT_DONE(mc.hd.order.bond)) break;
        //            //mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
        //            //string errmessage;
        //            tempSb.Clear(); tempSb.Length = 0;
        //            tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //            //errmessage = "X[" + (padX + 1).ToString() + "], Y[" + (padY + 1).ToString() + "]";
        //            if (attachError == 1)
        //            {
        //                placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
        //                mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
        //                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_UNDER_PRESS);
        //            }
        //            else if (attachError == 2)
        //            {
        //                placeResult = PAD_STATUS.ATTACH_OVERPRESS;
        //                mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
        //                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_HEAT_SLUG_OVER_PRESS);
        //            }
        //            if (attachError == 3)
        //            {
        //                placeResult = PAD_STATUS.ATTACH_UNDERPRESS;
        //                mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
        //                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_UNDER_PRESS);
        //            }
        //            else if (attachError == 4)
        //            {
        //                placeResult = PAD_STATUS.ATTACH_OVERPRESS;
        //                mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);
        //                errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_SENSOR_OVER_PRESS);
        //            }
        //            mc.board.padStatus(BOARD_ZONE.WORKING, mc.hd.tool.padX, mc.hd.tool.padY, placeResult, out ret.b);
        //            sqc = SQC.STOP; break;

        //        #endregion

        //        #region case 80 xy pad c2 move
        //        case 80:
        //            rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
        //            rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
        //            if (mc.para.HDC.detectDirection.value == 0)
        //            {
        //                Y.move(cPos.y.PADC1(padY, true), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.move(cPos.x.PADC1(padX, true), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            else
        //            {
        //                Y.move(cPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.move(cPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            sqc++; break;
        //        case 81:
        //            if (mc.para.HDC.detectDirection.value == 0)
        //            {
        //                #region HDC.PADC1.req
        //                hdcP1X = 0;
        //                hdcP1Y = 0;
        //                hdcP1T_1 = 0;
        //                if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //                mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
        //                if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                #endregion

        //            }
        //            else
        //            {
        //                #region HDC.PADC2.req
        //                hdcP1X = 0;
        //                hdcP1Y = 0;
        //                hdcP1T_1 = 0;
        //                if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //                mc.hdc.lighting_exposure(mc.para.HDC.modelPADC2.light, mc.para.HDC.modelPADC2.exposureTime);
        //                if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                #endregion

        //            }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 82:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 83:
        //            if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE_ALL()) break;
        //            sqc++; break;
        //        case 84:
        //            sqc = 90; break;
        //        #endregion

        //        #region case 90 triggerHDC
        //        case 90:
        //            if (mc.hdc.req == false) { sqc = 100; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 91:
        //            if (dwell.Elapsed < 15) break; // head camera delay
        //            if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
        //            triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 92:
        //            if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
        //            triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 100; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 93:
        //            if (dwell.Elapsed < 300) break;
        //            sqc = 100; break;
        //        #endregion

        //        #region case 100 xy pad c4 move
        //        case 100:
        //            rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
        //            rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
        //            if (mc.para.HDC.detectDirection.value == 0)
        //            {
        //                Y.move(cPos.y.PADC3(padY, true), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.move(cPos.x.PADC3(padX, true), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            else
        //            {
        //                Y.move(cPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
        //                X.move(cPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
        //            }
        //            sqc++; break;
        //        case 101:
        //            if (mc.hdc.RUNING) break;
        //            if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

        //            if (mc.para.HDC.detectDirection.value == 0)
        //            {
        //                #region HDC.PADC1.result
        //                if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
        //                        hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
        //                        hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
        //                        hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
        //                        hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                    hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                    hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                }
        //                if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                {
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PAD P1 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                        //string str = "PAD P1 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                        mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                    }
        //                }
        //                if (dev.debug)
        //                {
        //                    if (Math.Abs(hdcP1X) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1Y) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1T_1) > 10)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Math.Abs(hdcP1X) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_X_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_X_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1Y) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_Y_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_Y_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1T_1) > 5)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P1-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C1_T_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_T_RESULT_OVER); break;
        //                        }
        //                    }
        //                }
        //                #endregion
        //                #region HDC.PADC3.req
        //                hdcP2X = 0;
        //                hdcP2Y = 0;
        //                hdcP2T_1 = 0;
        //                if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //                mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
        //                if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                #endregion
        //            }
        //            else
        //            {
        //                #region HDC.PADC2.result
        //                if (mc.hd.reqMode == REQMODE.DUMY) { }
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
        //                        hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
        //                        hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        hdcP1X = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
        //                        hdcP1Y = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
        //                        hdcP1T_1 = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
        //                    }
        //                }
        //                else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
        //                    hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
        //                    hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
        //                }
        //                if (hdcP1X == -1 && hdcP1Y == -1 && hdcP1T_1 == -1) // HDC Vision Result Error
        //                {
        //                    if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                    {
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PAD P2 Chk Fail(Processing ERROR)-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //                        //string str = "PAD P2 Chk Fail(Processing ERROR)-PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]";
        //                        mc.log.debug.write(mc.log.CODE.ERROR, tempSb.ToString());
        //                        sqc = 120; break;
        //                    }
        //                    else
        //                    {
        //                        tempSb.Clear(); tempSb.Length = 0;
        //                        tempSb.AppendFormat("PadX[{0}],PadY[{1}]", (padX + 1), (padY + 1));
        //                        //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "]";
        //                        errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P1_VISION_PROCESS_FAIL); break;
        //                    }
        //                }
        //                if (dev.debug)
        //                {
        //                    if (Math.Abs(hdcP1X) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1Y) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1T_1) > 10)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (Math.Abs(hdcP1X) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-X Compensation Amount Limit Error : {0:F1}um", hdcP1X));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_X_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1X);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1X).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_X_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1Y) > 5000)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-Y Compensation Amount Limit Error : {0:F1}um", hdcP1Y));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_Y_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C21_Y_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1Y);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1Y).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_Y_RESULT_OVER); break;
        //                        }
        //                    }
        //                    if (Math.Abs(hdcP1T_1) > 5)
        //                    {
        //                        mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HDC P2-T Compensation Amount Limit Error : {0:F1}degree", hdcP1T_1));
        //                        if (mc.para.HDC.failretry.value > 0 && mc.hd.tool.hdcfailcount < mc.para.HDC.failretry.value)
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                            sqc = 120; break;
        //                        }
        //                        else
        //                        {
        //                            if (mc.para.HDC.imageSave.value == 1) mc.hdc.cam.writeLogGrapImage("HDC_C2_T_Limit");
        //                            tempSb.Clear(); tempSb.Length = 0;
        //                            tempSb.AppendFormat("PadX[{0}],PadY[{1}],Result[{2:F1}]", (padX + 1), (padY + 1), hdcP1T_1);
        //                            //string str = "PadX[" + (padX + 1).ToString() + "],PadY[" + (padY + 1).ToString() + "],Result[" + Math.Round(hdcP1T).ToString() + "]";
        //                            errorCheck(ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_HDC_P2_T_RESULT_OVER); break;
        //                        }
        //                    }
        //                }
        //                #endregion
        //                #region HDC.PADC4.req
        //                hdcP2X = 0;
        //                hdcP2Y = 0;
        //                hdcP2T_1 = 0;
        //                if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
        //                {
        //                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
        //                {
        //                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
        //                    {
        //                        mc.hdc.reqMode = REQMODE.FIND_MODEL;
        //                        mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
        //                    }
        //                    else mc.hdc.reqMode = REQMODE.GRAB;
        //                }
        //                else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
        //                {
        //                    mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
        //                }
        //                else mc.hdc.reqMode = REQMODE.GRAB;
        //                mc.hdc.lighting_exposure(mc.para.HDC.modelPADC4.light, mc.para.HDC.modelPADC4.exposureTime);
        //                if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
        //                #endregion
        //            }

        //            dwell.Reset();
        //            sqc++; break;
        //        case 102:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 103:
        //            if (!X_AT_DONE || !Y_AT_DONE) break;
        //            sqc = 110; break;
        //        #endregion

        //        #region case 110 triggerHDC
        //        case 110:
        //            if (mc.hdc.req == false) { sqc = 50; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 111:
        //            if (dwell.Elapsed < 15) break; // head camera delay
        //            if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
        //            triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 112:
        //            if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
        //            triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
        //            if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 50; break; }
        //            dwell.Reset();
        //            sqc++; break;
        //        case 113:
        //            if (dwell.Elapsed < 300) break;
        //            sqc = 50; break;
        //        #endregion

        //        case 120:
        //            if (!X_AT_TARGET || !Y_AT_TARGET) break;
        //            dwell.Reset();
        //            sqc++; break;
        //        case 121:
        //            if (!X_AT_DONE || !Y_AT_DONE) break;
        //            tempSb.Clear(); tempSb.Length = 0;
        //            tempSb.AppendFormat("PAD Chk Fail-PadX[{0}],PadY[{1}], FailCnt[{2}]", (padX + 1), (padY + 1), mc.hd.tool.hdcfailcount);
        //            mc.log.debug.write(mc.log.CODE.EVENT, tempSb.ToString());
        //            //EVENT.statusDisplay("PAD Chk Fail-PadX[" + (padX + 1).ToString() + "],PadY:[" + (padY + 1).ToString() + "], FailCnt[" + mc.hd.tool.hdcfailcount.ToString() + "]");
        //            mc.hd.tool.hdcfailcount++;
        //            hdcfailchecked = true;
        //            if ((mc.hd.tool.hdcfailcount % 2) == 0) sqc = 10;
        //            else sqc = 80;
        //            break;

        //        case SQC.ERROR:
        //            //string dspstr = "HD ulc_place Esqc " + Esqc.ToString();

                

        //            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD ulc_place Esqc {0}", Esqc));
        //            //EVENT.statusDisplay(str);
        //            sqc = SQC.STOP; break;

        //        case SQC.STOP:
        //            sqc = SQC.END; break;


        //    }
        //}
		public void place_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					sqc++; break;
				case 14:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					sqc++; break;
				case 16:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.REF0
				case 20:
					double tmpPos;
					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (ret.d - tPos.y[mc.hd.order.bond_done].PAD(0) < 10000) tmpPos = tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000; else tmpPos = tPos.z[mc.hd.order.bond_done].XY_MOVING - 3500;
					Y.moveCompare(cPos.y.REF0, Z, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.REF0, Z, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.bond_done].moveCompare(tPos.t[mc.hd.order.bond_done].ZERO, Z, tmpPos, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(mc.hd.order.bond_done) || !T_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					 if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.bond_done) || !T_AT_DONE(mc.hd.order.bond_done)) break;
					 sqc++; break;
				case 24:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				#endregion

				case SQC.ERROR:
					//string str = "HD place_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		public void place_standby()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
                    Z[workingZ].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[workingZ].config.axisCode, sqc, ret.message)) break;

					//Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[workingZ].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[workingZ].config.axisCode, sqc, ret.message)) break;
                    //Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
					mc.OUT.HD.SUC(workingZ, false, out ret.message);
                    mc.OUT.HD.BLW(workingZ, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(workingZ, false, out ret.message);
					sqc++; break;
				case 14:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (!Z_AT_TARGET(workingZ)) break;
                    //if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(workingZ, false, out ret.message);
					}
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					sqc++; break;
				case 16:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (levelD2 == 0) { sqc += 3; break; }
					Z[workingZ].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[workingZ].config.axisCode, sqc, ret.message)) break;
                    //Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

                    if (!Z_AT_TARGET(workingZ)) break;
                    //if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z[workingZ].move(tPos.z[workingZ].XY_MOVING, out ret.message); if (mpiCheck(Z[workingZ].config.axisCode, sqc, ret.message)) break;
                    //Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    sqc = 20; break;
				#endregion

				#region case 20 XY.move.standby
				case 20:
					double tmpPos;
					Y.commandPosition(out ret.d, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (ret.d - tPos.y[workingZ].PAD(0) < 10000) tmpPos = tPos.z[workingZ].XY_MOVING - 2000; else tmpPos = tPos.z[workingZ].XY_MOVING - 3500;
					Y.moveCompare(mc.para.CAL.standbyPosition.y.value, Z[workingZ].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(mc.para.CAL.standbyPosition.x.value, Z[workingZ].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[workingZ].moveCompare(tPos.t[workingZ].ZERO, Z[workingZ].config, tmpPos, true, false, out ret.message); if (mpiCheck(T[workingZ].config.axisCode, sqc, ret.message)) break;
                    mc.log.workHistory.write("Compare Axis : " + workingZ);
					sqc++; break;
				case 21:
                    if (!Z_AT_TARGET(workingZ)) break;
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
					dwell.Reset();
					sqc++; break;
				case 22:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(workingZ) || !T_AT_TARGET(workingZ)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(workingZ) || !T_AT_DONE(workingZ)) break;
					sqc++; break;
				case 24:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.OFF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.OFF]);		// 동작이 끝난 후 조명을 끈다.
			        mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.OFF], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.OFF]);

					sqc = SQC.STOP; break;

                default:        // 이상하게 30으로 호출되는 경우가 생김. 그래서 멍때림.
                    sqc = SQC.ERROR;
                    break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_standby Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		
		// 사용하지 않음
		public void place_waste()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
                    if (mc.hd.order.waste == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
                    if (mc.hd.order.bond_done == (int)ORDER.EMPTY) { sqc = 30; break; }
					sqc++; break;
				case 1:
					#region pos set
					Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					sqc++; break;
				case 14:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					sqc++; break;
				case 16:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.WASTE
				case 20:
					 double tmpPos;
					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (ret.d - tPos.y[mc.hd.order.waste].PAD(0) < 10000) tmpPos = tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000; else tmpPos = tPos.z[mc.hd.order.bond_done].XY_MOVING - 3500;
					Y.moveCompare(tPos.y[mc.hd.order.waste].WASTE, Z[mc.hd.order.bond_done].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(tPos.x[mc.hd.order.waste].WASTE, Z[mc.hd.order.bond_done].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.waste].moveCompare(tPos.t[mc.hd.order.waste].ZERO, Z[mc.hd.order.bond_done].config, tmpPos, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    mc.log.workHistory.write("Compare Axis : " + (int)mc.hd.order.bond_done);
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(mc.hd.order.bond_done) || !T_AT_TARGET(mc.hd.order.waste)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
                    if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.bond_done) || !T_AT_DONE(mc.hd.order.waste)) break;
					mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(mc.hd.order.waste, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.hd.order.set(mc.hd.order.waste, (int)ORDER.NO_DIE);
					sqc++; break;
				case 25:
					sqc = SQC.STOP; break;
				#endregion

                #region case 30 둘 다 Fail 일 경우
                case 30:
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
                    dwell.Reset();
                    sqc++; break;
                case 31:
                    if (!Z_AT_TARGET_ALL()) break;
                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (!Z_AT_DONE_ALL()) break;
                    X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(tPos.x[mc.hd.order.waste].WASTE - ret.d1) > 50000 || Math.Abs(tPos.y[mc.hd.order.waste].WASTE - ret.d2) > 50000)
                    {
                        X.move(tPos.x[mc.hd.order.waste].WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.waste].WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        X.move(tPos.x[mc.hd.order.waste].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.waste].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 33:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 34:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    mc.IN.HD.VAC_CHK(mc.hd.order.waste, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.HD.SUC(mc.hd.order.waste, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.HD.BLW(mc.hd.order.waste, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b) mc.para.ETC.wasteCount.value += 1;
                    dwell.Reset();
                    sqc++; break;
                case 35:
                    if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
                    mc.OUT.HD.BLW(mc.hd.order.waste, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.hd.order.set(mc.hd.order.waste, (int)ORDER.NO_DIE);
                    sqc++; break;
                case 36:
                    if (mc.pd.ERROR && ((mc.hd.reqMode != REQMODE.STEP && mc.hd.reqMode != REQMODE.PICKUP && mc.hd.reqMode != REQMODE.WASTE)))
                    { sqc = SQC.ERROR; break; }
                    if (mc.hd.order.waste == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
                    else { sqc = 30; break; }  
                #endregion

				case SQC.ERROR:
					//string str = "HD place_waste Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_waste Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double laserPosX;
		double laserPosY;
		public void check_trayreverse()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region Set method
					if ((int)mc.para.CV.trayReverseCheckMethod1.value == 0) sqc = 10;
					else sqc = 20;
					break;
					#endregion

				case 10:
                    laserPosX = mc.para.CV.trayReverseXPos.value - mc.coor.MP.TOOL.X.LASER.value + mc.para.CAL.HDC_LASER.x.value;
                    laserPosY = mc.para.CV.trayReverseYPos.value - mc.coor.MP.TOOL.Y.LASER.value + mc.para.CAL.HDC_LASER.y.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if ((int)mc.para.CV.trayReverseResult.value == 1)   // ON Check
					{
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						if (ret.b1)
						{
							sqc++;
							dwell.Reset();
							break;
						}
						else   // Time Check.. 5초
						{
							if (dwell.Elapsed < 5000) break;
							else
							{
								// ERROR
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
					else  // OFF Check
					{
						if (dwell.Elapsed < 3000) break;    // 3초를 기다려야 한다. 안 기다려도 상관없는데..
						else
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
							if (ret.b1 == false)
							{
								sqc++;
								dwell.Reset();
								break;
							}
							else
							{
								// ERROR;
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
				case 14:
					if (dwell.Elapsed < 20) break;
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					if ((int)mc.para.CV.trayReverseResult.value == 1)   // ON Check
					{
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Tray Reverse Check OK(#1). Tray Height : {0:F3}", ret.d));
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#1)");
					}
					sqc = SQC.STOP; break;

				case 20:
					laserPosX = mc.para.CV.trayReverseXPos.value;
					laserPosY = mc.para.CV.trayReverseYPos.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					hdcResult = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelTrayReversePattern1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE1;
                            mc.hdc.reqPassScore = mc.para.HDC.modelTrayReversePattern1.passScore.value;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                    mc.hdc.req = true;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 15) break; // head camera delay
                    //if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    //dwell.Reset();
					sqc++; break;
				case 25:
                    if (mc.hdc.visionEnd) break;

                    //if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    //dwell.Reset();
					sqc++; break;
				case 26:
					if (mc.hdc.RUNING || !mc.hdc.visionEnd) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region Tray Reverse result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelTrayReversePattern1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern1.isCreate.value == (int)BOOL.TRUE)
						{
							trayReversePX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultX;
							trayReversePY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultY;
							trayReversePT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultAngle;
							hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE1].resultScore * 100;
						}
					}
					#endregion
					if (trayReversePX == -1 && trayReversePY == -1 && trayReversePT == -1) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#1)!");
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else if (hdcResult < mc.para.HDC.modelTrayReversePattern1.passScore.value) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Result Score is too Low(#1)!!");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#1)");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));

						sqc = SQC.STOP; 
					}
					break;

				case SQC.ERROR:
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_trayreverse(#1) Esqc {0}", Esqc));
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void check_trayreverse2()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region Set method
					if ((int)mc.para.CV.trayReverseCheckMethod2.value == 0) sqc = 10;
					else sqc = 20;
					break;
					#endregion

				case 10:
                    laserPosX = mc.para.CV.trayReverseXPos2.value - mc.coor.MP.TOOL.X.LASER.value + mc.para.CAL.HDC_LASER.x.value;
                    laserPosY = mc.para.CV.trayReverseYPos2.value - mc.coor.MP.TOOL.Y.LASER.value + mc.para.CAL.HDC_LASER.y.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if ((int)mc.para.CV.trayReverseResult2.value == (int)ON_OFF.ON)   // ON Check
					{
						mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
						if (ret.b1)
						{
							sqc++;
							dwell.Reset();
							break;
						}
						else   // Time Check.. 5초
						{
							if (dwell.Elapsed < 5000) break;
							else
							{
								// ERROR
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
					else  // OFF Check
					{
						if (dwell.Elapsed < 3000) break;    // 3초를 기다려야 한다. 안 기다려도 상관없는데..
						else
						{
							mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
							if (ret.b1 == false)
							{
								sqc++;
								dwell.Reset();
								break;
							}
							else
							{
								// ERROR;
								mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
								Esqc = sqc; sqc = SQC.ERROR;
								break;
							}
						}
					}
				case 14:
					if (dwell.Elapsed < 20) break;
					mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
					mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					if ((int)mc.para.CV.trayReverseResult2.value == (int)ON_OFF.ON)   // ON Check
					{
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Tray Reverse Check OK(#2). Tray Height : {0:F3}", ret.d));
					}
					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#2)");
					}
					sqc = SQC.STOP; break;

				case 20:
					laserPosX = mc.para.CV.trayReverseXPos2.value;
					laserPosY = mc.para.CV.trayReverseYPos2.value;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(laserPosX - ret.d1) > 50000 || Math.Abs(laserPosY - ret.d2) > 50000)
					{
						X.move(laserPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(laserPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(laserPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					hdcResult = 0;
					if (mc.hd.reqMode == REQMODE.DUMY) mc.hdc.reqMode = REQMODE.GRAB;
					else if (mc.para.HDC.modelTrayReversePattern2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.TRAY_REVERSE_SHAPE2;
                            mc.hdc.reqPassScore = mc.para.HDC.modelTrayReversePattern2.passScore.value;
						}
						else mc.hdc.reqMode = REQMODE.GRAB;
					}
					else mc.hdc.reqMode = REQMODE.GRAB;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.TRAY], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.TRAY]);
					//if (mc.swcontrol.useHwTriger == 1) mc.hdc.req = true;
                    mc.hdc.req = true;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (dwell.Elapsed < 15) break; // head camera delay
                    //if (mc.swcontrol.useHwTriger == 0) mc.hdc.req = true;
                    //triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    //dwell.Reset();
					sqc++; break;
				case 25:
                    if (mc.hdc.visionEnd) break;
                    //if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
                    //triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
                    //dwell.Reset();
					sqc++; break;
				case 26:
					if (mc.hdc.RUNING || !mc.hdc.visionEnd) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.hdc.cam.refresh_req) break;
					#region Tray Reverse result
					if (mc.hd.reqMode == REQMODE.DUMY) { }
					else if (mc.para.HDC.modelTrayReversePattern2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						if (mc.para.HDC.modelTrayReversePattern2.isCreate.value == (int)BOOL.TRUE)
						{
							trayReversePX = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultX;
							trayReversePY = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultY;
							trayReversePT = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultAngle;
							hdcResult = mc.hdc.cam.model[(int)HDC_MODEL.TRAY_REVERSE_SHAPE2].resultScore; 
						}
					}
					#endregion
					if (trayReversePX == -1 && trayReversePY == -1 && trayReversePT == -1) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Tray is NOT-correct Position or Reversed(#2)!");
						Esqc = sqc; sqc = SQC.ERROR;
					}
					else if (hdcResult < mc.para.HDC.modelTrayReversePattern2.passScore.value) // HDC Tray Reverse Result Error
					{
						mc.log.debug.write(mc.log.CODE.ERROR, "Result Score is too Low(#2)!!");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						Esqc = sqc; sqc = SQC.ERROR;
					}

					else
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "Tray reverse Check OK(#2)");
						mc.log.debug.write(mc.log.CODE.TRACE, String.Format("Result X : {0:f2}, Result Y : {1:f2}, Result T : {2:f2}, Result Score : {3:f2}%"
							, trayReversePX, trayReversePY, trayReversePT, hdcResult));
						sqc = SQC.STOP;
					}
					break;

				case SQC.ERROR:
					//string str = "HD check_trayreverse Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_trayreverse(#2) Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

        public void check_PCB_Tilt()
        {
            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    sqc++; break;
                case 1:
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
                    dwell.Reset();
                    sqc++; break;
                case 2:
                    if (!mc.hd.tool.Z_AT_TARGET_ALL()) break;
                    dwell.Reset();
                    sqc++; break;

                case 3:
                    if (!mc.hd.tool.Z_AT_DONE_ALL()) break;
                    sqc = 10; break;

                #region case 10 XY.move.PADC1
                case 10:
                    Y.move(lPos.y.PEDC1(padY), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.PEDC1(padX), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 11:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 13:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d <= -10) ret.d = -100;
                    laserResult[0] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 1 : {0:F3}", laserResult[0]));
                    sqc = 20; break;
                #endregion

                #region case 20 XY.move.PADC2
                case 20:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    Y.move(lPos.y.PEDC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.PEDC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 21:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 22:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 23:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[1] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 2 : {0:F3}", laserResult[1]));
                    sqc = 30; break;
                #endregion

                #region case 30 XY.move.PADC3
                case 30:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    Y.move(lPos.y.PEDC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.PEDC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 31:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 33:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[2] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 3 : {0:F3}", laserResult[2]));
                    sqc = 40; break;
                #endregion

                #region case 40 XY.move.PADC4
                case 40:
                    rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
                    rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
                    Y.move(lPos.y.PEDC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    X.move(lPos.x.PEDC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    sqc++; break;
                case 41:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 42:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 43:
                    if (dwell.Elapsed < 100) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
                    laserResult[3] = ret.d;
                    mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 4 : {0:F3}", laserResult[3]));
                    sqc++; break;
                case 44:
                    laserMaxVal = -1000;
                    laserMinVal = 1000;
                    for (int i = 0; i < 4; i++)
                    {
                        if (laserMaxVal < laserResult[i]) laserMaxVal = laserResult[i];
                        if (laserMinVal > laserResult[i]) laserMinVal = laserResult[i];
                    }
                    mc.log.debug.write(mc.log.CODE.INFO, "Max : " + Math.Round(laserMaxVal, 3).ToString() + ", Min : " + Math.Round(laserMinVal, 3).ToString() + " Tilt : " + Math.Round(Math.Abs(laserMaxVal - laserMinVal) * 1000, 3).ToString() + "[um]");

                    if ((laserMaxVal - laserMinVal) * 1000 > mc.para.HD.place.pressTiltLimit.value)
                    {
                        tempSb.Clear(); tempSb.Length = 0;
                        tempSb.AppendFormat("Tilt : {0:F1}[um]", (laserMaxVal - laserMinVal) * 1000);
                        mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.LASER_TILT_ERROR, out ret.b);
                        //string dispMsg = "Tilt : " + Math.Round((laserMaxVal - laserMinVal) * 1000).ToString() + "[um]";
                        errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_PRESS_TILT_ERROR);
                        break;
                    }
                    else
                    {
                        mc.board.padStatus(BOARD_ZONE.WORKING, padX, padY, PAD_STATUS.TILT_OK, out ret.b);
                    }
                    sqc = SQC.STOP; break;
                #endregion

                case SQC.ERROR:
                    //string str = "HD place_pbi Esqc " + Esqc.ToString();
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check Tilt Esqc {0}", Esqc));
                    //EVENT.statusDisplay(str);
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;


            }
        }
		double forceCheckX;
		double forceCheckY;
		public void check_force()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set

					#endregion
					sqc = 10; break;

				case 10:
					// move to force check position
                    forceCheckX = tPos.x[mc.hd.order.pick].LOADCELL;
                    forceCheckY = tPos.y[mc.hd.order.pick].LOADCELL;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(forceCheckX - ret.d1) > 50000 || Math.Abs(forceCheckX - ret.d2) > 50000)
					{
						X.move(forceCheckX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(forceCheckY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(forceCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(forceCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (dwell.Elapsed < 500) break;
					//mc.hd.tool.F.kilogram(0.2, out ret.message); if (ioCheck(sqc, ret.message)) break;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].LOADCELL + 1000, 0.01, 0.01, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].LOADCELL + 100, 0.001, 0.01, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					if (dwell.Elapsed < 1000) break;		// 추가 Delay
					sqc++; break;
				case 17:
					// Check Speed는 1mm/sec
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].LOADCELL - mc.para.CAL.force.touchOffset.value, 0.0005, 0.01, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 19:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 20:
					if (dwell.Elapsed < 1500) break;
					sqc++; break;
				case 21:
					//ret.d = mc.loadCell.getData(0);	// read from bottom loadcell
					ret.d1 = mc.AIN.HeadLoadcell();
					sqc++; break;
				case 22:
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].LOADCELL + 100, 0.001, 0.01, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					tempSb.Clear(); tempSb.Length = 0;
					tempSb.AppendFormat("Command: {0}[kg], Bottom : {1}[kg], Top : {2:F3}[kg], Diff : {3:F3}[kg]", mc.para.ETC.forceCompenSet.value, ret.d, ret.d2, Math.Abs(ret.d - ret.d2));
					mc.log.trace.write(mc.log.CODE.FORCE, tempSb.ToString());
					mc.log.debug.write(mc.log.CODE.FORCE, tempSb.ToString());
					// Percentage
					if (Math.Abs(mc.para.ETC.forceCompenSet.value - ret.d) > mc.para.ETC.forceCompenLimit.value)
					{
						errorCheck(ERRORCODE.FULL, sqc, "Different Gram : " + Math.Round((Math.Abs(mc.para.ETC.forceCompenSet.value - ret.d)) * 1000).ToString(), ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
						break;
					}
					if (Math.Abs(ret.d - ret.d2) > mc.para.ETC.forceCompenLimit.value)
					{
						errorCheck(ERRORCODE.FULL, sqc, "Different Gram : " + Math.Round((Math.Abs(ret.d - ret.d2)) * 1000).ToString(), ALARM_CODE.E_MACHINE_RUN_FORCE_LEVEL_OVER);
						break;
					}
					dwell.Reset();
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_force Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_force Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		double flatCheckX;
		double flatCheckY;
		int flatCheckIndex;
		double moveDirX;
		double moveDirY;
		double[] flatCheckResult = new double[4];
		double flatMax;
		double flatMin;

        public double flatCheckDifference;

		public void check_flatness()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
                    if (mc.hd.order.checktilt == (int)ORDER.EMPTY) { sqc = SQC.STOP; break; }
					#region probe reset
					mc.touchProbe.setZero(out ret.b); if (!ret.b) { errorCheck(ERRORCODE.UTILITY, sqc, "Touch Probe Zero Setting Error!"); break;}
                    flatCheckIndex = 0;
					flatCheckDifference = 0.0;

					#endregion
					sqc = 10; break;

				case 10:
					// move to touch probe check position
					if (flatCheckIndex == 0) { moveDirX = 0; moveDirY = -1; }
					else if (flatCheckIndex == 1) { moveDirX = -1; moveDirY = 0; }
					else if (flatCheckIndex == 2) { moveDirX = 0; moveDirY = 1; }
					else if (flatCheckIndex == 3) { moveDirX = 1; moveDirY = 0; }

                    flatCheckX = tPos.x[mc.hd.order.checktilt].TOUCHPROBE + (mc.para.MT.flatCompenToolSize.x.value * 500 - (mc.para.ETC.flatCompenOffset.value + 1000)) * moveDirX;
                    flatCheckY = tPos.y[mc.hd.order.checktilt].TOUCHPROBE + (mc.para.MT.flatCompenToolSize.y.value * 500 - (mc.para.ETC.flatCompenOffset.value + 1000)) * moveDirY;

					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //if (Math.Abs(flatCheckX - ret.d1) > 50000 || Math.Abs(flatCheckY - ret.d2) > 50000)
                    //{
                    //   X.move(flatCheckX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    //   Y.move(flatCheckY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //}
                    //else
					{
						X.move(flatCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(flatCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					// move to Z
					Z[mc.hd.order.checktilt].move(tPos.z[mc.hd.order.checktilt].TOUCHPROBE, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.checktilt].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 14:
                    if (!Z_AT_TARGET(mc.hd.order.checktilt)) break;
					dwell.Reset();
					sqc++; break;
				case 15:
                    if (!Z_AT_DONE(mc.hd.order.checktilt)) break;
					dwell.Reset();
					sqc++; break;
				case 16:
					// wait settling
					if (dwell.Elapsed < 1000) break;
					dwell.Reset();
					sqc++; break;
				case 17:
					mc.touchProbe.getData(out ret.d, out ret.b);
                    if (!ret.b) errorCheck(ERRORCODE.UTILITY, sqc, "Touch Probe Error!");
                    else
                    {
                        flatCheckResult[flatCheckIndex] = ret.d;
                        mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_HD_TOOL_FLATNESS, flatCheckIndex, ret.d.ToString("f4")));
                        sqc++;
                    }
					break;
				case 18:
					Z[mc.hd.order.checktilt].move(tPos.z[mc.hd.order.checktilt].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.checktilt].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 19:
                    if (!Z_AT_TARGET(mc.hd.order.checktilt)) break;
					dwell.Reset();
					sqc++; break;
				case 20:
                    if (!Z_AT_DONE(mc.hd.order.checktilt)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					flatCheckIndex++;
					if (flatCheckIndex == 4) { sqc++; break; }
					sqc = 10; break;
				case 22:
					double big=-100, small=100;
					for (int i = 0; i < 4; i++)
					{
						if (flatCheckResult[i] > big) big = flatCheckResult[i];
						if (flatCheckResult[i] < small) small = flatCheckResult[i];
					}
					mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_HD_TOOL_FLATNESS_RESULT, Math.Round(big, 4).ToString(), Math.Round(small, 4).ToString(), Math.Round((big - small) * 1000, 2).ToString("f1")));
					if (Math.Round((big - small) * 1000, 2) > mc.para.ETC.flatCompenLimit.value)
					{
						flatCheckDifference = Math.Round((big - small) * 1000, 2);
						errorCheck(ERRORCODE.FULL, sqc, "Difference : " + flatCheckDifference.ToString(), ALARM_CODE.E_MACHINE_RUN_NOZZLE_FLATNESS_OVER);
                        break;
					}
                    mc.hd.order.set(mc.hd.order.checktilt, (int)ORDER.EMPTY);
					sqc = 1; break;

				case SQC.ERROR:
					//string str = "HD check_flat Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD check_flat Esqc {0}",Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

        double distanceX; double distanceY;

		public void check_Pedestal_flatness()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					flatCheckIndex = 0;
					flatCheckDifference = 0.0;
                    distanceX = mc.para.ETC.flatPedestalOffset.value;
                    distanceY = mc.para.ETC.flatPedestalOffset.value;
					padX = 0;
					padY = 0;
					sqc++; break;
				case 1:
					if (mc.pd.RUNING) break;
					mc.pd.req = true;
					mc.pd.reqMode = REQMODE.COMPEN_FLAT;
					sqc++; break;
				case 2:
					if (mc.pd.RUNING) break;
					sqc = 10; break;

				case 10:
					// move to laser check position
					if (flatCheckIndex == 0) 
					{
                        padX = Convert.ToInt32(mc.para.MT.padCount.x.value - 1);
                        padY = Convert.ToInt32(mc.para.MT.padCount.y.value - 1);
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) + mc.para.MT.pedestalSize.x.value * 500 - distanceX;
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) + mc.para.MT.pedestalSize.y.value * 500 - distanceY;
					}
                    else if (flatCheckIndex == 1)
                    {
                        padX = Convert.ToInt32(mc.para.MT.padCount.x.value) - 1;
                        padY = 0;
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) + mc.para.MT.pedestalSize.x.value * 500 - distanceX;
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) - (mc.para.MT.pedestalSize.y.value * 500 - distanceY);
                    }
                    else if (flatCheckIndex == 2)
                    {
                        padX = 0;
                        padY = 0;
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) - (mc.para.MT.pedestalSize.x.value * 500 - distanceX);
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) - (mc.para.MT.pedestalSize.y.value * 500 - distanceY);
                    }
                    else if (flatCheckIndex == 3)
                    {
                        padX = 0;
                        padY = Convert.ToInt32(mc.para.MT.padCount.y.value - 1);
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) - (mc.para.MT.pedestalSize.x.value * 500 - distanceX);
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) + mc.para.MT.pedestalSize.y.value * 500 - distanceY;
                    }
					sqc++; break;
				case 11:
					X.move(flatCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.move(flatCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;

					dwell.Reset();
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 14:
					// move to Z
					if (dwell.Elapsed < 1000) break;
					//mc.OUT.HD.LS.ZERO(true, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
                    //mc.IN.LS.ALM(out ret.b1, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
                    //mc.IN.LS.FAR(out ret.b2, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;
                    //mc.IN.LS.NEAR(out ret.b3, out ret.message); if (ret.message != RetMessage.OK) ret.d = -1;

					flatCheckResult[flatCheckIndex] = ret.d;
					mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_PD_FLATNESS, flatCheckIndex, (Math.Round((flatCheckResult[flatCheckIndex] * 1000), 1)).ToString()));
					flatCheckIndex++;
					if (flatCheckIndex == 4)
						sqc++;
					else sqc = 10;
					break;
				
				case 15:
					flatMax = flatCheckResult[0];
					flatMin = flatCheckResult[0];

					for (int i = 0; i < 4; i++)
					{
						if (flatMax < flatCheckResult[i])
							flatMax = flatCheckResult[i];
						if (flatMin > flatCheckResult[i])
							flatMin = flatCheckResult[i];
					}
                    mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_PD_FLATNESS_RESULT, Math.Round(flatMax, 4).ToString(), Math.Round(flatMin, 4).ToString(), (Math.Round(flatMax - flatMin, 3) * 1000).ToString()));

                    if (Math.Round(flatMax - flatMin, 3) * 1000 >= mc.para.ETC.flatCompenLimit.value)
                    {
                        errorCheck(ERRORCODE.HD, sqc, "Pedestal 평탄도가 너무 높습니다. 평탄도를 조정해주세요.");
                        break;
                    }
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR; 
                        break;
                    }
					sqc++; break;
				case 16:
					if (mc.pd.RUNING) break;
                    if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_flat Esqc " + Esqc.ToString();
                    mc.pd.SetZDown();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("PD check_flat Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

        public void check_Attach_flatness()
        {
            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    flatCheckIndex = 0;
                    flatCheckDifference = 0.0;
                    distanceX = 3500;
                    distanceY = 3500;
                    sqc++; break;
                case 1:
                    if (mc.pd.RUNING) break;
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
                    dwell.Reset();
                    sqc++; break;
                case 2:
                    if (!Z_AT_TARGET_ALL()) break;
                    dwell.Reset();
                    sqc++; break;
                case 3:
                    if (!Z_AT_DONE((int)UnitCodeHead.HD1) || !Z_AT_DONE((int)UnitCodeHead.HD2)) break;
                    sqc = 10; break;

                case 10:
                    // move to laser check position
                    if (flatCheckIndex == 0)      
                    {
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) + mc.para.MT.lidSize.x.value * 500 - distanceX;
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) + mc.para.MT.lidSize.y.value * 500 - distanceY;
                    }
                    else if (flatCheckIndex == 1)
                    {
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) + mc.para.MT.lidSize.x.value * 500 - distanceX;
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) - (mc.para.MT.lidSize.y.value * 500 - distanceY);
                    }
                    else if (flatCheckIndex == 2)
                    {
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) - (mc.para.MT.lidSize.x.value * 500 - distanceX);
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) - (mc.para.MT.lidSize.y.value * 500 - distanceY);
                    }
                    else if (flatCheckIndex == 3)
                    {
                        flatCheckX = mc.hd.tool.lPos.x.PAD(padX) - (mc.para.MT.lidSize.x.value * 500 - distanceX);
                        flatCheckY = mc.hd.tool.lPos.y.PAD(padY) + mc.para.MT.lidSize.y.value * 500 - distanceY;
                    }
                    sqc++; break;
                case 11:
                    X.move(flatCheckX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                    Y.move(flatCheckY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;

                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!X_AT_TARGET || !Y_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case 13:
                    if (!X_AT_DONE || !Y_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case 14:
                    // move to Z
                    if (dwell.Elapsed < 2000) break;
                    ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -1;
                    flatCheckResult[flatCheckIndex] = ret.d;
                    mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_ATTACH_FLATNESS, flatCheckIndex, (Math.Round((flatCheckResult[flatCheckIndex] * 1000), 1)).ToString()));
                    flatCheckIndex++;
                    if (flatCheckIndex == 4)
                        sqc++;
                    else sqc = 10;
                    break;

                case 15:
                    flatMax = flatCheckResult[0];
                    flatMin = flatCheckResult[0];

                    for (int i = 0; i < 4; i++)
                    {
                        if (flatMax < flatCheckResult[i])
                            flatMax = flatCheckResult[i];
                        if (flatMin > flatCheckResult[i])
                            flatMin = flatCheckResult[i];
                    }
                    mc.log.trace.write(mc.log.CODE.FLATNESS, String.Format(textResource.LOG_TRACE_ATTACH_FLATNESS_RESULT, Math.Round(flatMax, 4).ToString(), Math.Round(flatMin, 4).ToString(), (Math.Round(flatMax - flatMin, 3) * 1000).ToString()));

                    if (Math.Round(flatMax - flatMin, 3) * 1000 >= mc.para.HD.place.pressTiltLimit.value)
                    {
                        errorCheck(ERRORCODE.HD, sqc, "Slug Flatness is too High!");
                        break;
                    }
                    if (!mc.pd.ERROR) { mc.pd.req = true; mc.pd.reqMode = REQMODE.READY; }
                    else
                    {
                        mc.log.debug.write(mc.log.CODE.ERROR, textResource.LOG_ERROR_PEDESTAL_NOT_READY);
                        Esqc = sqc; sqc = SQC.ERROR;
                        break;
                    }
                    sqc++; break;
                case 16:
                    if (mc.pd.RUNING) break;
                    if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc = SQC.STOP; break;

                case SQC.ERROR:
                    //string str = "HD check_flat Esqc " + Esqc.ToString();
                    mc.pd.SetZDown();
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("PD check_flat Esqc {0}", Esqc));
                    //EVENT.statusDisplay(str);
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    sqc = SQC.END; break;
            }
        }

		double refPosX;
		double refPosY;
		public int refcheckcount;
		public void check_reference()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set

					#endregion
					sqc = 10; break;

				case 10:
					refPosX = mc.hd.tool.cPos.x.REF0;
					refPosY = mc.hd.tool.cPos.y.REF0;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (Math.Abs(refPosX - ret.d1) > 50000 || Math.Abs(refPosY - ret.d2) > 50000)
					{
						X.move(refPosX, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(refPosY, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
						X.move(refPosX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
						Y.move(refPosY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 11:
					mc.hdc.reqMode = REQMODE.FIND_CIRCLE;
					mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.REF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.REF]);
					mc.hdc.req = true;
					sqc++; break;
				case 12:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 13:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc = 20; break;

				case 20:
					if (mc.hdc.req == false) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 15) break; // head camera delay
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 30; break; }
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 300) break;
					sqc = 30; break;

				case 30:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 31:
					if ((double)mc.hdc.cam.circleCenter.resultRadius == -1)
					{
						refcheckcount++;
						if (refcheckcount < 3)
						{
							sqc = 10; break;
						}
						else
						{
							errorCheck(ERRORCODE.HD, sqc, "", ALARM_CODE.E_HDC_NOT_FIND_REFERENC_MARK);
							break;
						}
					}
					else
					{
						double refX = mc.hdc.cam.circleCenter.resultX;
						double refY = mc.hdc.cam.circleCenter.resultY;
						mc.log.REF.write(mc.log.CODE.REFERENCE, String.Format("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY));
						mc.log.debug.write(mc.log.CODE.REFERENCE, String.Format("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY));
						if (Math.Abs(refX) > mc.para.ETC.refCompenLimit.value || Math.Abs(refY) > mc.para.ETC.refCompenLimit.value)
						{
							refcheckcount++;
							if (refcheckcount < 3)
							{
								sqc = 10; break;
							}
							else
							{
								tempSb.Clear(); tempSb.Length = 0;
								tempSb.AppendFormat("X Offset : {0:F3} [um], Y Offset : {1:F3} [um]", refX, refY);
								//string str = "X Offset : " + Math.Round(refX, 3).ToString() + " [um], Y Offset : " + Math.Round(refY, 3).ToString() + " [um]";
								errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_REFERENCE_OVER);
								break;
							}
						}
					}
					sqc = SQC.STOP; break;

				case SQC.ERROR:
					//string str = "HD check_reference Esqc " + Esqc.ToString();
					//mc.log.debug.write(mc.log.CODE.ERROR, str);
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;
			}
		}

		public void place_pbi()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					sqc++; break;
				case 16:
					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (dwell.Elapsed < delayD2) break;
					sqc++; break;
				case 19:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PADC1
				case 20:
					Y.moveCompare(cPos.y.PADC1(padY, true), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.PADC1(padX, true), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.bond_done].moveCompare(tPos.t[mc.hd.order.bond_done].ZERO, Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(mc.hd.order.bond_done) || !T_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.bond_done) || !T_AT_DONE(mc.hd.order.bond_done)) break;
					sqc = 30; break;
					#endregion

				#region case 30 hdc.req
				case 30:
					#region HDC.PADC1.req
					mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if(mc.hdc.RUNING) break;
					if(mc.hdc.ERROR) {sqc = SQC.ERROR; break;}
					hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;

					if (mc.hd.reqMode == REQMODE.AUTO || mc.hd.reqMode == REQMODE.DUMY) { sqc = 40; break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 300) break;
					sqc = 40; break;
				#endregion

				#region case 40 XY.move.PADC3
				case 40:
					Y.moveCompare(cPos.y.PADC3(padY, true), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(cPos.x.PADC3(padX, true), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.bond_done].moveCompare(tPos.t[mc.hd.order.bond_done].ZERO, Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(mc.hd.order.bond_done) || !T_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 43:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.bond_done) || !T_AT_DONE(mc.hd.order.bond_done)) break;
					sqc = 50; break;
				#endregion

				#region case 50 hdc.req
				case 50:
					#region HDC.PADC3.req
					mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { sqc = SQC.ERROR; break; }
					//hdcP2X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP2Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP2T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;

					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T_1 + hdcP2T_1) / 2;
					ret.s =  "PBI hdcX " + Math.Round(hdcX, 2).ToString();
					ret.s += "  hdcY " + Math.Round(hdcY, 2).ToString();
					ret.s += "  hdcT " + Math.Round(hdcT, 2).ToString() + "\n";
					mc.log.debug.write(mc.log.CODE.TRACE, ret.s);
					//EVENT.statusDisplay(ret.s);
					sqc++; break;
				case 54:
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_pbi Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pbi Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		double[] laserResult = new double[4];
		double laserMaxVal, laserMinVal;
		public void place_laser()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					#region pos set
					Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					sqc++; break;
					#endregion
				case 2:
					mc.OUT.HD.LS.ON(out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					if (!ret.b)
					{
						mc.OUT.HD.LS.ON(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						mc.log.debug.write(mc.log.CODE.INFO, "Laser On Delay : 5000 ms");
						dwell.Reset();
						sqc++;
					}
					else sqc = 10; 
					break;
				case 3:
					if (dwell.Elapsed < 5000) break;		// laser on delay
					else sqc = 10;
					break;

				#region case 10 Z move up
				case 10:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++;
					}
					else
					{
						sqc += 3;
					}
					break;
				case 12:	// suction off & blow on
					if (dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					if (dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					sqc++; break;
				case 14:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 15:
					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					sqc++; break;
				case 16:
					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
                    if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 18:
					if (dwell.Elapsed < delayD2) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					sqc++; break;
				case 19:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PADC1
				case 20:
					Y.moveCompare(lPos.y.PADC1(padY), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.moveCompare(lPos.x.PADC1(padX), Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.bond_done].moveCompare(tPos.t[mc.hd.order.bond_done].ZERO, Z[mc.hd.order.bond_done].config, tPos.z[mc.hd.order.bond_done].XY_MOVING - 2000, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 21:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_TARGET || !Y_AT_TARGET || !Z_AT_TARGET(mc.hd.order.bond_done) || !T_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.bond_done) || !T_AT_DONE(mc.hd.order.bond_done)) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[0] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 1 : {0:F3}", laserResult[0]));
					sqc = 30; break;
				#endregion

				#region case 30 XY.move.PADC2
				case 30:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					Y.move(lPos.y.PADC2(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.PADC2(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 33:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[1] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 2 : {0:F3}", laserResult[1]));
					sqc = 40; break;
				#endregion

				#region case 40 XY.move.PADC3
				case 40:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					Y.move(lPos.y.PADC3(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.PADC3(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 43:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 44:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[2] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 3 : {0:F3}", laserResult[2]));
					sqc = 50; break;
				#endregion

				#region case 50 XY.move.PADC4
				case 50:
					rateY = Y.config.speed.rate; Y.config.speed.rate = Math.Max(rateY * 0.3, 0.1);
					rateX = X.config.speed.rate; X.config.speed.rate = Math.Max(rateX * 0.3, 0.1);
					Y.move(lPos.y.PADC4(padY), out ret.message); Y.config.speed.rate = rateY; if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(lPos.x.PADC4(padX), out ret.message); X.config.speed.rate = rateX; if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 51:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (dwell.Elapsed < 100) break;
					ret.d = mc.AIN.Laser(); if (ret.d < -10) ret.d = -100;
					laserResult[3] = ret.d;
					mc.log.debug.write(mc.log.CODE.INFO, String.Format("Laser Point 4 : {0:F3}", laserResult[3]));
					sqc++; break;
				case 55:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 56:
					laserMaxVal = -1000;
					laserMinVal = 1000;
					for(int i=0; i<4; i++)
					{
						if (laserMaxVal < laserResult[i]) laserMaxVal = laserResult[i];
						if (laserMinVal > laserResult[i]) laserMinVal = laserResult[i];
					}
					mc.log.debug.write(mc.log.CODE.INFO, "Max : " + Math.Round(laserMaxVal, 3).ToString() + ", Min : " + Math.Round(laserMinVal, 3).ToString() + " Tilt : " + Math.Round(Math.Abs(laserMaxVal - laserMinVal) * 1000, 3).ToString() + "[um]");

					if ((laserMaxVal - laserMinVal) * 1000 > mc.para.HD.place.pressTiltLimit.value)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Tilt : {0:F1}[um]", (laserMaxVal - laserMinVal) * 1000);
						//string dispMsg = "Tilt : " + Math.Round((laserMaxVal - laserMinVal) * 1000).ToString() + "[um]";
						errorCheck(ERRORCODE.FULL, sqc, tempSb.ToString(), ALARM_CODE.E_MACHINE_RUN_PRESS_TILT_ERROR);
						break;
					}
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD place_pbi Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pbi Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

        bool pickToPick = false;
        int compareUpAxis = 0;

		bool _pick_move;
		public QueryTimer placeMissCheckTime = new QueryTimer();
		public void place_pick()
		{
			double tmpPos;

			#region PICK_SUCTION_MODE.SEARCH_LEVEL_ON
			if (sqc > 30 && sqc < 40 && mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.SEARCH_LEVEL_ON)
			{
                mc.OUT.HD.SUC(mc.hd.order.pick, out ret.b, out ret.message); ioCheck(sqc, ret.message);
				if (!ret.b)
				{
                    Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message);
					if (ret.d < posZ + mc.para.HD.pick.suction.level.value)
					{
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); ioCheck(sqc, ret.message);
					}
				}
			}
			#endregion

			switch (sqc)
			{
				case 0:
                    pickToPick = false;
					pickupFailDone = false;
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					#region pos set
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 0);
					Z[mc.hd.order.bond_done].commandPosition(out posZ, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
					{
						levelD1 = mc.para.HD.place.driver.level.value;
						delayD1 = mc.para.HD.place.driver.delay.value;
						if (delayD1 == 0) delayD1 = 1;
						velD1 = (mc.para.HD.place.driver.vel.value / 1000);
						accD1 = mc.para.HD.place.driver.acc.value;
					}
					else
					{
						levelD1 = 0;
						delayD1 = 0;
					}
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
					{
						levelD2 = mc.para.HD.place.driver2.level.value;
						delayD2 = mc.para.HD.place.driver2.delay.value;
						velD2 = (mc.para.HD.place.driver2.vel.value / 1000);
						accD2 = mc.para.HD.place.driver2.acc.value;
					}
					else
					{
						levelD2 = 0;
						delayD2 = 0;
					}
					#endregion
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACE2M;
					sqc++; break;
				case 11:
					if (levelD1 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					//if (delayD1 == 0) { sqc += 3; break; }
					if (delayD1 == 0 && mc.para.HD.place.suction.mode.value != (int)PLACE_SUCTION_MODE.PLACE_UP_OFF) { sqc += 5; break; }
					dwell.Reset();
					if(mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
					{
						sqc++; 
					}
					else
					{
						sqc += 3;	
					}
					break;
				case 12:	// suction off & blow on
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if(dwell.Elapsed < mc.para.HD.place.suction.delay.value) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, false, out ret.message);
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, true, out ret.message);
					sqc++; break;
				case 13:	// blow off
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if(dwell.Elapsed < (mc.para.HD.place.suction.delay.value + mc.para.HD.place.suction.purse.value)) break;
                    mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					sqc++; break;
				case 14:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z[mc.hd.order.bond_done].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
                        Z[mc.hd.order.bond_done].checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z[mc.hd.order.bond_done].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Move Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					dwell.Reset();
					sqc++; break;
				case 15:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD1) break;
					if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
					{
                        mc.OUT.HD.BLW(mc.hd.order.bond_done, false, out ret.message);
					}
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 1) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Drive1 Delay Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					sqc++; break;
				case 16:
					//if (UtilityControl.graphEndPoint >= 1) DisplayGraph(4);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (levelD2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.bond_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (delayD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 17:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					#region Z.AT_TARGET
					if (timeCheck(UnitCodeAxis.Z, sqc, 20)) break;
					Z[mc.hd.order.bond_done].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if(ret.b)
					{
                        Z[mc.hd.order.bond_done].checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
						break;
					}
					Z[mc.hd.order.bond_done].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (!ret.b) break;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 18:
					//if (UtilityControl.graphEndPoint >= 2) DisplayGraph(5);
                    // 160704. jhlim
                    DisplayGraph(mc.hd.order.bond_done, useTopLoadcell);

					if (dwell.Elapsed < delayD2) break;
					//if (UtilityControl.graphDisplayEnabled == 1 && graphDispStart && UtilityControl.graphEndPoint >= 2) EVENT.addLoadcellData(1, loadTime.Elapsed, loadForce, sgaugeForce);		// Place Done
                    // 160704. jhlim
                    DisplayGraphPoint(mc.hd.order.bond_done, useTopLoadcell);
					sqc++; break;
				case 19:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					//sqc = 20; 
					pickretrycount = 0;
					mc.log.mcclog.write(mc.log.MCCCODE.Z_AXIS_MOVE_UP, 1);

                    // 160530. jhlim : 여기서 Set 하면 compare 할 수 없다
                    //mc.hd.order.set(mc.hd.order.bond_done, (int)ORDER.NO_DIE);
					sqc = 20;
					break;
				#endregion

				#region case 20 XY.move.PICK
				case 20:
					//double tmpPos;
					//if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					//{
					//    errorCheck(ERRORCODE.SF, sqc, "There is NOT ready feederZ(place->pick)."); break;
					//}
                    //mc.para.runInfo.startCycleTime();
                    
                    // 160530. jhlim : 여기서 하자.
                    if (!pickToPick)
                    {
                        compareUpAxis = mc.hd.order.bond_done;
                        mc.hd.order.set(mc.hd.order.bond_done, (int)ORDER.NO_DIE);
                        mc.para.runInfo.checkCycleTime();
                        mc.para.runInfo.startCycleTime();
                    }
                    else compareUpAxis = mc.hd.order.pick_done;
                    
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 0);

					Y.commandPosition(out ret.d, out ret.message);if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					if (ret.d - tPos.y[mc.hd.order.pick].PAD(0) < 10000) tmpPos = tPos.z[compareUpAxis].XY_MOVING - 2000; else tmpPos = tPos.z[compareUpAxis].XY_MOVING - 3500;	// Place Up-Arc Motion인데, Z-Up되는 시간이 Tight하다..이건 Z축 속도를 굉장히 느리게 했을 경우, Conveyor와 Collision이 발생할 가능성이 있다.
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					if (mc.sf.RUNING)
					{
                        Y.moveCompare(tPos.y[mc.hd.order.pick].WASTE, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        X.moveCompare(tPos.x[mc.hd.order.pick].WASTE, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        T[mc.hd.order.pick].moveCompare(tPos.t[mc.hd.order.pick].ZERO, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
						_pick_move = false;
					}
					else
					{
                        if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                        mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;

                        X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;

                        double targetX = tPos.x[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);
                        double targetY = tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);

                        if (Math.Abs(targetX - ret.d1) > 50000)
                        {
                            X.moveCompare(targetX, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                        else
                        {
                            X.moveCompare(targetX, mc.speed.slow, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        }
                        if (Math.Abs(targetY - ret.d2) > 50000)
                        {
                            Y.moveCompare(targetY, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        }
                        else
                        {
                            Y.moveCompare(targetY, mc.speed.slow, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        }
                        //if (Math.Abs(targetX - ret.d1) > 50000 || Math.Abs(targetY - ret.d2) > 50000)
                        //{
                        //    Y.moveCompare(targetY, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        //    X.moveCompare(targetX, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        //}
                        //else
                        //{
                        //    Y.moveCompare(targetY, mc.speed.slow, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                        //    X.moveCompare(targetX, mc.speed.slow, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        //}
                        T[mc.hd.order.pick].moveCompare(tPos.t[mc.hd.order.pick].ZERO, Z[compareUpAxis].config, tmpPos, true, false, out ret.message); if (mpiCheck(T[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
						_pick_move = true;
					}
                    mc.log.workHistory.write("Compare Axis : " + compareUpAxis);
                    mc.log.workHistory.write("---------------> Start Pick Up(#" + mc.hd.order.pick + ")");

					dwell.Reset();
					sqc++; break;
				case 21:
					if (dwell.Elapsed < 100) break;		// 다행히 100mSec Delay가 존재하기는 한다. 100mSec동안 이동이 가능한 시간은 X,Y는 이미 이동을 시작했을 가능성이 있다. Z축 속도에 대한 Table이 없네..젠장.
					//Derek 수정예정 하기 주석 삭제 
                    //mc.pd.req = true; 
					//mc.pd.reqMode = REQMODE.AUTO;  // 20131022
					if (_pick_move) { sqc += 2; break; }
					sqc++; break;
				case 22:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) mc.hd.pickedPosition = (int)UnitCodeSF.SF1;
                    mc.hd.pickedPosition = (int)mc.sf.workingTubeNumber;
                    //Y.move(tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    //X.move(tPos.x[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;

                    if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.ON)
					{
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
						placeMissCheckTime.Reset();
					}
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET(mc.hd.order.pick)) break;
					if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.ON)
					{
						mc.IN.HD.VAC_CHK(mc.hd.order.pick, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							errorCheck(ERRORCODE.HD, sqc, "Vac Check Time:" + Math.Round(placeMissCheckTime.Elapsed).ToString(), ALARM_CODE.E_HD_PLACE_MISSCHECK);
							break;
						}
						if (mc.para.HD.pick.suction.mode.value != (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
						{
                            mc.OUT.HD.SUC(mc.hd.order.pick, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
						}
					}
					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.MOVING_LEVEL_ON)
					{
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_PICK_POS, 1);
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //mc.hd.tool.F.stackFeedNum = mc.sf.workingTubeNumber;
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICK;
					#region pos set
					if (mc.hd.reqMode == REQMODE.DUMY)
                        posZ = tPos.z[mc.hd.order.pick].DRYRUNPICK(mc.sf.workingTubeNumber);
					else
                        posZ = tPos.z[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber);
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON)
					{
						levelS1 = mc.para.HD.pick.search.level.value;
						delayS1 = mc.para.HD.pick.search.delay.value;

						velS1 = (mc.para.HD.pick.search.vel.value) / 1000;
						accS1 = mc.para.HD.pick.search.acc.value;
					}
					else
					{
						levelS1 = 0;
						delayS1 = 0;
					}
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON)
					{
						levelS2 = mc.para.HD.pick.search2.level.value;
						delayS2 = mc.para.HD.pick.search2.delay.value;
						velS2 = (mc.para.HD.pick.search2.vel.value) / 1000;
						accS2 = mc.para.HD.pick.search2.acc.value;
					}
					else
					{
						levelS2 = 0;
						delayS2 = 0;
					}
					delay = mc.para.HD.pick.delay.value;
					#endregion
					mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 0);
					if (levelS1 != 0)
					{
                        Z[mc.hd.order.pick].moveCompare(posZ + levelS1 + levelS2, -velS1, Y.config, tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                        Z[mc.hd.order.pick].move(posZ + levelS2, velS1, accS1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
						if (delayS1 == 0) { sqc += 3; break; }
					}
					else
					{
                        Z[mc.hd.order.pick].moveCompare(posZ + levelS1 + levelS2, Y.config, tPos.y[mc.hd.order.pick].PICK(mc.sf.workingTubeNumber) + 3000, false, false, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
						sqc += 3; break;
					}
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < delayS1 - 3) break;
					sqc++; break;
				case 33:
					if (levelS2 == 0) { sqc += 3; break; }
					Z[mc.hd.order.pick].move(posZ, velS2, accS2, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					if (levelD2 == 0) { sqc += 3; break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 35:
					if (dwell.Elapsed < delayS2 - 3) break;
					dwell.Reset();
					sqc++; break;
				case 36:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 37:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					if (mc.para.HD.pick.suction.mode.value == (int)PICK_SUCTION_MODE.PICK_LEVEL_ON)
					{
                        mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					}                                                                                                                                                                                                    
					dwell.Reset();
					sqc++; break;
				case 38:
					if (dwell.Elapsed < delay - 3) break;
					sqc = 40; break;
				#endregion

				#region case 40 suction.check
				case 40:
					mc.para.runInfo.writePickInfo(mc.sf.workingTubeNumber, PickCodeInfo.PICK);
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 켜준다.
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, true, out ret.message);
					}
					if (mc.para.HD.pick.suction.check.value == (int)ON_OFF.OFF)
                    {
                        sqc = 50; 
                        break; 
                    }
					dwell.Reset();
					sqc++; break;
				case 41:
					if (dwell.Elapsed > mc.para.HD.pick.suction.checkLimitTime.value)   // 공압 검사 ERROR
					{
						// 여기서 Suction을 OFF하는데, Waste Position으로 움직인 뒤에 Suction OFF해야 한다.
						if (mc.hd.reqMode != REQMODE.AUTO)
						{
                            Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
							errorCheck(ERRORCODE.HD, sqc, "Pick Suction Check Time Limit Error"); break;
						}
						else
						{
							if (mc.para.HD.pick.missCheck.enable.value == (int)ON_OFF.ON)
							{
								if ((pickretrycount+1) < (int)mc.para.HD.pick.missCheck.retry.value)
								{	// retry pick up
									// move to waste position
                                    mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_FAIL);
									sqc = 80;
									mc.sf.req = true;
									pickupFailDone = false;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail. FailCnt[{0}]", pickretrycount + 1));
								}
								else
								{
									// 버린 다음에 알람
                                    mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_FAIL);
									pickupFailDone = true;
									mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail", pickretrycount + 1));
									sqc = 80;
									break;
								}
								pickretrycount++;
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							else
							{
                                mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_FAIL);
								pickupFailDone = true;
                                mc.OUT.HD.SUC(mc.hd.order.pick, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
								mc.log.debug.write(mc.log.CODE.EVENT, String.Format("PickUp Suction Check Fail"));
								sqc = 80; 
								mc.para.runInfo.writePickInfo(PickCodeInfo.AIRERR);
							}
							break;
						}
					}
					mc.IN.HD.VAC_CHK(mc.hd.order.pick, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) break;
					sqc = 50; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.pick)) break;
					sqc++; break;
				case 52:
                    mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_SUCESS);
                    pickToPick = true;
                    if (mc.hd.order.pick != (int)ORDER.EMPTY) 
                    {
                        if (remainCount < mc.activate.headCnt)
                        {
                            mc.hd.order.set(mc.hd.order.pick, (int)ORDER.EMPTY);
                            sqc = SQC.STOP;
                        }
                        else
                        {
                            sqc = 100;
                        }
                        break;
                    }
					sqc = SQC.STOP; break;
				#endregion

				#region case 60, 70 next stack feeder
				case 60:
					pickretrycount = 0;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, mc.speed.slow, out ret.message); //if (mpiCheck(Z.config.axisCode, sqc, ret.message)) break;
					sqc++; break;
				case 61:
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
					}
					if (!mc.sf.nextTubeChange)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
					}
					#region mc.sf.req
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID)
					{
						mc.sf.req = true; mc.sf.reqMode = REQMODE.DOWN;
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG1, true, out ret.message);
						mc.OUT.SF.MG_RESET(UnitCodeSFMG.MG2, true, out ret.message);
						sqc = 70; break;
						//errorCheck(ERRORCODE.SF, sqc, "Stack Feeder Tube Empty"); break;
					}
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;
					mc.sf.req = true;
					#endregion
					sqc++; break;
				case 62:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 20; break;

				case 70:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc++; break;
				case 71:
					if (mc2.req == MC_REQ.STOP) { Esqc = sqc; sqc = SQC.ERROR; break; }     // 20130816
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) break;
					dwell.Reset();
					sqc++; break;
				case 72:
					if (dwell.Elapsed < 2000) break;
					mc.sf.reqTubeNumber = mc.sf.workingTubeNumber;  // 20130816
					mc.sf.req = true;
					sqc++; break;
				case 73:
					if (mc.sf.RUNING) break;
					if (mc.sf.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 20; break;
				#endregion

				#region case 80 move to waste position
				case 80:
					for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!Z_AT_TARGET_ALL()) break;
					if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)
					{
						mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message);
					}
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_2M;
					// 쓰레기통으로 갈 때. 요놈도 문제를 발생할 가능성이 보인다.
					//mc.pd.req = true; mc.pd.reqMode = REQMODE.READY;
					//mc.log.debug.write(mc.log.CODE.TRACE, "PD-Debug(11) : Ready");
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!Z_AT_DONE_ALL()) break;
					X.commandPosition(out ret.d1, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					Y.commandPosition(out ret.d2, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                    if (Math.Abs(tPos.x[mc.hd.order.pick_fail].WASTE - ret.d1) > 50000 || Math.Abs(tPos.y[mc.hd.order.pick_fail].WASTE - ret.d2) > 50000)
					{
                        X.move(tPos.x[mc.hd.order.pick_fail].WASTE, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.pick_fail].WASTE, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					else
					{
                        X.move(tPos.x[mc.hd.order.pick_fail].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                        Y.move(tPos.y[mc.hd.order.pick_fail].WASTE, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					}
					dwell.Reset();
					sqc++; break;
				case 83:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					mc.OUT.HD.SUC(mc.hd.order.pick_fail, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					mc.OUT.HD.BLW(mc.hd.order.pick_fail, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 85:
					if (dwell.Elapsed < Math.Max(mc.para.HD.pick.wasteDelay.value, 15)) break;
					mc.OUT.HD.BLW(mc.hd.order.pick_fail, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break; 
				case 86:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; mc.hd.wastedonestop = true; break; }
					if (pickupFailDone) { pickupFailDone = false; errorCheck(ERRORCODE.HD, sqc, "Pick up 할 때 흡착이 되지 않습니다! Tube의 HeatSlug 기울기, Pick Up Z축 높이 위치를 확인해주세요!"); break; }
					else sqc = 20; break;
				#endregion

                #region case 100, 110, 120 pick to pick
                // Derek Multi Head를 위한 작업
                // Tool안에 pick_pick형식으로 만드려고 했으나... classHeadTool 컨트롤이 겁나 하드코딩!! 
                #region case 100 Z move up
                case 100:
                    //Derek status 확인해서 error 추가필요
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICK2M;
                    sqc++; break;
                case 101:
                    if (levelD1 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.pick_done].move(posZ + levelD1, velD1, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    if (delayD1 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 102:
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 103:
                    if (dwell.Elapsed < delayD1) break;
                    sqc++; break;
                case 104:
                    if (levelD2 == 0) { sqc += 3; break; }
                    Z[mc.hd.order.pick_done].move(posZ + levelD1 + levelD2, velD2, accD2, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    if (delayD2 == 0) { sqc += 3; break; }
                    dwell.Reset();
                    sqc++; break;
                case 105:
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 106:
                    if (dwell.Elapsed < delayD2) break;

                    if (mc.para.SF.useBlow.value == (int)ON_OFF.ON)         // Air Blow 꺼준다.
                    {
                        mc.OUT.SF.TUBE_BLOW(mc.sf.workingTubeNumber, false, out ret.message); { if (ioCheck(sqc, ret.message)) break; }
                    }
                    sqc++; break;
                case 107:

                    if (mc.para.HD.pick.shake.enable.value == (int)ON_OFF.ON)
                    {
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_VIB);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        shakeCount = 0;
                        sqc = 110;
                    }
                    else sqc = 120;
                    break;
                #endregion

                #region case 110 Z Shaking Motion
                // 일단 아래방향으로 떤다.
                // DOUBLE_DET 대신 Blow Position값을 입력해야 할 필요가 생길 수도 있다.
                case 110:
                    Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 111:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 112:
                    if (mc.para.HD.pick.shake.level.value == 0) mc.para.HD.pick.shake.level.value = 1000;
                    if (mc.para.HD.pick.shake.speed.value == 0) mc.para.HD.pick.shake.speed.value = 0.5;
                    if (shakeCount % 2 == 0)
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET - mc.para.HD.pick.shake.level.value, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    }
                    else
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, mc.para.HD.pick.shake.speed.value / 1000, accD1, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case 113:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 114:
                    if (dwell.Elapsed < mc.para.HD.pick.shake.delay.value) break;
                    sqc++; break;
                case 115:
                    shakeCount++;
                    if (shakeCount < mc.para.HD.pick.shake.count.value) sqc = 112;
                    else sqc = 120;
                    break;
                #endregion

                #region case 120 Slug Double Check
                case 120:
                    mc.log.workHistory.write("---------------> End Pick Up(#" + (int)mc.hd.order.pick_done + ")");
                    if (mc.para.HD.pick.doubleCheck.enable.value == (int)ON_OFF.ON)
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].DOUBLE_DET, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_DOUBLE_DET);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        sqc++;
                        dwell.Reset();
                    }
                    else
                    {
                        Z[mc.hd.order.pick_done].move(tPos.z[mc.hd.order.pick_done].XY_MOVING, out ret.message); if (mpiCheck(Z[mc.hd.order.pick_done].config.axisCode, sqc, ret.message)) break;
                        if (mc.hd.cycleMode)
                        {
                            mc.hd.userMessageBox.SetDisplayItems(DIAG_SEL_MODE.NextCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_CYCLE_MOVE_ULC);
                            mc.hd.userMessageBox.ShowDialog();
                            if (FormUserMessage.diagResult == DIAG_RESULT.Cancel) { mc.hd.stepCycleExit = true; sqc = SQC.STOP; break; }
                            mc.hd.stepCycleExit = false;
                        }
                        sqc = 20;
                    }
                    break;
                case 121:
                    if (!Z_AT_TARGET(mc.hd.order.pick_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 122:
                    sqc = 20;
                    break;
                #endregion

                #endregion

				case SQC.ERROR:
					//string str = "HD place_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD place_pick Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}

		public void jig_home_pick()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z.move.XY_MOVING
				case 10:
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                        T[i].move(tPos.t[i].ZERO, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) for_break = true; ;
                    }
                    if (for_break) break;

					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET_ALL() || !T_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL() || !T_AT_DONE_ALL()) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.PICK
				case 20:
					mc.OUT.HD.SUC(mc.hd.order.pick, true, out ret.message); if(ioCheck(sqc, ret.message)) break;
					Y.move(tPos.y[mc.hd.order.pick].JIG_PICK,mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.pick].JIG_PICK,mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PICKJIG;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].REF0 + 100, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					sqc++; break;
				case 33:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 40; break;

				case 40:
					Z[mc.hd.order.pick].movePlus(-50, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					mc.IN.HD.VAC_CHK(mc.hd.order.pick, out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					if (ret.b) { sqc = 50; break; }
					sqc++; break;
				case 42:
					Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                    if (ret.d < tPos.z[mc.hd.order.pick].REF0 - 1000) 
					{
                        Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
						errorCheck(ERRORCODE.HD, sqc, "Jig Pickup Error"); break; 
					}
					sqc -= 2; break;
				#endregion

				#region case 50 XY.AT_DONE
				case 50:
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 500) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (!X_AT_DONE || !Y_AT_DONE || !Z_AT_DONE(mc.hd.order.pick)) break;
                    mc.hd.order.set(mc.hd.order.pick, (int)ORDER.PICK_SUCESS);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_home_pick Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_home_pick Esqc ", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}

		public void jig_pick_ulc()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

                #region case 10 Z move up
                case 10:
                    //mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PICKJIG2M;
                    Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 11:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
                    dwell.Reset();
                    sqc++; break;
                case 12:
                    if (!Z_AT_DONE(mc.hd.order.ulc)) break;
                    sqc++; break;
                case 13:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.log.mcclog.write(mc.log.MCCCODE.PICK_UP_HEAT_SLUG, 1);
                    sqc = 20; break;
                #endregion

				#region case 20 XYZ.move.ULC
				case 20:
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 0);
					Y.move(tPos.y[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.ulc].move(tPos.t[mc.hd.order.ulc].ZERO, out ret.message); if (mpiCheck(T[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.ulc)) break;
					Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].ULC_FOCUS, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					mc.log.mcclog.write(mc.log.MCCCODE.HEAD_MOVE_ULC_POS, 1);
					sqc = 30; break;
				#endregion

				#region case 30 ulc req
				case 30:
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 0);
					#region ULC.req
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.req = true; mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if(dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					mc.log.mcclog.write(mc.log.MCCCODE.SCAN_HEAT_SLUG, 1);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_pick_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_pick_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void jig_move_home()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z.move.XY_MOVING
				case 10:
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                        T[i].move(tPos.t[i].ZERO, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) for_break = true; ;
                    }
                    if (for_break) break;

					dwell.Reset();
					sqc++; break;
				case 11:
                    if (!Z_AT_TARGET_ALL() || !T_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL() || !T_AT_DONE_ALL()) break;
					sqc = 20; break;
				#endregion

				#region case 20 XY.move.JIG_PICK
				case 20:
					Y.move(tPos.y[mc.hd.order.pick].JIG_PICK, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.pick].JIG_PICK, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					sqc = 30; break;
				#endregion

				#region case 30 Z move down
				case 30:
					//F.force_kilogram(1, out ret.message); if (ioCheck(sqc, ret.message)) break;
					Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].REF0 + 100, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 31:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
                    if (!Z_AT_DONE(mc.hd.order.ulc)) break;
					sqc = 50; break;
				#endregion

				#region SUC off
				case 50:
					mc.OUT.HD.SUC(mc.hd.order.ulc, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
					if (dwell.Elapsed < 500) break;
					mc.OUT.HD.BLW(mc.hd.order.ulc, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if (dwell.Elapsed < 30) break;
					mc.OUT.HD.BLW(mc.hd.order.ulc, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (dwell.Elapsed < 500) break;
					sqc = 60; break;
				#endregion

				#region case 60 Z.move.XY_MOVING
				case 60:
					Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 61:
                    if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 62:
                    if (!Z_AT_DONE(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 63:
					if (dwell.Elapsed < 500) break;
					Y.move(tPos.y[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (!X_AT_DONE || !Y_AT_DONE) break;
                    mc.hd.order.set(mc.hd.order.ulc, (int)ORDER.NO_DIE);
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_move_home Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_move_home Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;

			}
		}
		public void jig_ulc_place()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					mc.pd.req = true; 
                    for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;

                    dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL()) break;
					sqc++; break;
				case 13:
					if (mc.pd.RUNING) break;
					if (mc.pd.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //mc.OUT.PD.SUC(false, out ret.message);
                    //mc.OUT.PD.BLW(true, out ret.message);
					dwell.Reset();
					sqc++; break;
				case 14:
					if (dwell.Elapsed < 20) break;
                    //mc.OUT.PD.BLW(false, out ret.message);
					sqc = 20; break;
				#endregion

				#region case 20 XYZ.move.ULC
				case 20:
					Y.move(tPos.y[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.ulc].ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 22:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].ULC_FOCUS, mc.speed.slow, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 24:
					if (!Z_AT_DONE(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 25:
					if (dwell.Elapsed < 100) break;
					sqc = 30; break;
				#endregion

				#region case 30 ulc req
				case 30:
					#region ULC.req
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.req = true; mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 31:
					if (dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 32:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 33:
					if (mc.ulc.RUNING) break;
					if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 40; break;
				#endregion

				#region case 40 Z move up
				case 40:
					Z[mc.hd.order.ulc].move(tPos.z[mc.hd.order.ulc].XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET(mc.hd.order.ulc)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
                    if (!Z_AT_DONE(mc.hd.order.ulc)) break;
                    mc.hd.order.set(mc.hd.order.ulc, (int)ORDER.ULCI_SUCESS);
					sqc = 50; break;
				#endregion

				#region case 50 xy pad move
				case 50:
					placeX = tPos.x[mc.hd.order.bond].PAD(padX);
                    placeY = tPos.y[mc.hd.order.bond].PAD(padY);
					Y.move(placeY,mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX,mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
                    #region ULC result
                    //clacULCX = mc.ulc.cam.rectangleCenter.resultX;
                    //clacULCY = mc.ulc.cam.rectangleCenter.resultY;
                    //clacULCT = mc.ulc.cam.rectangleCenter.resultAngle;
                    //double cosTheta, sinTheta;
                    //cosTheta = Math.Cos(-clacULCT * Math.PI / 180);
                    //sinTheta = Math.Sin(-clacULCT * Math.PI / 180);
                    //clacULCX = (cosTheta * clacULCX) - (sinTheta * clacULCY);
                    //clacULCY = (sinTheta * clacULCX) + (cosTheta * clacULCY);
                    //if (Math.Abs(clacULCX) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC X Compensation Amount Limit Error : " + Math.Round(clacULCX).ToString() + " um"); break; }
                    //if (Math.Abs(clacULCY) > 5000) { errorCheck(ERRORCODE.HD, sqc, "ULC Y Compensation Amount Limit Error : " + Math.Round(clacULCY).ToString() + " um"); break; }
                    //if (Math.Abs(clacULCT) > 30) { errorCheck(ERRORCODE.HD, sqc, "ULC T Compensation Amount Limit Error : " + Math.Round(clacULCT).ToString() + " deg"); break; }
                    ////EVENT.statusDisplay("ULC : " + Math.Round(ulcX, 2).ToString() + "  " + Math.Round(ulcY, 2).ToString() + "  " + Math.Round(ulcT, 2).ToString());
                    //placeX -= clacULCX;
                    //placeY -= clacULCY;
                    //placeT = tPos.t[mc.hd.order.bond].ZERO + clacULCT;
                    #endregion
					Y.move(placeY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.bond].move(placeT, out ret.message); if (mpiCheck(T[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
					if(!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET(mc.hd.order.bond)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if(!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.bond)) break;
					sqc = 60; break;
				#endregion

				#region case 60 z down
				case 60:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_M2PLACEJIG;
					Z[mc.hd.order.bond].move(tPos.z[mc.hd.order.bond].PEDESTAL + 300, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 61:
					if (!Z_AT_TARGET(mc.hd.order.bond)) break;
					dwell.Reset();
					sqc++; break;
				case 62:
					if (!Z_AT_DONE(mc.hd.order.bond)) break;
					mc.OUT.HD.SUC(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 63:
					if (dwell.Elapsed < 500) break;
					mc.OUT.HD.BLW(mc.hd.order.bond, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (dwell.Elapsed < Math.Max(mc.para.HD.place.suction.purse.value, 25)) break;
					mc.OUT.HD.BLW(mc.hd.order.bond, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (dwell.Elapsed < 500) break;
					//mc.OUT.PD.SUC(true, out ret.message);
					sqc++; break;
				case 66:
                    //if (mc.hd.tool.F.RUNING) break;
                    //if (mc.hd.tool.F.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					sqc = 70; break;
				#endregion

				#region case 70 Z move up
				case 70:
					//mc.hd.tool.F.req = true; mc.hd.tool.F.reqMode = REQMODE.F_PLACEJIG2M;
					Z[mc.hd.order.bond].move(tPos.z[mc.hd.order.bond].XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 71:
					if (!Z_AT_TARGET(mc.hd.order.bond)) break;
					dwell.Reset();
					sqc++; break;
				case 72:
					if (!Z_AT_DONE(mc.hd.order.bond)) break;
					sqc++; break;
				case 73:
                    mc.hd.order.set(mc.hd.order.bond, (int)ORDER.BOND_SUCESS);
					sqc = 80; break;
				#endregion

				#region case 80 xy pad c1 move
				case 80:
					Y.move(cPos.y.PAD(padY) + 7500, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.PAD(padX) + 7500, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 81:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 82:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					#region HDC.PADC1.req
					//mc.hdc.lighting_exposure(mc.para.HDC.modelPADC1.light, mc.para.HDC.modelPADC1.exposureTime);
					light_2channel_paramer tmpLight = new light_2channel_paramer();
					para_member tmpExpoure = new para_member();
					tmpLight.ch1.value = 100; tmpLight.ch2.value = 100;
					tmpExpoure.value = 10000;
					mc.hdc.lighting_exposure(tmpLight, tmpExpoure);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
					hdcX = -1;
					hdcY = -1;
					hdcT = -1;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 83:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 84:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 85:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					//hdcP1X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP1Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP1T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP1X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP1Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP1T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;
					sqc = 90; break;
				#endregion

				#region case 90 xy pad c3 move
				case 90:
					Y.move(cPos.y.PAD(padY) - 7500, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(cPos.x.PAD(padX) - 7500, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 91:
					if (!X_AT_TARGET || !Y_AT_TARGET) break;
					dwell.Reset();
					sqc++; break;
				case 92:
					if (!X_AT_DONE || !Y_AT_DONE) break;
					#region HDC.PADC3.req
					//mc.hdc.lighting_exposure(mc.para.HDC.modelPADC3.light, mc.para.HDC.modelPADC3.exposureTime);
					mc.hdc.req = true; mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 93:
					if (dwell.Elapsed < 100) break;
					triggerHDC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 94:
					if (dwell.Elapsed < mc.hdc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerHDC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 95:
					if (mc.hdc.RUNING) break;
					if (mc.hdc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					//hdcP2X = mc.hdc.cam.cornerEdge.resultX;
					//hdcP2Y = mc.hdc.cam.cornerEdge.resultY;
					//hdcP2T = mc.hdc.cam.cornerEdge.resultAngleH;
					hdcP2X = mc.hdc.cam.edgeIntersection.resultX;
					hdcP2Y = mc.hdc.cam.edgeIntersection.resultY;
					hdcP2T_1 = mc.hdc.cam.edgeIntersection.resultAngleH;

					hdcX = (hdcP1X + hdcP2X) / 2;
					hdcY = (hdcP1Y + hdcP2Y) / 2;
					hdcT = (hdcP1T_1 + hdcP2T_1) / 2;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_ulc_place Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_ulc_place Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		public void jig_place_ulc()
		{
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc = 10; break;

				#region case 10 Z move up
				case 10:
					 for_break = false;
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        Z[i].move(tPos.z[i].XY_MOVING, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) for_break = true;
                    }
                    if (for_break) break;
					dwell.Reset();
					sqc++; break;
				case 11:
					if (!Z_AT_TARGET_ALL()) break;
					dwell.Reset();
					sqc++; break;
				case 12:
                    if (!Z_AT_DONE_ALL()) break;
					sqc = 20; break;
				#endregion

				#region case 20 xy pad move
				case 20:
                    //??
					//placeX = tPos.x.PAD(padX);
					//placeY = tPos.y.PAD(padY);
					//placeX -= ulcX;
					//placeY -= ulcY;
					//placeT = tPos.t.ZERO + ulcT;
					Y.move(placeY, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(placeX, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 21:
					if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 22:
                    if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.bond_done)) break;
					//mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 23:
					if (dwell.Elapsed < 500) break;
					sqc = 30; break;
				#endregion
                     
				#region case 30 z down
				case 30:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].PEDESTAL + 300, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 31:
                    if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
                    dwell.Reset();
                    sqc++; break;
                case 32:
                    if (!Z_AT_DONE(mc.hd.order.bond_done)) break;
                    mc.OUT.HD.SUC(mc.hd.order.bond_done, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case 33:
                    if (dwell.Elapsed < 500) break;
                    mc.IN.HD.VAC_CHK((int)UnitCodeHead.HD1, out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
					if (!ret.b) { errorCheck(ERRORCODE.HD, sqc, "jig re-suction error"); break; }
					dwell.Reset();
					sqc++; break;
				case 34:
					if (dwell.Elapsed < 100) break;
					sqc = 40; break;
				#endregion

				#region case 40 Z move up
				case 40:
					Z[mc.hd.order.bond_done].move(tPos.z[mc.hd.order.bond_done].XY_MOVING, mc.speed.homing, out ret.message); if (mpiCheck(Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 41:
					if (!Z_AT_TARGET(mc.hd.order.bond_done)) break;
					dwell.Reset();
					sqc++; break;
				case 42:
					if (!Z_AT_DONE(mc.hd.order.bond_done)) break;
                    mc.hd.order.set(mc.hd.order.bond_done, (int)ORDER.NO_DIE);
					sqc = 50; break;
				#endregion

				#region case 50 XYZ.move.ULC
				case 50:
					Y.move(tPos.y[mc.hd.order.pick].ULC, mc.speed.slow, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
					X.move(tPos.x[mc.hd.order.pick].ULC, mc.speed.slow, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
					T[mc.hd.order.pick].move(tPos.t[mc.hd.order.pick].ZERO, out ret.message); if (mpiCheck(T[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 51:
                    if (!X_AT_TARGET || !Y_AT_TARGET || !T_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 52:
                    if (!X_AT_DONE || !Y_AT_DONE || !T_AT_DONE(mc.hd.order.pick)) break;
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].ULC_FOCUS, mc.speed.slow, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 53:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 54:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 55:
					if (dwell.Elapsed < 100) break;
					sqc = 60; break;
				#endregion

				#region case 60 ulc req
				case 60:
					#region ULC.req
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.req = true;
					#endregion
					dwell.Reset();
					sqc++; break;
				case 61:
					if (dwell.Elapsed < 100) break;
					triggerULC.output(true, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 62:
					if (dwell.Elapsed < mc.ulc.cam.acq.ExposureTimeAbs * 0.001 + 2) break;
					triggerULC.output(false, out ret.message); if (mpiCheck(sqc, ret.message)) break;
					sqc++; break;
				case 63:
					if (mc.ulc.RUNING) break;
					if (mc.ulc.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
					Z[mc.hd.order.pick].move(tPos.z[mc.hd.order.pick].XY_MOVING, mc.speed.slow, out ret.message); if (mpiCheck(Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					dwell.Reset();
					sqc++; break;
				case 64:
					if (!Z_AT_TARGET(mc.hd.order.pick)) break;
					dwell.Reset();
					sqc++; break;
				case 65:
					if (!Z_AT_DONE(mc.hd.order.pick)) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//string str = "HD jig_place_ulc Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD jig_place_ulc Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					sqc = SQC.END; break;


			}
		}
		#endregion

		#region AT_TARGET , AT_DONE
		bool X_AT_TARGET
		{
			get
			{
				X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					X.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				X.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 50000)
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 20);
					return false;
				}
				X.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 50000) // 장비 내 모든 축 이동이 20초안에 된다보고 넘어갓는지 
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool X_AT_DONE
		{
			get
			{
				X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					X.checkAlarmStatus(out ret.s, out ret.message);
					errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				X.AT_DONE(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500) // 멈췄는지만 판단하니까 0.5초에도 가능 
					{
						X.checkAlarmStatus(out ret.s, out ret.message);
						errorCheck((int)UnitCodeAxisNumber.HD_X, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.X, sqc, 0.5);
					return false;
				}
				return true;
			}
		}

		bool Y_AT_TARGET
		{
			get
			{
				Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if(ret.b)
				{
					Y.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
					return false;
				}
				Y.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					if (dwell.Elapsed > 50000)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 20);
					return false;
				}
				Y.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 50000)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 20);
					return false;
				}
				return true;
			}
		}
		bool Y_AT_DONE
		{
			get
			{
				Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (ret.b)
				{
					Y.checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
					return false;
				}
				Y.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
				if (!ret.b)
				{
					if (dwell.Elapsed > 500)
					{
						Y.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Y1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
					}
					//timeCheck(UnitCodeAxis.Y, sqc, 0.5);
					return false;
				}
				return true;
			}
		}

        bool Z_AT_MOVING_DONE(int headNum)
        {
            Z[headNum].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                return false;
            }
            Z[headNum].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                if (dwell.Elapsed > 50000)
                {
                    Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.Z, sqc, 20);
                return false;
            }
            return true;

        }
        bool Z_AT_TARGET(int headNum)
        {
            Z[headNum].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                return false;
            }
            Z[headNum].AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                if (dwell.Elapsed > 50000)	// 20000
                {
                    Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.Z, sqc, 20);
                return false;
            }
            Z[headNum].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (!ret.b)
            {
                if (dwell.Elapsed > 50000)	// 20000
                {
                    Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.Z, sqc, 20);
                return false;
            }
            return true;
        }
        bool Z_AT_DONE(int headNum)
        {
            Z[headNum].AT_ERROR(out ret.b, out ret.message);
            if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                return false;
            }
            Z[headNum].AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z[headNum].config.axisCode, sqc, ret.message))
                return false;
            if (!ret.b)
            {
                if (dwell.Elapsed > 500)
                {
                    Z[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.Z, sqc, 0.5);
                return false;
            }
            return true;
        }
        bool Z_AT_MOVING_DONE_ALL()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z[i].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    return false;
                }
                Z[i].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 50000)
                    {
                        Z[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 20);
                    return false;
                }
            }
            return true;
        }
        bool Z_AT_TARGET_ALL()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z[i].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    return false;
                }
                Z[i].AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 50000)	// 20000
                    {
                        Z[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 20);
                    return false;
                }
                Z[i].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 50000)	// 20000
                    {
                        Z[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 20);
                    return false;
                }
            }
            return true;
        }
        bool Z_AT_DONE_ALL()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                Z[i].AT_ERROR(out ret.b, out ret.message);
                if (mpiCheck(Z[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    Z[i].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    return false;
                }
                Z[i].AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z[i].config.axisCode, sqc, ret.message))
                    return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 500)
                    {
                        Z[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_Z1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 0.5);
                    return false;
                }
            }
            return true;
        }

        bool T_AT_TARGET(int headNum)
        {
            T[headNum].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(T[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                T[headNum].checkAlarmStatus(out ret.s, out ret.message);
                errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                return false;
            }
            T[headNum].AT_MOVING(out ret.b, out ret.message); if (mpiCheck(T[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                if (dwell.Elapsed > 50000)
                {
                    T[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.T, sqc, 20);
                return false;
            }
            T[headNum].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(T[headNum].config.axisCode, sqc, ret.message)) return false;
            if (!ret.b)
            {
                if (dwell.Elapsed > 50000)
                {
                    T[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.T, sqc, 20);
                return false;
            }
            return true;
        }
        bool T_AT_DONE(int headNum)
        {
            T[headNum].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(T[headNum].config.axisCode, sqc, ret.message)) return false;
            if (ret.b)
            {
                T[headNum].checkAlarmStatus(out ret.s, out ret.message);
                errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                return false;
            }
            T[headNum].AT_DONE(out ret.b, out ret.message); if (mpiCheck(T[headNum].config.axisCode, sqc, ret.message)) return false;
            if (!ret.b)
            {
                if (dwell.Elapsed > 500)
                {
                    T[headNum].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                }
                //timeCheck(UnitCodeAxis.T, sqc, 0.5);
                return false;
            }
            return true;
        }
        bool T_AT_TARGET_ALL()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                T[i].AT_ERROR(out ret.b, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    T[i].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    return false;
                }
                T[i].AT_MOVING(out ret.b, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 50000)	// 20000
                    {
                        T[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 20);
                    return false;
                }
                T[i].AT_TARGET(out ret.b, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 50000)	// 20000
                    {
                        T[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 20);
                    return false;
                }
            }
            return true;
        }
        bool T_AT_DONE_ALL()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                T[i].AT_ERROR(out ret.b, out ret.message);
                if (mpiCheck(T[i].config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    T[i].checkAlarmStatus(out ret.s, out ret.message);
                    errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    return false;
                }
                T[i].AT_DONE(out ret.b, out ret.message); if (mpiCheck(T[i].config.axisCode, sqc, ret.message))
                    return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 500)
                    {
                        T[i].checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck((int)UnitCodeAxisNumber.HD_T1, ERRORCODE.HD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    //timeCheck(UnitCodeAxis.Z, sqc, 0.5);
                    return false;
                }
            }
            return true;
        }
		#endregion

        #region muliti runing check
        public bool mRUNING(ref captureHoming[] homing)
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                if (homing[i].RUNING) return true;
            }
            return false;
        }
        public bool mERROR(ref captureHoming[] homing)
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                if (homing[i].ERROR) return true;
            }
            return false;
        }
        #endregion
    }
	public class classForce : CONTROL
	{
		public UnitCodeSF stackFeedNum = 0;

        //public void kilogram(double kg, out RetMessage retMessage, bool useTopLoadcell = false)
        //{
        //    double voltage;
        //    if (useTopLoadcell)
        //    {
        //        kilogram2sgVoltage(kg, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
        //    }
        //    else
        //    {
        //        kilogram2voltage(kg, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
        //    }
        //    if (voltage > UtilityControl.forceMaxPressVoltage) voltage = UtilityControl.forceMaxPressVoltage;
        //    mc.AOUT.VPPM(voltage, out retMessage);
        //}

        //public void kilogram(para_member kg, out RetMessage retMessage, bool useToploadcell = false)
        //{
        //    double voltage;
        //    if (useToploadcell)
        //    {
        //        kilogram2sgVoltage(kg.value, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
        //    }
        //    else
        //    {
        //        kilogram2voltage(kg.value, out voltage, out retMessage); if (retMessage != RetMessage.OK) return;
        //    }
        //    if (voltage > UtilityControl.forceMaxPressVoltage) voltage = UtilityControl.forceMaxPressVoltage;
        //    if (kg.value == 0) mc.AOUT.VPPM(0, out retMessage);
        //    else mc.AOUT.VPPM(voltage, out retMessage);
        //}

        //public void voltage(double volt, out RetMessage retMessage)
        //{
        //    mc.AOUT.VPPM(volt, out retMessage);
        //}

        public double kilogram2Height(int head, double kg, out RetMessage retMessage, bool useHeadLoadcell = true)
        {
            //double a, b;
            double value = 0;
            double a, b;

            #region Use Bottom Loadcell
            if (useHeadLoadcell)
            {
                if (head == (int)UnitCodeHead.HD2)
                {
                    for (int i = 0; i < 19; i++)
                    {
                        if (kg >= mc.para.CAL.Tool_Force2.forceLevel[i].value
                            && kg < mc.para.CAL.Tool_Force2.forceLevel[i + 1].value)
                        {
                            a = (mc.para.CAL.Tool_Force2.forceLevel[i + 1].value - mc.para.CAL.Tool_Force2.forceLevel[i].value) / (mc.para.CAL.Tool_Force2.heightLevel[i + 1].value - mc.para.CAL.Tool_Force2.heightLevel[i].value);
                            b = mc.para.CAL.Tool_Force2.forceLevel[i].value - (a * mc.para.CAL.Tool_Force2.heightLevel[i].value);
                            goto SUCCESS;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 19; i++)
                    {
                        if (kg >= mc.para.CAL.Tool_Force1.forceLevel[i].value
                            && kg < mc.para.CAL.Tool_Force1.forceLevel[i + 1].value)
                        {
                            a = (mc.para.CAL.Tool_Force1.forceLevel[i + 1].value - mc.para.CAL.Tool_Force1.forceLevel[i].value) / (mc.para.CAL.Tool_Force1.heightLevel[i + 1].value - mc.para.CAL.Tool_Force1.heightLevel[i].value);
                            b = mc.para.CAL.Tool_Force1.forceLevel[i].value - (a * mc.para.CAL.Tool_Force1.heightLevel[i].value);
                            goto SUCCESS;
                        }
                    }
                }
            }
            #endregion
            #region Use Bottom Loadcell
            else
            {
                if (head == (int)UnitCodeHead.HD2)
                {
                    for (int i = 0; i < 19; i++)
                    {
                        if (kg >= mc.para.CAL.Tool_Force2.topForce[i].value
                            && kg < mc.para.CAL.Tool_Force2.topForce[i + 1].value)
                        {
                            a = (mc.para.CAL.Tool_Force2.topForce[i + 1].value - mc.para.CAL.Tool_Force2.topForce[i].value) / (mc.para.CAL.Tool_Force2.heightLevel[i + 1].value - mc.para.CAL.Tool_Force2.heightLevel[i].value);
                            b = mc.para.CAL.Tool_Force2.topForce[i].value - (a * mc.para.CAL.Tool_Force2.heightLevel[i].value);
                            goto SUCCESS;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 19; i++)
                    {
                        if (kg >= mc.para.CAL.Tool_Force1.topForce[i].value
                            && kg < mc.para.CAL.Tool_Force1.topForce[i + 1].value)
                        {
                            a = (mc.para.CAL.Tool_Force1.topForce[i + 1].value - mc.para.CAL.Tool_Force1.topForce[i].value) / (mc.para.CAL.Tool_Force1.heightLevel[i + 1].value - mc.para.CAL.Tool_Force1.heightLevel[i].value);
                            b = mc.para.CAL.Tool_Force1.topForce[i].value - (a * mc.para.CAL.Tool_Force1.heightLevel[i].value);
                            goto SUCCESS;
                        }
                    }
                }
            }
            #endregion

            goto FAIL;

        SUCCESS:
            value = (kg - b) / a;
            retMessage = RetMessage.OK;
            return value;

        FAIL:
            //volt = -1;
            retMessage = RetMessage.PARAM_INVALID;
            return -1;
        }

        public double TopLDToBottomLD(int head, double kg, out RetMessage retMessage)
        {
            double value = 0;
            double a, b;

            if (head == (int)UnitCodeHead.HD2)
            {
                for (int i = 0; i < 19; i++)
                {
                    if (kg >= mc.para.CAL.Tool_Force2.topForce[i].value
                        && kg < mc.para.CAL.Tool_Force2.topForce[i + 1].value)
                    {
                        a = (mc.para.CAL.Tool_Force2.topForce[i + 1].value - mc.para.CAL.Tool_Force2.topForce[i].value) / (mc.para.CAL.Tool_Force2.bottomForce[i + 1].value - mc.para.CAL.Tool_Force2.bottomForce[i].value);
                        b = mc.para.CAL.Tool_Force2.topForce[i].value - (a * mc.para.CAL.Tool_Force2.bottomForce[i].value);
                        goto SUCCESS;
                    }
                    //if (i == 18)
                    //{
                    //    a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
                    //    b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
                    //    goto SUCCESS;
                    //}
                }
            }
            else
            {
                for (int i = 0; i < 19; i++)
                {
                    if (kg >= mc.para.CAL.Tool_Force1.topForce[i].value
                        && kg < mc.para.CAL.Tool_Force1.topForce[i + 1].value)
                    {
                        a = (mc.para.CAL.Tool_Force1.topForce[i + 1].value - mc.para.CAL.Tool_Force1.topForce[i].value) / (mc.para.CAL.Tool_Force1.bottomForce[i + 1].value - mc.para.CAL.Tool_Force1.bottomForce[i].value);
                        b = mc.para.CAL.Tool_Force1.topForce[i].value - (a * mc.para.CAL.Tool_Force1.bottomForce[i].value);
                        goto SUCCESS;
                    }
                    //if (i == 18)
                    //{
                    //    a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
                    //    b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
                    //    goto SUCCESS;
                    //}
                }
            }
            goto FAIL;

        SUCCESS:
            value = (kg - b) / a;
            retMessage = RetMessage.OK;
            return value;

        FAIL:
            //volt = -1;
            retMessage = RetMessage.PARAM_INVALID;
            return -1;
        }

        //public void kilogram2voltage(double kg, out double volt, out RetMessage retMessage)
        //{
        //    double a, b;

        //    for (int i = 0; i < 19; i++)
        //    {
        //        if (kg <= mc.para.CAL.force.B[i + 1].value)
        //        {
        //            a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
        //            b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
        //            goto SUCCESS;
        //        }
        //        if (i == 18)
        //        {
        //            a = (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
        //            b = mc.para.CAL.force.B[i].value - (a * mc.para.CAL.force.A[i].value);
        //            goto SUCCESS;
        //        }
        //    }
        //    goto FAIL;

        //SUCCESS:
        //    volt = (kg - b) / a;
        //    if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
        //    if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
        //    retMessage = RetMessage.OK;
        //    return;

        //FAIL:
        //    volt = -1;
        //    retMessage = RetMessage.PARAM_INVALID;
        //    return;
        //}

        //// top loadcell값을 기준으로 voltage를 생성하는 함수
        //public void kilogram2sgVoltage(double kg, out double volt, out RetMessage retMessage)
        //{
        //    double a, b;

        //    for (int i = 0; i < 19; i++)
        //    {
        //        if (kg <= mc.para.CAL.force.D[i + 1].value)
        //        {
        //            a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
        //            b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.A[i].value);
        //            goto SUCCESS;
        //        }
        //        if (i == 18)
        //        {
        //            a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.A[i + 1].value - mc.para.CAL.force.A[i].value);
        //            b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.A[i].value);
        //            goto SUCCESS;
        //        }
        //    }
        //    goto FAIL;

        //SUCCESS:
        //    volt = (kg - b) / a;
        //    if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
        //    if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
        //    retMessage = RetMessage.OK;
        //    return;

        //FAIL:
        //    volt = -1;
        //    retMessage = RetMessage.PARAM_INVALID;
        //    return;
        //}

        //// vppm voltage를 기준으로 kilogram 산출
        //public void voltage2kilogram(double volt, out double kg, out RetMessage retMessage)
        //{
        //    double a, b;

        //    for (int i = 0; i < 19; i++)
        //    {
        //        if (volt <= mc.para.CAL.force.C[i + 1].value)
        //        {
        //            a = (mc.para.CAL.force.C[i + 1].value - mc.para.CAL.force.C[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
        //            b = mc.para.CAL.force.C[i].value - (a * mc.para.CAL.force.B[i].value);
        //            goto SUCCESS;
        //        }
        //        if (i == 18)
        //        {
        //            a = (mc.para.CAL.force.C[i + 1].value - mc.para.CAL.force.C[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
        //            b = mc.para.CAL.force.C[i].value - (a * mc.para.CAL.force.B[i].value);
        //            goto SUCCESS;
        //        }
        //    }
        //    goto FAIL;

        //SUCCESS:
        //    kg = (volt - b) / a;
        //    //if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
        //    //if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
        //    retMessage = RetMessage.OK;
        //    return;

        //FAIL:
        //    kg = -1;
        //    retMessage = RetMessage.PARAM_INVALID;
        //    return;
        //}
        //// top loadcell 전압을 kilogram으로 산출...전압과 indicator force는 동일...
        //public void sgVoltage2kilogram(double volt, out double kg, out RetMessage retMessage)
        //{
        //    if ((mc.swcontrol.mechanicalRevision & 0x01) == 0)
        //    {
        //        kg = -1;
        //        retMessage = RetMessage.OK;
        //        return;
        //    }
        //    double a, b;

        //    for (int i = 0; i < 19; i++)
        //    {
        //        if (volt <= mc.para.CAL.force.D[i + 1].value)
        //        {
        //            a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
        //            b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.B[i].value);
        //            goto SUCCESS;
        //        }
        //        if (i == 18)
        //        {
        //            a = (mc.para.CAL.force.D[i + 1].value - mc.para.CAL.force.D[i].value) / (mc.para.CAL.force.B[i + 1].value - mc.para.CAL.force.B[i].value);
        //            b = mc.para.CAL.force.D[i].value - (a * mc.para.CAL.force.B[i].value);
        //            goto SUCCESS;
        //        }
        //    }
        //    goto FAIL;

        //SUCCESS:
        //    kg = (volt - b) / a;
        //    //if (volt < MIN_VOLTAGE) volt = MIN_VOLTAGE;
        //    //if (volt > MAX_VOLTAGE) volt = MAX_VOLTAGE;
        //    retMessage = RetMessage.OK;
        //    return;

        //FAIL:
        //    kg = -1;
        //    retMessage = RetMessage.PARAM_INVALID;
        //    return;
        //}

		double pos, srch, srch2, drive, drive2, forceChangePos;
		bool forceChangeFlag;
		UnitCodeSF tubeNum;
		bool fdCheck;
		double srch1pos, srch2pos;
		//string errString;
		StringBuilder tempSb = new StringBuilder();

		public void control()
		{
			if (!req) return;
		   
			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (reqMode == REQMODE.F_2M) { sqc = SQC.F_2M; break; }
					if (reqMode == REQMODE.F_M2PICK) { fdCheck = false; dwell.Reset(); sqc = SQC.F_M2PICK; break; }
					if (reqMode == REQMODE.F_PICK2M) { sqc = SQC.F_PICK2M; break; }
					if (reqMode == REQMODE.F_M2PLACE) { sqc = SQC.F_M2PLACE; break; }
					if (reqMode == REQMODE.F_M2PLACEREV) { sqc = SQC.F_M2PLACEREV; break; }
					if (reqMode == REQMODE.F_PLACE2M) { sqc = SQC.F_PLACE2M; break; }
					if (reqMode == REQMODE.F_M2PICKJIG) { sqc = SQC.F_M2PICKJIG; break; }
					if (reqMode == REQMODE.F_PICKJIG2M) { sqc = SQC.F_PICKJIG2M; break; }
					if (reqMode == REQMODE.F_M2PLACEJIG) { sqc = SQC.F_M2PLACEJIG; break; }
					if (reqMode == REQMODE.F_PLACEJIG2M) { sqc = SQC.F_PLACEJIG2M; break; }
					errorCheck(ERRORCODE.HD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_FORCE_LIST_NONE); break;

				#region case SQC.F_2M:
				case SQC.F_2M:
					//kilogram(mc.para.HD.moving_force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PICK
				case SQC.F_M2PICK:
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
						;
					}
					else
					{
						if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID && dwell.Elapsed < 20000)
						{
							if(fdCheck==false)
							{
								mc.log.debug.write(mc.log.CODE.ETC,"Wait for changing stack feeder number to correct one.");
								fdCheck = true;
							}
							break;
						}
						if (fdCheck == true)
						{
							mc.log.debug.write(mc.log.CODE.ETC, "Waited time for changing SF number : " + Math.Round(dwell.Elapsed).ToString());
						}
					}
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) tubeNum = UnitCodeSF.SF1;
					else
					{
						tubeNum = mc.sf.workingTubeNumber;
					}
					if (tubeNum != stackFeedNum && mc.hd.reqMode != REQMODE.DUMY)
					{
						mc.log.debug.write(mc.log.CODE.TRACE, "[Force] Invalid Feeder Num - Set:" + tubeNum.ToString() + ", Cur:" + stackFeedNum.ToString());
						tubeNum = stackFeedNum;
					}
					if (mc.hd.reqMode == REQMODE.DUMY)
                        pos = mc.hd.tool.tPos.z[mc.hd.order.pick].DRYRUNPICK(tubeNum) + 10;
					else
						pos = mc.hd.tool.tPos.z[mc.hd.order.pick].PICK(tubeNum) + 10;  // 살짝 뜬 위치..
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + mc.para.HD.pick.search2.level.value; else srch2 = pos;
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.pick.search.level.value; else srch = srch2;
					if (mc.para.HD.pick.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//errString = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_1ST_SEARCH_TIMEOUT);
						//mc.hd.tool.Z.actualPosition(out ret.d1, out ret.message); if (mpiCheck(sqc, ret.message)) break;
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Search Position Incorrect - Feed: " + tubeNum.ToString() + ", Cmd: " + Math.Round(ret.d).ToString() + ", Sch: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Force Cal Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Target: " + ret.d.ToString() + ", Cur Pos: " + Math.Round(ret.d1).ToString() + ", Search2: " + srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch) break;
					//kilogram(mc.para.HD.pick.search.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					srch1pos = ret.d;
					sqc++; break;
				case SQC.F_M2PICK + 2:
					if (mc.para.HD.pick.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//errString = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_SEARCH_TIMEOUT);
						//mc.hd.tool.Z.actualPosition(out ret.d1, out ret.message); if (mpiCheck(sqc, ret.message)) break;
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Search2 Position Incorrect - Feed: " + tubeNum.ToString() + ", Cmd: " + Math.Round(ret.d).ToString() + "< Sch2: " + srch2.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Force Cal Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Pos: " + ret.d.ToString() + ", Cur Pos: " + Math.Round(ret.d1).ToString() + ", Search2: " + srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					//kilogram(mc.para.HD.pick.search2.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					srch2pos = ret.d;
					sqc++; break;
				case SQC.F_M2PICK + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICK + 5:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5))
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//errString = "Target[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_SEARCH_TIMEOUT);
						//mc.log.debug.write(mc.log.CODE.ERROR, "Pick Target Position Incorrect- Cmd:" + Math.Round(ret.d).ToString() + ", Tgt:" + pos.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Target: " + pos.ToString() + ", Search2: " + srch2.ToString() + ", Search: " + srch.ToString());
						//mc.log.debug.write(mc.log.CODE.TRACE, "Cmd Target: " + ret.d.ToString() + ", Search2: " +srch2pos.ToString() + ", Search: " + srch1pos.ToString());
						break;
					}
					mc.hd.tool.Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); if(mpiCheck(mc.hd.tool.Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos) break;
					//kilogram(mc.para.HD.pick.force, out ret.message); if(ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PICK2M
				case SQC.F_PICK2M:
					if (mc.sf.workingTubeNumber == UnitCodeSF.INVALID) tubeNum = UnitCodeSF.SF1;
					else tubeNum = mc.sf.workingTubeNumber;
					if (mc.hd.reqMode == REQMODE.DUMY)
					{
                        if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z[mc.hd.order.ulc].DRYRUNPICK(tubeNum) + mc.para.HD.pick.driver.level.value; else drive = mc.hd.tool.tPos.z[mc.hd.order.ulc].DRYRUNPICK(tubeNum);
					}
					else
					{
                        if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z[mc.hd.order.ulc].PICK(tubeNum) + mc.para.HD.pick.driver.level.value; else drive = mc.hd.tool.tPos.z[mc.hd.order.ulc].PICK(tubeNum);
					}
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.ON) drive2 = drive + mc.para.HD.pick.driver2.level.value; else drive2 = drive;
					pos = mc.hd.tool.tPos.z[mc.hd.order.ulc].XY_MOVING - 3000;//drive2 + 300;
					if (drive2 >= pos)
					{
						mc.log.debug.write(mc.log.CODE.INFO, "Pick Driver Pos is BIGGER than Target Pos - " + drive2.ToString() + ":" + pos.ToString());
					}
					if (mc.para.HD.pick.driver.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 1:
					if (!mc.hd.cycleMode && dwell.Elapsed > 10000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive[{0}],Command[{1}]", drive, Math.Abs(ret.d));
						//errString = "Drive[" + drive.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_1ST_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.ulc].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive - 2)) break;
					//kilogram(mc.para.HD.pick.driver.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PICK2M + 2:
					if (mc.para.HD.pick.driver2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 3:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive2[{0}],Command[{1}]", drive2, Math.Abs(ret.d));
						//errString = "Drive2[" + drive2.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_2ND_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.ulc].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive2 - 2)) break;
					//kilogram(mc.para.HD.pick.driver2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PICK2M + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_PICK2M + 5:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//errString = "Target[" + pos.ToString() + "],Command[" + ret.d.ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PICK_TARGET_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.ulc].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (pos - 2)) break;
					//kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACE
				case SQC.F_M2PLACE:
					//if (mc.hd.reqMode == REQMODE.DUMY && mc.para.ETC.placeTimeSensorCheckUse.value == (int)ON_OFF.ON) pos = mc.hd.tool.tPos.z.DRYRUNPLACE + 10;
					//else pos = mc.hd.tool.tPos.z.PLACE + 10;
					pos = mc.hd.tool.forceTargetZPos + 10;
					
					forceChangeFlag = false;
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + (mc.para.HD.place.search2.level.value - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value); else srch2 = pos;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.place.search.level.value; else srch = srch2;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//string str = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.LOW_HIGH_MODE || mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.HIGH_LOW_MODE)
					{
						if (ret.d < forceChangePos && forceChangeFlag==false)
						{
							//mc.log.debug.write(mc.log.CODE.ETC, "Change Force:" + Math.Round(dwell .Elapsed).ToString());
                            //kilogram(mc.para.HD.place.forceMode.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
							forceChangeFlag = true;
						}
					}
					if (ret.d > srch) break;
					//kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search1 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACE + 2:
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 10000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//string str = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					//kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search2 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search2.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACE + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACE + 5:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (!mc.hd.cycleMode && dwell.Elapsed > 25000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//string str = "TARGET[" + pos.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_TARGET_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos) break;
				//    sqc++; break;
				//case SQC.F_M2PLACE + 6:

					//kilogram(mc.para.HD.place.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Place Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.force.value.ToString());
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACEREV
				case SQC.F_M2PLACEREV:
					pos = mc.hd.tool.forceTargetZPos + 10;
					//forceChangePos = pos + mc.para.HD.place.forceMode.level.value;
					forceChangeFlag = false;
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON) srch2 = pos + mc.para.HD.place.search2.level.value; else srch2 = pos;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON) srch = srch2 + mc.para.HD.place.search.level.value; else srch = srch2;
					if (mc.para.HD.place.search.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEREV + 1:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)		// Force Timeout 때문에 mc.hd.cycleMode 상태 확인함. cyclemode 일 때는 무시
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search[{0}],Command[{1}]", srch, Math.Abs(ret.d));
						//string str = "Search[" + srch.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d < forceChangePos && forceChangeFlag == false)
					{
						//mc.log.debug.write(mc.log.CODE.ETC, "Change Force:" + Math.Round(dwell.Elapsed).ToString());
						//kilogram(mc.para.HD.place.forceMode.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
						forceChangeFlag = true;
					}
					if (ret.d > srch) break;
					//kilogram(mc.para.HD.place.search.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search1 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search.force.value.ToString());
					sqc++; break;
				case SQC.F_M2PLACEREV + 2:
					if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEREV + 3:
					if (!mc.hd.cycleMode && dwell.Elapsed > 5000)		// Force Timeout 때문에 mc.hd.cycleMode 상태 확인함. cyclemode 일 때는 무시
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Search2[{0}],Command[{1}]", srch2, Math.Abs(ret.d));
						//string str = "Search2[" + srch2.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_SEARCH_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d > srch2) break;
					//kilogram(mc.para.HD.place.search2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					//mc.log.debug.write(mc.log.CODE.ETC, "Search2 Done:" + Math.Round(dwell.Elapsed).ToString() + ", Force:" + mc.para.HD.place.search2.force.value.ToString());
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PLACE2M
				case SQC.F_PLACE2M:
                    if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON) drive = mc.hd.tool.tPos.z[mc.hd.order.bond_done].PLACE + mc.para.HD.place.driver.level.value; else drive = mc.hd.tool.tPos.z[mc.hd.order.bond_done].PLACE;
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON) drive2 = drive + mc.para.HD.place.driver2.level.value; else drive2 = drive;
					pos = mc.hd.tool.tPos.z[mc.hd.order.bond_done].XY_MOVING - 3000;//drive2 + 300;
					//pos = drive2 + 300;
					if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 1:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive[{0}],Command[{1}]", drive, Math.Abs(ret.d));
						//string str = "Drive[" + drive.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_1ST_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond_done].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive - 2)) break;
					//kilogram(mc.para.HD.place.driver.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PLACE2M + 2:
					if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.OFF) { sqc += 2; break; }
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 3:
					//if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Drive2[{0}],Command[{1}]", drive2, Math.Abs(ret.d));
						//string str = "Drive2[" + drive.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_2ND_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond_done].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (drive2 - 2)) break;
					//kilogram(mc.para.HD.place.driver2.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc++; break;
				case SQC.F_PLACE2M + 4:
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACE2M + 5:
					if (dwell.Elapsed > 5000)
					{
						tempSb.Clear(); tempSb.Length = 0;
						tempSb.AppendFormat("Target[{0}],Command[{1}]", pos, Math.Abs(ret.d));
						//string str = "Target[" + pos.ToString() + "],Command[" + Math.Abs(ret.d).ToString() + "]";
						errorCheck((int)UnitCodeAxisNumber.INVALID, ERRORCODE.HD, sqc, tempSb.ToString(), ALARM_CODE.E_FORCE_PLACE_TARGET_DRIVE_TIMEOUT);
						break;
					}
					mc.hd.tool.Z[mc.hd.order.bond_done].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond_done].config.axisCode, sqc, ret.message)) break;
					if (ret.d < (pos - 2)) break;
					//kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PICKJIG
				case SQC.F_M2PICKJIG:
					pos = mc.hd.tool.tPos.z[mc.hd.order.pick].REF0 + 100;
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PICKJIG + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
                    mc.hd.tool.Z[mc.hd.order.pick].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.pick].config.axisCode, sqc, ret.message)) break;
                    if (ret.d > pos + 100) break;
                    //kilogram(mc.para.HD.pick.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    sqc = SQC.STOP; break;
                #endregion

                #region case F_PICKJIG2M
                case SQC.F_PICKJIG2M:
                    pos = mc.hd.tool.tPos.z[mc.hd.order.ulc].REF0 + 2000;
                    dwell.Reset();
                    sqc++; break;
				case SQC.F_PICKJIG2M + 1:
					mc.hd.tool.Z[mc.hd.order.ulc].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.ulc].config.axisCode, sqc, ret.message)) break;
					if (ret.d < pos) break;
					//kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_M2PLACEJIG
				case SQC.F_M2PLACEJIG:
					pos = mc.hd.tool.tPos.z[mc.hd.order.bond].PEDESTAL + 300;
					dwell.Reset();
					sqc++; break;
				case SQC.F_M2PLACEJIG + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d > pos + 500) break;
					//kilogram(mc.para.HD.place.force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				#region case F_PLACEJIG2M
				case SQC.F_PLACEJIG2M:
                    pos = mc.hd.tool.tPos.z[mc.hd.order.bond].PEDESTAL + 2000;
					dwell.Reset();
					sqc++; break;
				case SQC.F_PLACEJIG2M + 1:
					if (timeCheck(UnitCodeAxis.F, sqc, 5)) break;
					mc.hd.tool.Z[mc.hd.order.bond].commandPosition(out ret.d, out ret.message); if (mpiCheck(mc.hd.tool.Z[mc.hd.order.bond].config.axisCode, sqc, ret.message)) break;
					if (ret.d < pos) break;
					//kilogram(mc.para.HD.moving_force, out ret.message); if (ioCheck(sqc, ret.message)) break;
					sqc = SQC.STOP; break;
				#endregion

				case SQC.ERROR:
					//errString = "HD Force Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("HD Force Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					req = false;
					sqc = SQC.END; break;
			}
		}
	}

	#region position class
	public class classHeadCamearPosition
	{
        public classHeadCamearPositionX x = new classHeadCamearPositionX();
        public classHeadCamearPositionY y = new classHeadCamearPositionY();
	}
	public class classHeadToolPosition
	{
		public classHeadToolPositionX[] x = new classHeadToolPositionX[mc.activate.headCnt];
		public classHeadToolPositionY[] y = new classHeadToolPositionY[mc.activate.headCnt];
        public classHeadToolPositionZ[] z = new classHeadToolPositionZ[mc.activate.headCnt];
        public classHeadToolPositionT[] t = new classHeadToolPositionT[mc.activate.headCnt];

        public classHeadToolPosition()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                x[i] = new classHeadToolPositionX(i);
                y[i] = new classHeadToolPositionY(i);
                z[i] = new classHeadToolPositionZ(i);
                t[i] = new classHeadToolPositionT(i);
            }
        }
	}
	public class classHeadLaserPosition
	{
		public classHeadLaserPositionX x = new classHeadLaserPositionX();
		public classHeadLaserPositionY y = new classHeadLaserPositionY();
	}

	public class classHeadCamearPositionX
	{
		public double REF0
		{
			get
			{
				double tmp;
                tmp = mc.coor.MP.TOOL.X.CAMERA.value;
                tmp += mc.coor.MP.HD.X.REF0.value  + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_1.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_2.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp;
			}
		}
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.BD_EDGE_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.BD_EDGE_RR.value;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
        public double HDC_PD_P1
        {
            get
            {
                double tmp;
                tmp = BD_EDGE;
                //tmp += mc.para.CAL.conveyorEdge.x.value;
                tmp -= 50000;
                return tmp;
            }
        }
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			return tmp;
		}
		public double PADC1(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC1.patternPos.x.value;
			return tmp;
		}
        public double PADC2(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PADC1(column);
            if (alignPos) tmp += mc.para.HDC.modelPADC2.patternPos.x.value;
            return tmp;
		}
        public double PADC3(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(column);
			tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC3.patternPos.x.value;
			return tmp;
		}
        public double PADC4(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PADC3(column);
            if (alignPos) tmp += mc.para.HDC.modelPADC4.patternPos.x.value;
			return tmp;
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.ULC.value + mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (mc.swcontrol.mechanicalRevision == 0)
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.X.SF_TUBE1.value;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.X.SF_TUBE2.value;
                else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.coor.MP.HD.X.SF_TUBE3.value;
                else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.coor.MP.HD.X.SF_TUBE4.value;
                else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.X.SF_TUBE5.value;
                else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.X.SF_TUBE6.value;
                else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.coor.MP.HD.X.SF_TUBE7.value;
                else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.coor.MP.HD.X.SF_TUBE8.value;
                else tmp += (double)mc.coor.MP.HD.X.SF_TUBE1.value;
			}
			else
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.X.SF_TUBE2_4SLOT.value;
                else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.X.SF_TUBE3_4SLOT.value;
                else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.X.SF_TUBE4_4SLOT.value;
                else tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value;
			}
			tmp += mc.para.CAL.pick.x.value;
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P2.value;
            else tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}

		public double PD_P1
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.PD_P1_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.PD_P1_RR.value;
				return tmp;
			}
		}
		public double PD_P2
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.PD_P2_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.PD_P2_RR.value;
				return tmp;
			}
		}
		public double PD_P3
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.PD_P3_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.PD_P3_RR.value;
				return tmp;
			}
		}
		public double PD_P4
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.PD_P4_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.PD_P4_RR.value;
				return tmp;
			}
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.x.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.x.value;
				return tmp;
			}
		}


	}
	public class classHeadCamearPositionY
	{
		public double REF0
		{
			get
			{
				double tmp;
                tmp = mc.coor.MP.TOOL.Y.CAMERA.value;
                tmp += mc.coor.MP.HD.Y.REF0.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_1.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_2.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}

		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.BD_EDGE.value + mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
        public double HDC_PD_P1
        {
            get
            {
                double tmp;
                tmp = BD_EDGE;
                //tmp += mc.para.CAL.conveyorEdge.y.value;
                tmp += 50000;
                return tmp;
            }
        }
		public double PAD(int row)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
			}
            tmp += mc.swcontrol.pdOffsetY;
			return tmp;
		}
        public double PADC1(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.padSize.y.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC1.patternPos.y.value;
			return tmp;
		}
        public double PADC2(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.padSize.y.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC2.patternPos.y.value;
			return tmp;
		}
        public double PADC3(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PADC2(row);
            if (alignPos) tmp += mc.para.HDC.modelPADC3.patternPos.y.value;
			return tmp;
		}
        public double PADC4(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PADC1(row);
            if (alignPos) tmp += mc.para.HDC.modelPADC4.patternPos.y.value;
			return tmp;
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.ULC.value + mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}

        public double LIDC1()
        {
            double tmp;
            tmp = ULC;
            tmp += (mc.para.MT.lidSize.y.value * 1000 * 0.5);
            return tmp;
        }
        public double LIDC2()
        {
            double tmp;
            tmp = ULC;
            tmp -= (mc.para.MT.lidSize.y.value * 1000 * 0.5);
            return tmp;
        }
        public double LIDC3()
        {
            double tmp;
            tmp = LIDC2();
            return tmp;
        }
        public double LIDC4()
        {
            double tmp;
            tmp = LIDC1();
            return tmp;
        }

		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
            if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.Y.SF_TUBE1.value;
            else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.Y.SF_TUBE2.value;
            else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.coor.MP.HD.Y.SF_TUBE3.value;
            else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.coor.MP.HD.Y.SF_TUBE4.value;
            else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.Y.SF_TUBE5.value;
            else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.Y.SF_TUBE6.value;
            else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.coor.MP.HD.Y.SF_TUBE7.value;
            else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.coor.MP.HD.Y.SF_TUBE8.value;
            else tmp += mc.coor.MP.HD.Y.SF_TUBE1.value;
			tmp += mc.para.CAL.pick.y.value;
			#endregion
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P2.value;
            else tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}

		public double PD_P1
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                tmp += mc.coor.MP.HD.Y.PD_P1.value;
				return tmp;
			}
		}
		public double PD_P2
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                tmp += mc.coor.MP.HD.Y.PD_P2.value;
				return tmp;
			}
		}
		public double PD_P3
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                tmp += mc.coor.MP.HD.Y.PD_P3.value;
				return tmp;
			}
		}
		public double PD_P4
		{
			get
			{
				double tmp;
				tmp = BD_EDGE;
                tmp += mc.coor.MP.HD.Y.PD_P4.value;
				return tmp;
			}
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.y.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.y.value;
				return tmp;
			}
		}
	}

	public class classHeadToolPositionX
	{
        int headNum;
        public classHeadToolPositionX()
        {
            headNum = 0;
        }
        public classHeadToolPositionX(int Num)
        {
            headNum = Num;
        }

		public double REF0
		{
			get
			{
				double tmp;
                tmp = -mc.coor.MP.TOOL.X.TOOL.value;
				tmp += mc.para.CAL.HDC_TOOL[headNum].x.value;
                tmp += mc.coor.MP.HD.X.REF0.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				tmp -= mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_1.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_2.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp; 
			}
		}
		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.ULC.value + mc.para.CAL.ulc.x.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= mc.coor.MP.TOOL.X.TOOL1.value; /*tmp -= 85000;*/
				return tmp;
			}
		}
        public double LIDC1
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC2
        {
            get
            {
                double tmp;
                tmp = LIDC1;
                return tmp;            
            }

        }
        public double LIDC3
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC4
        {
            get
            {
                double tmp;
                tmp = LIDC3;
                return tmp;
            }
        }

		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.BD_EDGE_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.BD_EDGE_RR.value;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
				// 160606. jhlim
                if (headNum == 0) tmp = tmp + mc.para.HD.place.offset.x.value + mc.swcontrol.placeOffset_HD1X;
                else tmp = tmp + mc.para.HD.place.offset2.x.value + mc.swcontrol.placeOffset_HD2X;
                //Derek 수정예정. LJS 값을 coor.xlsx에 추가해놓음. head1 <-> head2  거리(85000)로 기입.
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			return tmp;
		}
        public double PADC1(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5);
            //if (alignPos) tmp += mc.para.HDC.modelPADC1.patternPos.x.value;
			return tmp;
		}
        public double PADC2(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PADC1(column);
            //if (alignPos) tmp += mc.para.HDC.modelPADC2.patternPos.x.value;
			return tmp;
		}
        public double PADC3(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(column);
			//tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC3.patternPos.x.value;
			return tmp;
		}
        public double PADC4(int column, bool alignPos = false)
		{
			double tmp;
			tmp = PADC3(column);
            //if (alignPos) tmp += mc.para.HDC.modelPADC1.patternPos.y.value;
			return tmp;
		}

        public double PEDC1(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(column);
            tmp += (mc.para.MT.pedestalSize.x.value * 1000 * 0.5);
            if (usePDOffset) tmp -= mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC2(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC1(column, usePDOffset);
            return tmp;
        }
        public double PEDC3(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(column);
            tmp -= (mc.para.MT.pedestalSize.x.value * 1000 * 0.5);
            if (usePDOffset) tmp += mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC4(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC3(column, usePDOffset);
            return tmp;
        }

		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (mc.swcontrol.mechanicalRevision == 0)
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.X.SF_TUBE1.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.X.SF_TUBE2.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].x.value;
                else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.coor.MP.HD.X.SF_TUBE3.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].x.value;
                else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.coor.MP.HD.X.SF_TUBE4.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].x.value;
                else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.X.SF_TUBE5.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF5].x.value;
                else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.X.SF_TUBE6.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF6].x.value;
                else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.coor.MP.HD.X.SF_TUBE7.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF7].x.value;
                else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.coor.MP.HD.X.SF_TUBE8.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF8].x.value;
                else tmp += mc.coor.MP.HD.X.SF_TUBE1.value;
			}
			else
			{
                if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
                else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.X.SF_TUBE2_4SLOT.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].x.value;
                else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.X.SF_TUBE3_4SLOT.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF5].x.value;
                else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.X.SF_TUBE4_4SLOT.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF6].x.value;
                else tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].x.value;
			}
			tmp += mc.para.CAL.pick.x.value;

			if (!mc.swcontrol.noUseCompPickPosition)
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF1].x.value;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF2].x.value;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF3].x.value;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF4].x.value;
				else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF5].x.value;
				else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF6].x.value;
				else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF7].x.value;
				else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF8].x.value;
			}
			#endregion
            if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P2.value;
            else if (changerNumber == UnitCodeToolChanger.T3) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P3.value;
            else if (changerNumber == UnitCodeToolChanger.T4) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P4.value;
            else tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.x.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.x.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}
		public double WASTE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.WASTE.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}

		public double JIG_PICK
		{
			get
			{
				double tmp;
				tmp = (REF1_1 + REF1_2) / 2;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionY
	{
        int headNum;
        public classHeadToolPositionY()
        {
            headNum = 0;
        }
        public classHeadToolPositionY(int Num)
        {
            headNum = Num;
        }

		public double REF0
		{
			get
			{
				double tmp;
                tmp = -mc.coor.MP.TOOL.Y.TOOL.value;
				tmp += mc.para.CAL.HDC_TOOL[headNum].y.value;
                tmp += mc.coor.MP.HD.Y.REF0.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				tmp -= mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_1.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_2.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}
		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.ULC.value + mc.para.CAL.ulc.y.value;
                //if (headNum == (int)UnitCodeHead.HD2) tmp -= mc.coor.MP.TOOL.Y.TOOL1.value; /*tmp -= 85000;*/
				return tmp;
			}
		}
        public double LIDC1
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp -= (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }
        }
        public double LIDC2
        {
            get
            {
                double tmp;
                tmp = ULC;
                tmp += (mc.para.MT.lidSize.x.value * 1000 * 0.5);
                return tmp;
            }

        }
        public double LIDC3
        {
            get
            {
                double tmp;
                tmp = LIDC2;
                return tmp;
            }
        }
        public double LIDC4
        {
            get
            {
                double tmp;
                tmp = LIDC1;
                return tmp;
            }
        }
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.BD_EDGE.value + mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
		public double PAD(int row)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
                // 160606. jhlim
                if (headNum == 0) tmp = tmp + mc.para.HD.place.offset.y.value + mc.swcontrol.placeOffset_HD1Y;
                else tmp = tmp + mc.para.HD.place.offset2.y.value + mc.swcontrol.placeOffset_HD2Y;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
				tmp += mc.para.HD.place.offset.y.value;
			}
            tmp += mc.swcontrol.pdOffsetY;
			return tmp;
		}
        public double PADC1(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(row);
			tmp += (mc.para.MT.padSize.y.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC1.patternPos.y.value;
			return tmp;
		}
		public double PADC2(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PAD(row);
			tmp -= (mc.para.MT.padSize.y.value * 1000 * 0.5);
            if (alignPos) tmp += mc.para.HDC.modelPADC2.patternPos.y.value;
			return tmp;
		}
        public double PADC3(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PADC2(row);
            if (alignPos) tmp += mc.para.HDC.modelPADC3.patternPos.y.value;
			return tmp;
		}
        public double PADC4(int row, bool alignPos = false)
		{
			double tmp;
			tmp = PADC1(row);
            if (alignPos) tmp += mc.para.HDC.modelPADC4.patternPos.y.value;
			return tmp;
		}

        public double PEDC1(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(row);
            tmp += (mc.para.MT.pedestalSize.y.value * 1000 * 0.5);
            if (usePDOffset) tmp -= mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC2(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(row);
            tmp -= (mc.para.MT.pedestalSize.y.value * 1000 * 0.5);
            if (usePDOffset) tmp += mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC3(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC2(row, usePDOffset);
            return tmp;
        }
        public double PEDC4(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC1(row, usePDOffset);
            return tmp;
        }

		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
            if (tubeNumber == UnitCodeSF.SF1) tmp += mc.coor.MP.HD.Y.SF_TUBE1.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].y.value;
            else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.coor.MP.HD.Y.SF_TUBE2.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].y.value;
            else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.coor.MP.HD.Y.SF_TUBE3.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].y.value;
            else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.coor.MP.HD.Y.SF_TUBE4.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].y.value;
            else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.coor.MP.HD.Y.SF_TUBE5.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF5].y.value;
            else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.coor.MP.HD.Y.SF_TUBE6.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF6].y.value;
            else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.coor.MP.HD.Y.SF_TUBE7.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF7].y.value;
            else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.coor.MP.HD.Y.SF_TUBE8.value + mc.para.HD.pick.offset[(int)UnitCodeSF.SF8].y.value;
            else tmp += mc.coor.MP.HD.Y.SF_TUBE1.value;
			tmp += mc.para.CAL.pick.y.value;

			if (!mc.swcontrol.noUseCompPickPosition)
			{
				if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF1].y.value;
				else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF2].y.value;
				else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF3].y.value;
				else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF4].y.value;
				else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF5].y.value;
				else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF6].y.value;
				else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF7].y.value;
				else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.para.HD.pick.pickPosComp[(int)UnitCodeSF.SF8].y.value;
			}
			#endregion
            if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.y[(int)UnitCodeHead.HD2].ULC);
			return tmp;
		}
		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P2.value;
            else if (changerNumber == UnitCodeToolChanger.T3) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P3.value;
            else if (changerNumber == UnitCodeToolChanger.T4) tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P4.value;
            else tmp += mc.coor.MP.HD.Y.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.y.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.y[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.y.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.y[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}
		public double WASTE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.WASTE.value;
                if (headNum == (int)UnitCodeHead.HD2) tmp -= (mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC - mc.hd.tool.tPos.y[(int)UnitCodeHead.HD2].ULC);
				return tmp;
			}
		}

		public double JIG_PICK
		{
			get
			{
				double tmp;
				tmp = (REF1_1 + REF1_2) / 2;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionZ
	{
        int headNum;
        public classHeadToolPositionZ()
        {
            headNum = 0;
        }
        public classHeadToolPositionZ(int Num)
        {
            headNum = Num;
        }

		public double REF0
		{
			get
			{
				double tmp;
                tmp = mc.coor.MP.HD.Z.REF.value + mc.para.CAL.z.ref0.value;
                tmp -= mc.para.CAL.z.heightOffset[headNum].value;
				return tmp;
			}
		}
		public double ULC_FOCUS
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.ULC_FOCUS.value + mc.para.CAL.z.ulcFocus.value;
				return tmp;
			}
		}
		public double ULC_FOCUS_WITH_MT
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.ULC_FOCUS.value + mc.para.CAL.z.ulcFocus.value;
				tmp += (double)mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}

		public double XY_MOVING
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.XY_MOVING.value + mc.para.CAL.z.xyMoving.value;
				return tmp;
			}
		}
		public double DOUBLE_DET
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.DOUBLE_DET.value + mc.para.CAL.z.doubleDet.value;
				tmp += mc.para.HD.pick.doubleCheck.offset.value;
				return tmp;
			}
		}
		public double TOOL_CHANGER
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.TOOL_CHANGER.value + mc.para.CAL.z.toolChanger.value;
				return tmp;
			}
		}
		public double PICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = REF0;
            tmp += mc.coor.MP.HD.Z.PICK.value + mc.para.CAL.z.pick.value;
            if (headNum == 1 && mc.para.ETC.useHeadMode.value == 2) tmp -= ((mc.para.MT.lidSize.h.value * 1000 + 300)); 
			#region tube select
			if (tubeNumber == UnitCodeSF.SF1) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].z.value;
			else if (tubeNumber == UnitCodeSF.SF2) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF2].z.value;
			else if (tubeNumber == UnitCodeSF.SF3) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF3].z.value;
			else if (tubeNumber == UnitCodeSF.SF4) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF4].z.value;
			else if (tubeNumber == UnitCodeSF.SF5) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF5].z.value;
			else if (tubeNumber == UnitCodeSF.SF6) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF6].z.value;
			else if (tubeNumber == UnitCodeSF.SF7) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF7].z.value;
			else if (tubeNumber == UnitCodeSF.SF8) tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF8].z.value;
			else tmp += mc.para.HD.pick.offset[(int)UnitCodeSF.SF1].z.value;
			#endregion
			return tmp;
		}
		public double DRYRUNPICK(UnitCodeSF tubeNumber)
		{
			double tmp;
			tmp = PICK(tubeNumber);
			tmp += 1000;	// 1mm정도 띄운다.
			return tmp;
		}
		public double RAWPICK
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.PICK.value + mc.para.CAL.z.pick.value;
				return tmp;
			}
		}
		public double PEDESTAL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.PEDESTAL.value + mc.para.CAL.z.pedestal.value;
				return tmp;
			}
		}
		public double PLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
                tmp += mc.para.HD.place.forceOffset.z.value;
                tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double DRYRUNPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				tmp += mc.para.HD.place.forceOffset.z.value;
				tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double FIXEDPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
				//tmp += mc.para.HD.place.forceOffset.z.value;
				//tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double FIXEDDRYRUNPLACE
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				//tmp += mc.para.HD.place.forceOffset.z.value;
				//tmp += mc.para.HD.place.offset.z.value;
				return tmp;
			}
		}
		public double CONTACTPOS
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				tmp += mc.para.MT.padSize.h.value * 1000;
				tmp += mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}
		public double DRYCONTACTPOS
		{
			get
			{
				double tmp;
				tmp = PEDESTAL;
				//tmp += mc.para.MT.padSize.h.value * 1000;
				//tmp += mc.para.MT.lidSize.h.value * 1000;
				return tmp;
			}
		}
		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.TOUCHPROBE.value + mc.para.CAL.z.touchProbe.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Z.LOADCELL.value + mc.para.CAL.z.loadCell.value;
				return tmp;
			}
		}
		public double SENSOR1
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += mc.para.CAL.z.sensor1.value;
				return tmp;
			}
		}
		public double SENSOR2
		{
			get
			{
				double tmp;
				tmp = REF0;
				tmp += mc.para.CAL.z.sensor2.value;
				return tmp;
			}
		}
	}
	public class classHeadToolPositionT
	{
        int headNum;
        public classHeadToolPositionT()
        {
            headNum = 0;
        }
        public classHeadToolPositionT(int Num)
        {
            headNum = Num;
        }

		public double ZERO
		{
			get
			{
				double tmp;
                tmp = mc.para.CAL.toolAngleOffset[headNum].value;
				return tmp;
			}
		}
		public double HOME
		{
			get
			{
				double tmp;
				tmp = 0;
				return tmp;
			}
		}
	}

	public class classHeadLaserPositionX
	{
		public double REF0
		{
			get
			{
				double tmp;
                tmp = -mc.coor.MP.TOOL.X.LASER.value;
				tmp += mc.para.CAL.HDC_LASER.x.value;
                tmp += mc.coor.MP.HD.X.REF0.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_1.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].x.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.REF1_2.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].x.value;
				return tmp;
			}
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.ULC.value;// +mc.para.CAL.ulc.x.value;
				return tmp;
			}
		}
		public double BD_EDGE
		{
			get
			{
				double tmp;
				tmp = REF0;
                if (mc.para.mcType.FrRr == McTypeFrRr.FRONT) tmp += mc.coor.MP.HD.X.BD_EDGE_FR.value;
                if (mc.para.mcType.FrRr == McTypeFrRr.REAR) tmp += mc.coor.MP.HD.X.BD_EDGE_RR.value;
				tmp += mc.para.CAL.conveyorEdge.x.value;
				return tmp;
			}
		}
		public double PAD(int column)
		{
			double tmp;
			tmp = BD_EDGE;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
				tmp -= (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp -= (mc.para.MT.padCount.x.value - column - 1) * (mc.para.MT.padPitch.x.value) * 1000;
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
				tmp += (mc.para.MT.edgeToPadCenter.x.value * 1000);
				if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
				tmp += (mc.para.MT.padCount.x.value - column - 1) * (mc.para.MT.padPitch.x.value) * 1000;
                tmp += mc.swcontrol.pdOffsetX;
				return tmp;
			}
			return tmp;
		}
		public double PADC1(int column)
		{
			double tmp;
			tmp = PAD(column);
			tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5 - mc.para.ETC.flatPedestalOffset.value);
			return tmp;
		}
		public double PADC2(int column)
		{
			double tmp;
			tmp = PADC1(column);
			return tmp;
		}
		public double PADC3(int column)
		{
			double tmp;
			tmp = PAD(column);
            tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5 - mc.para.ETC.flatPedestalOffset.value);
			return tmp;
		}
		public double PADC4(int column)
		{
			double tmp;
			tmp = PADC3(column);
			return tmp;
		}

        public double PEDC1(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(column);
            tmp += (mc.para.MT.pedestalSize.x.value * 1000 * 0.5);
            if (usePDOffset) tmp -= mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC2(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC1(column, usePDOffset);
            return tmp;
        }
        public double PEDC3(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(column);
            tmp -= (mc.para.MT.pedestalSize.x.value * 1000 * 0.5);
            if (usePDOffset) tmp += mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC4(int column, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC3(column, usePDOffset);
            return tmp;
        }

		public double PICK(int tubeNumber)	// not used
		{
			double tmp;
			tmp = REF0;
			#region tube select
			if (mc.swcontrol.mechanicalRevision == 0)
			{
                if (tubeNumber == 1) tmp += mc.coor.MP.HD.X.SF_TUBE1.value;
                else if (tubeNumber == 2) tmp += mc.coor.MP.HD.X.SF_TUBE2.value;
                else if (tubeNumber == 3) tmp += mc.coor.MP.HD.X.SF_TUBE3.value;
                else if (tubeNumber == 4) tmp += mc.coor.MP.HD.X.SF_TUBE4.value;
                else if (tubeNumber == 5) tmp += mc.coor.MP.HD.X.SF_TUBE5.value;
                else if (tubeNumber == 6) tmp += mc.coor.MP.HD.X.SF_TUBE6.value;
                else if (tubeNumber == 7) tmp += mc.coor.MP.HD.X.SF_TUBE7.value;
                else if (tubeNumber == 8) tmp += mc.coor.MP.HD.X.SF_TUBE8.value;
                else tmp += mc.coor.MP.HD.X.SF_TUBE1.value;
			}
			else
			{
                if (tubeNumber == 1) tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value;
                else if (tubeNumber == 2) tmp += mc.coor.MP.HD.X.SF_TUBE2_4SLOT.value;
                else if (tubeNumber == 5) tmp += mc.coor.MP.HD.X.SF_TUBE3_4SLOT.value;
                else if (tubeNumber == 6) tmp += mc.coor.MP.HD.X.SF_TUBE4_4SLOT.value;
                else tmp += mc.coor.MP.HD.X.SF_TUBE1_4SLOT.value;
			}
			#endregion
			return tmp;
		}

		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P2.value;
            else tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.x.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.X.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.x.value;
				return tmp;
			}
		}
	}
	public class classHeadLaserPositionY
	{
		public double REF0
		{
			get
			{
				double tmp;
                tmp = -mc.coor.MP.TOOL.Y.LASER.value;
				tmp += mc.para.CAL.HDC_LASER.y.value;
                tmp += mc.coor.MP.HD.Y.REF0.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
				return tmp;
			}
		}
		public double REF1_1
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_1.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_1].y.value;
				return tmp;
			}
		}
		public double REF1_2
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.REF1_2.value;// +mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF1_2].y.value;
				return tmp;
			}
		}

		public double ULC
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.ULC.value;// +mc.para.CAL.ulc.y.value;
				return tmp;
			}
		}

		public double BD_EDGE
		{
			get
			{
				double tmp;
                tmp = REF0;
                tmp += mc.coor.MP.HD.Y.BD_EDGE.value;
				tmp += mc.para.CAL.conveyorEdge.y.value;
				return tmp;
			}
		}
		public double PAD(int row)
		{
			double tmp;
			tmp = REF0;
			if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
			{
                tmp += mc.coor.MP.HD.Y.BD_EDGE.value + mc.para.CAL.conveyorEdge.y.value;
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += row * mc.para.MT.padPitch.y.value * 1000;
			}
			if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
			{
                tmp += mc.coor.MP.HD.Y.BD_EDGE.value + mc.para.CAL.conveyorEdge.y.value;
				tmp += (mc.para.MT.edgeToPadCenter.y.value * 1000);
				if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
				tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
			}
            tmp += mc.swcontrol.pdOffsetY;
			return tmp;
		}

		public double PADC1(int row)
		{
			double tmp;
			tmp = PAD(row);
            tmp += (mc.para.MT.padSize.x.value * 1000 * 0.5 - mc.para.ETC.flatPedestalOffset.value);
			return tmp;
		}
		public double PADC2(int row)
		{
			double tmp;
			tmp = PAD(row);
            tmp -= (mc.para.MT.padSize.x.value * 1000 * 0.5 - mc.para.ETC.flatPedestalOffset.value);
			return tmp;
		}
		public double PADC3(int row)
		{
			double tmp;
			tmp = PADC2(row);
			return tmp;
		}
		public double PADC4(int row)
		{
			double tmp;
			tmp = PADC1(row);
			return tmp;
		}

        public double PEDC1(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(row);
            tmp += (mc.para.MT.pedestalSize.y.value * 1000 * 0.5);
            if (usePDOffset) tmp -= mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC2(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PAD(row);
            tmp -= (mc.para.MT.pedestalSize.y.value * 1000 * 0.5);
            if (usePDOffset) tmp += mc.para.ETC.flatPedestalOffset.value;
            return tmp;
        }
        public double PEDC3(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC2(row, usePDOffset);
            return tmp;
        }
        public double PEDC4(int row, bool usePDOffset = false)
        {
            double tmp;
            tmp = PEDC1(row, usePDOffset);
            return tmp;
        }

		public double TOOL_CHANGER(UnitCodeToolChanger changerNumber)
		{
			double tmp;
			tmp = REF1_1;
			#region tool changer select
            if (changerNumber == UnitCodeToolChanger.T1) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
            else if (changerNumber == UnitCodeToolChanger.T2) tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P2.value;
            else tmp += mc.coor.MP.HD.X.TOOL_CHANGER_P1.value;
			#endregion
			return tmp;
		}

		public double PICK(int tubeNumber)
		{
			double tmp;
			tmp = REF0;
			#region tube select
            if (tubeNumber == 1) tmp += mc.coor.MP.HD.Y.SF_TUBE1.value;
            else if (tubeNumber == 2) tmp += mc.coor.MP.HD.Y.SF_TUBE2.value;
            else if (tubeNumber == 3) tmp += mc.coor.MP.HD.Y.SF_TUBE3.value;
            else if (tubeNumber == 4) tmp += mc.coor.MP.HD.Y.SF_TUBE4.value;
            else if (tubeNumber == 5) tmp += mc.coor.MP.HD.Y.SF_TUBE5.value;
            else if (tubeNumber == 6) tmp += mc.coor.MP.HD.Y.SF_TUBE6.value;
            else if (tubeNumber == 7) tmp += mc.coor.MP.HD.Y.SF_TUBE7.value;
            else if (tubeNumber == 8) tmp += mc.coor.MP.HD.Y.SF_TUBE8.value;
            else tmp += mc.coor.MP.HD.Y.SF_TUBE1.value;
			#endregion
			return tmp;
		}

		public double TOUCHPROBE
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.TOUCHPROBE.value;
				tmp += mc.para.CAL.touchProbe.y.value;
				return tmp;
			}
		}
		public double LOADCELL
		{
			get
			{
				double tmp;
				tmp = REF0;
                tmp += mc.coor.MP.HD.Y.LOADCELL.value;
				tmp += mc.para.CAL.loadCell.y.value;
				return tmp;
			}
		}
	}
	#endregion

    #region order class
    public class classMultiHeadOrder
    {
        //Derek 
        //multi head를 위한 현재 각 Head의 status를 정의
        //pickup은   noDei에 해당하면 실행
        //ulc는      pickupsucess에 해당하면실행
        //bond는     ulcinpsucess에 해당하면실행
        //waete는    pickupfail or ulcinpfail or boningfail 에 해당하면실행
       

        static int[] _sts = new int[mc.activate.headCnt];

        public classMultiHeadOrder()
        {
            for (int i = 0; i < mc.activate.headCnt; i++)
            {
                _sts[i] = 0;
            }
        }
        //sts는 항상 동작 완료후 status를 set할것
        public void set(int headNum, int status)
        {
            _sts[headNum] = status;
        }


        public int pick
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.NO_DIE) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int pick_done
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.PICK_SUCESS) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int pick_fail
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.PICK_FAIL) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }

        public int ulc
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.PICK_SUCESS) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int ulc_done
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.ULCI_SUCESS) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int ulc_fail
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.ULCI_FAIL) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }

        public int bond
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.ULCI_SUCESS) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int bond_done
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.BOND_SUCESS) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int bond_fail
        {
            get
            {
                //마지막 작업한 헤드 번호 Return
                for (int i = mc.activate.headCnt - 1; i >= 0; i--)
                {
                    if (_sts[i] == (int)ORDER.BOND_FAIL) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }

        public int waste
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.PICK_FAIL || _sts[i] == (int)ORDER.ULCI_FAIL || _sts[i] == (int)ORDER.BOND_FAIL) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int checktilt
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.NO_DIE) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
        public int checktilt_done
        {
            get
            {
                for (int i = 0; i < mc.activate.headCnt; i++)
                {
                    if (_sts[i] == (int)ORDER.CHECK_TILT_DONE) { mc.hd.tool.workingZ = i; return i; }
                }
                return (int)ORDER.EMPTY;
            }
        }
    }
    #endregion
}
