
namespace PLCSimulation
{
    partial class Setting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tb_IP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.b_OK = new System.Windows.Forms.Button();
            this.b_Cancel = new System.Windows.Forms.Button();
            this.tb_Port = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_Mode = new System.Windows.Forms.ComboBox();
            this.Modbus_GroupBox = new System.Windows.Forms.GroupBox();
            this.tb_InputRegister = new System.Windows.Forms.TextBox();
            this.tb_Input = new System.Windows.Forms.TextBox();
            this.tb_HoldingRegister = new System.Windows.Forms.TextBox();
            this.tb_Coil = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.FEnet_Protocol = new System.Windows.Forms.GroupBox();
            this.ck_UDP = new System.Windows.Forms.CheckBox();
            this.ck_TCP = new System.Windows.Forms.CheckBox();
            this.FEnet_GroupBox = new System.Windows.Forms.GroupBox();
            this.tb_LSize = new System.Windows.Forms.TextBox();
            this.tb_DSize = new System.Windows.Forms.TextBox();
            this.tb_KSize = new System.Windows.Forms.TextBox();
            this.tb_MSize = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.Modbus_GroupBox.SuspendLayout();
            this.FEnet_Protocol.SuspendLayout();
            this.FEnet_GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_IP
            // 
            this.tb_IP.Enabled = false;
            this.tb_IP.Location = new System.Drawing.Point(91, 12);
            this.tb_IP.Name = "tb_IP";
            this.tb_IP.Size = new System.Drawing.Size(129, 23);
            this.tb_IP.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // b_OK
            // 
            this.b_OK.Location = new System.Drawing.Point(248, 26);
            this.b_OK.Name = "b_OK";
            this.b_OK.Size = new System.Drawing.Size(75, 23);
            this.b_OK.TabIndex = 3;
            this.b_OK.TabStop = false;
            this.b_OK.Text = "OK";
            this.b_OK.UseVisualStyleBackColor = true;
            this.b_OK.Click += new System.EventHandler(this.b_OK_Click);
            // 
            // b_Cancel
            // 
            this.b_Cancel.Location = new System.Drawing.Point(248, 66);
            this.b_Cancel.Name = "b_Cancel";
            this.b_Cancel.Size = new System.Drawing.Size(75, 23);
            this.b_Cancel.TabIndex = 3;
            this.b_Cancel.TabStop = false;
            this.b_Cancel.Text = "Cancel";
            this.b_Cancel.UseVisualStyleBackColor = true;
            this.b_Cancel.Click += new System.EventHandler(this.b_Cancel_Click);
            // 
            // tb_Port
            // 
            this.tb_Port.Location = new System.Drawing.Point(91, 48);
            this.tb_Port.Name = "tb_Port";
            this.tb_Port.Size = new System.Drawing.Size(129, 23);
            this.tb_Port.TabIndex = 1;
            this.tb_Port.Tag = "";
            this.tb_Port.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_Port.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mode";
            // 
            // cb_Mode
            // 
            this.cb_Mode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_Mode.FormattingEnabled = true;
            this.cb_Mode.Items.AddRange(new object[] {
            "FEnet",
            "Modbus"});
            this.cb_Mode.Location = new System.Drawing.Point(91, 90);
            this.cb_Mode.Name = "cb_Mode";
            this.cb_Mode.Size = new System.Drawing.Size(129, 23);
            this.cb_Mode.TabIndex = 4;
            this.cb_Mode.TabStop = false;
            this.cb_Mode.SelectedIndexChanged += new System.EventHandler(this.cb_Mode_SelectedIndexChanged);
            // 
            // Modbus_GroupBox
            // 
            this.Modbus_GroupBox.Controls.Add(this.tb_InputRegister);
            this.Modbus_GroupBox.Controls.Add(this.tb_Input);
            this.Modbus_GroupBox.Controls.Add(this.tb_HoldingRegister);
            this.Modbus_GroupBox.Controls.Add(this.tb_Coil);
            this.Modbus_GroupBox.Controls.Add(this.label7);
            this.Modbus_GroupBox.Controls.Add(this.label5);
            this.Modbus_GroupBox.Controls.Add(this.label6);
            this.Modbus_GroupBox.Controls.Add(this.label4);
            this.Modbus_GroupBox.Location = new System.Drawing.Point(12, 174);
            this.Modbus_GroupBox.Name = "Modbus_GroupBox";
            this.Modbus_GroupBox.Size = new System.Drawing.Size(323, 95);
            this.Modbus_GroupBox.TabIndex = 5;
            this.Modbus_GroupBox.TabStop = false;
            this.Modbus_GroupBox.Text = "I/O Memory Size";
            // 
            // tb_InputRegister
            // 
            this.tb_InputRegister.Location = new System.Drawing.Point(241, 58);
            this.tb_InputRegister.Name = "tb_InputRegister";
            this.tb_InputRegister.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_InputRegister.Size = new System.Drawing.Size(70, 23);
            this.tb_InputRegister.TabIndex = 6;
            this.tb_InputRegister.Text = "0";
            this.tb_InputRegister.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_InputRegister.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_Input
            // 
            this.tb_Input.Location = new System.Drawing.Point(59, 57);
            this.tb_Input.Name = "tb_Input";
            this.tb_Input.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_Input.Size = new System.Drawing.Size(70, 23);
            this.tb_Input.TabIndex = 5;
            this.tb_Input.Text = "0";
            this.tb_Input.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_Input.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_HoldingRegister
            // 
            this.tb_HoldingRegister.Location = new System.Drawing.Point(241, 24);
            this.tb_HoldingRegister.Name = "tb_HoldingRegister";
            this.tb_HoldingRegister.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_HoldingRegister.Size = new System.Drawing.Size(70, 23);
            this.tb_HoldingRegister.TabIndex = 4;
            this.tb_HoldingRegister.Text = "0";
            this.tb_HoldingRegister.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_HoldingRegister.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_Coil
            // 
            this.tb_Coil.Location = new System.Drawing.Point(59, 24);
            this.tb_Coil.Name = "tb_Coil";
            this.tb_Coil.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_Coil.Size = new System.Drawing.Size(70, 23);
            this.tb_Coil.TabIndex = 3;
            this.tb_Coil.Text = "0";
            this.tb_Coil.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_Coil.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "InputRegister";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(143, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "HoldingRegister";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Input";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Coil";
            // 
            // FEnet_Protocol
            // 
            this.FEnet_Protocol.Controls.Add(this.ck_UDP);
            this.FEnet_Protocol.Controls.Add(this.ck_TCP);
            this.FEnet_Protocol.Location = new System.Drawing.Point(46, 121);
            this.FEnet_Protocol.Name = "FEnet_Protocol";
            this.FEnet_Protocol.Size = new System.Drawing.Size(258, 47);
            this.FEnet_Protocol.TabIndex = 2;
            this.FEnet_Protocol.TabStop = false;
            this.FEnet_Protocol.Text = "TCP/UDP";
            // 
            // ck_UDP
            // 
            this.ck_UDP.AutoSize = true;
            this.ck_UDP.Location = new System.Drawing.Point(147, 22);
            this.ck_UDP.Name = "ck_UDP";
            this.ck_UDP.Size = new System.Drawing.Size(50, 19);
            this.ck_UDP.TabIndex = 1;
            this.ck_UDP.Text = "UDP";
            this.ck_UDP.UseVisualStyleBackColor = true;
            this.ck_UDP.CheckedChanged += new System.EventHandler(this.ck_UDP_CheckedChanged);
            // 
            // ck_TCP
            // 
            this.ck_TCP.AutoSize = true;
            this.ck_TCP.Checked = true;
            this.ck_TCP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ck_TCP.Location = new System.Drawing.Point(25, 22);
            this.ck_TCP.Name = "ck_TCP";
            this.ck_TCP.Size = new System.Drawing.Size(47, 19);
            this.ck_TCP.TabIndex = 0;
            this.ck_TCP.Text = "TCP";
            this.ck_TCP.UseVisualStyleBackColor = true;
            this.ck_TCP.CheckedChanged += new System.EventHandler(this.ck_TCP_CheckedChanged);
            // 
            // FEnet_GroupBox
            // 
            this.FEnet_GroupBox.Controls.Add(this.tb_LSize);
            this.FEnet_GroupBox.Controls.Add(this.tb_DSize);
            this.FEnet_GroupBox.Controls.Add(this.tb_KSize);
            this.FEnet_GroupBox.Controls.Add(this.tb_MSize);
            this.FEnet_GroupBox.Controls.Add(this.label8);
            this.FEnet_GroupBox.Controls.Add(this.label9);
            this.FEnet_GroupBox.Controls.Add(this.label10);
            this.FEnet_GroupBox.Controls.Add(this.label11);
            this.FEnet_GroupBox.Location = new System.Drawing.Point(12, 174);
            this.FEnet_GroupBox.Name = "FEnet_GroupBox";
            this.FEnet_GroupBox.Size = new System.Drawing.Size(323, 95);
            this.FEnet_GroupBox.TabIndex = 5;
            this.FEnet_GroupBox.TabStop = false;
            this.FEnet_GroupBox.Text = "I/O Memory Size";
            // 
            // tb_LSize
            // 
            this.tb_LSize.Location = new System.Drawing.Point(222, 57);
            this.tb_LSize.Name = "tb_LSize";
            this.tb_LSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_LSize.Size = new System.Drawing.Size(70, 23);
            this.tb_LSize.TabIndex = 6;
            this.tb_LSize.Text = "0";
            this.tb_LSize.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_LSize.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_DSize
            // 
            this.tb_DSize.Location = new System.Drawing.Point(59, 57);
            this.tb_DSize.Name = "tb_DSize";
            this.tb_DSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_DSize.Size = new System.Drawing.Size(70, 23);
            this.tb_DSize.TabIndex = 5;
            this.tb_DSize.Text = "0";
            this.tb_DSize.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_DSize.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_KSize
            // 
            this.tb_KSize.Location = new System.Drawing.Point(222, 25);
            this.tb_KSize.Name = "tb_KSize";
            this.tb_KSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_KSize.Size = new System.Drawing.Size(70, 23);
            this.tb_KSize.TabIndex = 4;
            this.tb_KSize.Text = "0";
            this.tb_KSize.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_KSize.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // tb_MSize
            // 
            this.tb_MSize.Location = new System.Drawing.Point(59, 25);
            this.tb_MSize.Name = "tb_MSize";
            this.tb_MSize.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tb_MSize.Size = new System.Drawing.Size(70, 23);
            this.tb_MSize.TabIndex = 3;
            this.tb_MSize.Text = "0";
            this.tb_MSize.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.tb_MSize.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(195, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "L";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(194, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(14, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "K";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(25, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 15);
            this.label10.TabIndex = 2;
            this.label10.Text = "D";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(25, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(18, 15);
            this.label11.TabIndex = 2;
            this.label11.Text = "M";
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 308);
            this.Controls.Add(this.FEnet_Protocol);
            this.Controls.Add(this.FEnet_GroupBox);
            this.Controls.Add(this.Modbus_GroupBox);
            this.Controls.Add(this.cb_Mode);
            this.Controls.Add(this.b_Cancel);
            this.Controls.Add(this.b_OK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tb_Port);
            this.Controls.Add(this.tb_IP);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Setting";
            this.Text = " ";
            this.Modbus_GroupBox.ResumeLayout(false);
            this.Modbus_GroupBox.PerformLayout();
            this.FEnet_Protocol.ResumeLayout(false);
            this.FEnet_Protocol.PerformLayout();
            this.FEnet_GroupBox.ResumeLayout(false);
            this.FEnet_GroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_IP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button b_OK;
        private System.Windows.Forms.Button b_Cancel;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_Mode;
        private System.Windows.Forms.GroupBox Modbus_GroupBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox FEnet_Protocol;
        private System.Windows.Forms.CheckBox ck_UDP;
        private System.Windows.Forms.CheckBox ck_TCP;
        private System.Windows.Forms.GroupBox FEnet_GroupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tb_InputRegister;
        private System.Windows.Forms.TextBox tb_Input;
        private System.Windows.Forms.TextBox tb_HoldingRegister;
        private System.Windows.Forms.TextBox tb_Coil;
        private System.Windows.Forms.TextBox tb_LSize;
        private System.Windows.Forms.TextBox tb_DSize;
        private System.Windows.Forms.TextBox tb_KSize;
        private System.Windows.Forms.TextBox tb_MSize;
    }
}