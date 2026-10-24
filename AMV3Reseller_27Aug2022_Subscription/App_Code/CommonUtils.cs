using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Summary description for CommonUtils
/// </summary>
public class CommonUtils
{
    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    public static string Encrypt(string plainText, string passPhrase)
    {
        // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
        // so that the same Salt and IV values can be used when decrypting.  
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
        // Get the complete stream of bytes that represent:
        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            // var plainTextBytes = Convert.FromBase64String(cipherText.Replace("", "+"));

                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            // Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }

    ///// <summary>
    /////     ''' Encrypts a string
    /////     ''' </summary>
    /////     ''' <param name="PlainText">Text to be encrypted</param>
    /////     ''' <param name="Password">Password to encrypt with</param>
    /////     ''' <param name="Salt">Salt to encrypt with</param>
    /////     ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
    /////     ''' <param name="PasswordIterations">Number of iterations to do</param>
    /////     ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
    /////     ''' <param name="KeySize">Can be 128, 192, or 256</param>
    /////     ''' <returns>An encrypted string</returns>
    //public static string Encrypt(string PlainText, string Password , string Salt = "", string HashAlgorithm = "SHA1", int PasswordIterations = 2, string InitialVector = "", int KeySize = 256)
    //{
    //    if (string.IsNullOrEmpty(PlainText))
    //        return "";
    //    byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
    //    byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
    //    byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
    //    PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
    //    byte[] KeyBytes = DerivedPassword.GetBytes((int)(KeySize / (double)8));
    //    RijndaelManaged SymmetricKey = new RijndaelManaged();
    //    SymmetricKey.Mode = CipherMode.CBC;
    //    byte[] CipherTextBytes = null;
    //    using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
    //    {
    //        using (MemoryStream MemStream = new MemoryStream())
    //        {
    //            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
    //            {
    //                CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
    //                CryptoStream.FlushFinalBlock();
    //                CipherTextBytes = MemStream.ToArray();
    //                MemStream.Close();
    //                CryptoStream.Close();
    //            }
    //        }
    //    }
    //    SymmetricKey.Clear();
    //    return Convert.ToBase64String(CipherTextBytes);
    //}

    ///// <summary>
    /////     ''' Decrypts a string
    /////     ''' </summary>
    /////     ''' <param name="CipherText">Text to be decrypted</param>
    /////     ''' <param name="Password">Password to decrypt with</param>
    /////     ''' <param name="Salt">Salt to decrypt with</param>
    /////     ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
    /////     ''' <param name="PasswordIterations">Number of iterations to do</param>
    /////     ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
    /////     ''' <param name="KeySize">Can be 128, 192, or 256</param>
    /////     ''' <returns>A decrypted string</returns>
    //public static string Decrypt(string CipherText, string Password, string Salt = "", string HashAlgorithm = "", int PasswordIterations = 2, string InitialVector = "", int KeySize = 256)
    //{
    //    if (string.IsNullOrEmpty(CipherText))
    //        return "";
    //    byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
    //    byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
    //    byte[] CipherTextBytes = Convert.FromBase64String(CipherText.Replace(" ", "+"));
    //    PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
    //    byte[] KeyBytes = DerivedPassword.GetBytes((int)(KeySize / (double)8));
    //    RijndaelManaged SymmetricKey = new RijndaelManaged();
    //    SymmetricKey.Mode = CipherMode.CBC;
    //    byte[] PlainTextBytes = new byte[CipherTextBytes.Length - 1 + 1];
    //    int ByteCount = 0;
    //    using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
    //    {
    //        using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
    //        {
    //            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
    //            {
    //                ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
    //                MemStream.Close();
    //                CryptoStream.Close();
    //            }
    //        }
    //    }
    //    SymmetricKey.Clear();
    //    return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
    //}

    ///// <summary>
    /////     ''' Encrypts a string
    /////     ''' </summary>
    /////     ''' <param name="PlainText">Text to be encrypted</param>
    /////     ''' <param name="Password">Password to encrypt with</param>
    /////     ''' <param name="Salt">Salt to encrypt with</param>
    /////     ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
    /////     ''' <param name="PasswordIterations">Number of iterations to do</param>
    /////     ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
    /////     ''' <param name="KeySize">Can be 128, 192, or 256</param>
    /////     ''' <returns>An encrypted string</returns>
    //public static string Encrypt(string PlainText, string Password, string Salt = "", string HashAlgorithm = "SHA1", int PasswordIterations = 2, string InitialVector = "", int KeySize = 256)
    //{
    //    if (string.IsNullOrEmpty(PlainText))
    //        return "";
    //    byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
    //    byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
    //    byte[] PlainTextBytes = Encoding.UTF8.GetBytes(PlainText);
    //    PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
    //    byte[] KeyBytes = DerivedPassword.GetBytes((int)(KeySize / (double)8));
    //    RijndaelManaged SymmetricKey = new RijndaelManaged();
    //    SymmetricKey.Mode = CipherMode.CBC;
    //    byte[] CipherTextBytes = null;
    //    using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
    //    {
    //        using (MemoryStream MemStream = new MemoryStream())
    //        {
    //            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
    //            {
    //                CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
    //                CryptoStream.FlushFinalBlock();
    //                CipherTextBytes = MemStream.ToArray();
    //                MemStream.Close();
    //                CryptoStream.Close();
    //            }
    //        }
    //    }
    //    SymmetricKey.Clear();
    //    return Convert.ToBase64String(CipherTextBytes);
    //}

    ///// <summary>
    /////     ''' Decrypts a string
    /////     ''' </summary>
    /////     ''' <param name="CipherText">Text to be decrypted</param>
    /////     ''' <param name="Password">Password to decrypt with</param>
    /////     ''' <param name="Salt">Salt to decrypt with</param>
    /////     ''' <param name="HashAlgorithm">Can be either SHA1 or MD5</param>
    /////     ''' <param name="PasswordIterations">Number of iterations to do</param>
    /////     ''' <param name="InitialVector">Needs to be 16 ASCII characters long</param>
    /////     ''' <param name="KeySize">Can be 128, 192, or 256</param>
    /////     ''' <returns>A decrypted string</returns>
    //public static string Decrypt(string CipherText, string Password, string Salt = "", string HashAlgorithm = "", int PasswordIterations = 2, string InitialVector = "", int KeySize = 256)
    //{
    //    if (string.IsNullOrEmpty(CipherText))
    //        return "";
    //    byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
    //    byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
    //    byte[] CipherTextBytes = Convert.FromBase64String(CipherText.Replace(" ", "+"));
    //    PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
    //    byte[] KeyBytes = DerivedPassword.GetBytes((int)(KeySize / (double)8));
    //    RijndaelManaged SymmetricKey = new RijndaelManaged();
    //    SymmetricKey.Mode = CipherMode.CBC;
    //    byte[] PlainTextBytes = new byte[CipherTextBytes.Length - 1 + 1];
    //    int ByteCount = 0;
    //    using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
    //    {
    //        using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
    //        {
    //            using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
    //            {
    //                ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
    //                MemStream.Close();
    //                CryptoStream.Close();
    //            }
    //        }
    //    }
    //    SymmetricKey.Clear();
    //    return Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);
    //}
}
