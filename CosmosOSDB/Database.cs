using System.Runtime.InteropServices;

namespace CosmosOSDB
{
    public unsafe class Database
    {
        public string DbPath { get; init; }

        private UnknownSector* dbBuffer;
        private byte[] bufArray;

        public Dictionary<string, ManagedTable> Tables { get; set; }


        public Database(string path)
        {
            if (!File.Exists(path))
            {
                // TODO: Allow creation of empty DBs
            }

            DbPath = path;
            bufArray = File.ReadAllBytes(path);
            fixed (byte* ptr = &bufArray[0]) {
                dbBuffer = ((UnknownSector*)(ptr));
            }

            Initiliaze();
        }

        public void Initiliaze()
        {
            DBHeader header = Marshal.PtrToStructure<DBHeader>((IntPtr)dbBuffer);

            if (header.Magic != "COSMOSOSDBFILEv1") throw new Exception("Invalid file!");

            ReadTablesTable(header.TablesTableSectorId);
        }

        public void ReadTablesTable(int sectorId)
        {
            TablesTable* tablesTable = (TablesTable*)GetSector(sectorId);



            if (tablesTable->NextSector != 0) ReadTablesTable(tablesTable->NextSector);
        }

        public UnknownSector* GetSector(int sectorId)
        {
            return (dbBuffer + sectorId);
        }
    }

    public class ManagedTable
    {
        public string Name {
            get
            {
                return _name;
            }
            set
            {
                if (value.Length > 8) throw new Exception("Table names may not exceed 8 chars.");
                _name = value;
            }
        }

        private string _name;

        public List<ManagedField> Fields { get; init; } = new();
        public List<ManagedRow> Rows { get; init; } = new();
    }

    public class ManagedField
    {
        public DataType DataType { get; set; }
        public string Name { get; set; }
    }

    public class ManagedRow
    {
        public List<long> Data { get; set; }

        public object GetAs(DataType dt, int index)
        {
            var data = Data[index];

            switch(dt)
            {
                case DataType.Byte:
                case DataType.Short:
                case DataType.UShort:
                case DataType.Int:
                case DataType.UInt:
                case DataType.Long:
                case DataType.ULong:
                    return data;
                case DataType.Float:
                    return BitConverter.ToSingle(BitConverter.GetBytes(data));
                case DataType.Double:
                    return BitConverter.ToDouble(BitConverter.GetBytes(data));
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public enum SectorType : byte
    {
        DBHeader = 0x00,
        TablesTable = 0x01,
        TableHeader = 0x02,
        TableDataSector = 0x03
    }

    [StructLayout(LayoutKind.Sequential, Size = 24)]
    public unsafe struct DBSectorHeader
    {
        public SectorType SectorType;
        public fixed byte CustomHeaderData[17];
        public fixed byte Rsv[6];
    }

    [StructLayout(LayoutKind.Sequential, Size = 4096)]
    public unsafe struct UnknownSector
    {
        public DBSectorHeader Header;
        public fixed byte Data[4072];
    }

    [StructLayout(LayoutKind.Sequential, Size = 4096)]
    public unsafe struct DBHeader
    {
        public DBSectorHeader Header;

        public fixed char Magic[16];

        public int TablesTableSectorId;
    }

    [StructLayout(LayoutKind.Sequential, Size = 4096)]
    public unsafe struct TablesTable
    {
        public DBSectorHeader Header;

        public int PrevSector;
        public int NextSector;
        public long Rsv;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 338)]
        public fixed TablesTableEntry Entries[338];
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct TableHeader
    {
        public DBSectorHeader Header;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string TableName;

        public int FirstDataSectorId;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 253)]
        public TableFieldDef[] Fields;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct TableDataSector
    {
        public int PrevSector, NextSector;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 127)]
        public TableDataSubSector[] SubSectors;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct TableDataSubSector
    {
        public byte PrevSubSector, NextSubSector;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public long[] Data;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct TablesTableEntry
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string TableName;

        public int SectorId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public unsafe struct TableFieldDef
    {
        public DataType DataType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string FieldName;

        public TableFieldDefFlags Flags;
    }

    [Flags]
    public enum TableFieldDefFlags : UInt16
    {

    }

    public enum DataType : UInt16
    {
        Byte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,
        Float,
        Double,
        ANSIString,
        UTF8String,
        UTF16String
    }
}