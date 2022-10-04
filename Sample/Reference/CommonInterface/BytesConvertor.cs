using System;
using System.Collections.Generic;
using System.Text;

namespace CommonInterface
{
    public class BytesConvertor
    {
        #region HexString
        public static byte[] HexStringToBytes(string tagName, int size, object data)
        {// H / L
            byte[] dataBytes = new byte[size];

            if (data is string str)
            {
                if (str.Length % 2 > 0)
                    str = "0" + str;

                if (str.Length != size * 2)
                {
                    throw new Exception($"tagName : {tagName}, Size not matched");
                }

                for (int i = 0; i < size * 2; i += 2)
                {
                    string byteStr = str.Substring(i, 2);
                    dataBytes[size - (i / 2 + 1)] = (byte)HexToDec(byteStr);
                }

                return dataBytes;
            }

            System.Diagnostics.Trace.WriteLine($"tagName : {tagName}, Convert False");
            return null;
        }
        
        public static byte[] HexStringToBits(string tagName, int size, object data)
        {
            byte[] dataBytes = new byte[size];

            if(data is string str)
            {
                if (str.Length != size)
                    throw new Exception($"tagName : {tagName}, Size not matched ! (Size : {size} , Data Size : {str.Length}");

                for (int i = 0; i < size; i++)
                {
                    string byteStr = str.Substring(i, 1);
                    dataBytes[size - (i + 1)] = (byte)HexToDec(byteStr);
                }

                return dataBytes;
            }

            throw new Exception($"tagName : {tagName}, Convert Fail");
            return null;
        }

        public static string BytesToHexString(string tagName, object data)
        {
            if (data is byte[] bytes)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    sb.Append(bytes[i].ToString("X2"));
                }

                return sb.ToString() + "h";
            }

            System.Diagnostics.Trace.WriteLine($"tagName : {tagName}, Convert False");
            return null;
        }

        public static string BitsToHexString(string tagName, object data)
        {
            if(data is byte[] bytes)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = bytes.Length - 1; i >=  0; i--)
                {
                    sb.Append(bytes[i].ToString("X"));
                }

                return sb.ToString() + "b";
            }

            System.Diagnostics.Trace.WriteLine($"tagName : {tagName}, Convert False");
            return null;
        }
        #endregion

        #region Dec
        public static int HexToDec(string str)
        {
            string strHex = string.Format("{0:X6}", str.ToUpper());
            int address = Convert.ToInt32(strHex, 16);

            return address;
        }
        #endregion

    }
}
