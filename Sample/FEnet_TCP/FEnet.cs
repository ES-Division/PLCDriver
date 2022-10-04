using CommonInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace FEnet_TCP
{
    public class FEnet : DriverInterface.CommonDriver // Driver 인터페이스 상속
    {
        #region Memory List
        public struct MemoryNameList // 메모리 영역
        {
            public const string P = "P";
            public const string M = "M";
            public const string L = "L";
            public const string K = "K";
            public const string F = "F";
            public const string T = "T";
            public const string C = "C";
            public const string D = "D";
            public const string S = "S";
            public const string N = "N";
            public const string Z = "Z";
        }

        public struct MemoryTypeList // 메모리 타입
        {
            public const string X = "X"; // BIT
            public const string B = "B"; // BYTE (연속/Word)
            public const string W = "W"; // WORD
            public const string D = "D"; // DOUBLE WORD
        }
        #endregion

        // etc
        ConcurrentDictionary<string, ConcurrentDictionary<string, TagInfo>> conDictags; // Tag 정리 주소 key : equipName, value : (key : tagName , value : tagInfo)
        Communication comm; // 통신 객체

        bool bSetDriverInfo = false;
        bool bSetNet = false;

        public override Socket sock => comm.Sock; // 통신 객체 소켓

        public override DriverInfo getDriverInfo()
        {
            if (!bSetDriverInfo)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                driverInfo.driverName = fileVersionInfo.ProductName;
                driverInfo.protocolType = fileVersionInfo.Comments;
                driverInfo.provider = fileVersionInfo.CompanyName;
                driverInfo.releaseDate = new FileInfo(assembly.Location).LastWriteTime.ToString("yyyy-MM-dd");
                driverInfo.version = fileVersionInfo.ProductVersion;
                driverInfo.manufactureData = fileVersionInfo.FileDescription;
            }

            return driverInfo;
        }

        public override bool setTagInfo(string path)
        {//csv file ==> info Get
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists == false)
            {
                Trace.WriteLine($"File Path - {path}, File is not Exists");
                return false;
            }
            else if (fileInfo.Extension != Definition.CSV)
            {
                Trace.WriteLine($"File Path - {path}, File Extension is not .csv");
                return false;
            }

            if (conDictags == null)
                conDictags = new();
            else
                conDictags.Clear();

            if (lstTagInfo.Count > 0)
                lstTagInfo.Clear();

            try
            {
                string[] boolContainStrArr = { "1", "true", "TRUE", "t", "T", "O", "o", "" }; // Bool Convert Contain String Array

                // csv file korean encoding
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding encoding = Encoding.GetEncoding(949);

                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream, encoding))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        if (!bSetNet)
                        {// ip, port
                            ip = csv.GetField(0);
                            port = Convert.ToInt32(csv.GetField(1));

                            bSetNet = !bSetNet;
                        }

                        // EquipmentCode
                        if (csv.TryGetField(2, out string equipmentCode) == false)
                            continue;

                        // TagName
                        if (csv.TryGetField(3, out string name) == false)
                        { continue; }
                        else
                        {
                            if (name.Contains(' ') || string.IsNullOrEmpty(name))
                            {// 공백 문구 허용 X
                                Trace.WriteLine($"FEnet_TCP, Tag Name : {name}, No spaces are allowed in TagName");
                                continue;
                            }
                            else if (lstTagInfo.Where(x => x.name == name).Where(x => x.equipmentCode == equipmentCode).Count() != 0)
                            {// Tag Name 중복 허용 X
                                Trace.WriteLine($"FEnet_TCP, Equipment Code : {equipmentCode} / Tag Name : {name}, already registed");
                                continue;
                            }
                        }

                        // Address (addr/bitaddr/ishex)
                        if (csv.TryGetField(4, out string addrStr) == false)
                            continue;
                        bool isHex = addrStr.Substring(addrStr.Length - 1, 1) == "h" ? true : false;

                        string addr = string.Empty;
                        string bitAddr = string.Empty;

                        if (isHex == true)
                        {
                            var addrInfo = addrStr.Substring(0, addrStr.Length - 1).Split('.');
                            addr = BytesConvertor.HexToDec(addrInfo[0]).ToString();
                            bitAddr = BytesConvertor.HexToDec(addrInfo.Length > 1 ? addrInfo[1] : "0").ToString();
                        }
                        else
                        {
                            var addrInfo = addrStr.Split('.');
                            addr = addrInfo[0];
                            bitAddr = addrInfo.Length > 1 ? addrInfo[1] : "0";
                        }

                        // MemoryName
                        if (csv.TryGetField(5, out string memoryName) == false)
                            continue;

                        // MemoryType Check
                        if (csv.TryGetField(6, out string memoryType) == false)
                            continue;

                        switch (memoryType)
                        {
                            case MemoryTypeList.X:
                            case MemoryTypeList.B:
                            case MemoryTypeList.W:
                            case MemoryTypeList.D:
                                break;
                            default:
                                Trace.WriteLine($"FEnet_TCP, Tag Name : {name}, MemoryType : {memoryType} is not Defined");
                                continue;
                        }

                        // Data Size
                        if (csv.TryGetField(7, out int size) == false)
                            continue;

                        // dataType
                        if (csv.TryGetField(8, out string dataType) == false)
                        { continue; }
                        else
                        {
                            if (string.IsNullOrEmpty(dataType))
                                dataType = "int";
                        }

                        // ratio
                        if (csv.TryGetField(9, out string ratio) == false)
                        { continue; }
                        else
                        {
                            if (string.IsNullOrEmpty(ratio))
                                ratio = "1";
                        }

                        // scan send Data
                        bool isScan;
                        if (csv.TryGetField(10, out string scan) == false)
                            continue;
                        else
                        {
                            isScan = boolContainStrArr.Any(scan.Equals);
                        }

                        var taginfo = new TagInfo()
                        {
                            equipmentCode = equipmentCode,
                            addr = addr,
                            bitAddr = bitAddr,
                            name = name,
                            memoryName = memoryName,
                            memoryType = memoryType,
                            size = size,
                            isHex = isHex,
                            dataType = dataType,
                            ratio = ratio,
                            isScan = isScan
                        };

                        lstTagInfo.Add(taginfo);

                        if (conDictags.ContainsKey(equipmentCode) == false)
                            conDictags[equipmentCode] = new();

                        conDictags[equipmentCode][name] = taginfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }

        public override bool connect()
        {// PLC 연결
            if (comm == null)
                comm = new(ip, port);
            else if (comm.IsConnected)
                return false;

            return comm.Open();
        }

        public override bool disConnect()
        {// PLC 연결 해제
            if (comm == null)
                return true;

            var returnVal = comm.Close();
            comm = null;

            return returnVal;
        }

        public override Tuple<bool, Dictionary<string, Dictionary<string, object>>> readAllData() 
        {// 모든 설정된 Tag 데이터 읽기 [스캔 여부 허용된 것만]
            Dictionary<string, List<AddrInfo>> dicTags = new();
            Dictionary<string, Dictionary<string, object>> returnValue = new();

            try
            {
                var lstScan = lstTagInfo.Where(x => x.isScan == true);
                foreach (var tagInfo in lstScan) // 수집 여부 허용된 항목 주소만 데이터 읽기
                {
                    string containKey = tagInfo.memoryName;

                    string equipName = tagInfo.equipmentCode;
                    string tagName = tagInfo.name;
                    int addr = Convert.ToInt32(tagInfo.addr);
                    int length = tagInfo.size;
                    int bitAddr = Convert.ToInt32(tagInfo.bitAddr);
                    string memoryType = tagInfo.memoryType;

                    switch (memoryType) // 연속 읽기에 따른 주소 변환
                    {
                        case MemoryTypeList.X:
                            containKey += MemoryTypeList.X;
                            break;
                        case MemoryTypeList.B:
                            containKey += MemoryTypeList.B;
                            break;
                        case MemoryTypeList.W:
                            containKey += MemoryTypeList.B;
                            addr *= 2;
                            length *= 2;
                            break;
                        case MemoryTypeList.D:
                            containKey += MemoryTypeList.B;
                            addr *= 4;
                            length *= 4;
                            break;
                        default:
                            continue;
                    }

                    if (dicTags.ContainsKey(containKey))
                        dicTags[containKey].Add(new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipName}.{tagName}" });
                    else
                        dicTags.TryAdd(containKey, new List<AddrInfo>() { new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipName}.{tagName}" } });

                    if (returnValue.ContainsKey(equipName))
                        returnValue[equipName][tagName] = null;
                    else
                        returnValue.TryAdd(equipName, new() { [tagName] = null });
                }

                foreach(var memoryName in dicTags.Keys) // 비트 , (바이트,워드,더블워드)[레지스터] 타입 구분 처리 - 통신
                {
                    dicTags[memoryName] = dicTags[memoryName].OrderBy(x => x.bitAddr).OrderBy(x => x.addr).ToList();

                    var memoryType = memoryName.Substring(1, 1);
                    switch (memoryType)
                    {
                        case MemoryTypeList.X:
                            if (comm.ReadBits(memoryName, dicTags[memoryName], returnValue) == false)
                                Trace.WriteLine($"FEnet_TCP, memory : {memoryName} ReadBits Fail, Check ErrorCode or Message");
                            break;
                        case MemoryTypeList.B:
                            if (comm.ReadRegisters(memoryName, dicTags[memoryName], returnValue) == false)
                                Trace.WriteLine($"FEnet_TCP, memory : {memoryName} ReadRegisters Fail, Check ErrorCode or Message");
                            break;
                        default:
                            Trace.WriteLine($"FEnet_TCP, memory : {memoryName} MemoryType Wrong, Use MemoryType 'X,B,W,D'");
                            break;
                    }
                }

                foreach (var tagInfo in lstScan) // 반환 데이터 처리 (hex string 변환)
                {
                    string equipName = tagInfo.equipmentCode;
                    string tagName = tagInfo.name;
                    string memoryType = tagInfo.memoryType;

                    if (returnValue[equipName][tagName] == null)
                        continue;

                    string data = string.Empty;

                    switch (memoryType)
                    {
                        case MemoryTypeList.X:
                            data = BytesConvertor.BitsToHexString(tagName, returnValue[equipName][tagName]);
                            break;
                        case MemoryTypeList.B:
                        case MemoryTypeList.W:
                        case MemoryTypeList.D:
                            data = BytesConvertor.BytesToHexString(tagName, returnValue[equipName][tagName]);
                            break;
                    }

                    returnValue[equipName][tagName] = new Dictionary<string, string>()
                    {
                        ["value"] = data,
                        ["datatype"] = tagInfo.dataType,
                        ["ratio"] = tagInfo.ratio
                    };
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return Tuple.Create(false, returnValue);
            }

            return Tuple.Create(true, returnValue);
        }
        
        public override bool reads(Dictionary<string, Dictionary<string, object>> dicData) // 요청 Tag 데이터 읽기
        {
            Dictionary<string, List<AddrInfo>> dicTags = new(); // key : memoryName / value : addr, lenght ...

            try
            {
                // setting
                foreach (var equipName in dicData.Keys)
                {
                    if (conDictags.ContainsKey(equipName) == false)
                    {
                        foreach (var tagName in dicData[equipName].Keys)
                        {
                            dicData[equipName][tagName] = null;
                        }
                    }
                    else
                    {
                        foreach (var tagName in dicData[equipName].Keys)
                        {
                            if (conDictags[equipName].ContainsKey(tagName) == false)
                            {
                                dicData[equipName][tagName] = null;
                                continue;
                            }

                            var taginfo = conDictags[equipName][tagName];

                            string containKey = taginfo.memoryName;
                            int addr = Convert.ToInt32(taginfo.addr);
                            int length = taginfo.size;
                            int bitAddr = Convert.ToInt32(taginfo.bitAddr);
                            string memoryType = taginfo.memoryType;

                            switch (memoryType)
                            {
                                case MemoryTypeList.X:
                                    containKey += MemoryTypeList.X;
                                    break;
                                case MemoryTypeList.B:
                                    containKey += MemoryTypeList.B;
                                    break;
                                case MemoryTypeList.W:
                                    {
                                        containKey += MemoryTypeList.B;
                                        addr *= 2;
                                        length *= 2;
                                    }
                                    break;
                                case MemoryTypeList.D:
                                    {
                                        containKey += MemoryTypeList.B;
                                        addr *= 4;
                                        length *= 4;
                                    }
                                    break;
                                default:
                                    continue;

                            }

                            if (dicTags.ContainsKey(containKey))
                                dicTags[containKey].Add(new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipName}.{tagName}" });
                            else
                                dicTags.TryAdd(containKey, new List<AddrInfo>() { new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipName}.{tagName}" } });

                            dicData[equipName][tagName] = null;
                        }
                    }
                }

                // 통신
                foreach (var memoryName in dicTags.Keys)
                {
                    dicTags[memoryName] = dicTags[memoryName].OrderBy(x => x.bitAddr).OrderBy(x => x.addr).ToList();

                    var memoryType = memoryName.Substring(1, 1);
                    switch (memoryType)
                    {
                        case MemoryTypeList.X:
                            if (comm.ReadBits(memoryName, dicTags[memoryName], dicData) == false)
                                Trace.WriteLine($"FEnet_TCP, MemoryName : {memoryName} ReadBits Fail, Check ErrorCode or Message");
                            break;
                        case MemoryTypeList.B:
                            if (comm.ReadRegisters(memoryName, dicTags[memoryName], dicData) == false)
                                Trace.WriteLine($"FEnet_TCP, MemoryName : {memoryName}, ReadRegisters Fail, Check ErrorCode or Message");
                            break;
                        default:
                            break;
                    }
                }

                // 변환
                bool onceTrueCheck = false;
                foreach (var equipName in dicData.Keys)
                {
                    if (conDictags.ContainsKey(equipName) == false)
                        continue;

                    foreach (var tagName in dicData[equipName].Keys)
                    {
                        if (conDictags[equipName].ContainsKey(tagName) == false)
                            continue;
                        else if (dicData[equipName][tagName] == null)
                            continue;

                        onceTrueCheck = true;
                        string memoryType = conDictags[equipName][tagName].memoryType;

                        string data = string.Empty;
                        var tagInfo = conDictags[equipName][tagName];
                        
                        switch (memoryType)
                        {
                            case MemoryTypeList.X:
                                data = BytesConvertor.BitsToHexString(tagName, dicData[equipName][tagName]);
                                break;
                            case MemoryTypeList.B:
                            case MemoryTypeList.W:
                            case MemoryTypeList.D:
                                data = BytesConvertor.BytesToHexString(tagName, dicData[equipName][tagName]);
                                break;
                        }

                        dicData[equipName][tagName] = new Dictionary<string, string>()
                        {
                            ["value"] = data,
                            ["datatype"] = tagInfo.dataType,
                            ["ratio"] = tagInfo.ratio
                        };
                    }
                }

                return onceTrueCheck;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }
        }

        public override bool read(string equipmentCode, string tagName, ref object data) // 요청 단일 Tag 데이터 읽기
        {
            try
            {
                if (conDictags.ContainsKey(equipmentCode))
                {
                    Trace.WriteLine($"FEnet_TCP, equipment : {equipmentCode} is not Registed");
                    data = null;
                    return false;
                }
                else if (conDictags[equipmentCode].ContainsKey(tagName))
                {
                    Trace.WriteLine($"FEnet_TCP, equipment : {equipmentCode}, tagName : {tagName} is not Registed");
                    data = null;
                    return false;
                }

                TagInfo tagInfo = conDictags[equipmentCode][tagName];

                int addr = Convert.ToInt32(tagInfo.addr);
                int length = tagInfo.size;
                int bitAddr = Convert.ToInt32(tagInfo.bitAddr);
                string memoryName = tagInfo.memoryName;
                string memoryType = tagInfo.memoryType;

                switch (memoryType)
                {
                    case MemoryTypeList.X:
                        memoryName += MemoryTypeList.X;
                        break;
                    case MemoryTypeList.B:
                        memoryName += MemoryTypeList.X;
                        break;
                    case MemoryTypeList.W:
                        memoryName += MemoryTypeList.X;
                        addr *= 2;
                        length *= 2;
                        break;
                    case MemoryTypeList.D:
                        addr *= 4;
                        length *= 4;
                        break;
                    default:
                        Trace.WriteLine($"FEnet_TCP, equipment : {equipmentCode}, tagName : {tagName} is MemoryType Wrong");
                        data = null;
                        return false;
                }

                AddrInfo addrInfo = new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipmentCode}.{tagName}" };
                memoryType = memoryName.Substring(1, 1);
                object readObj = null;

                string readData = string.Empty;

                // 통신 -> 변환
                switch (memoryType)
                {
                    case MemoryTypeList.X:
                        if (comm.ReadBit(memoryName, addrInfo, ref readObj) == false)
                        {
                            Trace.WriteLine($"FEnet_TCP, MemoryName : {memoryName}, ReadBit Return False");
                            return false;
                        }

                        if (readObj != null)
                            readData = BytesConvertor.BitsToHexString(tagName, readObj);
                        break;
                    case MemoryTypeList.B:
                        if (comm.ReadRegister(memoryName, addrInfo, ref readObj) == false)
                        {
                            Trace.WriteLine($"FEnet_TCP, emoryName : {memoryName}, ReadRegister Return False");
                            return false;
                        }

                        if (readObj != null)
                            readData = BytesConvertor.BytesToHexString(tagName, readObj);
                        break;
                }

                data = new Dictionary<string, string>()
                {
                    ["value"] = readData,
                    ["datatype"] = tagInfo.dataType,
                    ["ratio"] = tagInfo.ratio
                };
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }
        
        public override bool writes(Dictionary<string, Dictionary<string, object>> dicData) // 요청 Tag 데이터 쓰기
        {
            Dictionary<string, List<AddrInfo>> dicTags = new Dictionary<string, List<AddrInfo>>(); // key : memoryName , value : addr, length, (byte[])data, ....

            try
            {
                foreach (var equipName in dicData.Keys)
                {
                    if (conDictags.ContainsKey(equipName) == false)
                    {
                        Trace.WriteLine($"FEnet_TCP, equipment : {equipName} is not Regist");
                        return false;
                    }

                    // 태그 저장
                    foreach (var tagName in dicData[equipName].Keys)
                    {
                        if (conDictags[equipName].ContainsKey(tagName) == false)
                        {
                            Trace.WriteLine($"FEnet_TCP, equipment : {equipName}, tagName : {tagName} is not Regist");
                            return false;
                        }

                        TagInfo tagInfo = conDictags[equipName][tagName];

                        int addr = Convert.ToInt32(tagInfo.addr);
                        int length = tagInfo.size;
                        int bitAddr = Convert.ToInt32(tagInfo.bitAddr);
                        string memoryName = tagInfo.memoryName;

                        byte[] data;
                        if (dicData[equipName][tagName] is byte[])
                            data = dicData[equipName][tagName] as byte[];
                        else
                        {
                            var containType = tagInfo.memoryType;
                            switch (containType)
                            {
                                case MemoryTypeList.X:
                                    memoryName += MemoryTypeList.X;
                                    break;
                                case MemoryTypeList.B:
                                    memoryName += MemoryTypeList.B;
                                    break;
                                case MemoryTypeList.W:
                                    memoryName += MemoryTypeList.B;
                                    addr *= 2;
                                    length *= 2;
                                    break;
                                case MemoryTypeList.D:
                                    memoryName += MemoryTypeList.B;
                                    addr *= 4;
                                    length *= 4;
                                    break;
                                default:
                                    return false;

                            }

                            // 데이터 변환 및 요청 데이터 확인
                            if (containType == MemoryTypeList.X)
                                data = BytesConvertor.HexStringToBits(tagName, length, dicData[equipName][tagName]);
                            else
                                data = BytesConvertor.HexStringToBytes(tagName, length, dicData[equipName][tagName]);
                        }

                        if (dicTags.ContainsKey(memoryName))
                            dicTags[memoryName].Add(new() { addr = addr, length = length, bitAddr = bitAddr, tagName = tagName, data = data });
                        else
                            dicTags[memoryName] = new List<AddrInfo>() { new() { addr = addr, length = length, bitAddr = bitAddr, tagName = tagName, data = data } };
                    }
                }

                // 통신
                foreach (var memoryName in dicTags.Keys)
                {
                    dicTags[memoryName] = dicTags[memoryName].OrderBy(x => x.bitAddr).OrderBy(x => x.addr).ToList();

                    List<byte[]> lstBytes = new();
                    foreach (var addrInfo in dicTags[memoryName])
                    {
                        lstBytes.Add(addrInfo.data);
                    }

                    switch (memoryName.Substring(1, 1))
                    {
                        case MemoryTypeList.X:
                            if (comm.WriteBits(memoryName, dicTags[memoryName], lstBytes) == false)
                            {
                                Trace.WriteLine($"FEnet_TCP, MemoryName : {memoryName}, WriteBits Return False");
                                return false;
                            }
                            break;
                        case MemoryTypeList.B:
                            if (comm.WriteRegisters(memoryName, dicTags[memoryName], lstBytes) == false)
                            {
                                Trace.WriteLine($"FEnet_TCP, MemoryName : {memoryName}, WriteRegister Return False");
                                return false;
                            }
                            break;
                        default:
                            return false;
                    }
                }
                
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }
        
        public override bool write(string equipmentCode, string tagName, object data) // 요청 단일 Tag 데이터 쓰기
        {
            if(conDictags.ContainsKey(equipmentCode) == false)
            {
                Trace.WriteLine($"FEnet_TCP, equipment : {equipmentCode} is not Regist");
                return false;
            }
            else if(conDictags[equipmentCode].ContainsKey(tagName) == false)
            {
                Trace.WriteLine($"FEnet_TCP, equipment : {equipmentCode} , tagName : {tagName} is not Regist");
                return false;
            }

            try
            {
                TagInfo tagInfo = conDictags[equipmentCode][tagName];

                int addr = Convert.ToInt32(tagInfo.addr);
                int length = tagInfo.size;
                int bitAddr = Convert.ToInt32(tagInfo.bitAddr);
                string memoryName = tagInfo.memoryName;

                AddrInfo addrInfo = new() { addr = addr, length = length, bitAddr = bitAddr, tagName = $"{equipmentCode}.{tagName}" };

                byte[] dataBytes;
                if (data is byte[])
                    dataBytes = (data as byte[]);
                else
                {
                    var containType = tagInfo.memoryType;
                    switch (containType)
                    {
                        case MemoryTypeList.X:
                            memoryName += MemoryTypeList.X;
                            break;
                        case MemoryTypeList.B:
                            memoryName += MemoryTypeList.B;
                            break;
                        case MemoryTypeList.W:
                            memoryName += MemoryTypeList.B;
                            addrInfo.addr *= 2;
                            addrInfo.length *= 2;
                            break;
                        case MemoryTypeList.D:
                            memoryName += MemoryTypeList.B;
                            addrInfo.addr *= 4;
                            addrInfo.length *= 4;
                            break;
                        default:
                            Trace.WriteLine($"FEnet_TCP, equipment : {tagInfo.equipmentCode}, Memory : {tagInfo.memoryName}, tagName : {tagInfo.name}, Data cannot be written to this memory area.");
                            return false;
                    }

                    // 데이터 변환 및 요청 데이터 확인
                    if (containType == MemoryTypeList.X)
                        dataBytes = BytesConvertor.HexStringToBits(tagName, addrInfo.length, data);
                    else
                        dataBytes = BytesConvertor.HexStringToBytes(tagName, addrInfo.length, data);
                }

                // 통신
                switch (memoryName.Substring(1, 1))
                {
                    case MemoryTypeList.X:
                        if (comm.WriteBit(memoryName, addrInfo, dataBytes) == false)
                        {
                            Trace.WriteLine($"FEnet_TCP, equipment : {tagInfo.equipmentCode}, memoryName : {memoryName}, WriteBit Return False");
                            return false;
                        }
                        break;
                    case MemoryTypeList.B:
                        if (comm.WriteRegister(memoryName, addrInfo, dataBytes) == false)
                        {
                            Trace.WriteLine($"FEnet_TCP, equipment : {tagInfo.equipmentCode}, memoryName : {memoryName}, WriteRegister Return False");
                            return false;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return false;
            }

            return true;
        }
    }
}
