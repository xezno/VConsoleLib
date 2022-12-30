# VConsoleLib

Connects to `vconsole2.exe` from Source 2 tools.
Targets current dota 2 release (protocol `0xD4`).

## Usage

Create a server
```cs
var vconsoleServer = new VConsoleServer();
```

Log to the server
```cs
vconsoleServer.Log( s );
```

Register command events
```cs
vconsoleServer.OnCommand += ( s ) =>
{
	Console.WriteLine( $"Command: {s}" );
};
```