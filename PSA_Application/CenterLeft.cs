﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using HalconDotNet;
using AccessoryLibrary;
using System.IO;

namespace PSA_Application
{
	public partial class CenterLeft : UserControl
	{
		public CenterLeft()
		{
			InitializeComponent();

			#region EVENT 등록
			EVENT.onAdd_hWindowInitialize += new EVENT.InsertHandler(hWindowInitialize);
			EVENT.onAdd_hWindowClose += new EVENT.InsertHandler(hWindowClose);
			EVENT.onAdd_hWindowLargeDisplay += new EVENT.InsertHandler_Htuple(hWindowLargeDisplay);
			EVENT.onAdd_hWindow2Display += new EVENT.InsertHandler(hWindow2Display);
			EVENT.onAdd_hWindow2DisplayClear += new EVENT.InsertHandler(hWindow2DisplayClear);
			EVENT.onAdd_hWindowAdvanceMode += new EVENT.InsertHandler_bool(hWindowAdvanceMode);
			EVENT.onAdd_refresh += new EVENT.InsertHandler(refresh);
			EVENT.onAdd_boardActivate += new EVENT.InsertHandler_boardActivate(boardActivate);
			EVENT.onAdd_padChange += new EVENT.InsertHandler_padChange(padChange);
			EVENT.onAdd_tubeChange += new EVENT.InsertHandler_tubeChange(tubeChange);
			EVENT.onAdd_TerminalDisplay += new EVENT.InsertHandler_string(terminalDisplay);
            EVENT.onAdd_refreshShowMagazine += new EVENT.InsertHandler_bool2(refreshShowMagazine);
            EVENT.onAdd_refreshEditMagazine += new EVENT.InsertHandler_int2(refreshEditMagazine);
			#endregion
		}
        Panel panel;
        RetValue ret;
		bool worktrayrefresh;
		int attachCnt;
		int skipCnt;
		int readyCnt;
		int failCnt;
		int totalCnt;
		DateTime loadTime;
		TimeSpan diffTime;
        string MG_BackupTMSWrite = "";
        bool mgResetOn = false;

		#region EVENT용 delegate 함수
		delegate void hWindowInitialize_Call();
		void hWindowInitialize()
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindowInitialize_Call d = new hWindowInitialize_Call(hWindowInitialize);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWindow.hWindowInitialize();
			}
		}

		delegate void hWindowClose_Call();
		void hWindowClose()
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindowClose_Call d = new hWindowClose_Call(hWindowClose);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWindow.hWindowClose();
			}
		}

		delegate void hWindowLargeDisplay_Call(HTuple camNumber);
		void hWindowLargeDisplay(HTuple camNumber)
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindowLargeDisplay_Call d = new hWindowLargeDisplay_Call(hWindowLargeDisplay);
				this.BeginInvoke(d, new object[] { camNumber });
			}
			else
			{
                // 160627. jhlim: 여기서 숨기기
				BoardStatus_LoadingZone.Visible = false;
				BoardStatus_WorkingZone.Visible = false;
				BoardStatus_UnloadingZone.Visible = false;
				stackFeederStatus.Visible = false;
				LB_InbufConv.Visible = false;
				LB_WorkConv.Visible = false;
				LB_OutbufConv.Visible = false;
				LB_TubeStatus.Visible = false;

                LB_MG1.Visible = false;
                LB_MG2.Visible = false;
                LB_MG3.Visible = false;
                Panel_MG1.Visible = false;
                Panel_MG2.Visible = false;
                Panel_MG3.Visible = false;
				if (dev.NotExistHW.CAMERA) return;
				hWindow.hWindowLargeDisplay(camNumber);
			}
		   
		}

		delegate void hWindow2Display_Call();
		void hWindow2Display()
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindow2Display_Call d = new hWindow2Display_Call(hWindow2Display);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
                // 160627. jhlim: 여기서 보이기
				hWindow.hWindow2Display();
				BoardStatus_LoadingZone.Visible = true;
				BoardStatus_WorkingZone.Visible = true;
				BoardStatus_UnloadingZone.Visible = true;
				stackFeederStatus.Visible = true;
				LB_InbufConv.Visible = true;
				LB_WorkConv.Visible = true;
				LB_OutbufConv.Visible = true;
				LB_TubeStatus.Visible = true;

                if (mc.para.ETC.unloaderControl.value > 0)
                {
                    LB_MG1.Visible = true;
                    LB_MG2.Visible = true;
                    LB_MG3.Visible = true;
                    Panel_MG1.Visible = true;
                    Panel_MG2.Visible = true;
                    Panel_MG3.Visible = true;
                }
			}
		}

		delegate void hWindow2DisplayClear_Call();
		void hWindow2DisplayClear()
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindow2DisplayClear_Call d = new hWindow2DisplayClear_Call(hWindow2DisplayClear);
				this.BeginInvoke(d, new object[] { });
			}
			else
			{
				hWindow.hWindow2DisplayClear();
			}
		}

		delegate void hWindowAdvanceMode_Call(bool ADVANCE_MODE);
		void hWindowAdvanceMode(bool ADVANCE_MODE)
		{
			if (this.hWindow.InvokeRequired)
			{
				hWindowAdvanceMode_Call d = new hWindowAdvanceMode_Call(hWindowAdvanceMode);
				this.BeginInvoke(d, new object[] { ADVANCE_MODE });
			}
			else
			{
				hWindow.ADVANCE_MODE = ADVANCE_MODE;
			}
		}

		FormTerminalMessage termianlForm = null;
		delegate void termianlDisplay_Call(string msg);
		void terminalDisplay(string msg)
		{
			if (this.hWindow.InvokeRequired)
			{
				termianlDisplay_Call d = new termianlDisplay_Call(terminalDisplay);
				this.BeginInvoke(d, new object[] { msg });
			}
			else
			{
				if (FormTerminalMessage.isAlive == false)
				{
					termianlForm = new FormTerminalMessage();
					termianlForm.MB_TERMINAL_MSG.Text = msg;
					termianlForm.Show();
				}
				else
				{
					termianlForm.MB_TERMINAL_MSG.Text = msg;
					termianlForm.BringToFront();
				}
			}
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
				TB_PROD_RecipeName.Text = mc.commMPC.WorkData.receipeName;

				if (mc.para.mmiOption.editMode) LB_WorkConv.ForeColor = Color.RoyalBlue;
				else LB_WorkConv.ForeColor = Color.Black;

				worktrayrefresh = false;

				//if (mc.user.IS_ABOVE.developer || dev.debug) BT_hWindowAdvanceMode.Visible = true;
				//else BT_hWindowAdvanceMode.Visible = false;

				if (mc.board.loading.tmsInfo.LotID.S == "INVALID")
					TB_PROD_Inbuf_LotID.Text = "";
				else
					TB_PROD_Inbuf_LotID.Text = mc.board.loading.tmsInfo.LotID;

				if (mc.board.loading.tmsInfo.TrayID.S == "INVALID")
					TB_PROD_Inbuf_TrayID.Text = "";
				else
					TB_PROD_Inbuf_TrayID.Text = mc.board.loading.tmsInfo.TrayID;

				//if (mc.board.loading.tmsInfo.LotQTY.Type == HTupleType.STRING)
				//    TB_PROD_Inbuf_TrayQty.Text = "0";
				//else
				//    TB_PROD_Inbuf_TrayQty.Text = mc.board.loading.tmsInfo.LotQTY.I.ToString();

				if (mc.board.loading.tmsInfo.TrayType.Type == HTupleType.STRING)
				{
					TB_PROD_Inbuf_TrayType.Text = "";
					GB_InputTray.ForeColor = Color.DimGray;
				}
				else
				{
					if (mc.board.loading.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY)
						TB_PROD_Inbuf_TrayType.Text = "COVER";
					else if (mc.board.loading.tmsInfo.TrayType.I == (int)TRAY_TYPE.LAST_TRAY)
						TB_PROD_Inbuf_TrayType.Text = "NORMAL END";
					else if (mc.board.loading.tmsInfo.TrayType.I == (int)TRAY_TYPE.NOMAL_TRAY)
						TB_PROD_Inbuf_TrayType.Text = "NORMAL";
					else
						TB_PROD_Inbuf_TrayType.Text = "";
					GB_InputTray.ForeColor = Color.Black;
				}
				if (mc.board.loading.loadingTime.Type == HTupleType.STRING)
				{
					if (mc.board.loading.loadingTime == "INVALID")
						TB_PROD_Inbuf_ElapsedTime.Text = "";
					else
					{
						loadTime = Convert.ToDateTime(mc.board.loading.loadingTime.S);
						diffTime = DateTime.Now - loadTime;
						TB_PROD_Inbuf_ElapsedTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", diffTime.Hours, diffTime.Minutes, diffTime.Seconds);
					}
				}

				if (mc.board.working.tmsInfo.LotID.S == "INVALID")
					TB_PROD_Work_LotID.Text = "";
				else
					TB_PROD_Work_LotID.Text = mc.board.working.tmsInfo.LotID;

				if (mc.board.working.tmsInfo.TrayID.S == "INVALID")
					TB_PROD_Work_TrayID.Text = "";
				else
					TB_PROD_Work_TrayID.Text = mc.board.working.tmsInfo.TrayID;

				// Display Tray Count
				TB_PROD_Work_TrayCount.Text = mc.para.runInfo.trayLotCount.ToString();
				
				if (mc.board.working.tmsInfo.LotQTY.Type == HTupleType.STRING)
					TB_PROD_Work_TrayQty.Text = "0";
				else
					TB_PROD_Work_TrayQty.Text = mc.board.working.tmsInfo.LotQTY.I.ToString();

				TB_PROD_Today_TrayCount.Text = mc.para.runInfo.trayTodayCount.ToString();

				// Display Working Area Tray Type
				if (mc.board.working.tmsInfo.TrayType.Type == HTupleType.STRING)
				{
					TB_PROD_Work_TrayType.Text = "";
					GB_WorkTray.ForeColor = Color.DimGray;
					GB_ProdSet.ForeColor = Color.DimGray;
					GB_TrayProd.ForeColor = Color.DimGray;
					GB_TrayCount.ForeColor = Color.DimGray;
				}
				else
				{
					if (mc.board.working.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY)
						TB_PROD_Work_TrayType.Text = "COVER";
					else if (mc.board.working.tmsInfo.TrayType.I == (int)TRAY_TYPE.LAST_TRAY)
					{
						TB_PROD_Work_TrayType.Text = "NORMAL END";
						worktrayrefresh = true;
					}
					else if (mc.board.working.tmsInfo.TrayType.I == (int)TRAY_TYPE.NOMAL_TRAY)
					{
						TB_PROD_Work_TrayType.Text = "NORMAL";
						worktrayrefresh = true;
					}
					else
						TB_PROD_Work_TrayType.Text = "";
					GB_WorkTray.ForeColor = Color.Black;
					GB_ProdSet.ForeColor = Color.Black;
					GB_TrayProd.ForeColor = Color.Black;
					GB_TrayCount.ForeColor = Color.Black;
				}
				if (mc.board.working.loadingTime.Type == HTupleType.STRING)
				{
					if (mc.board.working.loadingTime == "INVALID")
						TB_PROD_Work_ElapsedTime.Text = "";
					else
					{
						loadTime = Convert.ToDateTime(mc.board.working.loadingTime.S);
						diffTime = DateTime.Now - loadTime;
						TB_PROD_Work_ElapsedTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", diffTime.Hours, diffTime.Minutes, diffTime.Seconds);
					}
				}

				if (worktrayrefresh)
				{
					mc.board.trayStatus(BOARD_ZONE.WORKING, out readyCnt, out skipCnt, out attachCnt, out failCnt, out totalCnt);

					TB_PROD_Work_SkipCnt.Text = skipCnt.ToString();
					TB_PROD_Work_FailCnt.Text = failCnt.ToString();
					TB_PROD_Work_AttachCnt.Text = attachCnt.ToString();
					TB_PROD_Work_Progress.Text = String.Format("{0}/{1}", (skipCnt + attachCnt), totalCnt);

					PB_AttachProgress.Visible = true;
					if(totalCnt > 0)
						PB_AttachProgress.Value = (int)((skipCnt + attachCnt) * 100 / (double)totalCnt);
				}
				else
				{
					TB_PROD_Work_SkipCnt.Text = "";
					TB_PROD_Work_FailCnt.Text = "";
					TB_PROD_Work_AttachCnt.Text = "";
					TB_PROD_Work_Progress.Text = "";

					PB_AttachProgress.Value = 0;
					PB_AttachProgress.Visible = false;
				}

				TB_PROD_Work_SetForce.Text = mc.para.HD.place.force.value.ToString();
                if (mc.hd.tool.placeForce < 0.01)
					TB_PROD_Work_PreForce.Text = "";
				else
                    TB_PROD_Work_PreForce.Text = Math.Round(mc.hd.tool.placeForce, 2).ToString("f2");
                if (mc.hd.tool.placeForce < 0.01)
					TB_PROD_Work_PostForce.Text = "";
				else
                    TB_PROD_Work_PostForce.Text = Math.Round(mc.hd.tool.placeForce, 2).ToString("f2");
				TB_PROD_Work_PlaceTime.Text = mc.para.HD.place.delay.value.ToString();
				TB_PROD_Work_SetSpeed1.Text = mc.para.HD.place.search.vel.value.ToString();
				TB_PROD_Work_SetSpeed2.Text = mc.para.HD.place.search2.vel.value.ToString();
				TB_PROD_Work_HDC_Score.Text = mc.para.HDC.modelPADC1.passScore.value.ToString();
				TB_PROD_Work_ULC_Score.Text = mc.para.ULC.model.passScore.value.ToString();

				if(mc.board.unloading.tmsInfo.LotID.S == "INVALID")
					TB_PROD_Outbuf_LotID.Text = "";
				else
					TB_PROD_Outbuf_LotID.Text = mc.board.unloading.tmsInfo.LotID.S;

				if(mc.board.unloading.tmsInfo.TrayID.S == "INVALID")
					TB_PROD_Outbuf_TrayID.Text = "";
				else
					TB_PROD_Outbuf_TrayID.Text = mc.board.unloading.tmsInfo.TrayID.S;

				//if (mc.board.unloading.tmsInfo.LotQTY.Type == HTupleType.STRING)
				//    TB_PROD_Outbuf_TrayQty.Text = "0";
				//else
				//    TB_PROD_Outbuf_TrayQty.Text = mc.board.unloading.tmsInfo.LotQTY.I.ToString();

				if (mc.board.unloading.tmsInfo.TrayType.Type == HTupleType.STRING)
				{
					TB_PROD_Outbuf_TrayType.Text = "";
					GB_OutputTray.ForeColor = Color.DimGray;
				}
				else
				{
					if (mc.board.unloading.tmsInfo.TrayType.I == (int)TRAY_TYPE.COVER_TRAY)
						TB_PROD_Outbuf_TrayType.Text = "COVER";
					else if (mc.board.unloading.tmsInfo.TrayType.I == (int)TRAY_TYPE.LAST_TRAY)
						TB_PROD_Outbuf_TrayType.Text = "NORMAL END";
					else if (mc.board.unloading.tmsInfo.TrayType.I == (int)TRAY_TYPE.NOMAL_TRAY)
						TB_PROD_Outbuf_TrayType.Text = "NORMAL";
					else
						TB_PROD_Outbuf_TrayType.Text = "";

					GB_OutputTray.ForeColor = Color.Black;
				}

				if (mc.board.unloading.loadingTime.Type == HTupleType.STRING)
				{
					if(mc.board.unloading.loadingTime == "INVALID")
						TB_PROD_Outbuf_ElapsedTime.Text = "";
					else
					{
						loadTime = Convert.ToDateTime(mc.board.unloading.loadingTime.S);
						diffTime = DateTime.Now - loadTime;
						TB_PROD_Outbuf_ElapsedTime.Text = String.Format("{0:d2}:{1:d2}:{2:d2}", diffTime.Hours, diffTime.Minutes, diffTime.Seconds);
					}
				}

                if (!mc.main.THREAD_RUNNING)
                {
                    //if (!mgResetOn)
                    //{
                    //    mc.IN.MG.MG_RESET(out ret.b, out ret.message);
                    //    if (ret.b)
                    //    {
                    //        mgResetOn = true;
                    //        for (int i = 0; i < mc.UnloaderControl.MG_COUNT; i++)
                    //        {
                    //            for (int j = 0; j < mc.UnloaderControl.MG_SLOT_COUNT; j++)
                    //            {
                    //                mc.idle(0);
                    //                mc.UnloaderControl.MG_Status[i, j] = (int)MG_STATUS.READY;
                    //                EVENT.refreshEditMagazine(i, j);
                    //            }
                    //        }
                    //        mc.UnloaderControl.writeconfig();
                    //    }
                    //}
                    //else
                    //{
                    //    mc.IN.MG.MG_RESET(out ret.b, out ret.message);
                    //    if (!ret.b)
                    //    {
                    //        mgResetOn = false;
                    //    }
                    //}
                }
			}
		}
        
		delegate void boardActivate_Call(BOARD_ZONE zone, int x, int y);
		void boardActivate(BOARD_ZONE zone, int x, int y)
		{
			if (this.InvokeRequired)
			{
				boardActivate_Call d = new boardActivate_Call(boardActivate);
				this.BeginInvoke(d, new object[] { zone, x, y });
			}
			else
			{
				//mc.board.activate(mc.para.MT.padCount.x.value, mc.para.MT.padCount.y.value);
				
				// Kenny Check..2013-07-17 기껏 읽어놓고 초기화는 왜 한다냐?
				//bool b;
				//mc.board.initialize(out b);
				
				if (zone == BOARD_ZONE.LOADING)
					BoardStatus_LoadingZone.activate(mc.para.mcType.FrRr, BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				if (zone == BOARD_ZONE.WORKING)
					BoardStatus_WorkingZone.activate(mc.para.mcType.FrRr, BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
				if (zone == BOARD_ZONE.UNLOADING)
					BoardStatus_UnloadingZone.activate(mc.para.mcType.FrRr, BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
			}
		}

		FormTrayEdit ff;
		delegate void padChange_Call(BOARD_ZONE zone, int x, int y);
		void padChange(BOARD_ZONE zone, int x, int y)
		{
			if (this.InvokeRequired)
			{
				padChange_Call d = new padChange_Call(padChange);
				this.BeginInvoke(d, new object[] { zone, x, y });
			}
			else
			{
// 				if (mc.para.ETC.passwordProtect.value == 1)
// 				{
// 					if (mc.user.logInDone == false)
// 					{
// 						FormLogIn ff = new FormLogIn();
// 						ff.ShowDialog();
// 
// 						if (FormLogIn.logInCheck == false) return;
// 					}
// 				}
				if (FormTrayEdit.IsDisplayed)
				{
					//mc.board.workingedit = mc.board.working;
					ff.UpdateSelectedPad(x, y);
					ff.indexRow = x;
					ff.indexColumn = y;
					ff.TopLevel = true;
					ff.BringToFront();
					//ff.editFlag = false;
					ff.refresh();
				}
				else
				{
					ff = new FormTrayEdit();
					ff.indexRow = x;
					ff.indexColumn = y;
					ff.TopLevel = true;
					ff.Show();
				}
			}
		}

		delegate void tubeChange_Call();
		void tubeChange()
		{
			if (this.InvokeRequired)
			{
				tubeChange_Call d = new tubeChange_Call(tubeChange);
				this.BeginInvoke(d, new object[] {  });
			}
			else
			{
				FormTubeEdit ff = new FormTubeEdit();
				ff.ShowDialog();
			}
		}

        System.Windows.Forms.Button[,] MGbtnArray;
        System.Windows.Forms.Button[,] BoatbtnArrary;
        string[,] BoatbtnStatusArray;
        delegate void refreshMagazine_Call(bool remove, bool show);
        public void refreshShowMagazine(bool remove, bool show)
        {
            if (this.InvokeRequired)
            {
                refreshMagazine_Call d = new refreshMagazine_Call(refreshShowMagazine);
                this.BeginInvoke(d, new object[] { remove, show });
            }
            else
            {
                try
                {
                    if (remove)
                    {
                        for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
                            for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
                            {
                                if (x == 0) panel = Panel_MG1;
                                else if (x == 1) panel = Panel_MG2;
                                else if (x == 2) panel = Panel_MG3;
                                panel.Controls.Remove(MGbtnArray[x, y]);
                            }
                    }
                    if (show)
                    {
                        MGbtnArray = new System.Windows.Forms.Button[mc.UnloaderControl.MG_COUNT, mc.UnloaderControl.MG_SLOT_COUNT];
                        BoatbtnArrary = new System.Windows.Forms.Button[(int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value];
                        BoatbtnStatusArray = new string[(int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value];
                        int sX, sY, size;
                        int ox, oy;


                        for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
                        {
                            for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
                            {
                                if (x == 0) panel = Panel_MG1;
                                else if (x == 1) panel = Panel_MG2;
                                else if (x == 2) panel = Panel_MG3;

                                sY = (int)(panel.Size.Height / mc.UnloaderControl.MG_SLOT_COUNT);

                                MGbtnArray[x, y] = new System.Windows.Forms.Button();
                                MGbtnArray[x, y].Tag = (y + 1).ToString();
                                MGbtnArray[x, y].Width = panel.Size.Width - 2;
                                MGbtnArray[x, y].Height = sY - 1;
                                MGbtnArray[x, y].Top = (sY * (mc.UnloaderControl.MG_SLOT_COUNT - 1 - y));
                                panel.Controls.Add(MGbtnArray[x, y]);

                                if (mc.UnloaderControl.MG_Status[x, y] == (int)MG_STATUS.DONE) MGbtnArray[x, y].BackColor = Color.LimeGreen;
                                else if (mc.UnloaderControl.MG_Status[x, y] == (int)MG_STATUS.READY) MGbtnArray[x, y].BackColor = Color.Yellow;
                                else if (mc.UnloaderControl.MG_Status[x, y] == (int)MG_STATUS.SKIP) MGbtnArray[x, y].BackColor = Color.Black;

                                MGbtnArray[x, y].MouseDown += new MouseEventHandler(ClickButton);
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                }
            }
        }

        delegate void refreshEditMagazine_Call(int mgNumber, int slotNumber);
        public void refreshEditMagazine(int mgNumber, int slotNumber)
        {
            if (this.InvokeRequired)
            {
                refreshEditMagazine_Call d = new refreshEditMagazine_Call(refreshEditMagazine);
                this.BeginInvoke(d, new object[] { mgNumber, slotNumber });
            }
            else
            {
                try
                {
                    if (mc.UnloaderControl.MG_Status[mgNumber, slotNumber] == (int)MG_STATUS.DONE) MGbtnArray[mgNumber, slotNumber].BackColor = Color.LimeGreen;
                    else if (mc.UnloaderControl.MG_Status[mgNumber, slotNumber] == (int)MG_STATUS.READY) MGbtnArray[mgNumber, slotNumber].BackColor = Color.Yellow;
                    else if (mc.UnloaderControl.MG_Status[mgNumber, slotNumber] == (int)MG_STATUS.SKIP) MGbtnArray[mgNumber, slotNumber].BackColor = Color.Black;
                }
                catch (Exception err)
                {
                }
            }
        }
		#endregion

        private void ClickButton(Object sender, MouseEventArgs e)
        {
            //if (!mc.check.READY_AUTORUN(sender)) return;
            if (mc.main.THREAD_RUNNING) return;
            //mc.check.push(sender, true);

            FormMagazineEdit ff = new FormMagazineEdit();
            ff.ShowDialog();
            
            //mc.check.push(sender, false);
        }

		private void hWindow_Click(object sender, EventArgs e)
		{
			//hWindow.ADVANCE_MODE = !hWindow.ADVANCE_MODE;
		}

		private void BT_hWindowAdvanceMode_Click(object sender, EventArgs e)
		{
			/*
			if (hWindow.ADVANCE_MODE)
			{
				EVENT.hWindowAdvanceMode(false);
				EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL);
				BoardStatus_LoadingZone.Visible = true;
				BoardStatus_WorkingZone.Visible = true;
				BoardStatus_UnloadingZone.Visible = true;
				stackFeederStatus.Visible = true;
				TC_.Visible = true;
			}
			else
			{
				BoardStatus_LoadingZone.Visible = false;
				BoardStatus_WorkingZone.Visible = false;
				BoardStatus_UnloadingZone.Visible = false;
				stackFeederStatus.Visible = false;
				TC_.Visible = false;
				EVENT.hWindowAdvanceMode(true);
				EVENT.mainFormPanelMode(SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND);
			}
			*/
			//EVENT.hWindow2Display();
			if (mc.para.runOption.runPanel == false)
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.EXPAND, SPLITTER_MODE.EXPAND);
				mc.para.runOption.runPanel = true;
			}
			else
			{
				EVENT.mainFormPanelMode(SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL, SPLITTER_MODE.NORMAL);
				mc.para.runOption.runPanel = false;
			}
		}

		private void CenterLeft_Load(object sender, EventArgs e)
		{
            mc.UnloaderControl.readconfig();
            refreshShowMagazine(false, true); 

			boardActivate(BOARD_ZONE.LOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
			boardActivate(BOARD_ZONE.WORKING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
			boardActivate(BOARD_ZONE.UNLOADING, (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

            EVENT.boardStatus(BOARD_ZONE.LOADING, mc.board.padStatus(BOARD_ZONE.LOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
			EVENT.boardStatus(BOARD_ZONE.WORKING, mc.board.padStatus(BOARD_ZONE.WORKING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);
			EVENT.boardStatus(BOARD_ZONE.UNLOADING, mc.board.padStatus(BOARD_ZONE.UNLOADING), (int)mc.para.MT.padCount.x.value, (int)mc.para.MT.padCount.y.value);

			TB_PROD_Work_PreForce.ForeColor = Color.RoyalBlue;
			TB_PROD_Work_PostForce.ForeColor = Color.RoyalBlue;
			
			mc.commMPC.WorkData.receipeName = Path.GetFileNameWithoutExtension(mc.para.ETC.recipeName.description);
			mc.commMPC.WorkData.recipePath = Path.GetDirectoryName(mc.para.ETC.recipeName.description);
		}

		private void Control_Click(object sender, MouseEventArgs e)
		{
			if (sender.Equals(BoardStatus_WorkingZone))
			{
				FormBoardStatusEdit ff = new FormBoardStatusEdit();
				ff.ShowDialog();
			}
		}

		private void Control_Click(object sender, EventArgs e)
		{
			if (sender.Equals(BoardStatus_WorkingZone))
			{
				FormBoardStatusEdit ff = new FormBoardStatusEdit();
				ff.ShowDialog();
			}
		}

		private void CL_Timer_Tick(object sender, EventArgs e)
		{
			CL_Timer.Enabled = false;
			refresh();
			CL_Timer.Enabled = true;
		}

		private void WorkEdit_DblClick(object sender, EventArgs e)
		{
			//if (mc.para.mmiOption.editMode)
			//{
			//    mc.para.mmiOption.editMode = false;
			//    LB_WorkConv.ForeColor = Color.Black;
			//    EVENT.boardEdit(BOARD_ZONE.WORKING, false);
			//}
			//else
			//{
			//    mc.para.mmiOption.editMode = true;
			//    LB_WorkConv.ForeColor = Color.Red;
			//    EVENT.boardEdit(BOARD_ZONE.WORKING, true);
			//}
		}

		private void LB_TubeStatus_DoubleClick(object sender, EventArgs e)
		{
			if (mc.para.mmiOption.slugEditMode)
			{
				mc.para.mmiOption.slugEditMode = false;
				LB_TubeStatus.ForeColor = Color.Black;
				EVENT.boardEdit(BOARD_ZONE.WORKING, false);
			}
			else
			{
				mc.para.mmiOption.slugEditMode = true;
				LB_TubeStatus.ForeColor = Color.Red;
				EVENT.boardEdit(BOARD_ZONE.WORKING, true);
			}
		}

		private void LB_TodayTrayCount_DoubleClick(object sender, EventArgs e)
		{
			mc.para.runInfo.clearTrayCountInfo();
		}

		private void BoardStatus_WorkingZone_ClientSizeChanged(object sender, EventArgs e)
		{

		}


        private void MG_RESET_CLICK(object sender, EventArgs e)
        {
            if (!mc.check.READY_AUTORUN(sender)) return;
            mc.check.push(sender, true);

            for (int i = 0; i < mc.UnloaderControl.MG_COUNT; i++)
            {
                for (int j = 0; j < mc.UnloaderControl.MG_SLOT_COUNT; j++)
                {
                    mc.UnloaderControl.MG_Status[i, j] = (int)MG_STATUS.READY;
                    //mc.Magazinecontrol.MG_MAP_DATA[i, j] = "";
                    MGbtnArray[i, j].BackColor = Color.Yellow;
                }
            }
            //BoatbtnStatusArray = null;

            for (int x = 0; x < mc.para.MT.padCount.x.value; x++)
                for (int y = 0; y < mc.para.MT.padCount.y.value; y++)
                {
                    BoatbtnArrary[x, y].BackColor = Color.White;
                }
            mc.UnloaderControl.writeconfig();
            mc.OUT.MG.MG_RESET(false, out ret.message);

            mc.check.push(sender, false);
        }

        private void BT_MG_UPDATE_Click(object sender, EventArgs e)
        {
            FormMagazineEdit ff = new FormMagazineEdit();
            ff.ShowDialog();
        }

        private void MG_Click(object sender, EventArgs e)
        {
            FormMagazineEdit ff = new FormMagazineEdit();
            ff.ShowDialog();
        }
	}
}
