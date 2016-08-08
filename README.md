# pokemongo-encryption-sharp
Managed C# code for the Pokemon Go API request signature encryption (port of "encrypt.c")


________

Project contains a fully managed port of the `encrypt.c` code as well as a p/invoke wrapper for the native encrypt.dll binaries. 

The managed lib is a PCL that targets .NET 4.5.1, ASP.NET Core, Xamarin.iOS/Android/Mac.


Both libs should work on about any mono platform. However, only the 32bit & 64bit encrypt.dll Windows binaries are included. 


________

### Sample usage

```csharp
byte[] input = System.Text.Encoding.UTF8.GetBytes("Sample input data..");
byte[] iv = new byte[32];
new Random().NextBytes(iv);

// using managed lib
byte[] output = PokemonGoEncryptSharp.Util.Encrypt(input, iv);

// using native wrapper lib
byte[] output = PokemonGoEncryptNative.Util.Encrypt(input, iv);

```

_______

### Performance
Results from running the test app
```
Success! Managed code produces same output as native
Performing 3000 encryption operations with both native and managed functions...
native took 0.1162 ms per round, total: 348.63 ms
managed took 0.2669 ms per round, total: 800.9 ms
managed is 2.3 times slower
```
