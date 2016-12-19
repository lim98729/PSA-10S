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
    public class classPusher : CONTROL
    {
        public mpiMotion X = new mpiMotion();
        public double init_X;
        public sensorHoming homingX = new sensorHoming();
        public classPusherPositionX pos = new classPusherPositionX();
        MPIState mpiState;
        public bool pusher_finish = false;
        public bool jamError = false;
        public bool isActivate
        {
            get
            {
                if (!X.isActivate) return false;
                if (!homingX.isActivate) return false;
                return true;
            }
        }
        public void activate(axisConfig x, out RetMessage retMessage)
        {
            if (!X.isActivate)
            {
                X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage, "", false)) return;
            }
            if (!homingX.isActivate)
            {
                homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage, "", false)) return;
            }
            retMessage = RetMessage.OK;
        }
        public void deactivate(out RetMessage retMessage)
        {
            X.deactivate(out retMessage);

            homingX.deactivate(out retMessage);
        }
        public void jogMove(double posX, out RetMessage retMessage)
        {
			pusher_finish = false;
            X.move(posX, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
            #region endcheck
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
 				//mc.IN.PS.JAM_IN(out ret.b2, out ret.message);
                //if (ret.b2) 
                //{
                //    X.stop(out ret.message);
                //    retMessage = RetMessage.JAM_SENSOR_DETECT_ERROR; 
                //    goto FAIL; 
                //}
				if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                X.AT_TARGET(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b) break;
            }
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                //mc.IN.PS.JAM_IN(out ret.b2, out ret.message);
                //if (ret.b2) 
                //{
                //    X.stop(out ret.message);
                //    retMessage = RetMessage.JAM_SENSOR_DETECT_ERROR; 
                //    goto FAIL;
                //}
                if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                X.AT_DONE(out ret.b, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
                if (ret.b) break;
            }
            return;
            #endregion
        FAIL:
            mc.init.success.PS = false;
            return;
        }
        public void motorDisable(out RetMessage retMessage)
        {
            mc.init.success.PS = false;
            X.motorEnable(false, out retMessage);
            X.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            return;
        }
        public void motorAbort(out RetMessage retMessage)
        {
            mc.init.success.PS = false;
            X.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            return;
        }

        public void control()
        {
            if (!req) return;
         
            switch (sqc)
            {
                case 0:
                    dwell.Reset();
                    Esqc = 0;
                    sqc++; break;
                case 1:
                    if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_PUSHER_NOT_READY, false); break; }
                    sqc++; break;
                case 2:
                    if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
                    if (reqMode == REQMODE.AUTO) { sqc = SQC.AUTO; break; }
                    if (reqMode == REQMODE.READY) { sqc = SQC.READY; break; }
                    errorCheck(ERRORCODE.INVALID, sqc, "unknown reqMode[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_PUSHER_LIST_NONE, false);
                    break;

                #region HOMING
                    
                case SQC.HOMING:
					pusher_finish = false;
                    if (dev.NotExistHW.ZMP) { mc.init.success.PS = true; sqc = SQC.STOP; break; }
                    X.N_LimitEventConfig(MPIAction.E_STOP, MPIPolarity.ActiveHigh, 0.001, out ret.message);
                    mc.IN.PS.READY(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    if (ret.b)
                    {
                        errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_BOAT_IN_SENSOR_DETECT, false	); break;
                    }
					sqc++; break;

                case SQC.HOMING + 1:
					mc.init.success.PS = false;
                    sqc++; break;

                case SQC.HOMING + 2 :
                    X.abort(out ret.message);
                    sqc++; break;
                    
                case SQC.HOMING + 3 :
                    mc.OUT.PS.PS_UPDOWN(false, out ret.message);
                    dwell.Reset();
                    sqc++; break;

                case SQC.HOMING + 4:
                    mc.IN.PS.DOWN(out ret.b, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (!ret.b) break;
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PS, sqc, "Pusher Down Sensor Error");
                        break;
                    }
                    homingX.req = true;
                    sqc++; break;

                case SQC.HOMING + 5:
                    if (homingX.RUNING) break;
                    if (homingX.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }           
                    X.move(pos.READY, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message, "", false)) break;
                    dwell.Reset();
                    sqc++; break;

                case SQC.HOMING + 6:
                    if (!X_AT_TARGET)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        if (ret.b)
                        {
                            jamError = true;
                            errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
                        }
                        break;
                    }
                    dwell.Reset(); sqc++; break;

                case SQC.HOMING + 7:
                    if (!X_AT_DONE)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        if (ret.b)
                        {
                            jamError = true;
                            errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
                        }
                        break;
                    }
                    sqc++; break;

                case SQC.HOMING + 8 :
                    mc.init.success.PS = true;
                    X.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveHigh, 0.001, out ret.message);
                    X.P_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveHigh, 0.001, out ret.message);
                    sqc = SQC.STOP; break;

                case SQC.HOMING_ERROR:
                    X.motorEnable(false, out ret.message);
					mc.init.success.PS = false;
                    sqc = SQC.ERROR; break;
                #endregion

                #region AUTO
                case SQC.AUTO:
                    pusher_finish = false;
                    mc.log.debug.write(mc.log.CODE.INFO, "Start Pusher");
                    sqc++; break;                    
                case SQC.AUTO + 1:
                    mc.OUT.PS.PS_UPDOWN(false, out ret.message);
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 2:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    mc.IN.PS.DOWN(out ret.b, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (!ret.b) break;
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PS, sqc, "Pusher Down Sensor Error");
                        break;
                    }

                    //if (homingX.RUNING) break;
                    //if (homingX.ERROR) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
                    X.move(pos.READY, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message, "", false)) break;
                    dwell.Reset();
					dwell2.Reset();
                    sqc++; break;
                case SQC.AUTO + 3 :
                    if (!X_AT_TARGET)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
                        break;
                    }
                    dwell.Reset(); 
					dwell2.Reset();
					sqc++; break;
                case SQC.AUTO + 4:
                    if (!X_AT_DONE)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
                        break;
                    }
                    dwell.Reset();
					sqc++; break;
                case SQC.AUTO + 5:
                    if (dwell.Elapsed < 15000)
                    {
                        mc.IN.PS.READY(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                        if (ret.b) break;
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_BOAT_IN_SENSOR_DETECT);
                        break;
                    }
                    dwell.Reset();
                    sqc++; break;
                    
                case SQC.AUTO + 6:
                    if (dwell.Elapsed < 100) break;
                    mc.OUT.PS.PS_UPDOWN(true, out ret.message);
                    dwell.Reset();
                    sqc++; break;

                case SQC.AUTO + 7:
					if (mc2.req == MC_REQ.STOP) { sqc = SQC.STOP; break; }
                    mc.IN.PS.UP(out ret.b, out ret.message);
                    if (dwell.Elapsed < 3000)
                    {
                        if (!ret.b) break;
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PS, sqc, "Pusher Down Sensor Error");
                        break;
                    }
                    X.move(mc.ps.pos.PUSH, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message, "", false)) break;   // move 는 이동하면서 sqc 넘어가고, jogmove 는 이동 완료후 sqc 넘어간다..
                    dwell.Reset();
					dwell2.Reset();
                    sqc++; break;

                case SQC.AUTO + 8:
                    if (!X_AT_TARGET)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
						break;
					}
                    dwell.Reset(); 
					dwell2.Reset();
					sqc++; break;

                case SQC.AUTO + 9:
                    if (!X_AT_DONE) 
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
						break;
                    }
                    dwell.Reset(); 
					sqc++; break;

                case SQC.AUTO + 10:
                    X.move(mc.ps.pos.PUSH - 2000, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message, "", false)) break;
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 11:
                    if (!X_AT_TARGET) break;
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 12:
                    if (!X_AT_DONE) break;
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 13:
                    mc.IN.PS.END(out ret.b, out ret.message);
                    if (ret.b)
                    {
                        errorCheck(ERRORCODE.PS, sqc, "Pusher Work End Senser Detected!");
                        break;
                    }
					pusher_finish = true;
					sqc = SQC.STOP; break;
				#endregion

                #region READY
                case SQC.READY:
					pusher_finish = false;
                    if (!mc.init.success.PS) { errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_SYSTEM_SW_PUSHER_NOT_READY); break; } 
					else sqc++; break;

				case SQC.READY + 1:
                    mc.OUT.PS.PS_UPDOWN(false, out ret.message);
                    dwell.Reset();
                    sqc++; break;

                case SQC.READY + 2:
                    if (dwell.Elapsed < 500) break;
					//mc.IN.PS.BOAT_IN(out ret.b, out ret.message); if(ioCheck(sqc, ret.message)) break;
					//if (ret.b) { errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_BOAT_IN_SENSOR_DETECT); break; }
                    X.move(pos.READY, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message, "", false)) break;
                    dwell.Reset();
					dwell2.Reset();
                    sqc++; break;

				case SQC.READY + 3:
                    if (!X_AT_TARGET) 
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
						break;
					}
                    dwell.Reset();
					dwell2.Reset();
					sqc++; break;

                case SQC.READY + 4:
                    if (!X_AT_DONE)
                    {
                        mc.IN.PS.JAM(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
						if (ret.b)
						{
							if (dwell2.Elapsed > 500)
							{
								jamError = true;
								errorCheck(ERRORCODE.PS, sqc, "", ALARM_CODE.E_PUSHER_IO_JAM_DETECT);
							}
							else break;
						}
						else dwell2.Reset();
						break;
					}
                    sqc = SQC.STOP; break;
                #endregion

                case SQC.STOP:
                    reqMode = REQMODE.AUTO;
                    req = false;
                    sqc = SQC.END; break;

                case SQC.ERROR:
					pusher_finish = false;
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Pusher Esqc {0}", Esqc));
                    if(jamError)
                    {
                        jamError = false;
                        motorAbort(out ret.message);
                    }
                    sqc = SQC.STOP; break;
            }
        }

        public class classPusherPositionX
        {
            public double READY
            {
                get
                {
                    double tmp;
					tmp = (double)MP_PS_X.HOME_SENSOR;
					//tmp += 2000;
                    return tmp;
                }
            }

            public double PUSH
            {
                get
                {
                    double tmp;
                    //tmp = (double)MP_PS_X.PUSH;
                    tmp = READY;
                    tmp += mc.para.UD.PusherPos.value;
                    return tmp;
                }
            }
        }
        bool X_AT_TARGET
        {
            get
            {
                X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    X.checkAlarmStatus(out ret.s, out ret.message);
                    //errorCheck(X.config.axisCode, ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    errorCheck(ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
                    return false;
                }
                X.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
                if (ret.b)
                {
                    if (dwell.Elapsed > 20000)
                    {
                        X.checkAlarmStatus(out ret.s, out ret.message);
                        //errorCheck(X.config.axisCode, ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                        errorCheck(ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
                    }
                    return false;
                }
                X.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 20000)
                    {
                        X.checkAlarmStatus(out ret.s, out ret.message);
                        //errorCheck(X.config.axisCode, ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                        errorCheck(ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
                    }
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
                if (ret.b)
                {
                    X.checkAlarmStatus(out ret.s, out ret.message);
                    //errorCheck(X.config.axisCode, ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    errorCheck(ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
                    return false;
                }
                X.AT_DONE(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
                if (!ret.b)
                {
                    if (dwell.Elapsed > 500)
                    {
                        X.checkAlarmStatus(out ret.s, out ret.message);
                        //errorCheck(X.config.axisCode, ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                        errorCheck(ERRORCODE.PS, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
                    }
                     return false;
                }
                return true;
            }
        }
    }
}
