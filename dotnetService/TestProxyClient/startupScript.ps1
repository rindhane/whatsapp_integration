echo "server starting....";
cd .\ProxyClient\;
.\TestProxyClient.exe;

#test commands:
#curl -v --request POST --header "Content-Type: text/plain" http://localhost:8000/trialMessage
#or try the check_command_script;
#powershell D:\Hexagon\check_command_PS2.ps1 -probe switch -device device1(123) -status down -description no-ping
