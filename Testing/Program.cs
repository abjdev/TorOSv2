using System.Runtime.InteropServices;

namespace Testing {
    internal class Program {
        static void Main(string[] args) {
            Console.WriteLine(Path.GetFullPath("0:\\os\\.."));
        }
    }

    internal struct BitField8 {
        public byte data;

        public BitField8() {
            data = 0b00000000;
        }

        public BitField8(byte data) {
            this.data = data;
        }

        public bool this[int index] {
            get {
                if (index > 8) return false;
                byte bitIndex = (byte)(index % 8);
                byte bitIndexer = (byte)(0b10000000 >> bitIndex);
                if ((data & bitIndexer) > 0) {
                    return true;
                }
                return false;
            }

            set {
                if (index > 8) return;
                byte bitIndex = (byte)(index % 8);
                byte bitIndexer = (byte)(0b10000000 >> bitIndex);
                data &= (byte)~bitIndexer;
                if (value) {
                    data |= bitIndexer;
                }
            }
        }
    }

    internal struct BitField4x8 {
        public BitField8 bf0, bf1, bf2, bf3;

        //public bool this[int index] {
        //    get {
        //        int bfIdx = index / 8;
        //
        //       switch(bfIdx) {
        //            case 0:
        //                bf
        //        }
        //    }
        //}

        public BitField4x8() {
            this.bf0 = new();
            this.bf1 = new();
            this.bf2 = new();
            this.bf3 = new();
        }

        public BitField4x8(int data) {
            byte[] arr = BitConverter.GetBytes(data);
            this.bf0 = new(arr[0]);
            this.bf1 = new(arr[1]);
            this.bf2 = new(arr[2]);
            this.bf3 = new(arr[3]);
        }

        public int ToInt() {
            return BitConverter.ToInt32(new byte[] { bf0.data, bf1.data, bf2.data, bf3.data });
        }
    }
}