namespace VConsoleLib;
static class Utils
{
	public static uint SwapEndianness( uint x )
	{
		return ((x & 0x000000ff) << 24) +  // First byte
			   ((x & 0x0000ff00) << 8) +   // Second byte
			   ((x & 0x00ff0000) >> 8) +   // Third byte
			   ((x & 0xff000000) >> 24);   // Fourth byte
	}

}
