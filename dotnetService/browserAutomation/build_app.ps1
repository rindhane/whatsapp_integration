$buildFolder="publish";
$appFolder="BrowserApp";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
mv $buildFolder\* $appFolder ;
#cp -r proxyData.txt $appFolder ;
Compress-Archive -Path $appFolder -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;