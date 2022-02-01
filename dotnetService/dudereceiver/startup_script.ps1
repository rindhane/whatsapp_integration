$Env:DUDE_PORT='5000';
$Env:logFile="notifications.log";
echo "server starting....";
.\dudereceiver\dudereceiver.exe;
#dotnet run;