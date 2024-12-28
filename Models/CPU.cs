namespace PC_Detective.Models
{
	public class CPU
	{
		public string ID { get; set; }
		public string Socket { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ushort AddressWidth { get; set; }
		public ushort DataWidth { get; set; }
		public CpuArchitecture Architecture { get; set; }
		public uint SpeedMHz { get; set; }
		public uint BusSpeedMHz { get; set; }
		public ulong L2Cache { get; set; }
		public ulong L3Cache { get; set; }
		public uint Cores { get; set; }
		public uint Threads { get; set; }
		public uint CurrentClockSpeed { get; set; }
	}

	public enum CpuArchitecture
	{
		x86 = 0,
		x64 = 1
	}
}
