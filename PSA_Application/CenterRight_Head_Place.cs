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
	public partial class CenterRight_Head_Place : UserControl
	{
		public CenterRight_Head_Place()
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
		RetValue ret;

		private void Control_Click(object sender, EventArgs e)
		{
			if (!mc.check.READY_PUSH(sender)) return;
			mc.check.push(sender, true);

			#region search
			if (sender.Equals(BT_Search1st_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.search.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Search1st_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.search.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Search1st_Level)) mc.para.setting(mc.para.HD.place.search.level, out mc.para.HD.place.search.level);
			if (sender.Equals(TB_Search1st_Speed)) mc.para.setting(mc.para.HD.place.search.vel, out mc.para.HD.place.search.vel);
			if (sender.Equals(TB_Search1st_Delay)) mc.para.setting(mc.para.HD.place.search.delay, out mc.para.HD.place.search.delay);
			#endregion
			#region search2
			if (sender.Equals(BT_Search2nd_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.search2.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Search2nd_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.search2.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Search2nd_Level)) mc.para.setting(mc.para.HD.place.search2.level, out mc.para.HD.place.search2.level);
			if (sender.Equals(TB_Search2nd_Speed)) mc.para.setting(mc.para.HD.place.search2.vel, out mc.para.HD.place.search2.vel);
			if (sender.Equals(TB_Search2nd_Delay)) mc.para.setting(mc.para.HD.place.search2.delay, out mc.para.HD.place.search2.delay);
			#endregion
			#region delay
			if (sender.Equals(TB_Delay))
			{
				mc.para.setting(mc.para.HD.place.delay, out mc.para.HD.place.delay);
			}
			if (sender.Equals(TB_Force))
			{
				mc.para.setting(mc.para.HD.place.force, out mc.para.HD.place.force);

				// NT Style이던 아니던 모든 Z Offset값은 자동적으로 입력되어야 한다. Z Offset은 자동 계산..1kg에 500um이므로 
				//TB_ForceOffset_Z.Text = (-mc.para.HD.place.force.value * 500).ToString();
				//mc.para.HD.place.forceOffset.z.value = (-mc.para.HD.place.force.value * 500);
			}
			if (sender.Equals(TB_AirForce))
			{
				mc.para.setting(mc.para.HD.place.airForce, out mc.para.HD.place.airForce);
				// 모든 Force값들이 자동적으로 변경된다.
				//mc.para.HD.place.forceMode.force.value = mc.para.HD.place.airForce.value;
				//mc.para.HD.place.search.force.value = mc.para.HD.place.airForce.value;
				//mc.para.HD.place.search2.force.value = mc.para.HD.place.airForce.value;
				//mc.para.HD.place.driver.force.value = mc.para.HD.place.airForce.value;
				//mc.para.HD.place.driver2.force.value = mc.para.HD.place.airForce.value;
			}

            if (sender.Equals(TB_PRESS_FORCE))
            {
                mc.para.setting(mc.para.HD.press.force, out  mc.para.HD.press.force);
            }

            if (sender.Equals(TB_PRESS_TIME))
            {
                mc.para.setting(mc.para.HD.press.pressTime, out mc.para.HD.press.pressTime);
            }
			#endregion

            #region force Tracking
            if (sender.Equals(BT_ForceTracking_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.forceTracking.enable, (int)ON_OFF.ON);
            if (sender.Equals(BT_ForceTracking_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.forceTracking.enable, (int)ON_OFF.OFF);
            if (sender.Equals(TB_ForceTracking_Force)) mc.para.setting(mc.para.HD.place.forceTracking.force, out mc.para.HD.place.forceTracking.force);
            if (sender.Equals(TB_ForceTracking_Speed)) mc.para.setting(mc.para.HD.place.forceTracking.vel, out mc.para.HD.place.forceTracking.vel);
            #endregion

			#region driver
			if (sender.Equals(BT_Drive1st_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.driver.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Drive1st_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.driver.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Drive1st_Level))
			{
				mc.para.setting(mc.para.HD.place.driver.level, out mc.para.HD.place.driver.level);

				if (mc.para.HD.place.driver.level.value < (300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value))
				{
                    mc.message.inform("This value must be bigger than " + (300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value).ToString());
                    mc.para.HD.place.driver.level.value = 300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value;
				}
			}
			if (sender.Equals(TB_Drive1st_Speed)) mc.para.setting(mc.para.HD.place.driver.vel, out mc.para.HD.place.driver.vel);
			if (sender.Equals(TB_Drive1st_Delay)) mc.para.setting(mc.para.HD.place.driver.delay, out mc.para.HD.place.driver.delay);
			#endregion
			#region driver2
			if (sender.Equals(BT_Drive2nd_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.driver2.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_Drive2nd_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.driver2.enable, (int)ON_OFF.OFF);
			if (sender.Equals(TB_Drive2nd_Level)) mc.para.setting(mc.para.HD.place.driver2.level, out mc.para.HD.place.driver2.level);
			if (sender.Equals(TB_Drive2nd_Speed)) mc.para.setting(mc.para.HD.place.driver2.vel, out mc.para.HD.place.driver2.vel);
			if (sender.Equals(TB_Drive2nd_Delay)) mc.para.setting(mc.para.HD.place.driver2.delay, out mc.para.HD.place.driver2.delay);
			#endregion
			#region offset
			if (sender.Equals(TB_ForceOffset_Z))
			{
				//if (mc.para.HD.place.forceMode.mode.value == (int)PLACE_FORCE_MODE.SPRING)
				//{
					//mc.message.inform("CANNOT Change! This Valus is automatically calculated from Force Value.");
				//}
				//else
				//{
				mc.para.setting(mc.para.HD.place.forceOffset.z, out mc.para.HD.place.forceOffset.z);
				//}
			}
			if (sender.Equals(TB_PositionOffset_X)) mc.para.setting(mc.para.HD.place.offset.x, out mc.para.HD.place.offset.x);
			if (sender.Equals(TB_PositionOffset_Y)) mc.para.setting(mc.para.HD.place.offset.y, out mc.para.HD.place.offset.y);
			if (sender.Equals(TB_PositionOffset_Z)) mc.para.setting(mc.para.HD.place.offset.z, out mc.para.HD.place.offset.z);
			if (sender.Equals(TB_PositionOffset_T)) mc.para.setting(mc.para.HD.place.offset.t, out mc.para.HD.place.offset.t);

            if (sender.Equals(TB_PositionOffset_X2)) mc.para.setting(mc.para.HD.place.offset2.x, out mc.para.HD.place.offset2.x);
            if (sender.Equals(TB_PositionOffset_Y2)) mc.para.setting(mc.para.HD.place.offset2.y, out mc.para.HD.place.offset2.y);
            if (sender.Equals(TB_PositionOffset_T2)) mc.para.setting(mc.para.HD.place.offset2.t, out mc.para.HD.place.offset2.t);

			#endregion
			#region suction
			if (sender.Equals(BT_SuctionMode_Select_SearchLevelOff)) mc.para.setting(ref mc.para.HD.place.suction.mode, (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF);
			if (sender.Equals(BT_SuctionMode_Select_PlaceLevelOff)) mc.para.setting(ref mc.para.HD.place.suction.mode, (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF);
			if (sender.Equals(BT_SuctionMode_Select_PlaceEndOff)) mc.para.setting(ref mc.para.HD.place.suction.mode, (int)PLACE_SUCTION_MODE.PLACE_END_OFF);
			if (sender.Equals(BT_SuctionMode_Select_PlaceUpOff)) mc.para.setting(ref mc.para.HD.place.suction.mode, (int)PLACE_SUCTION_MODE.PLACE_UP_OFF);
			if (sender.Equals(TB_SuctionMode_Level)) mc.para.setting(mc.para.HD.place.suction.level, out mc.para.HD.place.suction.level);
			if (sender.Equals(TB_SuctionMode_SuctionOffDelay)) mc.para.setting(mc.para.HD.place.suction.delay, out mc.para.HD.place.suction.delay);
			if (sender.Equals(TB_SuctionMode_Purse)) mc.para.setting(mc.para.HD.place.suction.purse, out mc.para.HD.place.suction.purse);
			#endregion
			#region missCheck
			if (sender.Equals(BT_MissCheck_SelectOnOff_On)) mc.para.setting(ref mc.para.HD.place.missCheck.enable, (int)ON_OFF.ON);
			if (sender.Equals(BT_MissCheck_SelectOnOff_Off)) mc.para.setting(ref mc.para.HD.place.missCheck.enable, (int)ON_OFF.OFF);
			#endregion
			#region preForce
			//if (sender.Equals(BT_PreForceChange_SelectOn)) mc.para.HD.place.preForce.enable.value = (int)ON_OFF.ON;
			//if (sender.Equals(BT_PreForceChange_SelectOff)) mc.para.HD.place.preForce.enable.value = (int)ON_OFF.OFF;
			#endregion
			if (sender.Equals(TB_PressTiltLimit)) mc.para.setting(mc.para.HD.place.pressTiltLimit, out mc.para.HD.place.pressTiltLimit);
			mc.para.write(out ret.b); if (!ret.b) { mc.message.alarm("para write error"); }
			refresh();
			mc.main.Thread_Polling();
			mc.check.push(sender, false);
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
				#region search
				if (mc.para.HD.place.search.enable.value == (int)ON_OFF.ON)
				{
					LB_Search1st_Level.Visible = true; TB_Search1st_Level.Visible = true;
					LB_Search1st_Speed.Visible = true; TB_Search1st_Speed.Visible = true;
					LB_Search1st_Delay.Visible = true; TB_Search1st_Delay.Visible = true;
					BT_Search1st_SelectOnOff.Text = BT_Search1st_SelectOnOff_On.Text;
					BT_Search1st_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Search1st_Level.Text = mc.para.HD.place.search.level.value.ToString();
					TB_Search1st_Speed.Text = mc.para.HD.place.search.vel.value.ToString();
					TB_Search1st_Delay.Text = mc.para.HD.place.search.delay.value.ToString();
				}
				if (mc.para.HD.place.search.enable.value == (int)ON_OFF.OFF)
				{
					LB_Search1st_Level.Visible = false; TB_Search1st_Level.Visible = false;
					LB_Search1st_Speed.Visible = false; TB_Search1st_Speed.Visible = false;
					LB_Search1st_Delay.Visible = false; TB_Search1st_Delay.Visible = false;
					BT_Search1st_SelectOnOff.Text = BT_Search1st_SelectOnOff_Off.Text;
					BT_Search1st_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

                #region forceTracking
                if (mc.para.HD.place.forceTracking.enable.value == (int)ON_OFF.ON)
                {
                    LB_ForceTracking_Speed.Visible = true; TB_ForceTracking_Speed.Visible = true;
                    BT_ForceTracking_SelectOnOff.Text = BT_ForceTracking_SelectOnOff_On.Text;
                    BT_ForceTracking_SelectOnOff.Image = Properties.Resources.Yellow_LED;
                    LB_ForceTracking_Force.Visible = true; TB_ForceTracking_Force.Visible = true;
                    TB_ForceTracking_Force.Text = mc.para.HD.place.forceTracking.force.value.ToString();
                    TB_ForceTracking_Speed.Text = mc.para.HD.place.forceTracking.vel.value.ToString();
                }
                if (mc.para.HD.place.forceTracking.enable.value == (int)ON_OFF.OFF)
                {
                    LB_ForceTracking_Force.Visible = false; TB_ForceTracking_Force.Visible = false;
                    LB_ForceTracking_Speed.Visible = false; TB_ForceTracking_Speed.Visible = false;
                    BT_ForceTracking_SelectOnOff.Text = BT_ForceTracking_SelectOnOff_Off.Text;
                    BT_ForceTracking_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
                }
                #endregion
				#region search2
				if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.ON)
				{
					LB_Search2nd_Level.Visible = true; TB_Search2nd_Level.Visible = true;
					LB_Search2nd_Speed.Visible = true; TB_Search2nd_Speed.Visible = true;
					LB_Search2nd_Delay.Visible = true; TB_Search2nd_Delay.Visible = true;
					BT_Search2nd_SelectOnOff.Text = BT_Search2nd_SelectOnOff_On.Text;
					BT_Search2nd_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Search2nd_Level.Text = mc.para.HD.place.search2.level.value.ToString();
					TB_Search2nd_Speed.Text = mc.para.HD.place.search2.vel.value.ToString();
					TB_Search2nd_Delay.Text = mc.para.HD.place.search2.delay.value.ToString();
				}
				if (mc.para.HD.place.search2.enable.value == (int)ON_OFF.OFF)
				{
					LB_Search2nd_Level.Visible = false; TB_Search2nd_Level.Visible = false;
					LB_Search2nd_Speed.Visible = false; TB_Search2nd_Speed.Visible = false;
					LB_Search2nd_Delay.Visible = false; TB_Search2nd_Delay.Visible = false;
					BT_Search2nd_SelectOnOff.Text = BT_Search2nd_SelectOnOff_Off.Text;
					BT_Search2nd_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				if (mc.swcontrol.setupMode == 0)
				{
					TB_Delay.Text = mc.para.HD.place.delay.value.ToString();
					TS_SUCTION_MODE.Visible = false;

				}
				else
				{
					TB_Delay.Text = mc.para.HD.place.delay.value.ToString();
					TS_SUCTION_MODE.Visible = true;
				}
				TB_Force.Text = mc.para.HD.place.force.value.ToString();
				TB_AirForce.Text = mc.para.HD.place.airForce.value.ToString();
                TB_PRESS_FORCE.Text = mc.para.HD.press.force.value.ToString();
                TB_PRESS_TIME.Text = mc.para.HD.press.pressTime.value.ToString();

				#region driver
				if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.ON)
				{
					LB_Drive1st_Level.Visible = true; TB_Drive1st_Level.Visible = true;
					LB_Drive1st_Speed.Visible = true; TB_Drive1st_Speed.Visible = true;
					LB_Drive1st_Delay.Visible = true; TB_Drive1st_Delay.Visible = true;
					BT_Drive1st_SelectOnOff.Text = BT_Drive1st_SelectOnOff_On.Text;
					BT_Drive1st_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Drive1st_Level.Text = mc.para.HD.place.driver.level.value.ToString();
					TB_Drive1st_Speed.Text = mc.para.HD.place.driver.vel.value.ToString();
					TB_Drive1st_Delay.Text = mc.para.HD.place.driver.delay.value.ToString();

                    if (mc.para.HD.place.driver.level.value < (300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value))
                    {
                        mc.message.inform("1st Drive Level is automatically changed to " + (300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value).ToString());
                        mc.para.HD.place.driver.level.value = 300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value;
                    }
				}
				if (mc.para.HD.place.driver.enable.value == (int)ON_OFF.OFF)
				{
					LB_Drive1st_Level.Visible = false; TB_Drive1st_Level.Visible = false;
					LB_Drive1st_Speed.Visible = false; TB_Drive1st_Speed.Visible = false;
					LB_Drive1st_Delay.Visible = false; TB_Drive1st_Delay.Visible = false;
					BT_Drive1st_SelectOnOff.Text = BT_Drive1st_SelectOnOff_Off.Text;
					BT_Drive1st_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region driver2
				if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.ON)
				{
					LB_Drive2nd_Level.Visible = true; TB_Drive2nd_Level.Visible = true;
					LB_Drive2nd_Speed.Visible = true; TB_Drive2nd_Speed.Visible = true;
					LB_Drive2nd_Delay.Visible = true; TB_Drive2nd_Delay.Visible = true;
					BT_Drive2nd_SelectOnOff.Text = BT_Drive2nd_SelectOnOff_On.Text;
					BT_Drive2nd_SelectOnOff.Image = Properties.Resources.Yellow_LED;
					TB_Drive2nd_Level.Text = mc.para.HD.place.driver2.level.value.ToString();
					TB_Drive2nd_Speed.Text = mc.para.HD.place.driver2.vel.value.ToString();
					TB_Drive2nd_Delay.Text = mc.para.HD.place.driver2.delay.value.ToString();
				}
				if (mc.para.HD.place.driver2.enable.value == (int)ON_OFF.OFF)
				{
					LB_Drive2nd_Level.Visible = false; TB_Drive2nd_Level.Visible = false;
					LB_Drive2nd_Speed.Visible = false; TB_Drive2nd_Speed.Visible = false;
					LB_Drive2nd_Delay.Visible = false; TB_Drive2nd_Delay.Visible = false;
					BT_Drive2nd_SelectOnOff.Text = BT_Drive2nd_SelectOnOff_Off.Text;
					BT_Drive2nd_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region offset
				TB_ForceOffset_Z.Text = mc.para.HD.place.forceOffset.z.value.ToString();
				TB_PositionOffset_X.Text = mc.para.HD.place.offset.x.value.ToString();
				TB_PositionOffset_Y.Text = mc.para.HD.place.offset.y.value.ToString();
				TB_PositionOffset_Z.Text = mc.para.HD.place.offset.z.value.ToString();
				TB_PositionOffset_T.Text = mc.para.HD.place.offset.t.value.ToString();

                TB_PositionOffset_X2.Text = mc.para.HD.place.offset2.x.value.ToString();
                TB_PositionOffset_Y2.Text = mc.para.HD.place.offset2.y.value.ToString();
                TB_PositionOffset_T2.Text = mc.para.HD.place.offset2.t.value.ToString();
				#endregion

				#region suction
				if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.SEARCH_LEVEL_OFF)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_SearchLevelOff.Text;
					LB_SuctionMode_Level.Visible = true; TB_SuctionMode_Level.Visible = true;
					LB_SuctionMode_SuctionOffDelay.Visible = false; TB_SuctionMode_SuctionOffDelay.Visible = false;
					TB_SuctionMode_Level.Text = mc.para.HD.place.suction.level.value.ToString();
					TB_SuctionMode_Purse.Text = mc.para.HD.place.suction.purse.value.ToString();
				}
				if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_LEVEL_OFF)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_PlaceLevelOff.Text;
					LB_SuctionMode_Level.Visible = false; TB_SuctionMode_Level.Visible = false;
					LB_SuctionMode_SuctionOffDelay.Visible = false; TB_SuctionMode_SuctionOffDelay.Visible = false;
					TB_SuctionMode_Purse.Text = mc.para.HD.place.suction.purse.value.ToString();
				}
				if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_END_OFF)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_PlaceEndOff.Text;
					LB_SuctionMode_Level.Visible = false; TB_SuctionMode_Level.Visible = false;
					LB_SuctionMode_SuctionOffDelay.Visible = true; TB_SuctionMode_SuctionOffDelay.Visible = true;
					TB_SuctionMode_SuctionOffDelay.Text = mc.para.HD.place.suction.delay.value.ToString();
					TB_SuctionMode_Purse.Text = mc.para.HD.place.suction.purse.value.ToString();
				}
				if (mc.para.HD.place.suction.mode.value == (int)PLACE_SUCTION_MODE.PLACE_UP_OFF)
				{
					BT_SuctionMode_Select.Text = BT_SuctionMode_Select_PlaceUpOff.Text;
					LB_SuctionMode_Level.Visible = false; TB_SuctionMode_Level.Visible = false;
					LB_SuctionMode_SuctionOffDelay.Visible = true; TB_SuctionMode_SuctionOffDelay.Visible = true;
					TB_SuctionMode_SuctionOffDelay.Text = mc.para.HD.place.suction.delay.value.ToString();
					TB_SuctionMode_Purse.Text = mc.para.HD.place.suction.purse.value.ToString();
				}

				#endregion

				#region missCheck
				if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.ON)
				{
					BT_MissCheck_SelectOnOff.Text = BT_MissCheck_SelectOnOff_On.Text;
					BT_MissCheck_SelectOnOff.Image = Properties.Resources.Yellow_LED;
				}
				if (mc.para.HD.place.missCheck.enable.value == (int)ON_OFF.OFF)
				{
					BT_MissCheck_SelectOnOff.Text = BT_MissCheck_SelectOnOff_Off.Text;
					BT_MissCheck_SelectOnOff.Image = Properties.Resources.YellowLED_OFF;
				}
				#endregion

				#region preForce
				//if (mc.para.HD.place.preForce.enable.value == (int)ON_OFF.ON)
				//{
				//    BT_PreForce_Select.Text = BT_PreForceChange_SelectOn.Text;
				//}
				//if (mc.para.HD.place.preForce.enable.value == (int)ON_OFF.OFF)
				//{
				//    BT_PreForce_Select.Text = BT_PreForceChange_SelectOff.Text;
				//}
				#endregion

				#region z down distance
                TB_DownDistance.Text = Math.Round(mc.hd.tool.tPos.z[0].XY_MOVING - mc.hd.tool.tPos.z[0].PLACE).ToString();
				#endregion

				TB_PressTiltLimit.Text = mc.para.HD.place.pressTiltLimit.value.ToString();

				LB_.Focus();
			}
		}

		private void CenterRight_Head_Place_Load(object sender, EventArgs e)
		{
			// Force Mode에 따라 Visible 상태를 변경한다.
//			Setup Mode에 따라 Visible 상태 변경은 Refresh 에서 함. Setup Mode가 바뀔때마다 화면을 갱신해줘야 하므로..
// 			if (mc.swcontrol.setupMode == 0)
// 			{
// 				LB_ForceDownLevel.Visible = false;
// 				TB_ForceDownLevel.Visible = false;
// 
// 				LB_ForceMode_Force.Visible = false;
// 				TB_ForceMode_Force.Visible = false;
// 
// 				LB_ForceMode_SpeedPercent.Visible = false;
// 				TB_ForceMode_SpeedPercent.Visible = false;
// 
// 				LB_Search1st_Force.Visible = false;
// 				TB_Search1st_Force.Visible = false;
// 
// 				LB_Search2nd_Force.Visible = false;
// 				TB_Search2nd_Force.Visible = false;
// 
// 				LB_Drive1st_Force.Visible = false;
// 				TB_Drive1st_Force.Visible = false;
// 
// 				LB_Drive2nd_Force.Visible = false;
// 				TB_Drive2nd_Force.Visible = false;
// 
// 				TS_SUCTION_MODE.Visible = false;
// 
// 			}

			mc.para.HD.place.offset.z.value = 0.0;	// 20140710 Place Offset은 이제 필요없는 Factor가 됐다. Spring을 이제는 사용하지 않을 것이기 때문에...

			// mc.para.HD.place.forceOffset.z.value = (-mc.para.HD.place.force.value * 500);

			// Drive1 Value 자동 계산.
			// drive2는 Default Offset + Add Offset + BLT두께 + warpage 보다 큰 값으로 입력되어야 한다.
			// 250 + 500 + 110 + 110 = 
			// BLT하고 WARPAGE를 대량 300으로 잡으면...
			// 마지막으로 Linear Compensation을 사용한다면, Compensation되는 Z축 길이만큼도 같이 더해 주어야 한다.
			if (mc.para.HD.place.driver.level.value < (300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value))
			{
				mc.para.HD.place.driver.level.value = 300 - mc.para.HD.place.forceOffset.z.value - mc.para.HD.place.offset.z.value;
			}
		}
	}
}
