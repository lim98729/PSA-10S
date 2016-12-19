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
	public partial class BottomRight_Pusher : UserControl
	{
		public BottomRight_Pusher()
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
                mc.IN.PS.UP(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_UP.Image = image;
                
                mc.IN.PS.DOWN(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_DOWN.Image = image;

                mc.IN.PS.JAM(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_JAM.Image = image;

                mc.IN.PS.READY(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_READY.Image = image;

                mc.IN.PS.END(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.Green_LED;
                else image = Properties.Resources.Green_LED_OFF;
                LB_IN_END.Image = image;

                mc.OUT.PS.PS_UPDOWN(out ret.b, out ret.message);
                if (ret.message != RetMessage.OK) image = Properties.Resources.Fail;
                else if (ret.b) image = Properties.Resources.yellow_ball;
                else image = Properties.Resources.gray_ball;
                BT_OUT_PS_UPDOWN.Image = image;
			}
		}
		#endregion
		RetValue ret;
        double posX;

		private void timer_Tick(object sender, EventArgs e)
		{
			refresh();
		}

        private void PUSHER_READY_MOVE(object sender, EventArgs e)
        {
            //if (!mc.check.READY_AUTORUN(sender)) return;
            //mc.user.selected_IO_Menu = false;
            mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);

			mc.IN.PS.READY(out ret.b, out ret.message);
			if (ret.b) { mc.message.alarm("Boat In Sensor Detected!!"); goto EXIT; }

            posX = mc.ps.pos.READY;
            mc.ps.jogMove(posX, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }
      
        EXIT:
            mc.main.Thread_Polling();
            mc.check.push(sender, false);
        }

        private void PUSHER_PSUH_MOVE(object sender, EventArgs e)
        {
            //if (!mc.check.READY_AUTORUN(sender)) return;
            //mc.user.selected_IO_Menu = false;
            mc.check.push(sender, true, (int)SelectedMenu.BOTTOM_RIGHT);

			mc.IN.PS.READY(out ret.b, out ret.message);
			if (ret.b) { mc.message.alarm("Boat In Sensor Detected!!"); goto EXIT; }
			posX = mc.ps.pos.PUSH;
            mc.ps.jogMove(posX, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarm("Motion Error : " + ret.message.ToString()); goto EXIT; }

        EXIT:
            mc.main.Thread_Polling();
            mc.check.push(sender, false);
        }

        private void Out_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_OUT_PS_UPDOWN))
            {
                mc.OUT.PS.PS_UPDOWN(out ret.b, out ret.message);
                if (ret.message == RetMessage.OK) mc.OUT.PS.PS_UPDOWN(!ret.b, out ret.message);
            }
        }
	}
}
