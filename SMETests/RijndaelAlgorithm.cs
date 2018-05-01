using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Rijndael Algorithm
/// </summary>
public class RijndaelAlgorithm
{

    public RijndaelAlgorithm()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    /// <summary>
    /// The vendor token is constructed by concatenating the fields in the following format and then encrypting them using 128 bit Rijndael Algorithm. 
    /// The algorithm should use the Vendor Password as the key and Vendor Block as the Initialization Vector.
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <param name="vendorPassword">78AA2A2EF58A7931195C7AFCD441865F</param>
    /// <param name="vendorBlock">5487BACC306E03435A2D561F788DB49E</param>
    /// <param name="user">username</param>
    /// <param name="password">password</param>
    /// <param name="rememberMe">bool-checkbox</param>
    /// <returns></returns>
    public static string GetVendorToken(string returnUrl, string vendorPassword, string vendorBlock, string user, string password, bool rememberMe)
    {
        return Encrypt(GetTimeStamp() + "|" + returnUrl + "|" + user + "|" + password + "|" + rememberMe, vendorPassword, vendorBlock);
    }


    /// <summary>
    /// Encryption overrides CreateEncryptor() with two custom parameters password and block to create unique CryptoStream.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="password">Password is KEY</param>
    /// <param name="block">BLOCK is IV</param>
    /// <returns></returns>
    private static string Encrypt(string text, string password, string block)
    {
        SymmetricAlgorithm provider = null;
        MemoryStream buffer = null;
        CryptoStream writer = null;
        try
        {
            provider = new RijndaelManaged();
            buffer = new MemoryStream(text.Length);
            writer = new CryptoStream(buffer, provider.CreateEncryptor(FromHexaDecimal(password), FromHexaDecimal(block)), CryptoStreamMode.Write);
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(text);
            writer.Write(bytes, 0, bytes.Length);
            writer.Close();
            return ToHexaDecimal(buffer.ToArray());
        }
        finally
        {
            if (provider != null) provider.Clear();
            if (buffer != null) buffer.Close();
            //    writer.Close()
        }
    }

    /// <summary>
    /// Converts bytes to Hexadecimal string
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static string ToHexaDecimal(byte[] bytes)
    {
        if (bytes == null)
        {
            return "";
        }
        StringBuilder buffer = new StringBuilder();
        int length = bytes.Length;
        for (int n = 0; n <= length - 1; n++)
        {
            buffer.Append(string.Format("{0,2:x}", bytes[n]).Replace(" ", "0"));
        }
        return buffer.ToString();
    }


    public static string Decrypt(string encrypted, string password, string block)
    {
        SymmetricAlgorithm provider = null;
        MemoryStream buffer = null;
        CryptoStream reader = null;
        try
        {
            provider = new RijndaelManaged();
            byte[] bytes = FromHexaDecimal(encrypted);
            buffer = new MemoryStream(bytes.Length);
            reader = new CryptoStream(buffer, provider.CreateDecryptor(FromHexaDecimal(password), FromHexaDecimal(block)), CryptoStreamMode.Read);
            buffer.Write(bytes, 0, bytes.Length);
            buffer.Position = 0;
            string decrypted = new StreamReader(reader).ReadToEnd();
            reader.Close();
            return decrypted;
        }
        finally
        {
            if (provider != null) provider.Clear();
            if (buffer != null) buffer.Close();
            //    writer.Close()
        }
    }

    /// <summary>
    /// Converts Hexadeciaml String to Bytes.
    /// </summary>
    /// <param name="hexadecimal">"Password" & "Vendor Block"</param>
    /// <returns></returns>
    private static byte[] FromHexaDecimal(string hexadecimal)
    {
        if (hexadecimal == null || hexadecimal.Length == 0)
        {
            return new byte[0];
        }
        bool hasOddLength = (hexadecimal.Length & 1) == 1;
        if (hasOddLength)
        {
            throw new Exception("The hexadecimal string must have an even length (2 characters per byte).");
        }
        int length = hexadecimal.Length;
        byte[] bytes = new byte[Convert.ToInt32(length / 2)];
        for (int n = 0; n <= length - 2; n += 2)
        {
            string hexValue = hexadecimal.Substring(n, 2);
            bytes[Convert.ToInt32(n / 2)] = Convert.ToByte(hexValue, 16);
        }
        return bytes;
    }

    private static string GetTimeStamp()
    {
        StringBuilder buffer = new StringBuilder();
        DateTime now = DateTime.Now;
        buffer.Append(now.Year);
        buffer.Append(GetAsTwoDigits(now.Month));
        buffer.Append(GetAsTwoDigits(now.Day));
        buffer.Append(GetAsTwoDigits(now.Hour)); //Adjust for eastern time
        buffer.Append(GetAsTwoDigits(now.Minute));
        buffer.Append(GetAsTwoDigits(now.Second));
        buffer.Append(GetAsThreeDigits(now.Millisecond));
        return buffer.ToString();
    }

    private static string GetAsTwoDigits(int number)
    {
        return string.Format("{0,2:d}", number).Replace(" ", "0");
    }

    private static string GetAsThreeDigits(int number)
    {
        return string.Format("{0,3:d}", number).Replace(" ", "0");
    }
}