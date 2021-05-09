using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CDS.APICore.Services
{
    public interface IHashService
    {
        HashResult Hash(string password);
        bool VerifyHash(string password, string passwordHash);
    }

    public class HashService : IHashService
    {
        private readonly HashAlgorithm _hashAlgorithm;

        public HashService()
        {
            _hashAlgorithm = SHA256.Create();
        }

        public HashResult Hash(string password)
        {
            return this.hash(password);
        }

        public bool VerifyHash(string password, string passwordHash)
        {
            return passwordHash == this.hash(password).Digest;
        }

        private HashResult hash(string password)
        {
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
            var passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            byte[] digestBytes = _hashAlgorithm.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashResult(Convert.ToBase64String(digestBytes));
        }
    }

    public class HashResult
    {
        public string Digest { get; set; }

        public HashResult(string digest)
        {
            this.Digest = digest;
        }
    }
}
