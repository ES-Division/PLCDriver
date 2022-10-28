using System;
using System.Windows.Forms;

namespace PLCSimulation
{
    public partial class Setting : Form
    {
        #region setting value
        public string IP
        {
            get { return tb_IP.Text; }
        }
        public string Port
        {
            get { return tb_Port.Text; }
        }
        public string Mode
        {
            get { return cb_Mode.SelectedItem.ToString(); }
        }
        public string Protocol
        {
            get
            {
                if (ck_TCP.Checked)
                    return ck_TCP.Text;

                return ck_UDP.Text;
            }
        }

        int index;
        #endregion

        #region MemorySize
        public string MSize
        {
            get { return tb_MSize.Text; }
        }
        public string DSize
        {
            get { return tb_DSize.Text; }
        }
        public string KSize
        {
            get { return tb_KSize.Text; }
        }
        public string LSize
        {
            get { return tb_LSize.Text; }
        }
        public string CoilSize
        {
            get { return tb_Coil.Text; }
        }
        public string InputSize
        {
            get { return tb_Input.Text; }
        }
        public string HoldingRegisterSize
        {
            get { return tb_HoldingRegister.Text; }
        }
        public string InputRegisterSize
        {
            get { return tb_InputRegister.Text; }
        }

        public int ProtocolMode
        {
            get { return cb_Mode.SelectedIndex; }
        }
        #endregion

        public Func<string, int, bool> PortSettingFunc;

        public Setting()
        {
            InitializeComponent();
        }

        public Setting(string ip, string port, string mode, int index) : this()
        {
            if (!string.IsNullOrEmpty(ip))
                tb_IP.Text = ip;
            else
                tb_IP.Text = "127.0.0.1";

            if (!string.IsNullOrEmpty(port))
                tb_Port.Text = port;
            else
                tb_Port.Text = "2000";

            if (!string.IsNullOrEmpty(mode))
                cb_Mode.SelectedIndex = cb_Mode.Items.IndexOf(mode);
            else
                cb_Mode.SelectedIndex = 0;

            this.index = index;
        }

        private void b_OK_Click(object sender, EventArgs e)
        {
            // Port 검증이 필요함 Socket Open Port 가 겹치면 안된다!
            var result = PortSettingFunc?.Invoke(tb_Port.Text, index);
            if (result.Value == false)
            {
                MessageBox.Show("This port is already in use. Please enter a different port number.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void b_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ck_TCP_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_TCP.Checked)
                ck_UDP.Checked = false;
            else if (ck_TCP.Checked == false)
            {
                if (ck_UDP.Checked == false)
                {
                    ck_TCP.Checked = true;
                    return;
                }
            }
        }

        private void ck_UDP_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_UDP.Checked)
                ck_TCP.Checked = false;
            else if (ck_UDP.Checked == false)
            {
                if (ck_TCP.Checked == false)
                {
                    ck_UDP.Checked = true;
                    return;
                }
            }

        }

        private void cb_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_Mode.SelectedItem.ToString() == "Modbus")
            {
                Modbus_GroupBox.Visible = true;

                FEnet_GroupBox.Visible = false;
                ck_TCP.Enabled = false;
                ck_UDP.Enabled = false;
                ck_TCP.Checked = true;
            }
            else
            {
                FEnet_GroupBox.Visible = true;
                ck_TCP.Enabled = true;
                ck_UDP.Enabled = true;

                Modbus_GroupBox.Visible = false;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if(sender is TextBox tb)
            {
                if(System.Text.RegularExpressions.Regex.IsMatch(tb.Text, "[^0-9]"))
                {
                    tb.Text = tb.Text.Remove(tb.Text.Length - 1);
                    tb.SelectionStart = tb.Text.Length;
                }
                else
                {
                    if (tb.Name.Contains("Port"))
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(tb.Text))
                    {
                        return;
                    }

                    if(Convert.ToInt32(tb.Text) > 2000)
                    {
                        tb.Text = tb.Text.Remove(tb.Text.Length - 1);
                        tb.SelectionStart = tb.Text.Length;

                        MessageBox.Show("MaxSize is 2000");
                    }
                }
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if(sender is TextBox tb)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    if (tb.Name.Contains("Port"))
                    {
                        tb.Text = "2000";
                        return;
                    }

                    tb.Text = "0";
                }
            }
        }
    }
}
