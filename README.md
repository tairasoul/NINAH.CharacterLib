# CharacterLib

character library mod for No, I'm Not A Human

should be mostly functional, only seems to be missing the capability to add specific day visits and proper handling of character removal
  - i need to read through more decompiled code to figure out how to do this

## general info

you'll primarily be interacting with the CustomCharacterBuilder class (or the base CustomCharacter class if you want to bypass that entirely)

dialogue is written using YarnSpinner, the game uses 2.4 specifically (as does this mod)

when positioning characters in rooms, it's recommended to use UnityExplorer as there's no easy way to do it currently

you can find dialogue-related info in [dialogue_info.md](https://github.com/tairasoul/NINAH.CharacterLib/blob/main/dialogue_info.md)

## node-related info

entrance_{CID} - the node played when your character appears at the door. unsure if there's multiple or not

talk_{CID}_(number) - the nodes played when talking to your character. this starts at 2, increments by one per day

when registering a node redirect, {CID} will be replaced in both from and to, but it's recommended to only use it in from as the character enum id can change