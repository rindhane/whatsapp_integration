$path=Get-Location
$folder="publish" ;
$path=$path.tostring()+'\'+$folder;
$appFolder="dudereceiver"
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $folder ;
mv $folder\* $appFolder ;
Compress-Archive -Path $appFolder , check_command.ps1 , check_command_PS2.ps1 ,startup_script.ps1 -DestinationPath dudereceiver.zip;
#Get-ChildItem -Path $appFolder | Compress-Archive -DestinationPath dudereceiver.zip;
rm -r $appFolder\* ;
