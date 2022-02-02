$buildFolder="publish";
$appFolder="serviceApp";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
#dotnet publish -c Release -r linux-x64 --self-contained true -o $buildFolder; #not working
mv $buildFolder\* $appFolder ;
Compress-Archive -Path $appFolder ,
                        startupScript.ps1,
                        check_command_PS2.ps1,
                        startup_preparation.ps1,
                        database_generator.sql `
                -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;