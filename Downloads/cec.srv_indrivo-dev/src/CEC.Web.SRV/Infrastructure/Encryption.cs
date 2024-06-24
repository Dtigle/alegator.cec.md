using System.Configuration;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CEC.Web.SRV.Infrastructure
{
	public class Encryption
	{
		protected static string _Key = "";
		protected static string EncryptionKey
		{
			get
			{
				if (String.IsNullOrEmpty(_Key))
				{
					_Key = ConfigurationManager.AppSettings["EncryptionKey"];
				}

				return _Key;
			}
		}
		public static string Encrypt(string clearText)
		{
			byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
			using (Aes encryptor = Aes.Create())
			{
				byte[] IV = new byte[15];
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
					{
						cs.Write(clearBytes, 0, clearBytes.Length);
						cs.Close();
					}
					clearText = Convert.ToBase64String(IV) + Convert.ToBase64String(ms.ToArray());
				}
			}
			return clearText;
		}
		public static string Decrypt(string cipherText)
		{
			byte[] IV = Convert.FromBase64String(cipherText.Substring(0, 20));
			cipherText = cipherText.Substring(20).Replace(" ", "+");
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			using (Aes encryptor = Aes.Create())
			{
				Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
				encryptor.Key = pdb.GetBytes(32);
				encryptor.IV = pdb.GetBytes(16);
				using (MemoryStream ms = new MemoryStream())
				{
					using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
					{
						cs.Write(cipherBytes, 0, cipherBytes.Length);
						cs.Close();
					}
					cipherText = Encoding.Unicode.GetString(ms.ToArray());
				}
			}
			return cipherText;
		}
	}
}