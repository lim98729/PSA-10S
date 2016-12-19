using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;

namespace PSA_Application
{
	public partial class CenterRight_Calibration : UserControl
	{
		public CenterRight_Calibration()
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
		#endregion

		RetValue ret;
		UnitCodeMachineRef unitCodeMachineRef = UnitCodeMachineRef.REF0;
        UnitCodeHead UnitCodeHD = UnitCodeHead.HD1;
        UnitCodeHead UnitCodeHDAngle = UnitCodeHead.HD1;
		UnitCodePDRef unitCodePDRef = UnitCodePDRef.P1;
		MP_HD_Z_MODE unitCodeZAxis = MP_HD_Z_MODE.REF;

		double posX, posY, posZ, posT, posT2, posW;
        int selectedHead = (int)UnitCodeHead.HD1;
		JOGMODE jogMode;

		private void Control_Click(object sender, EventArgs e)
		{

			#region BT_Origin_Offset
			if (sender.Equals(BT_Origin_Offset))
			{
				FormOriginOffset ff = new FormOriginOffset();
				DialogResult drst = ff.ShowDialog();
				if (drst == DialogResult.OK)
				{
					if (!mc.hd.tool.X.config.write()) { mc.message.alarm("Gantry X homing para write error"); }
					if (!mc.hd.tool.Y.config.write()) { mc.message.alarm("Gantry Y homing para write error"); }
                    for (int i = 0; i < mc.activate.headCnt; i++)
                    {
                        if (!mc.hd.tool.Z[i].config.write()) { mc.message.alarm("Gantry Z homing para write error"); }
                        if (!mc.hd.tool.T[i].config.write()) { mc.message.alarm("Gantry T homing para write error"); }
                    }

					if (!mc.sf.Z2.config.write()) { mc.message.alarm("Stack Feeder Z2 homing para write error"); }
					if (!mc.sf.Z.config.write()) { mc.message.alarm("Stack Feeder Z homing para write error"); }

					if (!mc.cv.W.config.write()) { mc.message.alarm("Conveyor W homing para write error"); }
				}
				return;
			}
			#endregion

			if (!mc.check.READY_AUTORUN(sender)) return;
			mc.check.push(sender, true);
			this.Enabled = false; 
			#region BT_MachineRef_Calibration
			if (sender.Equals(BT_MachineRef_Calibration))
			{
				#region moving
				if (unitCodeMachineRef == UnitCodeMachineRef.REF0) 
				{
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					jogMode = JOGMODE.HDC_REF0;
				}
				else if (unitCodeMachineRef == UnitCodeMachineRef.REF1_1) 
				{
					posX = mc.hd.tool.cPos.x.REF1_1;
					posY = mc.hd.tool.cPos.y.REF1_1;
					jogMode = JOGMODE.HDC_REF1_1;
				}
				else if (unitCodeMachineRef == UnitCodeMachineRef.REF1_2)
				{
					posX = mc.hd.tool.cPos.x.REF1_2;
					posY = mc.hd.tool.cPos.y.REF1_2;
					jogMode = JOGMODE.HDC_REF1_2;
				}
				else goto EXIT;

				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = jogMode;
				ff.dataX = mc.para.CAL.machineRef[(int)unitCodeMachineRef].x;
				ff.dataY = mc.para.CAL.machineRef[(int)unitCodeMachineRef].y;
				ff.ShowDialog();
				mc.para.CAL.machineRef[(int)unitCodeMachineRef].x.value = ff.dataX.value;
				mc.para.CAL.machineRef[(int)unitCodeMachineRef].y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_HDC_TOOL_Calibration
			if (sender.Equals(BT_HDC_TOOL_Calibration))
			{
				double tmpX, tmpY;
                tmpX = mc.para.CAL.ulc.x.value; mc.para.CAL.ulc.x.value = 0;
                tmpY = mc.para.CAL.ulc.y.value; mc.para.CAL.ulc.y.value = 0;
				#region moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHD].ULC;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHD].ULC;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHD].ULC_FOCUS;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHD].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHD, posX, posY, posZ, posT, out ret.message); 
                if (ret.message != RetMessage.OK)
				{
					mc.message.alarmMotion(ret.message);
					mc.para.CAL.ulc.x.value = tmpX;
					mc.para.CAL.ulc.y.value = tmpY; goto EXIT;
				}
				#endregion
				FormJogPad ff = new FormJogPad();
                ff.jogMode = JOGMODE.ULC_TOOL;
                ff.selectedTool = (int)UnitCodeHD;
				ff.dataX = mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].x;
				ff.dataY = mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].y;
				ff.ShowDialog();
                mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].x.value = ff.dataX.value;
                mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                posT2 = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD2].ZERO;
                mc.hd.tool.jogMove(posX, posY, posT, posT2, out ret.message);
                if (ret.message != RetMessage.OK)
				{
					mc.message.alarmMotion(ret.message); 
					mc.para.CAL.ulc.x.value = tmpX;
					mc.para.CAL.ulc.y.value = tmpY; goto EXIT;
				}
				#endregion
				mc.para.CAL.ulc.x.value = tmpX;
				mc.para.CAL.ulc.y.value = tmpY;
			}
			#endregion
			#region BT_HDC_LASER_Calibration
			if (sender.Equals(BT_HDC_LASER_Calibration))
			{
				// Laser가 ON 상태일 경우 Up Looking Camera가 damage를 얻을 수 있으므로 Laser를 Off한 상태로 봐야 한다.
				mc.OUT.HD.LS.ON(false, out ret.message);
				#region moving
				posX = mc.hd.tool.lPos.x.ULC;
				posY = mc.hd.tool.lPos.y.ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_LASER;
				ff.dataX = mc.para.CAL.HDC_LASER.x;
				ff.dataY = mc.para.CAL.HDC_LASER.y;
				ff.ShowDialog();
				mc.para.CAL.HDC_LASER.x.value = ff.dataX.value;
				mc.para.CAL.HDC_LASER.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_HDC_PD_Calibration
			if (sender.Equals(BT_HDC_PD_Calibration))
			{
				#region HD moving
				if (unitCodePDRef == UnitCodePDRef.P1)
				{
                    posX = mc.hd.tool.cPos.x.HDC_PD_P1;
                    posY = mc.hd.tool.cPos.y.HDC_PD_P1;
					jogMode = JOGMODE.HDC_PD_P1;
				}
				else if (unitCodePDRef == UnitCodePDRef.P2)
				{
                    posX = mc.hd.tool.cPos.x.PAD(0);
                    posY = mc.hd.tool.cPos.y.PAD(0);
					jogMode = JOGMODE.HDC_PD_P2;
				}
				else goto EXIT;

				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				#region PD moving
				if (unitCodePDRef == UnitCodePDRef.P1)
				{
                    //mc.pd.PD_Mode(PedestalMode.PD_UP, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

                    //FormJogPad ff = new FormJogPad();
                    //ff.jogMode = jogMode;
                    //ff.dataX = mc.para.CAL.HDC_PD.x;
                    //ff.dataY = mc.para.CAL.HDC_PD.y;
                    //ff.ShowDialog();
                    //#region PD moving
                    //mc.pd.PD_Mode(PedestalMode.PD_DOWN, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    //#endregion
                    //mc.para.CAL.HDC_PD.x.value = ff.dataX.value;
                    //mc.para.CAL.HDC_PD.y.value = ff.dataY.value;
				}
				else if (unitCodePDRef == UnitCodePDRef.P2)
				{
                    //posX = mc.pd.pos.x.PAD(0);
                    //posY = mc.pd.pos.y.PAD(0);
                    //posW = mc.pd.pos.w.READY;
                    //mc.pd.jogMove(posX, posY, posW, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

                    //FormJogPad ff = new FormJogPad();
                    //ff.dataX = mc.para.CAL.HDC_PD.w;
                    //ff.jogMode = jogMode;
                    //ff.ShowDialog();
                    //#region PD moving
                    //mc.pd.jogMove(posX, posY, posW, out ret.message, false); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    //#endregion
                    //mc.para.CAL.HDC_PD.w.value = ff.dataX.value;
				}
				else goto EXIT;

				#endregion

				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_TouchProve_Calibration
			if (sender.Equals(BT_TouchProve_Calibration))
			{
				#region moving
				posX = mc.hd.tool.cPos.x.TOUCHPROBE;
				posY = mc.hd.tool.cPos.y.TOUCHPROBE;
                //posX = mc.coor.MP.TOOL.X.CAMERA.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
                //posX += mc.coor.MP.HD.X.TOUCHPROBE.value + ff.dataX.value;
                //posY = mc.coor.MP.TOOL.Y.CAMERA.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
                //posY += mc.coor.MP.HD.Y.TOUCHPROBE.value + ff.dataY.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_TOUCHPROBE;
				ff.dataX = mc.para.CAL.touchProbe.x;
				ff.dataY = mc.para.CAL.touchProbe.y;

				ff.ShowDialog();

				mc.para.CAL.touchProbe.x.value = ff.dataX.value;
				mc.para.CAL.touchProbe.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_LoadCell_Calibration
			if (sender.Equals(BT_LoadCell_Calibration))
			{
				#region moving
				posX = mc.hd.tool.cPos.x.LOADCELL;
				posY = mc.hd.tool.cPos.y.LOADCELL;
                //posX = mc.coor.MP.TOOL.X.CAMERA.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].x.value;
                //posX += mc.coor.MP.HD.X.LOADCELL.value + ff.dataX.value;
                //posY = mc.coor.MP.TOOL.Y.CAMERA.value + mc.para.CAL.machineRef[(int)UnitCodeMachineRef.REF0].y.value;
                //posY += mc.coor.MP.HD.Y.LOADCELL.value + ff.dataY.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_LOADCELL;
				ff.dataX = mc.para.CAL.loadCell.x;
				ff.dataY = mc.para.CAL.loadCell.y;

				ff.ShowDialog();

				mc.para.CAL.loadCell.x.value = ff.dataX.value;
				mc.para.CAL.loadCell.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_Pick_Calibration
			if (sender.Equals(BT_Pick_Calibration))
			{
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_PICK_INIT_OFFSET_XYZ);
				//mc.message.OkCancel("모든 Pick Offset X,Y,Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
				if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
				#region mc.para.HD.pick.offset clear
				for (int i = 0; i < 8; i++)
				{
					mc.para.HD.pick.offset[i].x.value = 0;
					mc.para.HD.pick.offset[i].y.value = 0;
					mc.para.HD.pick.offset[i].z.value = 0;
				}
				#endregion
				#region moving
				posX = mc.hd.tool.cPos.x.PICK(UnitCodeSF.SF1);
				posY = mc.hd.tool.cPos.y.PICK(UnitCodeSF.SF1);
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_PICK;
				ff.dataX = mc.para.CAL.pick.x;
				ff.dataY = mc.para.CAL.pick.y;
				ff.ShowDialog();
				mc.para.CAL.pick.x.value = ff.dataX.value;
				mc.para.CAL.pick.y.value = ff.dataY.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion
			#region BT_ULC_Offset_Calibration
			if (sender.Equals(BT_ULC_Offset_Calibration))
			{
				#region moving ready
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC - 70000;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.CALJIG], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.CALJIG]);
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CIRCLE_CENTER;
				
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_CAL_INSERT_JIG);
				mc.ulc.LIVE = false;
                if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;

				#region moving
				posX = mc.hd.tool.cPos.x.ULC;
				posY = mc.hd.tool.cPos.y.ULC;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC;
                ff.dataX = mc.para.CAL.ulc.x;
                ff.dataY = mc.para.CAL.ulc.y;

				ff.ShowDialog();

				mc.para.CAL.ulc.x.value = ff.dataX.value;
				mc.para.CAL.ulc.y.value = ff.dataY.value;

				#region moving ready
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_REMOVE_JIG);
            }
			#endregion
			#region BT_ConveyorEdge_Calibration
			if (sender.Equals(BT_ConveyorEdge_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_BD_EDGE;
				ff.dataX = mc.para.CAL.conveyorEdge.x;
				ff.dataY = mc.para.CAL.conveyorEdge.y;

				mc.OUT.CV.BD_STOP(true, out ret.message);
				mc.OUT.CV.SIDE_PUSHER(false, out  ret.message);

				#region moving
				posX = mc.hd.tool.cPos.x.BD_EDGE;
				posY = mc.hd.tool.cPos.y.BD_EDGE;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion

                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_INSERT_TRAY);
				mc.OUT.CV.FD_MTR2(true, out ret.message);
				mc.AOUT.CV.FD_MTR2(255, out ret.message);
				int tmp = 0;
				while (true)
				{
					mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
					if (ret.b) break;
					tmp++; mc.idle(1);
					if (tmp > 3000) break;
				}
				mc.idle(500);
				mc.OUT.CV.SIDE_PUSHER(true, out ret.message);
				mc.idle(500);
				mc.OUT.CV.FD_MTR2(false, out ret.message);
				ff.ShowDialog();
				mc.OUT.CV.BD_STOP(true, out ret.message);
				mc.OUT.CV.SIDE_PUSHER(false, out ret.message);

				mc.para.CAL.conveyorEdge.x.value = ff.dataX.value;
				mc.para.CAL.conveyorEdge.y.value = ff.dataY.value;
			트레이제거:
                ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, textResource.MB_CAL_REMOVE_TRAY);
                mc.IN.CV.BD_NEAR(out ret.b, out ret.message);
				if (ret.b) goto 트레이제거;
				#region moving ready
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Tool_AngleOffset_Calibration
			if (sender.Equals(BT_Tool_AngleOffset_Calibration))
			{
                //FormJogPad ff = new FormJogPad();
                //ff.jogMode = JOGMODE.TOOL_ANGLEOFFSET;
                //#region moving and motorDisable
                //posX = mc.coor.MP.HD.X.ULC.value;
                //posY = mc.coor.MP.HD.Y.BD_EDGE.value + 30000;
                //posZ = mc.coor.MP.HD.Z.PEDESTAL.value;
                //posT = 0;
                //mc.hd.tool.jogMove((int)UnitCodeHDAngle, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                //mc.idle(1000);
                //mc.hd.tool.motorDisable(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                //#endregion
                //ff.selectedTool = (int)UnitCodeHDAngle;
                //ff.ShowDialog();
                //mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle].value = ff.dataX.value;
                //#region moving ready
                //mc.hd.tool.motorEnable(out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

                #region moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHDAngle].ULC;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHDAngle].ULC;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHDAngle].ULC_FOCUS;
                double a = 0;
                mc.hd.tool.T[0].actualPosition(out a, out ret.message);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHDAngle].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHDAngle, posX, posY, posZ, posT, out ret.message);
                if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
                FormJogPad ff = new FormJogPad();
                ff.jogMode = JOGMODE.TOOL_ANGLEOFFSET;
                ff.selectedTool = (int)UnitCodeHDAngle;
                ff.dataX = mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle];
                ff.ShowDialog();
                mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle].value = ff.dataX.value;
                #region moving

                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHDAngle].XY_MOVING;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHDAngle].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHDAngle, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Z_Axis_Calibration
			if (sender.Equals(BT_Z_Axis_Calibration))
			{
                if (unitCodeZAxis != MP_HD_Z_MODE.HEIGHT_OFFSET)
                {
                    ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.HD1HD2Cancel, DIAG_ICON_MODE.QUESTION, "Select Head");
                    if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
                    else if (ret.usrDialog == DIAG_RESULT.HD1) selectedHead = (int)UnitCodeHead.HD1;
                    else if (ret.usrDialog == DIAG_RESULT.HD2) selectedHead = (int)UnitCodeHead.HD2;
                }

				#region REF
				if (unitCodeZAxis == MP_HD_Z_MODE.REF)
				{
					#region moving
                    posX = mc.hd.tool.tPos.x[selectedHead].REF0;
                    posY = mc.hd.tool.tPos.y[selectedHead].REF0;
                    posZ = mc.hd.tool.tPos.z[selectedHead].REF0;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
                    mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(1000);
					#endregion
					mc.para.HD.place.offset.z.value = 0; // 20140628 이 값이 VPPM Range를 키워주는 역할을 하므로 이 값을 Clear하지 않는다. Recipe에 사용된다.
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.ref0;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.para.CAL.z.ref0.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
					posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region ULC_FOCUS
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS)
				{
					#region moving
                    posX = mc.hd.tool.tPos.x[selectedHead].ULC;
                    posY = mc.hd.tool.tPos.y[selectedHead].ULC;
                    posZ = mc.hd.tool.tPos.z[selectedHead].ULC_FOCUS;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
                    mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
					mc.ulc.lighting_exposure(mc.para.ULC.model.light, mc.para.ULC.model.exposureTime);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.ulcFocus;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.ulc.LIVE = false;
					mc.para.CAL.z.ulcFocus.value = ff.dataZ.value;
					EVENT.hWindow2Display();
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region XY_MOVING
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING)
				{
					#region moving
                    posX = mc.hd.tool.tPos.x[selectedHead].ULC;
                    posY = mc.hd.tool.tPos.y[selectedHead].BD_EDGE - 7000;
                    posZ = mc.hd.tool.tPos.z[selectedHead].XY_MOVING;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
                    mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.xyMoving;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.para.CAL.z.xyMoving.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PICK
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK)
				{
                    ret.usrDialog = FormMain.UserMessageBox(DIAG_SEL_MODE.OKCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_HD_PICK_INIT_OFFSET_Z);
					//mc.message.OkCancel("모든 Pick Offset Z 값은 초기화 됩니다. 계속 진행할까요?", out ret.dialog);
					if (ret.usrDialog == DIAG_RESULT.Cancel) goto EXIT;
                    int pickAxis = selectedHead;
					#region moving
                    posX = mc.hd.tool.tPos.x[pickAxis].PICK(UnitCodeSF.SF1);
                    posY = mc.hd.tool.tPos.y[pickAxis].PICK(UnitCodeSF.SF1);
                    posZ = mc.hd.tool.tPos.z[pickAxis].PICK(UnitCodeSF.SF1);
                    posT = mc.para.CAL.toolAngleOffset[pickAxis].value;
                    mc.hd.tool.jogMove(pickAxis, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                    mc.OUT.HD.SUC(pickAxis, true, out ret.message);
                    for (int i = 0; i < 8; i++) mc.para.HD.pick.offset[i].z.value = 0;
                    FormJogPadZ ff = new FormJogPadZ();
                    ff.mode = unitCodeZAxis;
                    ff.dataZ = mc.para.CAL.z.pick;
                    ff.calHead = selectedHead;
                    ff.ShowDialog();
                    mc.para.CAL.z.pick.value = ff.dataZ.value;
                    mc.OUT.HD.SUC(pickAxis, false, out ret.message);
                    mc.OUT.HD.BLW(pickAxis, true, out ret.message); mc.idle(30);
                    mc.OUT.HD.BLW(pickAxis, false, out ret.message);
                    mc.idle(100);
                    #region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
                    mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PEDESTAL
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL)
				{
					#region moving
                    mc.pd.PD_Mode(PedestalMode.PD_UP, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    posX = mc.hd.tool.tPos.x[selectedHead].PAD(0);
                    posY = mc.hd.tool.tPos.y[selectedHead].PAD(0);
                    posZ = mc.hd.tool.tPos.z[selectedHead].PEDESTAL;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
                    mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.para.HD.place.offset.z.value = 0; // 20140628 이 값이 VPPM Range를 키워주는 역할을 하므로 이 값을 Clear하지 않는다. Recipe에 사용된다.
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.pedestal;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.para.CAL.z.pedestal.value = ff.dataZ.value;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    mc.pd.PD_Mode(PedestalMode.PD_DOWN, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region TOUCHPROBE
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE)
				{
					//mc.touchProbe.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Touch Probe Set Zero Error"); goto EXIT; }
					#region moving
                    posX = mc.hd.tool.tPos.x[selectedHead].TOUCHPROBE;
                    posY = mc.hd.tool.tPos.y[selectedHead].TOUCHPROBE;
                    posZ = mc.hd.tool.tPos.z[selectedHead].TOUCHPROBE;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
					mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.touchProbe;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.para.CAL.z.touchProbe.value = ff.dataZ.value;
                    double value = 0;
                    double pos = 0;
                    mc.touchProbe.getData(out value, out ret.b);
                    if (ret.b)
                    {
                        mc.para.CAL.probeValue.value = value;
                        mc.hd.tool.Z[selectedHead].actualPosition(out pos, out ret.message);
                        mc.para.CAL.probePosZ.value = pos;
                    }
                    
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LOADCELL
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL)
				{
					//mc.loadCell.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Load Cell Set Zero Error"); goto EXIT; } 20141012
					#region moving
                    posX = mc.hd.tool.tPos.x[selectedHead].LOADCELL;
                    posY = mc.hd.tool.tPos.y[selectedHead].LOADCELL;
                    posZ = mc.hd.tool.tPos.z[selectedHead].LOADCELL;
                    posT = mc.para.CAL.toolAngleOffset[selectedHead].value;
                    mc.hd.tool.jogMove(selectedHead, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
					ff.dataZ = mc.para.CAL.z.loadCell;
                    ff.calHead = selectedHead;
					ff.ShowDialog();
					mc.para.CAL.z.loadCell.value = ff.dataZ.value;
                    //mc.hd.tool.Z[selectedHead].actualPosition(out posValue, out ret.message);
                    //mc.para.CAL.z.loadCellPos.value = posValue;
					mc.idle(100);
					#region moving
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region HeightOffset
				if (unitCodeZAxis == MP_HD_Z_MODE.HEIGHT_OFFSET)
				{
					//mc.loadCell.setZero(out ret.b); if (ret.b == false) { mc.message.alarm("Load Cell Set Zero Error"); goto EXIT; } 20141012
                    #region moving
                    posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LOADCELL;
                    posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LOADCELL;
                    posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].XY_MOVING;
                    posT = mc.para.CAL.toolAngleOffset[(int)UnitCodeHead.HD1].value;
                    mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion

                    FormJogPadZ ff = new FormJogPadZ();
					ff.mode = unitCodeZAxis;
                    ff.dataZ = mc.para.CAL.z.heightOffset[(int)UnitCodeHead.HD2];
					ff.ShowDialog();
                    mc.para.CAL.z.heightOffset[(int)UnitCodeHead.HD2].value = ff.dataZ.value;
					mc.idle(100);
					#region move to standby position
                    posX = mc.para.CAL.standbyPosition.x.value;
                    posY = mc.para.CAL.standbyPosition.y.value;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
			}
			#endregion

			#region BT_HDC_Calibration
			if (sender.Equals(BT_HDC_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_CALIBRATION;
				ff.dataX = mc.para.CAL.HDC_Resolution.x;
				ff.dataY = mc.para.CAL.HDC_Resolution.y;

                #region moving
					posX = mc.hd.tool.cPos.x.REF0;
					posY = mc.hd.tool.cPos.y.REF0;
					mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				ff.ShowDialog();

                mc.para.CAL.HDC_Resolution.x.value = ff.dataX.value;
				mc.para.CAL.HDC_Resolution.y.value = ff.dataY.value;
				mc.hdc.cam.acq.ResolutionX = mc.para.CAL.HDC_Resolution.x.value;
				mc.hdc.cam.acq.ResolutionY = mc.para.CAL.HDC_Resolution.y.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_HDC_Angle_Calibration
			if (sender.Equals(BT_HDC_Angle_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.HDC_ANGLE_CALIBRATION;
				ff.dataX = mc.para.CAL.HDC_AngleOffset;
				#region moving
				posX = mc.hd.tool.cPos.x.REF0;
				posY = mc.hd.tool.cPos.y.REF0;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.HDC_AngleOffset.value = ff.dataX.value;
				mc.hdc.cam.acq.AngleOffset = mc.para.CAL.HDC_AngleOffset.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_ULC_Calibration
			if (sender.Equals(BT_ULC_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_CALIBRATION;
				ff.dataX = mc.para.CAL.ULC_Resolution.x;
				ff.dataY = mc.para.CAL.ULC_Resolution.y;
				#region moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC;
				posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS;
                posT = mc.para.CAL.toolAngleOffset[(int)UnitCodeHead.HD1].value;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.ULC_Resolution.x.value = ff.dataX.value;
				mc.para.CAL.ULC_Resolution.y.value = ff.dataY.value;
				mc.ulc.cam.acq.ResolutionX = mc.para.CAL.ULC_Resolution.x.value;
				mc.ulc.cam.acq.ResolutionY = mc.para.CAL.ULC_Resolution.y.value;

				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_ULC_Angle_Calibration
			if (sender.Equals(BT_ULC_Angle_Calibration))
			{
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.ULC_ANGLE_CALIBRATION;
				ff.dataX = mc.para.CAL.ULC_AngleOffset;
				#region moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS;
                posT = mc.para.CAL.toolAngleOffset[(int)UnitCodeHead.HD1].value;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();

				mc.para.CAL.ULC_AngleOffset.value = ff.dataX.value;
				mc.ulc.cam.acq.AngleOffset = mc.para.CAL.ULC_AngleOffset.value;
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Flatness_Calibration
			if (sender.Equals(BT_Flatness_Calibration))
			{
				FormFlatnessCalibration ff = new FormFlatnessCalibration();
				#region xy moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].TOUCHPROBE;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].TOUCHPROBE;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				ff.ShowDialog();
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_Pedestal_Flatness_Calibration
			if (sender.Equals(BT_Pedestal_Flatness_Calibration))
			{
				#region moving
                mc.pd.PD_Mode(PedestalMode.PD_UP, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				posX = mc.hd.tool.lPos.x.PAD(0);
				posY = mc.hd.tool.lPos.y.PAD(0);
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				#endregion

				PedestalFlatness ff = new PedestalFlatness(0, 0);

				ff.ShowDialog();

                #region moving
                mc.pd.PD_Mode(PedestalMode.PD_DOWN, out ret.message);
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
			}
			#endregion

			#region BT_Force_Calibration
			if (sender.Equals(BT_Force_Calibration))
			{
				FormForceCalibration ff = new FormForceCalibration();
				#region xy moving
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LOADCELL;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LOADCELL;
				//posX = mc.hd.tool.tPos.x.REF0;
				//posY = mc.hd.tool.tPos.y.REF0;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				//#region z moving
				//posZ = mc.hd.tool.tPos.z.LOADCELL - mc.para.CAL.force.touchOffset.value;
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL + 1000, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(mc.hd.tool.tPos.z.LOADCELL, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//mc.idle(100);
				//mc.hd.tool.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
				//#endregion
				ff.ShowDialog();
				#region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_LoadcellForce_Calibration
			if (sender.Equals(BT_LoadcellForce_Calibration))
			{
				FormLoadcellCalib ff = new FormLoadcellCalib();

				ff.ShowDialog();

                #region moving
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
                mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                #endregion
			}
			#endregion

			#region BT_ConveyorWidthOffset_Calibration
			if (sender.Equals(BT_ConveyorWidthOffset_Calibration))
			{
				#region moving
				mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.CV_WIDTH_OFFSET;
				ff.dataY = mc.para.CAL.conveyorWidthOffset;
				ff.ShowDialog();
				mc.para.CAL.conveyorWidthOffset.value = ff.dataY.value;
				#region moving
				mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT_PlaceOffset_Calibration
			if (sender.Equals(BT_PlaceOffset_Calibration))
			{
				FormPlaceOffsetCalibration ff = new FormPlaceOffsetCalibration();
				ff.ShowDialog();
				#region moving
                mc.pd.PD_Mode(PedestalMode.PD_DOWN, out ret.message);
                posX = mc.para.CAL.standbyPosition.x.value;
                posY = mc.para.CAL.standbyPosition.y.value;
				mc.hd.tool.jogMove(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
			}
			#endregion

			#region BT Head Loadcell Calibration
			//if (sender.Equals(BT_Head_Loadcell_Calibration))		// Loadcell calibration 화면을 새로 만들려고 했으나, FormJogZ에 이 기능을 삽입함. 현재는 의미없음.
			//{
			//    FormLoadcellCalib ff = new FormLoadcellCalib();
			//    ff.Show();
			//    ff.BringToFront();
			//}
			#endregion

			#region Stand-By Position
			if (sender.Equals(BT_STANDBYPOS_CALIBRATION))
			{
				mc.hd.tool.jogMove(mc.para.CAL.standbyPosition.x.value, mc.para.CAL.standbyPosition.y.value, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }

				FormJogPad ff = new FormJogPad();
				ff.jogMode = JOGMODE.STANDBY_POSITION;
				ff.dataX = mc.para.CAL.standbyPosition.x;
				ff.dataY = mc.para.CAL.standbyPosition.y;
				ff.ShowDialog();
				mc.para.setting(ref mc.para.CAL.standbyPosition.x, ff.dataX.value);
				mc.para.setting(ref mc.para.CAL.standbyPosition.y, ff.dataY.value);
			}
			#endregion

		EXIT:
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
			mc.hdc.lighting_exposure(mc.para.HDC.light[(int)LIGHTMODE_HDC.OFF], mc.para.HDC.exposure[(int)LIGHTMODE_HDC.OFF]);		// 동작이 끝난 후 조명을 끈다.
			mc.ulc.lighting_exposure(mc.para.ULC.light[(int)LIGHTMODE_ULC.OFF], mc.para.ULC.exposure[(int)LIGHTMODE_ULC.OFF]);
			this.Enabled = true;
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
				BT_MachineRefSelect.Text = unitCodeMachineRef.ToString();

				TB_MachineRef_OffsetX.Text = mc.para.CAL.machineRef[(int)unitCodeMachineRef].x.value.ToString();
				TB_MachineRef_OffsetY.Text = mc.para.CAL.machineRef[(int)unitCodeMachineRef].y.value.ToString();

                TB_HDC_TOOL_OffsetX.Text = mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].x.value.ToString();
                TB_HDC_TOOL_OffsetY.Text = mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].y.value.ToString();

				TB_HDC_LASER_OffsetX.Text = mc.para.CAL.HDC_LASER.x.value.ToString();
				TB_HDC_LASER_OffsetY.Text = mc.para.CAL.HDC_LASER.y.value.ToString();

                if (unitCodePDRef == UnitCodePDRef.P1)
                {
                    TB_HDC_PD_OffsetX.Text = mc.para.CAL.HDC_PD.x.value.ToString();
                    TB_HDC_PD_OffsetY.Text = mc.para.CAL.HDC_PD.y.value.ToString();
                    BT_HDC_PDSelelct.Text = "MAIN";
                    TB_HDC_PD_OffsetY.Visible = true;
                }
                else
                {
                    TB_HDC_PD_OffsetX.Text = mc.para.CAL.HDC_PD.w.value.ToString();
                    TB_HDC_PD_OffsetY.Text = "0".ToString();
                    BT_HDC_PDSelelct.Text = "WIDTH";
                    TB_HDC_PD_OffsetY.Visible = false;
                }

                BT_ToolSelect.Text = UnitCodeHD.ToString();
                BT_AngleSelect.Text = UnitCodeHDAngle.ToString();
                
				TB_TouchProve_OffsetX.Text = mc.para.CAL.touchProbe.x.value.ToString();
				TB_TouchProve_OffsetY.Text = mc.para.CAL.touchProbe.y.value.ToString();

				TB_LoadCell_OffsetX.Text = mc.para.CAL.loadCell.x.value.ToString();
				TB_LoadCell_OffsetY.Text = mc.para.CAL.loadCell.y.value.ToString();

				TB_ULC_OffsetX.Text = mc.para.CAL.ulc.x.value.ToString();
				TB_ULC_OffsetY.Text = mc.para.CAL.ulc.y.value.ToString();

				TB_Pick_OffsetX.Text = mc.para.CAL.pick.x.value.ToString();
				TB_Pick_OffsetY.Text = mc.para.CAL.pick.y.value.ToString();

				TB_ConveyorEdge_OffsetX.Text = mc.para.CAL.conveyorEdge.x.value.ToString();
				TB_ConveyorEdge_OffsetY.Text = mc.para.CAL.conveyorEdge.y.value.ToString();

                TB_Tool_AngleOffset.Text = mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle].value.ToString();

				if (unitCodeZAxis == MP_HD_Z_MODE.REF) TB_Z_Axis.Text = mc.para.CAL.z.ref0.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) TB_Z_Axis.Text = mc.para.CAL.z.ulcFocus.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) TB_Z_Axis.Text = mc.para.CAL.z.xyMoving.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) TB_Z_Axis.Text = mc.para.CAL.z.pick.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) TB_Z_Axis.Text = mc.para.CAL.z.pedestal.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) TB_Z_Axis.Text = mc.para.CAL.z.touchProbe.value.ToString();
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) TB_Z_Axis.Text = mc.para.CAL.z.loadCell.value.ToString();
                if (unitCodeZAxis == MP_HD_Z_MODE.HEIGHT_OFFSET) TB_Z_Axis.Text = mc.para.CAL.z.heightOffset[(int)UnitCodeHead.HD2].value.ToString();

				if (unitCodeZAxis == MP_HD_Z_MODE.REF) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Ref0.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_UlcFocus.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_XyMoving.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Pick.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_Pedestal.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_TouchProve.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) BT_Z_AxisSelect.Text = BT_Z_AxisSelect_LoadCell.Text;
				if (unitCodeZAxis == MP_HD_Z_MODE.HEIGHT_OFFSET) BT_Z_AxisSelect.Text = BT_Z_HeightOffset.Text;
				
				TB_HDC_ResolutionX.Text = mc.para.CAL.HDC_Resolution.x.value.ToString();
				TB_HDC_ResolutionY.Text = mc.para.CAL.HDC_Resolution.y.value.ToString();

				TB_HDC_AngleOffset.Text = mc.para.CAL.HDC_AngleOffset.value.ToString();

				TB_ULC_ResolutionX.Text = mc.para.CAL.ULC_Resolution.x.value.ToString();
				TB_ULC_ResolutionY.Text = mc.para.CAL.ULC_Resolution.y.value.ToString();

				TB_ULC_AngleOffset.Text = mc.para.CAL.ULC_AngleOffset.value.ToString();

				TB_ConveyorWidthOffset.Text = mc.para.CAL.conveyorWidthOffset.value.ToString();

				TB_STANDBY_XPOS.Text = mc.para.CAL.standbyPosition.x.value.ToString();
				TB_STANDBY_YPOS.Text = mc.para.CAL.standbyPosition.y.value.ToString();

				//TB_PlaceOffsetX.Text = mc.para.CAL.place.x.value.ToString();
				//TB_PlaceOffsetY.Text = mc.para.CAL.place.y.value.ToString();

				LB_.Focus();
			}
		}

		private void NonControl_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			#region Button
			if (sender.Equals(BT_MachineRef0)) unitCodeMachineRef = UnitCodeMachineRef.REF0;
			if (sender.Equals(BT_MachineRef1_1)) unitCodeMachineRef = UnitCodeMachineRef.REF1_1;
			if (sender.Equals(BT_MachineRef1_2)) unitCodeMachineRef = UnitCodeMachineRef.REF1_2;

            if (sender.Equals(BT_ToolSelect_Tool1)) UnitCodeHD = UnitCodeHead.HD1;
            if (sender.Equals(BT_ToolSelect_Tool2)) UnitCodeHD = UnitCodeHead.HD2;

            if (sender.Equals(BT_AngleSelect_Angle1)) UnitCodeHDAngle = UnitCodeHead.HD1;
            if (sender.Equals(BT_AngleSelect_Angle2)) UnitCodeHDAngle = UnitCodeHead.HD2;

			if (sender.Equals(BT_HDC_PDSelelctP1)) unitCodePDRef = UnitCodePDRef.P1;
			if (sender.Equals(BT_HDC_PDSelelctP2)) unitCodePDRef = UnitCodePDRef.P2;

			if (sender.Equals(BT_Z_AxisSelect_Ref0)) unitCodeZAxis = MP_HD_Z_MODE.REF;
			if (sender.Equals(BT_Z_AxisSelect_UlcFocus)) unitCodeZAxis = MP_HD_Z_MODE.ULC_FOCUS;
			if (sender.Equals(BT_Z_AxisSelect_XyMoving)) unitCodeZAxis = MP_HD_Z_MODE.XY_MOVING;
			if (sender.Equals(BT_Z_AxisSelect_Pick)) unitCodeZAxis = MP_HD_Z_MODE.PICK;
			if (sender.Equals(BT_Z_AxisSelect_Pedestal)) unitCodeZAxis = MP_HD_Z_MODE.PEDESTAL;
			if (sender.Equals(BT_Z_AxisSelect_TouchProve)) unitCodeZAxis = MP_HD_Z_MODE.TOUCHPROBE;
			if (sender.Equals(BT_Z_AxisSelect_LoadCell)) unitCodeZAxis = MP_HD_Z_MODE.LOADCELL;
            if (sender.Equals(BT_Z_HeightOffset)) unitCodeZAxis = MP_HD_Z_MODE.HEIGHT_OFFSET;

			#endregion

			#region TextBox
			if (sender.Equals(TB_MachineRef_OffsetX)) mc.para.setting(mc.para.CAL.machineRef[(int)unitCodeMachineRef].x, out mc.para.CAL.machineRef[(int)unitCodeMachineRef].x);
			if (sender.Equals(TB_MachineRef_OffsetY)) mc.para.setting(mc.para.CAL.machineRef[(int)unitCodeMachineRef].y, out mc.para.CAL.machineRef[(int)unitCodeMachineRef].y);

            if (sender.Equals(TB_HDC_TOOL_OffsetX)) mc.para.setting(mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].x, out mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].x);
            if (sender.Equals(TB_HDC_TOOL_OffsetY)) mc.para.setting(mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].y, out mc.para.CAL.HDC_TOOL[(int)UnitCodeHD].y);

			if (sender.Equals(TB_HDC_LASER_OffsetX)) mc.para.setting(mc.para.CAL.HDC_LASER.x, out mc.para.CAL.HDC_LASER.x);
			if (sender.Equals(TB_HDC_LASER_OffsetY)) mc.para.setting(mc.para.CAL.HDC_LASER.y, out mc.para.CAL.HDC_LASER.y);

            if (sender.Equals(TB_HDC_PD_OffsetX))
            {
                if (unitCodePDRef == UnitCodePDRef.P1) mc.para.setting(mc.para.CAL.HDC_PD.x, out mc.para.CAL.HDC_PD.x);
                if (unitCodePDRef == UnitCodePDRef.P2) mc.para.setting(mc.para.CAL.HDC_PD.w, out mc.para.CAL.HDC_PD.w);
            }
            if (sender.Equals(TB_HDC_PD_OffsetY))
            {
                mc.para.setting(mc.para.CAL.HDC_PD.y, out mc.para.CAL.HDC_PD.y);
            }

			if (sender.Equals(TB_TouchProve_OffsetX)) mc.para.setting(mc.para.CAL.touchProbe.x, out mc.para.CAL.touchProbe.x);
			if (sender.Equals(TB_TouchProve_OffsetY)) mc.para.setting(mc.para.CAL.touchProbe.y, out mc.para.CAL.touchProbe.y);

			if (sender.Equals(TB_LoadCell_OffsetX)) mc.para.setting(mc.para.CAL.loadCell.x, out mc.para.CAL.loadCell.x);
			if (sender.Equals(TB_LoadCell_OffsetY)) mc.para.setting(mc.para.CAL.loadCell.y, out mc.para.CAL.loadCell.y);

            if (sender.Equals(TB_ULC_OffsetX)) mc.para.setting(mc.para.CAL.ulc.x, out mc.para.CAL.ulc.x);
            if (sender.Equals(TB_ULC_OffsetY)) mc.para.setting(mc.para.CAL.ulc.y, out mc.para.CAL.ulc.y);

			if (sender.Equals(TB_Pick_OffsetX)) mc.para.setting(mc.para.CAL.pick.x, out mc.para.CAL.pick.x);
			if (sender.Equals(TB_Pick_OffsetY)) mc.para.setting(mc.para.CAL.pick.y, out mc.para.CAL.pick.y);

			if (sender.Equals(TB_ConveyorEdge_OffsetX)) mc.para.setting(mc.para.CAL.conveyorEdge.x, out mc.para.CAL.conveyorEdge.x);
			if (sender.Equals(TB_ConveyorEdge_OffsetY)) mc.para.setting(mc.para.CAL.conveyorEdge.y, out mc.para.CAL.conveyorEdge.y);

            if (sender.Equals(TB_Tool_AngleOffset)) mc.para.setting(mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle], out mc.para.CAL.toolAngleOffset[(int)UnitCodeHDAngle]);

			if (sender.Equals(TB_Z_Axis))
			{
				if (unitCodeZAxis == MP_HD_Z_MODE.REF) mc.para.setting(mc.para.CAL.z.ref0, out mc.para.CAL.z.ref0);
				if (unitCodeZAxis == MP_HD_Z_MODE.ULC_FOCUS) mc.para.setting(mc.para.CAL.z.ulcFocus, out mc.para.CAL.z.ulcFocus);
				if (unitCodeZAxis == MP_HD_Z_MODE.XY_MOVING) mc.para.setting(mc.para.CAL.z.xyMoving, out mc.para.CAL.z.xyMoving);
				if (unitCodeZAxis == MP_HD_Z_MODE.PICK) mc.para.setting(mc.para.CAL.z.pick, out mc.para.CAL.z.pick);
				if (unitCodeZAxis == MP_HD_Z_MODE.PEDESTAL) mc.para.setting(mc.para.CAL.z.pedestal, out mc.para.CAL.z.pedestal);
				if (unitCodeZAxis == MP_HD_Z_MODE.TOUCHPROBE) mc.para.setting(mc.para.CAL.z.touchProbe, out mc.para.CAL.z.touchProbe);
				if (unitCodeZAxis == MP_HD_Z_MODE.LOADCELL) mc.para.setting(mc.para.CAL.z.loadCell, out mc.para.CAL.z.loadCell);
                if (unitCodeZAxis == MP_HD_Z_MODE.HEIGHT_OFFSET) mc.para.setting(mc.para.CAL.z.heightOffset[(int)UnitCodeHead.HD2], out mc.para.CAL.z.heightOffset[(int)UnitCodeHead.HD2]);
			}

			if (sender.Equals(TB_HDC_ResolutionX))
			{
				mc.para.setting(mc.para.CAL.HDC_Resolution.x, out mc.para.CAL.HDC_Resolution.x);
				mc.hdc.cam.acq.ResolutionX = mc.para.CAL.HDC_Resolution.x.value;
			}
			if (sender.Equals(TB_HDC_ResolutionY))
			{
				mc.para.setting(mc.para.CAL.HDC_Resolution.y, out mc.para.CAL.HDC_Resolution.y);
				mc.hdc.cam.acq.ResolutionY = mc.para.CAL.HDC_Resolution.y.value;
			}
			if (sender.Equals(TB_HDC_AngleOffset))
			{
				mc.para.setting(mc.para.CAL.HDC_AngleOffset, out mc.para.CAL.HDC_AngleOffset);
				mc.hdc.cam.acq.AngleOffset = mc.para.CAL.HDC_AngleOffset.value;
			}

			if (sender.Equals(TB_ULC_ResolutionX))
			{
				mc.para.setting(mc.para.CAL.ULC_Resolution.x, out mc.para.CAL.ULC_Resolution.x);
				mc.ulc.cam.acq.ResolutionX = mc.para.CAL.ULC_Resolution.x.value;
			}
			if (sender.Equals(TB_ULC_ResolutionY))
			{
				mc.para.setting(mc.para.CAL.ULC_Resolution.y, out mc.para.CAL.ULC_Resolution.y);
				mc.ulc.cam.acq.ResolutionY = mc.para.CAL.ULC_Resolution.y.value;
			}

			if (sender.Equals(TB_ULC_AngleOffset))
			{
				mc.para.setting(mc.para.CAL.ULC_AngleOffset, out mc.para.CAL.ULC_AngleOffset);
				mc.ulc.cam.acq.AngleOffset = mc.para.CAL.ULC_AngleOffset.value;
			}

			if (sender.Equals(TB_ConveyorWidthOffset))
			{
				mc.para.setting(mc.para.CAL.conveyorWidthOffset, out mc.para.CAL.conveyorWidthOffset);
				if (mc.init.success.CV)
				{
					mc.cv.jogMove(mc.cv.pos.w.WIDTH, out ret.message); if (ret.message != RetMessage.OK) mc.message.alarmMotion(ret.message);
				}
			}
			if (sender.Equals(TB_STANDBY_XPOS))
			{
				mc.para.setting(mc.para.CAL.standbyPosition.x, out mc.para.CAL.standbyPosition.x);
			}
			if (sender.Equals(TB_STANDBY_YPOS))
			{
				mc.para.setting(mc.para.CAL.standbyPosition.y, out mc.para.CAL.standbyPosition.y);
			}
			//if (sender.Equals(TB_PlaceOffsetX)) mc.para.setting(mc.para.CAL.place.x, out mc.para.CAL.place.x);
			//if (sender.Equals(TB_PlaceOffsetY)) mc.para.setting(mc.para.CAL.place.y, out mc.para.CAL.place.y);
			#endregion
			
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.error.CHECK();
			mc.check.push(sender, false);
        }	  
	}
}
