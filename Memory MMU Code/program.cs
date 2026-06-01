using System;
using System.Collections.Generic;

class PageTableEntry
{
    public int VPN { get; set; }
    public int Dirty { get; set; }
    public int Resident { get; set; }
    public int PPN { get; set; }
}

class TLBEntry
{
    public int VPN { get; set; }
    public int Resident { get; set; }
    public int Dirty { get; set; }
    public int PPN { get; set; }
}

class Program
{
    static void Main()
    {
        Task1_PageMapping();
        Task2_PageFrames();
        Task3_AddressTranslation();
        Task4_TLBCalculations();
        Task4_2_TLBAddressExamples();
        Task5_MemoryInterface();
    }

    static void Task1_PageMapping()
    {
        Console.WriteLine("===== TASK 1: PAGE MAPPING =====");

        int vpnBits = 4;
        int offsetBits = 8;

        int numberOfVirtualPages = (int)Math.Pow(2, vpnBits);
        int bytesPerPage = (int)Math.Pow(2, offsetBits);

        int virtualAddress = 0x2C8;
        int vpn = virtualAddress >> offsetBits;
        int offset = virtualAddress & 0xFF;

        Dictionary<int, PageTableEntry> pageTable = new Dictionary<int, PageTableEntry>
        {
            { 2, new PageTableEntry { VPN = 2, Dirty = 0, Resident = 1, PPN = 4 } }
        };

        PageTableEntry entry = pageTable[vpn];

        Console.WriteLine($"Number of virtual pages = 2^{vpnBits} = {numberOfVirtualPages}");
        Console.WriteLine($"Bytes per page = 2^{offsetBits} = {bytesPerPage} bytes");
        Console.WriteLine($"Virtual address = 0x{virtualAddress:X}");
        Console.WriteLine($"VPN = 0x{vpn:X}");
        Console.WriteLine($"Offset = 0x{offset:X}");
        Console.WriteLine($"Resident bit = {entry.Resident}");
        Console.WriteLine($"PPN = {entry.PPN}");

        int physicalAddress = (entry.PPN << offsetBits) + offset;

        Console.WriteLine($"Physical address = PPN x page size + offset");
        Console.WriteLine($"Physical address = {entry.PPN} x {bytesPerPage} + {offset}");
        Console.WriteLine($"Physical address = {physicalAddress} decimal = 0x{physicalAddress:X}");
        Console.WriteLine();
    }

    static void Task2_PageFrames()
    {
        Console.WriteLine("===== TASK 2: PAGE FRAMES =====");

        int pageSize = 4096;

        Console.WriteLine("Each page frame is 4KB = 4096 bytes.");
        Console.WriteLine("The bottom 32KB of memory is divided into eight page frames.");
        Console.WriteLine();

        for (int frame = 0; frame < 8; frame++)
        {
            int start = frame * pageSize;
            int end = start + pageSize - 1;

            Console.WriteLine($"Frame {frame}: {start} - {end}");
        }

        Console.WriteLine();
        Console.WriteLine("Therefore:");
        Console.WriteLine("A = 32767");
        Console.WriteLine("B = 24575");
        Console.WriteLine("C = 16383");
        Console.WriteLine("D = 4095");
        Console.WriteLine();
    }

    static void Task3_AddressTranslation()
    {
        Console.WriteLine("===== TASK 3: 32-BIT ADDRESS TRANSLATION =====");

        string binaryAddress = "00000000000000000011000010110111";

        string vpnBinary = binaryAddress.Substring(0, 20);
        string offsetBinary = binaryAddress.Substring(20, 12);

        int vpn = Convert.ToInt32(vpnBinary, 2);
        int offset = Convert.ToInt32(offsetBinary, 2);

        Console.WriteLine($"Virtual address binary = {binaryAddress}");
        Console.WriteLine($"First 20 bits VPN = {vpnBinary} = {vpn}");
        Console.WriteLine($"Last 12 bits offset = {offsetBinary} = {offset} decimal = 0x{offset:X}");

        Dictionary<int, PageTableEntry> pageTable = new Dictionary<int, PageTableEntry>
        {
            { 0, new PageTableEntry { VPN = 0, Resident = 0, PPN = 7 } },
            { 1, new PageTableEntry { VPN = 1, Resident = 1, PPN = 1 } },
            { 2, new PageTableEntry { VPN = 2, Resident = 1, PPN = 2 } },
            { 3, new PageTableEntry { VPN = 3, Resident = 1, PPN = 3 } },
            { 4, new PageTableEntry { VPN = 4, Resident = 0, PPN = 1 } }
        };

        PageTableEntry entry = pageTable[vpn];

        Console.WriteLine($"Referenced virtual page = {vpn}");
        Console.WriteLine($"Present/Resident bit = {entry.Resident}");
        Console.WriteLine($"Page frame value = {Convert.ToString(entry.PPN, 2).PadLeft(3, '0')} binary = {entry.PPN} decimal");

        int physicalAddress = (entry.PPN * 4096) + offset;

        Console.WriteLine($"Physical address = PPN x page size + offset");
        Console.WriteLine($"Physical address = {entry.PPN} x 4096 + {offset}");
        Console.WriteLine($"Physical address = {physicalAddress} decimal = 0x{physicalAddress:X}");
        Console.WriteLine();
    }

    static void Task4_TLBCalculations()
    {
        Console.WriteLine("===== TASK 4.1: TLB CALCULATIONS =====");

        int virtualMemoryBits = 32;
        int physicalMemoryBits = 24;
        int pageSizeBits = 10;
        int vpnBits = 22;
        int ppnBits = 14;

        int physicalPages = (int)Math.Pow(2, physicalMemoryBits - pageSizeBits);
        int pageTableEntries = (int)Math.Pow(2, virtualMemoryBits - pageSizeBits);
        int bitsPerEntry = ppnBits + 2; // PPN + resident bit + dirty bit
        long totalPageTableBits = (long)pageTableEntries * bitsPerEntry;
        long totalPageTableBytes = totalPageTableBits / 8;
        long pagesOccupied = totalPageTableBytes / (long)Math.Pow(2, pageSizeBits);

        Console.WriteLine($"Physical memory = 2^{physicalMemoryBits} bytes");
        Console.WriteLine($"Page size = 2^{pageSizeBits} bytes");
        Console.WriteLine($"Pages in physical memory = 2^{physicalMemoryBits} / 2^{pageSizeBits} = {physicalPages}");
        Console.WriteLine($"Page table entries = 2^{virtualMemoryBits} / 2^{pageSizeBits} = 2^{vpnBits} = {pageTableEntries}");
        Console.WriteLine($"Bits per page table entry = PPN bits + resident bit + dirty bit = {ppnBits} + 2 = {bitsPerEntry}");
        Console.WriteLine($"Page table size = {totalPageTableBytes} bytes");
        Console.WriteLine($"Pages occupied by page table = {pagesOccupied}");
        Console.WriteLine();
    }

    static void Task4_2_TLBAddressExamples()
    {
        Console.WriteLine("===== TASK 4.2: TLB ADDRESS EXAMPLES =====");

        List<TLBEntry> tlb = new List<TLBEntry>
        {
            new TLBEntry { VPN = 0, Resident = 0, Dirty = 0, PPN = 7 },
            new TLBEntry { VPN = 6, Resident = 1, Dirty = 1, PPN = 2 },
            new TLBEntry { VPN = 1, Resident = 1, Dirty = 1, PPN = 9 },
            new TLBEntry { VPN = 3, Resident = 0, Dirty = 0, PPN = 5 }
        };

        TranslateWithTLB(0x1804, tlb);
        TranslateWithTLB(0x0FC, tlb);

        Console.WriteLine();
    }

    static void TranslateWithTLB(int virtualAddress, List<TLBEntry> tlb)
    {
        int pageSize = 1024;
        int vpn = virtualAddress / pageSize;
        int offset = virtualAddress % pageSize;

        Console.WriteLine($"Virtual address = 0x{virtualAddress:X}");
        Console.WriteLine($"VPN = {vpn}, Offset = 0x{offset:X}");

        TLBEntry? match = tlb.Find(entry => entry.VPN == vpn);

        if (match == null)
        {
            Console.WriteLine("TLB miss. The page table must be checked.");
        }
        else if (match.Resident == 0)
        {
            Console.WriteLine("TLB entry found, but resident bit is 0. This causes a page fault.");
        }
        else
        {
            int physicalAddress = (match.PPN * pageSize) + offset;
            Console.WriteLine($"TLB hit. PPN = {match.PPN}");
            Console.WriteLine($"Physical address = {match.PPN} x {pageSize} + {offset} = {physicalAddress} decimal = 0x{physicalAddress:X}");
        }

        Console.WriteLine();
    }

    static void Task5_MemoryInterface()
    {
        Console.WriteLine("===== TASK 5: MEMORY INTERFACE =====");

        Console.WriteLine("For 16 memory locations, the decoder needs 4 address input bits because 2^4 = 16.");
        Console.WriteLine("The memory interface also includes Write, Read and Enable control bits.");
        Console.WriteLine("Therefore, the combined address pattern uses 3 control bits + 4 decoder bits = 7 bits.");
        Console.WriteLine();
        Console.WriteLine("Read memory location with content FEAD:");
        Console.WriteLine("The content FEAD is at index A.");
        Console.WriteLine("Index A in binary = 1010.");
        Console.WriteLine("Read pattern = Write 0, Read 1, Enable 1, Index 1010.");
        Console.WriteLine("Combined address = 0111010.");
        Console.WriteLine();
        Console.WriteLine("Write to memory location with index C:");
        Console.WriteLine("Index C in binary = 1100.");
        Console.WriteLine("Write pattern = Write 1, Read 0, Enable 1, Index 1100.");
        Console.WriteLine("Combined address = 1011100.");
        Console.WriteLine();
    }
}