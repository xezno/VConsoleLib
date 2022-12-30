# VConsoleLib

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