$buildFolder="publish";
$appFolder="dbApp";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
mv $buildFolder\* $appFolder ;
cp dbData.txt $appFolder;
Compress-Archive -Path $appFolder -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;