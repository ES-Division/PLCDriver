using Newtonsoft.Json;

namespace CommonInterface
{
    public class Definition
    {
        public const string STRING = "string";
        public const string BOOL = "bool";
        public const string SHORT = "short";
        public const string USHORT = "ushort";
        public const string INT = "int";
        public const string UINT = "uint";
        public const string FLOAT = "float";
        public const string LONG = "long";
        public const string ULONG = "ulong";
        public const string DOUBLE = "double";

        public const string DllExtension = ".dll";
        public const string TEXT = ".txt";
        public const string JSON = ".json";
        public const string CONFIG = "config";
        public const string CSV = ".csv";
        public const string IP = "ip";
        public const string PORT = "port";
    }

    public class AddrInfo
    {
        public int addr;
        public int length;
        public int? bitAddr;
        public string tagName;
        public byte[] data;
    }
}
