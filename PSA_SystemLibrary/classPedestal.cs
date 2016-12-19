using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using MeiLibrary;
using DefineLibrary;

namespace PSA_SystemLibrary
{
	public class classPedestal : CONTROL
	{
        //public mpiMotion X = new mpiMotion();
        //public mpiMotion Y = new mpiMotion();
        //public mpiMotion W = new mpiMotion();

        //public double _X = new double();
        //public double _Y = new double();
        //public double _W = new double();

        //public sensorHoming homingX = new sensorHoming();
        //public sensorHoming homingY = new sensorHoming();
        //public sensorHoming homingW = new sensorHoming();

        //public classPedestalPosition pos = new classPedestalPosition();

        //public int jogMode;
        //MPIAction mpiAct;
        //MPIPolarity mpiPole;
        QueryTimer limitCheckTime = new QueryTimer();

		public bool isActivate
		{
			get
			{
                //if (!X.isActivate) return false;
                //if (!Y.isActivate) return false;
                //if (!W.isActivate) return false;

                //if (!homingX.isActivate) return false;
                //if (!homingY.isActivate) return false;
                //if (!homingW.isActivate) return false;

				return true;
			}
		}
        public void activate(out RetMessage retMessage)
        {
            retMessage = RetMessage.OK;
            return;
        }

        //public void activate(axisConfig x, axisConfig y, axisConfig w, out RetMessage retMessage)
        //{
        //    //if (!X.isActivate)
        //    //{
        //    //    X.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
        //    //}
        //    //if (!Y.isActivate)
        //    //{
        //    //    Y.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
        //    //}
        //    //if (!W.isActivate)
        //    //{
        //    //    W.activate(w, out retMessage); if (mpiCheck(UnitCodeAxis.W, 0, retMessage)) return;
        //    //}

        //    //if (!homingX.isActivate)
        //    //{
        //    //    homingX.activate(x, out retMessage); if (mpiCheck(UnitCodeAxis.X, 0, retMessage)) return;
        //    //}
        //    //if (!homingY.isActivate)
        //    //{
        //    //    homingY.activate(y, out retMessage); if (mpiCheck(UnitCodeAxis.Y, 0, retMessage)) return;
        //    //}
        //    //if (!homingW.isActivate)
        //    //{
        //    //    homingW.activate(w, out retMessage); if (mpiCheck(UnitCodeAxis.W, 0, retMessage)) return;
        //    //}

        //    retMessage = RetMessage.OK;
        //    return;
        //}
		public void deactivate(out RetMessage retMessage)
		{
            //X.deactivate(out retMessage);
            //Y.deactivate(out retMessage);
            //W.deactivate(out retMessage);


            //homingX.deactivate(out retMessage);
            //homingY.deactivate(out retMessage);
            //homingW.deactivate(out retMessage);
            retMessage = RetMessage.OK;
		}

        #region jogMove
        //public void jogMovePlus(double posX, double posY, out RetMessage retMessage, bool moveUp = true, bool checkSensor = true)
        //{
        //    if (checkSensor)
        //    {
        //        #region Z down
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
        //        #endregion

        //        #region Down Senser Check
        //        dwell.Reset();
        //        while (true)
        //        {
        //            mc.idle(10);
        //            if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //            if (ret.b1 && ret.b2) break;
        //        }
        //        #endregion
        //    }

        //    #region Move XY
        //    X.movePlus(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    Y.movePlus(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    #endregion

        //    if (checkSensor)
        //    {
        //        if (moveUp)
        //        {
        //            #region Z Up
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, true, out ret.message);
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, true, out ret.message);
        //            #endregion

        //            #region Up Senser Check
        //            dwell.Reset();
        //            while (true)
        //            {
        //                mc.idle(10);
        //                if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //                if (ret.b1 && ret.b2) break;
        //            }
        //            #endregion
        //        }
        //    }

        //    return;

        //FAIL:
        //    mc.init.success.PD = false;
        //    return;
        //}

        //public void jogMove(double posX, double posY, double posW, out RetMessage retMessage, bool moveUp = true, bool checkSensor = true)
        //{
        //    if (checkSensor)
        //    {
        //        #region Z down
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
        //        #endregion

        //        #region Down Senser Check
        //        dwell.Reset();
        //        while (true)
        //        {
        //            mc.idle(10);
        //            if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //            if (ret.b1 && ret.b2) break;
        //        }
        //        #endregion
        //    }

        //    #region Move XY
        //    X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    W.move(posW, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        W.AT_TARGET(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2 && ret.b3) break;
        //    }
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        W.AT_DONE(out ret.b3, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2 && ret.b3) break;
        //    }
        //    #endregion

        //    if (checkSensor)
        //    {
        //        if (moveUp)
        //        {
        //            #region Z Up
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, true, out ret.message);
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, true, out ret.message);
        //            #endregion

        //            #region Up Senser Check
        //            dwell.Reset();
        //            while (true)
        //            {
        //                mc.idle(10);
        //                if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //                if (ret.b1 && ret.b2) break;
        //            }
        //            #endregion
        //        }
        //    }

        //    return;

        //FAIL:
        //    mc.init.success.PD = false;
        //    return;
        //}

        //public void jogMove(double posX, double posY, out RetMessage retMessage, bool moveUp = true, bool checkSensor = true)
        //{
        //    if(checkSensor)
        //    {
        //        #region Z down
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
        //        #endregion

        //        #region Down Senser Check
        //        dwell.Reset();
        //        while (true)
        //        {
        //            mc.idle(10);
        //            if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //            if (ret.b1 && ret.b2) break;
        //        }
        //        #endregion
        //    }

        //    #region Move XY
        //    X.move(posX, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    Y.move(posY, mc.speed.slow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_TARGET(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        X.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        Y.AT_DONE(out ret.b2, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    #endregion

        //    if(checkSensor)
        //    {
        //        if (moveUp)
        //        {
        //            #region Z Up
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, true, out ret.message);
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, true, out ret.message);
        //            #endregion

        //            #region Up Senser Check
        //            dwell.Reset();
        //            while (true)
        //            {
        //                mc.idle(10);
        //                if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //                if (ret.b1 && ret.b2) break;
        //            }
        //            #endregion
        //        }
        //    }

        //    return;

        //FAIL:
        //    mc.init.success.PD = false;
        //    return;
        //}

        //public void jogMove(double posW, out RetMessage retMessage, bool moveUp = true, bool checkSensor = true)
        //{
        //    if(checkSensor)
        //    {
        //        #region Z down
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
        //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
        //        #endregion

        //        #region Down Senser Check
        //        dwell.Reset();
        //        while (true)
        //        {
        //            mc.idle(10);
        //            if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //            mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //            if (ret.b1 && ret.b2) break;
        //        }
        //        #endregion
        //    }

        //    #region Move W
        //    W.move(posW, mc.speed.homingSlow, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        W.AT_TARGET(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    dwell.Reset();
        //    while (true)
        //    {
        //        mc.idle(10);
        //        if (dwell.Elapsed > 500) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //        W.AT_DONE(out ret.b1, out retMessage); if (retMessage != RetMessage.OK) goto FAIL;
        //        if (ret.b1 && ret.b2) break;
        //    }
        //    #endregion

        //    if (checkSensor)
        //    {
        //        if (moveUp)
        //        {
        //            #region Z Up
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, true, out ret.message);
        //            mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, true, out ret.message);
        //            #endregion

        //            #region Up Senser Check
        //            dwell.Reset();
        //            while (true)
        //            {
        //                mc.idle(10);
        //                if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
        //                mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b2, out ret.message);
        //                if (ret.b1 && ret.b2) break;
        //            }
        //            #endregion
        //        }
        //    }

        //    return;

        //FAIL:
        //    mc.init.success.PD = false;
        //    return;
        //}
        #endregion

        public void PD_Mode(PedestalMode mode, out RetMessage retMessage)
        {
            if (mode == PedestalMode.PD_DOWN)
            {
                #region Z down
                mc.OUT.PD.UPDOWN(false, out retMessage);
                #endregion

                #region Down Senser Check
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out retMessage);
                    if (ret.b) break;
                }
                #endregion
            }
            else
            {
                #region Up
                mc.OUT.PD.UPDOWN(true, out retMessage);
                #endregion

                #region Up Senser Check
                dwell.Reset();
                while (true)
                {
                    mc.idle(10);
                    if (dwell.Elapsed > 20000) { retMessage = RetMessage.TIMEOUT; goto FAIL; }
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out retMessage);
                    if (ret.b) break;
                }
                #endregion
            }

            return;

        FAIL:
            mc.init.success.PD = false;
            return;
        }

		public void motorDisable(out RetMessage retMessage)
		{
            //mc.init.success.PD = false;
            //X.motorEnable(false, out retMessage);
            //Y.motorEnable(false, out retMessage);
            //W.motorEnable(false, out retMessage);

            //X.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            //Y.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;
            //W.motorEnable(false, out retMessage); if (retMessage != RetMessage.OK) return;

            mc.OUT.PD.UPDOWN(false, out ret.message);
            
            retMessage = RetMessage.OK;
		}

		public void motorAbort(out RetMessage retMessage)
		{
			mc.init.success.PD = false;
            //X.abort(out retMessage);
            //Y.abort(out retMessage);
            //W.abort(out retMessage);

            //X.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            //Y.abort(out retMessage); if (retMessage != RetMessage.OK) return;
            //W.abort(out retMessage); if (retMessage != RetMessage.OK) return;

            mc.OUT.PD.UPDOWN(false, out ret.message);

            retMessage = RetMessage.OK;
		}

		public void motorEnable(out RetMessage retMessage)
		{
            //X.reset(out retMessage); if (retMessage != RetMessage.OK) return;
            //Y.reset(out retMessage); if (retMessage != RetMessage.OK) return;
            //W.reset(out retMessage); if (retMessage != RetMessage.OK) return;

            //mc.idle(100);
            //X.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
            //Y.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;
            //W.clearPosition(out retMessage); if (retMessage != RetMessage.OK) return;

            //mc.idle(100);
            //X.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
            //Y.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;
            //W.motorEnable(true, out retMessage); if (retMessage != RetMessage.OK) return;

            mc.OUT.PD.UPDOWN(false, out ret.message);

            retMessage = RetMessage.OK;
			//mc.init.success.PD = true;
		}

        //public bool singleUp = false;

        //public int PDUpMode = (int)UnitCodePedestal.Dual_PD;
        //bool moveWidth = false;

		//double posX, posY;
		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_PEDESTAL_NOT_READY); break; }
					sqc++; break;
				case 2:
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					if (reqMode == REQMODE.AUTO || reqMode == REQMODE.DUMY) { sqc = SQC.AUTO; break; }
					if (reqMode == REQMODE.READY) { sqc = SQC.READY; break; }
					if (reqMode == REQMODE.COMPEN_FLAT) { sqc = SQC.COMPEN_FLAT; break; }
					errorCheck(ERRORCODE.PD, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_PEDESTAL_LIST_NONE); break;
                    
				#region HOMING
				case SQC.HOMING:
                    if (dev.NotExistHW.ZMP) { mc.init.success.PD = true; sqc = SQC.STOP; break; }
					mc.init.success.PD = false;
                    mc.OUT.PD.SUC(false, out ret.message);
					sqc++; break;
				case SQC.HOMING + 1:
                    mc.OUT.PD.UPDOWN(false, out ret.message);
                    if (ret.message != RetMessage.OK) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
                    sqc++; dwell.Reset();
                    break;
                case SQC.HOMING + 2:
                    if (dwell.Elapsed < 3000)
                    {
                        mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                        if(ret.message != RetMessage.OK) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
                        else if (ret.b)
                        {
                            mc.init.success.PD = true;
                            sqc = SQC.STOP; break;
                        }
                    }
                    else errorCheck(ERRORCODE.PD, sqc, "Pedestal (#1)", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                    break;

				case SQC.HOMING_ERROR:
                    //X.motorEnable(false, out ret.message);
                    //Y.motorEnable(false, out ret.message);
                    //W.motorEnable(false, out ret.message);
					sqc = SQC.ERROR; break;
				#endregion

                #region AUTO(XY사용)
                //case SQC.AUTO:
                //    moveWidth = false;
                //    X.P_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);
                //    Y.N_LimitEventConfig(MPIAction.NONE, MPIPolarity.ActiveLow, 0.001, out ret.message);
                //    sqc++; break;
                //case SQC.AUTO + 1:
                //    mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
                //    if (ret.message != RetMessage.OK) 
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); 
                //        break;
                //    }
                //    mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
                //    if (ret.message != RetMessage.OK) 
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); 
                //        break; 
                //    }

                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD2, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;

                //    dwell.Reset();
                //    sqc++; break;
                //case SQC.AUTO + 2:
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b, out ret.message);
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
                //    if(dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                dwell.Reset();
                //                sqc++; break;  
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT); 
                //        break;
                //    }
                //    break;
                //case SQC.AUTO + 3:
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b, out ret.message);
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b1, out ret.message);
                //    if (dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                sqc++; break;
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                //        break;
                //    }
                //    break;
                //case SQC.AUTO + 4:
                //    if ( PDUpMode == (int)UnitCodePedestal.Dual_PD || PDUpMode == (int)UnitCodePedestal.PD1)
                //    {
                //        mc.OUT.PD.SUC((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //        mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    }
                //    if (PDUpMode == (int)UnitCodePedestal.Dual_PD || PDUpMode == (int)UnitCodePedestal.PD2)
                //    {
                //        mc.OUT.PD.SUC((int)UnitCodePedestal.PD2, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //        mc.OUT.PD.BLW((int)UnitCodePedestal.PD2, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    }

                //    double posW = 0;
                //    W.actualPosition(out posW, out ret.message);
                //    if (Math.Abs(posW - pos.w.READY) > 1000) moveWidth = true;

                //    Y.move(pos.y.PAD(mc.hd.tool.padY), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                //    X.move(pos.x.PAD(mc.hd.tool.padX), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                //    if (moveWidth)
                //    {
                //        W.move(pos.w.READY, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
                //    }
                //    sqc++; break;
                //case SQC.AUTO + 5:
                //    if (moveWidth)
                //    {
                //        if (!X_AT_TARGET || !Y_AT_TARGET || !W_AT_TARGET) break;
                //    }
                //    else
                //    {
                //        if (!X_AT_TARGET || !Y_AT_TARGET) break;
                //    }
                //    sqc++; break;
                //case SQC.AUTO + 6:
                //    //if (mc.hd.reqMode != REQMODE.DUMY)
                //    //{
                //    //    mc.IN.CV.BD_CL1_ON(out ret.b1, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    //    mc.IN.CV.BD_CL2_ON(out ret.b2, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    //    mc.IN.CV.BD_STOPER_ON(out ret.b, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    //    if (!ret.b || !ret.b1 || !ret.b2) { errorCheck(ERRORCODE.PD, sqc, "Side Pusher와 Stoper가 준비되지 않아 Pedestal Up을 하지 못했습니다."); break; }
                //    //}
                //    sqc++; break;
                //case SQC.AUTO + 7:
                //    if (moveWidth)
                //    {
                //        if (!X_AT_DONE || !Y_AT_DONE || !W_AT_DONE) break;
                //    }
                //    else
                //    {
                //        if (!X_AT_DONE || !Y_AT_DONE) break;
                //    }
                    
                //    dwell.Reset();
                //    sqc++; break; 
                //case SQC.AUTO + 8:
                //    if ( PDUpMode == (int)UnitCodePedestal.PD2 ) { sqc += 2; break; }
                //    else
                //    {
                //        mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, true, out ret.message);
                //        if (ret.message != RetMessage.OK)
                //        { errorCheck(ERRORCODE.PD, sqc, "PD1 UP ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break; }
                //        dwell.Reset();
                //        sqc++; break;
                //    }
                //case SQC.AUTO + 9:
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b, out ret.message);
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
                //    if(dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                sqc++; break;
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "상승하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
                //    }
                //    break;
                //case SQC.AUTO + 10:
                //    if (PDUpMode == (int)UnitCodePedestal.PD1) { sqc = SQC.STOP; break; }
                //    mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, true, out ret.message);
                //    if (ret.message != RetMessage.OK) 
                //    { errorCheck(ERRORCODE.PD, sqc, "PD1 UP ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break; }
                //    dwell.Reset();
                //    sqc++; break;
                //case SQC.AUTO + 11:
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b, out ret.message);
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b1, out ret.message);
                //    if (dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                sqc = SQC.STOP; break;
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "상승하던 중 발생", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); break;
                //    }
                //    break;
                #endregion

                #region AUTO
                case SQC.AUTO:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
                    mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.UPDOWN(false, out ret.message);
                    if (ret.message != RetMessage.OK)
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 1:
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b1, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b && !ret.b1)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                dwell.Reset();
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    break;
                case SQC.AUTO + 2:
                    mc.OUT.PD.SUC(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.UPDOWN(true, out ret.message);
                    if (ret.message != RetMessage.OK)
                    { 
                        errorCheck(ERRORCODE.PD, sqc, "PD1 UP ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); 
                        break; 
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.AUTO + 3:
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b1, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b && !ret.b1)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                sqc = SQC.STOP; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT); 
                        break;
                    }
                    break;              
                #endregion

                case SQC.COMPEN_FLAT:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
                    mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.UPDOWN(false, out ret.message);
                    if (ret.message != RetMessage.OK)
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.COMPEN_FLAT + 1:
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b1, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b && !ret.b1)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                dwell.Reset();
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    break;
                case SQC.COMPEN_FLAT + 2:
                    mc.OUT.PD.UPDOWN(true, out ret.message);
                    if (ret.message != RetMessage.OK)
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 UP ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT);
                        break;
                    }
                    dwell.Reset();
                    sqc++; break;
                case SQC.COMPEN_FLAT + 3:
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b, out ret.message);
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b1, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b && !ret.b1)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                sqc = SQC.STOP; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_UP_TIMEOUT);
                        break;
                    }
                    break;    
                #region COMPEN FLATNESS
             

				#endregion

				#region READY(기존)
                //case SQC.READY:
                //    if (UtilityControl.simulation) { sqc = SQC.STOP; break; }

                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD2, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    
                //    sqc++; break;
                //case SQC.READY + 1:
                //    mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
                //    if (ret.message != RetMessage.OK) 
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                //        break;
                //    }
                //    mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
                //    if (ret.message != RetMessage.OK)
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                //        break;
                //    }
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD2, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    dwell.Reset();
                //    sqc++; break;
                //case SQC.READY + 2:
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b, out ret.message);
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD1, out ret.b1, out ret.message);
                //    if (dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                limitCheckTime.Reset();
                //                sqc++; break;
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                //        break;
                //    }
                //    break;
                //case SQC.READY + 3:
                //    mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b, out ret.message);
                //    mc.IN.PD.UP_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b1, out ret.message);
                //    if (dwell.Elapsed < 5000)
                //    {
                //        if (ret.b && !ret.b1)
                //        {
                //            if (limitCheckTime.Elapsed > 50)
                //            {
                //                sqc++; break;
                //            }
                //        }
                //        else
                //        {
                //            limitCheckTime.Reset();
                //        }
                //    }
                //    else
                //    {
                //        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                //        break;
                //    }
                //    break;

                ////case SQC.READY + 4:
                ////    if (dwell.Elapsed < 3000)
                ////    {
                ////        mc.IN.PD.DOWN_SENSOR_CHK((int)UnitCodePedestal.PD2, out ret.b, out ret.message);
                ////        if (ret.message != RetMessage.OK) { Esqc = sqc; sqc = SQC.HOMING_ERROR; break; }
                ////        else if (ret.b) sqc++;
                ////    }
                ////    else errorCheck(ERRORCODE.PD, sqc, "Pedestal (#2)", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                ////    break;
                //case SQC.READY + 4:
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.SUC((int)UnitCodePedestal.PD2, false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                //    mc.OUT.PD.BLW((int)UnitCodePedestal.PD1, false, out ret.message); if (ioCheck(sqc, ret.message)) break;

                //    double tmpPosW = 0;
                //    W.actualPosition(out posW, out ret.message);
                //    if (Math.Abs(tmpPosW - pos.w.READY) > 1000) moveWidth = true;

                //    Y.move(pos.y.PAD(0), out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) break;
                //    X.move(pos.x.PAD(0), out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) break;
                //    if (moveWidth)
                //    {
                //        W.move(pos.w.READY, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) break;
                //    }
                //    dwell.Reset();
                //    sqc++; break;
                //case SQC.READY + 5:
                //    if (moveWidth)
                //    {
                //        if (!X_AT_TARGET || !Y_AT_TARGET || !W_AT_TARGET) break;
                //    }
                //    else
                //    {
                //        if (!X_AT_TARGET || !Y_AT_TARGET) break;
                //    }
                //    dwell.Reset();
                //    sqc++; break;
                //case SQC.READY + 6:
                //    if (moveWidth)
                //    {
                //        if (!X_AT_DONE || !Y_AT_DONE || !W_AT_DONE) break;
                //    }
                //    else
                //    {
                //        if (!X_AT_DONE || !Y_AT_DONE) break;
                //    }
                //    sqc = SQC.STOP; break;

				#endregion

                #region READY
                case SQC.READY:
                    if (dev.NotExistHW.ZMP) { sqc = SQC.STOP; break; }
                    mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    sqc++; break;
                case SQC.READY + 1:
                    mc.OUT.PD.UPDOWN(false, out ret.message);
                    if (ret.message != RetMessage.OK)
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD2 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(true, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    dwell.Reset();
                    sqc++; break;
                case SQC.READY + 2:
                    mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                    mc.IN.PD.UP_SENSOR_CHK(out ret.b1, out ret.message);
                    if (dwell.Elapsed < 5000)
                    {
                        if (ret.b && !ret.b1)
                        {
                            if (limitCheckTime.Elapsed > 50)
                            {
                                limitCheckTime.Reset();
                                sqc++; break;
                            }
                        }
                        else
                        {
                            limitCheckTime.Reset();
                        }
                    }
                    else
                    {
                        errorCheck(ERRORCODE.PD, sqc, "PD1 DOWN ERROR", ALARM_CODE.E_MACHINE_RUN_PEDESTAL_DOWN_TIMEOUT);
                        break;
                    }
                    break;

                case SQC.READY + 3:
                    mc.OUT.PD.SUC(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    mc.OUT.PD.BLW(false, out ret.message); if (ioCheck(sqc, ret.message)) break;
                    sqc = SQC.STOP; break;

                #endregion

				case SQC.ERROR:
					string str = "PD Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("PD Esqc {0}", Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
                    //X.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.PD = false;
                    //Y.AT_IDLE(out ret.b, out ret.message); if (!ret.b || ret.message != RetMessage.OK) mc.init.success.PD = false;
                    //mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD1, false, out ret.message);
                    //mc.OUT.PD.UPDOWN((int)UnitCodePedestal.PD2, false, out ret.message);
					reqMode = REQMODE.AUTO;
					req = false;
					sqc = SQC.END; break;
			}


		}

		#region AT_TARGET , AT_DONE
        //bool X_AT_TARGET
        //{
        //    get
        //    {
        //        X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
        //        if(ret.b)
        //        {
        //            X.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
        //            return false;
        //        }
        //        X.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
        //        if (ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                X.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 20);
        //            return false;
        //        }
        //        X.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                X.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 20);
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //bool X_AT_DONE
        //{
        //    get
        //    {
        //        X.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
        //        if(ret.b)
        //        {
        //            X.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
        //            return false;
        //        }
        //        X.AT_DONE(out ret.b, out ret.message); if (mpiCheck(X.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 500)
        //            {
        //                X.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 0.5);
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //bool Y_AT_TARGET
        //{
        //    get
        //    {
        //        Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
        //        if(ret.b)
        //        {
        //            Y.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
        //            return false;
        //        }
        //        Y.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
        //        if (ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                Y.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.Y, sqc, 20);
        //            return false;
        //        }
        //        Y.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                Y.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.Y, sqc, 20);
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //bool Y_AT_DONE
        //{
        //    get
        //    {
        //        Y.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
        //        if(ret.b)
        //        {
        //            Y.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
        //            return false;
        //        }
        //        Y.AT_DONE(out ret.b, out ret.message); if (mpiCheck(Y.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 500)
        //            {
        //                Y.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_Y, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.Y, sqc, 0.5);
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //bool W_AT_TARGET
        //{
        //    get
        //    {
        //        W.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) return false;
        //        if (ret.b)
        //        {
        //            W.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_X, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_ERROR);
        //            return false;
        //        }
        //        W.AT_MOVING(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) return false;
        //        if (ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                W.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_W, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 20);
        //            return false;
        //        }
        //        W.AT_TARGET(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 20000)
        //            {
        //                W.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_W, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_TARGET_MOVE_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 20);
        //            return false;
        //        }
        //        return true;
        //    }
        //}
        //bool W_AT_DONE
        //{
        //    get
        //    {
        //        W.AT_ERROR(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) return false;
        //        if (ret.b)
        //        {
        //            W.checkAlarmStatus(out ret.s, out ret.message);
        //            errorCheck((int)UnitCodeAxisNumber.PD_W, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_ERROR);
        //            return false;
        //        }
        //        W.AT_DONE(out ret.b, out ret.message); if (mpiCheck(W.config.axisCode, sqc, ret.message)) return false;
        //        if (!ret.b)
        //        {
        //            if (dwell.Elapsed > 500)
        //            {
        //                W.checkAlarmStatus(out ret.s, out ret.message);
        //                errorCheck((int)UnitCodeAxisNumber.PD_W, ERRORCODE.PD, sqc, ret.s, ALARM_CODE.E_AXIS_CHECK_DONE_MOTION_TIMEOUT);
        //            }
        //            //timeCheck(UnitCodeAxis.X, sqc, 0.5);
        //            return false;
        //        }
        //        return true;
        //    }
        //}

		#endregion

        public void SetZDown()
        {
            bool fail = false;

            mc.OUT.PD.SUC(false, out ret.message);
            mc.idle(100);
            #region Z down
            mc.OUT.PD.UPDOWN(false, out ret.message);
            #endregion

            #region Down Senser Check
            dwell.Reset();
            while (true)
            {
                mc.idle(10);
                if (dwell.Elapsed > 20000) { fail = true; break; }
                mc.IN.PD.DOWN_SENSOR_CHK(out ret.b, out ret.message);
                if (ret.b) break;
            }
            #endregion
            if (fail) mc.init.success.PD = false;
        }
	}

	public class classPedestalPosition
	{
		public classPedestalPositionX x = new classPedestalPositionX();
		public classPedestalPositionY y = new classPedestalPositionY();
		public classPedestalPositionW w = new classPedestalPositionW();
	}
	public class classPedestalPositionX
	{
        //public double BD_EDGE
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = P1;
        //        tmp += 50000;
        //        tmp += mc.para.CAL.HDC_PD.x.value;
        //        return tmp;
        //    }
        //}
        //public double PAD(int column)
        //{
        //    double tmp;

        //    mc.pd.PDUpMode = (int)UnitCodePedestal.Dual_PD;

        //    double value = mc.para.MT.padCount.x.value;
        //    if (column + 1 < value / 2)
        //    {
        //        if(mc.swcontrol.useHead != 2) mc.pd.PDUpMode = (int)UnitCodePedestal.PD1;
        //        column = column + (int)Math.Ceiling(value / 2);
        //    }
        //    else if (mc.swcontrol.useHead != 2) mc.pd.PDUpMode = (int)UnitCodePedestal.PD2;

        //    if (column == Convert.ToInt32(value / 2) && mc.swcontrol.useHead == 2) mc.pd.PDUpMode = (int)UnitCodePedestal.PD2;

        //    tmp = BD_EDGE;
        //    if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //    {
        //        tmp -= mc.para.MT.edgeToPadCenter.x.value * 1000;
        //        if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
        //        tmp -= (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
        //    }
        //    if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //    {
        //        tmp += mc.para.MT.edgeToPadCenter.x.value * 1000;
        //        if (column < 0 || column >= mc.para.MT.padCount.x.value) return tmp;
        //        tmp += (mc.para.MT.padCount.x.value - column - 1) * mc.para.MT.padPitch.x.value * 1000;
        //    }
        //    return tmp;
        //}

        //public double READY
        //{
        //    get
        //    {
        //        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //        {
        //            return BD_EDGE - 30000;
        //        }
        //        if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //        {
        //            return BD_EDGE + 30000;
        //        }
        //        return -1;
        //    }
        //}
        //public double P1
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = mc.coor.MP.PD.X.HOME_SENSOR.value;
        //        tmp -= 50000;
        //        return tmp;
        //    }
        //}
        //public double P2
        //{
        //    get
        //    {
        //        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_FR.value - 30000;
        //        }
        //        if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_RR.value + 30000;
        //        }
        //        return -1;
        //    }
        //}
        //public double P3
        //{
        //    get
        //    {
        //        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_FR.value - 200000;
        //        }
        //        if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_RR.value + 200000;
        //        }
        //        return -1;
        //    }
        //}
        //public double P4
        //{
        //    get
        //    {
        //        if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_FR.value - 200000;
        //        }
        //        if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //        {
        //            return mc.coor.MP.PD.X.BD_EDGE_RR.value + 200000;
        //        }
        //        return -1;
        //    }
        //}
	}
	public class classPedestalPositionY
	{
        //public double BD_EDGE
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = P1;
        //        tmp -= 50000;
        //        tmp += mc.para.CAL.HDC_PD.y.value;
        //        return tmp;
        //    }
        //}
        //public double PAD(int row)
        //{
        //    double tmp;

        //    tmp = BD_EDGE;

        //    if (mc.para.mcType.FrRr == McTypeFrRr.FRONT)
        //    {
        //        tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
        //        if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
        //        tmp += row * mc.para.MT.padPitch.y.value * 1000;
        //    }
        //    if (mc.para.mcType.FrRr == McTypeFrRr.REAR)
        //    {
        //        tmp += mc.para.MT.edgeToPadCenter.y.value * 1000;
        //        if (row < 0 || row >= mc.para.MT.padCount.y.value) return tmp;
        //        tmp += (mc.para.MT.padCount.y.value - row - 1) * mc.para.MT.padPitch.y.value * 1000;
        //    }

        //    return tmp;
        //}
        //public double READY
        //{
        //    get
        //    {
        //        return BD_EDGE + 30000;
        //    }
        //}
        //public double P1
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = mc.coor.MP.PD.X.HOME_SENSOR.value;
        //        tmp += 50000;
        //        return tmp;
        //    }
        //}
        //public double P2
        //{
        //    get
        //    {
        //        return mc.coor.MP.PD.Y.BD_EDGE.value + 100000;
        //    }
        //}
        //public double P3
        //{
        //    get
        //    {
        //        return mc.coor.MP.PD.Y.BD_EDGE.value + 30000;
        //    }
        //}
        //public double P4
        //{
        //    get
        //    {
        //        return mc.coor.MP.PD.Y.BD_EDGE.value + 100000;
        //    }
        //}
	}
	public class classPedestalPositionW
	{
        //public double READY
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = mc.coor.MP.PD.W.READY.value;
        //        tmp += mc.para.CAL.HDC_PD.w.value;
        //        return tmp;
        //    }
        //}
        //public double HOME
        //{
        //    get
        //    {
        //        double tmp;
        //        tmp = mc.coor.MP.PD.W.HOME.value;
        //        return tmp;
        //    }
        //}
	}
}
