using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PasswordManager.Encryption
{
    class MyAes
    {

        #region Variablen
        AesCryptoServiceProvider crypto;
        #endregion

        public MyAes()
        {
            crypto = new AesCryptoServiceProvider
            {
                KeySize = 256,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
        }
        public String Encrypt(String clear_text)
        {
            ICryptoTransform transform = crypto.CreateEncryptor();
            byte[] encrypted_bytes = transform.TransformFinalBlock(
                Encoding.Default.GetBytes(clear_text),
                0,
                clear_text.Length);
            string str = Convert.ToBase64String(encrypted_bytes);
            return str;
        }
        public String Decrypt(String cipher_text)
        {
            ICryptoTransform transform = crypto.CreateDecryptor();
            byte[] enc_bytes = Convert.FromBase64String(cipher_text);
            byte[] decrypted_bytes = transform.TransformFinalBlock(
                enc_bytes,
                0,
                enc_bytes.Length);
            string str = Encoding.Default.GetString(decrypted_bytes);
            return str;
        }
        public void SetParams(String key, String iv_str)
        {
            byte[] Key_bytes = Encoding.Default.GetBytes(key);
            crypto.Key = Key_bytes;
            byte[] IV = Encoding.Default.GetBytes(iv_str);
            crypto.IV = IV;
        }
    }
}