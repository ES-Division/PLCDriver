using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLCSimulation
{
    class FEnet
    {
        enum DataType
        {
            BIT = 0x00,
            BYTE = 0x01,
            WORD = 0x02,
            DOUBLEWORD = 0x03,
            LONGWORD = 0x04,
            CONTINUITY = 0x14
        }

        enum MemoryArea
        {
            M,
            D,
            K,
            L
        }

        const int HEADER_LENGTH = 20;
        const int BODY_LENGTH = 21;
        const int RESPONSE_BODY_LENGTH = 10;

        bool isRead = false;
        bool isBit = false;
        byte dataType;
        string memory;
        string memoryType;
        string address;
        int count;
        int blockCount;
        byte[] writeBytes;

        // Memory Address Map [M/D/K/L]
        List<PLCMemory> map_M;
        List<PLCMemory> map_D;
        List<PLCMemory> map_K;
        List<PLCMemory> map_L;

        int m_size;
        int d_size;
        int k_size;
        int l_size;

        public FEnet(int msize = 0, int dsize = 0, int ksize = 0, int lsize = 0)
        {
            m_size = msize;
            d_size = dsize;
            k_size = ksize;
            l_size = lsize;

            if (msize > 0)
            {
                map_M = new List<PLCMemory>() { new PLCMemory() { Address = "0" } };
                int count = msize / 10 + (msize % 10 > 0 ? 1 : 0);

                for (int i = 1; i < count; i++)
                {
                    map_M.Add(new PLCMemory() { Address = (i * 10).ToString() });
                }
            }
            else if(msize == 0)
            {
                map_M = new List<PLCMemory>();
            }

            if (dsize > 0)
            {
                map_D = new List<PLCMemory>() { new PLCMemory() { Address = "0" } };
                int count = dsize / 10 + (dsize % 10 > 0 ? 1 : 0);

                for (int i = 1; i < count; i++)
                {
                    map_D.Add(new PLCMemory() { Address = (i * 10).ToString() });
                }
            }
            else if (dsize == 0)
            {
                map_D = new List<PLCMemory>();
            }

            if (ksize > 0)
            {
                map_K = new List<PLCMemory>() { new PLCMemory() { Address = "0" } };
                int count = ksize / 10 + (ksize % 10 > 0 ? 1 : 0);

                for (int i = 1; i < count; i++)
                {
                    map_K.Add(new PLCMemory() { Address = (i * 10).ToString() });
                }
            }
            else if (ksize == 0)
            {
                map_K = new List<PLCMemory>();
            }

            if (lsize > 0)
            {
                map_L = new List<PLCMemory>() { new PLCMemory() { Address = "0" } };
                int count = lsize / 10 + (lsize % 10 > 0 ? 1 : 0);

                for (int i = 1; i < count; i++)
                {
                    map_L.Add(new PLCMemory() { Address = (i * 10).ToString() });
                }
            }
            else if (lsize == 0)
            {
                map_L = new List<PLCMemory>();
            }
        }

        public byte[] ResponsePacket(byte[] message)
        {
            bool isError = false;
            byte[] dataBytes = null;
            try
            {
                DesPacket(message);

                if (count > 1 && dataType != 0x14) // 복수개의 요청 시 연속 요청으로 와야함
                    isError = true;
                else if (!Enum.IsDefined(typeof(MemoryArea), memory)) // 정의되지 않은 메모리 영역
                    isError = true;
                else if (Enum.GetName(typeof(DataType), dataType) == null) // 정의되지 않은 요청 데이터 타입
                    isError = true;
                else if (isError == false) // 주소 범위 확인
                {
                    switch (Enum.Parse(typeof(MemoryArea), memory))
                    {
                        case MemoryArea.M:
                            if (isBit)
                            {
                                if (Convert.ToInt32(address, 16) + count > m_size * 16) // 1size => 2byte
                                    isError = true;
                            }
                            else if (Convert.ToInt32(address) + count > m_size * 2) // 1 size => 2byte | address,count -> 1byte
                                isError = true;
                            break;
                        case MemoryArea.D:
                            if (isBit)
                            {
                                if (Convert.ToInt32(address, 16) + count > d_size * 16)
                                    isError = true;
                            }
                            else if (Convert.ToInt32(address) + count > d_size * 2)
                                isError = true;
                            break;
                        case MemoryArea.K:
                            if (isBit)
                            {
                                if (Convert.ToInt32(address, 16) + count > k_size * 16)
                                    isError = true;
                            }
                            else if (Convert.ToInt32(address) + count > k_size * 2)
                                isError = true;
                            break;
                        case MemoryArea.L:
                            if (isBit)
                            {
                                if (Convert.ToInt32(address, 16) + count > l_size * 16)
                                    isError = true;
                            }
                            else if (Convert.ToInt32(address) + count > l_size * 2)
                                isError = true;
                            break;
                    }
                }

                if (isError == false)
                    DataProcessing(ref dataBytes);

                return MakePacket(isError, dataBytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return MakePacket(true, dataBytes);
            }
        }

        private void DataProcessing(ref byte[] dataBytes) // address = startAddr , count = size
        {
            List<PLCMemory> memoryMap = new();

            switch (Enum.Parse(typeof(MemoryArea), memory))
            {
                case MemoryArea.M:
                    memoryMap = map_M;
                    break;
                case MemoryArea.D:
                    memoryMap = map_D;
                    break;
                case MemoryArea.K:
                    memoryMap = map_K;
                    break;
                case MemoryArea.L:
                    memoryMap = map_L;
                    break;
            }

            if (isRead)
            {
                dataBytes = new byte[count];

                if (isBit)
                {
                    int lstIndex = Convert.ToInt32(address, 16) / 160;
                    int memoryIndex = Convert.ToInt32(address, 16) % 160;
                    int bitIndex = memoryIndex % 16;

                    var tempByte = BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex / 16));
                    BitArray bitArr = new BitArray(tempByte);
                    var bits = bitArr.Cast<bool>().Select(bit => bit ? (byte)1 : (byte)0).ToArray();

                    dataBytes[0] = bits[bitIndex];
                }
                else
                {
                    int lstIndex = Convert.ToInt32(address) / 20; // => Byte ? %M'B'0001 ?
                    int memoryIndex = Convert.ToInt32(address) % 20;

                    for (int i = 0; i < count; i++)
                    {
                        dataBytes[i] = BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex / 2))[memoryIndex % 2];

                        memoryIndex++;

                        if (memoryIndex / 2 > 9)
                        {
                            lstIndex += 1;
                            memoryIndex = 0;
                        }
                    }
                }
            }
            else
            {
                if (isBit)
                {
                    int lstIndex = Convert.ToInt32(address, 16) / 160;
                    int memoryIndex = Convert.ToInt32(address, 16) % 160;
                    int bitIndex = memoryIndex % 16;

                    // get
                    var tempByte = BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex / 16));
                    BitArray bitArr = new BitArray(tempByte);
                    var bits = bitArr.Cast<bool>().Select(bit => bit ? (byte)1 : (byte)0).ToArray();

                    // data change
                    bits[bitIndex] = writeBytes[0];

                    // set
                    var a = bits.Cast<byte>().Select(bit => bit == 0x01? true : false).ToArray();
                    bitArr = new BitArray(a);
                    byte[] setBytes = new byte[2];
                    bitArr.CopyTo(setBytes, 0);
                    memoryMap[lstIndex].SetMemoryData(memoryIndex / 16, BitConverter.ToUInt16(setBytes));
                }
                else
                {
                    int lstIndex = Convert.ToInt32(address) / 20; // => Byte ? %M'B'0001 ?
                    int memoryIndex = Convert.ToInt32(address) % 20;

                    for (int i = 0; i < count; i++)
                    {
                        var data = BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex / 2)); // get
                        data[memoryIndex % 2] = writeBytes[i]; // data change
                        memoryMap[lstIndex].SetMemoryData(memoryIndex / 2, BitConverter.ToUInt16(data)); // set

                        memoryIndex++;

                        if (memoryIndex / 2 > 9)
                        {
                            lstIndex += 1;
                            memoryIndex = 0;
                        }
                    }
                }
            }
        }

        private void DesPacket(byte[] bytes) // reqPacket
        {
            var lstPacket = bytes.ToList();
            byte[] headerBytes = lstPacket.GetRange(0,HEADER_LENGTH).ToArray();

            var bodyLength = BitConverter.ToInt16(headerBytes.ToList().GetRange(16, 2).ToArray());
            var bodyBytes = lstPacket.GetRange(HEADER_LENGTH, bodyLength);

            if (bodyBytes[0] == 0x54)
                isRead = true;
            else
                isRead = false;

            if (bodyBytes[2] == 0x00)
                isBit = true;
            else
            {
                isBit = false;
                dataType = bodyBytes[2];
            }

            blockCount = BitConverter.ToInt16(bodyBytes.GetRange(6, 2).ToArray());

            var memoryLength = BitConverter.ToInt16(bodyBytes.GetRange(8,2).ToArray());

            memory = Encoding.ASCII.GetString(bodyBytes.GetRange(10, 3).ToArray()).Substring(1,1);
            memoryType = Encoding.ASCII.GetString(bodyBytes.GetRange(10, 3).ToArray()).Substring(2, 1);

            address = Encoding.ASCII.GetString(bodyBytes.GetRange(13, memoryLength - 3).ToArray());

            if (isBit && isRead)
            {
                count = 1;
            }
            else
            {
                var countBytes = bodyBytes.GetRange(10 + memoryLength, 2).ToArray();
                count = BitConverter.ToInt16(countBytes);
            }

            if (isRead == false)
            {
                writeBytes = new byte[count];
                Buffer.BlockCopy(bodyBytes.ToArray(), BODY_LENGTH, writeBytes, 0, count);
            }
        }

        private byte[] MakePacket(bool isError, byte[] dataByte = null) // responsePacket
        {
            byte[] headerBytes = new byte[HEADER_LENGTH];
            byte[] bodyBytes = new byte[RESPONSE_BODY_LENGTH];

            #region Packet Body
            // 명령어
            if (isRead)
            {
                bodyBytes = new byte[RESPONSE_BODY_LENGTH + count + 2];
                bodyBytes[0] = 0x54;
            }
            else
            {
                bodyBytes[0] = 0x58;
            }
            bodyBytes[1] = 0x00;

            // 데이터타입
            bodyBytes[2] = dataType; // [연속:14 비트:00 바이트:01 워드:02 더블워드:03 롱워드:04 , 응답은 받은대로 보내주면 된다.]
            bodyBytes[3] = 0x00;

            // 예약 영역
            bodyBytes[4] = 0x00;
            bodyBytes[5] = 0x00;

            // 에러 상태 , 블록 수 or 에러코드
            if (isError)
            {
                bodyBytes[6] = 0xff;
                bodyBytes[7] = 0xff;

                bodyBytes[8] = 0x5b;
                bodyBytes[9] = 0x00;
            }
            else
            {
                bodyBytes[6] = 0x00;
                bodyBytes[7] = 0x00;

                bodyBytes[8] = BitConverter.GetBytes(blockCount)[0];
                bodyBytes[9] = BitConverter.GetBytes(blockCount)[1];

                if(isRead == true)
                {
                    // 데이터 개수
                    bodyBytes[10] = BitConverter.GetBytes(count)[0];
                    bodyBytes[11] = BitConverter.GetBytes(count)[1];

                    // 데이터
                    Buffer.BlockCopy(dataByte, 0, bodyBytes, 12, count);
                }
            }
            #endregion

            #region Packet Header
            // CompanyID
            // [LGIS-GLOFA]
            headerBytes[0] = 0x4C;   //L
            headerBytes[1] = 0x47;   //G
            headerBytes[2] = 0x49;   //I
            headerBytes[3] = 0x53;   //S
            headerBytes[4] = 0x2D;   //-
            headerBytes[5] = 0x47;   //G
            headerBytes[6] = 0x4C;   //L
            headerBytes[7] = 0x4F;   //O
            headerBytes[8] = 0x46;   //F
            headerBytes[9] = 0x41;   //A

            // PLC Info
            // Client->PLC : 0x00 0x00
            headerBytes[10] = 0x02;
            headerBytes[11] = 0x08;

            // CPU Info
            // XGK:0xA0 / KGB:0xB0 / XGI:0xA4 / XGB:0xB4 / XGR:0xA8
            headerBytes[12] = 0xB4;

            // Source of Frame
            // Client->PLC : 0x33 / PLC->Client : 0x11
            headerBytes[13] = 0x11;

            // InvokeID
            headerBytes[14] = 0x00;
            headerBytes[15] = 0x01;

            // Length
            // Command + DataType + Data Length(Body Length)
            headerBytes[16] = (byte)bodyBytes.Length;
            headerBytes[17] = 0x00;

            // FEnet Position
            headerBytes[18] = 0x01;

            // CheckSum
            // Header Byte Sum
            headerBytes[19] = HeaderSum(headerBytes);
            #endregion

            byte[] responseByte = new byte[headerBytes.Length + bodyBytes.Length];
            Buffer.BlockCopy(headerBytes, 0, responseByte, 0, headerBytes.Length);
            Buffer.BlockCopy(bodyBytes, 0, responseByte, headerBytes.Length, bodyBytes.Length);
            return responseByte;
        }

        byte HeaderSum(byte[] value)
        {
            int v = 0;

            for (int i = 0; i < 20; i++)
            {
                v += value[i];

                if (v > 256)
                {
                    v = v - 256;
                }
            }

            return (byte)v;
        }

        public List<PLCMemory> GetMemoryAddressMap(int index)
        {
            switch (index)
            {
                case 0:
                    return map_M;
                case 1:
                    return map_D;
                case 2:
                    return map_K;
                case 3:
                    return map_L;
                default:
                    return null;
            }

        }
    }
}
