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

namespace PSA_Application
{
    public partial class FormMagazineEdit : Form
    {
        public FormMagazineEdit()
        {
            InitializeComponent();
        }

        MG_STATUS status = MG_STATUS.INVALID;
		static int[,] tmp_MG_Status = new int[(int)MG_NUM.MAX, (int)SLOT_NUM.MAX];
		bool SelectAll;

		System.Windows.Forms.Button[,] MGbtnArray;
		
        private void Control_Click(object sender, EventArgs e)
        {
            if (sender.Equals(BT_READY)) 
			{
				status = MG_STATUS.READY;
				BT_MGStatus.BackColor = Color.Yellow;
				BT_MGStatus.ForeColor = Color.Black;
				BT_MGStatus.Text = "READY";
			}
			if (sender.Equals(BT_DONE)) 
			{
				status = MG_STATUS.DONE;
				BT_MGStatus.BackColor = Color.LimeGreen;
				BT_MGStatus.ForeColor = Color.Black;
				BT_MGStatus.Text = "DONE";
			}
			if (sender.Equals(BT_SKIP)) 
			{
				status = MG_STATUS.SKIP;
				BT_MGStatus.BackColor = Color.Black;
				BT_MGStatus.ForeColor = Color.White;
				BT_MGStatus.Text = "SKIP";
			}
			if (sender.Equals(BT_Cancel))
			{
				this.Close();
			}
			if (sender.Equals(BT_Apply))
			{
				for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
					for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
						EVENT.refreshEditMagazine(x, y);
				//mc.Magazinecontrol.MG_Status = tmp_MG_Status;
				mc.UnloaderControl.writeconfig();
				this.Close();
			}
			if (sender.Equals(CB_SELECTALL))
			{
				if (CB_SELECTALL.Checked) SelectAll = true;
				else SelectAll = false;
			}
			if (sender.Equals(BT_MGStatus))
			{
				if (SelectAll)
				{
					for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
						for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
						{
                            MGbtnArray[x, y].ForeColor = Color.Black;
                            if (status == MG_STATUS.SKIP)
                            {
                                MGbtnArray[x, y].BackColor = Color.Black;
                                MGbtnArray[x, y].ForeColor = Color.White;
                            }
                            else if (status == MG_STATUS.READY) MGbtnArray[x, y].BackColor = Color.Yellow;
                            else if (status == MG_STATUS.DONE) MGbtnArray[x, y].BackColor = Color.LimeGreen;

							tmp_MG_Status[x, y] = (int)status;
						}
				}
			}
        }   
  
		Panel panel;
        private void FormMagazineEdit_Load(object sender, EventArgs e)
        {
            this.Location = new System.Drawing.Point(610, 170);
            MGbtnArray = new System.Windows.Forms.Button[mc.UnloaderControl.MG_COUNT, mc.UnloaderControl.MG_SLOT_COUNT];

            mc.UnloaderControl.readconfig();
            tmp_MG_Status = mc.UnloaderControl.MG_Status;
            //backup_MG_Status = mc.Magazinecontrol.MG_Status;
            int sY, oy;

            for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
            {
                for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
                {
                    if (x == 0) panel = panel1;
                    else if (x == 1) panel = panel2;
                    else if (x == 2) panel = panel3;
                    panel.Controls.Remove(MGbtnArray[x, y]);
                }
            }


            for (int x = 0; x < mc.UnloaderControl.MG_COUNT; x++)
            {
                for (int y = 0; y < mc.UnloaderControl.MG_SLOT_COUNT; y++)
                {
                    if (x == 0) panel = panel1;
                    else if (x == 1) panel = panel2;
                    else if (x == 2) panel = panel3;

                    sY = (int)(panel.Size.Height / mc.UnloaderControl.MG_SLOT_COUNT);
                    oy = (int)(panel.Size.Height / mc.UnloaderControl.MG_SLOT_COUNT);

                    MGbtnArray[x, y] = new System.Windows.Forms.Button();
                    MGbtnArray[x, y].Tag = y.ToString();
                    MGbtnArray[x, y].Width = panel.Size.Width;
                    MGbtnArray[x, y].Height = panel.Size.Height / mc.UnloaderControl.MG_SLOT_COUNT;
                    MGbtnArray[x, y].Top = (sY * (mc.UnloaderControl.MG_SLOT_COUNT - y - 1));
                    MGbtnArray[x, y].Text = (y + 1).ToString();
                    MGbtnArray[x, y].Font = new System.Drawing.Font("ArialBlack", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    panel.Controls.Add(MGbtnArray[x, y]);

                    if (tmp_MG_Status[x, y] == (int)MG_STATUS.DONE) MGbtnArray[x, y].BackColor = Color.LimeGreen;
                    else if (tmp_MG_Status[x, y] == (int)MG_STATUS.READY) MGbtnArray[x, y].BackColor = Color.Yellow;
                    else if (tmp_MG_Status[x, y] == (int)MG_STATUS.SKIP)
                    {
                        MGbtnArray[x, y].BackColor = Color.Black;
                        MGbtnArray[x, y].ForeColor = Color.White;
                    }

                    if (x == 0)
                        MGbtnArray[x, y].MouseDown += new MouseEventHandler(ClickButton);
                    else if (x == 1)
                        MGbtnArray[x, y].MouseDown += new MouseEventHandler(ClickButton2);
                    else if (x == 2)
                        MGbtnArray[x, y].MouseDown += new MouseEventHandler(ClickButton3);
                }
            }
        }

		
		private void ClickButton(Object sender, MouseEventArgs e)
		{
			object tag;
			tag = ((System.Windows.Forms.Button)sender).Tag;

            MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].ForeColor = Color.Black;

            if (status == MG_STATUS.SKIP)
            {
                MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].BackColor = Color.Black;
                MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].ForeColor = Color.White;
            }
            else if (status == MG_STATUS.READY) MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].BackColor = Color.Yellow;
            else if (status == MG_STATUS.DONE) MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].BackColor = Color.LimeGreen;
			//else if (status == MG_STATUS.ERROR) MGbtnArray[(int)MG_NUM.MG1, Convert.ToInt32(tag)].BackColor = Color.Red;

			tmp_MG_Status[(int)MG_NUM.MG1, Convert.ToInt32(tag)] = (int)status;
		}

		private void ClickButton2(Object sender, EventArgs e)
		{
			object tag;
			tag = ((System.Windows.Forms.Button)sender).Tag;

            MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].ForeColor = Color.Black;

            if (status == MG_STATUS.SKIP)
            {
                MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].BackColor = Color.Black;
                MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].ForeColor = Color.White;
            }
            else if (status == MG_STATUS.READY) MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].BackColor = Color.Yellow;
            else if (status == MG_STATUS.DONE) MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].BackColor = Color.LimeGreen;
			//else if (status == MG_STATUS.ERROR) MGbtnArray[(int)MG_NUM.MG2, Convert.ToInt32(tag)].BackColor = Color.Red;

			tmp_MG_Status[(int)MG_NUM.MG2, Convert.ToInt32(tag)] = (int)status;
		}

		private void ClickButton3(Object sender, EventArgs e)
		{
			object tag;
			tag = ((System.Windows.Forms.Button)sender).Tag;

            MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].ForeColor = Color.Black;

            if (status == MG_STATUS.SKIP)
            {
                MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].BackColor = Color.Black;
                MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].ForeColor = Color.White;
            }
            else if (status == MG_STATUS.READY) MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].BackColor = Color.Yellow;
            else if (status == MG_STATUS.DONE) MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].BackColor = Color.LimeGreen;
			//else if (status == MG_STATUS.ERROR) MGbtnArray[(int)MG_NUM.MG3, Convert.ToInt32(tag)].BackColor = Color.Red;

			tmp_MG_Status[(int)MG_NUM.MG3, Convert.ToInt32(tag)] = (int)status;
		}
    }
}
