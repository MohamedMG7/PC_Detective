using Hardware.Info;
using PC_Detective.Models;
using System.Windows;
using LibreHardwareMonitor.Hardware;
using PC_Detective.repositories;
using System.Diagnostics;
using System.Windows.Input;

namespace PC_Detective
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	
	public partial class MainWindow : Window
	{
		HardwareRepository hr = new HardwareRepository();
			
		public MainWindow()
		{
			InitializeComponent();
			//LoadCPUData();
			//LoadGPUData();
			//LoadGeneralData();
			//LoadGeneralDataAsync();
			usingLibreLib();
			
		}

		#region hide
		//private void LoadCPUData() {
		//	CPU cpuInfo = s.GetCPUInformation();

		//	if (cpuInfo != null)
		//	{
		//		IDText.Text = cpuInfo.ID;
		//		SocketText.Text = cpuInfo.Socket;
		//		NameText.Text = cpuInfo.Name;
		//		DescriptionText.Text = cpuInfo.Description;
		//		AddressWidthText.Text = cpuInfo.AddressWidth.ToString();
		//		DataWidthText.Text = cpuInfo.DataWidth.ToString();
		//		ArchitectureText.Text = cpuInfo.Architecture.ToString();
		//		SpeedMHzText.Text = cpuInfo.SpeedMHz.ToString();
		//		BusSpeedMHzText.Text = cpuInfo.BusSpeedMHz.ToString();
		//		L2CacheText.Text = cpuInfo.L2Cache.ToString();
		//		L3CacheText.Text = cpuInfo.L3Cache.ToString();
		//		CoresText.Text = cpuInfo.Cores.ToString();
		//		ThreadsText.Text = cpuInfo.Threads.ToString();
		//		CurrentClockSpeedText.Text = cpuInfo.CurrentClockSpeed.ToString();
		//	}
		//	else
		//	{
		//		MessageBox.Show("CPU information could not be retrieved.");
		//	}
		//}

		//private void LoadGPUData() {

		//	GPU GPUInformation = s.GetGPUInformation();

		//	if (GPUInformation != null)
		//	{
		//		GPUIDText.Text = GPUInformation.ID;
		//		GPUNameText.Text = GPUInformation.Name;
		//		GPUDescriptionText.Text = GPUInformation.Description;
		//		MemorySizeText.Text = GPUInformation.MemorySize.ToString();
		//		GPUCoreClockText.Text = GPUInformation.CoreClock.ToString();
		//		GPUMemoryClockText.Text = GPUInformation.MemoryClock.ToString();
		//		GPUDriverVersionText.Text = GPUInformation.DriverVersion;
		//		GPUManufacturerText.Text = GPUInformation.Manufacturer;
		//		GPUTemperatureText.Text = GPUInformation.Temperature.ToString();
		//		GPUArchitectureText.Text = GPUInformation.Architecture;
		//		GPUCoresText.Text = GPUInformation.Cores.ToString();
		//	}
		//	else
		//	{
		//		MessageBox.Show("GPU information could not be retrieved.");
		//	}
		//}
		#endregion

		//public void LoadGeneralData() {
		//	General gData = s.hardwareinfolib();
		//	GeneralOSNameText.Text = gData.OperatingSystem_Name;
		//	GeneralOSVersionText.Text = gData.OperatingSystem_Version;
		//	GeneralCPUText.Text = gData.CPU;
		//	GeneralGPUText.Text = gData.GPU;
		//	GeneralTimeTestText.Text = gData.time;
		//}

		//optimizing loading in async
		public async Task LoadGeneralDataAsync()
		{
			try
			{
				// Show loading indication (optional)
				GeneralTimeTestText.Text = "Loading...";

				// Run the hardware info gathering in the background
				General gData = await Task.Run(() =>
				{
					var stopwatch = System.Diagnostics.Stopwatch.StartNew();
					HardwareInfo hardwareInfo = new HardwareInfo();

					// Parallel refresh for faster execution
					Parallel.Invoke(
						() => hardwareInfo.RefreshCPUList(),
						() => hardwareInfo.RefreshOperatingSystem(),
						() => hardwareInfo.RefreshVideoControllerList()
					);

					General data = new General();
					data.CPU = hardwareInfo.CpuList.FirstOrDefault()?.Name;
					data.GPU = hardwareInfo.VideoControllerList.FirstOrDefault()?.Name;
					data.OperatingSystem_Name = hardwareInfo.OperatingSystem.Name;
					data.OperatingSystem_Version = hardwareInfo.OperatingSystem.VersionString;

					stopwatch.Stop();
					data.time = stopwatch.ElapsedMilliseconds.ToString();
					return data;
				});

				// Update UI
				GeneralOSNameText.Text = gData.OperatingSystem_Name;
				GeneralOSVersionText.Text = gData.OperatingSystem_Version;
				GeneralCPUText.Text = gData.CPU;
				GeneralGPUText.Text = gData.GPU;
				GeneralTimeTestText.Text = gData.time;
			}
			catch (Exception ex)
			{
				// Handle any errors
				MessageBox.Show($"Error loading hardware info: {ex.Message}");
			}
		}

		public void usingLibreLib() {
			HardwareInfo HI = new HardwareInfo();
			General data = new General();
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				GeneralTimeTestText.Text = "Loading...";

				var computer = new Computer
					{
						IsCpuEnabled = true,
						IsGpuEnabled = true,
						IsMemoryEnabled = true,
						IsMotherboardEnabled = false,
						IsControllerEnabled = false,
						IsNetworkEnabled = false,
						IsStorageEnabled = false
					};


				computer.Open();

				// Update UI
				GeneralOSNameText.Text = hr.GetOsName();
				GeneralOSVersionText.Text = hr.GetOsVersion();
				GeneralCPUText.Text = hr.GetCPUName(computer);
				GeneralGPUText.Text = hr.GetGPUName(computer);
				GeneralMotherboardText.Text = hr.GetMotherBoardModel();
				GeneralRAMText.Text = hr.GetRAMsData();
				GeneralStorageText.Text = hr.GetDiskDriveData();
				stopwatch.Stop();
				GeneralTimeTestText.Text = stopwatch.ElapsedMilliseconds.ToString();
				computer.Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading hardware info: {ex.Message}");
			}
		}

        public void DetailsFill(string type)
        {
            List<DetailItem> details = new List<DetailItem>();

            switch (type)
            {
                case "CPU":
                    details.Add(new DetailItem { Key = "Model", Value = hr.GetCPUModelWMI() });
                    details.Add(new DetailItem { Key = "Cores", Value = hr.GetCPUCoreInfo() });
                    details.Add(new DetailItem { Key = "Default Clock Speed", Value = hr.GetCPUMaxClockSpeed() });
                    details.Add(new DetailItem { Key = "Serial Number", Value = hr.GetCPUSerialNumber() });
                    break;

                case "OS":
                    details.Add(new DetailItem { Key = "OS Name", Value = hr.GetOsName() });
                    details.Add(new DetailItem { Key = "OS Version", Value = hr.GetOsVersion() });
                    break;

                case "RAM":
                    details.Add(new DetailItem { Key = "RAM Info", Value = hr.GetRAMsData() });
                    break;

                case "Storage":
                    details.Add(new DetailItem { Key = "Storage Info", Value = hr.GetDiskDriveData() });
                    break;

                case "GPU":
                    var gpuName = hr.GetGPUName(new LibreHardwareMonitor.Hardware.Computer { IsGpuEnabled = true });
                    details.Add(new DetailItem { Key = "GPU Name", Value = gpuName });
                    break;

                default:
                    details.Add(new DetailItem { Key = "Error", Value = "No details available!" });
                    break;
            }

            // Bind the details to the UI
            DetailsPanel.ItemsSource = details;
        }

        private void ShowCPUDetails(object sender, MouseButtonEventArgs e)
        {
            DetailsFill("CPU");
        }
        private void ShowOSDetails(object sender, MouseButtonEventArgs e)
        {
            DetailsFill("OS");
        }
    }
}