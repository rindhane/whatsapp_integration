param($probe, $device, $status, $description);

#$Uri = 'http://10.192.65.63:5000/notification';
$Uri = 'http://localhost:8000/notification';
$name="dude-primary";
$text="$probe; $device; $status; $description ; $name";
$content="text/plain";
$request=[System.Net.WebRequest]::Create($Uri);
$request.ContentType=$content;
$request.Method = "POST";
$request.timeout = 60000;#1 minute

try
{
    $requestStream = $request.GetRequestStream();
    $streamWriter = New-Object System.IO.StreamWriter($requestStream);
    $streamWriter.Write($text);
}

finally
{
    if ($null -ne $streamWriter) { $streamWriter.Dispose() }
    if ($null -ne $requestStream) { $requestStream.Dispose() }
}

$res = $request.GetResponse();
#$response.Close()
echo $res;
#powershell D:\Hexagon\check_command_PS2.ps1 -probe switch -device device1(123) -status down -description no-ping
#powershell D:\Hexagon\check_command_PS2.ps1 -probe [Probe.Name] -device [Device.Name] -status [Service.Status] -description [Service.ProblemDescription]
