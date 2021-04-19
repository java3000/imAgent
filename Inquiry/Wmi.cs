using System;
using System.Collections.Generic;
using System.Text;

namespace ImAgent.Inquiry
{
    internal class Wmi
    {
        private const string DEVICE_QUERY1 = "select Name, Model, Manufacturer, UserName from Win32_ComputerSystem";
        private const string DEVICE_QUERY2 = "select SerialNumber, SMBIOSAssetTag from Win32_SystemEnclosure";
        private const string DEVICE_QUERY3 = "select Description from Win32_OperatingSystem";
        private const string DEVICE_QUERY4 = "select SerialNumber from Win32_BIOS";
        private const string DEVICE_QUERY5 = "select IPAddress, IPSubnet, MACAddress, DefaultIPGateway, DHCPServer, WINSPrimaryServer, WINSSecondaryServer, DNSServerSearchOrder from Win32_NetworkAdapterConfiguration";

        private const string MOTHERBOARD_QUERY1 = "select Product, Manufacturer, SerialNumber from Win32_BaseBoard";
        private const string MOTHERBOARD_QUERY2 = "select PrimaryBusType, SecondaryBusType from Win32_MotherboardDevice";
        //private const string MOTHERBOARD_QUERY3 = "select BIOSVersion from Win32_BIOS";
        private const string MOTHERBOARD_QUERY4 = "select MemoryDevices from Win32_PhysicalMemoryArray";
        private const string MOTHERBOARD_QUERY5 = "select Name, Manufacturer from Win32_FloppyDrive";

        private const string PROCESSOR_QUERY1 = "select InstalledSize from Win32_CacheMemory where (CacheType = 3 or CacheType = 4) and Level = 3";
        private const string PROCESSOR_QUERY2 = "select Name, Manufacturer, Family, SocketDesignation, AddressWidth, L2CacheSize from Win32_Processor";

        private const string DISKDRIVE_QUERY = "select DeviceID, Model, Manufacturer, InterfaceType, Size from Win32_DiskDrive";
        private const string CDDVD_QUERY = "select Name, Manufacturer, DeviceID from Win32_CDROMDrive";
        private const string CONTROLLER_QUERY = "select Name, ProtocolSupported, MaxNumberControlled from CIM_Controller";
        private const string AUDIOCARD_QUERY = "select ProductName, Manufacturer from Win32_SoundDevice";
        private const string VIDEOADAPTER_QUERY = "select Name, AdapterCompatibility, AdapterRAM, VideoProcessor, AdapterDACType from Win32_VideoController";
        //private const string MEMORY_QUERY = "select Name, Manufacturer, Family, SocketDesignation, AddressWidth, L2CacheSize from Win32_Processor";//TODO THIS CLASS ALREADY IS AS processor enquiry. need to try to work without dublicatesquery
        private const string NETWORKADAPTER_QUERY = "select MACAddress, Manufacturer, ProductName, PNPDeviceID from Win32_NetworkAdapter";
        private const string MONITOR_QUERY = "select Name, MonitorManufacturer, Bandwidth from Win32_DesktopMonitor";
        private const string KEYBOARD_QUERY = "select Name, NumberOfFunctionKeys from Win32_Keyboard";
        private const string PRINTER_QUERY = "select Caption, Default, DriverName, Capabilities, HorizontalResolution, VerticalResolution from Win32_Printer";
        private const string POINTINGDEVICE_QUERY = "select Manufacturer, Name, NumberOfButtons, PointingType from Win32_PointingDevice";
        private const string MODEM_QUERY = "select Model, MaxBaudRateToPhone, DeviceType from Win32_POTSModem";
        //private const string SOFTWARE_QUERY = "select Manufacturer, Name, Version, InstallDate from Win32_OperatingSystem";
        private const string LOGICALDRIVE_QUERY = "select Caption, DriveType, VolumeSerialNumber, VolumeName, FileSystem, Size, FreeSpace from Win32_LogicalDisk";

        private static readonly uint HKEY_LOCAL_MACHINE = unchecked((uint)0x80000002);
        private static readonly uint HKEY_USERS = unchecked((uint)0x80000003);

        //InquireSoftwareByRegistryPath(device, installationDic, HKEY_LOCAL_MACHINE, @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*", wmiRegistry);
        //InquireSoftwareByRegistryPath(device, installationDic, HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\*", wmiRegistry);
        //InquireSoftwareByRegistryPath(device, installationDic, HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\*\Products\*\InstallProperties", wmiRegistry);
        //InquireSoftwareByRegistryPath(device, installationDic, HKEY_USERS, @"*\Software\Microsoft\Windows\CurrentVersion\Uninstall\*", wmiRegistry);

        //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\*\Products
        //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall
        //HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall
        //HKEY_USERS\*\Software\Microsoft\Windows\CurrentVersion\Uninstall
        //HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows NT\CurrentVersion\Update\TargetingInfo\Installed
        //HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths
        //HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Appx\AppxAllUserStore\Config
        //HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths
        //HKEY_CURRENT_USER\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\App Paths
        //HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths
        //HKEY_CLASSES_ROOT\Applications\

        public Wmi()
        {
        }

        /*private static bool GetInfoByEDID(byte[] edid, out int width, out int height, out int inch, out string manufacturerName, out string modelName, out string serialNumber)
        {
            width = 0;
            height = 0;
            inch = 0;
            manufacturerName = string.Empty;
            modelName = string.Empty;
            serialNumber = string.Empty;
            //
            if (edid == null || edid.Length < 126)
                return false;
            //
            #region manufacturerList
            //кодирование производителя
            char[] ascii = new char[] { '\0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string[,] vendors = new string[,]
{
    { "AUO", "AU Optronics" },
    { "AIC", "AG Neovo" },
    { "ACR", "Acer" },
    { "DEL", "DELL" },
    { "SAM", "SAMSUNG" },
    { "SNY", "SONY" },
    { "SEC", "Epson" },
    { "WAC", "Wacom" },
    { "NEC", "NEC" },
    { "CMO", "CMO" },	*//* Chi Mei *//*
    { "BNQ", "BenQ" },
    { "ABP", "Advansys" },
    { "ACC", "Accton" },
    { "ACE", "Accton" },
    { "ADP", "Adaptec" },
    { "ADV", "AMD" },
    { "AIR", "AIR" },
    { "AMI", "AMI" },
    { "ASU", "ASUS" },
    { "ATI", "ATI" },
    { "ATK", "Allied Telesyn" },
    { "AZT", "Aztech" },
    { "BAN", "Banya" },
    { "BRI", "Boca Research" },
    { "BUS", "Buslogic" },
    { "CCI", "Cache Computers Inc." },
    { "CHA", "Chase" },
    { "CMD", "CMD Technology, Inc." },
    { "COG", "Cogent" },
    { "CPQ", "Compaq" },
    { "CRS", "Crescendo" },
    { "CSC", "Crystal" },
    { "CSI", "CSI" },
    { "CTL", "Creative Labs" },
    { "DBI", "Digi" },
    { "DEC", "Digital Equipment" },
    { "DBK", "Databook" },
    { "EGL", "Eagle Technology" },
    { "ELS", "ELSA" },
    { "ESS", "ESS" },
    { "FAR", "Farallon" },
    { "FDC", "Future Domain" },
    { "HWP", "Hewlett-Packard" },
    { "IBM", "IBM" },
    { "INT", "Intel" },
    { "ISA", "Iomega" },
    { "LEN", "Lenovo" },
    { "MDG", "Madge" },
    { "MDY", "Microdyne" },
    { "MET", "Metheus" },
    { "MIC", "Micronics" },
    { "MLX", "Mylex" },
    { "NVL", "Novell" },
    { "OLC", "Olicom" },
    { "PRO", "Proteon" },
    { "RII", "Racal" },
    { "RTL", "Realtek" },
    { "SCM", "SCM" },
    { "SKD", "SysKonnect" },
    { "SGI", "SGI" },
    { "SMC", "SMC" },
    { "SNI", "Siemens Nixdorf" },
    { "STL", "Stallion Technologies" },
    { "SUN", "Sun" },
    { "SUP", "SupraExpress" },
    { "SVE", "SVEC" },
    { "TCC", "Thomas-Conrad" },
    { "TCI", "Tulip" },
    { "TCM", "3Com" },
    { "TCO", "Thomas-Conrad" },
    { "TEC", "Tecmar" },
    { "TRU", "Truevision" },
    { "TOS", "Toshiba" },
    { "TYN", "Tyan" },
    { "UBI", "Ungermann-Bass" },
    { "USC", "UltraStor" },
    { "VDM", "Vadem" },
    { "VMI", "Vermont" },
    { "WDC", "Western Digital" },
    { "ZDS", "Zeos" },
    { "ACT", "Targa" },
    { "ADI", "ADI" },
    { "AOC", "AOC Intl" },
    { "API", "Acer America" },
    { "APP", "Apple Computer" },
    { "ART", "ArtMedia" },
    { "AST", "AST Research" },
    { "CPL", "Compal" },
    { "CTX", "Chuntex Electronic Co." },
    { "DPC", "Delta Electronics" },
    { "DWE", "Daewoo" },
    { "ECS", "ELITEGROUP" },
    { "EIZ", "EIZO" },
    { "FCM", "Funai" },
    { "GSM", "LG Electronics" },
    { "LGD", "LG Electronics" },
    { "GWY", "Gateway 2000" },
    { "HEI", "Hyundai" },
    { "HIT", "Hitachi" },
    { "HSL", "Hansol" },
    { "HTC", "Hitachi" },
    { "ICL", "Fujitsu ICL" },
    { "IVM", "Idek Iiyama" },
    { "KFC", "KFC Computek" },
    { "LKM", "ADLAS" },
    { "LNK", "LINK Tech" },
    { "LTN", "Lite-On" },
    { "MAG", "MAG InnoVision" },
    { "MAX", "Maxdata" },
    { "MEI", "Panasonic" },
    { "MEL", "Mitsubishi" },
    { "MIR", "miro" },
    { "MTC", "MITAC" },
    { "NAN", "NANAO" },
    { "NEC", "NEC Tech" },
    { "NOK", "Nokia" },
    { "OQI", "OPTIQUEST" },
    { "PBN", "Packard Bell" },
    { "PGS", "Princeton" },
    { "PHL", "Philips" },
    { "REL", "Relisys" },
    { "SDI", "Samtron" },
    { "SMI", "Smile" },
    { "SPT", "Sceptre" },
    { "SRC", "Shamrock Technology" },
    { "STP", "Sceptre" },
    { "TAT", "Tatung" },
    { "TRL", "Royal Information Company" },
    { "TSB", "Toshiba, Inc." },
    { "UNM", "Unisys" },
    { "VSC", "ViewSonic" },
    { "WTC", "Wen Tech" },
    { "ZCM", "Zenith Data Systems" },
    { "???", "Unknown" },
};
            #endregion
            //
            #region manufacturer by specification = 8 and 9 bytes = 3 coded symbols
            byte[] manufacturerData = new byte[2];
            Array.Copy(edid, 8, manufacturerData, 0, 2);
            byte[] m = new byte[3];
            m[0] = (byte)((manufacturerData[0] & 0x7c) >> 2);
            m[1] = (byte)((byte)((manufacturerData[0] & 0x03) << 3) | (byte)((manufacturerData[1] & 0xE0) >> 5));
            m[2] = (byte)(manufacturerData[1] & 0x1F);
            string manID = string.Format("{0}{1}{2}", ascii[m[0]], ascii[m[1]], ascii[m[2]]);
            for (int i = 0; i < vendors.GetLength(0); i++)
                if (vendors[i, 0] == manID)
                {
                    manufacturerName = vendors[i, 1];
                    break;
                }
            #endregion
            //
            #region width by specification = 21 byte
            width = edid[21];
            #endregion
            //
            #region height by specification = 22 byte
            height = edid[22];
            #endregion
            //
            #region inch
            inch = (int)Math.Floor(Math.Sqrt(width * width + height * height) / 2.54d); // from cm to inch
            #endregion
            //
            #region model information by specification = descriptor III: 90..90+18, descriptor IV: 108..108+18 bytes
            byte[] descriptor3 = new byte[18];
            Array.Copy(edid, 90, descriptor3, 0, 18);
            //
            byte[] descriptor4 = new byte[18];
            Array.Copy(edid, 108, descriptor4, 0, 18);
            //
            int descriptorNumber = 0;
            foreach (byte[] descriptor in new byte[][] { descriptor3, descriptor4 })
            {
                if (descriptor[0] != 0 || descriptor[1] != 0 || descriptor[2] != 0 || descriptor[4] != 0)
                    continue;//another info by specification
                //
                byte[] str = new byte[13]; //length by specification
                Array.Copy(descriptor, 5, str, 0, 13);
                string text = Encoding.GetEncoding(437).GetString(str).Replace("\r", "").Trim();
                //
                switch (descriptor[3])
                {
                    case 0xFC://model
                        modelName = text;
                        break;
                    case 0xFE://unknown info
                        if (descriptorNumber == 0)
                        {
                            descriptorNumber++;
                            continue;
                        }
                        else
                            modelName = text;
                        break;
                    case 0xFF://serial
                        serialNumber = text;
                        break;
                    default:
                        continue;
                }
            }
            #endregion
            //
            return modelName != string.Empty;
        }

        private static bool GetMonitorsFromRegistry(Device device, List<Subdevice> monitorList, InquiryObjectType iot, uint hKey, string hKeyPath, ManagementClass wmiRegistry)
        {
            //monitor list in @"SYSTEM\CurrentControlSet\Enum\DISPLAY"
            //active monitors in @"SYSTEM\CurrentControlSet\Services\monitor\Enum"
            try
            {
                UInt32 activeMonitorCount = 0;
                {
                    ManagementBaseObject inParam = wmiRegistry.GetMethodParameters("GetDWORDValue");
                    inParam["hDefKey"] = hKey;
                    inParam["sSubKeyName"] = hKeyPath;
                    inParam["sValueName"] = "Count";
                    //
                    ManagementBaseObject outParam = wmiRegistry.InvokeMethod("GetDWORDValue", inParam, null);
                    if ((uint)outParam["ReturnValue"] == 0)
                        activeMonitorCount = (UInt32)outParam["uValue"]; //example: 1
                }
                //
                for (int i = 0; i < activeMonitorCount; i++)
                {
                    string monitorPath = null;
                    {
                        ManagementBaseObject inParam = wmiRegistry.GetMethodParameters("GetStringValue");
                        inParam["hDefKey"] = hKey;
                        inParam["sSubKeyName"] = hKeyPath;
                        inParam["sValueName"] = i.ToString();
                        //
                        ManagementBaseObject outParam = wmiRegistry.InvokeMethod("GetStringValue", inParam, null);
                        if ((uint)outParam["ReturnValue"] == 0)
                            monitorPath = outParam["sValue"] as string; //example: DISPLAY\BNQ78B2\5&2348ef14&0&UID1048833
                    }
                    //
                    if (string.IsNullOrWhiteSpace(monitorPath))
                        continue;
                    //
                    byte[] edid = null;
                    {
                        ManagementBaseObject inParam = wmiRegistry.GetMethodParameters("GetBinaryValue");
                        inParam["hDefKey"] = hKey;
                        inParam["sSubKeyName"] = string.Format(@"SYSTEM\CurrentControlSet\Enum\{0}\Device Parameters", monitorPath);
                        inParam["sValueName"] = "EDID";
                        //
                        ManagementBaseObject outParam = wmiRegistry.InvokeMethod("GetBinaryValue", inParam, null);
                        if ((uint)outParam["ReturnValue"] == 0)
                            edid = outParam["uValue"] as byte[];
                    }
                    //
                    if (edid == null)
                        continue;
                    //
                    string serialNumber = string.Empty;
                    string modelName = string.Empty;
                    string manufacturerName = string.Empty;
                    int width = 0;
                    int height = 0;
                    int inch = 0;
                    //
                    if (!GetInfoByEDID(edid, out width, out height, out inch, out manufacturerName, out modelName, out serialNumber))
                        break; //не получили информацию
                    //
                    bool @new = false;
                    Manufacturer manufacturer = Manufacturer.Get(manufacturerName, device.ProcessCache);
                    SubdeviceModel model = SubdeviceModel.Get(InquiryObjectType.Monitor, modelName, manufacturer, device.ProcessCache, out @new);
                    if (model == null)
                        continue;
                    if (@new)
                    {
                        if (inch > 0)
                            ((StringParameter)model.ParameterList[Parameter.MONITOR_BANDWIDTH]).Value = inch.ToString();
                        if (width > 0)
                            ((Int32Parameter)model.ParameterList[Parameter.MONITOR_SCREENWIDTH]).Value = width;
                        if (height > 0)
                            ((Int32Parameter)model.ParameterList[Parameter.MONITOR_SCREENHEIGHT]).Value = height;
                    }
                    monitorList.Add(new Subdevice(model, device) { SerialNumber = serialNumber });
                }
                //
                return monitorList.Count > 0;
            }
            catch (ManagementException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Ошибка получения мониторов по реестру.");
                return false;
            }
        }

        internal override List<Subdevice> InquireMonitor(Device device)
        {
            List<Subdevice> retval = new List<Subdevice>();
            //            
            uint HKEY_LOCAL_MACHINE = unchecked((uint)0x80000002);
            _managementScope.Path = new ManagementPath(string.Concat(@"\\", _currentHost, @"\root\default"));
            ManagementClass wmiRegistry = new ManagementClass(_managementScope, new ManagementPath("StdRegProv"), null);
            //
            bool getByRegistry = GetMonitorsFromRegistry(device, retval, InquiryObjectType.Monitor, HKEY_LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Services\monitor\Enum", wmiRegistry);
            //
            if (!getByRegistry)
            {
                _managementScope.Path = new ManagementPath(string.Concat(@"\\", _currentHost, @"\root\cimv2"));
                if (!_managementScope.IsConnected)
                {
                    _managementScope.Connect();
                }
                ObjectQuery query = new ObjectQuery("select Name, MonitorManufacturer, Bandwidth from Win32_DesktopMonitor");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(_managementScope, query))
                {
                    ManagementObjectCollection objList;
                    if (TryGetValue(searcher, out objList))
                        foreach (ManagementObject obj in objList)
                        {
                            bool @new;
                            string manufacturerName = Convert.ToString(obj["MonitorManufacturer"]).Trim();
                            Manufacturer manufacturer = Manufacturer.Get(manufacturerName, device.ProcessCache);
                            //
                            string modelName = Convert.ToString(obj["Name"]).Trim();
                            SubdeviceModel model = SubdeviceModel.Get(InquiryObjectType.Monitor, modelName, manufacturer, device.ProcessCache, out @new);
                            if (model != null)
                            {
                                if (@new)
                                {
                                    ((StringParameter)model.ParameterList[Parameter.MONITOR_BANDWIDTH]).Value = Convert.ToString(obj["Bandwidth"]);
                                }
                                Subdevice monitor = new Subdevice(model, device);
                                retval.Add(monitor);
                            }
                            //
                            obj.Dispose();
                        }
                }
            }
        }

        private static string DecodeProductKey(byte[] digitalProductId)
        {//так уж решило Microsoft (c Vista изменилось)
            if (digitalProductId == null || digitalProductId.Length < 66)
                return null;
            //
            int keyOffset = 52;
            if (digitalProductId.Length == 1272)
                keyOffset = 808;
            //
            var base64 = "BCDFGHJKMPQRTVWXY2346789";
            var decodeStringLength = base64.Length;
            var decodeLength = 14;
            var retval = string.Empty;
            //
            var isContainsN = (digitalProductId[decodeLength + keyOffset] >> 3) & 1;
            digitalProductId[decodeLength + keyOffset] = (byte)((digitalProductId[decodeLength + keyOffset] & 247) | ((isContainsN & 2) << 2));
            for (var i = decodeStringLength; i >= 0; i--)
            {
                var index = 0;
                for (var j = decodeLength; j >= 0; j--)
                {
                    index = index * 256 ^ digitalProductId[j + keyOffset];
                    digitalProductId[j + keyOffset] = (byte)Math.Truncate((decimal)index / (decimal)base64.Length);
                    index = index % base64.Length;
                }
                retval = retval.Insert(0, base64[index].ToString());
            }
            //
            if (isContainsN == 1)
            {
                var index = 0;
                for (var i = 0; i < decodeStringLength; i++)
                {
                    if (retval[0] != base64[i])
                        continue;
                    index = i;
                    break;
                }
                //
                var keyWithoutN = retval;
                keyWithoutN = keyWithoutN.Remove(0, 1);
                keyWithoutN = keyWithoutN.Substring(0, index) + "N" + keyWithoutN.Remove(0, index);
                retval = keyWithoutN;
            }
            //
            for (var i = 20; i >= 5; i -= 5)
                retval = retval.Insert(i, "-");
            //
            return retval;
        }

        private string GetOSProductKey(ManagementClass wmiRegistry, uint hDefKey)
        {
            var key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DigitalProductId");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DigitalProductId4");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\DefaultProductKey", "DigitalProductId");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\DefaultProductKey", "DigitalProductId4");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "DefaultProductKey2");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SYSTEM\Setup\Source OS (Updated on 11/22/2017 15:29:40)", "DigitalProductId");
            if (string.IsNullOrWhiteSpace(key))
                key = ReadAndParseProductKey(wmiRegistry, hDefKey, @"SYSTEM\Setup\Source OS (Updated on 11/22/2017 15:29:40)", "DigitalProductId4");
            //
            return key;
        }

        private string ReadAndParseProductKey(ManagementClass wmiRegistry, uint hDefKey, string path, string parameterName)
        {
            try
            {
                byte[] digitalProductID = null;
                //
                ManagementBaseObject inParam = wmiRegistry.GetMethodParameters("GetBinaryValue");
                inParam["hDefKey"] = hDefKey;
                inParam["sSubKeyName"] = path;
                inParam["sValueName"] = parameterName;
                //
                InvokeMethodOptions invokeOptions = null;
                //
                ManagementBaseObject outParam = wmiRegistry.InvokeMethod("GetBinaryValue", inParam, invokeOptions);
                if ((uint)outParam["ReturnValue"] == 0)
                    digitalProductID = outParam["uValue"] as byte[];
                //
                if (digitalProductID == null)
                    return null;
                else
                {
                    var productKey = DecodeProductKey(digitalProductID);
                    if (!string.IsNullOrWhiteSpace(productKey))
                        return productKey;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Ошибка получения ключа лицензии.");
                return null;
            }
        }

        private string ReadProductKey(ManagementClass wmiRegistry, uint hDefKey, string path, string parameterName)
        {
            try
            {
                string key = null;
                //
                ManagementBaseObject inParam = wmiRegistry.GetMethodParameters("GetBinaryValue");
                inParam["hDefKey"] = hDefKey;
                inParam["sSubKeyName"] = path;
                inParam["sValueName"] = parameterName;
                //
                InvokeMethodOptions invokeOptions = null;
                //
                ManagementBaseObject outParam = wmiRegistry.InvokeMethod("GetStringValue", inParam, invokeOptions);
                if ((uint)outParam["ReturnValue"] == 0)
                    key = outParam["sValue"] as string;
                //
                if (!string.IsNullOrWhiteSpace(key) && key.Length == 25)
                    key = key.Insert(5, "-").Insert(5 * 2 + 1, "-").Insert(5 * 3 + 2, "-").Insert(5 * 4 + 3, "-");
                return key;
            }
            catch (Exception ex)
            {
                Logger.Info(ex, "Ошибка получения ключа лицензии.");
                return null;
            }
        }

        private Dictionary<String, String> GetProductKeys(uint hDefKey, ManagementClass wmiRegistry)
        {
            Dictionary<String, String> retval = new Dictionary<string, string>();
            //
            var collect = new Func<string, string, string, string, bool>((path, param, name, ver) =>
            {
                var key = ReadAndParseProductKey(wmiRegistry, hDefKey, path, param);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    if (ver != string.Empty)
                        name = string.Concat(name, " ", ver);
                    lock (retval)
                        if (!retval.ContainsKey(name))
                        {
                            retval.Add(name, key);
                            return true;
                        }
                }
                return false;
            });
            var collect2 = new Func<string, string, string, string, bool>((path, param, name, ver) =>
            {
                var key = ReadProductKey(wmiRegistry, hDefKey, path, param);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    if (ver != string.Empty)
                        name = string.Concat(name, " ", ver);
                    lock (retval)
                        if (!retval.ContainsKey(name))
                        {
                            retval.Add(name, key);
                            return true;
                        }
                }
                return false;
            });
            //
            //
            //ie
            if (!collect(@"SOFTWARE\Microsoft\Internet Explorer\Registration", "DigitalProductId", "Internet Explorer", string.Empty))
                collect(@"SOFTWARE\Microsoft\Internet Explorer\Registration", "DigitalProductId4", "Internet Explorer", string.Empty);
            //
            //
            //sql
            collect(@"SOFTWARE\Microsoft\Microsoft SQLServer\80\Registration", "DigitalProductID", "Microsoft SQL Server", "2000");
            collect(@"SOFTWARE\Microsoft\Microsoft SQLServer\90\Registration", "DigitalProductID", "Microsoft SQL Server", "2005");
            //
            var r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2008");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\100\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2008");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\100\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2008");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\100\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2008");
            //
            r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\110\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2012");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\110\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2012");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\110\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2012");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\110\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2012");
            //
            r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\120\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2014");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\120\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2014");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\120\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2014");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\120\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2014");
            //
            r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\130\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2016");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\130\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2016");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\130\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2016");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\130\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2016");
            //
            r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\140\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2017");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Microsoft SQL Server\140\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2017");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\140\Tools\ClientSetup", "DigitalProductID", "Microsoft SQL Server", "2017");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server\140\Tools\Setup", "DigitalProductID", "Microsoft SQL Server", "2017");
            //
            //
            //visual studio
            r = collect2(@"SOFTWARE\Microsoft\VisualStudio\8.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2005");
            if (!r) r = collect2(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\8.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2005");
            //
            r = collect2(@"SOFTWARE\Microsoft\VisualStudio\9.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2008");
            if (!r) r = collect2(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\9.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2008");
            //
            r = collect2(@"SOFTWARE\Microsoft\VisualStudio\10.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2010");
            if (!r) r = collect2(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\10.0\Registration", "PIDKEY", "Microsoft Visual Studio", "2010");
            //
            r = collect2(@"SOFTWARE\Microsoft\VisualStudio\12.0\Registration\1000.0x0010", "PIDKEY", "Microsoft Visual Studio", "2013");
            if (!r) r = collect2(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\12.0\Registration\1000.0x0010", "PIDKEY", "Microsoft Visual Studio", "2013");
            //
            r = collect2(@"SOFTWARE\Microsoft\VisualStudio\14.0\Registration\2000.0x0000", "PIDKEY", "Microsoft Visual Studio", "2015");
            if (!r) r = collect2(@"SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\Registration\2000.0x0000", "PIDKEY", "Microsoft Visual Studio", "2015");
            //
            //
            //esset
            collect2(@"SOFTWARE\ESET\ESET Security\CurrentVersion\LicenseInfo", "UserName", "ESET", string.Empty);
            //
            //
            //office            
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\16.0\Registration\{90160000-0012-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\16.0\Registration\{90160000-0012-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\16.0\Registration\{90160000-0053-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\16.0\Registration\{90160000-0053-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\16.0\Registration\{90160000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\16.0\Registration\{90160000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2016");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{91150000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{91150000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{91150000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{91150000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-0011-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-0011-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2013");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-002A-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-002A-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{91150000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{91150000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-0051-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-0051-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2013");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2013");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\15.0\Registration\{90150000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2013");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\15.0\Registration\{90150000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2013");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{91140000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{91140000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-002A-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-002A-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{191301D3-A579-428C-B0C7-D7988500F9E3}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{191301D3-A579-428C-B0C7-D7988500F9E3}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{6F327760-8C5C-417C-9B61-836A98287E0C}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{6F327760-8C5C-417C-9B61-836A98287E0C}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{FDF3ECB9-B56F-43B2-A9B8-1B48B6BAE1A7}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{FDF3ECB9-B56F-43B2-A9B8-1B48B6BAE1A7}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-003D-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-003D-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-0011-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-0011-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-0011-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2010");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{91140000-0057-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{91140000-0057-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-0057-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-0057-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-0057-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-0057-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2010");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-003B-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2010");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\14.0\Registration\{90140000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2010");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\14.0\Registration\{90140000-003B-0000-1000-0000000FF1CE}", "DigitalProductID", "Microsoft Project", "2010");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{91120000-00CA-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{91120000-00CA-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{91120000-0014-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{91120000-0014-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{91120000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{91120000-0051-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{91120000-0053-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{91120000-0053-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{90120000-0021-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{90120000-0021-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{90120000-0030-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{90120000-0030-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{90120000-0012-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{90120000-0012-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Office", "2007");
            //            
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\12.0\Registration\{90120000-0053-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2007");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\12.0\Registration\{90120000-0053-0000-0000-0000000FF1CE}", "DigitalProductID", "Microsoft Visio", "2007");
            //
            r = collect(@"SOFTWARE\WOW6432Node\Microsoft\Office\11.0\Registration\{91110409-6000-11D3-8CFE-0150048383C9}", "DigitalProductID", "Microsoft Office", "2003");
            if (!r) r = collect(@"SOFTWARE\Microsoft\Office\11.0\Registration\{91110409-6000-11D3-8CFE-0150048383C9}", "DigitalProductID", "Microsoft Office", "2003");
            //
            return retval;
        }*/
    }
}