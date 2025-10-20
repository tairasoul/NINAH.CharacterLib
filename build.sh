dotnet build -p:Configuration=Release
rm ../ninah-gamedir/BepInEx/plugins/CharacterLib.dll
rm ../ninah-gamedir/BepInEx/plugins/CharacterLib.pdb
cp bin/Release/net6.0/CharacterLib.dll ../ninah-gamedir/BepInEx/plugins/
cp bin/Release/net6.0/CharacterLib.pdb ../ninah-gamedir/BepInEx/plugins/
