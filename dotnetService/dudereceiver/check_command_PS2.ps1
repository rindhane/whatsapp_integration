param($probe, $device, $status, $description);

$Uri = "http://localhost:5000/notification";
'''$Json = @{
    probe = ${probe};
    device   = $device;
    status     = $status;
    description   = $description;
    name = "dude-primary"
}'''
$name="dude-primary";
$text="$probe; $device; $status; $description ; $name";
#$content = "application/json";
$content="text/plain";
$request=[System.Net.WebRequest]::Create($Uri);
$request.ContentType=$content;
$request.Method = "POST";
$request.timeout = 60000;#1 minute
try
{
    $requestStream = $request.GetRequestStream()
    $streamWriter = New-Object System.IO.StreamWriter($requestStream)
    $streamWriter.Write($text)
    #$streamWriter.Write(($Json|ConvertTo-Json)) #ConvertTo-Json doesn't works in powershellv2
}

finally
{
    if ($null -ne $streamWriter) { $streamWriter.Dispose() }
    if ($null -ne $requestStream) { $requestStream.Dispose() }
}

$res = $request.GetResponse()

#$response.Close()
echo $res;
#powershell .\check_command_PS2.ps1 -probe switch -device device1(123) -status down -description no-ping
# powershell .\check_command_PS2.ps1 -probe switch -device "'device1(123)'" -status down -description no-ping
#powershell D:\Hexagon\check_command_PS2.ps1 -probe [Probe.Name] -device [Device.Name] -status [Service.Status] -description [Service.ProblemDescription]

#Reference :
    #2)https://stackoverflow.com/questions/25120703/invoke-webrequest-equivalent-in-powershell-v2
    #1)https://stackoverflow.com/questions/55915224/how-to-pass-json-parameter-with-post-method-on-powershell-2-0
    #3)https://social.technet.microsoft.com/Forums/ie/en-US/9380fa4a-ce23-4473-838c-84bdb65c763c/getresponse-with-quot0quot-arguments-quotthe-operation-has-timed-out?forum=winserverpowershell 
    #4) json seralizer for ps2 : https://stackoverflow.com/questions/28077854/powershell-2-0-convertfrom-json-and-convertto-json-implementation
    #5 escaping characters: https://stackoverflow.com/questions/36142786/how-to-escape-ampersands-semicolons-and-curly-braces-in-command-line-powershel/36143995