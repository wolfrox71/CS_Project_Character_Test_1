using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace GameWIndowTest1.Global.Datatypes
{
    public class Password
    {
        private string _password;
        public Password(string E_Password, bool HashPassword = true)
        {
            // set the password to either the provided value or the value hashed
            _password = HashPassword ? HashString(E_Password) : E_Password;
        }

        // from https://www.codegrepper.com/profile/sean-lloyd
        public static string HashString(string text, string salt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
        public static string generateSalt(int length = 256)
        {
            Random rnd = new Random();
            char[] salt = new char[length];
            for (int i = 0; i < length; i++)
            {
                salt[i] = (char)(rnd.Next(40, 90));

            }
            return new string(salt);
        }
        public override string ToString()
        {
            return _password.ToString();
        }

        public bool Equals(String other)
        {
            // this was causing an issue
            return _password.ToString().Equals(other);
        }
        public static bool operator ==(Password lhs, String rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(Password lhs, String rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}

