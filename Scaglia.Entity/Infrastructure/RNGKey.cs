using System.Security.Cryptography;
using System.Text;

namespace Scaglia.Entity.Infrastructure
{
    public sealed class RNGKey
    {
        public static string New()
        {
            int maxSize = 36;
            char[] chars = new char[62];
            const string a = Constants.RNG_KEY;
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
                result.Append(chars[b % (chars.Length - 1)]);
            return result.ToString();
        }
    }
}
