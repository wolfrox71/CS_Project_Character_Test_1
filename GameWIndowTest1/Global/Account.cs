using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using GameWIndowTest1.Global.Datatypes;

namespace GameWIndowTest1.Global
{
    public class Account
    {
        public Password password;
        public string username;
        public Account(string E_username, Password E_password)
        {
            username = E_username;
            password = E_password;
        }
        // overload for string for password not password datatype
        public Account(string E_username, string E_password, bool HashPassword = true)
        {
            username = E_username;
            password = new Password(E_password, HashPassword);
        }

        // this is for json
        public Account() { }

        public bool Equals(Account other)
        {
            return this == other;
        }
        public static bool operator ==(Account lhs, Account rhs)
        {
            if (lhs is null || rhs is null)
            {
                return false;
            }
            // if the usersnames and the passwords are the same
            // then the accounts are the same
            // the to string part was because without it wasnt working
            // i will try to fix at some point
            return lhs.username == rhs.username && lhs.ToString() == rhs.ToString();
        }
        public static bool operator !=(Account lhs, Account rhs)
        {
            return !(lhs == rhs);
        }
        public override string ToString()
        {
            return username + "\n" + password.ToString();
        }
    }
}
