using System.Runtime.InteropServices;

namespace VConsoleLib;

[StructLayout( LayoutKind.Sequential, Pack = 1 )]
internal struct Chan
{
	public ushort ChannelCount = 0;

	[StructLayout( LayoutKind.Sequential )]
	public struct ChanEntry
	{
		public uint ChannelId = 0;

		[MarshalAs( UnmanagedType.ByValArray, SizeConst = 20 )]
		public byte[] _Unknown = new byte[]
		{
			00, 00, 00, 00,
			00, 00, 00, 00,

			00, 00, 00, 02,
			00, 00, 00, 02,

			00, 00, 00, 00
		};

		[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 30 )]
		public string ChannelName = "";

		public uint _Unknown2 = 0x01000000;

		public static uint CalcChannelID( string channelName )
		{
			uint hash = 0;

			foreach ( char c in channelName.ToLower() )
			{
				hash += (uint)c;
				hash += (hash << 10);
				hash ^= (hash >> 6);
			}

			hash += (hash << 3);
			hash ^= (hash >> 11);
			hash += (hash << 15);

			return hash;
		}

		public ChanEntry( string channelName )
		{
			ChannelName = channelName;
			ChannelId = CalcChannelID( channelName );
		}
	}

	[MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
	public ChanEntry[] ChanEntries = new ChanEntry[4];

	public Chan( ChanEntry[] entries )
	{
		for ( int i = 0; i < 4; ++i )
		{
			if ( i >= entries.Length )
				ChanEntries[i] = new();
			else
				ChanEntries[i] = entries[i];
		}

		ChannelCount = (ushort)entries.Length;
	}
}
