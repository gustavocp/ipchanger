namespace OTLandIPChanger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class TibiaClient
    {
        private static List<SignatureEntry> _mcSigs;
        private static readonly string[] CipsoftHostnames = new string[] { "login01.tibia.com", "login02.tibia.com", "login03.tibia.com", "login04.tibia.com", "login05.tibia.com", "tibia01.cipsoft.com", "tibia02.cipsoft.com", "tibia03.cipsoft.com", "tibia04.cipsoft.com", "tibia05.cipsoft.com", "test.tibia.com", "test.cipsoft.com", "tibia2.cipsoft.com", "tibia1.cipsoft.com", "server.tibia.com", "server2.tibia.com" };
        private static readonly string[] CipsoftRSAPubKeys = new string[] { "132127743205872284062295099082293384952776326496165507967876361843343953435544496682053323833394351797728954155097012103928360786959821132214473291575712138800495033169914814069637740318278150290733684032524174782740134357629699062987023311132821016569775488792221429527047321331896351555606801473202394175817", "124710459426827943004376449897985582167801707960697037164044904862948569380850421396904597686953877022394604239428185498284169068581802277612081027966724336319448537811441719076484340922854929273517308661370727105382899118999403808045846444647284499123164879035103627004668521005328367415259939915284902061793", "142996239624163995200701773828988955507954033454661532174705160829347375827760388829672133862046006741453928458538592179906264509724520840657286865659265687630979195970404721891201847792002125535401292779123937207447574596692788513647179235335529307251350570728407373705564708871762033017096809910315212883967" };
        public const int IPDistance = 0x70;
        private const string OpenTibiaRSAPubKey = "109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413";
        private const int PAGE_READWRITE = 4;
        public const int PortDistance = 100;

        static TibiaClient()
        {
            List<SignatureEntry> list = new List<SignatureEntry>();
            SignatureEntry item = new SignatureEntry {
                Bytes = new byte[] { 
                    0x80, 0xbd, 0x70, 0xf4, 0xff, 0xff, 0, 0x75, 0x40, 0x68, 0xd4, 0x77, 0x70, 0, 0x6a, 0, 
                    0x6a, 0, 0xff, 0x15, 12, 0x42, 0x6c, 0, 0x8b, 0x3d, 0xf4, 0x41, 0x6c, 0, 0xff, 0xd7, 
                    0x3d, 0xb7, 0, 0, 0, 0x74, 7, 0xff, 0xd7, 0x83, 0xf8, 5, 0x75, 0x1b
                 },
                Signature = "x?????xx?x????xxxxx?????x?????xxxxxxxx?xxxxxx?",
                Offset = 7
            };
            list.Add(item);
            SignatureEntry entry2 = new SignatureEntry {
                Bytes = new byte[] { 
                    0x8a, 0x45, 0xe7, 0x84, 0xc0, 0x75, 0x52, 0x68, 60, 4, 0x5d, 0, 0x6a, 0, 0x6a, 0, 
                    0xff, 0x15, 0xe4, 0xa2, 0x5b, 0, 0x89, 0x45, 0x98, 0x8b, 0x3d, 0x38, 0xa2, 0x5b, 0, 0xff, 
                    0xd7, 0x3d, 0xb7, 0, 0, 0, 0x74, 11, 0xff, 0xd7, 0x83, 0xf8, 5, 0x74, 4
                 },
                Signature = "x??xxx?x????xxxxx?????x??x?????xxxxxxxx?xxxxxx?",
                Offset = 5
            };
            list.Add(entry2);
            SignatureEntry entry3 = new SignatureEntry {
                Bytes = new byte[] { 
                    0x8a, 0x45, 0xeb, 0x84, 0xc0, 0x75, 0x5f, 0x6a, 0, 0x68, 0x30, 0xdb, 0x44, 0, 0xc7, 5, 
                    240, 0x45, 0x5f, 0, 0, 0, 0, 0, 0xff, 0x15, 0x74, 0xe4, 0x47, 0, 0xa1, 240, 
                    0x45, 0x5f, 0, 0x85, 0xc0, 0x75, 0x2c, 0x6a, 0x30, 0x68, 180, 0xda, 0x48, 0, 0x68, 0x2c, 
                    0xed, 0x48, 0, 0x6a, 0, 0xff, 0x15, 0x84, 0xe4, 0x47, 0
                 },
                Signature = "x??xxx?xxx????x????xxxxxx?????x????xxx?xxx????x????xxx?????",
                Offset = 5
            };
            list.Add(entry3);
            _mcSigs = list;
        }

        [DllImport("kernel32.dll")]
        private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        public static string EnsureConfigurationFolder(string version)
        {
            string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tibia"), version);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static IntPtr FindClient()
        {
            IntPtr ptr = FindWindow("tibiaclient", null);
            if (ptr == IntPtr.Zero)
            {
                ptr = FindWindow("tibiatestclient", null);
            }
            return ptr;
        }

        [DllImport("user32.dll", SetLastError=true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static IntPtr GetBaseAddress(IntPtr hProcess)
        {
            SYSTEM_INFO system_info;
            GetSystemInfo(out system_info);
            IntPtr lpMinimumApplicationAddress = system_info.lpMinimumApplicationAddress;
            MEMORY_BASIC_INFORMATION structure = new MEMORY_BASIC_INFORMATION();
            uint dwLength = (uint) Marshal.SizeOf(structure);
            while (lpMinimumApplicationAddress.ToInt64() < system_info.lpMaximumApplicationAddress.ToInt64())
            {
                if (!VirtualQueryEx(hProcess, lpMinimumApplicationAddress, out structure, dwLength))
                {
                    Console.WriteLine("Could not VirtualQueryEx {0} segment at {1}; error {2}", hProcess.ToInt64(), lpMinimumApplicationAddress.ToInt64(), Marshal.GetLastWin32Error());
                    return IntPtr.Zero;
                }
                if (((structure.Type == 0x1000000) && (structure.BaseAddress == structure.AllocationBase)) && ((structure.Protect & 0x100) != 0x100))
                {
                    IMAGE_DOS_HEADER image_dos_header = ReadUnmanagedStructure<IMAGE_DOS_HEADER>(hProcess, lpMinimumApplicationAddress);
                    if (image_dos_header.e_magic == 0x5a4d)
                    {
                        IntPtr lpAddr = new IntPtr(lpMinimumApplicationAddress.ToInt64() + (image_dos_header.e_lfanew + 4));
                        if ((ReadUnmanagedStructure<IMAGE_FILE_HEADER>(hProcess, lpAddr).Characteristics & 2) == 2)
                        {
                            return lpMinimumApplicationAddress;
                        }
                    }
                }
                long introduced7 = structure.BaseAddress.ToInt64();
                lpMinimumApplicationAddress = new IntPtr(introduced7 + structure.RegionSize.ToInt64());
            }
            return lpMinimumApplicationAddress;
        }

        private static List<long> GetLoginAddresses(Process process)
        {
            IntPtr baseAddress = process.MainModule.BaseAddress;
            if (baseAddress == IntPtr.Zero)
            {
                return null;
            }
            IntPtr dataAddress = IntPtr.Zero;
            byte[] data = GetSectionBytes(process.Handle, baseAddress, ".data", ref dataAddress);
            if (dataAddress == IntPtr.Zero)
            {
                return null;
            }
            return (from ipAddress in CipsoftHostnames
                select SearchBytes(data, Encoding.ASCII.GetBytes(ipAddress)) into loginAddrOffset
                where loginAddrOffset != -1
                select dataAddress.ToInt64() + loginAddrOffset).ToList<long>();
        }

        [DllImport("kernel32", SetLastError=true)]
        public static extern int GetProcessId(IntPtr hProcess);
        private static IMAGE_SECTION_HEADER GetSection(IntPtr hProcess, IntPtr baseAddress, string section)
        {
            IMAGE_DOS_HEADER image_dos_header = ReadUnmanagedStructure<IMAGE_DOS_HEADER>(hProcess, baseAddress);
            IntPtr lpAddr = new IntPtr(baseAddress.ToInt64() + (image_dos_header.e_lfanew + 4));
            IMAGE_FILE_HEADER image_file_header = ReadUnmanagedStructure<IMAGE_FILE_HEADER>(hProcess, lpAddr);
            lpAddr = new IntPtr(lpAddr.ToInt64() + (Marshal.SizeOf(typeof(IMAGE_FILE_HEADER)) + image_file_header.SizeOfOptionalHeader));
            for (int i = 0; i < image_file_header.NumberOfSections; i++)
            {
                IMAGE_SECTION_HEADER image_section_header = ReadUnmanagedStructure<IMAGE_SECTION_HEADER>(hProcess, lpAddr);
                lpAddr = new IntPtr(lpAddr.ToInt64() + Marshal.SizeOf(typeof(IMAGE_SECTION_HEADER)));
                for (int j = 0; j < 8; j++)
                {
                    if (section.Length == j)
                    {
                        return image_section_header;
                    }
                    if (section[j] != image_section_header.Name[j])
                    {
                        break;
                    }
                }
            }
            return new IMAGE_SECTION_HEADER();
        }

        private static byte[] GetSectionBytes(IntPtr hProcess, IntPtr baseAddress, string section, ref IntPtr sectionAddress)
        {
            IMAGE_SECTION_HEADER image_section_header = GetSection(hProcess, baseAddress, section);
            if (image_section_header.Name == null)
            {
                throw new Exception("Could not find section " + section);
            }
            sectionAddress = new IntPtr(baseAddress.ToInt64() + image_section_header.VirtualAddress);
            return ReadBytes(hProcess, sectionAddress, image_section_header.Misc);
        }

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] out SYSTEM_INFO lpSystemInfo);
        private static byte[] ReadBytes(IntPtr hProcess, IntPtr lpAddr, uint bytes)
        {
            byte[] lpBuffer = new byte[bytes];
            ReadProcessMemory(hProcess, lpAddr, lpBuffer, new UIntPtr(bytes), IntPtr.Zero);
            return lpBuffer;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);
        private static string ReadString(IntPtr hProcess, IntPtr lpAddr, uint maxLen = 0x20)
        {
            byte[] buffer = ReadBytes(hProcess, lpAddr, maxLen);
            StringBuilder builder = new StringBuilder();
            int index = 0;
            while (buffer[index] != 0)
            {
                builder.Append((char) buffer[index++]);
            }
            return builder.ToString();
        }

        private static ushort ReadUInt16(IntPtr hProcess, IntPtr lpAddr)
        {
            return BitConverter.ToUInt16(ReadBytes(hProcess, lpAddr, 2), 0);
        }

        private static uint ReadUInt32(IntPtr hProcess, IntPtr lpAddr)
        {
            return BitConverter.ToUInt32(ReadBytes(hProcess, lpAddr, 4), 0);
        }

        private static T ReadUnmanagedStructure<T>(IntPtr hProcess, IntPtr lpAddr)
        {
            byte[] lpBuffer = new byte[Marshal.SizeOf(typeof(T))];
            ReadProcessMemory(hProcess, lpAddr, lpBuffer, new UIntPtr((uint) lpBuffer.Length), IntPtr.Zero);
            GCHandle handle = GCHandle.Alloc(lpBuffer, GCHandleType.Pinned);
            T local = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return local;
        }

        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(IntPtr hThread);
        private static int SearchBytes(byte[] haystack, byte[] needle)
        {
            int index = 0;
            for (int i = 0; index < haystack.Length; i++)
            {
                if (needle.Length == i)
                {
                    return (index - i);
                }
                if (haystack[index] != needle[i])
                {
                    i = -1;
                }
                index++;
            }
            return -1;
        }

        private static int SearchPatternBytes(byte[] haystack, byte[] needle, string needlePattern)
        {
            int index = 0;
            for (int i = 0; index < haystack.Length; i++)
            {
                if (needle.Length == i)
                {
                    return (index - i);
                }
                if ((needlePattern[i] != '?') && (haystack[index] != needle[i]))
                {
                    i = -1;
                }
                index++;
            }
            return -1;
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);
        public static IntPtr StartNewClient(string path, string version)
        {
            PROCESS_INFORMATION process_information;
            List<string> source = new List<string>();
            if (Config.Instance.LaunchWithGamemasterFlag)
            {
                source.Add("gamemaster");
            }
            if (Config.Instance.ForceGraphicsEngine)
            {
                source.Add("engine " + Config.Instance.SelectedGraphicsEngine);
            }
            if (Config.Instance.StoreClientConfigurationSeparate)
            {
                source.Add(string.Format("path \"{0}\"", EnsureConfigurationFolder(version)));
            }
            STARTUPINFO lpStartupInfo = new STARTUPINFO();
            CreateProcess(path, source.Aggregate<string, string>("", (current, arg) => current + " " + arg), IntPtr.Zero, IntPtr.Zero, false, 4, IntPtr.Zero, Path.GetDirectoryName(path) ?? "", ref lpStartupInfo, out process_information);
            IntPtr baseAddress = GetBaseAddress(process_information.hProcess);
            if (baseAddress != IntPtr.Zero)
            {
                TryPatchMC(process_information.hProcess, baseAddress);
            }
            ResumeThread(process_information.hThread);
            return process_information.hProcess;
        }

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);
        public static bool TryPatchMC(IntPtr hProcess, IntPtr baseAddress)
        {
            IntPtr zero = IntPtr.Zero;
            long codeAddr = -1L;
            byte[] textSectionData = GetSectionBytes(hProcess, baseAddress, ".text", ref zero);
            SignatureEntry entry = _mcSigs.FirstOrDefault<SignatureEntry>(sig => (codeAddr = SearchPatternBytes(textSectionData, sig.Bytes, sig.Signature)) != -1L);
            if (entry == null)
            {
                return false;
            }
            IntPtr lpAddr = new IntPtr((zero.ToInt64() + codeAddr) + entry.Offset);
            WriteByte(hProcess, lpAddr, 0xeb);
            return true;
        }

        public static bool TryPatchRSA(Process process)
        {
            IntPtr baseAddress = process.MainModule.BaseAddress;
            if (baseAddress != IntPtr.Zero)
            {
                IntPtr zero = IntPtr.Zero;
                byte[] haystack = GetSectionBytes(process.Handle, baseAddress, ".rdata", ref zero);
                foreach (string str in CipsoftRSAPubKeys)
                {
                    int num = SearchBytes(haystack, Encoding.ASCII.GetBytes(str));
                    if (num != -1)
                    {
                        uint num2;
                        IntPtr lpAddress = new IntPtr(zero.ToInt64() + num);
                        VirtualProtectEx(process.Handle, lpAddress, new UIntPtr((uint) str.Length), 4, out num2);
                        WriteString(process.Handle, lpAddress, "109120132967399429278860960508995541528237502902798129123468757937266291492576446330739696001110603907230888610072655818825358503429057592827629436413108566029093628212635953836686562675849720620786279431090218017681061521755056710823876476444260558147179707119674283982419152118103759076030616683978566631413");
                        VirtualProtectEx(process.Handle, lpAddress, new UIntPtr((uint) str.Length), num2, out num2);
                        return true;
                    }
                }
            }
            return false;
        }

        public static int TryReplaceHostname(Process process, string hostname, ushort port)
        {
            List<long> loginAddresses = GetLoginAddresses(process);
            if (loginAddresses.Count > 0)
            {
                foreach (long num in loginAddresses)
                {
                    WriteString(process.Handle, new IntPtr(num), hostname);
                    WriteUInt16(process.Handle, new IntPtr(num + 100L), port);
                }
            }
            else
            {
                IntPtr zero = IntPtr.Zero;
                byte[] haystack = GetSectionBytes(process.Handle, process.MainModule.BaseAddress, ".text", ref zero);
                byte[] needle = new byte[] { 
                    0x75, 90, 0x68, 0, 4, 0, 0, 0xe8, 0x85, 0x3f, 7, 0, 0x83, 0xc4, 4, 0x8b, 
                    240, 0x8b, 0x3d, 0xe0, 2, 0x3f, 1, 0x8b, 0xcf, 0x2b, 13, 220, 2, 0x3f, 1, 0xb8, 
                    0x93, 0x24, 0x49, 0x92, 0xf7, 0xe9, 3, 0xd1, 0xc1, 250, 5, 0x8b, 0xc2, 0xc1, 0xe8, 0x1f, 
                    3, 0xc2, 0x74, 10, 0x8b, 0x47, 0xfc, 0xeb, 7, 0x83, 0xc0, 0x38, 0xeb, 0xa9
                 };
                int num2 = SearchPatternBytes(haystack, needle, "x?xxxxxx????xxxxxxx????xxxx????xxxxxxxxxxxxxxxxxxxx?xxxx?xxxx?");
                if (num2 == -1)
                {
                    return 1;
                }
                IntPtr lpAddr = new IntPtr((long) BitConverter.ToUInt32(haystack, num2 + 0x13));
                IntPtr ptr3 = new IntPtr((long) BitConverter.ToUInt32(haystack, (num2 + 0x13) + 8));
                IntPtr ptr4 = new IntPtr((long) ReadUInt32(process.Handle, ptr3));
                IntPtr ptr5 = new IntPtr((long) ReadUInt32(process.Handle, lpAddr));
                long num3 = (ptr5.ToInt64() - ptr4.ToInt64()) / 0x38L;
                for (int i = 0; i < num3; i++)
                {
                    IntPtr ptr6 = new IntPtr(ptr4.ToInt64() + (i * 0x38));
                    byte[] buffer3 = ReadBytes(process.Handle, ptr6, 0x38);
                    uint num5 = BitConverter.ToUInt32(buffer3, 0x18);
                    IntPtr ptr7 = ptr6;
                    if (num5 > 0x10)
                    {
                        ptr7 = new IntPtr((long) BitConverter.ToUInt32(buffer3, 4));
                    }
                    if (hostname.Length > num5)
                    {
                        return 2;
                    }
                    WriteString(process.Handle, ptr7, hostname);
                    WriteUInt16(process.Handle, new IntPtr(ptr6.ToInt64() + 20L), (ushort) hostname.Length);
                    WriteUInt16(process.Handle, new IntPtr(ptr6.ToInt64() + 0x30L), port);
                }
            }
            return 0;
        }

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UIntPtr dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        private static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        private static void WriteByte(IntPtr hProcess, IntPtr lpAddr, byte v)
        {
            WriteProcessMemory(hProcess, lpAddr, new byte[] { v }, new IntPtr(1), IntPtr.Zero);
        }

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, IntPtr nSize, IntPtr lpNumberOfBytesWritten);
        private static void WriteString(IntPtr hProcess, IntPtr lpAddr, string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str + '\0');
            WriteProcessMemory(hProcess, lpAddr, bytes, new IntPtr(bytes.Length), IntPtr.Zero);
        }

        private static void WriteUInt16(IntPtr hProcess, IntPtr lpAddr, ushort v)
        {
            WriteProcessMemory(hProcess, lpAddr, BitConverter.GetBytes(v), new IntPtr(2), IntPtr.Zero);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;
            public ushort e_cblp;
            public ushort e_cp;
            public ushort e_crlc;
            public ushort e_cparhdr;
            public ushort e_minalloc;
            public ushort e_maxalloc;
            public ushort e_ss;
            public ushort e_sp;
            public ushort e_csum;
            public ushort e_ip;
            public ushort e_cs;
            public ushort e_lfarlc;
            public ushort e_ovno;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
            public ushort[] e_res1;
            public ushort e_oemid;
            public ushort e_oeminfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=10)]
            public ushort[] e_res2;
            public int e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public uint ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x10)]
            public TibiaClient.IMAGE_DATA_DIRECTORY[] Directories;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public ulong ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x10)]
            public TibiaClient.IMAGE_DATA_DIRECTORY[] Directories;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        private struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            public byte[] Name;
            public uint Misc;
            public uint VirtualAddress;
            public uint SizeOfRawData;
            public uint PointerToRawData;
            public uint PointerToRelocations;
            public uint PointerToLinenumbers;
            public ushort NumberOfRelocations;
            public ushort NumberOfLinenumbers;
            public uint Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        private class SignatureEntry
        {
            public byte[] Bytes;
            public int Offset;
            public string Signature;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            private ushort reserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }
    }
}

