function test(x, y, z)
{
WScript.Echo("Parameters");
WScript.Echo(x);
WScript.Echo(y);
WScript.Echo(z);
}
var args = WScript.Arguments;
test(args.Item(0),args.Item(1),args.Item(2));
