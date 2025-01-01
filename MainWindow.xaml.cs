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
		CachingSys Cs = new CachingSys();
			
		public MainWindow()
		{
			InitializeComponent();

			IsCpuCachedAsync();
			IsOsDataCachedAsync();

			//usingLibreLib();
			
		}

		

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
				GeneralOSNameText.Text =Cs.OperatingSystem.Name;
				GeneralCPUText.Text = Cs.cpuModel.Model;
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

		public async Task<bool> IsCpuCachedAsync()
		{
			if (Cs.cpuModel == null)
			{
				var l2CacheTask = Task.Run(() => hr.GetCPUL2Cache());
				var l3CacheTask = Task.Run(() => hr.GetCPUL3Cache());
				var modelTask = Task.Run(() => hr.GetCPUModelWMI());
				var archTask = Task.Run(() => hr.GetCPUArcticture());
				var clockTask = Task.Run(() => hr.GetCPUMaxClockSpeed());
				var coresTask = Task.Run(() => hr.GetCPUCoreInfo());

				await Task.WhenAll(l2CacheTask, l3CacheTask, modelTask, archTask, clockTask, coresTask);

				Cs.cpuModel = new CPUModel
				{
					L2Cache = await l2CacheTask,
					L3Cache = await l3CacheTask,
					Model = await modelTask,
					Architecture = await archTask,
					DefaultClockSpeed = await clockTask,
					Cores = await coresTask
				};
				return false;
			}
			return true;
		}

		public async Task<bool> IsOsDataCachedAsync()
		{
			if (Cs.OperatingSystem == null)
			{
				
				var nameTask = Task.Run(() => hr.GetOsName());
				var versionTask = Task.Run(() => hr.GetOsVersion());

				await Task.WhenAll(nameTask, versionTask);

				Cs.OperatingSystem = new OperatingSystemModel
				{
					Name = await nameTask,
					Version = await versionTask
				};
				return false;
			}
			return true;
		}

		public void DetailsFill(string type)
        {
            List<DetailItem> details = new List<DetailItem>();

            switch (type)
            {
                case "CPU":
					IsCpuCachedAsync();
                    details.Add(new DetailItem { Key = "Model", Value = Cs.cpuModel.Model });
                    details.Add(new DetailItem { Key = "Cores", Value = Cs.cpuModel.Cores });
                    details.Add(new DetailItem { Key = "Default Clock Speed", Value = Cs.cpuModel.DefaultClockSpeed });
                    details.Add(new DetailItem { Key = "Archticture", Value = Cs.cpuModel.Architecture.ToString() });
					details.Add(new DetailItem { Key = "L2 Cache", Value = Cs.cpuModel.L2Cache });
					details.Add(new DetailItem { Key = "L3 Cache", Value = Cs.cpuModel.L3Cache });
					break;

                case "OS":
					IsOsDataCachedAsync();
                    details.Add(new DetailItem { Key = "OS Name", Value = Cs.OperatingSystem.Name });
                    details.Add(new DetailItem { Key = "OS Version", Value = Cs.OperatingSystem.Version });
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