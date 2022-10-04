using System;
using System.Collections.Generic;

namespace DriverInterface
{
    public abstract class CommonDriver
    {
        // connect Info
        string ipAddress;
        int portNumber;
        // PLC Mapping Info
        List<TagInfo> lstTagInfoClass;
        // DriverInfo
        DriverInfo driverInfoClass;

        #region Attribute
        public string ip { get => ipAddress; set => ipAddress = value; }
        public int port { get => portNumber;  set => portNumber = value; }
        public List<TagInfo> lstTagInfo { get { if (lstTagInfoClass == null) { lstTagInfoClass = new(); } return lstTagInfoClass; } }
        public DriverInfo driverInfo { get { if (driverInfoClass == null) { driverInfoClass = new(); } return driverInfoClass; } }
        #endregion

        #region Properties
        public abstract System.Net.Sockets.Socket sock { get; }
        #endregion

        #region Abstract Method
        public abstract DriverInfo getDriverInfo();

        public abstract bool setTagInfo(string path);

        public abstract bool connect();

        public abstract bool disConnect();

        public abstract Tuple<bool, Dictionary<string, Dictionary<string,object>>> readAllData();

        public abstract bool reads(Dictionary<string, Dictionary<string, object>> dicData);

        public abstract bool read(string equipmentCode, string tagName, ref object data);
        
        public abstract bool writes(Dictionary<string, Dictionary<string, object>> dicData);

        public abstract bool write(string equipmentCode, string tagName, object data);
        #endregion

        #region Class
        public class TagInfo
        {
            public string equipmentCode; // 설비 명
            public string name; // 주소 태그 명 (명칭)
            public string addr;
            public string bitAddr;
            public int size = 1;
            public string memoryName;
            public string memoryType;
            public bool isHex;
            public string dataType;
            public string ratio;
            public bool isScan;
        }

        public class DriverInfo
        { // PLC Driver 정보 정의
            public string version;
            public string provider;
            public string releaseDate;
            public string protocolType;
            public string driverName;
            public string manufactureData;
        }
        #endregion
    }
}
