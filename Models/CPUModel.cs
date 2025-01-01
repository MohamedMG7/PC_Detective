namespace PC_Detective.Models
{
	public class CPUModel
	{
		public string Model { get; set; }
		public CpuArchitecture Architecture { get; set; }
		public string DefaultClockSpeed { get; set; }
		public string L2Cache { get; set; }
		public string L3Cache { get; set; }
		public string Cores { get; set; }
		
	}

	public enum CpuArchitecture
	{
		x86 = 0,
		MIPS = 1,
		Alpha = 2,
		PowerPC = 3,
		ARM = 5,
		Itanium = 6,
		x64 = 9,
		ARM64 = 12,
		IA32onIA64 = 10, 
		Unknown = -1 
	}
}
