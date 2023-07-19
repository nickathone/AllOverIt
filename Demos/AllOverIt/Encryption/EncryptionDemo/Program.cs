using System;
using System.Security.Cryptography;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var rsa = RSA.Create(3072);

        var publicKeyBytes = rsa.ExportRSAPublicKey();
        var privateKeyBytes = rsa.ExportRSAPrivateKey();




        // testing conversions

        var publicKeyString = Convert.ToBase64String(publicKeyBytes);
        var privateKeyString = Convert.ToBase64String(privateKeyBytes);

        var publicKey = Convert.FromBase64String(publicKeyString);
        var privateKey = Convert.FromBase64String(privateKeyString);




        var testString = "yo yo";
        byte[] encrypted;
        byte[] decrypted;

        using (var rsa2 = new RSACryptoServiceProvider())
        {
            rsa2.ImportRSAPublicKey(publicKey, out _);
            encrypted = rsa2.Encrypt(Encoding.UTF8.GetBytes(testString), true);
        }

        using (var rsa3 = new RSACryptoServiceProvider())
        {
            rsa3.ImportRSAPrivateKey(privateKey, out _);
            decrypted = rsa3.Decrypt(encrypted, true);
        }

        var decoded = Encoding.UTF8.GetString(decrypted);
    }
}