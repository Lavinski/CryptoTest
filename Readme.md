# Cross platform crypto, AES in javascript with CryptoJs and .net in C# 

This is a sample project that clearly shows how to encrypt a message in javascript on the client and decrypt it on the server using .NET and C#.

The key aspect of this project is interoperability between the languages. Given a password and salt either .NET or Javascript is able to regenerate the Key and Initialization Vector allowing them to decrypt messages encrypted by the other. 

The logic is located primrily in these two files:

- [Index.js: Javascript side](https://github.com/Lavinski/CryptoTest/blob/master/CryptoTest/Scripts/Views/Home/Index.js)
- [ServiceController.cs: C# side](https://github.com/Lavinski/CryptoTest/blob/master/CryptoTest/Controllers/ServiceController.cs)

Also see the [the official documentation for CryptoJs](http://code.google.com/p/crypto-js/#AES)