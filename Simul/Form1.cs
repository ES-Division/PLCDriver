using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace PLCSimulation
{
    public partial class Form1 : Form
    {
        List<SimulForm> lstChildForm;
        SimulForm currentChildForm;

        uint index = 0; // simulForm Text index
        const int MAX_FORM_COUNT = 10;
        #region Mdi Form AutoScroll Disable...?
        MdiClient mdiClient = null;
        const int SB_BOTH = 3;
        const int WM_NCCALCSIZE = 0x83;
        [DllImport("user32.dll")]
        private static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        protected override void WndProc(ref Message m)
        {
            try
            {
                if (mdiClient != null)
                {
                    ShowScrollBar(mdiClient.Handle, SB_BOTH, 0);
                }
                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient)
                    mdiClient = c as MdiClient;
            }
        }
        #endregion

        bool bOnceSave = false;
        readonly string downloadsPath = System.IO.Path.DirectorySeparatorChar + "Downloads";
        public Form1()
        {
            InitializeComponent();

            window_Menu.Visible = false;
            lstChildForm = new List<SimulForm>();

            saveFileDialog.Filter = "시뮬레이터 파일|*.bat";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + downloadsPath;
            openFileDialog.Filter = "시뮬레이터 파일|*.bat";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + downloadsPath;
        }

        private void menu_New_Click(object sender, EventArgs e)
        {
            if (lstChildForm.Count >= MAX_FORM_COUNT)
            {
                MessageBox.Show("Max Simulator 10");
                return;
            }

            SimulForm simulForm = new SimulForm();
            simulForm.Text += $"_{index++}";
            simulForm.MdiParent = this;
            simulForm.FormClosed += F2_FormClosed;
            simulForm.Activated += SimulForm_Activated;
            simulForm.Show();
            
            lstChildForm.Add(simulForm);

            if (window_Menu.Visible == false)
            {
                window_Menu.Visible = true;
            }

            ToolStripMenuItem tsmi = new ToolStripMenuItem($"&{simulForm.Text}");
            tsmi.Click += Tsmi_Click;
            window_Menu.DropDownItems.Add(tsmi);
        }

        private void Tsmi_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var strContain = menuItem.Text.Replace("&","");

            foreach (var simulForm in lstChildForm)
            {
                if (simulForm.Text.Contains(strContain))
                {
                    simulForm.Activate();
                    break;
                }
            }
        }

        private void F2_FormClosed(object sender, FormClosedEventArgs e)
        {
            var simulForm = sender as SimulForm;
            lstChildForm.Remove(simulForm);

            simulForm.SimulatorClose();

            var strContain = simulForm.Text;
            ToolStripMenuItem removeItem = new ToolStripMenuItem(string.Empty);
            foreach (ToolStripMenuItem dropDownItem in window_Menu.DropDownItems)
            {
                if (dropDownItem.Text.Contains(strContain))
                {
                    removeItem = dropDownItem;
                    break;
                }
            }

            if(!string.IsNullOrEmpty(removeItem.Text))
            {
                window_Menu.DropDownItems.Remove(removeItem);
            }

            if (lstChildForm.Count == 0)
            {
                window_Menu.Visible = false;
                currentChildForm = null;
            }
        }

        public bool PortCheck(string port, int index)
        {
            foreach (var childForm in lstChildForm)
            {
                // 자신 skip
                var containIndex = Convert.ToInt32(childForm.Text.Split('_')[1]);
                if (containIndex == index)
                    continue;

                var formPort = childForm.Port;
                if (!string.IsNullOrEmpty(formPort))
                {
                    if (formPort == port)
                        return false;
                }
            }

            return true;
        }

        private void menu_Save_Click(object sender, EventArgs e)
        {
            if (currentChildForm == null)
            {
                MessageBox.Show("There is no Simulator to save.");
                return;
            }

            if (bOnceSave == false)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (FileSaveProc(saveFileDialog.FileName))
                    {
                        bOnceSave = true;
                        MessageBox.Show("Save Success!");
                    }
                    else
                        MessageBox.Show("Save Fail..!");
                }
            }
        }

        private void menu_SaveAs_Click(object sender, EventArgs e)
        {
            if (currentChildForm == null)
            {
                MessageBox.Show("There is no Simulator to save.");
                return;
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (FileSaveProc(saveFileDialog.FileName))
                {
                    bOnceSave = true;
                    MessageBox.Show("Save Success!");
                }
                else
                    MessageBox.Show("Save Fail..!");
            }
        }

        private void menu_Open_Click(object sender, EventArgs e)
        {
            if (lstChildForm.Count >= MAX_FORM_COUNT)
            {
                MessageBox.Show("can no longer be loaded. Max Simulator 10.");
                return;
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (FileLoadProc(openFileDialog.FileName))
                {
                    MessageBox.Show("Load Success!");
                }
                else
                    MessageBox.Show("Load Fail..!");
            }

            openFileDialog.FileName = System.IO.Path.GetFileName(openFileDialog.FileName);
        }

        private void menu_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool FileSaveProc(string path)
        {
            try
            {
                #region 한번에 저장 ... ?
                //List<SaveLoadData> lstData = new();

                //foreach (var simulForm in lstChildForm)
                //{
                //    lstData.Add(new SaveLoadData()
                //    {
                //        Port = simulForm.Port,
                //        IP = simulForm.IP,
                //        Mode = simulForm.Mode,
                //        Protocol = simulForm.Protocol,
                //        FormIndex = simulForm.FormIndex,
                //        MSize = simulForm.MSize,
                //        DSize = simulForm.DSize,
                //        KSize = simulForm.KSize,
                //        LSize = simulForm.LSize,
                //        CoilSize = simulForm.COilSize,
                //        InputSize = simulForm.InputSize,
                //        HoldingRegisterSize = simulForm.HoldingRegisterSize,
                //        InputRegisterSize = simulForm.InputRegisterSize
                //    });
                //}

                //using (var stream = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    bf.Serialize(stream, lstData);
                //    stream.Close();
                //}
                #endregion

                #region 하나만 저장
                var simulForm = currentChildForm;

                SaveLoadData slData = new()
                {
                    Port = simulForm.Port,
                    IP = simulForm.IP,
                    Mode = simulForm.Mode,
                    Protocol = simulForm.Protocol,
                    MSize = simulForm.MSize,
                    DSize = simulForm.DSize,
                    KSize = simulForm.KSize,
                    LSize = simulForm.LSize,
                    CoilSize = simulForm.CoilSize,
                    InputSize = simulForm.InputSize,
                    HoldingRegisterSize = simulForm.HoldingRegisterSize,
                    InputRegisterSize = simulForm.InputRegisterSize
                };

                using (var stream = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(stream, slData);
                    stream.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }
        
        private bool FileLoadProc(string path)
        {
            try
            {

                #region 하나만 로드
                SaveLoadData slData;

                using (var stream = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    BinaryFormatter bf = new();
                    slData = (SaveLoadData)bf.Deserialize(stream);
                    stream.Close();
                }

                menu_New_Click(null, null);

                if (string.IsNullOrEmpty(slData.IP))
                    return true;

                currentChildForm.IP = slData.IP;
                currentChildForm.Port = slData.Port;
                currentChildForm.Mode = slData.Mode;
                currentChildForm.Protocol = slData.Protocol;

                switch (currentChildForm.Mode)
                {
                    case "FEnet":
                        currentChildForm.MSize = slData.MSize;
                        currentChildForm.DSize = slData.DSize;
                        currentChildForm.KSize = slData.KSize;
                        currentChildForm.LSize = slData.LSize;
                        break;
                    case "Modbus":
                        currentChildForm.CoilSize = slData.CoilSize;
                        currentChildForm.InputSize = slData.InputSize;
                        currentChildForm.HoldingRegisterSize = slData.HoldingRegisterSize;
                        currentChildForm.InputRegisterSize = slData.InputRegisterSize;
                        break;
                }

                currentChildForm.InitMode = false;
                #endregion

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private void SimulForm_Activated(object sender, EventArgs e)
        {
            var simulForm = (sender as SimulForm);
            currentChildForm = simulForm;
        }
    }
}
