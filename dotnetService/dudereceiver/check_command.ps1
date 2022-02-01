param($probe, $device, $status, $description);

$Uri = 'http://localhost:5000/notification';
$Json = @{
    probe  = $probe;
    device   = $device;
    status     = $status;
    description   = $description;
    name = "dude-primary";
}
$content = "application/json"; 
$Result = Invoke-RestMethod -Method Post -Uri $Uri -ContentType $content -Body ($Json|ConvertTo-Json); 
echo $Result;
#powershell .\check_command.ps1 -probe switch -device device1(123) -status down -description no-ping
#powershell .\check_command.ps1 -probe [Probe.Name] -device [Device.Name] -status [Service.Status] -description [Service.ProblemDescription]