$path=Get-Location
$folder="publish" ;
$path=$path.tostring()+'\'+$folder;
$appFolder="dudeNotifier"
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $folder ;
mv $folder\* $appFolder ;
Compress-Archive -Path $appFolder , settings.json, dudeCommand.txt -DestinationPath dudeNotifier.zip;
rm -r $appFolder\* ;
