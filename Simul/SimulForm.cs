using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PLCSimulation
{
    public partial class SimulForm : Form
    {
        Simulator sim;

        #region fenet
        int m_size;
        int d_size;
        int k_size;
        int l_size;
        #endregion

        #region modbus
        int coil_size;
        int input_size;
        int holdingRegister_size;
        int inputRegister_size;
        #endregion

        #region common
        string protocol;

        ushort value;

        string[] fenetMemory = new string[4] { "M", "D", "K", "L" };
        string[] modbusMemory = new string[4] { "Coil", "Input", "HoldingRegister", "InputRegister" };

        SetBind SetDataTableBind;
        BindingList<PLCMemory> bindingList;

        public string Port
        {
            get { return tb_Port.Text; }
            set { tb_Port.Text = value; }
        }
        public string Mode
        {
            get { return lb_Mode.Text; }
            set { lb_Mode.Text = value; }
        }
        public string IP
        {
            get { return tb_IP.Text; }
            set { tb_IP.Text = value; }
        }
        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        public int MSize
        {
            get { return m_size; }
            set { m_size = value; }
        }
        public int DSize
        {
            get { return d_size; }
            set { d_size = value; }
        }
        public int KSize
        {
            get { return k_size; }
            set { k_size = value; }
        }
        public int LSize
        {
            get { return l_size; }
            set { l_size = value; }
        }
        public int CoilSize
        {
            get { return coil_size; }
            set { coil_size = value; }
        }
        public int InputSize
        {
            get { return input_size; }
            set { input_size = value; }
        }
        public int HoldingRegisterSize
        {
            get { return holdingRegister_size; }
            set { holdingRegister_size = value; }
        }
        public int InputRegisterSize
        {
            get { return inputRegister_size; }
            set { inputRegister_size = value; }
        }

        public bool InitMode
        {
            set
            {
                if (value) // init
                {
                    bt_Setting.Enabled = true;
                    bt_Start.Enabled = false;
                    bt_End.Enabled = false;
                }
                else // setting | stop
                {
                    bt_Setting.Enabled = true;
                    bt_Start.Enabled = true;
                    bt_End.Enabled = false;
                }
            }
        }
        #endregion

        #region text
        public String btRandomNoRunningText
        {
            get{
                return "Random Start";
            }
        }

        public String btRandomRunningText
        {
            get
            {
                return "Random Stop";
            }
        }

        #endregion

        public SimulForm()
        {
            //(MdiParent as Form1).Test();
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
            bt_Setting.Enabled = true;
            bt_Start.Enabled = false;
            bt_End.Enabled = false;

            this.Text = "Simulator";
            this.bt_random.Text = btRandomNoRunningText;
        }

        private void bt_Setting_Click(object sender, EventArgs e)
        {
            int index = Convert.ToInt32(this.Text.Split('_')[1]);
            Setting setting = new Setting(tb_IP.Text, tb_Port.Text, lb_Mode.Text, index);
            setting.PortSettingFunc = (MdiParent as Form1).PortCheck;


            if (setting.ShowDialog() == DialogResult.OK)
            {
                tb_IP.Text = setting.IP;
                tb_Port.Text = setting.Port;
                lb_Mode.Text = setting.Mode;
                protocol = setting.Protocol;

                switch (setting.ProtocolMode)
                {
                    case 0:
                        m_size = Convert.ToInt32(setting.MSize);
                        d_size = Convert.ToInt32(setting.DSize);
                        k_size = Convert.ToInt32(setting.KSize);
                        l_size = Convert.ToInt32(setting.LSize);
                        break;
                    case 1:
                        coil_size = Convert.ToInt32(setting.CoilSize);
                        input_size = Convert.ToInt32(setting.InputSize);
                        holdingRegister_size = Convert.ToInt32(setting.HoldingRegisterSize);
                        inputRegister_size = Convert.ToInt32(setting.InputRegisterSize);
                        break;
                }

                bt_Setting.Enabled = true;
                bt_Start.Enabled = true;
                bt_End.Enabled = false;
            }
        }

        private void bt_Start_Click(object sender, EventArgs e)
         {
            try
            {
                switch (Mode)
                {
                    case "FEnet":
                        sim = new Simulator(tb_IP.Text, tb_Port.Text, protocol, lb_Mode.Text, m_size, d_size, k_size, l_size);
                        SetDataTableBind += sim.GetMemoryMap;

                        cb_memoryArea.Items.Clear();
                        cb_memoryArea.Items.AddRange(fenetMemory);
                        break;
                    case "Modbus":
                        sim = new Simulator(tb_IP.Text, tb_Port.Text, protocol, lb_Mode.Text, coil_size, input_size, holdingRegister_size, inputRegister_size);
                        SetDataTableBind += sim.GetMemoryMap;

                        cb_memoryArea.Items.Clear();
                        cb_memoryArea.Items.AddRange(modbusMemory);
                        break;
                }
            }
            catch (System.Net.Sockets.SocketException socketEx)
            {
                switch (socketEx.SocketErrorCode)
                {
                    case System.Net.Sockets.SocketError.AddressAlreadyInUse:
                        MessageBox.Show("사용 중인 포트번호 입니다.", $"{socketEx.SocketErrorCode}");
                        break;
                    default:
                        MessageBox.Show("설정 내용에 실행을 실패하였습니다.", $"{socketEx.SocketErrorCode}");
                        break;
                }

                System.Diagnostics.Trace.WriteLine(socketEx.Message);
                return;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            try
            {
                if (sim.Start())
                {
                    cb_memoryArea.SelectedIndex = 0;
                    SetDataGridViewBind(cb_memoryArea.SelectedIndex);
                    MessageBox.Show("PLC Simulator Start!");

                    lb_State.Text = "Start";
                    lb_State.ForeColor = Color.Blue;

                    bt_Start.Enabled = false;
                    bt_Setting.Enabled = false;
                    bt_End.Enabled = true;
                }
                else
                    MessageBox.Show("PLC Simulator Start Fail!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bt_End_Click(object sender, EventArgs e)
        {
            if (SimulatorClose())
            {
                // 
                StopRandomValue();

                lb_State.Text = "Stop";
                lb_State.ForeColor = Color.Red;

                bt_End.Enabled = false;
                bt_Start.Enabled = true;
                bt_Setting.Enabled = true;

                cb_memoryArea.Items.Clear();
                cb_memoryArea.Refresh();

                dataGridView1.DataSource = null;
                sim = null;
            }
        }

        public bool SimulatorClose()
        {
            if (sim == null)
                return true;

            return sim.Stop();
        }

        private void SetDataGridViewBind(int index)
        {
            if (index < 0)
            {
                return;
            }

            var dt = SetDataTableBind?.Invoke(index);
            bindingList = new(dt);
            dataGridView1.DataSource = bindingList;
        }

        private void cb_memoryArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dt = SetDataTableBind?.Invoke((sender as ComboBox).SelectedIndex);
            bindingList = new(dt);
            dataGridView1.DataSource = bindingList;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Input range : 0~65535");

            return;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var datagridview = (sender as DataGridView);
            string name = datagridview.CurrentCell.OwningColumn.Name;

            if (name != "Address")
            {
                e.Control.KeyPress -= KeyPress_NumericCheck;
                e.Control.KeyPress += KeyPress_NumericCheck;
            }
        }

        private void KeyPress_NumericCheck(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
            else if(e.KeyChar != Convert.ToChar(Keys.Back))
            {
                var memoryArea = (cb_memoryArea.SelectedItem as string);

                if (memoryArea == "Coil" || memoryArea == "Input")
                {
                    var num = Convert.ToInt32(e.KeyChar.ToString());

                    if (num < 0 || num >= 2)
                        e.Handled = true;
                    else
                    {
                        var textBox = (sender as DataGridViewTextBoxEditingControl);
                        textBox.Text = "";
                    }
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            value = Convert.ToUInt16((sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var memoryArea = (cb_memoryArea.SelectedItem as string);
            if (lb_Mode.Text == "Modbus" && (memoryArea == "Coil" || memoryArea == "Input"))
            {
                var setValue = Convert.ToUInt16((sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value);

                if (!(setValue == 0 || setValue == 1))
                {
                    MessageBox.Show("Bit Value : 0 or 1");
                    (sender as DataGridView)[e.ColumnIndex, e.RowIndex].Value = value;
                }
            }
        }

        private void SimulForm_Move(object sender, EventArgs e)
        { // mdi 폼 밖으로 이동 방지
            if(this.Left < 0)
                this.Left = 0;
            else if(this.Left > this.MdiParent.ClientRectangle.Width - this.Width)
                this.Left = this.MdiParent.ClientRectangle.Width - this.Width;
            
            if (this.Top < 0)
                this.Top = 0;
            else if (this.Top > this.MdiParent.ClientRectangle.Height - this.Height)
                this.Top = this.MdiParent.ClientRectangle.Height - this.Height;
        }

        /*
        * Get Memory Mapping
        * map_M : 0
        * map_D : 1
        * map_K : 2
        * map_L : 3
        * 
        * Mapping
        * zero~nine
       */

        // 매개변수로 들어온 PlcMemory 리스트를 랜덤한 값으로 채우는 함수 
        static private void ApplyPlcMemoryRandom(List<PLCMemory> plcMemoryList)
        {
            plcMemoryList.ForEach(plcMemory =>
            {
                Random random = new Random();
                plcMemory.Zero = (ushort)random.Next(65535);
                plcMemory.One = (ushort)random.Next(65535);
                plcMemory.Two = (ushort)random.Next(65535);
                plcMemory.Three = (ushort)random.Next(65535);
                plcMemory.Four = (ushort)random.Next(65535);
                plcMemory.Five = (ushort)random.Next(65535);
                plcMemory.Six = (ushort)random.Next(65535);
                plcMemory.Seven = (ushort)random.Next(65535);
                plcMemory.Eight = (ushort)random.Next(65535);
                plcMemory.Nine = (ushort)random.Next(65535);
            });
        }

        // 시뮬레이터의 메모리 맵을 랜덤한 값으로 채우는 함수
        // 시뮬레이터가 생성된 이후에 호출되어야 합니다.
        private void ApplySimulatorPlcMemoryMapRandom()
        {
            if(sim == null)
            {
                // 시뮬레이터가 시작되지 않은 경우에는 실행하지 않는다.
                return;
            }

            // 메모리 랜덤
            List<PLCMemory> PLCMemories;
            // 0
            PLCMemories = sim.GetMemoryMap(0);
            ApplyPlcMemoryRandom(PLCMemories);

            // 1
            PLCMemories = sim.GetMemoryMap(1);
            ApplyPlcMemoryRandom(PLCMemories);

            // 2
            PLCMemories = sim.GetMemoryMap(2);
            ApplyPlcMemoryRandom(PLCMemories);

            // 3
            PLCMemories = sim.GetMemoryMap(3);
            ApplyPlcMemoryRandom(PLCMemories);
        }

        // 랜덤 기능의 활성화 유무를 의미하는 변수
        // 랜덤 기능이 활성화된 경우 : true
        // 랜덤 기능이 비활성화된 경우 : false
        bool randomRunning = false;

        // 시뮬레이터의 메모리 맵을 랜덤한 값으로 채우는 동기 함수
        private void ApplySimulatorPlcMemoryMapRandomSyncRepeat()
        {
            while (randomRunning)
            {
                // 메모리 맵을 랜덤으로 채우기
                ApplySimulatorPlcMemoryMapRandom();
                // 1초간 대기
                Thread.Sleep(1000);
            }
        }

        // 시뮬레이터의 메모리 맵을 랜덤한 값으로 채우는 비동기 함수
        async private void ApplySimulatorPlcMemoryMapRandomAsyncRepeat()
        {
            while (randomRunning)
            {
                // 메모리 맵을 랜덤으로 채우기
                ApplySimulatorPlcMemoryMapRandom();
                // 1초간 양보
                await Task.Delay(1000);
            }
        }

        // 랜덤 기능을 활성화하는 함수
        void StartRandomValue()
        {
            // 랜덤 기능 활성화
            if (randomRunning == false)
            {
                bt_random.Text = btRandomRunningText;
                randomRunning = true;
                ApplySimulatorPlcMemoryMapRandomAsyncRepeat();
            }
            else
            {
                // 에러 메시지
                throw new ArgumentException("랜덤 기능이 활성화 된 상태에서 다시 활성화 시킬 수 없습니다.");
            }
        }

        // 랜덤 기능을 비활성화하는 함수
        void StopRandomValue()
        {
            if (randomRunning == true)
            {
                // 랜덤 기능 비활성화
                bt_random.Text = btRandomNoRunningText;
                randomRunning = false;
            }
        }

        // 랜덤 시작/정지 버튼의 클릭 이벤트를 처리하는 함수
        private void bt_random_click(object sender, EventArgs e)
        {
            // 랜덤 기능이 활성화되지 않은 경우
            if (bt_random.Text == btRandomNoRunningText)
            {
                StartRandomValue();
            }
            // 랜덤 기능이 활성화 된 경우
            else if (bt_random.Text == btRandomRunningText) {
                StopRandomValue();   
            }
            else
            {
                // 이외의 경우
                throw new ArgumentException("해당되는 상태가 없습니다.");
            }
        }

        // 한번만 랜덤으로 값을 채우는 버튼의 이벤트를 처리하는 함수
        private void bt_random_once_click(object sender, EventArgs e)
        {
            // 메모리 맵을 랜덤으로 채우기
            ApplySimulatorPlcMemoryMapRandom();
        }
    }
}
