
cosdb file format specification (v1)

COSDB (CosmosOSDB) files are split into "sectors" which each are 4096 bytes in size. The first 24 bytes are reserved as the header meaning
that you will have 4072 bytes available for data.

# File Definition
Each sector header contains following information:
 1 Byte - SectorType (0x00 = DBHeader, 0x01 = TablesTable, 0x02=TableHeader, 0x03=TableDataSector)
 17 Bytes - Custom header information per sector type (I actually made that pretty unused too, oops lol)
 6 Bytes - Reserved for future

The first sector is ALWAYS a DBHeader sector (0x00)
 16 Bytes - COSDB Magic (COSMOSOSDBFILEv1)
 4 Bytes - "Tables Table" Start Sector Id (table of db tables)
 Rest - Reserved

Sector Type 0x01 - TablesTable
This is the table of tables. It can consist of as many sectors as needed.
 4 Bytes - Previous Sector Id (if applicable)
 4 Bytes - Next Sector Id (if applicable)
 8 Bytes - Reserved for future
 4056 Bytes - List of TablesTableEntrys @ 16 Bytes (max 338)

Sector Type 0x02 - TableHeader
This is the data required for tables. Its limited to 1 sector to reduce implementation complexity.
 8 Bytes - Table Name
 4 Bytes - First Data Sector Id
 4060 Bytes - TableFieldDefs @ 16 Bytes (max 253)

Sector Type 0x03 - TableDataSector
TableDataSectors are technically split into a bunch of sub sectors of 32 bytes which each store the data for exactly one row at most - which means that each row is at least as big as 32 bytes.
 4 Bytes - Previous Sector Id
 4 Bytes - Next Sector Id
 4064 Bytes - TableDataSubSector @ 32 Bytes (max 127)

Structural Type TablesTableEntry
 8 Bytes - Table Name
 4 Bytes - TableHeader Sector Id

Structural Type TableFieldDef
 2 Bytes - DataType
 12 Bytes - Name
 2 Bytes - TableFieldDefFlags (unused/reserved for now)

Structural Type TableDataSubSector
 1 Byte - Previous Sub Sector Id
 1 Byte - Next Sub Sector Id
 24 Bytes - TableRowFieldData @ 8 Bytes (max. 3)

Structural Type TableRowFieldData = long
 8 Bytes - The data for the field (always 8 Bytes!)

# Supported Data Types
Even though TableRowFieldData only supports exactly 8 byte long entries, other types are still supported for direct internal conversion.
  Byte
  Short
  UShort
  Int
  UInt
  Long
  ULong
  Float
  Double
  ANSIString / UTF8String / UTF16String (Special, see [Special - Strings](#special---strings))

### Special - Strings
As strings are commonly longer than 8 bytes, the database just stores the offset to the string which is stored in a seperate file, "strings.cdbbin".
When strings are deleted, they are replaced by \0 bytes but never fully removed from the file. A clean up method might be implemented, however, this would need to adjust all string entries accordingly.