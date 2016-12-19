using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using System.Threading;
using DefineLibrary;

namespace PSA_Application
{
    public partial class BottomRight_Magazine : UserControl
    {
        public BottomRight_Magazine()
        {
            InitializeComponent();
            #region EVENT 등록
            EVENT.onAdd_mainFormPanelMode += new EVENT.InsertHandler_splitterMode(mainFormPanelMode);
            EVENT.onAdd_refreshMGSlot += new EVENT.InsertHandler_refreshMGSlot(refreshSlotCount);
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
        delegate void refresh_Call();
        Image image;
		 void refresh()
        {
            if (this.InvokeRequired)
            {
                refresh_Call d = new refresh_Call(refresh);
                this.BeginInvoke(d, new object[] { });
            }
            else
            {
                BT_PositionSelect_MG.Text = unitCodeMGSelect.ToString();
                BT_PositionSelect_Slot.Text = "SLOT(#" + selectSlot.ToString() + ")";

				mc.IN.MG.MG_AREA_SENSOR1(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_AREA_SENSOR.Image = image;

				mc.IN.MG.MG_RESET(out ret.b, out ret.message);
				if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
				else if (ret.b) image = Properties.Resources.Green_LED;
				else image = Properties.Resources.Green_LED_OFF;
				LB_IN_MG_RESET.Image = image;

                mc.IN.MG.MG_IN(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_MG_IN.Image = image;
            }
        }
        #endregion

        #region refresh Slot Count
        delegate void InsertHandler_refreshMGSlotCall(int slotCount);
        void refreshSlotCount(int slotCount)
        {
            if (this.InvokeRequired)
            {
                InsertHandler_refreshMGSlotCall d = new InsertHandler_refreshMGSlotCall(refreshSlotCount);
                this.BeginInvoke(d, new object[] {slotCount});
            }
            else
            {
                BT_PositionSelect_Slot.DropDownItems.Clear();
                for (int i = 0; i < mc.UnloaderControl.MG_SLOT_COUNT; i++)
                {
                    BT_PositionSelect_Slot.DropDownItems.Add("Slot " + (i + 1).ToString(), null, SLOT_Click);
                }
                if (selectSlot > mc.UnloaderControl.MG_SLOT_COUNT)
                {
                    selectSlot = 1;
                    refresh();
                }
            }
        }
        #endregion

        RetValue ret;
        double posZ;
        int selectSlot = 1;
        public UnitCodeBoatMGSelect unitCodeMGSelect = UnitCodeBoatMGSelect.MG1;

        ToolStripMenuItem[] BT_MG_SLOT = new ToolStripMenuItem[20];
        private void BottomRight_Magazine_Load(object sender, EventArgs e)
        {
            int slotCount = mc.UnloaderControl.MG_SLOT_COUNT;

            for (int i = 0; i < slotCount; i++ )
            {
                BT_PositionSelect_Slot.DropDownItems.Add("Slot " + (i + 1).ToString(), null, SLOT_Click);
            }
        }
   
        private void BT_MG_MOVE_Click(object sender, EventArgs e)
        {
            if (!mc.check.READY_AUTORUN(sender)) return;
            //mc.user.selected_IO_Menu = false;
            mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);

			if (ret.b2)
			{
				mc.message.alarm("Boat Sensor Detect !!");
				goto EXIT;
			}

            if (unitCodeMGSelect == UnitCodeBoatMGSelect.MG1) posZ = mc.unloader.Elev.pos.MG1_READY;
            else if (unitCodeMGSelect == UnitCodeBoatMGSelect.MG2) posZ = mc.unloader.Elev.pos.MG2_READY;
            else if (unitCodeMGSelect == UnitCodeBoatMGSelect.MG3) posZ = mc.unloader.Elev.pos.MG3_READY;

			posZ -= mc.para.UD.slotPitch.value * 1000 * (selectSlot - 1);

			mc.ps.jogMove(mc.ps.pos.READY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
			
			mc.unloader.Elev.jogMove(posZ, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }

         EXIT:
            mc.main.Thread_Polling();
            mc.check.push(sender, false);
        }
	
		void timer_Tick(object sender, EventArgs e)
        {
            refresh();
        }

        private void SLOT_Click(object sender, EventArgs e)
        {
            string senderName = sender.ToString();
            string[] temp = senderName.Split(' ');
            selectSlot = Convert.ToInt32(temp[1]);

            refresh();
        }

        private void MG_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_MG1_SELECT)) unitCodeMGSelect = UnitCodeBoatMGSelect.MG1;
            else if (sender.Equals(BT_MG2_SELECT)) unitCodeMGSelect = UnitCodeBoatMGSelect.MG2;
            else if (sender.Equals(BT_MG3_SELECT)) unitCodeMGSelect = UnitCodeBoatMGSelect.MG3;
        }
    }
}