using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PLCSimulation
{
    class PLCMemory : INotifyPropertyChanged
    {
        string address;
        ushort zero;
        ushort one;
        ushort two;
        ushort three;
        ushort four;
        ushort five;
        ushort six;
        ushort seven;
        ushort eight;
        ushort nine;

        public string Address
        {
            get { return address; }
            set
            {
                if (value != address)
                {
                    address = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Zero
        {
            get { return zero; }
            set
            {
                if (value != zero)
                {
                    zero = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort One
        {
            get { return one; }
            set
            {
                if (value != one)
                {
                    one = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Two
        {
            get { return two; }
            set
            {
                if (value != two)
                {
                    two = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Three
        {
            get { return three; }
            set
            {
                if (value != three)
                {
                    three = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Four
        {
            get { return four; }
            set
            {
                if (value != four)
                {
                    four = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Five
        {
            get { return five; }
            set
            {
                if (value != five)
                {
                    five = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Six
        {
            get { return six; }
            set
            {
                if (value != six)
                {
                    six = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Seven
        {
            get { return seven; }
            set
            {
                if (value != seven)
                {
                    seven = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Eight
        {
            get { return eight; }
            set
            {
                if (value != eight)
                {
                    eight = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ushort Nine
        {
            get { return nine; }
            set
            {
                if (value != nine)
                {
                    nine = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ushort GetMemoryData(int index)
        {
            switch (index)
            {
                case 0:
                    return zero;
                case 1:
                    return one;
                case 2:
                    return two;
                case 3:
                    return three;
                case 4:
                    return four;
                case 5:
                    return five;
                case 6:
                    return six;
                case 7:
                    return seven;
                case 8:
                    return eight;
                case 9:
                    return nine;
                default:
                    throw new IndexOutOfRangeException("index range 0-9");
            }
        }

        public bool SetMemoryData(int index, ushort data)
        {
            try
            {
                switch (index)
                {
                    case 0:
                        Zero = data;
                        break;
                    case 1:
                        One = data;
                        break;
                    case 2:
                        Two = data;
                        break;
                    case 3:
                        Three = data;
                        break;
                    case 4:
                        Four = data;
                        break;
                    case 5:
                        Five = data;
                        break;
                    case 6:
                        Six = data;
                        break;
                    case 7:
                        Seven = data;
                        break;
                    case 8:
                        Eight = data;
                        break;
                    case 9:
                        Nine = data;
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;

        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") 
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    class PLCProtocolType
    {
        public const string FENET = "FEnet";
        public const string MODBUS = "Modbus";

        public const string TCP = "TCP";
        public const string UDP = "UDP";
    }

    [Serializable]
    class SaveLoadData
    {
        public string Port;
        public string Mode;
        public string IP;
        public string Protocol;
        public int FormIndex;
        public int MSize;
        public int DSize;
        public int KSize;
        public int LSize;
        public int CoilSize;
        public int InputSize;
        public int HoldingRegisterSize;
        public int InputRegisterSize;
    }
}
