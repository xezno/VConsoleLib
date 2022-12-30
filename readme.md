# VConsoleLib

Connects to `vconsole2.exe` from Source 2 tools.
Targets current dota 2 release (protocol `0xD4`).

![image](https://user-images.githubusercontent.com/12881812/210095059-d3a7bd32-9fd0-40cb-884c-053ec0c67086.png)

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
