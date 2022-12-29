using System.Net;
using System.Net.Sockets;
using System.Text;

namespace VConsoleLib;

public class VConsoleServer
{
	private TcpListener tcpListener;
	private TcpClient tcpClient;
	private NetworkStream stream;

	public Action<string> OnCommand;

	public bool Connected;

	public VConsoleServer()
	{
		tcpListener = new TcpListener( IPAddress.Loopback, 29100 );
		tcpListener.Start();

		var thread = new Thread( ListenThread );
		thread.Start();
	}

	private void EncodeAndSend<T>( string identifier, T obj ) where T : struct
	{
		List<byte> sendBuffer = new();
		var encodedIdentifier = Encoding.ASCII.GetBytes( identifier );

		var data = Serializer.ToBytes( obj );

		// Header
		sendBuffer.AddRange( encodedIdentifier );
		sendBuffer.AddRange( new byte[] { 0x00, 0xd4 } ); // Protocol
		sendBuffer.AddRange( BitConverter.GetBytes( data.Length + 12 ).Reverse() ); // Length
		sendBuffer.Add( 0x00 ); // Padding

		sendBuffer.Add( (identifier == "PPCR") ? (byte)0x01 : (byte)0x00 ); // Don't know

		// Body
		sendBuffer.AddRange( data ); // data

		stream.Write( sendBuffer.ToArray(), 0, sendBuffer.Count );
	}

	public void Log( string str, uint color = 0xFFFFFFFF )
	{
		if ( !Connected )
			return;

		var prntPacket = new Prnt( "Managed", str + "\n", color );
		EncodeAndSend( "PRNT", prntPacket );
	}

	private void ListenThread()
	{
		while ( true )
		{
			Connected = false;

			byte[] buf = new byte[128];

			tcpClient = tcpListener.AcceptTcpClient();
			stream = tcpClient.GetStream();

			// Create pipe
			var ppcrPacket = new Ppcr();
			EncodeAndSend( "PPCR", ppcrPacket );

			// Send addon info
			var ainfPacket = new AInf( "My Game", "C:\\MyGame.exe" );
			EncodeAndSend( "AINF", ainfPacket );

			// Send channels
			var chanPacket = new Chan( new[] {
				new Chan.ChanEntry("Managed")
			} );
			EncodeAndSend( "CHAN", chanPacket );

			Connected = true;

			// Notify vconsole
			Log( "Connected to VConsole using VConsoleLib." );

			while ( (_ = stream.Read( buf, 0, buf.Length )) > 0 )
			{
				var bufStr = Encoding.ASCII.GetString( buf );
				var identifier = bufStr[..4];

				if ( identifier == "CMND" )
				{
					using var memStream = new MemoryStream( buf );
					using var binaryReader = new BinaryReader( memStream );

					_ = binaryReader.ReadBytes( 8 );
					_ = binaryReader.ReadByte();
					var length = binaryReader.ReadInt16();
					_ = binaryReader.ReadByte();

					var currByte = (byte)0;
					var strbytes = new List<byte>();
					do
					{
						currByte = binaryReader.ReadByte();
						strbytes.Add( currByte );
					} while ( currByte != 0 );

					var str = Encoding.ASCII.GetString( strbytes.ToArray() );
					OnCommand?.Invoke( str );
				}
			}
		}
	}
}
