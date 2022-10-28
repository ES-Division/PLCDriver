
namespace PLCSimulation
{
    partial class SimulForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.cb_memoryArea = new System.Windows.Forms.ComboBox();
            this.bt_End = new System.Windows.Forms.Button();
            this.lb_Mode = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Address = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.one = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.two = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.three = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.four = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.five = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.six = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.seven = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tb_Port = new System.Windows.Forms.TextBox();
            this.tb_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_Start = new System.Windows.Forms.Button();
            this.bt_Setting = new System.Windows.Forms.Button();
            this.lb_State = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 15);
            this.label4.TabIndex = 21;
            this.label4.Text = "MemoryName : ";
            // 
            // cb_memoryArea
            // 
            this.cb_memoryArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_memoryArea.FormattingEnabled = true;
            this.cb_memoryArea.Location = new System.Drawing.Point(110, 56);
            this.cb_memoryArea.Name = "cb_memoryArea";
            this.cb_memoryArea.Size = new System.Drawing.Size(121, 23);
            this.cb_memoryArea.TabIndex = 20;
            this.cb_memoryArea.TabStop = false;
            this.cb_memoryArea.SelectedIndexChanged += new System.EventHandler(this.cb_memoryArea_SelectedIndexChanged);
            // 
            // bt_End
            // 
            this.bt_End.Location = new System.Drawing.Point(716, 19);
            this.bt_End.Name = "bt_End";
            this.bt_End.Size = new System.Drawing.Size(75, 23);
            this.bt_End.TabIndex = 19;
            this.bt_End.TabStop = false;
            this.bt_End.Text = "End";
            this.bt_End.UseVisualStyleBackColor = true;
            this.bt_End.Click += new System.EventHandler(this.bt_End_Click);
            // 
            // lb_Mode
            // 
            this.lb_Mode.AutoSize = true;
            this.lb_Mode.Location = new System.Drawing.Point(474, 23);
            this.lb_Mode.Name = "lb_Mode";
            this.lb_Mode.Size = new System.Drawing.Size(36, 15);
            this.lb_Mode.TabIndex = 18;
            this.lb_Mode.Text = "FEnet";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(429, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 15);
            this.label3.TabIndex = 17;
            this.label3.Text = "Mode : ";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Address,
            this.zero,
            this.one,
            this.two,
            this.three,
            this.four,
            this.five,
            this.six,
            this.seven,
            this.eight,
            this.nine});
            this.dataGridView1.Location = new System.Drawing.Point(9, 85);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(776, 347);
            this.dataGridView1.TabIndex = 16;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            // 
            // Address
            // 
            this.Address.DataPropertyName = "Address";
            this.Address.HeaderText = "Address";
            this.Address.Name = "Address";
            this.Address.ReadOnly = true;
            // 
            // zero
            // 
            this.zero.DataPropertyName = "Zero";
            this.zero.HeaderText = "+0";
            this.zero.Name = "zero";
            // 
            // one
            // 
            this.one.DataPropertyName = "One";
            this.one.HeaderText = "+1";
            this.one.Name = "one";
            // 
            // two
            // 
            this.two.DataPropertyName = "Two";
            this.two.HeaderText = "+2";
            this.two.Name = "two";
            // 
            // three
            // 
            this.three.DataPropertyName = "Three";
            this.three.HeaderText = "+3";
            this.three.Name = "three";
            // 
            // four
            // 
            this.four.DataPropertyName = "Four";
            this.four.HeaderText = "+4";
            this.four.Name = "four";
            // 
            // five
            // 
            this.five.DataPropertyName = "Five";
            this.five.HeaderText = "+5";
            this.five.Name = "five";
            // 
            // six
            // 
            this.six.DataPropertyName = "Six";
            this.six.HeaderText = "+6";
            this.six.Name = "six";
            // 
            // seven
            // 
            this.seven.DataPropertyName = "Seven";
            this.seven.HeaderText = "+7";
            this.seven.Name = "seven";
            // 
            // eight
            // 
            this.eight.DataPropertyName = "Eight";
            this.eight.HeaderText = "+8";
            this.eight.Name = "eight";
            // 
            // nine
            // 
            this.nine.DataPropertyName = "Nine";
            this.nine.HeaderText = "+9";
            this.nine.Name = "nine";
            // 
            // tb_Port
            // 
            this.tb_Port.Enabled = false;
            this.tb_Port.Location = new System.Drawing.Point(303, 21);
            this.tb_Port.Name = "tb_Port";
            this.tb_Port.Size = new System.Drawing.Size(80, 23);
            this.tb_Port.TabIndex = 14;
            // 
            // tb_IP
            // 
            this.tb_IP.Enabled = false;
            this.tb_IP.Location = new System.Drawing.Point(93, 21);
            this.tb_IP.Name = "tb_IP";
            this.tb_IP.Size = new System.Drawing.Size(141, 23);
            this.tb_IP.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 13;
            this.label1.Text = "IP Address";
            // 
            // bt_Start
            // 
            this.bt_Start.Location = new System.Drawing.Point(635, 20);
            this.bt_Start.Name = "bt_Start";
            this.bt_Start.Size = new System.Drawing.Size(75, 23);
            this.bt_Start.TabIndex = 11;
            this.bt_Start.TabStop = false;
            this.bt_Start.Text = "Start";
            this.bt_Start.UseVisualStyleBackColor = true;
            this.bt_Start.Click += new System.EventHandler(this.bt_Start_Click);
            // 
            // bt_Setting
            // 
            this.bt_Setting.Location = new System.Drawing.Point(554, 20);
            this.bt_Setting.Name = "bt_Setting";
            this.bt_Setting.Size = new System.Drawing.Size(75, 23);
            this.bt_Setting.TabIndex = 10;
            this.bt_Setting.TabStop = false;
            this.bt_Setting.Text = "Setting";
            this.bt_Setting.UseVisualStyleBackColor = true;
            this.bt_Setting.Click += new System.EventHandler(this.bt_Setting_Click);
            // 
            // lb_State
            // 
            this.lb_State.AutoSize = true;
            this.lb_State.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lb_State.ForeColor = System.Drawing.Color.Red;
            this.lb_State.Location = new System.Drawing.Point(592, 54);
            this.lb_State.Name = "lb_State";
            this.lb_State.Size = new System.Drawing.Size(36, 15);
            this.lb_State.TabIndex = 22;
            this.lb_State.Text = "Stop";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(554, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 15);
            this.label5.TabIndex = 23;
            this.label5.Text = "State :";
            // 
            // SimulForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lb_State);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cb_memoryArea);
            this.Controls.Add(this.bt_End);
            this.Controls.Add(this.lb_Mode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tb_Port);
            this.Controls.Add(this.tb_IP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_Start);
            this.Controls.Add(this.bt_Setting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SimulForm";
            this.Text = "Form2";
            this.Move += new System.EventHandler(this.SimulForm_Move);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cb_memoryArea;
        private System.Windows.Forms.Button bt_End;
        private System.Windows.Forms.Label lb_Mode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address;
        private System.Windows.Forms.DataGridViewTextBoxColumn zero;
        private System.Windows.Forms.DataGridViewTextBoxColumn one;
        private System.Windows.Forms.DataGridViewTextBoxColumn two;
        private System.Windows.Forms.DataGridViewTextBoxColumn three;
        private System.Windows.Forms.DataGridViewTextBoxColumn four;
        private System.Windows.Forms.DataGridViewTextBoxColumn five;
        private System.Windows.Forms.DataGridViewTextBoxColumn six;
        private System.Windows.Forms.DataGridViewTextBoxColumn seven;
        private System.Windows.Forms.DataGridViewTextBoxColumn eight;
        private System.Windows.Forms.DataGridViewTextBoxColumn nine;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.TextBox tb_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_Start;
        private System.Windows.Forms.Button bt_Setting;
        private System.Windows.Forms.Label lb_State;
        private System.Windows.Forms.Label label5;
    }
}