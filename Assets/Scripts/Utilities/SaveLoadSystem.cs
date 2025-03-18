using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

public static class SaveLoadSystem
{
    public static void Save<T>(T data, bool isEncrypt = true, string fileName = null)
    {
        fileName ??= $"{data.GetType().Name}.json";
        var json = JsonUtility.ToJson(data);
        var encryptedData = isEncrypt ? Encrypt(json) : json;
        var filePath = Path.Combine(Application.persistentDataPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(encryptedData);
        }
    }

    public static T Load<T>(bool isEncrypt = true, string fileName = null) where T : new()
    {
        fileName ??= $"{typeof(T).Name}.json";
        var filePath = Path.Combine(Application.persistentDataPath, fileName);

        if (File.Exists(filePath))
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new StreamReader(stream))
            {
                var encryptedData = reader.ReadToEnd();
                var decryptedData = isEncrypt ? Decrypt(encryptedData) : encryptedData;
                return JsonUtility.FromJson<T>(decryptedData);
            }
        }
        Debug.LogWarning($"{fileName} file not found!");
        return new T();
    }

    private static string Encrypt(string data)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = new byte[16];
        Array.Copy(Encoding.UTF8.GetBytes(GameConstants.EncryptionKey),
            aesAlg.Key,
            Math.Min(GameConstants.EncryptionKey.Length, aesAlg.Key.Length));
        aesAlg.IV = new byte[16];

        using var msEncrypt = new MemoryStream();
        using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(data);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    private static string Decrypt(string data)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = new byte[16];
        Array.Copy(Encoding.UTF8.GetBytes(GameConstants.EncryptionKey),
            aesAlg.Key,
            Math.Min(GameConstants.EncryptionKey.Length, aesAlg.Key.Length));
        aesAlg.IV = new byte[16];

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(data));
        using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        using (var srDecrypt = new StreamReader(csDecrypt))
        {
            return srDecrypt.ReadToEnd();
        }
    }
}