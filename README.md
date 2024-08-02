# Descryption

The Unity dll's are XOR'd, and then the main one has some weird encryption on the .text section. I didn't care to figure it out, but you can hardcode the pointer to correct it, and then decompile it with a .NET decompiler.

This can be used to decrypt/re-encrypt the main login Lua script at login/CSharpObjectForLogin.lua, then re-encrypt with the same function, push it back into the unity3d file, then voila! You have Lua scripting within ROM.