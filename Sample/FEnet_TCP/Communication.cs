using CommonInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FEnet_TCP
{
    class Communication : TCP
    {
        const int MAX_PROTOCOL_SIZE = 512; // 프로토콜 최대 크기
        const int HEADER_LENGTH = 20; // 요청 헤더 길이
        const int BODY_LENGTH = 21; // 요청 바디 길이
        const int SINGLE_BODY_LENGTH = 19; // 비트 요청 바디 길이
        const int READ_EXCEPT_LENGTH = 32; // 응답 패킷 제외 길이
        const int RETRY_COUNT = 3; // 재시도 횟수 제한

        public Communication(string ipAddr, int port, string hostName = "", int timeout = 3000) : base(ipAddr, port, hostName, timeout)
        {
        }

        public bool ReadBit(string memoryName, AddrInfo addrInfo, ref object data) // 단일 비트 영역 읽기
        {
            try
            {
                List<ushort> lstAddrs = new();

                ushort addr = (ushort)addrInfo.addr;
                if (addrInfo.bitAddr != null)
                    addr = (ushort)(addrInfo.addr * 16 + addrInfo.bitAddr);

                for(ushort i = 0; i < addrInfo.length; i++)
                {
                    lstAddrs.Add((ushort)(addr + i));
                }

                int loopIndex = 0;
                int maxCount = lstAddrs.Count / 16 + (lstAddrs.Count % 16 == 0 ? 0 : 1); // 한 패킷당 요청 주소 갯수 16개 제한
                byte[] readData = new byte[lstAddrs.Count];

                //통신
                while(loopIndex != maxCount) // 요청 주소 길이에 따른 반복 횟수 제한
                {
                    var requestAddrs = lstAddrs.Skip(16 * loopIndex).Take(16).ToList(); // 요청 주소 추출

                    var commandByte = MakePacketBits(false, memoryName, requestAddrs, (ushort)requestAddrs.Count); // 요청 패킷 생성

                    byte[] readByte = new byte[READ_EXCEPT_LENGTH + (3 * requestAddrs.Count)];
                    for(int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                    {
                        Write(commandByte); // 패킷 전송
                        if(Read(ref readByte)) // 응답 패킷 수신
                        {
                            var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                            var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                            if(BitConverter.ToInt16(errorStateByte) != 0) // 에러 코드 확인
                            {
                                Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                                return false;
                            }

                            break;
                        }

                        if (retryCount == RETRY_COUNT - 1) // 재시도 횟수 확인
                            return false;
                    }

                    for(int i = 0; i < requestAddrs.Count; i++) // 데이터 추출
                    {
                        Buffer.BlockCopy(readByte, READ_EXCEPT_LENGTH + (i * 3), readData, (16 * loopIndex) + i, 1);
                    }

                    loopIndex++;
                }

                data = readData; // 데이터 반환 장소 저장
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        public bool ReadBits(string memoryName, List<AddrInfo> lstAddrInfo, Dictionary<string, Dictionary<string, object>> dicData) // 복수 비트 영역 읽기
        {
            List<ushort> lstAddrs = new();

            for (int i = 0; i < lstAddrInfo.Count; i++)
            {
                ushort addr = (ushort)(lstAddrInfo[i].addr);
                if (lstAddrInfo[i].bitAddr != null)
                    addr = (ushort)(lstAddrInfo[i].addr * 16 + lstAddrInfo[i].bitAddr);

                for (ushort j = 0; j < lstAddrInfo[i].length; j++)
                {
                    lstAddrs.Add((ushort)(addr + j));
                }
            }

            int loopIndex = 0;
            int maxCount = lstAddrs.Count / 16 + (lstAddrs.Count % 16 == 0 ? 0 : 1); // 한 패킷당 요청 주소 갯수 16개 제한
            byte[] readData = new byte[lstAddrs.Count];

            // 통신
            while (loopIndex != maxCount)
            {
                var requestAddrs = lstAddrs.Skip(16 * loopIndex).Take(16).ToList();

                var commandByte = MakePacketBits(false, memoryName, requestAddrs, (ushort)requestAddrs.Count);

                byte[] readByte = new byte[READ_EXCEPT_LENGTH + (3 * requestAddrs.Count)];
                for(int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                {
                    Write(commandByte);
                    if(Read(ref readByte))
                    {
                        var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                        var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                        if(BitConverter.ToInt16(errorStateByte) != 0)
                        {
                            Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                            return false;
                        }

                        break;
                    }

                    if (retryCount == RETRY_COUNT - 1)
                        return false;
                }

                for (int i = 0; i < requestAddrs.Count; i++)
                {
                    Buffer.BlockCopy(readByte, READ_EXCEPT_LENGTH + (i * 3), readData, (16 * loopIndex) + i, 1);
                }

                loopIndex++;
            }

            // 할당
            int offset = 0;
            foreach (var addrInfo in lstAddrInfo)
            {
                var splitStr = addrInfo.tagName.Split('.');
                string equipName = splitStr[0];
                string tagName = splitStr[1];
                int length = addrInfo.length;

                byte[] data = new byte[length];
                Buffer.BlockCopy(readData, offset, data, 0, length);
                dicData[equipName][tagName] = data;

                offset += length;
            }

            return true;
        }

        public bool ReadRegister(string memoryName, AddrInfo addrInfo, ref object data) // 단일 레지스터 영역 읽기
        {
            try
            {
                var commandByte = MakePacketRegisters(false, memoryName, (ushort)addrInfo.addr, (ushort)(addrInfo.length)); // 패킷 생성
                
                byte[] readByte = new byte[READ_EXCEPT_LENGTH + addrInfo.length * 2]; // 응답 패킷 할당
                for (int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                {
                    Write(commandByte); // 요청 패킷 전송
                    if (Read(ref readByte)) // 응답 패킷 수신
                    {
                        var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                        var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                        if (BitConverter.ToInt16(errorStateByte) != 0) // 에러 코드 확인
                        {
                            Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                            return false;
                        }

                        break;
                    }

                    if (retryCount == RETRY_COUNT - 1)
                        return false;
                }

                byte[] dataByte = new byte[addrInfo.length];
                Buffer.BlockCopy(readByte, READ_EXCEPT_LENGTH, dataByte, 0, dataByte.Length); // 데이터 추출

                data = dataByte; // 데이터 저장
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        public bool ReadRegisters(string memoryName, List<AddrInfo> lstAddrInfo, Dictionary<string, Dictionary<string, object>> dicData) // 복수 레지스터 영역 읽기
        {// memoryName : M/D + Type (W/B) | addrs -> addr/length/bitAddr/data | 1addr = 2byte
            List<int> lstAddrs = new List<int>();
            List<int> lstLengths = new List<int>();
            foreach (var info in lstAddrInfo)
            {
                lstAddrs.Add(info.addr);
                lstLengths.Add(info.length);
            }

            // Cycle Map
            MemoryManager map = new MemoryManager();
            map.CreateMap(lstAddrs, lstLengths, MAX_PROTOCOL_SIZE); // 요청 주소 패킷 정보 객체 생성

            int index = 0;

            for (int i = 0; i < map.Addr.Count; i++)
            {
                AddrInfo addrInfo = new AddrInfo() { addr = map.Addr[i], length = map.Length[i] };
                object readObj = new byte[map.Length[i]];

                if (ReadRegister(memoryName, addrInfo, ref readObj)) // 시작 주소, 길이를 통한 단일 주소 읽기 요청
                {
                    byte[] readByte = (readObj as byte[]);

                    for (; index < lstAddrInfo.Count; index++)
                    {
                        if (i < map.Addr.Count - 1)
                        {
                            if (map.Addr[i + 1] == lstAddrInfo[index].addr)
                                break;
                        }

                        int offset = lstAddrInfo[index].addr - map.Addr[i];
                        byte[] data = new byte[lstAddrInfo[index].length];
                        Buffer.BlockCopy(readByte, offset, data, 0, data.Length); // 데이터 추출

                        var splitStr = lstAddrInfo[index].tagName.Split('.');
                        string equipName = splitStr[0];
                        string tagName = splitStr[1];
                        dicData[equipName][tagName] = data; // 데이터 저장
                    }
                }
                else // 통신 오류에 의한 재정리 (index)
                {
                    if (IsError)
                        throw new System.Net.Sockets.SocketException(10061);

                    for (; index < lstAddrInfo.Count; index++)
                    {
                        if (i < map.Addr.Count - 1)
                        {
                            if (map.Addr[i + 1] == lstAddrInfo[index].addr)
                                break;
                        }
                    }
                }
            }

            return true;
        }

        public bool WriteBit(string memoryName, AddrInfo addrInfo, byte[] data)
        {//register area
            try
            {
                List<ushort> lstAddrs = new();

                ushort addr = (ushort)(addrInfo.addr * 16 + addrInfo.bitAddr);

                for(int i = 0; i < addrInfo.length; i++)
                {
                    lstAddrs.Add((ushort)(addr + i));
                }

                int loopIndex = 0;
                int maxCount = lstAddrs.Count / 16 + (lstAddrs.Count % 16 == 0 ? 0 : 1);

                while(loopIndex != maxCount)
                {
                    var requestAddrs = lstAddrs.Skip(16 * loopIndex).Take(16).ToList();
                    var requestWriteData = data.Skip(16 * loopIndex).Take(16).ToArray();

                    var commandByte = MakePacketBits(true, memoryName, requestAddrs, (ushort)requestAddrs.Count, requestWriteData);

                    byte[] readByte = new byte[HEADER_LENGTH + 10];
                    for (int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                    {
                        Write(commandByte);
                        if (Read(ref readByte))
                        {
                            var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                            var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                            if (BitConverter.ToInt16(errorStateByte) != 0)
                            {
                                Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                                return false;
                            }

                            break;
                        }

                        if (retryCount == RETRY_COUNT - 1)
                            return false;
                    }

                    loopIndex++;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        public bool WriteBits(string memoryName, List<AddrInfo> lstAddrInfo, List<byte[]> lstData)
        {
            List<ushort> lstAddrs = new();

            for (int i = 0; i < lstAddrInfo.Count; i++)
            {
                ushort addr = (ushort)(lstAddrInfo[i].addr);
                if (lstAddrInfo[i].bitAddr != null)
                    addr = (ushort)(lstAddrInfo[i].addr * 16 + lstAddrInfo[i].bitAddr);

                for (ushort j = 0; j < lstAddrInfo[i].length; j++)
                {
                    lstAddrs.Add((ushort)(addr + j));
                }
            }

            int loopIndex = 0;
            int maxCount = lstAddrs.Count / 16 + (lstAddrs.Count % 16 == 0 ? 0 : 1);
            byte[] writeData = lstData.SelectMany(x => x).ToArray();

            while(loopIndex != maxCount)
            {
                var requestAddrs = lstAddrs.Skip(16 * loopIndex).Take(16).ToList();
                var requestWriteData = writeData.Skip(16 * loopIndex).Take(16).ToArray();

                var commandByte = MakePacketBits(true, memoryName, requestAddrs, (ushort)requestAddrs.Count, requestWriteData);

                byte[] readByte = new byte[HEADER_LENGTH + 10];
                for (int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                {
                    Write(commandByte);
                    if (Read(ref readByte))
                    {
                        var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                        var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                        if (BitConverter.ToInt16(errorStateByte) != 0)
                        {
                            Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                            return false;
                        }

                        break;
                    }

                    if (retryCount == RETRY_COUNT - 1)
                        return false;
                }

                loopIndex++;
            }

            return true;
        }

        public bool WriteRegister(string memoryName, AddrInfo addrInfo, byte[] data)
        {
            try
            {
                // Packet Create
                var commandByte = MakePacketRegisters(true, memoryName, (ushort)addrInfo.addr, (ushort)addrInfo.length, data);

                // Communication
                byte[] readByte = new byte[HEADER_LENGTH + 10];
                for (int retryCount = 0; retryCount < RETRY_COUNT; retryCount++)
                {
                    Write(commandByte);
                    if (Read(ref readByte))
                    {
                        var errorStateByte = readByte.ToList().GetRange(26, 2).ToArray();
                        var errorCodeByte = readByte.ToList().GetRange(28, 2).ToArray();

                        if (BitConverter.ToInt16(errorStateByte) != 0)
                        {
                            Trace.WriteLine($"ErrorCode : {BitConverter.ToInt16(errorCodeByte)}");
                            return false;
                        }

                        break;
                    }

                    if (retryCount == RETRY_COUNT - 1)
                        return false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        public bool WriteRegisters(string memoryName, List<AddrInfo> lstAddrInfo, List<byte[]> lstData)
        {
            try
            {
                List<int> lstAddrs = new List<int>();
                List<int> lstLengths = new List<int>();

                foreach (var Info in lstAddrInfo)
                {
                    lstAddrs.Add(Info.addr);
                    lstLengths.Add(Info.length);
                }

                MemoryManager map = new MemoryManager();
                map.CreateMap(lstAddrs, lstLengths, MAX_PROTOCOL_SIZE);

                int index = 0;
                for (int i = 0; i < map.Addr.Count; i++)
                {
                    AddrInfo addrInfo = new AddrInfo() { addr = map.Addr[i], length = map.Length[i] };

                    //read
                    object readObj = new byte[map.Length[i] * 2];
                    if(ReadRegister(memoryName, addrInfo, ref readObj) == false)
                    {
                        for(; index < lstAddrInfo.Count; index++)
                        {
                            if(i < map.Addr.Count - 1)
                            {
                                if (map.Addr[i + 1] == lstAddrInfo[index].addr)
                                    break;
                            }
                        }

                        continue;
                    }

                    byte[] readByte = (readObj as byte[]);

                    // data modify
                    for(; index < lstAddrInfo.Count; index++)
                    {
                        if(i < map.Addr.Count - 1)
                        {
                            if (map.Addr[i + 1] == lstAddrInfo[index].addr)
                                break;
                        }

                        int offset = lstAddrInfo[index].addr - map.Addr[i];
                        Buffer.BlockCopy(lstData[index], 0, readByte, offset, lstData[index].Length);
                    }

                    // write
                    WriteRegister(memoryName, addrInfo, readByte);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        #region 패킷 구성
        private byte[] MakePacketRegisters(bool isWrite, string memoryName, ushort startAddr, ushort count, byte[] writeByte = null)
        {
            byte[] headerBytes = new byte[HEADER_LENGTH]; // 패킷 헤더
            byte[] bodyBytes = new byte[BODY_LENGTH]; // 패킷 바디

            #region Packet Body
            if (isWrite)
            {
                bodyBytes = new byte[BODY_LENGTH + writeByte.Length];
            }

            if (isWrite)
            {
                bodyBytes[0] = 0x58;
                bodyBytes[1] = 0x00;
            }
            else
            {
                bodyBytes[0] = 0x54;
                bodyBytes[1] = 0x00;
            }

            // 데이터 타입
            bodyBytes[2] = 0x14;
            bodyBytes[3] = 0x00;

            // 데이터
            // Reserve
            bodyBytes[4] = 0x00;
            bodyBytes[5] = 0x00;

            // 블록 개수
            bodyBytes[6] = 0x01;
            bodyBytes[7] = 0x00;

            // 변수 길이
            bodyBytes[8] = 0x09;
            bodyBytes[9] = 0x00;

            // 데이터 주소
            bodyBytes[10] = 0x25; // %
            bodyBytes[11] = Encoding.ASCII.GetBytes(memoryName.Substring(0, 1))[0];
            bodyBytes[12] = Encoding.ASCII.GetBytes(memoryName.Substring(1, 1))[0];

            bodyBytes[13] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(0, 1))).ToString("X"))[0];
            bodyBytes[14] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(1, 1))).ToString("X"))[0];
            bodyBytes[15] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(2, 1))).ToString("X"))[0];
            bodyBytes[16] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(3, 1))).ToString("X"))[0];
            bodyBytes[17] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(4, 1))).ToString("X"))[0];
            bodyBytes[18] = Encoding.ASCII.GetBytes(Convert.ToInt32((startAddr.ToString("D6").Substring(5, 1))).ToString("X"))[0];

            // 데이터 개수
            bodyBytes[19] = (byte)(count % 256);
            bodyBytes[20] = (byte)(count / 256);

            // Write Data
            if (isWrite)
            {
                Buffer.BlockCopy(writeByte, 0, bodyBytes, BODY_LENGTH, writeByte.Length);
            }
            #endregion

            #region Packet Header
            // CompanyID
            // [LSIS-XGT\n\n] = 0x4C 0x53 0x49 0x53 0x2D 0x58 0x47 0x54 0x00 0x00
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
            headerBytes[10] = 0x00;
            headerBytes[11] = 0x00;

            // CPU Info
            // XGK:0xA0 / KGB:0xB0 / XGI:0xA4 / XGB:0xB4 / XGR:0xA8
            headerBytes[12] = 0x00;

            // Source of Frame
            // Client->PLC : 0x33 / PLC->Client : 0x11
            headerBytes[13] = 0x33;

            // InvokeID
            headerBytes[14] = 0x00;
            headerBytes[15] = 0x00;

            // Length
            // Command + DataType + Data Length(Body Length)
            headerBytes[16] = (byte)bodyBytes.Length;
            headerBytes[17] = 0x00;

            // FEnet Position
            headerBytes[18] = 0x00;

            // CheckSum
            // Header Byte Sum
            headerBytes[19] = HeaderSum(headerBytes);
            #endregion

            byte[] commandByte = new byte[HEADER_LENGTH + bodyBytes.Length];
            Buffer.BlockCopy(headerBytes, 0, commandByte, 0, HEADER_LENGTH);
            Buffer.BlockCopy(bodyBytes, 0, commandByte, HEADER_LENGTH, bodyBytes.Length);
            return commandByte; // 생성 패킷 반환
        }

        private byte[] MakePacketBits(bool isWrite, string memoryName, List<ushort> startAddr, ushort count, byte[] writeByte = null)
        {
            byte[] headerBytes = new byte[HEADER_LENGTH];
            byte[] bodyBytes = new byte[SINGLE_BODY_LENGTH + 11 * (count - 1)];

            if (isWrite)
            {
                bodyBytes = new byte[BODY_LENGTH + 1 + 14 * (count - 1)];
            }

            #region Packet Body
            if (isWrite)
            {
                bodyBytes[0] = 0x58;
                bodyBytes[1] = 0x00;
            }
            else
            {
                bodyBytes[0] = 0x54;
                bodyBytes[1] = 0x00;
            }

            // 데이터 타입
            bodyBytes[2] = 0x00;
            bodyBytes[3] = 0x00;

            // 데이터
            // Reserve
            bodyBytes[4] = 0x00;
            bodyBytes[5] = 0x00;

            // 블록 개수
            bodyBytes[6] = (byte)count;
            bodyBytes[7] = 0x00;

            for (int i = 0; i < count; i++)
            {
                int bodyBytesIndex = i * 11;
                // 변수 길이
                bodyBytes[8 + bodyBytesIndex] = 0x09;
                bodyBytes[9 + bodyBytesIndex] = 0x00;

                // 데이터 주소
                bodyBytes[10 + bodyBytesIndex] = 0x25;
                bodyBytes[11 + bodyBytesIndex] = Encoding.ASCII.GetBytes(memoryName.Substring(0, 1))[0]; // 메모리 영역
                bodyBytes[12 + bodyBytesIndex] = Encoding.ASCII.GetBytes(memoryName.Substring(1, 1))[0]; //  bit(X) : 0x58, byte(B) : 0x42

                string hexstring;
                switch (memoryName.Substring(0, 1))
                {
                    case "T":
                    case "C":
                        hexstring = (startAddr[i] / 16).ToString().PadLeft(6, '0');
                        break;
                    default:
                        hexstring = (startAddr[i]).ToString().PadLeft(6, '0');
                        break;
                }

                //데이터 주소
                bodyBytes[13 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(0, 1))[0];
                bodyBytes[14 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(1, 1))[0];
                bodyBytes[15 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(2, 1))[0];
                bodyBytes[16 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(3, 1))[0];
                bodyBytes[17 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(4, 1))[0];
                bodyBytes[18 + bodyBytesIndex] = Encoding.ASCII.GetBytes(hexstring.Substring(5, 1))[0];

                if (isWrite)
                {
                    // Write Data
                    int bodyBytesDataIndex = i * 3;

                    // 데이터 개수
                    bodyBytes[19 + 11 * (count - 1) + bodyBytesDataIndex] = 0x00;
                    bodyBytes[20 + 11 * (count - 1) + bodyBytesDataIndex] = 0x01;

                    // 데이터
                    bodyBytes[21 + 11 * (count - 1) + bodyBytesDataIndex] = writeByte[i];
                }
            }
            #endregion

            #region Packet Header
            // CompanyID
            // [LSIS-XGT\n\n] = 0x4C 0x53 0x49 0x53 0x2D 0x58 0x47 0x54 0x00 0x00
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
            headerBytes[10] = 0x00;
            headerBytes[11] = 0x00;

            // CPU Info
            // XGK:0xA0 / KGB:0xB0 / XGI:0xA4 / XGB:0xB4 / XGR:0xA8
            headerBytes[12] = 0x00;

            // Source of Frame
            // Client->PLC : 0x33 / PLC->Client : 0x11
            headerBytes[13] = 0x33;

            // InvokeID
            headerBytes[14] = 0x00;
            headerBytes[15] = 0x00;

            // Length
            // Command + DataType + Data Length(Body Length)
            headerBytes[16] = (byte)bodyBytes.Length;
            headerBytes[17] = 0x00;

            // FEnet Position
            headerBytes[18] = 0x00;

            // CheckSum
            // Header Byte Sum
            headerBytes[19] = HeaderSum(headerBytes);
            #endregion

            byte[] commandByte = new byte[HEADER_LENGTH + bodyBytes.Length];
            Buffer.BlockCopy(headerBytes, 0, commandByte, 0, HEADER_LENGTH);
            Buffer.BlockCopy(bodyBytes, 0, commandByte, HEADER_LENGTH, bodyBytes.Length);
            return commandByte;
        }

        byte HeaderSum(byte[] value) // 헤더 체크 섬
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
        #endregion
    }
}
