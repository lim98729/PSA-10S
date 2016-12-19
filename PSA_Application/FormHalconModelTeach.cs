using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PSA_SystemLibrary;
using DefineLibrary;
using System.Threading;

namespace PSA_Application
{
	public partial class FormHalconModelTeach : Form
	{
		public FormHalconModelTeach()
		{
			InitializeComponent();
		}
		RetValue ret;
		public double posX, posY, posZ, posT;
		double _posX, _posY, _posT;
		double dXY, dT;
		bool bStop;
		bool isRunning;
		object oButton;
        light_2channel_paramer light_para;
        para_member exposure_para;

        int selectedAxis = 0;

		public SELECT_FIND_MODEL mode = SELECT_FIND_MODEL.ULC_PKG;
		public int padIndexX = 1;
		public int padIndexY = 1;
		public double teachCropArea = 2.5;

		private void Control_Click(object sender, EventArgs e)
		{
			if (isRunning) return;
			isRunning = true;
			this.Enabled = false;
			#region BT_AutoTeach
			if (sender.Equals(BT_AutoTeach))
			{
				#region ULC
                #region ULC PKAGE
                if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);
					halcon_region tmpRegion;
					tmpRegion.row1 = mc.ulc.cam.acq.height * 0.1;
					tmpRegion.column1 = mc.ulc.cam.acq.width * 0.1;
					tmpRegion.row2 = mc.ulc.cam.acq.height * 0.9;
					tmpRegion.column2 = mc.ulc.cam.acq.width * 0.9;
					mc.ulc.cam.createRectangleCenter(tmpRegion);
					#region center moving
					int retry = 0;
				RETRY:
					mc.ulc.cam.grabSofrwareTrigger();
					mc.ulc.cam.findRectangleCenter();
					while (true)
					{
						mc.idle(1);
						if (!mc.ulc.cam.refresh_req) break;
					}
					if ((double)mc.ulc.cam.rectangleCenter.resultWidth != -1)
					{
						posX -= Math.Round((double)mc.ulc.cam.rectangleCenter.resultX, 2);
						posY -= Math.Round((double)mc.ulc.cam.rectangleCenter.resultY, 2);
                        posZ = mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT;
						posT += Math.Round((double)mc.ulc.cam.rectangleCenter.resultAngle, 2);
						#region moving
						mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						#endregion
					}
					else goto EXIT;
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.ulc.cam.findRectangleCenter();
					while (true)
					{
						mc.idle(1);
						if (!mc.ulc.cam.refresh_req) break;
					}
					#endregion
					#region auto teach
					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC, tmpRegion);//, "auto", "auto");

						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 2);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 2);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_NCC, tmpRegion);
						mc.idle(500);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 1.1);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 1.1);
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE, tmpRegion);

						tmpRegion.row1 = (mc.ulc.cam.acq.height * 0.5) - (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.row2 = (mc.ulc.cam.acq.height * 0.5) + (mc.ulc.cam.rectangleCenter.findHeight * 2);
						tmpRegion.column1 = (mc.ulc.cam.acq.width * 0.5) - (mc.ulc.cam.rectangleCenter.findWidth * 2);
						tmpRegion.column2 = (mc.ulc.cam.acq.width * 0.5) + (mc.ulc.cam.rectangleCenter.findWidth * 2);

						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					#endregion
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region ULC_C1
                if (mode == SELECT_FIND_MODEL.ULC_CORNER1)
                {
                    mc.ulc.cam.edgeIntersection.cropArea = teachCropArea;
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner1.isCreate.value = (int)BOOL.FALSE;
                    int retry = 0;
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    if ((double)mc.ulc.cam.edgeIntersection.resultX > Math.Abs(15.0) || (double)mc.ulc.cam.edgeIntersection.resultY > Math.Abs(15.0))
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    double tmpT = (double)mc.ulc.cam.edgeIntersection.resultAngleH;
                    posT -= tmpT;

                    double cosTheta = 0, sinTheta = 0;
                    double alignX, alignY;
                    
                    alignX = (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultX) 
                        - (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultY);
                    alignY = (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultX) 
                        + (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultY);

                    DPOINT ct, pt1, pt2;
                    ct.x = -mc.para.MT.lidSize.x.value * 500 - mc.para.CAL.Tool_Con[0].x.value;
                    ct.y = -mc.para.MT.lidSize.y.value * 500 - mc.para.CAL.Tool_Con[0].y.value;
                    pt1.x = (double)mc.ulc.cam.edgeIntersection.resultX;
                    pt1.y = (double)mc.ulc.cam.edgeIntersection.resultY;

                    Calc.rotate(-tmpT, ct, pt1, out pt2);

                    cosTheta = Math.Cos((-tmpT) * Math.PI / 180);
                    sinTheta = Math.Sin((-tmpT) * Math.PI / 180);

                    alignX = pt2.x;
                    alignY = pt2.y;

                    posX -= alignX;
                    posY -= alignY;
                    mc.hd.tool.jogMoveXYT((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message);
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b, 1); if (!ret.b) goto EXIT;

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
				#endregion
                #region ULC_C2
                if (mode == SELECT_FIND_MODEL.ULC_CORNER2)
                {
                    mc.ulc.cam.edgeIntersection.cropArea = teachCropArea;
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner2.isCreate.value = (int)BOOL.FALSE;
                    int retry = 0;
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    if ((double)mc.ulc.cam.edgeIntersection.resultX > 15.0 || (double)mc.ulc.cam.edgeIntersection.resultY > 15.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    double tmpT = (double)mc.ulc.cam.edgeIntersection.resultAngleH;
                    posT -= tmpT;

                    double cosTheta = 0, sinTheta = 0;
                    double alignX, alignY;

                    alignX = (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        - (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultY);
                    alignY = (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        + (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultY);

                    DPOINT ct, pt1, pt2;
                    ct.x = -mc.para.MT.lidSize.x.value * 500 - mc.para.CAL.Tool_Con[(int)UnitCodeHead.HD1].x.value;
                    ct.y = mc.para.MT.lidSize.y.value * 500 - mc.para.CAL.Tool_Con[(int)UnitCodeHead.HD1].y.value;
                    pt1.x = (double)mc.ulc.cam.edgeIntersection.resultX;
                    pt1.y = (double)mc.ulc.cam.edgeIntersection.resultY;

                    Calc.rotate(-tmpT, ct, pt1, out pt2);

                    cosTheta = Math.Cos((-tmpT) * Math.PI / 180);
                    sinTheta = Math.Sin((-tmpT) * Math.PI / 180);

                    alignX = pt2.x;
                    alignY = pt2.y;

                    posX -= alignX;
                    posY -= alignY;
                    mc.hd.tool.jogMoveXYT((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message);
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b, 1); if (!ret.b) goto EXIT;

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region ULC_C3
                if (mode == SELECT_FIND_MODEL.ULC_CORNER3)
                {
                    mc.ulc.cam.edgeIntersection.cropArea = teachCropArea;
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner3.isCreate.value = (int)BOOL.FALSE;
                    int retry = 0;
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    if ((double)mc.ulc.cam.edgeIntersection.resultX > 15.0 || (double)mc.ulc.cam.edgeIntersection.resultY > 15.0)
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    double tmpT = (double)mc.ulc.cam.edgeIntersection.resultAngleH;
                    posT -= tmpT;

                    double cosTheta = 0, sinTheta = 0;
                    double alignX, alignY;

                    alignX = (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        - (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultY);
                    alignY = (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        + (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultY);

                    DPOINT ct, pt1, pt2;
                    ct.x = mc.para.MT.lidSize.x.value * 500 - mc.para.CAL.Tool_Con[(int)UnitCodeHead.HD1].x.value;
                    ct.y = mc.para.MT.lidSize.y.value * 500 - mc.para.CAL.Tool_Con[(int)UnitCodeHead.HD1].y.value;
                    pt1.x = (double)mc.ulc.cam.edgeIntersection.resultX;
                    pt1.y = (double)mc.ulc.cam.edgeIntersection.resultY;

                    Calc.rotate(-tmpT, ct, pt1, out pt2);

                    cosTheta = Math.Cos((-tmpT) * Math.PI / 180);
                    sinTheta = Math.Sin((-tmpT) * Math.PI / 180);

                    alignX = pt2.x;
                    alignY = pt2.y;

                    posX -= alignX;
                    posY -= alignY;
                    mc.hd.tool.jogMoveXYT((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message);
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b, 1); if (!ret.b) goto EXIT;

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region ULC_C4
                if (mode == SELECT_FIND_MODEL.ULC_CORNER4)
                {
                    mc.ulc.cam.edgeIntersection.cropArea = teachCropArea;
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner4.isCreate.value = (int)BOOL.FALSE;
                    int retry = 0;
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    if ((double)mc.ulc.cam.edgeIntersection.resultX > Math.Abs(15.0) || (double)mc.ulc.cam.edgeIntersection.resultY > Math.Abs(15.0))
                    {
                        string showstr;
                        showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                        showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                        showstr += "\nResult is OK?";
                        DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                        if (digrst == DialogResult.No) goto EXIT;
                    }
                    double tmpT = (double)mc.ulc.cam.edgeIntersection.resultAngleH;
                    posT -= tmpT;

                    double cosTheta = 0, sinTheta = 0;
                    double alignX, alignY;

                    alignX = (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        - (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultY);
                    alignY = (sinTheta * (double)mc.ulc.cam.edgeIntersection.resultX)
                        + (cosTheta * (double)mc.ulc.cam.edgeIntersection.resultY);

                    DPOINT ct, pt1, pt2;
                    ct.x = mc.para.MT.lidSize.x.value * 500 - mc.para.CAL.Tool_Con[0].x.value;
                    ct.y = -mc.para.MT.lidSize.y.value * 500 - mc.para.CAL.Tool_Con[0].y.value;
                    pt1.x = (double)mc.ulc.cam.edgeIntersection.resultX;
                    pt1.y = (double)mc.ulc.cam.edgeIntersection.resultY;

                    Calc.rotate(-tmpT, ct, pt1, out pt2);

                    cosTheta = Math.Cos((-tmpT) * Math.PI / 180);
                    sinTheta = Math.Sin((-tmpT) * Math.PI / 180);

                    alignX = pt2.x;
                    alignY = pt2.y;

                    posX -= alignX;
                    posY -= alignY;
                    mc.hd.tool.jogMoveXYT((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message);
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b, 1); if (!ret.b) goto EXIT;

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #endregion
                #region HDC_PADC1
                if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
                    mc.hdc.cam.edgeIntersection.cropArea = teachCropArea;
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;
					#region moving
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}

					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC2
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
                    mc.hdc.cam.edgeIntersection.cropArea = teachCropArea;
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC3
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
                    mc.hdc.cam.edgeIntersection.cropArea = teachCropArea;
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC4
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
                    mc.hdc.cam.edgeIntersection.cropArea = teachCropArea;
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach
				   
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);
					int retry = 0;
				RETRY:
					mc.hdc.circleFind(); if (!ret.b) goto EXIT;
					if ((double)mc.hdc.cam.edgeIntersection.resultX > 2.0 || (double)mc.hdc.cam.edgeIntersection.resultY > 2.0)
					{
						string showstr;
						showstr = "Result X:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2).ToString();
						showstr += "\nResult Y:" + Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2).ToString();
						showstr += "\nResult is OK?";
						DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
						if (digrst == DialogResult.No) goto EXIT;
					}
					#region moving
					posX += Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
					posY += Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);
					mc.hd.tool.jogMoveXY(posX, posY, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					#endregion
					mc.idle(100);
					if (retry++ < 5) goto RETRY;
					mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b); if (!ret.b) goto EXIT;

					halcon_region tmpRegion;
					#region auto teach

					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.15);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.15);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.15);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.15);
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);

						tmpRegion.row1 = mc.hdc.cam.acq.height * (0.5 - 0.3);
						tmpRegion.row2 = mc.hdc.cam.acq.height * (0.5 + 0.3);
						tmpRegion.column1 = mc.hdc.cam.acq.width * (0.5 - 0.3);
						tmpRegion.column2 = mc.hdc.cam.acq.width * (0.5 + 0.3);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE, tmpRegion);
						mc.idle(500);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					//if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					//{
					//    mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
					//}
					#endregion
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
			}
			#endregion
			#region BT_Teach
			if (sender.Equals(BT_Teach))
			{
				#region ULC
				if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);

					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						//mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC, "auto", "auto");
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_NCC);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_NCC);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						//mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE, "auto", "auto");
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_SHAPE);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_SHAPE);
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].isCreate == "false") mc.para.ULC.model.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
					{
						mc.ulc.cam.createRectangleCenter();
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
					}
					if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						mc.ulc.cam.createCircleCenter();
						mc.para.ULC.model.isCreate.value = (int)BOOL.TRUE;
					}
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
                if (mode == SELECT_FIND_MODEL.ULC_CORNER1)
                {
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner1.isCreate.value = (int)BOOL.TRUE;

                    #region 조명 설정
                    mc.para.setting(ref mc.para.ULC.modelCorner1.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner1.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner1.exposureTime, exposure_para.value);
                    #endregion

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.ULC_CORNER2)
                {
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner2.isCreate.value = (int)BOOL.TRUE;

                    #region 조명 설정
                    mc.para.setting(ref mc.para.ULC.modelCorner2.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner2.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner2.exposureTime, exposure_para.value);
                    #endregion

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.ULC_CORNER3)
                {
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner3.isCreate.value = (int)BOOL.TRUE;

                    #region 조명 설정
                    mc.para.setting(ref mc.para.ULC.modelCorner3.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner3.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner3.exposureTime, exposure_para.value);
                    #endregion

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.ULC_CORNER4)
                {
                    mc.ulc.LIVE = false;
                    mc.para.ULC.modelCorner4.isCreate.value = (int)BOOL.TRUE;

                    #region 조명 설정
                    mc.para.setting(ref mc.para.ULC.modelCorner4.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner4.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.ULC.modelCorner4.exposureTime, exposure_para.value);
                    #endregion

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
				#endregion
				#region HDC_PAD
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

                    #region 조명 설정
                    mc.para.setting(ref mc.para.HDC.modelPAD.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.HDC.modelPAD.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.HDC.modelPAD.exposureTime, exposure_para.value);
                    #endregion

					if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_NCC);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_NCC);
						mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].isCreate == "false") mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.FALSE;
					}
					else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_SHAPE);
						mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].isCreate == "false") mc.para.HDC.modelPAD.isCreate.value = (int)BOOL.FALSE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region HDC_PADC1
				if (mode == SELECT_FIND_MODEL.HDC_PADC1)
				{
					mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    #region 조명 설정
                    mc.para.setting(ref mc.para.HDC.modelPADC1.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC1.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC1.exposureTime, exposure_para.value);
                    #endregion

                    mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.grab();
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_NCC);//, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_NCC);
                        mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;

                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].isCreate == "true")
                        {   
                            #region Find NCC
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC1_NCC);
                            double pPosX, pPosY, cPosX, cPosY = 0;

                            pPosX = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX, 2);
                            pPosY = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY, 2);

                            pPosX = posX + pPosX;
                            pPosY = posY + pPosY;

                            mc.hdc.LIVE = true;

                            mc.hd.tool.jogMove(pPosX, pPosY, out ret.message, true);

                            mc.hdc.LIVE = false;

                            mc.hdc.cam.grab();
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC1_NCC);

                            mc.hd.tool.X.actualPosition(out pPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out pPosY, out ret.message);
                            #endregion

                            //#region ProjectionCorner
                            //mc.hdc.LIVE = true;

                            //posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
                            //posY = mc.hd.tool.cPos.y.PADC1(padIndexX);

                            //mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            //mc.hdc.LIVE = false;
                            //mc.hdc.cam.grab();

                            //mc.para.HDC.modelPADC1.quardrant.value = (int)QUARDRANT.QUARDRANT_RT;
                            //mc.para.HDC.modelPADC1.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                            //mc.para.HDC.modelPADC1.proj_Direction.value = 1;
                            //mc.para.HDC.modelPADC1.proj_EdgeFilter.value = 30;
                            //mc.para.HDC.modelPADC1.proj_MinTh.value = 200;
                            //mc.para.HDC.modelPADC1.proj_MaxTh.value = 255;

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MaxTh.value));

                            //#endregion

                            #region CornerEdge
                            mc.hdc.LIVE = true;

                            posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
                            posY = mc.hd.tool.cPos.y.PADC1(padIndexX);

                            mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            mc.hdc.LIVE = false;
                            mc.hdc.cam.grab();

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b);
                            #endregion

                            cPosX = Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                            cPosY = Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);

                            mc.hdc.LIVE = true;

                            cPosX = posX + cPosX;
                            cPosY = posY + cPosY;

                            mc.hd.tool.jogMove(cPosX, cPosY, out ret.message, true);
                            mc.hdc.LIVE = false;

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b);
                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MaxTh.value));

                            mc.hd.tool.X.actualPosition(out cPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out cPosY, out ret.message);

                            mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;

                            mc.log.debug.write(mc.log.CODE.INFO, "Pattern Pos X : " + Math.Round(pPosX, 2).ToString() + ", Y : " + Math.Round(pPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "Corner Pos X : " + Math.Round(cPosX, 2).ToString() + ", Y : " + Math.Round(cPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "C - P X : " + Math.Round(pPosX - cPosX, 2).ToString() + ", Y : " + Math.Round(pPosY - cPosY, 2).ToString());

                            mc.para.HDC.modelPADC1.patternPos.x.value = Math.Round(pPosX - cPosX, 2);
                            mc.para.HDC.modelPADC1.patternPos.y.value = Math.Round(pPosY - cPosY, 2);

                            mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_1;
                            mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                            mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                        }
                    }
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_SHAPE);
                        mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;

                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;

                        #region 조명 설정
                        mc.para.setting(ref mc.para.HDC.modelPADC1.light.ch1, light_para.ch1.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC1.light.ch2, light_para.ch2.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC1.exposureTime, exposure_para.value);
                        #endregion
                    }
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                    {
                        mc.hdc.cam.grab();
                        mc.para.HDC.modelPADC1.quardrant.value = (int)QUARDRANT.QUARDRANT_RT;
                        mc.para.HDC.modelPADC1.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                        mc.para.HDC.modelPADC1.proj_Direction.value = 1;
                        mc.para.HDC.modelPADC1.proj_EdgeFilter.value = 30;
                        mc.para.HDC.modelPADC1.proj_MinTh.value = 200;
                        mc.para.HDC.modelPADC1.proj_MaxTh.value = 255;

                        mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_1;
                        mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                        mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                        mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.quardrant.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Type.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Direction.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_EdgeFilter.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MinTh.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MaxTh.value));

                        mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                        mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                    }
                    if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC1_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC1_SHAPE);
                        mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].isCreate == "false") mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.FALSE;
                    }

                    mc.hdc.LIVE = true; 
				}
				#endregion
				#region HDC_PADC2
				if (mode == SELECT_FIND_MODEL.HDC_PADC2)
				{
					mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    #region 조명 설정
                    mc.para.setting(ref mc.para.HDC.modelPADC2.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC2.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC2.exposureTime, exposure_para.value);
                    #endregion

                    mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.grab();
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_NCC);//, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_NCC);
                        mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;

                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].isCreate == "true")
                        {
                            #region Find NCC
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC2_NCC);
                            double pPosX, pPosY, cPosX, cPosY = 0;

                            pPosX = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX, 2);
                            pPosY = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY, 2);

                            pPosX = posX + pPosX;
                            pPosY = posY + pPosY;

                            mc.hdc.LIVE = true;

                            mc.hd.tool.jogMove(pPosX, pPosY, out ret.message, true);

                            mc.hdc.LIVE = false;

                            mc.hdc.cam.grab();
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC2_NCC);

                            mc.hd.tool.X.actualPosition(out pPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out pPosY, out ret.message);
                            #endregion

                            //#region ProjectionCorner
                            //mc.hdc.LIVE = true;

                            //posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
                            //posY = mc.hd.tool.cPos.y.PADC2(padIndexX);

                            //mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            //mc.hdc.LIVE = false;
                            //mc.hdc.cam.grab();

                            //mc.para.HDC.modelPADC2.quardrant.value = (int)QUARDRANT.QUARDRANT_RB;
                            //mc.para.HDC.modelPADC2.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                            //mc.para.HDC.modelPADC2.proj_Direction.value = 1;
                            //mc.para.HDC.modelPADC2.proj_EdgeFilter.value = 30;
                            //mc.para.HDC.modelPADC2.proj_MinTh.value = 150;
                            //mc.para.HDC.modelPADC2.proj_MaxTh.value = 255;

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MaxTh.value));

                            //#endregion

                            #region CornerEdge
                            mc.hdc.LIVE = true;

                            posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
                            posY = mc.hd.tool.cPos.y.PADC2(padIndexX);

                            mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            mc.hdc.LIVE = false;
                            mc.hdc.cam.grab();

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b);
                            #endregion

                            cPosX = Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                            cPosY = Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);

                            mc.hdc.LIVE = true;

                            cPosX = posX + cPosX;
                            cPosY = posY + cPosY;

                            mc.hd.tool.jogMove(cPosX, cPosY, out ret.message, true);
                            mc.hdc.LIVE = false;

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.SECOND, out ret.b);

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MaxTh.value));

                            mc.hd.tool.X.actualPosition(out cPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out cPosY, out ret.message);

                            mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;

                            mc.log.debug.write(mc.log.CODE.INFO, "Pattern Pos X : " + Math.Round(pPosX, 2).ToString() + ", Y : " + Math.Round(pPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "Corner Pos X : " + Math.Round(cPosX, 2).ToString() + ", Y : " + Math.Round(cPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "C - P X : " + Math.Round(pPosX - cPosX, 2).ToString() + ", Y : " + Math.Round(pPosY - cPosY, 2).ToString());

                            mc.para.HDC.modelPADC2.patternPos.x.value = Math.Round(pPosX - cPosX, 2);
                            mc.para.HDC.modelPADC2.patternPos.y.value = Math.Round(pPosY - cPosY, 2);

                            mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_2;
                            mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                            mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;
                        }
                    }
                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC2_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC2_SHAPE);
                        mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].isCreate == "false") mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;

                        #region 조명 설정
                        mc.para.setting(ref mc.para.HDC.modelPADC2.light.ch1, light_para.ch1.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC2.light.ch2, light_para.ch2.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC2.exposureTime, exposure_para.value);
                        #endregion
                    }
                    if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                    {
                        mc.hdc.cam.grab();
                        mc.para.HDC.modelPADC2.quardrant.value = (int)QUARDRANT.QUARDRANT_RB;
                        mc.para.HDC.modelPADC2.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                        mc.para.HDC.modelPADC2.proj_Direction.value = 1;
                        mc.para.HDC.modelPADC2.proj_EdgeFilter.value = 30;
                        mc.para.HDC.modelPADC2.proj_MinTh.value = 150;
                        mc.para.HDC.modelPADC2.proj_MaxTh.value = 255;

                        mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_2;
                        mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                        mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                        mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.quardrant.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Type.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_Direction.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_EdgeFilter.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MinTh.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC2.proj_MaxTh.value));

                        mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                        mc.para.HDC.modelPADC2.isCreate.value = (int)BOOL.TRUE;
                    }
					mc.hdc.LIVE = true;
				}
				#endregion
				#region HDC_PADC3
				if (mode == SELECT_FIND_MODEL.HDC_PADC3)
				{
					mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    #region 조명 설정
                    mc.para.setting(ref mc.para.HDC.modelPADC3.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC3.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC3.exposureTime, exposure_para.value);
                    #endregion

                    mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                    {
                        mc.hdc.cam.grab();
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_NCC);//, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_NCC);
                        mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;

                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].isCreate == "true")
                        {
                            #region Find NCC
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC3_NCC);
                            double pPosX, pPosY, cPosX, cPosY = 0;

                            pPosX = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX, 2);
                            pPosY = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY, 2);

                            pPosX = posX + pPosX;
                            pPosY = posY + pPosY;

                            mc.hdc.LIVE = true;

                            mc.hd.tool.jogMove(pPosX, pPosY, out ret.message, true);

                            mc.hdc.LIVE = false;

                            mc.hdc.cam.grab();
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC3_NCC);

                            mc.hd.tool.X.actualPosition(out pPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out pPosY, out ret.message);
                            #endregion

                            //#region ProjectionCorner
                            //mc.hdc.LIVE = true;

                            //posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
                            //posY = mc.hd.tool.cPos.y.PADC3(padIndexX);

                            //mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            //mc.hdc.LIVE = false;
                            //mc.hdc.cam.grab();

                            //mc.para.HDC.modelPADC3.quardrant.value = (int)QUARDRANT.QUARDRANT_LB;
                            //mc.para.HDC.modelPADC3.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                            //mc.para.HDC.modelPADC3.proj_Direction.value = 1;
                            //mc.para.HDC.modelPADC3.proj_EdgeFilter.value = 30;
                            //mc.para.HDC.modelPADC3.proj_MinTh.value = 150;
                            //mc.para.HDC.modelPADC3.proj_MaxTh.value = 255;

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MaxTh.value));

                            //#endregion

                            #region CornerEdge
                            mc.hdc.LIVE = true;

                            posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
                            posY = mc.hd.tool.cPos.y.PADC3(padIndexX);

                            mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            mc.hdc.LIVE = false;
                            mc.hdc.cam.grab();

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b);
                            #endregion

                            cPosX = Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                            cPosY = Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);

                            mc.hdc.LIVE = true;

                            cPosX = posX + cPosX;
                            cPosY = posY + cPosY;

                            mc.hd.tool.jogMove(cPosX, cPosY, out ret.message, true);
                            mc.hdc.LIVE = false;

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b);

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MaxTh.value));

                            mc.hd.tool.X.actualPosition(out cPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out cPosY, out ret.message);

                            mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;

                            mc.log.debug.write(mc.log.CODE.INFO, "Pattern Pos X : " + Math.Round(pPosX, 2).ToString() + ", Y : " + Math.Round(pPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "Corner Pos X : " + Math.Round(cPosX, 2).ToString() + ", Y : " + Math.Round(cPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "C - P X : " + Math.Round(pPosX - cPosX, 2).ToString() + ", Y : " + Math.Round(pPosY - cPosY, 2).ToString());

                            mc.para.HDC.modelPADC3.patternPos.x.value = Math.Round(pPosX - cPosX, 2);
                            mc.para.HDC.modelPADC3.patternPos.y.value = Math.Round(pPosY - cPosY, 2);

                            mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_3;
                            mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                            mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;
                        }
                    }
                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                    {
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC3_SHAPE);
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC3_SHAPE);
                        mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].isCreate == "false") mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.FALSE;
                    }
                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                    {
                        mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;

                        #region 조명 설정
                        mc.para.setting(ref mc.para.HDC.modelPADC3.light.ch1, light_para.ch1.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC3.light.ch2, light_para.ch2.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC3.exposureTime, exposure_para.value);
                        #endregion
                    }
                    if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                    {
                        mc.hdc.cam.grab();
                        mc.para.HDC.modelPADC3.quardrant.value = (int)QUARDRANT.QUARDRANT_LB;
                        mc.para.HDC.modelPADC3.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                        mc.para.HDC.modelPADC3.proj_Direction.value = 1;
                        mc.para.HDC.modelPADC3.proj_EdgeFilter.value = 30;
                        mc.para.HDC.modelPADC3.proj_MinTh.value = 150;
                        mc.para.HDC.modelPADC3.proj_MaxTh.value = 255;

                        mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_3;
                        mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                        mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                        mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.quardrant.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Type.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_Direction.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_EdgeFilter.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MinTh.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC3.proj_MaxTh.value));

                        mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                        mc.para.HDC.modelPADC3.isCreate.value = (int)BOOL.TRUE;
                    }
					mc.hdc.LIVE = true;
				}
				#endregion
				#region HDC_PADC4
				if (mode == SELECT_FIND_MODEL.HDC_PADC4)
				{
					mc.hdc.LIVE = false;
                    mc.hdc.model_delete(mode);

                    #region 조명 설정
                    mc.para.setting(ref mc.para.HDC.modelPADC4.light.ch1, light_para.ch1.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC4.light.ch2, light_para.ch2.value);
                    mc.para.setting(ref mc.para.HDC.modelPADC4.exposureTime, exposure_para.value);
                    #endregion

                    mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
                        mc.hdc.cam.grab();
                        mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
                        mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_NCC);//, "auto", "auto");
                        mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_NCC);
                        mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;

                        if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].isCreate == "true")
                        {
                            #region Find NCC
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC4_NCC);
                            double pPosX, pPosY, cPosX, cPosY = 0;

                            pPosX = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX, 2);
                            pPosY = Math.Round((double)mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY, 2);

                            pPosX = posX + pPosX;
                            pPosY = posY + pPosY;

                            mc.hdc.LIVE = true;

                            mc.hd.tool.jogMove(pPosX, pPosY, out ret.message, true);

                            mc.hdc.LIVE = false;

                            mc.hdc.cam.grab();
                            mc.hdc.cam.findModel((int)HDC_MODEL.PADC4_NCC);

                            mc.hd.tool.X.actualPosition(out pPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out pPosY, out ret.message);
                            #endregion

                            //#region ProjectionCorner
                            //mc.hdc.LIVE = true;

                            //posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
                            //posY = mc.hd.tool.cPos.y.PADC4(padIndexX);

                            //mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            //mc.hdc.LIVE = false;
                            //mc.hdc.cam.grab();

                            //mc.para.HDC.modelPADC4.quardrant.value = (int)QUARDRANT.QUARDRANT_LT;
                            //mc.para.HDC.modelPADC4.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                            //mc.para.HDC.modelPADC4.proj_Direction.value = 1;
                            //mc.para.HDC.modelPADC4.proj_EdgeFilter.value = 30;
                            //mc.para.HDC.modelPADC4.proj_MinTh.value = 150;
                            //mc.para.HDC.modelPADC4.proj_MaxTh.value = 255;

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MaxTh.value));

                            //#endregion

                            #region CornerEdge
                            mc.hdc.LIVE = true;

                            posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
                            posY = mc.hd.tool.cPos.y.PADC4(padIndexX);

                            mc.hd.tool.jogMove(posX, posY, out ret.message, true);
                            mc.hdc.LIVE = false;
                            mc.hdc.cam.grab();

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b);
                            #endregion

                            cPosX = Math.Round((double)mc.hdc.cam.edgeIntersection.resultX, 2);
                            cPosY = Math.Round((double)mc.hdc.cam.edgeIntersection.resultY, 2);

                            mc.hdc.LIVE = true;

                            cPosX = posX + cPosX;
                            cPosY = posY + cPosY;

                            mc.hd.tool.jogMove(cPosX, cPosY, out ret.message, true);
                            mc.hdc.LIVE = false;

                            mc.hdc.edgeIntersectionFind(QUARTER_NUMBER.FOURTH, out ret.b);

                            //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                            //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.quardrant.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Type.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Direction.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_EdgeFilter.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MinTh.value)
                            //    , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MaxTh.value));

                            mc.hd.tool.X.actualPosition(out cPosX, out ret.message);
                            mc.hd.tool.Y.actualPosition(out cPosY, out ret.message);

                            mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;

                            mc.log.debug.write(mc.log.CODE.INFO, "Pattern Pos X : " + Math.Round(pPosX, 2).ToString() + ", Y : " + Math.Round(pPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "Corner Pos X : " + Math.Round(cPosX, 2).ToString() + ", Y : " + Math.Round(cPosY, 2).ToString());
                            mc.log.debug.write(mc.log.CODE.INFO, "C - P X : " + Math.Round(pPosX - cPosX, 2).ToString() + ", Y : " + Math.Round(pPosY - cPosY, 2).ToString());

                            mc.para.HDC.modelPADC4.patternPos.x.value = Math.Round(pPosX - cPosX, 2);
                            mc.para.HDC.modelPADC4.patternPos.y.value = Math.Round(pPosY - cPosY, 2);

                            mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_4;
                            mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                            mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;
                        }
                    }
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PADC4_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PADC4_SHAPE);
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].isCreate == "false") mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
					{
						mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;

                        #region 조명 설정
                        mc.para.setting(ref mc.para.HDC.modelPADC4.light.ch1, light_para.ch1.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC4.light.ch2, light_para.ch2.value);
                        mc.para.setting(ref mc.para.HDC.modelPADC4.exposureTime, exposure_para.value);
                        #endregion
					}
                    if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                    {
                        mc.hdc.cam.grab();
                        mc.para.HDC.modelPADC4.quardrant.value = (int)QUARDRANT.QUARDRANT_LT;
                        mc.para.HDC.modelPADC4.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                        mc.para.HDC.modelPADC4.proj_Direction.value = 1;
                        mc.para.HDC.modelPADC4.proj_EdgeFilter.value = 30;
                        mc.para.HDC.modelPADC4.proj_MinTh.value = 150;
                        mc.para.HDC.modelPADC4.proj_MaxTh.value = 255;

                        mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_4;
                        mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                        mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                        mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.quardrant.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Type.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_Direction.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_EdgeFilter.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MinTh.value)
                            , Convert.ToInt32(mc.para.HDC.modelPADC4.proj_MaxTh.value));

                        mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                        mc.para.HDC.modelPADC4.isCreate.value = (int)BOOL.TRUE;
                    }
                    mc.hdc.LIVE = true;
				}
				#endregion
				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					mc.hdc.model_delete(mode);

					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_FIDUCIAL_NCC, "auto", "auto");
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_FIDUCIAL_NCC);
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].isCreate == "false") mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.hdc.cam.createModel((int)HDC_MODEL.PAD_FICUCIAL_SHAPE);
						mc.hdc.cam.createFind((int)HDC_MODEL.PAD_FICUCIAL_SHAPE);
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
						if (mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].isCreate == "false") mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
					{
						mc.para.HDC.modelFiducial.isCreate.value = (int)BOOL.TRUE;
					}
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
				#region ULC_ORIENTATION
				if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
				{
					mc.ulc.LIVE = false;
					mc.ulc.model_delete(mode);

					if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].algorism = MODEL_ALGORISM.NCC.ToString();
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_ORIENTATION_NCC, "auto", "auto");
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_ORIENTATION_NCC);
						mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].isCreate == "false") mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.FALSE;
					}
					if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
					{
						mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].algorism = MODEL_ALGORISM.SHAPE.ToString();
						mc.ulc.cam.createModel((int)ULC_MODEL.PKG_ORIENTATION_SHAPE);
						mc.ulc.cam.createFind((int)ULC_MODEL.PKG_ORIENTATION_SHAPE);
						mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.TRUE;
						if (mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].isCreate == "false") mc.para.ULC.modelHSOrientation.isCreate.value = (int)BOOL.FALSE;
					}

					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion
			}
			#endregion
			#region BT_SpeedXY
			if (sender.Equals(BT_SpeedXY))
			{
				if (dXY == 1) dXY = 10;
				else if (dXY == 10) dXY = 100;
				else if (dXY == 100) dXY = 1000;
				else dXY = 1;
			}
			#endregion
			#region BT_SpeedT
			if (sender.Equals(BT_SpeedT))
			{
				if (dT == 0.01) dT = 0.1;
				else if (dT == 0.1) dT = 1;
				else if (dT == 1) dT = 10;
				else dT = 0.01;
			}
			#endregion
			#region BT_Test
			if (sender.Equals(BT_Test))
			{
				#region ULC
				if (mode == SELECT_FIND_MODEL.ULC_PKG)
				{
					bool preMessageDisplay;
					preMessageDisplay = false;
					mc.ulc.LIVE = false;
					#region moving ulc
                    mc.hd.tool.jogMove((int)UnitCodeHead.HD1, mc.hd.tool.tPos.x[0].ULC, mc.hd.tool.tPos.y[0].ULC, mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT, mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(100);
					#endregion
					#region ULC.req
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
                    mc.ulc.lighting_exposure(light_para, exposure_para);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region moving ulc 보상위치
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rWidth = 0;
					double rHeight = 0;
					#region ULC result
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							rX = mc.ulc.cam.rectangleCenter.resultX;
							rY = mc.ulc.cam.rectangleCenter.resultY;
							rT = mc.ulc.cam.rectangleCenter.resultAngle;
							rWidth = mc.ulc.cam.rectangleCenter.resultWidth * 2;
							rHeight = mc.ulc.cam.rectangleCenter.resultHeight * 2;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.ulc.cam.circleCenter.resultX;
							rY = mc.ulc.cam.circleCenter.resultY;
							rT = 0;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					if (rWidth > 0 && rHeight > 0)
					{
						TB_Result.AppendText("Result Width : " + Math.Round(rWidth, 3).ToString() + "\n");
						TB_Result.AppendText("Result Height: " + Math.Round(rHeight, 3).ToString() + "\n");
						TB_Result.AppendText("Diff Width   : " + Math.Round(rWidth - mc.para.MT.lidSize.x.value * 1000).ToString() + "\n");
						TB_Result.AppendText("Diff Height  : " + Math.Round(rHeight - mc.para.MT.lidSize.y.value * 1000).ToString() + "\n");

						if (rWidth < (mc.para.MT.lidSize.x.value*1000 - mc.para.MT.lidSizeLimit.value) || rWidth > (mc.para.MT.lidSize.x.value*1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Width Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("WIDTH SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						if (rHeight < (mc.para.MT.lidSize.y.value*1000 - mc.para.MT.lidSizeLimit.value) || rHeight > (mc.para.MT.lidSize.y.value*1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Height Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("HEIGHT SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rX > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** X Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("X POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rY > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** Y Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("Y POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					else
					{
						if (preMessageDisplay == false)
						{
							mc.ulc.displayUserMessage("DETECTION FAIL");
							mc.main.Thread_Polling();
						}
						preMessageDisplay = true;
					}
					if (mc.ulc.cam.rectangleCenter.ChamferIndex != -1)
					{
						TB_Result.AppendText("Chamfer[0]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[(int)UnitCodeHead.HD1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[1]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[2]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[2], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[3]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[3], 2).ToString() + "\n");
						if ((int)mc.para.ULC.chamferuse.value != 0)
						{
							if ((int)mc.para.ULC.chamferindex.value == mc.ulc.cam.rectangleCenter.ChamferIndex)
								TB_Result.AppendText("Chamfer Recog is OK! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
							else
							{
								TB_Result.AppendText("Chamfer Recog is FAIL! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
								if (preMessageDisplay == false)
								{
									mc.ulc.displayUserMessage("CHAMFER RECOG FAIL");
									mc.main.Thread_Polling();
								}
								preMessageDisplay = true;
							}
						}
					}

					double circleResult;
					if(mc.ulc.cam.rectangleCenter.findRadius < 0) circleResult = -1;
					else circleResult = mc.ulc.cam.rectangleCenter.findRadius * 2.0;
					if((int)mc.para.ULC.checkcircleuse.value != 0)
					{
						if (circleResult != -1)
						{
							TB_Result.AppendText("Circle Recog is OK! Diameter: " + Math.Round(circleResult, 3).ToString() + "\n");
						}
						else
						{
							TB_Result.AppendText("Circle Recog is Fail!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("BOTTOM SIDE RECOG FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}

					if (rX != -1 && rY != -1)
					{
						double cosTheta, sinTheta;
						double new_x, new_y;
						cosTheta = Math.Cos(-rT * Math.PI / 180);
						sinTheta = Math.Sin(-rT * Math.PI / 180);
						new_x = (cosTheta * rX) - (sinTheta * rY);
						new_y = (sinTheta * rX) + (cosTheta * rY);

                        posX = mc.hd.tool.tPos.x[0].ULC - new_x;
                        posY = mc.hd.tool.tPos.y[0].ULC - new_y;
                        posZ = mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT;
                        posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO + rT;
						mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
						mc.idle(100);
					}
					#endregion
					preMessageDisplay = false;
					#region ULC.req
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_NCC;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_SHAPE;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_RECTANGLE_HS;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							mc.ulc.reqMode = REQMODE.FIND_CIRCLE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB; 
					}
                    mc.ulc.lighting_exposure(light_para, exposure_para);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true; 
					#endregion
					mc.main.Thread_Polling();
					#region ULC result
					if (mc.para.ULC.model.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_NCC].resultAngle;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_SHAPE].resultAngle;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.RECTANGLE)
						{
							rX = mc.ulc.cam.rectangleCenter.resultX;
							rY = mc.ulc.cam.rectangleCenter.resultY;
							rT = mc.ulc.cam.rectangleCenter.resultAngle;
							rWidth = mc.ulc.cam.rectangleCenter.resultWidth * 2;
							rHeight = mc.ulc.cam.rectangleCenter.resultHeight * 2;
						}
						else if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.ulc.cam.circleCenter.resultX;
							rY = mc.ulc.cam.circleCenter.resultY;
							rT = 0;
						}
					}
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					if (rWidth > 0 && rHeight > 0)
					{
						TB_Result.AppendText("Result Width : " + Math.Round(rWidth, 3).ToString() + "\n");
						TB_Result.AppendText("Result Height: " + Math.Round(rHeight, 3).ToString() + "\n");
						TB_Result.AppendText("Diff Width   : " + Math.Round(rWidth - mc.para.MT.lidSize.x.value * 1000).ToString() + "\n");
						TB_Result.AppendText("Diff Height  : " + Math.Round(rHeight - mc.para.MT.lidSize.y.value * 1000).ToString() + "\n");

						if (rWidth < (mc.para.MT.lidSize.x.value * 1000 - mc.para.MT.lidSizeLimit.value) || rWidth > (mc.para.MT.lidSize.x.value * 1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Width Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("WIDTH SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						if (rHeight < (mc.para.MT.lidSize.y.value * 1000 - mc.para.MT.lidSizeLimit.value) || rHeight > (mc.para.MT.lidSize.y.value * 1000 + mc.para.MT.lidSizeLimit.value))
						{
							TB_Result.AppendText("*** Height Size FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("HEIGHT SIZE FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rX > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** X Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("X POS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
						else if (rY > (mc.para.MT.lidCheckLimit.value))
						{
							TB_Result.AppendText("*** Y Pos Over FAIL!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("YPOS OVER");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					else
					{
						if (preMessageDisplay == false)
						{
							mc.ulc.displayUserMessage("DETECTION FAIL");
							mc.main.Thread_Polling();
						}
						preMessageDisplay = true;
					}
					#endregion
					if (mc.ulc.cam.rectangleCenter.ChamferIndex != -1)
					{
						TB_Result.AppendText("Chamfer[1]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[(int)UnitCodeHead.HD1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[2]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[1], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[3]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[2], 2).ToString() + "\n");
						TB_Result.AppendText("Chamfer[4]   " + Math.Round(mc.ulc.cam.rectangleCenter.ChamferResult[3], 2).ToString() + "\n");
						if ((int)mc.para.ULC.chamferuse.value != 0)
						{
							if ((int)mc.para.ULC.chamferindex.value == mc.ulc.cam.rectangleCenter.ChamferIndex)
								TB_Result.AppendText("Chamfer Recog is OK! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
							else
							{
								TB_Result.AppendText("Chamfer Recog is FAIL! Result: " + (mc.ulc.cam.rectangleCenter.ChamferIndex + 1).ToString() + "\n");
								if (preMessageDisplay == false)
								{
									mc.ulc.displayUserMessage("CHAMFER RECOG FAIL");
									mc.main.Thread_Polling();
								}
								preMessageDisplay = true;
							}
						}
					}
					if (mc.ulc.cam.rectangleCenter.findRadius < 0) circleResult = -1;
					else circleResult = mc.ulc.cam.rectangleCenter.findRadius * 2.0;
					if ((int)mc.para.ULC.checkcircleuse.value != 0)
					{
						if (circleResult != -1)
						{
							TB_Result.AppendText("Circle Recog is OK! Diameter: " + Math.Round(circleResult, 3).ToString() + "\n");
						}
						else
						{
							TB_Result.AppendText("Circle Recog is Fail!\n");
							if (preMessageDisplay == false)
							{
								mc.ulc.displayUserMessage("BOTTOM SIDE RECOG FAIL");
								mc.main.Thread_Polling();
							}
							preMessageDisplay = true;
						}
					}
					mc.idle(1000);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
                #region ULC_C1
                if (mode == SELECT_FIND_MODEL.ULC_CORNER1)
                {
                    mc.ulc.LIVE = false;
                    int retry = 0;
                    double curX = 0;
                    double curY = 0;

                    #region 평균값 출력 초기화
                    double resultMinX = 0;
                    double resultMaxX = 0;

                    double resultMinY = 0;
                    double resultMaxY = 0;
                    
                    double resultMinAV = 0;
                    double resultMaxAV = 0;
                    
                    double resultMinAH = 0;
                    double resultMaxAH = 0;
                    #endregion
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    //if ((double)mc.ulc.cam.edgeIntersection.resultX > 15.0 || (double)mc.ulc.cam.edgeIntersection.resultY > 15.0)
                    //{
                    //    string showstr;
                    //    showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                    //    showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                    //    showstr += "\nResult is OK?";
                    //    DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                    //    if (digrst == DialogResult.No) goto EXIT;
                    //}
                    #endregion
                    double resultX = Math.Round(mc.ulc.cam.edgeIntersection.resultX.D, 2);
                    double resultY = Math.Round(mc.ulc.cam.edgeIntersection.resultY.D, 2);
                    double resultAH = Math.Round(mc.ulc.cam.edgeIntersection.resultAngleH.D, 2);
                    double resultAV = Math.Round(mc.ulc.cam.edgeIntersection.resultAngleV.D, 2);

                    //mc.log.debug.write(mc.log.CODE.INFO, "num : " + retry);
                    #region Min, Max 저장
                    if (retry == 0)
                    {
                        resultMinX = resultX;
                        resultMaxX = resultX;

                        resultMinY = resultY;
                        resultMaxY = resultY;

                        resultMinAV = resultAV;
                        resultMaxAV = resultAV;

                        resultMinAH = resultAH;
                        resultMaxAH = resultAH;
                    }
                    else
                    {
                        if (resultX > resultMaxX) resultMaxX = resultX;
                        if (resultX < resultMinX) resultMinX = resultX;

                        if (resultY > resultMaxY) resultMaxY = resultY;
                        if (resultY < resultMinY) resultMinY = resultY;

                        if (resultAV > resultMaxAV) resultMaxAV = resultAV;
                        if (resultAV < resultMinAV) resultMinAV = resultAV;

                        if (resultAH > resultMaxAH) resultMaxAH = resultAH;
                        if (resultAH < resultMinAH) resultMinAH = resultAH;
                    }
                    #endregion
                    mc.idle(100);
                    mc.log.debug.write(mc.log.CODE.INFO, "X : " + resultX
                        + ", Y : " + resultY + ", AngleH : " + resultAH
                        + ", AngleV : " + resultAV);
                    //#region Move
                    //mc.hd.tool.X.actualPosition(out curX, out ret.message);
                    //mc.hd.tool.Y.actualPosition(out curY, out ret.message);
                    //posX -= 2000;
                    //posY -= 2000;
                    //mc.hd.tool.jogMove(posX, posY, out ret.message, false);

                    //posX += 2000;
                    //posY += 2000;
                    //mc.hd.tool.jogMove(posX, posY, out ret.message, false);
                    //#endregion

                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b, 1); if (!ret.b) goto EXIT;
                    mc.log.debug.write(mc.log.CODE.INFO, "DIff X : " + (resultMaxX - resultMinX)
                        + ", Diff Y : " + (resultMaxY - resultMinY) + ", Diff AngleH : " + (resultMaxAH - resultMinAH)
                        + ", Diff AngleV : " + (resultMaxAV - resultMinAV));
                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion
                #region ULC_C2
                if (mode == SELECT_FIND_MODEL.ULC_CORNER3)
                {
                    mc.ulc.LIVE = false;
                    int retry = 0;
                RETRY:
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.THIRD, out ret.b, 1); if (!ret.b) goto EXIT;
                    #region moving
                    //if ((double)mc.ulc.cam.edgeIntersection.resultX > 15.0 || (double)mc.ulc.cam.edgeIntersection.resultY > 15.0)
                    //{
                    //    string showstr;
                    //    showstr = "Result X:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultX, 2).ToString();
                    //    showstr += "\nResult Y:" + Math.Round((double)mc.ulc.cam.edgeIntersection.resultY, 2).ToString();
                    //    showstr += "\nResult is OK?";
                    //    DialogResult digrst = MessageBox.Show(showstr, "Confirm", MessageBoxButtons.YesNo);
                    //    if (digrst` == DialogResult.No) goto EXIT;
                    //}
                    #endregion
                    mc.idle(100);
                    if (retry++ < 5) goto RETRY;
                    mc.ulc.edgeIntersectionFind(QUARTER_NUMBER.FIRST, out ret.b, 1); if (!ret.b) goto EXIT;

                    mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                #endregion

				#endregion

				#region HDC_PAD
				if (mode == SELECT_FIND_MODEL.HDC_PAD)
				{
					mc.hdc.LIVE = false;
					#region hd pd
					posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
					mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_NCC;
						}
						else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_SHAPE;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
                    mc.hdc.lighting_exposure(light_para, exposure_para);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					if (mc.para.HDC.modelPAD.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelPAD.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
                if (mode == SELECT_FIND_MODEL.HDC_PADC1)
                {
                    mc.hdc.LIVE = false;
                    #region HDC.req
                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_NCC;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC1_SHAPE;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_1;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_1;
                        }
                    }
                    else
                    {
                        mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    mc.hdc.lighting_exposure(light_para, exposure_para);

                    //mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
                    mc.hdc.req = true;

                    #endregion
                    mc.main.Thread_Polling();
                    #region HDC result
                    double rX = 0;
                    double rY = 0;
                    double rT = 0;
                    if (mc.para.HDC.modelPADC1.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_NCC].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC1_SHAPE].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            rX = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultX;
                            rY = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultY;
                            rT = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_1].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            rX = mc.hdc.cam.edgeIntersection.resultX;
                            rY = mc.hdc.cam.edgeIntersection.resultY;
                            rT = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                    }
                    #endregion
                    TB_Result.Clear();
                    TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
                    TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
                    mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

                    mc.idle(1000);

                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.HDC_PADC2)
                {
                    mc.hdc.LIVE = false;
                    #region HDC.req
                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_NCC;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC2_SHAPE;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_2;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_2;
                        }
                    }
                    else
                    {
                        mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    mc.hdc.lighting_exposure(light_para, exposure_para);

                    //mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
                    mc.hdc.req = true;

                    #endregion
                    mc.main.Thread_Polling();
                    #region HDC result
                    double rX = 0;
                    double rY = 0;
                    double rT = 0;
                    if (mc.para.HDC.modelPADC2.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_NCC].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC2_SHAPE].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            rX = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultX;
                            rY = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultY;
                            rT = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_2].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            rX = mc.hdc.cam.edgeIntersection.resultX;
                            rY = mc.hdc.cam.edgeIntersection.resultY;
                            rT = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                    }
                    #endregion
                    TB_Result.Clear();
                    TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
                    TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
                    mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

                    mc.idle(1000);

                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.HDC_PADC3)
                {
                    mc.hdc.LIVE = false;
                    #region HDC.req
                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_NCC;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC3_SHAPE;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_3;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_3;
                        }
                    }
                    else
                    {
                        mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    mc.hdc.lighting_exposure(light_para, exposure_para);

                    //mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
                    mc.hdc.req = true;

                    #endregion
                    mc.main.Thread_Polling();
                    #region HDC result
                    double rX = 0;
                    double rY = 0;
                    double rT = 0;
                    if (mc.para.HDC.modelPADC3.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_NCC].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC3_SHAPE].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            rX = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultX;
                            rY = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultY;
                            rT = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_3].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            rX = mc.hdc.cam.edgeIntersection.resultX;
                            rY = mc.hdc.cam.edgeIntersection.resultY;
                            rT = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                    }
                    #endregion
                    TB_Result.Clear();
                    TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
                    TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
                    mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

                    mc.idle(1000);

                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
                if (mode == SELECT_FIND_MODEL.HDC_PADC4)
                {
                    mc.hdc.LIVE = false;
                    #region HDC.req
                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_NCC;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_MODEL;
                            mc.hdc.reqModelNumber = (int)HDC_MODEL.PADC4_SHAPE;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_PROJECTION_QUARTER_4;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            mc.hdc.reqMode = REQMODE.FIND_EDGE_QUARTER_4;
                        }
                    }
                    else
                    {
                        mc.hdc.reqMode = REQMODE.GRAB;
                    }
                    mc.hdc.lighting_exposure(light_para, exposure_para);

                    //mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
                    mc.hdc.req = true;

                    #endregion
                    mc.main.Thread_Polling();
                    #region HDC result
                    double rX = 0;
                    double rY = 0;
                    double rT = 0;
                    if (mc.para.HDC.modelPADC4.isCreate.value == (int)BOOL.TRUE)
                    {
                        if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.NCC)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_NCC].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.SHAPE)
                        {
                            rX = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultX;
                            rY = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultY;
                            rT = mc.hdc.cam.model[(int)HDC_MODEL.PADC4_SHAPE].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.PROJECTION)
                        {
                            rX = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultX;
                            rY = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultY;
                            rT = mc.hdc.cam.projectionEdge[(int)SELECT_CORNER.PAD_CORNER_4].resultAngle;
                        }
                        else if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                        {
                            rX = mc.hdc.cam.edgeIntersection.resultX;
                            rY = mc.hdc.cam.edgeIntersection.resultY;
                            rT = mc.hdc.cam.edgeIntersection.resultAngleH;
                        }
                    }
                    #endregion
                    TB_Result.Clear();
                    TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
                    TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
                    mc.log.debug.write(mc.log.CODE.ETC, "X : " + Math.Round(rX, 3).ToString() + "/ Y : " + Math.Round(rY, 3).ToString());

                    mc.idle(1000);

                    mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
                }
				#endregion

				#region HDC_FIDUCIAL
				if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
				{
					mc.hdc.LIVE = false;
					#region move pd
					if (mc.para.HDC.fiducialPos.value == 0)
					{
						posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
					}
					else if (mc.para.HDC.fiducialPos.value == 1)
					{
						posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
					}
					else if (mc.para.HDC.fiducialPos.value == 2)
					{
						posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
					}
					else
					{
						posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
						posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
					}
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
					mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
					#endregion
					#region HDC.req
					if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FIDUCIAL_NCC;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.hdc.reqMode = REQMODE.FIND_MODEL;
							mc.hdc.reqModelNumber = (int)HDC_MODEL.PAD_FICUCIAL_SHAPE;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							if (mc.para.HDC.fiducialPos.value == 0) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER1;
							else if (mc.para.HDC.fiducialPos.value == 1) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER2;
							else if (mc.para.HDC.fiducialPos.value == 2) mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER3;
							else mc.hdc.reqMode = REQMODE.FIND_CIRCLE_QUARTER4;
						}
					}
					else
					{
						mc.hdc.reqMode = REQMODE.GRAB;
					}
                    mc.hdc.lighting_exposure(light_para, exposure_para);

					mc.hdc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.hdc.req = true;

					#endregion
					mc.main.Thread_Polling();
					#region HDC result
					double rX = 0;
					double rY = 0;
					double rT = 0;
					double rD = 0;	// Diameter
					if (mc.para.HDC.modelFiducial.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FIDUCIAL_NCC].resultAngle;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultX;
							rY = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultY;
							rT = mc.hdc.cam.model[(int)HDC_MODEL.PAD_FICUCIAL_SHAPE].resultAngle;
						}
						else if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.CIRCLE)
						{
							rX = mc.hdc.cam.circleCenter.resultX;
							rY = mc.hdc.cam.circleCenter.resultY;
							rT = 0;
							if (mc.hdc.cam.circleCenter.resultRadius < 0) rD = mc.hdc.cam.circleCenter.resultRadius;
							else rD = mc.hdc.cam.circleCenter.resultRadius * 2.0;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X        : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y        : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Diameter : " + Math.Round(rD, 3).ToString() + "\n");

					mc.idle(1000);
					mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;
				}
				#endregion

				//---------------------------------------------------------------------------------------------------------------------------------------------
				#region ULC_ORIENTATION
				if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
				{
					mc.ulc.LIVE = false;
					#region moving ulc
                    mc.hd.tool.jogMove((int)UnitCodeHead.HD1, mc.hd.tool.tPos.x[0].ULC, mc.hd.tool.tPos.y[0].ULC, mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT, mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
					mc.idle(100);
					#endregion
					#region ULC.req
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_NCC;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_SHAPE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
                    mc.ulc.lighting_exposure(light_para, exposure_para);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region moving ulc 보상위치
					double rX = 0;
					double rY = 0;
					double rT = 0;
					#region ULC result
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
						}
					}
					#endregion
					TB_Result.Clear();
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");

					#endregion

					#region ULC.req
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_NCC;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							mc.ulc.reqMode = REQMODE.FIND_MODEL;
							mc.ulc.reqModelNumber = (int)ULC_MODEL.PKG_ORIENTATION_SHAPE;
						}
					}
					else
					{
						mc.ulc.reqMode = REQMODE.GRAB;
					}
                    mc.ulc.lighting_exposure(light_para, exposure_para);
					mc.ulc.triggerMode = TRIGGERMODE.SOFTWARE;
					mc.ulc.req = true;
					#endregion
					mc.main.Thread_Polling();
					#region ULC result
					if (mc.para.ULC.modelHSOrientation.isCreate.value == (int)BOOL.TRUE)
					{
						if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.NCC)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_NCC].resultAngle;
						}
						else if (mc.para.ULC.modelHSOrientation.algorism.value == (int)MODEL_ALGORISM.SHAPE)
						{
							rX = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultX;
							rY = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultY;
							rT = mc.ulc.cam.model[(int)ULC_MODEL.PKG_ORIENTATION_SHAPE].resultAngle;
						}
					}
					TB_Result.AppendText("Result X     : " + Math.Round(rX, 3).ToString() + "\n");
					TB_Result.AppendText("Result Y     : " + Math.Round(rY, 3).ToString() + "\n");
					TB_Result.AppendText("Result Angle : " + Math.Round(rT, 3).ToString() + "\n");
					#endregion

					mc.idle(1000);
					mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				}
				#endregion

			}
			#endregion

            if (sender.Equals(bt_create_rect))
            {
                mc.hdc.LIVE = false;
                mc.hdc.cam.grabSofrwareTrigger();
                mc.hdc.cam.findModel((int)HDC_MODEL.PADC2_NCC);
                //mc.hdc.cam.grab();

                //int a, b;
                ////HTuple a;
                //mc.hdc.cam.drawCross(out a, out b);
                //posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
                //posY = mc.hd.tool.cPos.y.PADC1(padIndexX);
                //mc.hd.tool.jogMove(posX, posY, out ret.message);

                //mc.hdc.LIVE = false;

                //mc.hdc.cam.grab();
                //mc.para.HDC.modelPADC1.quardrant.value = (int)QUARDRANT.QUARDRANT_RT;
                //mc.para.HDC.modelPADC1.proj_Type.value = (int)PROJECTION_TYPE.PROJECTION_POSITIVE;
                //mc.para.HDC.modelPADC1.proj_Direction.value = 1;
                //mc.para.HDC.modelPADC1.proj_EdgeFilter.value = 30;
                //mc.para.HDC.modelPADC1.proj_MinTh.value = 200;
                //mc.para.HDC.modelPADC1.proj_MaxTh.value = 255;

                //mc.hdc.reqModelNumber = (int)SELECT_CORNER.PAD_CORNER_1;
                //mc.hdc.cam.refresh_reqModelNumber = mc.hdc.reqModelNumber;

                //mc.hdc.cam.createCornerRegion(mc.hdc.reqModelNumber);
                //mc.hdc.cam.DispBinaryRectImage(mc.hdc.reqModelNumber
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.quardrant.value)
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Type.value)
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_Direction.value)
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_EdgeFilter.value)
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MinTh.value)
                //    , Convert.ToInt32(mc.para.HDC.modelPADC1.proj_MaxTh.value));

                //mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;
                //mc.hdc.LIVE = true;
                //mc.para.HDC.modelPADC1.isCreate.value = (int)BOOL.TRUE;
            }

			if (sender.Equals(BT_ESC))
			{
				//EVENT.hWindow2Display(); 프로그램이 Hang-Up된다.
				this.Close();
			}
		EXIT:
			refresh();
			isRunning = false;
			this.Enabled = true;

		}

		private void FormHalconModelTeach_Load(object sender, EventArgs e)
		{
			this.Left = 620;
			this.Top = 170;
			this.TopMost = true;
			BT_Test.Visible = true;
            bt_create_rect.Visible = false;

            SB_Channel1.Maximum = 255 + 9;
            SB_Channel2.Maximum = 255 + 9;
            SB_Exposure.Maximum = 30000 + 9;

            if (mode == SELECT_FIND_MODEL.HDC_PAD)
            {
                light_para = mc.para.HDC.modelPAD.light;
                exposure_para = mc.para.HDC.modelPAD.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.HDC_PADC1)
            {
                light_para = mc.para.HDC.modelPADC1.light;
                exposure_para = mc.para.HDC.modelPADC1.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.HDC_PADC2)
            {
                light_para = mc.para.HDC.modelPADC2.light;
                exposure_para = mc.para.HDC.modelPADC2.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.HDC_PADC3)
            {
                light_para = mc.para.HDC.modelPADC3.light;
                exposure_para = mc.para.HDC.modelPADC3.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.HDC_PADC4)
            {
                light_para = mc.para.HDC.modelPADC4.light;
                exposure_para = mc.para.HDC.modelPADC4.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.ULC_PKG)
            {
                light_para = mc.para.ULC.model.light;
                exposure_para = mc.para.ULC.model.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.ULC_CORNER1)
            {
                light_para = mc.para.ULC.modelCorner1.light;
                exposure_para = mc.para.ULC.modelCorner1.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.ULC_CORNER2)
            {
                light_para = mc.para.ULC.modelCorner2.light;
                exposure_para = mc.para.ULC.modelCorner2.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.ULC_CORNER3)
            {
                light_para = mc.para.ULC.modelCorner3.light;
                exposure_para = mc.para.ULC.modelCorner3.exposureTime;
            }
            else if (mode == SELECT_FIND_MODEL.ULC_CORNER4)
            {
                light_para = mc.para.ULC.modelCorner4.light;
                exposure_para = mc.para.ULC.modelCorner4.exposureTime;
            }
            else
            {
                light_para.ch1.value = 0;
                light_para.ch2.value = 0;
                exposure_para.value = 1000;
            }
            SB_Channel1.Value = (int)light_para.ch1.value;
            SB_Channel2.Value = (int)light_para.ch2.value;
            SB_Exposure.Value = (int)exposure_para.value;
            light_control();

			#region ULC
			if (mode == SELECT_FIND_MODEL.ULC_PKG)
			{
                mc.ulc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].ULC;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].ULC;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				if (mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.ULC.model.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				{
					BT_AutoTeach.Visible = true;
				}
				else BT_AutoTeach.Visible = false;

                BT_MOVE_CORNER.Visible = false;
                BT_MOVE_PATTERN.Visible = false;
			}
            if (mode == SELECT_FIND_MODEL.ULC_CORNER1)
            {
                mc.ulc.lighting_exposure(light_para, exposure_para);
                EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LIDC1;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LIDC1;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_Teach.Text = "SAVE";
                BT_ESC.Visible = true;

                BT_JogT_CCW.Visible = true;
                BT_JogT_CW.Visible = true;
                BT_SpeedT.Visible = true;

                BT_Test.Visible = true;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
                BT_MOVE_CORNER.Visible = false;
                BT_MOVE_PATTERN.Visible = false;
            }
            if (mode == SELECT_FIND_MODEL.ULC_CORNER2)
            {
                mc.ulc.lighting_exposure(light_para, exposure_para);
                EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LIDC2;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LIDC2;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_Teach.Text = "SAVE";
                BT_ESC.Visible = true;

                BT_JogT_CCW.Visible = true;
                BT_JogT_CW.Visible = true;
                BT_SpeedT.Visible = true;

                BT_Test.Visible = true;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
                BT_MOVE_CORNER.Visible = false;
                BT_MOVE_PATTERN.Visible = false;
            }
            if (mode == SELECT_FIND_MODEL.ULC_CORNER3)
            {
                mc.ulc.lighting_exposure(light_para, exposure_para);
                EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LIDC3;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LIDC3;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_Teach.Text = "SAVE";
                BT_ESC.Visible = true;

                BT_JogT_CCW.Visible = true;
                BT_JogT_CW.Visible = true;
                BT_SpeedT.Visible = true;

                BT_Test.Visible = true;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
                BT_MOVE_CORNER.Visible = false;
                BT_MOVE_PATTERN.Visible = false;
            }
            if (mode == SELECT_FIND_MODEL.ULC_CORNER4)
            {
                mc.ulc.lighting_exposure(light_para, exposure_para);
                EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[(int)UnitCodeHead.HD1].LIDC4;
                posY = mc.hd.tool.tPos.y[(int)UnitCodeHead.HD1].LIDC4;
                posZ = mc.hd.tool.tPos.z[(int)UnitCodeHead.HD1].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

                _posX = posX;
                _posY = posY;
                _posT = posT;
                dXY = 10; dT = 1;

                BT_Teach.Visible = true;
                BT_Teach.Text = "SAVE";
                BT_ESC.Visible = true;

                BT_JogT_CCW.Visible = true;
                BT_JogT_CW.Visible = true;
                BT_SpeedT.Visible = true;

                BT_Test.Visible = true;

                BT_Teach.Visible = true;
                BT_ESC.Visible = true;
                BT_AutoTeach.Visible = true;
                BT_MOVE_CORNER.Visible = false;
                BT_MOVE_PATTERN.Visible = false;
            }
			#endregion
			#region HDC_PAD
			if (mode == SELECT_FIND_MODEL.HDC_PAD)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.cPos.x.PAD(padIndexX);
				posY = mc.hd.tool.cPos.y.PAD(padIndexY);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_AutoTeach.Visible = false;
				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
				BT_Test.Visible = true;
                BT_MOVE_CORNER.Visible = true;
                BT_MOVE_PATTERN.Visible = true;
			}
			#endregion
			#region HDC_PADC1
			if (mode == SELECT_FIND_MODEL.HDC_PADC1)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                BT_Test.Visible = true;
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
                
                if (mc.para.HDC.modelPADC1.algorism.value == (int)MODEL_ALGORISM.CORNER)
                {
                    BT_AutoTeach.Visible = true;
                    BT_Teach.Visible = true;
                    BT_Teach.Text = "SAVE";
                }
                else BT_AutoTeach.Visible = false;
				
                BT_MOVE_CORNER.Visible = true;
                BT_MOVE_PATTERN.Visible = true;
			}
			#endregion
			#region HDC_PADC2
			if (mode == SELECT_FIND_MODEL.HDC_PADC2)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
                if (mc.para.HDC.modelPADC2.algorism.value == (int)MODEL_ALGORISM.CORNER)
                {
                    BT_AutoTeach.Visible = true;
                    BT_Teach.Visible = true;
                    BT_Teach.Text = "SAVE";
                }
                else BT_AutoTeach.Visible = false;
                BT_MOVE_CORNER.Visible = true;
                BT_MOVE_PATTERN.Visible = true;
			}
			#endregion
			#region HDC_PADC3
			if (mode == SELECT_FIND_MODEL.HDC_PADC3)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
                if (mc.para.HDC.modelPADC3.algorism.value == (int)MODEL_ALGORISM.CORNER)
                {
                    BT_AutoTeach.Visible = true;
                    BT_Teach.Visible = true;
                    BT_Teach.Text = "SAVE";
                }
                else BT_AutoTeach.Visible = false;
                BT_MOVE_CORNER.Visible = true;
                BT_MOVE_PATTERN.Visible = true;
			}
			#endregion
			#region HDC_PADC4
			if (mode == SELECT_FIND_MODEL.HDC_PADC4)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
				posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }

                mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.PROJECTION_EDGE;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;
                if (mc.para.HDC.modelPADC4.algorism.value == (int)MODEL_ALGORISM.CORNER)
                {
                    BT_AutoTeach.Visible = true;
                    BT_Teach.Visible = true;
                    BT_Teach.Text = "SAVE";
                }
                else BT_AutoTeach.Visible = false;
                BT_MOVE_CORNER.Visible = true;
                BT_MOVE_PATTERN.Visible = true;
			}
			#endregion
			#region HDC_FIDUCIAL
			if (mode == SELECT_FIND_MODEL.HDC_FIDUCIAL)
			{
                mc.hdc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.hdc.cam.acq.grabber.cameraNumber);
				if (mc.para.HDC.fiducialPos.value == 0)
				{
					posX = mc.hd.tool.cPos.x.PADC1(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC1(padIndexY);
				}
				else if (mc.para.HDC.fiducialPos.value == 1)
				{
					posX = mc.hd.tool.cPos.x.PADC2(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC2(padIndexY);
				}
				else if (mc.para.HDC.fiducialPos.value == 2)
				{
					posX = mc.hd.tool.cPos.x.PADC3(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC3(padIndexY);
				}
				else
				{
					posX = mc.hd.tool.cPos.x.PADC4(padIndexX);
					posY = mc.hd.tool.cPos.y.PADC4(padIndexY);
				}
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.hdc.LIVE = true; mc.hdc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				BT_AutoTeach.Visible = false;
				//if (mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.NCC || mc.para.HDC.modelFiducial.algorism.value == (int)MODEL_ALGORISM.SHAPE)
				//{
				//    BT_AutoTeach.Visible = true;
				//}
				//else BT_AutoTeach.Visible = false;
			}
			#endregion
			#region ULC_ORIENTATION
			if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION)
			{

                mc.ulc.lighting_exposure(light_para, exposure_para);
				EVENT.hWindowLargeDisplay(mc.ulc.cam.acq.grabber.cameraNumber);
                posX = mc.hd.tool.tPos.x[0].ULC;
                posY = mc.hd.tool.tPos.y[0].ULC;
                posZ = mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT;
                posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
				mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
				mc.ulc.LIVE = true; mc.ulc.liveMode = REFRESH_REQMODE.CENTER_CROSS;

				_posX = posX;
				_posY = posY;
				_posT = posT;
				dXY = 10; dT = 1;

				BT_Teach.Visible = true;
				BT_ESC.Visible = true;

				BT_JogT_CCW.Visible = true;
				BT_JogT_CW.Visible = true;
				BT_SpeedT.Visible = true;

				BT_Test.Visible = true;
				BT_AutoTeach.Visible = false;
			}
			#endregion

			refresh();

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
				BT_SpeedXY.Text = "±" + dXY.ToString();
				BT_SpeedT.Text  = "±" + dT.ToString();
				BT_ESC.Focus();
			}
		}

		private void FormHalconModelTeach_FormClosed(object sender, FormClosedEventArgs e)
		{
			mc.ulc.LIVE = false;
			mc.hdc.LIVE = false;
		}

		void control()
		{
			isRunning = true;
			int interval = 300;
			while (true)
			{
				if (oButton == BT_JogX_Left) posX -= dXY;
				if (oButton == BT_JogX_Right) posX += dXY;
				if (oButton == BT_JogY_Outside) posY += dXY;
				if (oButton == BT_JogY_Inside) posY -= dXY;
				if (oButton == BT_JogT_CCW) posT -= dT;
				if (oButton == BT_JogT_CW) posT += dT;

				// Limit 제한 풀기
// 				if (posX > _posX + 5000) posX = _posX + 5000;
// 				if (posX < _posX - 5000) posX = _posX - 5000;
// 				if (posY > _posY + 5000) posY = _posY + 5000;
// 				if (posY < _posY - 5000) posY = _posY - 5000;
// 				if (posT > _posT + 180) posT = _posT + 180;
// 				if (posT < _posT - 180) posT = _posT - 180;
                posZ = mc.hd.tool.tPos.z[0].ULC_FOCUS_WITH_MT;

				refresh();
				interval -= 50; if (interval < 50) interval = 50;
                //mc.idle(interval);
				#region moving
                //if (mode == SELECT_FIND_MODEL.ULC_ORIENTATION || mode == SELECT_FIND_MODEL.ULC_PKG || mode == SELECT_FIND_MODEL.ULC_CORNER1 || mode == SELECT_FIND_MODEL.ULC_CORNER3)
                //    mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posZ, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
                //else
					mc.hd.tool.jogMove(posX, posY, out ret.message, false); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); goto EXIT; }
				#endregion
				if (bStop) break;
                break;
			}
		EXIT:
			isRunning = false;
		}

		private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (isRunning) return;
			oButton = sender;
			bStop = false;
			Thread th = new Thread(control);
			th.Name = "FormHalconModelTeach_MouseDownThread";
			th.Start();
			mc.log.processdebug.write(mc.log.CODE.INFO, "FormHalconModelTeach_MouseDownThread");
		}

		private void Control_MouseLeave(object sender, EventArgs e)
		{
			//oButton = null;
			bStop = true;
		}

		private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			//oButton = null;
			bStop = true;
		}

        private void Move_Pos(object sender, EventArgs e)
        {
            if (sender.Equals(BT_MOVE_CORNER) || sender.Equals(BT_MOVE_PATTERN))
            {
                bool movePatternPos = false;

                if (sender.Equals(BT_MOVE_CORNER))
                {
                    movePatternPos = false;
                }
                if (sender.Equals(BT_MOVE_PATTERN))
                {
                    movePatternPos = true;
                }

                #region HDC_PAD
                if (mode == SELECT_FIND_MODEL.HDC_PAD)
                {
                    posX = mc.hd.tool.cPos.x.PAD(padIndexX);
                    posY = mc.hd.tool.cPos.y.PAD(padIndexY);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                }
                #endregion
                #region HDC_PADC1
                if (mode == SELECT_FIND_MODEL.HDC_PADC1)
                {
                    posX = mc.hd.tool.cPos.x.PADC1(padIndexX, movePatternPos);
                    posY = mc.hd.tool.cPos.y.PADC1(padIndexY, movePatternPos);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                }
                #endregion
                #region HDC_PADC2
                if (mode == SELECT_FIND_MODEL.HDC_PADC2)
                {
                    posX = mc.hd.tool.cPos.x.PADC2(padIndexX, movePatternPos);
                    posY = mc.hd.tool.cPos.y.PADC2(padIndexY, movePatternPos);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                }
                #endregion
                #region HDC_PADC3
                if (mode == SELECT_FIND_MODEL.HDC_PADC3)
                {
                    posX = mc.hd.tool.cPos.x.PADC3(padIndexX, movePatternPos);
                    posY = mc.hd.tool.cPos.y.PADC3(padIndexY, movePatternPos);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                    mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }
                }
                #endregion
                #region HDC_PADC4
                if (mode == SELECT_FIND_MODEL.HDC_PADC4)
                {
                    posX = mc.hd.tool.cPos.x.PADC4(padIndexX, movePatternPos);
                    posY = mc.hd.tool.cPos.y.PADC4(padIndexY, movePatternPos);
                    posT = mc.hd.tool.tPos.t[(int)UnitCodeHead.HD1].ZERO;
                }
                #endregion

                mc.hd.tool.jogMove((int)UnitCodeHead.HD1, posX, posY, posT, out ret.message); if (ret.message != RetMessage.OK) { mc.message.alarmMotion(ret.message); this.Close(); }

                _posX = posX;
                _posY = posY;
                _posT = posT;
            }
        }

        void light_control()
        {
            if (mode == SELECT_FIND_MODEL.HDC_PAD || mode == SELECT_FIND_MODEL.HDC_PADC1
                || mode == SELECT_FIND_MODEL.HDC_PADC2 || mode == SELECT_FIND_MODEL.HDC_PADC3 ||mode == SELECT_FIND_MODEL.HDC_PADC4)
            {
                mc.light.HDC(light_para, out ret.b);
                if (!ret.b) mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "Lighting Controller"));
                LB_Channel1Value.Text = light_para.ch1.value.ToString();
                LB_Channel2Value.Text = light_para.ch2.value.ToString();

                mc.hdc.cam.acq.exposureTime = exposure_para.value;
                if (exposure_para.value != mc.hdc.cam.acq.exposureTime)
                {
                    mc.message.alarm("Exposure Error");
                }
                LB_ExposureValue.Text = mc.hdc.cam.acq.exposureTime.ToString();
            }
            else if (mode == SELECT_FIND_MODEL.ULC_PKG || mode == SELECT_FIND_MODEL.ULC_CORNER1
                || mode == SELECT_FIND_MODEL.ULC_CORNER2 || mode == SELECT_FIND_MODEL.ULC_CORNER3
                || mode == SELECT_FIND_MODEL.ULC_CORNER4)
            {
                mc.light.ULC(light_para, out ret.b);
                if (!ret.b) mc.message.alarm(String.Format(textResource.MB_ETC_COMM_ERROR, "Lighting Controller"));
                LB_Channel1Value.Text = light_para.ch1.value.ToString();
                LB_Channel2Value.Text = light_para.ch2.value.ToString();

                mc.ulc.cam.acq.exposureTime = exposure_para.value;
                if (exposure_para.value != mc.ulc.cam.acq.exposureTime)
                {
                    mc.message.alarm("Exposure Error");
                }
                LB_ExposureValue.Text = mc.ulc.cam.acq.exposureTime.ToString();
            }
        }

        private void SB_Scrol(object sender, ScrollEventArgs e)
        {
            if (SB_Channel1.Value < 0) SB_Channel1.Value = 0;
            if (SB_Channel1.Value > 255) SB_Channel1.Value = 255;
            if (SB_Channel2.Value < 0) SB_Channel2.Value = 0;
            if (SB_Channel2.Value > 255) SB_Channel2.Value = 255;
            if (SB_Exposure.Value < 100) SB_Exposure.Value = 100;
            if (SB_Exposure.Value > 30000) SB_Exposure.Value = 30000;

            light_para.ch1.value = SB_Channel1.Value;
            light_para.ch2.value = SB_Channel2.Value;
            exposure_para.value = SB_Exposure.Value;

            light_control();
        }
	}
}
