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

namespace PSA_Application
{
	public partial class FormJogPadZ : Form
	{
		public FormJogPadZ()
		{
			InitializeComponent();
		}

		public MP_HD_Z_MODE mode;
        public UnitCodeBoatMGSelect mgSelect;
        public UnitCodeSlotSelect slotSelect;
		public para_member dataZ;
        public int calHead;
		para_member _dataZ;
		RetValue ret;
		bool bStop;
		bool isRunning;
		object oButton;
		double dZ;
		double posZ;

		private void FormJogZ_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			_dataZ = dataZ;
			dZ = 10;
			refresh();
			this.Text = mode.ToString();

			#region mode check
			//if (mode == MP_HD_Z_MODE.REF)
			//{
			//    GB_.Visible = false;
			//    BT_JogZ_Down.Visible = false;
			//    BT_JogZ_Up.Visible = false;
			//    BT_SpeedZ.Visible = false;
			//    timer.Enabled = true;
			//    LB_SensorDetect.Visible = true;
			//}
            if (mode == MP_HD_Z_MODE.REF  || mode == MP_HD_Z_MODE.PEDESTAL)
            {
                timer.Enabled = true;
            }
			if (mode == MP_HD_Z_MODE.PICK)
			{
				timer.Enabled = true;
				LB_SensorDetect.Visible = true;
			}
			if (mode == MP_HD_Z_MODE.TOUCHPROBE)
			{
				timer.Enabled = true;
				LB_TouchProbe.Visible = true;
                LB_TouchProbe.Text = "Touch Probe :";
			}
            //if (mode == MP_HD_Z_MODE.LOADCELL)
            //{
            //    timer.Enabled = true;
            //    LB_TouchProbe.Visible = true;
            //    LB_TouchProbe.Text = "Loadcell :";
            //}
            if (mode == MP_HD_Z_MODE.HEIGHT_OFFSET)
            {
                BT_JogZ_Down.Visible = false;
                BT_JogZ_Up.Visible = false;
                BT_SpeedZ.Visible = false;

                BT_AutoCalibration.Visible = true;
            }
            if (mode == MP_HD_Z_MODE.MAGAZINE_CAL)
            {
                this.Text = "Magazine Z Offset";
                //LB_TouchProbe.Visible = true;
                //BT_Lighting.Visible = false;
            }
            if (mode == MP_HD_Z_MODE.MAGAZINE_READY)
            {
                this.Text = "Magazine Ready Offset";
            }
            //else
            //{
            //    BT_JogZ_Down.Visible = true;
            //    BT_JogZ_Up.Visible = true;
            //    BT_SpeedZ.Visible = true;

            //    BT_AutoCalibration.Visible = false;
            //}
			#endregion
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			if (sender.Equals(BT_ESC))
			{
				dataZ = _dataZ;
				timer.Enabled = false;
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_Set))
			{
				timer.Enabled = false;
				mc.idle(500);
				this.Close();
			}
			if (sender.Equals(BT_SpeedZ))
			{
				if (dZ == 1) dZ = 10;
				else if (dZ == 10) dZ = 100;
				else if (dZ == 100) dZ = 1000;
				else if (dZ == 1000) dZ = 1;
				else dZ = 1;
			}
			if (sender.Equals(BT_UpdateVolt))
			{
				double voltage = Convert.ToDouble(TB_OutVolt.Text);
				mc.AOUT.VPPM(voltage, out ret.message);
				TB_OutVolt.Text = voltage.ToString();
			}
			refresh();
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormJogPadZ_MouseDownThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormJogPadZ_MouseDownThread");
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			oButton = null;
			bStop = true;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			oButton = null;
			bStop = true;
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
				BT_SpeedZ.Text = "±" + dZ.ToString();
				TB_DataZ_Org.Text = _dataZ.value.ToString();
				TB_DataZ.Text = dataZ.value.ToString();
				TB_LowerLimitZ.Text = dataZ.lowerLimit.ToString();
				TB_UpperLimitZ.Text = dataZ.upperLimit.ToString();
				BT_ESC.Focus();
			}
		}

		void control()
		{
			isRunning = true;
			int interval = 300;
			while (true)
			{

				if (oButton == BT_JogZ_Down)
				{
					if (mode != MP_HD_Z_MODE.LOADCELL)
					{
						//mc.IN.HD.LOAD_CHK(out ret.b1, out ret.message); if (ret.b1) { mc.message.alarm("High Sensor Detected !!"); goto EXIT; }
					}
					dataZ.value -= dZ;
				}
				if (oButton == BT_JogZ_Up) dataZ.value += dZ;

				if (dataZ.value > dataZ.upperLimit) dataZ.value = dataZ.upperLimit;
				if (dataZ.value < dataZ.lowerLimit) dataZ.value = dataZ.lowerLimit;

				refresh();

				interval -= 50; if (interval < 50) interval = 50;
				mc.idle(interval);
				#region REF
				if (mode == MP_HD_Z_MODE.REF)
				{
					mc.para.CAL.z.ref0.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].REF0;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region ULC_FOCUS
				if (mode == MP_HD_Z_MODE.ULC_FOCUS)
				{
					mc.para.CAL.z.ulcFocus.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].ULC_FOCUS;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region XY_MOVING
				if (mode == MP_HD_Z_MODE.XY_MOVING)
				{
					mc.para.CAL.z.xyMoving.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].XY_MOVING;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PICK
				if (mode == MP_HD_Z_MODE.PICK)
				{
					mc.para.CAL.z.pick.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].PICK(UnitCodeSF.SF1);
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region PEDESTAL
				if (mode == MP_HD_Z_MODE.PEDESTAL)
				{
					mc.para.CAL.z.pedestal.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].PEDESTAL;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region TOUCHPROBE
				if (mode == MP_HD_Z_MODE.TOUCHPROBE)
				{
					mc.para.CAL.z.touchProbe.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].TOUCHPROBE;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
				#region LOADCELL
				if (mode == MP_HD_Z_MODE.LOADCELL)
				{
					mc.para.CAL.z.loadCell.value = dataZ.value;
					#region moving
                    posZ = mc.hd.tool.tPos.z[calHead].LOADCELL;
                    mc.hd.tool.jogMove(calHead, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
				}
				#endregion
                #region MAGAZINE
                if (mode == MP_HD_Z_MODE.MAGAZINE_CAL)
                {
                    mc.para.UD.MagazinePos[(int)mgSelect, (int)slotSelect].z.value = dataZ.value;
                    #region moving
                    if (mgSelect == UnitCodeBoatMGSelect.MG2)
                    {
                        if (slotSelect == UnitCodeSlotSelect.END) posZ = mc.unloader.Elev.pos.MG2_END;
                        else posZ = mc.unloader.Elev.pos.MG2_READY;
                    }
                    else if (mgSelect == UnitCodeBoatMGSelect.MG3)
                    {
                        if (slotSelect == UnitCodeSlotSelect.END) posZ = mc.unloader.Elev.pos.MG3_END;
                        else posZ = mc.unloader.Elev.pos.MG3_READY;
                    }
                    else
                    {
                        if (slotSelect == UnitCodeSlotSelect.END) posZ = mc.unloader.Elev.pos.MG1_END;
                        else posZ = mc.unloader.Elev.pos.MG1_READY;
                    }
                    mc.unloader.Elev.Z.move(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                }
                if (mode == MP_HD_Z_MODE.MAGAZINE_READY)
                {
                    mc.para.UD.ReadyPos.value = dataZ.value;
                    #region moving
                    posZ = mc.unloader.Elev.pos.READY;
                    mc.unloader.Elev.Z.move(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                    #endregion
                }
                #endregion

				if (bStop) break;
			}
		EXIT:
			isRunning = false;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Enabled = false;
			if (mode == MP_HD_Z_MODE.REF)
			{
				//mc.hd.tool.actualPosition_AxisZ(out ret.d, out ret.message);
				//dataZ.value = Math.Round(ret.d, 2);
				//TB_DataZ.Text = dataZ.value.ToString();

				//mc.IN.HD.LOAD_CHK(out ret.b, out ret.message);
				//LB_SensorDetect.Enabled = ret.b;
				ret.d1 = mc.AIN.HeadLoadcell();
				LB_VPPMVolt.Text = Math.Round(ret.d, 3).ToString();
				LB_LoadcellVolt.Text = Math.Round(ret.d1, 3).ToString();
			}
			if (mode == MP_HD_Z_MODE.PICK)
			{
				mc.IN.SF.TUBE_GUIDE(UnitCodeSF.SF1, out ret.b, out ret.message);
				LB_SensorDetect.Enabled = ret.b;
			}
			if (mode == MP_HD_Z_MODE.TOUCHPROBE)
			{
				mc.touchProbe.getData(out ret.d, out ret.b);
                LB_TouchProbe.Text = String.Format("Touch Probe : {0:F3}", ret.d.ToString());
			}
			if (mode == MP_HD_Z_MODE.LOADCELL)
			{
                //ret.d = mc.loadCell.getData((int)UnitCodeLoadcell.TOP1);
                //LB_TouchProbe.Text = String.Format(LB_TouchProbe.Text + " {0:F3}", ret.d.ToString());
                //ret.d1 = mc.AIN.HeadLoadcell();
                //LB_VPPMVolt.Text = Math.Round(ret.d, 3).ToString();
                //LB_LoadcellVolt.Text = Math.Round(ret.d1, 3).ToString();
			}
			timer.Enabled = true;
		}

        private void BT_AutoCalibration_Click(object sender, EventArgs e)
        {
            if (mode == MP_HD_Z_MODE.HEIGHT_OFFSET)
            {
                double head1 = 0;
                double head2 = 0;
                double posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LOADCELL;
                double posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LOADCELL;
                mc.hd.tool.jogMove(posX, posY, out ret.message);
                
                if (ret.message == RetMessage.OK)
                {
                    head1 = GetHeightOffset((int)UnitCodeHead.HD1, out ret.message);

                    if (ret.message == RetMessage.OK)
                    {
                        posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD2].LOADCELL;
                        posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD2].LOADCELL;
                        mc.hd.tool.jogMove(posX, posY, out ret.message);
                        if (ret.message == RetMessage.OK)
                        {
                            mc.idle(100);

                            head2 = GetHeightOffset((int)UnitCodeHead.HD2, out ret.message);
                            if (ret.message == RetMessage.OK)
                            {
                                dataZ.value = Math.Round(head1 - head2, 2);
                                mc.hd.tool.jogMove((int)UnitCodeHead.HD2, mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].XY_MOVING, out ret.message);
                            }
                            else mc.message.alarmMotion(ret.message);
                        }
                        else mc.message.alarmMotion(ret.message);
                    }
                    else mc.message.alarm("Invalid Loadcell Data!");
                }
                refresh();
            }
        }
	  
        private double GetHeightOffset(int head, out RetMessage message)
        {
            // 1. 무빙 위치로 이동
            double targetHeight = 1.0;

            posZ = mc.hd.tool.tPos.z[head].XY_MOVING;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return 0; }
            mc.idle(100);

            // 2. 로드셀 초기화 한다.
            mc.loadCell.setZero((int)UnitCodeLoadcell.CAL);
            mc.loadCell.setZero((int)UnitCodeLoadcell.TOP1);
            mc.loadCell.setZero((int)UnitCodeLoadcell.TOP2);

            // 3. 로드셀 위치에서 위로 1000 um 까지 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL + 1000;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return 0; }
            mc.idle(100);

            // 4. 로드셀 위치로 이동
            posZ = mc.hd.tool.tPos.z[head].LOADCELL;
            mc.hd.tool.jogMove(head, posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return 0; }

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
                    message = ret.message;
                    moveError = true;
                    break;
                }
                mc.idle(100);
            }

            // 6. 10 um 단위로 올라가며 찾기 시작
            mc.idle(500);

            curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);

            while (!moveError && curLoadcell > targetHeight)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
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

            while (!moveError && curLoadcell != targetHeight)
            {
                curLoadcell = mc.loadCell.getData((int)UnitCodeLoadcell.CAL);
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

            // 8. 현재 위치를 반환한다.
            if (!moveError)
            {
                double heightValue = 0;
                mc.hd.tool.Z[head].actualPosition(out heightValue, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); message = ret.message; return 0; }

                message = RetMessage.OK;
                return heightValue;
            }
            else message = ret.message;

            return 0;
        }
	}
}
