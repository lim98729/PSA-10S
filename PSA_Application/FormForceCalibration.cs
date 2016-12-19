using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using DefineLibrary;
using AccessoryLibrary;
using PSA_SystemLibrary;

namespace PSA_Application
{
	public partial class FormForceCalibration : Form
	{
		public FormForceCalibration()
		{
			InitializeComponent();
			testKilogram.name = "Test Force (Kg)";
			testKilogram.value = 5;
			testKilogram.lowerLimit = 0;
			testKilogram.upperLimit = 20;
		}

		para_member testKilogram;
		RetValue ret;
		QueryTimer dwell = new QueryTimer();
             
		static bool reqThreadStop = true;
		static double posZ;
		static RetValue retT;

		Thread threadForceCalibration;

        static parameterForce force = new parameterForce();
        static parameterForce selectedToolForce = new parameterForce();

		static bool threadAbortFlag;
		bool calDataChanged;
        int selectedHead;

        private void forceCalibraion()
		{
			try
			{
				threadAbortFlag = false;
              
                while (reqThreadStop == false)
				{
					for (int j = 0; j < 20; j++)
					{
						if (reqThreadStop == true)
						{
							threadAbortFlag = true;
							break;		// 매 동작을 시작하기 전에 Stop 신호 검사
						}
                        
                        mc.idle(10);

                        #region z moving
                        RetMessage retMsg;
                        double posZ = getHeight(force.forceLevel[j].value, out retMsg);
                        if (retMsg != RetMessage.OK) { mc.message.alarmMotion(retMsg); reqThreadStop = true;  break; }
                        else
                        {
                            double bottomValue = 0; double topValue = 0;
                            double height = 0;

                            if (selectedHead == (int)UnitCodeHead.HD1) topValue = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);
                            else topValue = mc.loadCell.getData((int)UnitCodeLoadcell.TOP2);

                            bottomValue = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

                            force.bottomForce[j].value = bottomValue;
                            force.topForce[j].value = topValue;
                            mc.hd.tool.Z[selectedHead].actualPosition(out height, out ret.message);
                            force.heightLevel[j].value = Math.Round(mc.hd.tool.tPos.z[selectedHead].LOADCELL - posZ, 1);
                            
                            refresh();
                        }

                        posZ = mc.hd.tool.tPos.z[selectedHead].XY_MOVING;
                        mc.hd.tool.jogMove(selectedHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); reqThreadStop = true; break; }
                        mc.idle(100);
                        #endregion
					}
					
                    if (threadAbortFlag == false)
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Force Calibration is Finish!!!");
					reqThreadStop = true;
				}
				if (threadAbortFlag == true)
				{
                    posZ = mc.hd.tool.tPos.z[0].XY_MOVING;
					mc.hd.tool.jogMove(0, posZ, out retT.message); if (retT.message != RetMessage.OK) { mc.message.alarmMotion(retT.message); }
					EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.INFORMATION, "Force Calibration is Aborted!!!");
				}
			}
			catch (System.Exception ex)
			{
				reqThreadStop = true;
				MessageBox.Show("Force Calibration Exception : " + ex.ToString());
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BT_ESC))
			{
				if (threadForceCalibration!=null)
				{
					if (threadForceCalibration.IsAlive)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Auto Calibration is Working!!!");
						return;
					}
				}

				for (int i = 0; i < 20; i++)
				{
                    if (force.forceLevel[i].value != selectedToolForce.forceLevel[i].value || force.bottomForce[i].value != selectedToolForce.bottomForce[i].value
                        || force.topForce[i].value != selectedToolForce.topForce[i].value || force.heightLevel[i].value != selectedToolForce.heightLevel[i].value)
					{
						calDataChanged = true;
						break;			// 다른 것이 있는지 없는지 유무만 판단하므로.. 하나라도 다르면 Break;
										// 사실 다른 것만 인덱스로 받아서 업데이트 시키면 되는데 귀찮아서 그냥 통짜로함.
					}
					else if(calDataChanged) calDataChanged = false;		// true 일 경우에만 false로 바꿈.
				}

				if (calDataChanged)
				{
					FormUserMessage ff = new FormUserMessage();

                    ff.SetDisplayItems(DIAG_SEL_MODE.YesNoCancel, DIAG_ICON_MODE.QUESTION, textResource.MB_ETC_PARA_SAVE);
					ff.ShowDialog();

					ret.usrDialog = FormUserMessage.diagResult;
					if (ret.usrDialog == DIAG_RESULT.Yes)
					{
                        if (selectedHead == 0)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                mc.para.CAL.Tool_Force1.forceLevel[i] = force.forceLevel[i];
                                mc.para.CAL.Tool_Force1.bottomForce[i] = force.bottomForce[i];
                                mc.para.CAL.Tool_Force1.topForce[i] = force.topForce[i];
                                mc.para.CAL.Tool_Force1.heightLevel[i] = force.heightLevel[i];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                mc.para.CAL.Tool_Force2.forceLevel[i] = force.forceLevel[i];
                                mc.para.CAL.Tool_Force2.bottomForce[i] = force.bottomForce[i];
                                mc.para.CAL.Tool_Force2.topForce[i] = force.topForce[i];
                                mc.para.CAL.Tool_Force2.heightLevel[i] = force.heightLevel[i];
                            }
                        }

						mc.para.HD.place.placeForceOffset[0].value = 0;		// clear
                        mc.para.HD.place.placeForceOffset[1].value = 0;		// clear
					}
				}
				this.Close();
			}
		   
			#region BT_AutoCalibration
			if (sender.Equals(BT_AutoCalibration))
			{
				if (threadForceCalibration != null)
				{
					if (threadForceCalibration.IsAlive)
					{
						EVENT.userDialogMessage(DIAG_SEL_MODE.OK, DIAG_ICON_MODE.WARNING, "Auto Calibration is Working!!!");
						return;
					}
				}
				calDataChanged = true;

				reqThreadStop = false;

				BT_STOP.Enabled = true;
				BT_AutoCalibration.Enabled = false;

                selectedHead = cb_selectedTool.SelectedIndex;
                double posX = mc.hd.tool.tPos.x[selectedHead].LOADCELL;
                double posY = mc.hd.tool.tPos.y[selectedHead].LOADCELL;
                mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                if(ret.message == RetMessage.OK)
                {
                    threadForceCalibration = new Thread(forceCalibraion);
                    threadForceCalibration.Priority = ThreadPriority.Normal;
                    threadForceCalibration.Name = "forceCalibraion";
                    threadForceCalibration.Start();
                    mc.log.processdebug.write(mc.log.CODE.INFO, " forceCalibration");
                }
			}
			#endregion

			#region 0~19버튼 이벤트
            if (sender.Equals(TB_Input_Force_0)) mc.para.setting(force.forceLevel[0], out force.forceLevel[0]);
            if (sender.Equals(TB_Input_Force_1)) mc.para.setting(force.forceLevel[1], out force.forceLevel[1]);
            if (sender.Equals(TB_Input_Force_2)) mc.para.setting(force.forceLevel[2], out force.forceLevel[2]);
            if (sender.Equals(TB_Input_Force_3)) mc.para.setting(force.forceLevel[3], out force.forceLevel[3]);
            if (sender.Equals(TB_Input_Force_4)) mc.para.setting(force.forceLevel[4], out force.forceLevel[4]);
            if (sender.Equals(TB_Input_Force_5)) mc.para.setting(force.forceLevel[5], out force.forceLevel[5]);
            if (sender.Equals(TB_Input_Force_6)) mc.para.setting(force.forceLevel[6], out force.forceLevel[6]);
            if (sender.Equals(TB_Input_Force_7)) mc.para.setting(force.forceLevel[7], out force.forceLevel[7]);
            if (sender.Equals(TB_Input_Force_8)) mc.para.setting(force.forceLevel[8], out force.forceLevel[8]);
            if (sender.Equals(TB_Input_Force_9)) mc.para.setting(force.forceLevel[9], out force.forceLevel[9]);
			if (sender.Equals(TB_Input_Force_10)) mc.para.setting(force.forceLevel[10], out force.forceLevel[10]);
            if (sender.Equals(TB_Input_Force_11)) mc.para.setting(force.forceLevel[11], out force.forceLevel[11]);
			if (sender.Equals(TB_Input_Force_12)) mc.para.setting(force.forceLevel[12], out force.forceLevel[12]);
			if (sender.Equals(TB_Input_Force_13)) mc.para.setting(force.forceLevel[13], out force.forceLevel[13]);
			if (sender.Equals(TB_Input_Force_14)) mc.para.setting(force.forceLevel[14], out force.forceLevel[14]);
			if (sender.Equals(TB_Input_Force_15)) mc.para.setting(force.forceLevel[15], out force.forceLevel[15]);
			if (sender.Equals(TB_Input_Force_16)) mc.para.setting(force.forceLevel[16], out force.forceLevel[16]);
			if (sender.Equals(TB_Input_Force_17)) mc.para.setting(force.forceLevel[17], out force.forceLevel[17]);
			if (sender.Equals(TB_Input_Force_18)) mc.para.setting(force.forceLevel[18], out force.forceLevel[18]);
			if (sender.Equals(TB_Input_Force_19)) mc.para.setting(force.forceLevel[19], out force.forceLevel[19]);

			if (sender.Equals(TB_Bottom_Force_0)) mc.para.setting(force.bottomForce[0], out force.bottomForce[0]);
			if (sender.Equals(TB_Bottom_Force_1)) mc.para.setting(force.bottomForce[1], out force.bottomForce[1]);
			if (sender.Equals(TB_Bottom_Force_2)) mc.para.setting(force.bottomForce[2], out force.bottomForce[2]);
			if (sender.Equals(TB_Bottom_Force_3)) mc.para.setting(force.bottomForce[3], out force.bottomForce[3]);
			if (sender.Equals(TB_Bottom_Force_4)) mc.para.setting(force.bottomForce[4], out force.bottomForce[4]);
			if (sender.Equals(TB_Bottom_Force_5)) mc.para.setting(force.bottomForce[5], out force.bottomForce[5]);
			if (sender.Equals(TB_Bottom_Force_6)) mc.para.setting(force.bottomForce[6], out force.bottomForce[6]);
			if (sender.Equals(TB_Bottom_Force_7)) mc.para.setting(force.bottomForce[7], out force.bottomForce[7]);
			if (sender.Equals(TB_Bottom_Force_8)) mc.para.setting(force.bottomForce[8], out force.bottomForce[8]);
			if (sender.Equals(TB_Bottom_Force_9)) mc.para.setting(force.bottomForce[9], out force.bottomForce[9]);
			if (sender.Equals(TB_Bottom_Force_10)) mc.para.setting(force.bottomForce[10], out force.bottomForce[10]);
			if (sender.Equals(TB_Bottom_Force_11)) mc.para.setting(force.bottomForce[11], out force.bottomForce[11]);
			if (sender.Equals(TB_Bottom_Force_12)) mc.para.setting(force.bottomForce[12], out force.bottomForce[12]);
			if (sender.Equals(TB_Bottom_Force_13)) mc.para.setting(force.bottomForce[13], out force.bottomForce[13]);
			if (sender.Equals(TB_Bottom_Force_14)) mc.para.setting(force.bottomForce[14], out force.bottomForce[14]);
			if (sender.Equals(TB_Bottom_Force_15)) mc.para.setting(force.bottomForce[15], out force.bottomForce[15]);
			if (sender.Equals(TB_Bottom_Force_16)) mc.para.setting(force.bottomForce[16], out force.bottomForce[16]);
			if (sender.Equals(TB_Bottom_Force_17)) mc.para.setting(force.bottomForce[17], out force.bottomForce[17]);
			if (sender.Equals(TB_Bottom_Force_18)) mc.para.setting(force.bottomForce[18], out force.bottomForce[18]);
			if (sender.Equals(TB_Bottom_Force_19)) mc.para.setting(force.bottomForce[19], out force.bottomForce[19]);

			if (sender.Equals(TB_Top_Force_0)) mc.para.setting(force.topForce[0], out force.topForce[0]);
			if (sender.Equals(TB_Top_Force_1)) mc.para.setting(force.topForce[1], out force.topForce[1]);
			if (sender.Equals(TB_Top_Force_2)) mc.para.setting(force.topForce[2], out force.topForce[2]);
			if (sender.Equals(TB_Top_Force_3)) mc.para.setting(force.topForce[3], out force.topForce[3]);
			if (sender.Equals(TB_Top_Force_4)) mc.para.setting(force.topForce[4], out force.topForce[4]);
			if (sender.Equals(TB_Top_Force_5)) mc.para.setting(force.topForce[5], out force.topForce[5]);
			if (sender.Equals(TB_Top_Force_6)) mc.para.setting(force.topForce[6], out force.topForce[6]);
			if (sender.Equals(TB_Top_Force_7)) mc.para.setting(force.topForce[7], out force.topForce[7]);
			if (sender.Equals(TB_Top_Force_8)) mc.para.setting(force.topForce[8], out force.topForce[8]);
            if (sender.Equals(TB_Top_Force_9)) mc.para.setting(force.topForce[9], out force.topForce[9]);
            if (sender.Equals(TB_Top_Force_10)) mc.para.setting(force.topForce[10], out force.topForce[10]);
            if (sender.Equals(TB_Top_Force_11)) mc.para.setting(force.topForce[11], out force.topForce[11]);
            if (sender.Equals(TB_Top_Force_12)) mc.para.setting(force.topForce[12], out force.topForce[12]);
            if (sender.Equals(TB_Top_Force_13)) mc.para.setting(force.topForce[13], out force.topForce[13]);
            if (sender.Equals(TB_Top_Force_14)) mc.para.setting(force.topForce[14], out force.topForce[14]);
            if (sender.Equals(TB_Top_Force_15)) mc.para.setting(force.topForce[15], out force.topForce[15]);
            if (sender.Equals(TB_Top_Force_16)) mc.para.setting(force.topForce[16], out force.topForce[16]);
            if (sender.Equals(TB_Top_Force_17)) mc.para.setting(force.topForce[17], out force.topForce[17]);
            if (sender.Equals(TB_Top_Force_18)) mc.para.setting(force.topForce[18], out force.topForce[18]);
            if (sender.Equals(TB_Top_Force_19)) mc.para.setting(force.topForce[19], out force.topForce[19]);
			#endregion

			if (sender.Equals(BT_STOP))
			{ 
				reqThreadStop = true;
				BT_STOP.Enabled = false;

				if (threadForceCalibration.IsAlive) mc.idle(10);

				BT_AutoCalibration.Enabled = true;
			}

			if (sender.Equals(TB_Force_Test_Kilogram)) mc.para.setting(testKilogram, out testKilogram);

			EXIT:
			refresh();
		}


		delegate void refresh_Call();
        private void refresh()
		{
			if (this.InvokeRequired)
			{
				refresh_Call d = new refresh_Call(refresh);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				if (threadForceCalibration != null)
				{
					if (threadForceCalibration.IsAlive)
					{
						BT_STOP.Enabled = true;
						//BT_AutoCalibration.Enabled = false;
					}
					else
					{
						BT_STOP.Enabled = false;
						//BT_AutoCalibration.Enabled = false;
					}
				}
				else
				{
					BT_STOP.Enabled = false;
					//BT_AutoCalibration.Enabled = true;
				}

                TB_Input_Force_0.Text = force.forceLevel[0].value.ToString();
                TB_Input_Force_1.Text = force.forceLevel[1].value.ToString();
                TB_Input_Force_2.Text = force.forceLevel[2].value.ToString();
                TB_Input_Force_3.Text = force.forceLevel[3].value.ToString();
                TB_Input_Force_4.Text = force.forceLevel[4].value.ToString();
                TB_Input_Force_5.Text = force.forceLevel[5].value.ToString();
                TB_Input_Force_6.Text = force.forceLevel[6].value.ToString();
                TB_Input_Force_7.Text = force.forceLevel[7].value.ToString();
                TB_Input_Force_8.Text = force.forceLevel[8].value.ToString();
                TB_Input_Force_9.Text = force.forceLevel[9].value.ToString();
                TB_Input_Force_10.Text = force.forceLevel[10].value.ToString();
                TB_Input_Force_11.Text = force.forceLevel[11].value.ToString();
                TB_Input_Force_12.Text = force.forceLevel[12].value.ToString();
                TB_Input_Force_13.Text = force.forceLevel[13].value.ToString();
                TB_Input_Force_14.Text = force.forceLevel[14].value.ToString();
                TB_Input_Force_15.Text = force.forceLevel[15].value.ToString();
                TB_Input_Force_16.Text = force.forceLevel[16].value.ToString();
                TB_Input_Force_17.Text = force.forceLevel[17].value.ToString();
                TB_Input_Force_18.Text = force.forceLevel[18].value.ToString();
                TB_Input_Force_19.Text = force.forceLevel[19].value.ToString();

                TB_Bottom_Force_0.Text = force.bottomForce[0].value.ToString();
                TB_Bottom_Force_1.Text = force.bottomForce[1].value.ToString();
                TB_Bottom_Force_2.Text = force.bottomForce[2].value.ToString();
                TB_Bottom_Force_3.Text = force.bottomForce[3].value.ToString();
                TB_Bottom_Force_4.Text = force.bottomForce[4].value.ToString();
                TB_Bottom_Force_5.Text = force.bottomForce[5].value.ToString();
                TB_Bottom_Force_6.Text = force.bottomForce[6].value.ToString();
                TB_Bottom_Force_7.Text = force.bottomForce[7].value.ToString();
                TB_Bottom_Force_8.Text = force.bottomForce[8].value.ToString();
                TB_Bottom_Force_9.Text = force.bottomForce[9].value.ToString();
                TB_Bottom_Force_10.Text = force.bottomForce[10].value.ToString();
                TB_Bottom_Force_11.Text = force.bottomForce[11].value.ToString();
                TB_Bottom_Force_12.Text = force.bottomForce[12].value.ToString();
                TB_Bottom_Force_13.Text = force.bottomForce[13].value.ToString();
                TB_Bottom_Force_14.Text = force.bottomForce[14].value.ToString();
                TB_Bottom_Force_15.Text = force.bottomForce[15].value.ToString();
                TB_Bottom_Force_16.Text = force.bottomForce[16].value.ToString();
                TB_Bottom_Force_17.Text = force.bottomForce[17].value.ToString();
                TB_Bottom_Force_18.Text = force.bottomForce[18].value.ToString();
                TB_Bottom_Force_19.Text = force.bottomForce[19].value.ToString();

                TB_Top_Force_0.Text = force.topForce[0].value.ToString();
                TB_Top_Force_1.Text = force.topForce[1].value.ToString();
                TB_Top_Force_2.Text = force.topForce[2].value.ToString();
                TB_Top_Force_3.Text = force.topForce[3].value.ToString();
                TB_Top_Force_4.Text = force.topForce[4].value.ToString();
                TB_Top_Force_5.Text = force.topForce[5].value.ToString();
                TB_Top_Force_6.Text = force.topForce[6].value.ToString();
                TB_Top_Force_7.Text = force.topForce[7].value.ToString();
                TB_Top_Force_8.Text = force.topForce[8].value.ToString();
                TB_Top_Force_9.Text = force.topForce[9].value.ToString();
                TB_Top_Force_10.Text = force.topForce[10].value.ToString();
                TB_Top_Force_11.Text = force.topForce[11].value.ToString();
                TB_Top_Force_12.Text = force.topForce[12].value.ToString();
                TB_Top_Force_13.Text = force.topForce[13].value.ToString();
                TB_Top_Force_14.Text = force.topForce[14].value.ToString();
                TB_Top_Force_15.Text = force.topForce[15].value.ToString();
                TB_Top_Force_16.Text = force.topForce[16].value.ToString();
                TB_Top_Force_17.Text = force.topForce[17].value.ToString();
                TB_Top_Force_18.Text = force.topForce[18].value.ToString();
                TB_Top_Force_19.Text = force.topForce[19].value.ToString();

                TB_HeightLevel_0.Text = force.heightLevel[0].value.ToString();
                TB_HeightLevel_1.Text = force.heightLevel[1].value.ToString();
                TB_HeightLevel_2.Text = force.heightLevel[2].value.ToString();
                TB_HeightLevel_3.Text = force.heightLevel[3].value.ToString();
                TB_HeightLevel_4.Text = force.heightLevel[4].value.ToString();
                TB_HeightLevel_5.Text = force.heightLevel[5].value.ToString();
                TB_HeightLevel_6.Text = force.heightLevel[6].value.ToString();
                TB_HeightLevel_7.Text = force.heightLevel[7].value.ToString();
                TB_HeightLevel_8.Text = force.heightLevel[8].value.ToString();
                TB_HeightLevel_9.Text = force.heightLevel[9].value.ToString();
                TB_HeightLevel_10.Text = force.heightLevel[10].value.ToString();
                TB_HeightLevel_11.Text = force.heightLevel[11].value.ToString();
                TB_HeightLevel_12.Text = force.heightLevel[12].value.ToString();
                TB_HeightLevel_13.Text = force.heightLevel[13].value.ToString();
                TB_HeightLevel_14.Text = force.heightLevel[14].value.ToString();
                TB_HeightLevel_15.Text = force.heightLevel[15].value.ToString();
                TB_HeightLevel_16.Text = force.heightLevel[16].value.ToString();
                TB_HeightLevel_17.Text = force.heightLevel[17].value.ToString();
                TB_HeightLevel_18.Text = force.heightLevel[18].value.ToString();
                TB_HeightLevel_19.Text = force.heightLevel[19].value.ToString();

				TB_Force_Test_Kilogram.Text = testKilogram.value.ToString();

				BT_ESC.Focus();

			}
		}

		private void FormForceCalibration_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;

			BT_STOP.Enabled = false;
			BT_AutoCalibration.Enabled = true;
			calDataChanged = false;

            cb_selectedTool.SelectedIndex = 0;
            selectedHead = cb_selectedTool.SelectedIndex;
		}
		private void BT_Force_FactorX_Click(object sender, EventArgs e)
		{
			this.Enabled = false;

			double forceLevelValue = 0;
            int forceLevel = 0;
            
			#region Button Events
            if (sender.Equals(BT_Force_FactorX0)) { forceLevelValue = force.forceLevel[0].value; forceLevel = 0; }
            else if (sender.Equals(BT_Force_FactorX1)) { forceLevelValue = force.forceLevel[1].value; forceLevel = 1; }
            else if (sender.Equals(BT_Force_FactorX2)) { forceLevelValue = force.forceLevel[2].value; forceLevel = 2; }
            else if (sender.Equals(BT_Force_FactorX3)) { forceLevelValue = force.forceLevel[3].value; forceLevel = 3; }
            else if (sender.Equals(BT_Force_FactorX4)) { forceLevelValue = force.forceLevel[4].value; forceLevel = 4; }
            else if (sender.Equals(BT_Force_FactorX5)) { forceLevelValue = force.forceLevel[5].value; forceLevel = 5; }
            else if (sender.Equals(BT_Force_FactorX6)) { forceLevelValue = force.forceLevel[6].value; forceLevel = 6; }
            else if (sender.Equals(BT_Force_FactorX7)) { forceLevelValue = force.forceLevel[7].value; forceLevel = 7; }
            else if (sender.Equals(BT_Force_FactorX8)) { forceLevelValue = force.forceLevel[8].value; forceLevel = 8; }
            else if (sender.Equals(BT_Force_FactorX9)) { forceLevelValue = force.forceLevel[9].value; forceLevel = 9; }
            else if (sender.Equals(BT_Force_FactorX10)) { forceLevelValue = force.forceLevel[10].value; forceLevel = 10; }
            else if (sender.Equals(BT_Force_FactorX11)) { forceLevelValue = force.forceLevel[11].value; forceLevel = 11; }
            else if (sender.Equals(BT_Force_FactorX12)) { forceLevelValue = force.forceLevel[12].value; forceLevel = 12; }
            else if (sender.Equals(BT_Force_FactorX13)) { forceLevelValue = force.forceLevel[13].value; forceLevel = 13; }
            else if (sender.Equals(BT_Force_FactorX14)) { forceLevelValue = force.forceLevel[14].value; forceLevel = 14; }
            else if (sender.Equals(BT_Force_FactorX15)) { forceLevelValue = force.forceLevel[15].value; forceLevel = 15; }
            else if (sender.Equals(BT_Force_FactorX16)) { forceLevelValue = force.forceLevel[16].value; forceLevel = 16; }
            else if (sender.Equals(BT_Force_FactorX17)) { forceLevelValue = force.forceLevel[17].value; forceLevel = 17; }
            else if (sender.Equals(BT_Force_FactorX18)) { forceLevelValue = force.forceLevel[18].value; forceLevel = 18; }
            else if (sender.Equals(BT_Force_FactorX19)) { forceLevelValue = force.forceLevel[19].value; forceLevel = 19; }
            else { mc.message.alarm("unknown Force Factor X Button Click"); goto EXIT; }
			#endregion

			#region z moving
            RetMessage retMsg;
            double posZ = getHeight(forceLevelValue, out retMsg);
            if (retMsg != RetMessage.OK) { mc.message.alarmMotion(retMsg); goto EXIT; }
            else
            {
                double bottomValue = 0; double topValue = 0;
                
                if(selectedHead == (int)UnitCodeHead.HD1) topValue = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);
                else topValue = mc.loadCell.getData((int)UnitCodeLoadcell.TOP2);

                bottomValue = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

                force.bottomForce[forceLevel].value = bottomValue;
                force.topForce[forceLevel].value = topValue;
                force.heightLevel[forceLevel].value = posZ;
            }

            posZ = mc.hd.tool.tPos.z[selectedHead].XY_MOVING;
            mc.hd.tool.jogMove(selectedHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
			mc.idle(100);
			#endregion

		EXIT:
			refresh();
			this.Enabled = true;
		}

		private void BT_Force_Test_Kilogram2DAValue_Click(object sender, EventArgs e)
		{
        //    this.Enabled = false;
        //    #region BT_Force_Test_Kilogram2DAValue
        //    #region z moving
        //    posZ = mc.hd.tool.tPos.z[0].XY_MOVING;
        //    mc.hd.tool.jogMove(0, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
        //    mc.idle(100);
        //    posZ = mc.hd.tool.tPos.z[0].LOADCELL + 1000;
        //    mc.hd.tool.jogMove(0, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
        //    mc.idle(100);
        //    posZ = mc.hd.tool.tPos.z[0].LOADCELL;
        //    mc.hd.tool.jogMove(0, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
        //    dwell.Reset();
        //    mc.hd.tool.F.kilogram(testKilogram, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }
        //    mc.idle(100);
        //    posZ = mc.hd.tool.tPos.z[0].LOADCELL - mc.para.CAL.force.touchOffset.value;

        //    mc.hd.tool.jogMove(0, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
        //    #endregion

        //    TB_Result.Clear();
        //    //dwell.Reset();
        //    //mc.hd.tool.F.kilogram(testKilogram, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }

        //    TB_Result.Visible = false;
        //    for (int i = 0; i < 30; i++)
        //    {
        //        ret.d = mc.loadCell.getData(0); if (ret.d < -1) { mc.message.alarm("Loadcell 통신에러"); goto EXIT; }
        //        TB_Result.AppendText(Math.Round(dwell.Elapsed).ToString() + " ms  :  " + ret.d.ToString() + " kg" + "\n");
        //        mc.idle(10);
        //    }
        //    TB_Force_Test_DAValue.Text = ret.d.ToString();
        //    TB_Result.Visible = true;
        //    #endregion
        //EXIT:
        //    refresh();
        //    this.Enabled = true;
		}

        private void cb_selectedTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selectedHead = cb_selectedTool.SelectedIndex;
                if (selectedHead == (int)UnitCodeHead.HD1)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        selectedToolForce.forceLevel[i] = mc.para.CAL.Tool_Force1.forceLevel[i];
                        selectedToolForce.bottomForce[i] = mc.para.CAL.Tool_Force1.bottomForce[i];
                        selectedToolForce.topForce[i] = mc.para.CAL.Tool_Force1.topForce[i];
                        selectedToolForce.heightLevel[i] = mc.para.CAL.Tool_Force1.heightLevel[i];
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        selectedToolForce.forceLevel[i] = mc.para.CAL.Tool_Force2.forceLevel[i];
                        selectedToolForce.bottomForce[i] = mc.para.CAL.Tool_Force2.bottomForce[i];
                        selectedToolForce.topForce[i] = mc.para.CAL.Tool_Force2.topForce[i];
                        selectedToolForce.heightLevel[i] = mc.para.CAL.Tool_Force2.heightLevel[i];
                    }
                }

                for (int i = 0; i < 20; i++)
                {
                    force.forceLevel[i] = selectedToolForce.forceLevel[i];
                    force.bottomForce[i] = selectedToolForce.bottomForce[i];
                    force.topForce[i] = selectedToolForce.topForce[i];
                    force.heightLevel[i] = selectedToolForce.heightLevel[i];
                }

                refresh();
            }
            catch
            {
                selectedHead = 0;
            }
        }

        private void LB_Force_FactorX_DoubleClick(object sender, EventArgs e)
        {
            FormUserMessage ff = new FormUserMessage();

            ff.SetDisplayItems(DIAG_SEL_MODE.YesNo, DIAG_ICON_MODE.QUESTION, "Do you want to calculate force level?");
            ff.ShowDialog();

            ret.usrDialog = FormUserMessage.diagResult;
            if (ret.usrDialog == DIAG_RESULT.Yes)
            {
                double step = (force.forceLevel[19].value - force.forceLevel[0].value) / 19;

                for (int i = 0; i < 20; i++)
                {
                    force.forceLevel[i].value = Math.Round(force.forceLevel[0].value + step * i, 2);
                }
            }
            refresh();
        }

        private double getHeight(double targetHeight, out RetMessage retMessage)
        {
            // 1. 무빙 위치로 이동
            int head = selectedHead;
            posZ = mc.hd.tool.tPos.z[head].XY_MOVING;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); retMessage = ret.message; return 0; }
            mc.idle(100);

            // 2. 로드셀 초기화 한다.
            mc.loadCell.setZero((int)UnitCodeLoadcell.CAL);
            mc.loadCell.setZero((int)UnitCodeLoadcell.TOP1);
            mc.loadCell.setZero((int)UnitCodeLoadcell.TOP2);

            // 3. 로드셀 위치에서 위로 1000 um 까지 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL + 1000;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); retMessage = ret.message; return 0; }
            mc.idle(100);

            // 4. 로드셀 위치로 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); retMessage = ret.message; return 0; }

            // 5. 100 um 단위로 내려가며 찾기 시작
            mc.idle(500);
            double curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
            bool moveError = false;
            while (curLoadcell < targetHeight)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                mc.hd.tool.jogMovePlus(head, -100, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    retMessage = ret.message;
                    moveError = true;
                    break; 
                }
                mc.idle(100);
            }

            // 6. 10 um 단위로 올라가며 찾기 시작
            mc.idle(3000);
            curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

            while (!moveError && curLoadcell > targetHeight)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                mc.hd.tool.jogMovePlus(head, 10, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    retMessage = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(500);
            }

            // 7. 1 um 단위로 내려가며 찾기 시작
            mc.idle(3000);
            curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

            while (!moveError && Math.Abs(curLoadcell - targetHeight) > 0.01)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
                mc.hd.tool.jogMovePlus(head, -1, out ret.message);
                if (ret.message != RetMessage.OK)
                {
                    mc.message.alarmMotion(ret.message);
                    retMessage = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(500);
            }

            // 8. 현재 위치를 반환한다.
            if(!moveError)
            {
                double heightValue = 0;
                mc.hd.tool.Z[head].actualPosition(out heightValue, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); retMessage = ret.message; return 0; }

                retMessage = RetMessage.OK;
                return heightValue;
            }
            else retMessage = ret.message;

            return 0;
        }

        private void BT_SAVEFILE_Click(object sender, EventArgs e)
        {
            try
            {
                string filename = "C:\\PROTEC\\DATA\\Calibration\\Force\\CalibData_" + selectedHead.ToString() + ".csv";
                
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);

                string saveData = "InputForce" + "," + "CalForce" + "," + "HeadForce" + "," + "HeightValue";
                sw.WriteLine(saveData);

                for (int i = 0; i < 20; i++)
                {
                    saveData = null;
                    saveData = selectedToolForce.forceLevel[i].value.ToString() + ","
                        + selectedToolForce.bottomForce[i].value.ToString() + ","
                        + selectedToolForce.topForce[i].value.ToString() + ","
                        + selectedToolForce.heightLevel[i].value.ToString();
                    sw.WriteLine(saveData);
                    sw.Flush();
                }
                sw.Close();
                fs.Close();
            }
            catch
            {

            }

        }
	}
}
