using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PluginsCore
{
    sealed class Util
    {
        private const string tokenDecript = "tr1d3@smcp@ss";
        public static string DecryptString(string inputText)
        {
            var RijndaelCipher = new RijndaelManaged();
            byte[] EncryptedData = Convert.FromBase64String(inputText);
            byte[] Salt = Encoding.ASCII.GetBytes(tokenDecript.Length.ToString());
            var SecretKey = new PasswordDeriveBytes(tokenDecript, Salt);
            var Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            using (MemoryStream memoryStream = new MemoryStream(EncryptedData))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read))
                {
                    byte[] PlainText = new byte[EncryptedData.Length];
                    int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);
                    string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);
                    return DecryptedData;
                }
            }
        }

        /// <summary>
        /// Compara strVelho com strNovo para criar uma string que possua, sem repetição, os valores das duas strings.
        /// </summary>
        /// <param name="strVelho">String com valores separados por ponto e vírgula</param>
        /// <param name="strNovo">String com valores separados por ponto e vírgula</param>
        /// <returns>
        /// String com valores, sem repetição, concatenados e separados por vírgula.
        /// Caso não haja valores diferentes entre as duas, retorna strNovo.
        /// </returns>
        public static string UnirValoresDiferentesStringUnica(string strVelho, string strNovo)
        {
            if (string.IsNullOrEmpty(strVelho))
                return string.Empty;

            if (string.IsNullOrEmpty(strNovo))
                return string.Empty;

            string[] arrayVelho = strVelho.Split(';');
            string[] arrayNovo = strNovo.Split(';');
            string strRetorno = string.Empty;
            bool _flag = false;

            for (int index = 0; index < arrayNovo.Length; index++)
            {
                for (int indice = 0; indice < arrayVelho.Length; indice++)
                {
                    if (arrayNovo[index].ToLower().Equals(arrayVelho[indice].ToLower()))
                    {
                        _flag = false;
                        break;
                    }
                    else
                        _flag = true;
                }

                if (_flag)
                    strRetorno += string.Concat(arrayNovo[index], ";");
            }
            if (!string.IsNullOrEmpty(strRetorno))
            {
                strRetorno = strRetorno.Substring(0, strRetorno.Length - 1);
                return string.Concat(strVelho, ";", strRetorno);
            }
            else
                return strVelho;
        }

    }
}
