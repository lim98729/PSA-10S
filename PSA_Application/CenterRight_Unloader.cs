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
using AccessoryLibrary;

namespace PSA_Application
{
    public partial class CenterRight_Unloader : UserControl
    {
        public CenterRight_Unloader()
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
                try
                {
                    TB_PUSHER_X.Text = mc.para.UD.PusherPos.value.ToString();

                    BT_Magazine_Select.Text = unitCodeBoatMGSelect.ToString();
                    TB_MG_POS.Text = mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z.value.ToString();

                    BT_Slot_Select.Text = unitCodeSlotSelect.ToString();

                    TB_SLOT_COUNT.Text = mc.para.UD.slotCount.value.ToString();
                    TB_SLOT_PITCH.Text = mc.para.UD.slotPitch.value.ToString();

                    TB_MG_READY_POS.Text = mc.para.UD.ReadyPos.value.ToString();

                    if ((int)mc.para.UD.MagazineInCheck.value == 0) { BT_MG_IN_SENSOR_CHECK.Text = "OFF"; BT_MG_IN_SENSOR_CHECK.Image = Properties.Resources.YellowLED_OFF; }
                    else { BT_MG_IN_SENSOR_CHECK.Text = "ON"; BT_MG_IN_SENSOR_CHECK.Image = Properties.Resources.Yellow_LED; }

                    if ((int)mc.para.UD.AreaCheck.value == 0) { BT_MG_AREA_SENSOR_CHECK.Text = "OFF"; BT_MG_AREA_SENSOR_CHECK.Image = Properties.Resources.YellowLED_OFF; }
                    else { BT_MG_AREA_SENSOR_CHECK.Text = "ON"; BT_MG_AREA_SENSOR_CHECK.Image = Properties.Resources.Yellow_LED; }
                }
                catch (Exception error)
                {
                    mc.log.debug.write(mc.log.CODE.ERROR, "CenterRight_Unloader Error : (" + error.Message + "), " + error.StackTrace + "," + error.TargetSite + ", " + error.Source);
                }
            }
        }

        public UnitCodeBoatMGSelect unitCodeBoatMGSelect = UnitCodeBoatMGSelect.MG1;
        public UnitCodeSlotSelect unitCodeSlotSelect = UnitCodeSlotSelect.START;

        double posX, posZ;
        JOGMODE jogMode;
        MP_HD_Z_MODE mode;
        RetValue ret;
         
        private void Control_Click(object sender, EventArgs e)
        {
            //if (!mc.check.READY_AUTORUN(sender)) return;
            mc.check.push(sender, true);

            #region BT_PUSHER_CALIBRATION
            if (sender.Equals(BT_PUSHER_CALIBRATION))
            {
                posX = mc.ps.pos.PUSH;
                jogMode = JOGMODE.PUSHER;

                mc.ps.jogMove(posX, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }

                FormJogPad ff = new FormJogPad();
                ff.jogMode = JOGMODE.PUSHER;
                ff.dataX = mc.para.UD.PusherPos;
                ff.ShowDialog();
                mc.para.UD.PusherPos.value = ff.dataX.value;
                mc.para.setting(ref mc.para.UD.PusherPos, ff.dataX.value);
            }
            #endregion

            #region BT_MAGAZINE_CALIBRATION
            if (sender.Equals(BT_MAGAZINE_CALIBRATION))
            {
                if (unitCodeBoatMGSelect == UnitCodeBoatMGSelect.MG2)
                {
                    if (unitCodeSlotSelect == UnitCodeSlotSelect.START)
                        posZ = (double)MP_MG_Z.MG2_READY;
                    else posZ = (double)MP_MG_Z.MG2_END;
                }

                else if (unitCodeBoatMGSelect == UnitCodeBoatMGSelect.MG3)
                {
                    if (unitCodeSlotSelect == UnitCodeSlotSelect.START)
                        posZ = (double)MP_MG_Z.MG3_READY;
                    else posZ = (double)MP_MG_Z.MG3_END;
                }
                else
                {
                    if (unitCodeSlotSelect == UnitCodeSlotSelect.START)
                        posZ = (double)MP_MG_Z.MG1_READY;
                    else posZ = (double)MP_MG_Z.MG1_END;
                }
                posZ += mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z.value;
                
                mc.ps.jogMove(mc.ps.pos.READY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
                mc.unloader.Elev.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }

                FormJogPadZ ff = new FormJogPadZ();
                ff.mode = MP_HD_Z_MODE.MAGAZINE_CAL;
                ff.mgSelect = unitCodeBoatMGSelect;
                ff.slotSelect = unitCodeSlotSelect;
                ff.dataZ = mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z;

                ff.ShowDialog();

                mc.para.setting(ref mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z, ff.dataZ.value);
            }
            #endregion

            #region BT_MAGAZINE_READY
            if (sender.Equals(BT_MAGAZINE_READY_POS_CALIBRATION))
            {
                posZ = mc.unloader.Elev.pos.READY;

                mc.ps.jogMove(mc.ps.pos.READY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
                mc.unloader.Elev.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }

                FormJogPadZ ff = new FormJogPadZ();
                ff.mode = MP_HD_Z_MODE.MAGAZINE_READY;
                ff.dataZ = mc.para.UD.ReadyPos;
                ff.ShowDialog();

                mc.para.setting(ref mc.para.UD.ReadyPos, ff.dataZ.value);
            }
            #endregion

            if (sender.Equals(BT_MG_PITCH))
            {
                double tmpPitch1 = (mc.unloader.Elev.pos.MG1_END - mc.unloader.Elev.pos.MG1_READY) / (mc.para.UD.slotCount.value - 1);
                //double tmpPitch2 = (mc.mg.Elev.pos.MG2_END - mc.mg.Elev.pos.MG2_READY) / (mc.para.UD.slotCount.value - 1);
                //double tmpPitch3 = (mc.mg.Elev.pos.MG3_END - mc.mg.Elev.pos.MG3_READY) / (mc.para.UD.slotCount.value - 1);
                mc.para.UD.slotPitch.value = Math.Round(Math.Abs(tmpPitch1 / 1000), 2);
            }

        EXIT:
            bool r;
            mc.para.write(out r);
            if (!r)
            {
                mc.message.alarm("para write error");
            }
            
            refresh();
            mc.error.CHECK();
            mc.check.push(sender, false);
        }

        private void BT_MG_SELECT_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_MG1_SELECT)) unitCodeBoatMGSelect = UnitCodeBoatMGSelect.MG1;
            if (sender.Equals(BT_MG2_SELECT)) unitCodeBoatMGSelect = UnitCodeBoatMGSelect.MG2;
            if (sender.Equals(BT_MG3_SELECT)) unitCodeBoatMGSelect = UnitCodeBoatMGSelect.MG3;

            if (sender.Equals(BT_Slot_Start)) unitCodeSlotSelect = UnitCodeSlotSelect.START;
            if (sender.Equals(BT_Slot_End)) unitCodeSlotSelect = UnitCodeSlotSelect.END;

            refresh();
        }

        private void TextBox_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_PUSH(sender)) return;
            mc.check.push(sender, true);

            if (sender.Equals(TB_SLOT_COUNT))
            {
                mc.para.setting(mc.para.UD.slotCount, out mc.para.UD.slotCount);
                mc.UnloaderControl.MG_SLOT_COUNT = (int)mc.para.UD.slotCount.value;
                mc.UnloaderControl.writeconfig();
                EVENT.refreshMGSlot(mc.UnloaderControl.MG_SLOT_COUNT);
            }

            if (sender.Equals(TB_MG_POS))
            {
                mc.para.setting(mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z, out mc.para.UD.MagazinePos[(int)unitCodeBoatMGSelect, (int)unitCodeSlotSelect].z);
            }

            if (sender.Equals(TB_MG_READY_POS))
            {
                mc.para.setting(mc.para.UD.ReadyPos, out mc.para.UD.ReadyPos);
            }

            if (sender.Equals(TB_PUSHER_X))
            {
                mc.para.setting(mc.para.UD.PusherPos, out mc.para.UD.PusherPos);
            }
            mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
            refresh();
            mc.error.CHECK();

            mc.check.push(sender, false);

        }

        private void CenterRight_Unloader_Load(object sender, EventArgs e)
        {
            unitCodeSlotSelect = UnitCodeSlotSelect.START;

            refresh();
        }

        private void NoActionClick(object sender, EventArgs e)
        {
            mc.check.push(sender, true);

            if (sender.Equals(BT_MG_IN_SENSOR_CHECK))
            {
                if (mc.para.UD.MagazineInCheck.value == 0)
                    mc.para.setting(ref mc.para.UD.MagazineInCheck, 1);
                else
                    mc.para.setting(ref mc.para.UD.MagazineInCheck, 0);
            }
            if (sender.Equals(BT_MG_AREA_SENSOR_CHECK))
            {
                if (mc.para.UD.AreaCheck.value == 0)
                    mc.para.setting(ref mc.para.UD.AreaCheck, 1);
                else
                    mc.para.setting(ref mc.para.UD.AreaCheck, 0);
            }

            mc.check.push(sender, false);
        }
    }
}
