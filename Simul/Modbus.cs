using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PLCSimulation
{
    class Modbus
    {
        readonly int[] FunctionCode = new int[8]
        {
            1,
            2,
            3,
            4,
            5,
            6,
            15,
            16
        };

        const int MBAP_LENGTH = 7;
        const int FUNCTION_LENGTH = 1;
        const byte ERROR_CODE_ADD = 0x80;

        readonly byte ON = 0xff;
        readonly byte OFF = 0x00;

        ushort transactionID;
        ushort protocolID;
        byte unitID;
        byte functionCode;
        string address;
        int count;
        ushort length;
        bool isRead;
        bool isBit;

        byte[] writeBytes;

        //Memory Address Map[Coil/Input/HoldingReg/InputReg]
        List<PLCMemory> map_Coil;
        List<PLCMemory> map_Input;
        List<PLCMemory> map_HoldingRegister;
        List<PLCMemory> map_InputRegister;

        int coil_Size;
        int input_Size;
        int holdingRegister_Size;
        int inputRegister_Size;

        public Modbus(int coilSize = 0, int inputSize = 0, int holdingRegisterSize = 0, int inputRegisterSize = 0)
        {
            coil_Size = coilSize;
            input_Size = inputSize;
            holdingRegister_Size = holdingRegisterSize;
            inputRegister_Size = inputRegisterSize;

            if(coilSize >= 0)
            {
                map_Coil = new List<PLCMemory>();
                int count = coilSize / 10 + (coilSize % 10 > 0 ? 1 : 0);

                for (int i = 0; i < count; i++)
                    map_Coil.Add(new PLCMemory() { Address = (i * 10).ToString() });
            }

            if (inputSize >= 0)
            {
                map_Input = new List<PLCMemory>();
                int count = inputSize / 10 + (inputSize % 10 > 0 ? 1 : 0);

                for (int i = 0; i < count; i++)
                    map_Input.Add(new PLCMemory() { Address = (i * 10).ToString() });
            }

            if (holdingRegisterSize >= 0)
            {
                map_HoldingRegister = new List<PLCMemory>();
                int count = holdingRegisterSize / 10 + (holdingRegisterSize % 10 > 0 ? 1 : 0); ;

                for (int i = 0; i < count; i++)
                    map_HoldingRegister.Add(new PLCMemory() { Address = (i * 10).ToString() });
            }

            if (inputRegisterSize >= 0)
            {
                map_InputRegister = new List<PLCMemory>();
                int count = inputRegisterSize / 10 + (inputRegisterSize % 10 > 0 ? 1 : 0); ;

                for (int i = 0; i < count; i++)
                    map_InputRegister.Add(new PLCMemory() { Address = (i * 10).ToString() });
            }
        }

        public byte[] ResponsePacket(byte[] message)
        {
            bool isError = false;
            byte[] dataBytes = null;

            try
            {
                DesPacket(message);

                if (FunctionCode.Contains(functionCode) == false) // 정의되지 않은 function code
                {
                    isError = true;
                }
                else if (isError == false) // 주소 범위 확인
                {
                    switch (functionCode)
                    {
                        // coil
                        case 0x01:
                            if (Convert.ToInt32(address) + count > coil_Size)
                                isError = true;
                            break;
                        case 0x05:
                            if (Convert.ToInt32(address) >= coil_Size)
                                isError = true;
                            break;
                        case 0x0f:
                            if (Convert.ToInt32(address) + length > coil_Size)
                                isError = true;
                            break;
                        // input
                        case 0x02:
                            if (Convert.ToInt32(address) + count > input_Size)
                                isError = true;
                            break;
                        // holdingRegister
                        case 0x03:
                            if (Convert.ToInt32(address) + count > holdingRegister_Size)
                                isError = true;
                            break;
                        case 0x06:
                            if (Convert.ToInt32(address) >= holdingRegister_Size)
                                isError = true;
                            break;
                        case 0x10:
                            if (Convert.ToInt32(address) + length > holdingRegister_Size)
                                isError = true;
                            break;
                        // inputRegister
                        case 0x04:
                            if (Convert.ToInt32(address) + count > inputRegister_Size)
                                isError = true;
                            break;
                    }
                }


                if (isError == false)
                    DataProcessing(ref dataBytes); // Read/Write dataBytes == readData

                var responseByte = MakePacket(isError, dataBytes);
                return responseByte;
            }
            catch (Exception ex)
            { // response Error
                System.Diagnostics.Trace.WriteLine(ex.Message);
                var responseByte = MakePacket(isError, dataBytes);
                return responseByte;
            }
        }

        private void DataProcessing(ref byte[] dataBytes) // address = startAddr, count = size , dataBytes => MemoryData
        {
            List<PLCMemory> memoryMap = new();

            if (functionCode == 0x01 || functionCode == 0x05 || functionCode == 0x0f)
                memoryMap = map_Coil;
            else if (functionCode == 0x02)
                memoryMap = map_Input;
            else if (functionCode == 0x03 || functionCode == 0x06 || functionCode == 0x10)
                memoryMap = map_HoldingRegister;
            else
                memoryMap = map_InputRegister;

            if (isRead)
            {
                if (isBit)
                {
                    dataBytes = new byte[count / 8 + (count % 8 > 0 ? 1 : 0)];

                    int lstIndex = Convert.ToInt32(address) / 10;
                    int memoryIndex = Convert.ToInt32(address) % 10; // == bitIndex

                    var lstReadData = new List<byte>();
                    for (int i = 0; i < count; i++)
                    {
                        lstReadData.Add(BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex++))[0]); // 1 bit

                        if (memoryIndex > 9)
                        {
                            lstIndex += 1;
                            memoryIndex = 0;
                        }
                    }

                    var tempArr = lstReadData.Cast<byte>().Select(bit => bit == 0x01 ? true : false).ToArray();
                    BitArray bitArr = new BitArray(tempArr);
                    bitArr.CopyTo(dataBytes, 0);
                }
                else
                {
                    dataBytes = new byte[count * 2];

                    int lstIndex = Convert.ToInt32(address) / 10;
                    int memoryIndex = Convert.ToInt32(address) % 10; // wordIndex

                    for(int i = 0; i < count; i++)
                    {
                        var data = BitConverter.GetBytes(memoryMap[lstIndex].GetMemoryData(memoryIndex++));
                        Buffer.BlockCopy(data, 0, dataBytes, i * 2, data.Length);

                        if (memoryIndex > 9)
                        {
                            lstIndex += 1;
                            memoryIndex = 0;
                        }
                    }
                }
            }
            else // write
            {
                if (isBit)
                {
                    dataBytes = new byte[count / 8 + (count % 8 > 0 ? 1 : 0)];

                    int lstIndex = Convert.ToInt32(address) / 10;
                    int memoryIndex = Convert.ToInt32(address) % 10;

                    if (functionCode == 0x05)
                    {
                        // single write
                        if (writeBytes[0] == ON)
                            memoryMap[lstIndex].SetMemoryData(memoryIndex, 1);
                        else
                            memoryMap[lstIndex].SetMemoryData(memoryIndex, 0);
                    }
                    else
                    {
                        // get
                        var bitArr = new BitArray(writeBytes);
                        var writeBits = bitArr.Cast<bool>().Select(bit => bit ? (byte)1 : (byte)0).ToArray();

                        // data change - set
                        for (int i = 0; i < writeBits.Length; i++)
                        {
                            memoryMap[lstIndex].SetMemoryData(memoryIndex++, writeBits[i]);

                            if (memoryIndex > 9)
                            {
                                lstIndex += 1;
                                memoryIndex = 0;
                            }
                        }
                    }
                }
                else
                {
                    int lstIndex = Convert.ToInt32(address) / 10;
                    int memoryIndex = Convert.ToInt32(address) % 10; // wordIndex

                    ushort[] data = new ushort[writeBytes.Length / 2];
                    writeBytes = SwapByte(writeBytes);
                    Buffer.BlockCopy(writeBytes, 0, data, 0, writeBytes.Length); // data change

                    for (int i = 0; i < data.Length; i++)
                    {
                        memoryMap[lstIndex].SetMemoryData(memoryIndex++, data[i]); // set

                        if (memoryIndex > 9)
                        {
                            lstIndex += 1;
                            memoryIndex = 0;
                        }
                    }
                }
            }

            return;
        }

        private void DesPacket(byte[] bytes) // reqPacket
        {
            var lstPacket = bytes.ToList();

            transactionID = BitConverter.ToUInt16(SwapByte(lstPacket.GetRange(0, 2).ToArray()));
            protocolID = BitConverter.ToUInt16(SwapByte(lstPacket.GetRange(2, 2).ToArray()));
            unitID = lstPacket.GetRange(6, 1)[0];

            var bodyLength = BitConverter.ToInt16(SwapByte(lstPacket.GetRange(4, 2).ToArray()));
            var bodyBytes = lstPacket.GetRange(7, bodyLength - 1);

            functionCode = bodyBytes[0];

            address = BitConverter.ToUInt16(SwapByte(bodyBytes.GetRange(1, 2).ToArray())).ToString();
            if (functionCode == 0x01 || functionCode == 0x02 || functionCode == 0x05 || functionCode == 0x0f)
            {
                isBit = true;
            }
            else
                isBit = false;

            switch (functionCode)
            {
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                    isRead = true;
                    count = BitConverter.ToUInt16(SwapByte(bodyBytes.GetRange(3, 2).ToArray()));
                    break;

                case 0x05:
                case 0x06:
                    isRead = false;
                    writeBytes = bodyBytes.GetRange(3, bodyLength - 4).ToArray();
                    break;

                case 0x0f:
                case 0x10:
                    isRead = false;
                    length = BitConverter.ToUInt16(SwapByte(bodyBytes.GetRange(3, 2).ToArray()));
                    writeBytes = bodyBytes.GetRange(6, bodyLength - 7).ToArray();
                    break;
            }
        }

        private byte[] MakePacket(bool isError, byte[] dataByte = null)
        {
            byte[] responseByte;
            byte[] headerByte = new byte[MBAP_LENGTH + FUNCTION_LENGTH];

            var transactionByte = BitConverter.GetBytes(transactionID);
            headerByte[0] = transactionByte[1];
            headerByte[1] = transactionByte[0];

            var protocolIDByte = BitConverter.GetBytes(protocolID);
            headerByte[2] = protocolIDByte[1];
            headerByte[3] = protocolIDByte[0];

            // headerbyte [4~5] : length byte
            
            headerByte[6] = unitID;

            if (isError == false)
                headerByte[7] = functionCode;
            else
            {
                byte[] errorByte = new byte[9];

                Buffer.BlockCopy(headerByte, 0, errorByte, 0, headerByte.Length);

                errorByte[7] = (byte)(functionCode | ERROR_CODE_ADD);
                errorByte[8] = 0x01;
                return headerByte;
                
            }

            byte[] bodyByte;
            if (isRead)
            {
                if (isBit)
                {
                    bodyByte = new byte[1 + dataByte.Length];
                    bodyByte[0] = BitConverter.GetBytes(count)[0];
                    Buffer.BlockCopy(dataByte, 0, bodyByte, 1, dataByte.Length);
                }
                else
                {
                    bodyByte = new byte[1 + count * 2];
                    bodyByte[0] = BitConverter.GetBytes(count)[0];
                    var swapByte = SwapByte(dataByte);
                    Buffer.BlockCopy(swapByte, 0, bodyByte, 1, swapByte.Length);
                }
            }
            else
            {
                if(functionCode == 0x0f || functionCode == 0x10) // multi
                {
                    bodyByte = new byte[2 + 2]; // start address(2) + read length(2)

                    var addrByte = BitConverter.GetBytes(Convert.ToUInt16(address));
                    bodyByte[0] = addrByte[1];
                    bodyByte[1] = addrByte[0];

                    var lengthByte = BitConverter.GetBytes(length);
                    bodyByte[2] = lengthByte[1];
                    bodyByte[3] = lengthByte[0];
                }
                else // single
                {
                    bodyByte = new byte[2 + writeBytes.Length];

                    var addrByte = BitConverter.GetBytes(Convert.ToUInt16(address));
                    bodyByte[0] = addrByte[1];
                    bodyByte[1] = addrByte[0];

                    Buffer.BlockCopy(writeBytes, 0, bodyByte, 2, writeBytes.Length);
                }
            }

            responseByte = new byte[headerByte.Length + bodyByte.Length];
            
            //length add
            var headerLengthByte = BitConverter.GetBytes((ushort)(2 + bodyByte.Length));
            headerByte[4] = headerLengthByte[0];
            headerByte[5] = headerLengthByte[1];

            Buffer.BlockCopy(headerByte, 0, responseByte, 0, headerByte.Length);
            Buffer.BlockCopy(bodyByte, 0, responseByte, headerByte.Length, bodyByte.Length);

            return responseByte;
        }

        public List<PLCMemory> GetMemoryAddressMap(int index)
        {
            switch (index)
            {
                case 0:
                    return map_Coil;
                case 1:
                    return map_Input;
                case 2:
                    return map_HoldingRegister;
                case 3:
                    return map_InputRegister;
                default:
                    return null;
            }
        }


        private byte[] SwapByte(byte[] bytes)
        {
            if (bytes.Length % 2 > 0)
            {
                System.Diagnostics.Trace.WriteLine("Packet Length Wrong");
                return bytes;
            }

            for(int i = 0; i < bytes.Length; i += 2)
            {
                var tempByte = bytes[i];
                bytes[i] = bytes[i+1];
                bytes[i+1] = tempByte;
            }

            return bytes;
        }
    }
}
