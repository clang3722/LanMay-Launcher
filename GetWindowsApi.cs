using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;


namespace LanMay_Launcher
{
    internal class GetWindowsApi
    {
        public int GetWindowsAvailableMBytes()
        {
            string PhysicalMemory = string.Empty;
            var managementClass = new ManagementClass("Win32_ComputerSystem");
            var managementObjectCollection = managementClass.GetInstances();
            foreach (var managementBaseObject in managementObjectCollection)
                if (managementBaseObject["TotalPhysicalMemory"] != null) { 
                    PhysicalMemory = managementBaseObject["TotalPhysicalMemory"].ToString();
            }
            
            long PhysicalMemoryTo = long.Parse(PhysicalMemory) / 1024 / 1024 - 128;
            return (int)PhysicalMemoryTo;
        }
    }
}
