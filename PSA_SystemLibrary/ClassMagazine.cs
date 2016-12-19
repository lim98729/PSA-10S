using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeiLibrary;
using DefineLibrary;
using System.IO;
using AccessoryLibrary;
using System.Globalization;
namespace PSA_SystemLibrary
{
    public class ClassMagazine : CONTROL
    {
        public classMagazineTooL Elev = new classMagazineTooL();
        public bool SMEMA = false;

		public bool isActivate
		{
			get
			{
				return Elev.isActivate;
			}
		}

		public void activate(axisConfig z, out RetMessage retMessage)
		{
			Elev.activate(z, out retMessage);
		}

		public void deactivate(out RetMessage retMessage)
		{
			Elev.deactivate(out retMessage);
		}

		double posZ;
        bool errorChecked = false;

        public void control()
        {
            if (!req) return;

            if (!Elev.isAreaSafe())
            {
                if (!errorChecked)
                {
                    errorChecked = true;
                    Elev.Z.stop(out ret.message);
                    errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                    return;
                }
                //else
                //{
                //    return;
                //}
            }

            switch (sqc)
            {
                case 0:
                    Esqc = 0;
                    errorChecked = false;
                    sqc++; break;
                case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_MAGAZINE_NOT_READY, false); break; }
                    sqc++; break;
                case 2:
                    if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
                    if (reqMode == REQMODE.AUTO) { sqc = SQC.AUTO; break; }
                    if (reqMode == REQMODE.READY) { sqc = SQC.READY; break; }
                    if (reqMode == REQMODE.DUMY) { sqc = SQC.DUMY; break; }
                    errorCheck(ERRORCODE.INVALID, sqc, "unknown reqMode[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_MAGAZINE_LIST_NONE, false);
                    break;

                #region HOMING
                case SQC.HOMING:
                    if (dev.NotExistHW.ZMP) { mc.init.success.MG = true; sqc = SQC.STOP; break; }
                    //					mc.ps.req = true; mc.ps.reqMode = REQMODE.READY;
                    sqc++; break;

                case SQC.HOMING + 1:
                    //if (mc.ps.RUNING) break;
                    //if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;

                case SQC.HOMING + 2:
                    if (!Elev.isAreaSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                        break;
                    }

                    if (!Elev.isConveyorSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT);
                        break;
                    }

                    mc.init.success.MG = false;
                    sqc++; break;

                case SQC.HOMING + 3:
                    Elev.Z.abort(out ret.message);
                    dwell.Reset();
                    sqc++; break;

                case SQC.HOMING + 4:
                    if (dwell.Elapsed < 100) break;
                    Elev.Z.clearPosition(out ret.message);
                    Elev.homingZ.req = true;
                    sqc++; break;

                case SQC.HOMING + 5:
                    if (Elev.homingZ.RUNING) break;
                    if (Elev.homingZ.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
                    mc.init.success.MG = true;
                    sqc = SQC.STOP; break;
                    Elev.Z.move(Elev.pos.READY, out ret.message); if (mpiCheck(Elev.Z.config.axisCode, sqc, ret.message, "", false)) break;
                    Elev.dwell.Reset();
                    //dwell.Reset();
                    sqc++; break;

                case SQC.HOMING + 6:
                    if (!Elev.Z_AT_TARGET) break;
                    Elev.dwell.Reset();
                    sqc++; break;
                case SQC.HOMING + 7:
                    if (!Elev.Z_AT_DONE) break;
                    Elev.Z.reset(out ret.message);
                    mc.init.success.MG = true;
                    mc.OUT.MG.MG_RESET(false, out ret.message);
                    sqc = SQC.STOP; break;

                case SQC.HOMING_ERROR:
                    mc.init.success.MG = false;
                    Elev.Z.motorEnable(false, out ret.message);
                    sqc = SQC.ERROR; break;
                #endregion

                #region AUTO
                case SQC.AUTO:
                    SMEMA = false;					// 처음 시작 후 무빙 완료까지 꺼놔야함..

                    if (!Elev.isAreaSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                        break;
                    }

                    if (!Elev.isConveyorSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT);
                        break;
                    }

                    mc.log.debug.write(mc.log.CODE.INFO, "Start Magazine");
                    sqc++;
                    break;
                case SQC.AUTO + 1:
                    //mc.ps.req = true; mc.ps.reqMode = REQMODE.READY;

                    sqc++; break;
                case SQC.AUTO + 2:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (mc.ps.RUNING) break;
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //mc.log.debug.write(mc.log.CODE.INFO, "Go to Next Slot : (#" + slotNumber.ToString() + ")");
                    //posZ = Elev.pos.MG1_READY - (slotNumber * mc.para.UD.slotPitch.value) * 1000;
                    //Elev.Z.move(posZ, out ret.message); if (mpiCheck(Elev.Z.config.axisCode, sqc, ret.message)) break;
                    Elev.dwell.Reset();
                    sqc++; break;

                case SQC.AUTO + 3:
                    Elev.moveReadyPosition();
                    if (Elev.RUNING) break;
                    if (Elev.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //if (!Elev.Z_AT_TARGET) break;
                    //Elev.dwell.Reset();
                    sqc++; break;

                case SQC.AUTO + 4:
                    //if (!Elev.Z_AT_DONE) break;
                    SMEMA = true;
                    sqc++; break;

                case SQC.AUTO + 5:
                    // Pusher 동작 대기
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }

                    if (mc.ps.RUNING) break;
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!mc.ps.pusher_finish) break;

                    sqc++; break;

                case SQC.AUTO + 6:
                    //if (mc.Magazinecontrol.BOAT_ERROR_EXIST(mc.Magazinecontrol.MG_MAP_DATA[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER]))
                    //{
                    //    mc.Magazinecontrol.MG_Status[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER] = MG_STATUS.ERROR;
                    //}
                    //else
                    //{
                    //    mc.Magazinecontrol.MG_Status[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER] = MG_STATUS.DONE;
                    //}
                    //mc.Magazinecontrol.writeconfig();
                    //EVENT.refreshEditMagazine(mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER);
                    SMEMA = false;
                    mc.ps.req = true; mc.ps.reqMode = REQMODE.READY;
                    sqc++; break;

                case SQC.AUTO + 7:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (mc.ps.RUNING) break; //{ errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_PUSHER_MOTION_NOT_COMPLETE); break; }  
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.UnloaderControl.MG_Status[Elev.workMG, Elev.workSlot] = (int)MG_STATUS.DONE;
                    mc.UnloaderControl.writeconfig();
                    EVENT.refreshEditMagazine(Elev.workMG, Elev.workSlot);
                    //slotNumber++; 
                    sqc = SQC.AUTO + 1;
                    break;


                #endregion

                #region READY
                case SQC.READY:
                    SMEMA = false;
                    //mc.OUT.MG.MG_RESET(false, out ret.message);

                    if (!Elev.isAreaSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                        break;
                    }

                    if (!Elev.isConveyorSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT);
                        break;
                    }

                    mc.ps.req = true;
                    mc.ps.reqMode = REQMODE.READY;
                    sqc++; break;
                case SQC.READY + 1:
                    if (mc.ps.RUNING) break;
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    sqc++; break;
                case SQC.READY + 2:
                    sqc++; break;
                case SQC.READY + 3:
                    for (int i = 0; i < mc.UnloaderControl.MG_COUNT; i++)
                    {
                        for (int j = 0; j < mc.UnloaderControl.MG_SLOT_COUNT; j++)
                        {
                            mc.UnloaderControl.MG_Status[i, j] = (int)MG_STATUS.READY;
                            EVENT.refreshEditMagazine(i, j);
                        }
                    }
                    mc.UnloaderControl.writeconfig();
                    sqc++; break;
                case SQC.READY + 4:
                    posZ = mc.unloader.Elev.pos.READY;
                    mc.unloader.Elev.Z.move(posZ, out ret.message);
                    if (mpiCheck(Elev.Z.config.axisCode, sqc, ret.message, "", false)) break;
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 5:
                    if (!Elev.Z_AT_TARGET) break;
                    sqc++; dwell.Reset(); break;
                case SQC.READY + 6:
                    if (!Elev.Z_AT_DONE) break;
                    sqc = SQC.STOP; break;
                #endregion

                #region DUMY(매뉴얼 동작)
                case SQC.DUMY:
                    SMEMA = false;					// 처음 시작 후 무빙 완료까지 꺼놔야함..

                    if (!Elev.isAreaSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                        break;
                    }

                    if (!Elev.isConveyorSafe())
                    {
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT);
                        break;
                    }
                    mc.log.debug.write(mc.log.CODE.INFO, "Start Magazine");
                    sqc++;
                    break;
                case SQC.DUMY + 1:
                    //mc.ps.req = true; mc.ps.reqMode = REQMODE.READY;

                    sqc++; break;
                case SQC.DUMY + 2:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (mc.ps.RUNING) break;
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //mc.log.debug.write(mc.log.CODE.INFO, "Go to Next Slot : (#" + slotNumber.ToString() + ")");
                    //posZ = Elev.pos.MG1_READY - (slotNumber * mc.para.UD.slotPitch.value) * 1000;
                    //Elev.Z.move(posZ, out ret.message); if (mpiCheck(Elev.Z.config.axisCode, sqc, ret.message)) break;
                    Elev.dwell.Reset();
                    sqc++; break;

                case SQC.DUMY + 3:
                    Elev.moveReadyPosition();
                    if (Elev.RUNING) break;
                    if (Elev.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    //if (!Elev.Z_AT_TARGET) break;
                    //Elev.dwell.Reset();
                    sqc++; break;

                case SQC.DUMY + 4:
                    //if (!Elev.Z_AT_DONE) break;
                    SMEMA = true;
                    sqc++; break;

                case SQC.DUMY + 5:
                    // Pusher 동작 대기
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }

                    if (mc.ps.RUNING) break;
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    if (!mc.ps.pusher_finish) break;

                    sqc++; break;

                case SQC.DUMY + 6:
                    //if (mc.Magazinecontrol.BOAT_ERROR_EXIST(mc.Magazinecontrol.MG_MAP_DATA[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER]))
                    //{
                    //    mc.Magazinecontrol.MG_Status[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER] = MG_STATUS.ERROR;
                    //}
                    //else
                    //{
                    //    mc.Magazinecontrol.MG_Status[mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER] = MG_STATUS.DONE;
                    //}
                    //mc.Magazinecontrol.writeconfig();
                    //EVENT.refreshEditMagazine(mc.Magazinecontrol.READY_MG_NUMBER, mc.Magazinecontrol.READY_SLOT_NUMBER);
                    SMEMA = false;
                    mc.ps.req = true; mc.ps.reqMode = REQMODE.READY;
                    sqc++; break;

                case SQC.DUMY + 7:
                    if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    if (mc.ps.RUNING) break; //{ errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_PUSHER_MOTION_NOT_COMPLETE); break; }  
                    if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }
                    mc.UnloaderControl.MG_Status[Elev.workMG, Elev.workSlot] = (int)MG_STATUS.DONE;
                    mc.UnloaderControl.writeconfig();
                    EVENT.refreshEditMagazine(Elev.workMG, Elev.workSlot);
                    //slotNumber++; 
                    sqc = SQC.STOP;
                    break;


                #endregion

                case SQC.ERROR:
                    Elev.Z.reset(out ret.message);
                    mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Unloader Esqc {0}", Esqc));
                    // 여기와서 알람만 뜨고 모션 정지 안하면 정지하는 부분 추가 
                    sqc = SQC.STOP; break;

                case SQC.STOP:
                    SMEMA = false;
                    reqMode = REQMODE.AUTO;
                    req = false;
                    sqc = SQC.END; break;
                default:
                    sqc = SQC.STOP; break;
            }
        }
        public class classMagazineTooL : TOOL_CONTROL
        {
            public int checkMgNum;
            public int makeSkipCount;
            public double init_Z; 
            public double posZ;
            
            public mpiMotion Z = new mpiMotion();
            public classMagazinePositionZ pos = new classMagazinePositionZ();
            public sensorHoming homingZ = new sensorHoming();

            public bool isActivate
            {
                get
                {
                    if (!Z.isActivate) return false;
                    if (!homingZ.isActivate) return false;
                    return true;
                }
            }
            public void activate(axisConfig z, out RetMessage retMessage)
            {
                if (!Z.isActivate)
                {
                    Z.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage, "", false)) return;
                }
                if (!homingZ.isActivate)
                {
                    homingZ.activate(z, out retMessage); if (mpiCheck(UnitCodeAxis.Z, 0, retMessage, "", false)) return;
                }
                mc.UnloaderControl.readconfig();
                retMessage = RetMessage.OK;
            }
            public void deactivate(out RetMessage retMessage)
            {
                Z.deactivate(out retMessage);

                homingZ.deactivate(out retMessage);
            }
            public void jogMove(double posZ, out RetMessage retMessage)
            {
                bool safe = true;
                Z.move(posZ, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                #region endcheck
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }

                    if (!isAreaSafe())
                    {
                        Z.stop(out ret.message);
                        safe = false;
                        break;
                    }

                    Z.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }
                dwell.Reset();
                while (safe)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    Z.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                    if (ret.b) break;
                }

                if (!safe)
                {
                    string tmpStr;
                    mc.idle(1000);
                    Z.reset(out ret.message);
                    mc.unloader.directErrorCheck(out tmpStr, ERRORCODE.MG, ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                    mc.error.set(mc.error.MG, ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT, tmpStr, false);
                    mc.error.CHECK();

                }
                return;
                #endregion
            FAIL:
                mc.init.success.MG = false;
                return;
            }
            public void motorDisable(out RetMessage retMessage)
            {
                mc.init.success.MG = false;
                Z.motorEnable(false, out retMessage);
                Z.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
                return;
            }
            public void motorAbort(out RetMessage retMessage)
            {
                mc.init.success.MG = false;
                Z.abort(out retMessage); if (retMessage != RetMessage.OK) return;
                return;
            }

			//public void checkSensor()
			//{
			//    switch (sqc)
			//    {
			//        case 0:
			//            dwell.Reset();
			//            Esqc = 0;
			//            checkMgNum = 0;
			//            sqc++; break;

			//        case 1:
			//            mc.IN.MG.MG_AREA_SENSOR(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
			//            mc.IN.MG.MG_BOAT_DETECT(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
			//            if (ret.b)
			//            { errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT); break; }
			//            if (ret.b1)
			//            { errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT); break; }

			//            if (MagazineFullStatus) { sqc = 20; break; }			// 꽉 찼을 시 down sqc로 이동

			//            if(mc.Magazinecontrol.READY_MG_NUMBER == (int)MG_NUM.MG1)	posZ = mc.mg.Elev.pos.MG1_READY;
			//            else if (mc.Magazinecontrol.READY_MG_NUMBER == (int)MG_NUM.MG2) posZ = mc.mg.Elev.pos.MG2_READY;
			//            else if (mc.Magazinecontrol.READY_MG_NUMBER == (int)MG_NUM.MG3) posZ = mc.mg.Elev.pos.MG3_READY;
			//            sqc++; break;

			//        case 2:
			//            mc.mg.Elev.Z.move(posZ, out ret.message);
			//            if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
			//            dwell.Reset();
			//            sqc++; break;

			//        case 3:
			//            if (!Z_AT_TARGET) break;
			//            sqc++; dwell.Reset(); break;

			//        case 4:
			//            if (!Z_AT_DONE) break;
			//            sqc++; break;

			//        case 5:
			//            mc.IN.MG.MG_IN(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
			//            if (ret.b)	sqc = SQC.STOP;
			//            else sqc++;
			//            break;

			//        case 6 :				
			//            FormUserMessage ff = new FormUserMessage();
			//            mc.OUT.MAIN.UserBuzzerCtl(true);
			//            ff.SetDisplayItems(DIAG_SEL_MODE.RetrySkipCancel, DIAG_ICON_MODE.WARNING, "MG(#" + (checkMgNum + 1).ToString() + ") 감지 에러.");
			//            ff.ShowDialog();
			//            DIAG_RESULT fResult = FormUserMessage.diagResult;

			//            if (fResult == DIAG_RESULT.Retry)
			//            {
			//                mc.OUT.MAIN.UserBuzzerCtl(false);
			//                sqc--; break;
			//            }
			//            else if (fResult == DIAG_RESULT.Skip)
			//            {
			//                mc.OUT.MAIN.UserBuzzerCtl(false);
			//                for (int i = 0; i < mc.Magazinecontrol.MG_SLOT_COUNT; i++)
			//                    mc.Magazinecontrol.MG_Status[checkMgNum, i] = MG_STATUS.SKIP;
			//                mc.Magazinecontrol.writeconfig();
			//                //EVENT.refreshMagazine(true, true);
			//                sqc = 1; break;
			//            }
			//            else
			//            {
			//                mc.OUT.MAIN.UserBuzzerCtl(false);
			//                errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT); 
			//                break;
			//            }
			//        #region 주석
			//        //case 1:
			//        //    if (makeSkipCount >= mc.Magazinecontrol.MG_COUNT) { sqc = SQC.ERROR; break; }
			//        //    if (checkMgNum >= mc.Magazinecontrol.MG_COUNT) { sqc = SQC.STOP; break; }
			//        //    sqc = 9; break;

			//        //case 9:
			//        //    if (mc.Magazinecontrol.MG_READY[checkMgNum]) sqc = 10;
			//        //    else { checkMgNum = checkMgNum + 1; sqc = 1; }
			//        //    break;

			//        //case 10:
			//        //    if ((MG_NUM)checkMgNum == MG_NUM.MG1) mc.mg.Elev.Z.move(mc.mg.Elev.pos.MG1_READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
			//        //    else if ((MG_NUM)checkMgNum == MG_NUM.MG2) mc.mg.Elev.Z.move(mc.mg.Elev.pos.MG2_READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
			//        //    else if ((MG_NUM)checkMgNum == MG_NUM.MG3) mc.mg.Elev.Z.move(mc.mg.Elev.pos.MG3_READY, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;

			//        //    dwell.Reset();
			//        //    sqc++; break;

			//        //case 11:
			//        //    if (!Z_AT_TARGET) break;
			//        //    sqc++; dwell.Reset(); break;

			//        //case 12:
			//        //    if (!Z_AT_DONE) break;
			//        //    sqc++; break;

			//        //case 13:
			//        //    mc.IN.MG.MG_IN(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
			//        //    if (ret.b)
			//        //    {
			//        //        checkMgNum++;
			//        //        sqc = 1;
			//        //    }
			//        //    else
			//        //        sqc++;
			//        //    break;

			//        //case 14:
			//        //    FormUserMessage ff = new FormUserMessage();
			//        //    mc.OUT.MAIN.UserBuzzerCtl(true);
			//        //    ff.SetDisplayItems(DIAG_SEL_MODE.RetrySkipCancel, DIAG_ICON_MODE.WARNING, "MG(#" + (checkMgNum + 1).ToString() + ") 감지 에러.");
			//        //    ff.ShowDialog();
			//        //    DIAG_RESULT fResult = FormUserMessage.diagResult;

			//        //    if (fResult == DIAG_RESULT.Retry)
			//        //    {
			//        //        mc.OUT.MAIN.UserBuzzerCtl(false);
			//        //        sqc = 13; break;
			//        //    }
			//        //    else if (fResult == DIAG_RESULT.Skip)
			//        //    {
			//        //        mc.OUT.MAIN.UserBuzzerCtl(false);
			//        //        for (int i = 0; i < mc.Magazinecontrol.MG_SLOT_COUNT; i++)
			//        //            mc.Magazinecontrol.MG_Status[checkMgNum, i] = MG_STATUS.SKIP;
			//        //        mc.Magazinecontrol.writeconfig();
			//        //        //EVENT.refreshMagazine(true, true);
			//        //        makeSkipCount++;
			//        //    }
			//        //    else if (fResult == DIAG_RESULT.Cancel)
			//        //    {
			//        //        mc.OUT.MAIN.UserBuzzerCtl(false);
			//        //        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT); 
			//        //        break;
			//        //    }

			//        //    checkMgNum++;
			//        //    sqc = 1;
			//        //    break;
			//        #endregion

			//        case 20:
			//            posZ = mc.mg.Elev.pos.MG1_READY;
			//            mc.mg.Elev.Z.move(posZ, out ret.message);
			//            if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
			//            dwell.Reset();
			//            sqc++; break;
			//        case 21:
			//            if (!Z_AT_TARGET) break;
			//            sqc++; dwell.Reset(); break;
			//        case 22:
			//            if (!Z_AT_DONE) break;
			//            sqc++; break;
			//        case 23:
			//            errorCheck(ERRORCODE.MG, sqc, "MAGAZINE FULL!!"); break;

			//        case SQC.ERROR:
			//            mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Unloader Esqc {0}", Esqc));
			//            sqc = SQC.STOP; break;

			//        case SQC.STOP:
			//            sqc = SQC.END; break;
			//    }
			//}

            public int workMG = 0;
            public int workSlot = 0;
             
            public bool isAreaSafe()
            {
                if (mc.para.UD.AreaCheck.value == 0) return true;

                mc.IN.MG.MG_AREA_SENSOR1(out ret.b, out ret.message);
                if (!ret.b) return true;
                else return false;
            }

            public bool isConveyorSafe()
            {
                return true;
                if (mc.para.UD.AreaCheck.value == 0) return true;

                mc.IN.PS.END(out ret.b, out ret.message);
                if (!ret.b) return true;
                else return false;
            }

            public void moveReadyPosition()
            {
                switch (sqc)
                {
                    case 0:
                        Esqc = 0;
                        sqc++; 
                        break;
						
                    case 1:
						if (mc.ps.RUNING) break;
						if (mc.ps.ERROR) { Esqc = sqc; sqc = SQC.ERROR; break; }

                        if (!isAreaSafe())
                        {
                            errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_AREA_SENSOR_DETECT);
                            break;
                        }

                        if (!isConveyorSafe())
                        {
                            errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAZINE_IO_BOAT_SENSOR_DETECT);
                            break;
                        }

						#region Check Magazine Status
                        mc.UnloaderControl.readconfig();
                        if (MagazineIsFull())
                        {
                            sqc = 20; 			// 꽉 찼을 시 down sqc로 이동
                            break; 
                        }
                        else
                        {
                            workMG = 0;
                            MagazineReadyPos(out workMG, out workSlot);
                        }

                        if (workMG == (int)MG_NUM.MG2) posZ = pos.MG2_READY;
                        else if (workMG == (int)MG_NUM.MG3) posZ = pos.MG3_READY;
                        else posZ = pos.MG1_READY;
                        posZ -= workSlot * mc.para.UD.slotPitch.value * 1000;
                        #endregion

                        Z.move(posZ, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
                        dwell.Reset();
                        sqc++; break;

                    case 2:
                        if (!Z_AT_TARGET) break;
                        dwell.Reset();
                        sqc++; break;

                    case 3:
                        if (!Z_AT_DONE) break;
						dwell.Reset();
						sqc++; break;

					case 4:
                        if (mc.para.UD.MagazineInCheck.value == 0)
                        {
                            sqc = SQC.STOP;
                        }
                        else
                        {
                            if (dwell.Elapsed < 100) break;
                            mc.IN.MG.MG_IN(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                            if (ret.b) sqc = SQC.STOP;
                            else sqc++;
                        }
						break;

					case 5:
						FormUserMessage ff = new FormUserMessage();
						mc.OUT.MAIN.UserBuzzerCtl(true);
                        ff.SetDisplayItems(DIAG_SEL_MODE.RetrySkipCancel, DIAG_ICON_MODE.WARNING, "MG(#" + (workMG + 1).ToString() + ") 감지 에러 발생");
						ff.ShowDialog();
						DIAG_RESULT fResult = FormUserMessage.diagResult;

                        mc.OUT.MAIN.UserBuzzerCtl(false);
						if (fResult == DIAG_RESULT.Retry)
						{
							dwell.Reset();
							sqc--; break;
						}
						else if (fResult == DIAG_RESULT.Skip)
						{
							for (int i = 0; i < mc.UnloaderControl.MG_SLOT_COUNT; i++)
							{
                                mc.UnloaderControl.MG_Status[workMG, i] = (int)MG_STATUS.SKIP;
                                EVENT.refreshEditMagazine(workMG, i);
							}
							mc.UnloaderControl.writeconfig();

							sqc = 1; break;
						}
						else
						{
                            errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAGINE_NOT_EXIST); 
							break;
						}

					case 20:
						posZ = mc.unloader.Elev.pos.READY;
						mc.unloader.Elev.Z.move(posZ, out ret.message);
						if (mpiCheck(Z.config.axisCode, sqc, ret.message, "", false)) break;
						dwell.Reset();
						sqc++; break;
					case 21:
						if (!Z_AT_TARGET) break;
						sqc++; dwell.Reset(); break;
					case 22:
						if (!Z_AT_DONE) break;
						sqc++; break;
					case 23:
						//mc.OUT.MG.MG_RESET(true, out ret.message);
                        errorCheck(ERRORCODE.MG, sqc, "", ALARM_CODE.E_MAGAGINE_STATUS_FULL); break; 

                    case SQC.ERROR:
                        sqc = SQC.STOP; break;

                    case SQC.STOP:
                        sqc = SQC.END; break;
                }
            }
            

            public class classMagazinePositionZ
            {
				public double READY
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.READY;
                        tmp += mc.para.UD.ReadyPos.value;
						return tmp;
					}
				}
                public double MG1_READY
                {
                    get
                    {
                        double tmp;
                        tmp = (double)MP_MG_Z.MG1_READY;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG1, (int)UnitCodeSlotSelect.START].z.value;
                        return tmp;
                    }
                }
				public double MG1_END
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.MG1_END;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG1, (int)UnitCodeSlotSelect.END].z.value;
						return tmp;
					}
				}
				public double MG2_READY
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.MG2_READY;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG2, (int)UnitCodeSlotSelect.START].z.value;
						return tmp;
					}
				}
				public double MG2_END
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.MG2_END;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG2, (int)UnitCodeSlotSelect.END].z.value;
						return tmp;
					}
				}
				public double MG3_READY
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.MG3_READY;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG3, (int)UnitCodeSlotSelect.START].z.value;
						return tmp;
					}
				}
				public double MG3_END
				{
					get
					{
						double tmp;
						tmp = (double)MP_MG_Z.MG3_END;
						tmp += mc.para.UD.MagazinePos[(int)MG_NUM.MG3, (int)UnitCodeSlotSelect.END].z.value;
						return tmp;
					}
				}
            }
           
            public bool MagazineIsFull()
            {
                for (int i = 0; i < mc.UnloaderControl.MG_COUNT; i++)
                {
                    for (int j = 0; j < mc.para.UD.slotCount.value; j++)
                    {
                        if (mc.UnloaderControl.MG_Status[i, j] == (int)MG_STATUS.READY) 
                            return false;
                    }
                }
                return true;
            }

            public bool MagazineReadyPos(out int mg, out int slot)
            {
                mg = -1; slot = -1;
                for (int i = 0; i < mc.UnloaderControl.MG_COUNT; i++)
                {
                    for (int j = 0; j < (int)mc.para.UD.slotCount.value; j++)
                    {
                        if (mc.UnloaderControl.MG_Status[i, j] == (int)MG_STATUS.READY)
                        {
                            mg = i; slot = j;
                            return true;
                        }
                    }
                }
                return true;
            }
            #region AT_TARGET , AT_DONE
            public bool Z_AT_TARGET
            {
                get
                {
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                    if (ret.b)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Z.config.axisCode, ERRORCODE.MG, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                        return false;
                    }
                    Z.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                    if (ret.b)
                    {
                        if (dwell.Elapsed > 120000)
                        {
                            Z.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Z.config.axisCode, ERRORCODE.MG, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                        }
                        return false;
                    }
                    Z.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                    if (!ret.b)
                    {
                        if (dwell.Elapsed > 120000)
                        {
                            Z.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Z.config.axisCode, ERRORCODE.MG, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                        }
                        return false;
                    }
                    return true;
                }
            }
            public bool Z_AT_DONE
            {
                get
                {
                    Z.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                    if (ret.b)
                    {
                        Z.checkAlarmStatus(out ret.s, out ret.message);
                        errorCheck(Z.config.axisCode, ERRORCODE.MG, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                        return false;
                    }
                    Z.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Z.config.axisCode, sqc, ret.message)) return false;
                    if (!ret.b)
                    {
                        if (dwell.Elapsed > 1000)
                        {
                            Z.checkAlarmStatus(out ret.s, out ret.message);
                            errorCheck(Z.config.axisCode, ERRORCODE.MG, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                        }
                        return false;
                    }
                    return true;
                }
            }
            #endregion
        }
    }
}
