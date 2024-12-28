namespace PC_Detective.Models
{
	public class GPU
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public uint MemorySize { get; set; }  // in MB
		public uint CoreClock { get; set; }   // in MHz
		public uint MemoryClock { get; set; } // in MHz
		public string DriverVersion { get; set; }
		public uint VRAM { get; set; }        // Video RAM in MB
		public uint Temperature { get; set; } // GPU temperature in Celsius
		public string Manufacturer { get; set; }
		public string Architecture { get; set; }
		public uint Cores { get; set; }
	}
}
