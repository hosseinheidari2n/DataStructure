using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataStructureProject
{
    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Hash { get; set; }

        public Token(string type, string value)
        {
            Type = type;
            Value = value;
            Hash = GetHash(value);
        }

        // Calculate Hash
        private int GetHash(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(value);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToInt32(hashBytes, 0);
            }
        }
    }
}
