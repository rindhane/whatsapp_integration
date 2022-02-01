#setting up environement variables
$Env:TWILIO_ACCOUNT_SID='account_sid';
$Env:TWILIO_AUTH_TOKEN='account token';
$Env:TEST_NUMBER="test number for trial";
$Env:ACCOUNT_NUMBER="whatsapp authorized number";
$Env:PUBLIC_ADDRESS="http://something.com";
$Env:DBNAME="Name.db";
$Env:logFile="notifications.log";
$Env:httpPort="8000";
$Env:httpsPort="8080";



#initating the service
#dotnet run --project .\WhatsappService
cd serviceApp
.\WhatsappService.exe ;