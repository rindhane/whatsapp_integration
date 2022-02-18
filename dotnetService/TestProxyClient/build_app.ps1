$buildFolder="publish";
$appFolder="ProxyClient";
rm -r $appFolder ;
rm config_temp.json;#json files are being automatically pulled by the publish statement
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
mv $buildFolder\* $appFolder ;
cp "RECORD.db" , "sqlite3.exe" $appFolder ; #configuration file for proxy client
Compress-Archive -Path $appFolder ,
                        check_command_PS2.ps1,
                        startupScript.ps1 `
                -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;