using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DefineLibrary;
using PSA_SystemLibrary;
using System.Threading;
using HalconDotNet;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;


namespace PSA_Application
{
	public partial class FormLoadcellCalib : Form
	{
		public FormLoadcellCalib()
		{
			InitializeComponent();
			#region local event registration
			
			#endregion
		}

		delegate void UpdateButtonStateDelegate(int val);
		void UpdateButtonState(int val)
		{
			if(this.InvokeRequired)
			{
				this.BeginInvoke(new UpdateButtonStateDelegate(UpdateButtonState), new object[]{val});
				return;
			}
			if (val == 0)
			{
				BT_AutoCalib.Enabled = true;
				BT_StopCalib.Enabled = false;
			}
			else
			{
				BT_AutoCalib.Enabled = false;
				BT_StopCalib.Enabled = true;
			}
		}

		RetValue ret;
		double targetForce;
		Thread threadCalib;

		static int headForceCalibStatus;
		static RetValue retT;
		static double[,] forceData = new double[20, 50];
		static double[,] forceDataTemp = new double[20, 50];
		static int checkDelay;
		static int countV, countD;
		static double offsetV, offsetD;
		static double currentV, currentD, currentF;
		static double checkTargetForce;
		static int checkRepeatCount;
		static double checkRepeatSpeed;

		private void FormLoadcellCalib_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			refreshTimer.Enabled = true;

			DomainUpDown.DomainUpDownItemCollection items = this.UD_TargetForce.Items;

			for (double force = 0.5; force < 5; force += 0.5)
				items.Add(force.ToString());

			this.UD_TargetForce.SelectedIndex = 3;		// 5;

			CB_VoltageInterval.SelectedIndex = 1;	// 0.5v, 1v(Default), 2v
			CB_CheckDistance.SelectedIndex = 1;		// 50um, 100um(Default), 200um
            cb_selectedTool.SelectedIndex = (int)UnitCodeHead.HD1;
			BT_StopCalib.Enabled = false;
		}

        private void GetHeightOffset(int head, double targetForce, out RetMessage message)
        {
            #region moving
            double posX = mc.hd.tool.tPos.x[head].LOADCELL;
            double posY = mc.hd.tool.tPos.y[head].LOADCELL;
            double posZ = mc.hd.tool.tPos.z[head].XY_MOVING;
            double posT = mc.para.CAL.toolAngleOffset[head].value;
            mc.hd.tool.jogMove(head, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message;  return; }
            #endregion

            // 1. 무빙 위치로 이동
            posZ = mc.hd.tool.tPos.z[head].XY_MOVING;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return; }
            mc.idle(100);

            // 2. 로드셀 초기화 한다.
            mc.loadCell.setZero(head);
            mc.loadCell.setZero((int)UnitCodeLoadcell.CAL);

            // 3. 로드셀 위치에서 위로 1000 um 까지 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL + 1000;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return; }
            mc.idle(100);

            // 4. 로드셀 위치로 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return; }

            // 5. 100 um 단위로 내려가며 찾기 시작
            mc.idle(500);
            double curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
            ret.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);

            LB_BottomLC.Text = curLoadcell.ToString("f3");
            LB_HeadLC.Text = ret.d1.ToString("f3");

            bool moveError = false;
            while (curLoadcell < targetForce)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                ret.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);

                LB_BottomLC.Text = curLoadcell.ToString("f3");
                LB_HeadLC.Text = ret.d1.ToString("f3");

                mc.hd.tool.jogMovePlus(head, -100, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    message = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(100);
            }

            // 6. 10 um 단위로 올라가며 찾기 시작
            mc.idle(500);

            curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

            while (!moveError && curLoadcell > targetForce)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                ret.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);

                LB_BottomLC.Text = curLoadcell.ToString("f3");
                LB_HeadLC.Text = ret.d1.ToString("f3"); 
                
                mc.hd.tool.jogMovePlus(head, 10, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    message = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(500);
            }

            // 7. 1 um 단위로 내려가며 찾기 시작
            mc.idle(500);
            curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
            ret.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);

            LB_BottomLC.Text = curLoadcell.ToString("f3");
            LB_HeadLC.Text = ret.d1.ToString("f3");

            while (!moveError && Math.Abs(curLoadcell - targetForce) > 0.01)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                ret.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);

                LB_BottomLC.Text = curLoadcell.ToString("f3");
                LB_HeadLC.Text = ret.d1.ToString("f3"); 
                
                mc.hd.tool.jogMovePlus(head, -1, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    message = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(500);
            }

            if (!moveError)
            {
                message = RetMessage.OK;
            }
            else message = ret.message;
        }

		private void Control_Click(object sender, EventArgs e)
		{
			//this.Enabled = false;
			if (sender.Equals(BT_Close))
			{
				if (headForceCalibStatus != 0)
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Machine is Working.\nFirst, Press Stop Button.");
					return;
				}
                refreshTimer.Enabled = false;
                refreshTimer.Dispose();
				this.Close();
			}

			if (sender.Equals(BT_AutoZPos))
			{
				BT_AutoZPos.Enabled = false;
				BT_Close.Enabled = false;
				#region check target force value
				try
				{
					targetForce = Convert.ToDouble(UD_TargetForce.Text);
				}
				catch
				{
					MessageBox.Show("Target Force Value is invalid : " + UD_TargetForce.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					targetForce = 5;
					goto EXIT;
				}
				
				#endregion

                GetHeightOffset(cb_selectedTool.SelectedIndex, targetForce, out ret.message);

                if (ret.message == RetMessage.OK)
                {
                }
                else mc.message.alarm("Invalid Loadcell Data!");

                BT_AutoZPos.Enabled = true;
			}
			if (sender.Equals(BT_AutoCalib))
			{
				try
				{
					checkDelay = Convert.ToInt32(TB_CheckDelay.Text);
					offsetV = Convert.ToDouble(CB_VoltageInterval.Text);
					offsetD = Convert.ToDouble(CB_CheckDistance.Text);
					countV = (int)(10 / offsetV);
					countD = (int)(900 / offsetD);
				}
				catch
				{
					checkDelay = 1000;
					countV = 10;
					countD = 25;
					goto EXIT;
				}
				try
				{
					if (headForceCalibStatus != 0)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Machine is Working.\nFirst, Press Stop Button.");
						return;
					}
					headForceCalibStatus = 1;
					threadCalib = new Thread(headForceCalib);
					threadCalib.Priority = ThreadPriority.Highest;
					threadCalib.Name = "HeadForceCalib";
					threadCalib.Start();
					mc.log.processdebug.write(mc.log.CODE.INFO, "HeadForceCalib");
					BT_AutoCalib.Enabled = false;
					BT_StopCalib.Enabled = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show("headForceCalib Exception : " + ex.ToString());
				}
			}
			if (sender.Equals(BT_StopCalib))
			{
				headForceCalibStatus = 0;
				while (threadCalib.IsAlive) mc.idle(10);
			}
			if (sender.Equals(BT_Check2LoadcellForce))
			{
				if (headForceCalibStatus != 0)
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Machine is Working.\nFirst, Press Stop Button.");
					return;
				}
				headForceCalibStatus = 2;
				threadCalib = new Thread(check2LoadcellForce);
				threadCalib.Priority = ThreadPriority.Highest;
				threadCalib.Name = "Check2LoadcellForce";
				threadCalib.Start();
				mc.log.processdebug.write(mc.log.CODE.INFO, "Check2LoadcellForce");
				BT_Check2LoadcellForce.Enabled = false;
				BT_StopCalib.Enabled = true;
			}
			if (sender.Equals(BT_CheckRepeatForce))
			{
				try
				{
					checkTargetForce = Convert.ToDouble(TB_RepeatCheckForce.Text);
					checkRepeatSpeed = Convert.ToDouble(TB_RepeatCheckSpeed.Text);
					checkRepeatCount = Convert.ToInt32(TB_RepeatCheckCount.Text);
					if (checkTargetForce < 0.3 || checkTargetForce > 15)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Target Force Range : 0.3 ~ 15");
						goto EXIT;
					}
					if (checkRepeatSpeed < 0.01 || checkRepeatSpeed > 10)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Target Speed Range : 0.01 ~ 10");
						goto EXIT;
					}
					if (checkRepeatCount < 1)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Repeat Count Range : 1 ~ ");
						goto EXIT;
					}
				}
				catch
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "NUmber Change Failure!! Please Check Input Values.");
					goto EXIT;
				}
				if (headForceCalibStatus != 0)
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Machine is Working.\nFirst, Press Stop Button.");
					return;
				}
				headForceCalibStatus = 3;
				threadCalib = new Thread(checkRepeatForce);
				threadCalib.Priority = ThreadPriority.Highest;
				threadCalib.Name = "CheckRepeatForce";
				mc.log.processdebug.write(mc.log.CODE.INFO, "CheckRepeatForce");
				threadCalib.Start();
				BT_CheckRepeatForce.Enabled = false;
				BT_StopCalib.Enabled = true;
			}
			if (sender.Equals(BT_HeadPressDryRun))
			{
				#region check target force value
				try
				{
					checkTargetForce = Convert.ToDouble(TB_RepeatCheckForce.Text);
					if (checkTargetForce < 0.01 || checkTargetForce > 1.5)
					{
						MessageBox.Show("Available Target Force(0.01 ~ 1.5)kg : [" + TB_RepeatCheckForce.Text, "] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
				catch
				{
					MessageBox.Show("Target Force Value Starnge : [" + TB_RepeatCheckForce.Text, "] Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					targetForce = 0.5;
					goto EXIT;
				}
				#endregion
				if (headForceCalibStatus != 0)
				{
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Machine is Working.\nFirst, Press Stop Button.");
					return;
				}
				headForceCalibStatus = 4;
				threadCalib = new Thread(headPressDryRun);
				threadCalib.Priority = ThreadPriority.Highest;
				threadCalib.Name = "HeadPressDryRun";
				threadCalib.Start();
				mc.log.processdebug.write(mc.log.CODE.INFO, "HeadPressDryRun");
				BT_HeadPressDryRun.Enabled = false;
				BT_StopCalib.Enabled = true;
			}
		EXIT:
			refresh();
			//this.Enabled = true;
			BT_Close.Enabled = true;
		}

		static void headForceCalib()
		{
			try
			{
				#region move to reference mark position
                double posX = mc.hd.tool.tPos.x[0].LOADCELL;
                double posY = mc.hd.tool.tPos.y[0].LOADCELL;
                double posZ = mc.hd.tool.tPos.z[0].LOADCELL + 100;		// 100um 살짝 뜬 위치
                double posT = mc.hd.tool.tPos.t[0].ZERO;
				mc.hd.tool.jogMove(0, posX, posY, posZ, posT, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
				#endregion

				#region start Calibration
				for (int i = 0; i < countV; i++)
				{
					currentV = (i + 1) * offsetV;
					//mc.hd.tool.F.voltage(currentV, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarm("Force Analog Output Error"); return; }
					mc.idle(3000);		// VPPM 안정화 시간
                    posZ = mc.hd.tool.tPos.z[0].LOADCELL;	// move to contact point
					mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }

					for (int j = 0; j < countD; j++)
					{
						currentD = (j + 1) * offsetD;
                        mc.hd.tool.jogMove(0, posZ - currentD, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
						mc.idle(checkDelay);
						retT.d = mc.loadCell.getData(0);
						currentF = retT.d;
						forceDataTemp[i, j] = currentF;
						if (headForceCalibStatus == 0) break;
					}

                    posZ = mc.hd.tool.tPos.z[0].LOADCELL + 1000;	// 1000um 살짝 뜬 위치
                    mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
					if (headForceCalibStatus == 0) break;
				}
				#endregion

				#region output data to file
				string filePath, filename;
				string savedata;
				FileStream fs = null;
				StreamWriter sw = null;

				filePath = mc2.savePath + "\\data\\calibration\\";
				if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
				filename = filePath + "ForceMeasure.dat";

				try
				{
					fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
					sw = new StreamWriter(fs, Encoding.Default);

					for (int i = 0; i < countD; i++)
					{
						savedata = null;
						for (int j = 0; j < countV; j++)
						{
							savedata += forceDataTemp[j, i].ToString() + ", ";
						}
						sw.WriteLine(savedata);
						sw.Flush();
					}
					sw.Close();
					fs.Close();
				}
				catch
				{

				}
				#endregion

				#region write force calib data
				ForceCalib.forceCheckVoltInterval = offsetV;
				ForceCalib.forceCheckDistInterval = offsetD;
				for (int i = 0; i < countD; i++)
				{
					for (int j = 0; j < countV; j++)
					{
						ForceCalib.forceCalibData[j, i] = forceDataTemp[j, i];
					}
				}
				ForceCalib.writeForceCalData();
				#endregion
				headForceCalibStatus = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show("headForceCalib Exception : " + ex.ToString());
			}
		}

		static string saveFilePath = mc2.savePath + "\\data\\calibration\\";
		static string saveFileName;
		static FileStream fileStm = null;
		static StreamWriter stmWriter = null;

		static void check2LoadcellForce()
		{
			try
			{
				#region move to loadcell position
                double posX = mc.hd.tool.tPos.x[0].LOADCELL;
                double posY = mc.hd.tool.tPos.y[0].LOADCELL;
                double posZ = mc.hd.tool.tPos.z[0].LOADCELL + 100;		// 100um 살짝 뜬 위치
                double posT = mc.hd.tool.tPos.t[0].ZERO;
				mc.hd.tool.jogMove(0, posX, posY, posZ, posT, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
				#endregion

				//mc.hd.tool.F.kilogram(2, out retT.message);
				mc.idle(3000);		// VPPM 안정화 시간

				#region start Calibration
				double outForce;
				string writeData;
				for (int i = 9; i >= 0; i--)		// 0.1mm/sec에서부터 1mm/sec까지...
				{
					saveFilePath = mc2.savePath + "\\data\\calibration\\";
					if (!Directory.Exists(saveFilePath)) Directory.CreateDirectory(saveFilePath);
					saveFileName = saveFilePath + "Speed" + ((double)((i + 1) / 10.0)).ToString() + "mm.dat";

					fileStm = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
					stmWriter = new StreamWriter(fileStm, Encoding.Default);

					writeData = "Count, Cmd, Bottom, Head";

					stmWriter.WriteLine(writeData);
					stmWriter.Flush();

					for (int j = 0; j < 150; j++)	// 10g단위로 1.5Kg까지...
					{
						// move to contact position
						//mc.hd.tool.F.kilogram(2, out retT.message);
                        posZ = mc.hd.tool.tPos.z[0].LOADCELL + 100;	// move to contact point
						mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
						//mc.hd.tool.F.kilogram(0.2, out retT.message);
						mc.idle(500);		// VPPM 안정화 시간
						outForce = (double)((j + 1) / 100.0);
                        //if(outForce < 0.2)
                        //    mc.hd.tool.F.kilogram(outForce, out retT.message);
                        //else
                        //    mc.hd.tool.F.kilogram(0.2, out retT.message);
						mc.idle(1000);
                        posZ = mc.hd.tool.tPos.z[0].LOADCELL - 300;
                        mc.hd.tool.Z[0].move(posZ, (double)((i + 1) / 10000.0), 0.01, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
						mc.hd.tool.checkZMoveEnd(0, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
						//mc.hd.tool.jogMove(posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
						//mc.hd.tool.F.kilogram(outForce, out retT.message);
						mc.idle(1500);
						retT.d = mc.loadCell.getData(0);
						//mc.loadCell.getData(out retT.d1, out retT.b1, 1);
						retT.d1 = mc.AIN.HeadLoadcell();
						//mc.hd.tool.F.sgVoltage2kilogram(retT.d1, out retT.d2, out retT.message);

						writeData = (j+1).ToString() + ", " + ((j + 1) / 100.0).ToString() + ", " + (retT.d).ToString() + ", " + Math.Round(retT.d2, 2).ToString();
						mc.log.debug.write(mc.log.CODE.INFO, "Vel[" + ((i + 1) / 10.0).ToString() + "]-" + writeData);

						stmWriter.WriteLine(writeData);
						stmWriter.Flush();

						if (headForceCalibStatus == 0) break;
					}
				THREADEND:
					stmWriter.Close();
					fileStm.Close();

					if (headForceCalibStatus == 0) break;
				}
				#endregion
                posZ = mc.hd.tool.tPos.z[0].XY_MOVING;	// move to Moving Point
				mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }

				headForceCalibStatus = 0;
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("check2LoadcellForce Exception : " + ex.ToString());
			}
		}

		static void checkRepeatForce()
		{
			try
			{
				#region move to loadcell position
                double posX = mc.hd.tool.tPos.x[0].LOADCELL;
                double posY = mc.hd.tool.tPos.y[0].LOADCELL;
                double posZ = mc.hd.tool.tPos.z[0].LOADCELL + 100;		// 100um 살짝 뜬 위치
                double posT = mc.hd.tool.tPos.t[0].ZERO;
				mc.hd.tool.jogMove(0, posX, posY, posZ, posT, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
				#endregion

				//mc.hd.tool.F.kilogram(2, out retT.message);
				mc.idle(3000);		// VPPM 안정화 시간

				string writeData;

				saveFilePath = mc2.savePath + "\\data\\calibration\\";
				if (!Directory.Exists(saveFilePath)) Directory.CreateDirectory(saveFilePath);
				saveFileName = saveFilePath + "RepeatCheck_Speed" + (checkRepeatSpeed).ToString() + "mm_Force" + (checkTargetForce).ToString() + "kg.dat";

				fileStm = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
				stmWriter = new StreamWriter(fileStm, Encoding.Default);

				writeData = "Count, Cmd, Bottom, Head";
				stmWriter.WriteLine(writeData);
				stmWriter.Flush();

				#region start Calibration
				for (int i = 0; i <checkRepeatCount; i++)
				{
                    posZ = mc.hd.tool.tPos.z[0].LOADCELL + 1000;	// move to contact point
					mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
                   
                    mc.loadCell.setZero((int)UnitCodeLoadcell.CAL);
                    mc.loadCell.setZero((int)UnitCodeLoadcell.TOP1);
                    
                    // move to contact position
                    posZ = mc.hd.tool.tPos.z[0].LOADCELL + 100;	// move to contact point
					mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }

                    mc.idle(1000);

                    double forceHeight = mc.hd.tool.F.kilogram2Height(0, checkTargetForce, out retT.message);
					//posZ = mc.hd.tool.tPos.z[0].LOADCELL - 300;
                    posZ = mc.hd.tool.tPos.z[0].LOADCELL - forceHeight;
                    mc.hd.tool.Z[0].move(posZ, (double)(checkRepeatSpeed / 1000.0), 0.01, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
					mc.hd.tool.checkZMoveEnd(0, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }

                    mc.idle(1500);
                    retT.d = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                    retT.d1 = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);
					

					writeData = (i + 1).ToString() + ", " + (checkTargetForce).ToString() + ", " + (retT.d).ToString() + ", " + Math.Round(retT.d1, 2).ToString();
					mc.log.debug.write(mc.log.CODE.INFO, writeData);

					stmWriter.WriteLine(writeData);
					stmWriter.Flush();

					if (headForceCalibStatus == 0) break;
				}
				THREADEND:
				stmWriter.Close();
				fileStm.Close();
				#endregion

                posZ = mc.hd.tool.tPos.z[0].XY_MOVING;	// move to Moving Point
				mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }

				headForceCalibStatus = 0;
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("checkRepeatForce Exception : " + ex.ToString());
			}
		}

		static void headPressDryRun()
		{
			try
			{
				#region move to reference mark position
				double posX = mc.hd.tool.tPos.x[0].REF0;
                double posY = mc.hd.tool.tPos.y[0].REF0;
                double posZ = mc.hd.tool.tPos.z[0].XY_MOVING;		// 100um 살짝 뜬 위치
                double posT = mc.hd.tool.tPos.t[0].ZERO;
				mc.hd.tool.jogMove(0, posX, posY, posZ, posT, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); return; }
				#endregion
				//mc.hd.tool.F.kilogram(2.0, out retT.message);
				mc.idle(1500);		// VPPM 안정화 시간

				while (true)
				{
                    posZ = mc.hd.tool.tPos.z[0].REF0 + 100;		// 100um 살짝 뜬 위치
					mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }

					//mc.hd.tool.F.kilogram(0.2, out retT.message);
					mc.idle(500);		// VPPM 안정화 시간

					// move to contact position
                    posZ = mc.hd.tool.tPos.z[0].REF0 + mc.para.HD.place.forceOffset.z.value;	// move to contact point
                    mc.hd.tool.Z[0].move(posZ, (mc.para.HD.place.search2.vel.value) / 1000.0, 0.01, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
					mc.hd.tool.checkZMoveEnd(0, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
					//mc.hd.tool.F.kilogram(checkTargetForce, out retT.message);
					mc.idle(1000);

                    posZ = mc.hd.tool.tPos.z[0].REF0 + 100;		// 100um 살짝 뜬 위치
                    mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }

                    posZ = mc.hd.tool.tPos.z[0].XY_MOVING;
                    mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); headForceCalibStatus = 0; goto THREADEND; }
					//mc.hd.tool.F.kilogram(2.0, out retT.message);
					mc.idle(500);		// VPPM 안정화 시간

					if (headForceCalibStatus == 0) break;
				}
				THREADEND:
                posZ = mc.hd.tool.tPos.z[0].XY_MOVING;	// move to contact point
                mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); }

				headForceCalibStatus = 0;
			}
			catch (System.Exception ex)
			{
				MessageBox.Show("headPressDryRun Exception : " + ex.ToString());
			}
		}

		private void refresh()
		{
			if (TC_Loadcell_T.SelectedIndex != 0)
			{
				LB_OutVolt.Text = currentV.ToString();
				LB_CurDist.Text = currentD.ToString();
				LB_CurForce.Text = currentF.ToString("f3");
				if (headForceCalibStatus == 0)
				{
					BT_StopCalib.Enabled = false;
					BT_AutoCalib.Enabled = true;
					BT_Check2LoadcellForce.Enabled = true;
					BT_CheckRepeatForce.Enabled = true;
					BT_HeadPressDryRun.Enabled = true;
				}
			}
		}

		private void refreshTimer_Tick(object sender, EventArgs e)
		{
			refreshTimer.Enabled = false;
			refresh();
			refreshTimer.Enabled = true;
		}
	}
}
