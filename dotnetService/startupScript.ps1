#setting up environement variables
$Env:TWILIO_ACCOUNT_SID='account_sid';
$Env:TWILIO_AUTH_TOKEN='account token';
$Env:TEST_NUMBER="test number for trial";
$Env:ACCOUNT_NUMBER="whatsapp authorized number";
$Env:PUBLIC_ADDRESS="http://something.com";
$Env:DBNAME="Name.db";



#initating the service
dotnet run --project .\WhatsappService