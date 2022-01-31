$buildFolder="publish";
$appFolder="serviceApp";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
mv $buildFolder\* $appFolder ;
Compress-Archive -Path $appFolder ,
                        startupScript.ps1,
                        startup_preparation.ps1,
                        database_generator.sql `
                -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;