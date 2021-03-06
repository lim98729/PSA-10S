﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HalconLibrary;
using DefineLibrary;

namespace PSA_SystemLibrary
{
	public class classVision : CONTROL
	{
		public halcon cam;

		public bool isActivate;
		UnitCode unitCode;
		string activateString;
		public void activate(UnitCode unit, int camNumber, string camName, out string s)
		{
			s = "";

			if (unit != UnitCode.HDC && unit != UnitCode.ULC) return;

			if (camNumber == 1) cam = hVision.cam1;
			else if (camNumber == 2) cam = hVision.cam2;
			else if (camNumber == 3) cam = hVision.cam3;
			else if (camNumber == 4) cam = hVision.cam4;
			else return;

			unitCode = unit;
			cam.activate(camNumber, camName, out s); activateString = s;
			isActivate = cam.isActivate;
		}

		public void deactivate(out bool b, out string s)
		{
			if (!isActivate)
			{
				b = true; s = "Deactivate : Already";
				return;
			}
			cam.deactivate(out b, out s);
			isActivate = cam.isActivate;
		}

		public void displayUserMessage(string msg)
		{
			cam.refresh_req = true; cam.refresh_reqMode = REFRESH_REQMODE.USER_MESSAGE_DISPLAY;
			cam.refresh_errorMessage = msg;
		}

		public TRIGGERMODE triggerMode = TRIGGERMODE.LINE1;
		public int reqModelNumber;
        public double reqPassScore = 70;
		int cnt;
		public double fps, grabTime;
        public bool visionEnd = false;

		public void control()
		{
			if (!req) return;

			switch (sqc)
			{
				case 0:
					Esqc = 0;
					sqc++; break;
				case 1:
					if (!isActivate) { errorCheck(ERRORCODE.ACTIVATE, sqc, "", ALARM_CODE.E_SYSTEM_SW_VISION_NOT_READY); break; }
					sqc++; break;
				case 2:
					if (reqMode == REQMODE.HOMING) { sqc = SQC.HOMING; break; }
					if (reqMode == REQMODE.LIVE) { sqc = SQC.LIVE; break; }
					if (reqMode == REQMODE.GRAB) { sqc = SQC.GRAB; break; }
					if (reqMode == REQMODE.FIND_MODEL) { sqc = SQC.FIND_MODEL; break; }
					if (reqMode == REQMODE.FIND_RECTANGLE) { sqc = SQC.FIND_RECTANGLE; break; }
					if (reqMode == REQMODE.FIND_CIRCLE_QUARTER1) { sqc = SQC.FIND_CIRCLE_QUARTER_1; break; }
					if (reqMode == REQMODE.FIND_CIRCLE_QUARTER2) { sqc = SQC.FIND_CIRCLE_QUARTER_2; break; }
					if (reqMode == REQMODE.FIND_CIRCLE_QUARTER3) { sqc = SQC.FIND_CIRCLE_QUARTER_3; break; }
					if (reqMode == REQMODE.FIND_CIRCLE_QUARTER4) { sqc = SQC.FIND_CIRCLE_QUARTER_4; break; }
					if (reqMode == REQMODE.FIND_CIRCLE) { sqc = SQC.FIND_CIRCLE; break; }
					if (reqMode == REQMODE.FIND_CORNER) { sqc = SQC.FIND_CORNER; break; }
					if (reqMode == REQMODE.FIND_EDGE_QUARTER_1) { sqc = SQC.FIND_EDGE_QUARTER_FIRST; break; }
					if (reqMode == REQMODE.FIND_EDGE_QUARTER_2) { sqc = SQC.FIND_EDGE_QUARTER_SECOND; break; }
					if (reqMode == REQMODE.FIND_EDGE_QUARTER_3) { sqc = SQC.FIND_EDGE_QUARTER_THIRD; break; }
					if (reqMode == REQMODE.FIND_EDGE_QUARTER_4) { sqc = SQC.FIND_EDGE_QUARTER_FOURTH; break; }
                    if (reqMode == REQMODE.FIND_PROJECTION_QUARTER_1) { sqc = SQC.FIND_PROJECTION_QUARTER_1; break; }
					if (reqMode == REQMODE.FIND_PROJECTION_QUARTER_2) { sqc = SQC.FIND_PROJECTION_QUARTER_2; break; }
					if (reqMode == REQMODE.FIND_PROJECTION_QUARTER_3) { sqc = SQC.FIND_PROJECTION_QUARTER_3; break; }
					if (reqMode == REQMODE.FIND_PROJECTION_QUARTER_4) { sqc = SQC.FIND_PROJECTION_QUARTER_4; break; }
					if (reqMode == REQMODE.FIND_RECTANGLE_HS) { sqc = SQC.FIND_RECTANGLE_HS; break; }
					if (unitCode == UnitCode.HDC)
					{
						errorCheck(ERRORCODE.HDC, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_HDC_LIST_NONE); break;
					}
					if (unitCode == UnitCode.ULC)
					{
						errorCheck(ERRORCODE.ULC, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_ULC_LIST_NONE); break;
					}
					errorCheck(ERRORCODE.INVALID, sqc, "요청 모드[" + reqMode.ToString() + "]", ALARM_CODE.E_SYSTEM_SW_VISION_LIST_NONE); break;

				#region HOMING
				case SQC.HOMING:
					sqc++; break;
				case SQC.HOMING + 1:
					EVENT.hWindow2Display();
					cam.window.setPart(cam.acq.width, cam.acq.height);
					cam.window.setPartRun();
					dwell.Reset();
					sqc++; break;
				case SQC.HOMING + 2:
                    if (dev.NotExistHW.CAMERA) { sqc = SQC.HOMING + 3; break; }
					if (dwell.Elapsed < 100) break;
					cam.clearWindow();
					cam.messageOff = false;
					cam.messageStatus(activateString);
					sqc++; break;
				case SQC.HOMING + 3:
                    mc.hd.tool.triggerHDC.output(true, out ret.message);
                    mc.hd.tool.triggerULC.output(true, out ret.message);
					if (unitCode == UnitCode.HDC) mc.init.success.HDC = true;
					if (unitCode == UnitCode.ULC) mc.init.success.ULC = true;
					sqc = SQC.STOP; break;

				case SQC.HOMING_ERROR:
					if (unitCode == UnitCode.HDC) mc.init.success.HDC = false;
					if (unitCode == UnitCode.ULC) mc.init.success.ULC = false;
					sqc = SQC.ERROR; break;
				#endregion

				#region LIVE
				case SQC.LIVE:
					_triggerSource = cam.acq.TriggerSource;
					cam.acq.TriggerSource = "Software";
					cam.acq.paraApply();
					sqc++; break;
				case SQC.LIVE + 1:
					cnt = 0;
					dwell.Reset();
					sqc++; break;
				case SQC.LIVE + 2:
					if (liveStop) { sqc++; break; }
					if (cam.refresh_req) break;
					cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					#region refresh
					if (liveMode == REFRESH_REQMODE.CIRCLE_CENTER)
					{
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
					}
					else if (liveMode == REFRESH_REQMODE.RECTANGLE_CENTER)
					{
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
					}
					else if (liveMode == REFRESH_REQMODE.CENTER_CROSS)
					{
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.CENTER_CROSS;
					}
					else if (liveMode == REFRESH_REQMODE.CALIBRATION)
					{
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.CALIBRATION;
					}
                    else if (liveMode == REFRESH_REQMODE.PROJECTION_EDGE)
                    {
                        cam.refresh_req = true;
                        cam.refresh_reqMode = REFRESH_REQMODE.PROJECTION_EDGE;
                    }
                    else
                    {
                        cam.refresh_req = true;
                        cam.refresh_reqMode = REFRESH_REQMODE.IMAGE;
                    }
					#endregion
					cnt++; if (cnt < 10) break;
					fps = Math.Round(10000 / dwell.Elapsed, 2);
					cnt = 0; dwell.Reset();
					break;
				case SQC.LIVE + 3:
					cam.acq.TriggerSource = _triggerSource;
					cam.acq.paraApply();
					sqc = SQC.STOP; break;

				case SQC.LIVE_ERROR:
					sqc = SQC.ERROR; break;
				#endregion

				#region GRAB
				case SQC.GRAB:
					if (cam.refresh_req) break;
                    dwell.Reset();
                    if(mc.hd.reqMode != REQMODE.DUMY) visionEnd = false;
                    if (triggerMode == TRIGGERMODE.SOFTWARE)
                    {
                        cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
                    else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
                    visionEnd = true;
                    mc.log.debug.write(mc.log.CODE.INFO, "Grab Time : " + cam.grabTime);
					cam.writeLogGrabImage("GRAB");
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.CENTER_CROSS;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_MODEL
				case SQC.FIND_MODEL:
					if (cam.refresh_req) break;
                    visionEnd = false;
					#region grab
					if (triggerMode == TRIGGERMODE.SOFTWARE)
					{
						cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_MODEL");
					#endregion
                    visionEnd = true;
					cam.findModel(reqModelNumber, out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.FIND_ERROR; break; }

                    if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value > 0)
                    {
                        if (reqPassScore >= cam.model[reqModelNumber].resultScore * 100)
                        {
                            if (reqModelNumber >= (int)HDC_MODEL.PAD_NCC && reqModelNumber <= (int)HDC_MODEL.PADC4_SHAPE)
                                cam.writeLogGrabImage("HDC_C" + ((reqModelNumber - (int)HDC_MODEL.PADC1_NCC) / 2 + 1).ToString() + "_NG");
                            else if (reqModelNumber == (int)HDC_MODEL.TRAY_REVERSE_SHAPE1)
                                cam.writeLogGrabImage("HDC_TrayReverse1_NG");
                        }
                        else
                        {
                            if ((int)mc.para.HDC.imageSave.value == 2)
                            {
                                if (reqModelNumber >= (int)HDC_MODEL.PAD_NCC && reqModelNumber <= (int)HDC_MODEL.PADC4_SHAPE)
                                    cam.writeLogGrabImage("HDC_C" + ((reqModelNumber - (int)HDC_MODEL.PADC1_NCC) / 2 + 1).ToString() + "_OK");
                                else if (reqModelNumber == (int)HDC_MODEL.TRAY_REVERSE_SHAPE1)
                                    cam.writeLogGrabImage("HDC_TrayReverse1_OK");
                            }
                        }
                    }
                    //if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value > 0)
                    //{
                    //    cam.writeLogGrapImage("ULC_C" + ((reqModelNumber - (int)HDC_MODEL.) / 2 + 1).ToString() + "_OK");
                    //}
                    
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.FIND_MODEL;
					cam.refresh_reqModelNumber = reqModelNumber;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_RECTANGLE
				case SQC.FIND_RECTANGLE:
					if (cam.refresh_req) break;
					#region grab
					if (triggerMode == TRIGGERMODE.SOFTWARE)
					{
						cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_RECTANGLE");
					#endregion
					cam.findRectangleCenter(out ret.message, out ret.s);
					if (ret.message != RetMessage.OK)
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("HDC_Process_Fail");
						}
						sqc = SQC.FIND_ERROR;
						break;
					}
					else
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 2)
						{
							//mc.log.debug.write(mc.log.CODE.TRACE, "ULC All Image Save");
							cam.writeLogGrabImage("HDC_Process_OK");
						}
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_RECTANGLE_HS
				case SQC.FIND_RECTANGLE_HS:
					if (cam.refresh_req) break;
					#region grab
					if (triggerMode == TRIGGERMODE.SOFTWARE)
					{
						cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_RECTANGLE");
					#endregion
					cam.findRectangleCenter(out ret.message, out ret.s);
					if (ret.message != RetMessage.OK)
					{
						if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("ULC_Process_Fail");
						}
						sqc = SQC.FIND_ERROR;
						break;
					}
					else
					{
						if (mc.para.ULC.orientationUse.value == 1)
						{
							if (mc.para.ULC.modelHSOrientation.algorism.value == 0)
								cam.findModel((int)ULC_MODEL.PKG_ORIENTATION_NCC, out ret.message, out ret.s);
							if (mc.para.ULC.modelHSOrientation.algorism.value == 1)
								cam.findModel((int)ULC_MODEL.PKG_ORIENTATION_SHAPE, out ret.message, out ret.s);
							if (ret.message != RetMessage.OK)
							{
								if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
								{
									//mc.log.debug.write(mc.log.CODE.TRACE, "ULC Fail Image Save");
									cam.writeLogGrabImage("ULC_Process_Fail");
								}
								sqc = SQC.FIND_ERROR;
								break;
							}
							else
							{
								if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 2)
								{
									//mc.log.debug.write(mc.log.CODE.TRACE, "ULC All Image Save");
									cam.writeLogGrabImage("ULC_Process_OK");
								}
							}
						}
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.RECTANGLE_CENTER;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_CIRCLE
				case SQC.FIND_CIRCLE_QUARTER_1:
					cam.createCircleCenter(0, true);
					sqc = SQC.FIND_CIRCLE; break;
				case SQC.FIND_CIRCLE_QUARTER_2:
					cam.createCircleCenter(1, true);
					sqc = SQC.FIND_CIRCLE; break;
				case SQC.FIND_CIRCLE_QUARTER_3:
					cam.createCircleCenter(2, true);
					sqc = SQC.FIND_CIRCLE; break;
				case SQC.FIND_CIRCLE_QUARTER_4:
					cam.createCircleCenter(3, true);
					sqc = SQC.FIND_CIRCLE; break;
				case SQC.FIND_CIRCLE:
					if (cam.refresh_req) break;
					#region grab
					if (triggerMode == TRIGGERMODE.SOFTWARE)
					{
						cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_CIRCLE");
					#endregion
					cam.findCircleCenter(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.FIND_ERROR; break; }
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.CIRCLE_CENTER;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_CORNER
				case SQC.FIND_CORNER:
					if (cam.refresh_req) break;
					#region grab
					if (triggerMode == TRIGGERMODE.SOFTWARE)
					{
						cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_CORNER");
					#endregion
					cam.findCornerEdge(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.FIND_ERROR; break; }
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.CORNER_EDGE;
					#endregion
					sqc = SQC.STOP; break;
				#endregion

				#region FIND_EDGE_QUARTER
				case SQC.FIND_EDGE_QUARTER_THIRD:
					if (cam.refresh_req) break;
                    visionEnd = false;
					#region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    {
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
					//cam.writeGrabImage("FIND_EDGE_QUARTER_1");
					#endregion
					cam.edgeIntersection.create(cam.acq.Image, cam.window, cam.acq.ResolutionX, cam.acq.ResolutionY, QUARTER_NUMBER.THIRD, out ret.b); if(!ret.b){ sqc = SQC.FIND_ERROR; break; }
					cam.edgeIntersection.find(out ret.b);
                    visionEnd = true;
					if (!ret.b)
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("HDC_C3_Process_Fail");
						}
                        else if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("ULC_C3_Process_Fail");
                        }
						sqc = SQC.FIND_ERROR; break;
					}
					else
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 2)
						{
							cam.writeLogGrabImage("HDC_C3_OK");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("ULC_C3_OK");
                        }
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
					#endregion
					sqc = SQC.STOP; break;
				case SQC.FIND_EDGE_QUARTER_SECOND:
					if (cam.refresh_req) break;
                    visionEnd = false;
					#region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_EDGE_QUARTER_2");
					#endregion
					cam.edgeIntersection.create(cam.acq.Image, cam.window, cam.acq.ResolutionX, cam.acq.ResolutionY, QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) { sqc = SQC.FIND_ERROR; break; }
					cam.edgeIntersection.find(out ret.b);
                    visionEnd = true;
					if (!ret.b)
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("HDC_C2_Process_Fail");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("ULC_C2_Process_Fail");
                        }
						sqc = SQC.FIND_ERROR; break;
					}
					else
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 2)
						{
							cam.writeLogGrabImage("HDC_C2_OK");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("ULC_C2_OK");
                        }
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
					#endregion
					sqc = SQC.STOP; break;
				case SQC.FIND_EDGE_QUARTER_FIRST:
					if (cam.refresh_req) break;
                    visionEnd = false;
					#region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    //{
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
					//cam.writeGrabImage("FIND_EDGE_QUARTER_3");
					#endregion
					cam.edgeIntersection.create(cam.acq.Image, cam.window, cam.acq.ResolutionX, cam.acq.ResolutionY, QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) { sqc = SQC.FIND_ERROR; break; }
					cam.edgeIntersection.find(out ret.b);
                    visionEnd = true;
					if (!ret.b)
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("HDC_C1_Process_Fail");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("ULC_C1_Process_Fail");
                        }
						sqc = SQC.FIND_ERROR; break;
					}
					else
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 2)
						{
							cam.writeLogGrabImage("HDC_C1_OK");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("ULC_C1_OK");
                        }
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
					#endregion
					sqc = SQC.STOP; break;
				case SQC.FIND_EDGE_QUARTER_FOURTH:
					if (cam.refresh_req) break;
                    visionEnd = false;
					#region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
					{
						cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
					}
					//cam.writeGrabImage("FIND_EDGE_QUARTER_4");
					#endregion
					cam.edgeIntersection.create(cam.acq.Image, cam.window, cam.acq.ResolutionX, cam.acq.ResolutionY, QUARTER_NUMBER.FOURTH, out ret.b); if (!ret.b) { sqc = SQC.FIND_ERROR; break; }
					cam.edgeIntersection.find(out ret.b);
                    visionEnd = true;
					if (!ret.b)
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 1)
						{
							cam.writeLogGrabImage("HDC_C4_Process_Fail");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("ULC_C4_Process_Fail");
                        }
						sqc = SQC.FIND_ERROR; break;
					}
					else
					{
						if (unitCode == UnitCode.HDC && (int)mc.para.HDC.imageSave.value == 2)
						{
							cam.writeLogGrabImage("HDC_C4_OK");
						}
                        if (unitCode == UnitCode.ULC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("ULC_C4_OK");
                        }
					}
					#region refresh
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
					#endregion
					sqc = SQC.STOP; break;
				#endregion


                #region FIND_PROJECTION_QUARTER
                case SQC.FIND_PROJECTION_QUARTER_1:
                    if (cam.refresh_req) break;
                    visionEnd = false;
                    #region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    {
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
                    //cam.writeGrabImage("FIND_EDGE_QUARTER_1");
                    #endregion

                    reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_1;
                    cam.refresh_reqModelNumber = reqModelNumber;

                    mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                    mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.quardrant.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Type.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Direction.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_EdgeFilter.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MinTh.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MaxTh.value));
                    
                    visionEnd = true;
                    if (!ret.b)
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("HDC_RT_Process_Fail");
                        }
                        sqc = SQC.FIND_ERROR; break;
                    }
                    else
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("HDC_RT_OK");
                        }
                    }
                    #region refresh
                    cam.refresh_req = true;
                    cam.refresh_reqMode = REFRESH_REQMODE.PROJECTION_EDGE;
                    #endregion
                    sqc = SQC.STOP; break;
                case SQC.FIND_PROJECTION_QUARTER_2:
                    if (cam.refresh_req) break;
                    visionEnd = false;
                    #region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    {
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
                    //cam.writeGrabImage("FIND_EDGE_QUARTER_1");
                    #endregion

                    reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_2;
                    cam.refresh_reqModelNumber = reqModelNumber;

                    mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                    mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.quardrant.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Type.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Direction.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_EdgeFilter.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MinTh.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MaxTh.value));
                        
                   visionEnd = true;
                    if (!ret.b)
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("HDC_RB_Process_Fail");
                        }
                        sqc = SQC.FIND_ERROR; break;
                    }
                    else
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("HDC_RB_OK");
                        }
                    }
                    #region refresh
                    cam.refresh_req = true;
                    cam.refresh_reqMode = REFRESH_REQMODE.PROJECTION_EDGE;
                    #endregion
                    sqc = SQC.STOP; break;
                case SQC.FIND_PROJECTION_QUARTER_3:
                    if (cam.refresh_req) break;
                    visionEnd = false;
                    #region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    {
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
                    //cam.writeGrabImage("FIND_EDGE_QUARTER_1");
                    #endregion

                    reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_3;
                    cam.refresh_reqModelNumber = reqModelNumber;

                    mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                    mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.quardrant.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Type.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Direction.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_EdgeFilter.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MinTh.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MaxTh.value));

                    visionEnd = true;
                    if (!ret.b)
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("HDC_LB_Process_Fail");
                        }
                        sqc = SQC.FIND_ERROR; break;
                    }
                    else
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("HDC_LB_OK");
                        }
                    }
                    #region refresh
                    cam.refresh_req = true;
                    cam.refresh_reqMode = REFRESH_REQMODE.PROJECTION_EDGE;
                    #endregion
                    sqc = SQC.STOP; break;
                case SQC.FIND_PROJECTION_QUARTER_4:
                    if (cam.refresh_req) break;
                    visionEnd = false;
                    #region grab
                    //if (triggerMode == TRIGGERMODE.SOFTWARE)
                    //{
                    //    cam.grabSofrwareTrigger(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    //}
                    //else
                    {
                        cam.grab(out ret.message, out ret.s); if (ret.message != RetMessage.OK) { sqc = SQC.GRAB_ERROR; break; }
                    }
                    //cam.writeGrabImage("FIND_EDGE_QUARTER_1");
                    #endregion

                    reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_4;
                    cam.refresh_reqModelNumber = reqModelNumber;

                    mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                    mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.quardrant.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Type.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Direction.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_EdgeFilter.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MinTh.value)
                        , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MaxTh.value));

                    visionEnd = true;
                    if (!ret.b)
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 1)
                        {
                            cam.writeLogGrabImage("HDC_LT_Process_Fail");
                        }
                        sqc = SQC.FIND_ERROR; break;
                    }
                    else
                    {
                        if (unitCode == UnitCode.HDC && (int)mc.para.ULC.imageSave.value == 2)
                        {
                            cam.writeLogGrabImage("HDC_LT_OK");
                        }
                    }
                    #region refresh
                    cam.refresh_req = true;
                    cam.refresh_reqMode = REFRESH_REQMODE.PROJECTION_EDGE;
                    #endregion
                    sqc = SQC.STOP; break;
                #endregion

				case SQC.GRAB_ERROR:
					cam.refresh_errorMessage = ret.s;
					errorCheck(ERRORCODE.HDC, sqc, cam.refresh_errorMessage);
					cam.refresh_req = true;
					cam.refresh_reqMode = REFRESH_REQMODE.ERROR_DISPLAY;
					sqc = SQC.ERROR; break;
				case SQC.FIND_ERROR:
					if (mc.full.req == true && mc.full.reqMode == REQMODE.AUTO && unitCode==UnitCode.ULC && mc.para.ULC.failretry.value > 0)
					{
						cam.refresh_errorMessage = ret.s;
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
						sqc = SQC.STOP;
						break;
					}
					else if (unitCode == UnitCode.HDC && mc.hd.reqMode == REQMODE.PRESS)
					{
						cam.refresh_errorMessage = ret.s;
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
						sqc = SQC.STOP;
						break;
					}
					else if (mc.full.req == true && mc.full.reqMode == REQMODE.AUTO && unitCode == UnitCode.HDC)
					{
						cam.refresh_errorMessage = ret.s;
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
						sqc = SQC.STOP;
						break;
					}
					else
					{
						cam.refresh_errorMessage = ret.s;
						errorCheck(ERRORCODE.INVALID, sqc, cam.refresh_errorMessage);
						cam.refresh_req = true;
						cam.refresh_reqMode = REFRESH_REQMODE.IMAGE_ERROR_DISPLAY;
						sqc = SQC.ERROR; break;
					}
				case SQC.ERROR:
					//string str = "Vision[";
					//str += cam.acq.grabber.cameraNumber.ToString() + "]";
					//str += " Esqc " + Esqc.ToString();
					mc.log.debug.write(mc.log.CODE.ERROR, String.Format("Vision[{0}] Esc {1}", cam.acq.grabber.cameraNumber, Esqc));
					//EVENT.statusDisplay(str);
					sqc = SQC.STOP; break;

				case SQC.STOP:
					reqMode = REQMODE.AUTO;
					triggerMode = TRIGGERMODE.LINE1;
					req = false;
					sqc = SQC.END; break;
			}


		}

		#region live
		bool isLive;
		string _triggerSource;
		bool liveStop;
		public REFRESH_REQMODE liveMode;
		public bool LIVE
		{
			get
			{
				return isLive;
			}
			set
			{
				if (value)
				{
					liveStop = false;
					Thread th = new Thread(liveThread);
					th.Priority = ThreadPriority.BelowNormal;
					th.Name = "Cam" + cam.acq.grabber.cameraNumber.ToString() + "LiveThread";
					th.Start();
					mc.log.processdebug.write(mc.log.CODE.INFO, "Cam" + cam.acq.grabber.cameraNumber.ToString() + "LiveThread");
				}
				else
				{
					liveStop = true;
					while (true)
					{
						mc.idle(10);
						if (!isLive) break;
					}
				}
			}
		}

		public void SetLive(bool onOff)
		{
			liveStop = !onOff;
		}

		void liveThread()
		{
			isLive = true;
			req = true; reqMode = REQMODE.LIVE;
			while (true)
			{
				mc.idle(1);
				control();
				if (!RUNING) break;
			}
			isLive = false;
		}
		#endregion
		halcon_region tmpRegion = new halcon_region();
		#region circle find
		public void circleFind()
		{
			//double row1 = 10;
			//double column1 = 10;
			//double row2 = cam.acq.height - 10;
			//double column2 = cam.acq.width - 10;
			//
			tmpRegion.row1 = 300;
			tmpRegion.column1 = 300;
			tmpRegion.row2 = cam.acq.height - 300;
			tmpRegion.column2 = cam.acq.width - 300;
			cam.createCircleCenter(tmpRegion);

			cam.grabSofrwareTrigger();

			cam.findCircleCenter();
			//row = cam.circleCenter.resultRow;
			//column = cam.circleCenter.resultColumn;
			//radius = cam.circleCenter.resultRadius;
			//if ((double)cam.circleCenter.resultRadius != -1) cam.refreshCircleCenter(true);
			//else cam.refreshCircleCenter(false);

			while (true)
			{
				mc.idle(1);
				if (!cam.refresh_req) break;
			}
		   
		}

        public void circleFind(int offsetX, int offsetY)
        {
            tmpRegion.row1 = offsetX;
            tmpRegion.column1 = offsetY;
            tmpRegion.row2 = cam.acq.height - offsetX;
            tmpRegion.column2 = cam.acq.width - offsetY;
            cam.createCircleCenter(tmpRegion);

            cam.grabSofrwareTrigger();

            cam.findCircleCenter();

            while (true)
            {
                mc.idle(1);
                if (!cam.refresh_req) break;
            }

        }

		#endregion

		#region circle find region
		public void circleFind(int quarterRegion)
		{
			//double row1 = 10;
			//double column1 = 10;
			//double row2 = cam.acq.height - 10;
			//double column2 = cam.acq.width - 10;
			//halcon_region tmpRegion = new halcon_region();
			
			if (quarterRegion == 2)
			{
				tmpRegion.row1 = 5;
				tmpRegion.column1 = cam.acq.width / 2 + 5;
				tmpRegion.row2 = cam.acq.height / 2 - 5;
				tmpRegion.column2 = cam.acq.width - 5;
			}
			else if (quarterRegion == 3)
			{
				tmpRegion.row1 = cam.acq.height / 2 + 5;
				tmpRegion.column1 = cam.acq.width / 2 + 5;
				tmpRegion.row2 = cam.acq.height - 5;
				tmpRegion.column2 = cam.acq.width - 5;
			}
			else if (quarterRegion == 0)
			{
				tmpRegion.row1 = cam.acq.height / 2 + 5;
				tmpRegion.column1 = 5;
				tmpRegion.row2 = cam.acq.height - 5;
				tmpRegion.column2 = cam.acq.width / 2 - 5;
			}
			else if (quarterRegion == 1)
			{
				tmpRegion.row1 = 5;
				tmpRegion.column1 = 5;
				tmpRegion.row2 = cam.acq.height / 2 - 5;
				tmpRegion.column2 = cam.acq.width / 2 - 5;
			}
			else
			{
				tmpRegion.row1 = 10;
				tmpRegion.column1 = 10;
				tmpRegion.row2 = cam.acq.height - 10;
				tmpRegion.column2 = cam.acq.width - 10;
			}
			cam.createCircleCenter(tmpRegion);

			cam.grabSofrwareTrigger();

			cam.findCircleCenter();
			//row = cam.circleCenter.resultRow;
			//column = cam.circleCenter.resultColumn;
			//radius = cam.circleCenter.resultRadius;
			//if ((double)cam.circleCenter.resultRadius != -1) cam.refreshCircleCenter(true);
			//else cam.refreshCircleCenter(false);

			while (true)
			{
				mc.idle(1);
				if (!cam.refresh_req) break;
			}

		}
		#endregion

		#region rectangle find
		public void rectangleFind()
		{
			//double row1 = 10;
			//double column1 = 10;
			//double row2 = cam.acq.height - 10;
			//double column2 = cam.acq.width - 10;
			//halcon_region tmpRegion = new halcon_region();
			tmpRegion.row1 = 10;
			tmpRegion.column1 = 10;
			tmpRegion.row2 = cam.acq.height - 10;
			tmpRegion.column2 = cam.acq.width - 10;
			cam.createRectangleCenter(tmpRegion);

			cam.grabSofrwareTrigger();

			cam.findRectangleCenter();

			while (true)
			{
				mc.idle(1);
				if (!cam.refresh_req) break;
			}

		}
		#endregion

		public void edgeIntersectionFind(QUARTER_NUMBER quarterNumber, out bool b, int mode = 0)
		{
            if(mode == 0)           // hdc
            {
                mc.hdc.cam.grabSofrwareTrigger();
                //mc.hdc.cam.writeGrabImage("LIDCORNER1");
                mc.hdc.cam.edgeIntersection.create(mc.hdc.cam.acq.Image, mc.hdc.cam.window, mc.hdc.cam.acq.ResolutionX, mc.hdc.cam.acq.ResolutionY, quarterNumber, out b); if (!b) return;
                mc.hdc.cam.edgeIntersection.find(out b); if (!b) return;
                mc.hdc.cam.refresh_req = true; mc.hdc.cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
                dwell.Reset();
                while (true)
                {
                    if (dwell.Elapsed > 5000) { mc.hdc.cam.refresh_req = false; b = false; return; }
                    mc.idle(1);
                    if (!mc.hdc.cam.refresh_req) break;
                }
                b = mc.hdc.cam.edgeIntersection.SUCCESS;
            }
            else           // ulc
            {
                mc.ulc.cam.grabSofrwareTrigger();
                //mc.ulc.cam.writeGrabImage("LIDCORNER1");
                mc.ulc.cam.edgeIntersection.create(mc.ulc.cam.acq.Image, mc.ulc.cam.window, mc.ulc.cam.acq.ResolutionX, mc.ulc.cam.acq.ResolutionY, quarterNumber, out b); if (!b) return;
                mc.ulc.cam.edgeIntersection.find(out b); if (!b) return;
                mc.ulc.cam.refresh_req = true; mc.ulc.cam.refresh_reqMode = REFRESH_REQMODE.EDGE_INTERSECTION;
                dwell.Reset();
                while (true)
                {
                    if (dwell.Elapsed > 5000) { mc.ulc.cam.refresh_req = false; b = false; return; }
                    mc.idle(1);
                    if (!mc.ulc.cam.refresh_req) break;
                }
                b = mc.ulc.cam.edgeIntersection.SUCCESS;
            }
		}

		public void cornerEdgeFind(QUARTER_NUMBER quarterNumber)
		{
			//halcon_region tmpRegion = new halcon_region();
			if (quarterNumber == QUARTER_NUMBER.THIRD)
			{
				tmpRegion.row1 = mc.hdc.cam.acq.height * 0.1;
				tmpRegion.row2 = mc.hdc.cam.acq.height * 0.6;
				tmpRegion.column1 = mc.hdc.cam.acq.width * 0.4;
				tmpRegion.column2 = mc.hdc.cam.acq.width * 0.9;
			}
			if (quarterNumber == QUARTER_NUMBER.SECOND)
			{
				tmpRegion.row1 = mc.hdc.cam.acq.height * 0.1;
				tmpRegion.row2 = mc.hdc.cam.acq.height * 0.6;
				tmpRegion.column1 = mc.hdc.cam.acq.width * 0.1;
				tmpRegion.column2 = mc.hdc.cam.acq.width * 0.6;
			}
			if (quarterNumber == QUARTER_NUMBER.FIRST)
			{
				tmpRegion.row1 = mc.hdc.cam.acq.height * 0.4;
				tmpRegion.row2 = mc.hdc.cam.acq.height * 0.9;
				tmpRegion.column1 = mc.hdc.cam.acq.width * 0.1;
				tmpRegion.column2 = mc.hdc.cam.acq.width * 0.6;
			}
			if (quarterNumber == QUARTER_NUMBER.FOURTH)
			{
				tmpRegion.row1 = mc.hdc.cam.acq.height * 0.4;
				tmpRegion.row2 = mc.hdc.cam.acq.height * 0.9;
				tmpRegion.column1 = mc.hdc.cam.acq.width * 0.4;
				tmpRegion.column2 = mc.hdc.cam.acq.width * 0.9;
			}
			mc.hdc.cam.createCornerEdge(tmpRegion);
			mc.hdc.cam.grabSofrwareTrigger();
			mc.hdc.cam.findCornerEdge();
			while (true)
			{
				mc.idle(1);
				if (!mc.hdc.cam.refresh_req) break;
			}
		}

		public void cornerEdgeFind()
		{
			//halcon_region tmpRegion = new halcon_region();
			tmpRegion.row1 = mc.hdc.cam.acq.height * 0.25;
			tmpRegion.row2 = mc.hdc.cam.acq.height * 0.75;
			tmpRegion.column1 = mc.hdc.cam.acq.width * 0.25;
			tmpRegion.column2 = mc.hdc.cam.acq.width * 0.75;
			mc.hdc.cam.createCornerEdge(tmpRegion);
			mc.hdc.cam.grabSofrwareTrigger();
			mc.hdc.cam.findCornerEdge();
			while (true)
			{
				mc.idle(1);
				if (!mc.hdc.cam.refresh_req) break;
			}
		}

		public void lighting_exposure(light_2channel_paramer light, para_member exposureTime)
		{
			if (unitCode == UnitCode.HDC) mc.light.HDC(light, out ret.b);
			if (unitCode == UnitCode.ULC) mc.light.ULC(light, out ret.b);
			cam.acq.exposureTime = exposureTime.value;
		}

		public void model_delete(SELECT_FIND_MODEL selectModel)
		{
			if (selectModel == SELECT_FIND_MODEL.ULC_PKG)
			{
				mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
				mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].delete();
				mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].delete();
				mc.ulc.cam.rectangleCenter.delete(mc.ulc.cam.acq.grabber.cameraNumber);
				mc.ulc.cam.circleCenter.delete(mc.ulc.cam.acq.grabber.cameraNumber);
			}
			if (selectModel == SELECT_FIND_MODEL.HDC_PAD)
			{
				mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.FALSE;
				mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].delete();
				mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].delete();
			}
			if (selectModel == SELECT_FIND_MODEL.HDC_PADC1)
			{
				mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
				mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].delete();
				mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].delete();
			}
			if (selectModel == SELECT_FIND_MODEL.HDC_PADC2)
			{
				mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
				mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].delete();
				mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].delete();
			}
			if (selectModel == SELECT_FIND_MODEL.HDC_PADC3)
			{
				mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
				mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].delete();
				mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].delete();
			}
			if (selectModel == SELECT_FIND_MODEL.HDC_PADC4)
			{
				mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
				mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].delete();
				mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].delete();
			}
		}
	}
}
