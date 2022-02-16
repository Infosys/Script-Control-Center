

strServer = WScript.Arguments.Item(0)
strPacketSize = WScript.Arguments.Item(1)
strTimeout = WScript.Arguments.Item(2)

Wscript.Echo "Pinging Server: " & strServer
Wscript.Echo "Packet Size: " & strPacketSize
Wscript.Echo "Timeout: " & strTimeout