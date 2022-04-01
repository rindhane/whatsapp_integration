$buildFolder="publish";
$appFolder="serverControl";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
#dotnet publish -c Release -r linux-x64 --self-contained true -o $buildFolder; #not working
mv $buildFolder\* $appFolder ;
#cp -r WhatsappService\chartLib $appFolder ; #copy other files relevant to dependency like config files
Compress-Archive -Path $appFolder -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;