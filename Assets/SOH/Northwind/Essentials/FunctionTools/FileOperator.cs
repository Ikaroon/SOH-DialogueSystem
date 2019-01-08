using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Northwind.Essentials
{
    public static class FileOperator
    {
        
        private static string key
        {
            get
            {
                return "SPYON519";
            }
        }

        //TODO: Make Player Encryption Operators

        public static string GetProjectPath()
        {
            return Directory.GetParent(Application.dataPath).ToString() + "/";
        }

        public static void SaveToStringFile(string content, string path)
        {
            using (FileStream sw = File.Open(path, FileMode.OpenOrCreate))
            {
                Encoding enc = Encoding.UTF8;
                byte[] bytes = enc.GetBytes(content);
                sw.SetLength(0);
                sw.Write(bytes, 0, bytes.Length);
            }
        }

        public static void LoadFromJSONFile(string path, Object overwrite)
        {
            string content;
            using (StreamReader sr = File.OpenText(path))
            {
                content = sr.ReadLine();
            }
            JsonUtility.FromJsonOverwrite(content, overwrite);
        }

        public static string LoadStringFromFile(string path)
        {
            string content = "";
            if (!File.Exists(path))
            {
                return null;
            }
            using (StreamReader sr = File.OpenText(path))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }

        public static byte[] LoadBytesFromFile(string path)
        {
            byte[] data;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                br.Read(data, 0, (int)fs.Length);
                br.Close();
            }
            return data;
        }

        public static void SaveToFileDecrypted(Object obj, string path)
        {

        }

        public static void LoadFileFromEncrypted(string path, Object overwrite)
        {

        }

        public static byte[] LoadBytesFromCryptoFile(string path)
        {
            byte[] data;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fs.Length];
                BinaryReader br = new BinaryReader(fs);
                br.Read(data, 0, (int)fs.Length);
                br.Close();
            }

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] keyBytes = UE.GetBytes(key);

            byte[] buffer = new byte[data.Length];
            MemoryStream msCrypt = new MemoryStream(buffer, true);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(msCrypt,
                RMCrypto.CreateDecryptor(keyBytes, keyBytes),
                CryptoStreamMode.Read);

            int index = 0;
            while (index < data.Length)
                cs.WriteByte(data[index]);

            cs.Close();
            msCrypt.Close();

            return buffer;
        }

        public static void EncryptFile(string path)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] keyBytes = UE.GetBytes(key);

            byte[] oldBytes = LoadBytesFromFile(path);
            
            FileStream fsCrypt = new FileStream(path, FileMode.Open);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateEncryptor(keyBytes, keyBytes),
                CryptoStreamMode.Write);

            int index = 0;
            while (index < oldBytes.Length)
                cs.WriteByte(oldBytes[index]);
            
            cs.Close();
            fsCrypt.Close();
        }

        public static void DecryptFile(string path)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] keyBytes = UE.GetBytes(key);

            byte[] oldBytes = LoadBytesFromFile(path);

            FileStream fsCrypt = new FileStream(path, FileMode.Open);

            RijndaelManaged RMCrypto = new RijndaelManaged();
            
            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(keyBytes, keyBytes),
                CryptoStreamMode.Read);

            int index = 0;
            while (index < oldBytes.Length)
                cs.WriteByte(oldBytes[index]);

            cs.Close();
            fsCrypt.Close();
        }

        public static void CopyFile(string srcPath, string destPath, bool overwrite)
        {
            File.Copy(srcPath, destPath, overwrite);
            File.SetAttributes(destPath, FileAttributes.Normal);
        }

        public static void CreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void DeleteFolder(string path)
        {
            Directory.Delete(path);
        }
    }
}