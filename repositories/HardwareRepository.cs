using LibreHardwareMonitor.Hardware;
using Hardware.Info;
using System.Management;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace PC_Detective.repositories
{
	public class HardwareRepository
	{
		HardwareInfo HI = new HardwareInfo();

		public string GetCPUName(Computer computer)
		{
			var cpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
			return cpu != null ? cpu.Name : "Data Not Available";
		}

		public string GetGPUName(Computer computer)
		{
			var gpu = computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.GpuNvidia || h.HardwareType == HardwareType.GpuAmd ||
																		  h.HardwareType == HardwareType.GpuIntel);
			return gpu != null ? gpu.Name : "Data Not Available";
		}

		public string GetOsName()
		{
			HI.RefreshOperatingSystem();
			string OSName = HI.OperatingSystem.Name;
			return OSName != null ? OSName : "Data Not Available";
		}

		public string GetOsVersion()
		{
			HI.RefreshOperatingSystem();
			string OSVersion = HI.OperatingSystem.VersionString;
			return OSVersion != null ? OSVersion : "Data Not Available";
		}


		public string GetMotherBoardModel()
		{

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
			ManagementObjectCollection queryCollection = searcher.Get();

			foreach (ManagementObject m in queryCollection)
			{

				string manufacturer = m["Manufacturer"]?.ToString() ?? "Data Not Available";
				string product = m["Product"]?.ToString() ?? "Data Not Available";

				return $"{manufacturer}, {product}";
			}

			return "Motherboard data not available";
		}

		public string GetRAMsData()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
			ManagementObjectCollection queryCollection = searcher.Get();

			List<string> ramDetails = new List<string>();

			int counter = 1;
			foreach (ManagementObject m in queryCollection)
			{
				string name = m["Manufacturer"].ToString();
				string Speed = m["Speed"].ToString();
				string type = m["SMBIOSMemoryType"].ToString();
				if (type == "26")
				{
					type = "DDR4";
				}
				else if (type == "34")
				{
					type = "DDR5";
				}
				long capacityInGigabytes = Convert.ToInt64(m["Capacity"]) / (1024 * 1024 * 1024);
				string CapacityText = capacityInGigabytes.ToString();
				ramDetails.Add($"{counter}- {name}, {Speed} MHz, {type}, {CapacityText} GB");
				counter++;
				//return $"{name}, {Speed}MHZ, {type}, {CapacityText}GB";
			}


			//HI.RefreshMemoryList();
			//string rData = HI.MemoryList.FirstOrDefault().Speed.ToString();
			return ramDetails.Count > 0 ? string.Join("\n", ramDetails) : "No RAM Data Found";
		}

		public string GetDiskDriveData()
		{
			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DISKDRIVE");
			ManagementObjectCollection queryCollection = searcher.Get();

			List<string> DisksDetails = new List<string>();

			foreach (ManagementObject m in queryCollection)
			{
				string name = m["Caption"].ToString();
				long capacityInGigabytes = Convert.ToInt64(m["Size"]) / (1024 * 1024 * 1024);
				string Capacity = capacityInGigabytes.ToString();
				return $"{name}, {Capacity}GB";
			}
			return "Data Not Found";
		}

		public string GetCPUModelWMI()
		{

			string cpuModel = "Unknown";

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

			foreach (ManagementObject obj in searcher.Get())
			{
				// Fetch the CPU Name property
				cpuModel = obj["Name"]?.ToString()!;
				break; // Use the first processor's information
			}

			return cpuModel;
		}

		public string GetCPUCoreInfo()
		{

			string cpuCoreInfo = "Unknown";

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

			foreach (ManagementObject obj in searcher.Get())
			{
                int physicalCores = Convert.ToInt32(obj["NumberOfCores"]);

                // Logical cores (includes Hyper-Threading)
                int logicalCores = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);

                // Add to output
                cpuCoreInfo = $"Physical Cores: {physicalCores}\nLogical Cores: {logicalCores}";
            }

			return cpuCoreInfo;

		}

		public string GetCPUMaxClockSpeed() { 
			string cpuMaxClockSpeed = "Unknown";

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

			foreach (ManagementObject m in searcher.Get()) {
				int MaxClockSpeed = Convert.ToInt32(m["MaxClockSpeed"]);

				cpuMaxClockSpeed = MaxClockSpeed.ToString();
			}

			return cpuMaxClockSpeed;
		}

		public string GetCPUSerialNumber() { 
			string cpuSerialNumber = "Unknown";

			ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");

			
            foreach (ManagementObject m in searcher.Get())
            {
                 cpuSerialNumber = m["CurrentVoltage"].ToString()!;
            }
            return cpuSerialNumber;
            
		}
	}
}
