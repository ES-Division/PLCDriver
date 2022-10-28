using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace PLCSimulation
{
    delegate byte[] ListenerResponseEvent(byte[] message);
    delegate List<PLCMemory> SetBind(int index);

    class Simulator // PLC Memory
    {
        readonly Dictionary<string, ProtocolType> socketType = new Dictionary<string, ProtocolType>()
        {
            {"TCP", ProtocolType.Tcp },
            {"UDP", ProtocolType.Udp }
        };

        ListenerSocket plcServer;

        FEnet fenet;
        Modbus modbus;

        public Simulator(string ipAddr, string port, string protocol, string mode, int m_coil_size, int d_input_size, int k_holdingregister_size, int l_inputregister_size)
        {
            //plcServer = new ListenerSocket(ipAddr, Convert.ToInt32(port), socketType[protocol], mode, m_coil_size, d_input_size, k_holdingregister_size, l_inputregister_size);
            plcServer = new ListenerSocket(ipAddr, Convert.ToInt32(port), socketType[protocol]);

            switch (mode)
            {
                case PLCProtocolType.FENET:
                    fenet = new FEnet(m_coil_size, d_input_size, k_holdingregister_size, l_inputregister_size);
                    plcServer.ResponseEvent += fenet.ResponsePacket;
                    break;
                case PLCProtocolType.MODBUS:
                    modbus = new Modbus(m_coil_size, d_input_size, k_holdingregister_size, l_inputregister_size);
                    plcServer.ResponseEvent += modbus.ResponsePacket;
                    break;
                default:
                    throw new Exception("not define PLC Protocol");
            }
        }

        public bool Start()
        {
            if (plcServer.IsRunning == false)
                plcServer.IsRunning = true;
            else
            {
                throw new Exception("simulrator already running, stop and restart");
            }

            return true;
        }

        public bool Stop()
        {
            if (plcServer.IsRunning == true)
            {
                plcServer.IsRunning = false;
            }

            return true;
        }

        public List<PLCMemory> GetMemoryMap(int index)
        {
            if(fenet != null) // 조건식은 수정이 필요함
            {
                return fenet.GetMemoryAddressMap(index);
            }

            return modbus.GetMemoryAddressMap(index);
        }
    }
}
