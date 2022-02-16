

Set colNamedArguments = WScript.Arguments.Named

strServer = colNamedArguments.Item("Server")
strPacketSize = colNamedArguments.Item("PacketSize")
strTimeout = colNamedArguments.Item("Timeout")
Wscript.Echo "Server Name: " & strServer
Wscript.Echo "Packet Size: " & strPacketSize
Wscript.Echo "Timeout (ms): " & strTimeout