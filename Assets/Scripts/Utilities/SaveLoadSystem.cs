using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public static class SaveLoadSystem
{
    private const string EncryptionKey = "mysecretkey123456";

    public static async void Save<T>(T data,bool isEncrypt = true, string fileName = null)
    {
        fileName ??= $"{data.GetType().Name}.json";
        var json = JsonUtility.ToJson(data);
        var encryptedData = isEncrypt ? Encrypt(json) : json;
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        await File.WriteAllTextAsync(filePath, encryptedData);
    }

    public static T Load<T>(bool isEncrypt = true,string fileName = null) where T : new()
    {
        fileName ??= $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            var encryptedData = File.ReadAllText(filePath);
            var decryptedData = isEncrypt ? Decrypt(encryptedData) : encryptedData;
            return JsonUtility.FromJson<T>(decryptedData);
        }
        Debug.LogWarning($"{fileName} file not found!");
        return new T();
    }

    private static string Encrypt(string data)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = new byte[16];
        Array.Copy(Encoding.UTF8.GetBytes(EncryptionKey),
            aesAlg.Key,
            Math.Min(EncryptionKey.Length, aesAlg.Key.Length));
        aesAlg.IV = new byte[16];
        var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        using var msEncrypt = new MemoryStream();
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        {
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(data);
            }
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    private static string Decrypt(string data)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = new byte[16];
        Array.Copy(Encoding.UTF8.GetBytes(EncryptionKey),
            aesAlg.Key,
            Math.Min(EncryptionKey.Length, aesAlg.Key.Length));
        aesAlg.IV = new byte[16];
        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
        using var msDecrypt = new MemoryStream(Convert.FromBase64String(data));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}
