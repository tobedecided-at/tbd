using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TBD.WS;
using TBD.IO;
using TBD.Globals;

namespace TBD.Crypto {

  public static class AesCrypto {
    public static RSACryptoServiceProvider Generate(double bytes = 4096, bool shouldGen = true) {
      if (shouldGen) {
        var rsa = new RSACryptoServiceProvider((int)bytes);
        rsa.PersistKeyInCsp = false;
        return rsa;
      } else {
        var rsa = new RSACryptoServiceProvider();
        rsa.PersistKeyInCsp = false;
        return rsa;
      }
    }

    public static IEncryptionResult EncryptString(string toEncrypt, string password, int iterations = 5000) {
      var b_toEncrypt = Encoding.Unicode.GetBytes(toEncrypt);
      var b_password = Encoding.Unicode.GetBytes(password);

      return Encrypt(b_toEncrypt, b_password, iterations);
    }

    public static IEncryptionResult Encrypt(byte[] b_toEncrypt, byte[] b_password, int iterations = 5000) {
      byte[] encryptedBytes = null;
      byte[] b_salt = null;

      using (var ms = new MemoryStream()) {
        using (Aes aes = Aes.Create()) {

          aes.KeySize = 256;
          aes.BlockSize = 128;

          var cryptoProvider = new RNGCryptoServiceProvider();
          
          b_salt = new byte[256];
          cryptoProvider.GetBytes(b_salt);

          var key = new Rfc2898DeriveBytes(b_password, b_salt, iterations);
          aes.Key = key.GetBytes(aes.KeySize / 8);
          aes.IV = key.GetBytes(aes.BlockSize / 8);
          aes.Mode = CipherMode.CBC;

          using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
            cs.Write(b_toEncrypt, 0, b_toEncrypt.Length);
            cs.Close();
          }
          encryptedBytes = ms.ToArray();
        }
      }

      return new EncryptionResult(iterations, b_salt, encryptedBytes, b_password);
    }

    public static string Decrypt(string s_toDecrypt, string s_password) {
      byte[] b_decrypted = null;
      byte[] b_toDecrypt = null;
      byte[] b_password = System.Text.Encoding.Unicode.GetBytes(s_password);

      using (var ms = new MemoryStream()) {
        using (Aes aes = Aes.Create()) {
          aes.KeySize = 256;
          aes.BlockSize = 128;

          string[] parts = s_toDecrypt.Split('.');

          int i_iterations = int.Parse(parts[0]);
          byte[] b_salt = Convert.FromBase64String(parts[1]);
          b_toDecrypt = Convert.FromBase64String(parts[2]);
          
          var key = new Rfc2898DeriveBytes(b_password, b_salt, i_iterations);
          aes.Key = key.GetBytes(aes.KeySize / 8);
          aes.IV = key.GetBytes(aes.BlockSize / 8);
          aes.Mode = CipherMode.CBC;

          try {
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
              cs.Write(b_toDecrypt, 0, b_toDecrypt.Length);
              cs.Close();
            }
          } catch (CryptographicException) {
            return "PASSWORD_INVALID";
          }

          b_decrypted = ms.ToArray();
        }
      }

      return System.Text.Encoding.Unicode.GetString(b_decrypted);
    }

    public class EncryptionResult : IEncryptionResult {
      public int iterations = 5000;
      public byte[] b_salt = null;
      public byte[] b_data = null;
      public byte[] b_password = null;
      public bool isPasswordPresent = false;

      public EncryptionResult(int i, byte[] salt, byte[] data, byte[] b_password = null) {
        this.iterations = i; this.b_salt = salt; this.b_data = data;
        if (b_password != null) {
          this.b_password = b_password;
          this.isPasswordPresent = true;
        }
      }

      public EncryptionResult() {}

      public string Serialize(bool includePassword = false) {
        string b64_salt = Convert.ToBase64String(b_salt);
        string b64_data = Convert.ToBase64String(b_data);
        string b64_password = Convert.ToBase64String(b_password);

        if (includePassword)
          return string.Format("{0}.{1}.{2}.{3}", iterations, b64_salt, b64_data, b64_password);
        else
          return string.Format("{0}.{1}.{2}", iterations, b64_salt, b64_data);

      }

      public static EncryptionResult Deserialize(string serializedString) {
        string[] parts = serializedString.Split('.');

        if (parts.Length < 3) throw new FormatException();
        
        int iterations = int.Parse(parts[0]);
        byte[] b_salt = Convert.FromBase64String(parts[1]);
        byte[] b_data = Convert.FromBase64String(parts[2]);

        if (parts.Length == 3) {
          // Without password
          return new EncryptionResult(iterations, b_salt, b_data);
        } else if (parts.Length == 4) {
          // With password
          byte[] b_password = Convert.FromBase64String(parts[3]);
          return new EncryptionResult(iterations, b_salt, b_data, b_password);
        }
        return null;
      }
    }

    public interface IEncryptionResult {
      string Serialize(bool includePassword = false);
    }
  }

  public static class RsaCrypto {
    public static RSACryptoServiceProvider Generate(double bytes = 4096, bool shouldGen = true) {
      if (shouldGen) {
        var rsa = new RSACryptoServiceProvider((int)bytes);

        rsa.PersistKeyInCsp = false;
        return rsa;
      } else {
        var rsa = new RSACryptoServiceProvider();
        rsa.PersistKeyInCsp = false;
        return rsa;
      }
    }
  }

  public static class JWToken {

		public static byte[] secret = new byte[512];
    
    public enum JWTErrorCode {
      Valid,
      Tampered,
      Expired,
      Invalid,
    }

    public static string CreateToken(string username) {
      DateTimeOffset now = DateTimeOffset.UtcNow;

      var dHeader = new Dictionary<string, object> {
        { "alg", "H512" },
        { "typ", "JWT" }
      };

      var dPayload = new Dictionary<string, object> {
        { "iss", "wss://tobedecided.at/auth" },
        { "iat", now.ToUnixTimeSeconds() },
        { "nbf", now.ToUnixTimeSeconds() },
        { "exp", now.AddHours(2).ToUnixTimeSeconds() },
        { "aud", "TBD/auth" },
        { "sub", username },

        { "state", JWTErrorCode.Valid },
        { "clearance", Io.GetPermissionLevel(username).Result }
      };

			string sHeader = JsonConvert.SerializeObject(dHeader);
			string sPayload = JsonConvert.SerializeObject(dPayload);
      string sHeaderB64 = Base64UrlEncode(sHeader);
      string sPayloadB64 = Base64UrlEncode(sPayload);

      var sha512 = new HMACSHA512(secret);
      byte[] sha512Hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(sHeader +"."+ sPayload));

      string sHashB64 = Base64UrlEncode(sha512Hash);

      return sHeaderB64 + "." + sPayloadB64 + "." + sHashB64;
    }

    // TODO
    public static bool ValidateToken(string token) {
			JObject t = DecodeToken(token);

			if ((int)t["state"] == (int)JWTErrorCode.Tampered) return false;

			string iss = (string)t["iss"];
			DateTimeOffset iat = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse((string)t["iat"]));
			DateTimeOffset nbf = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse((string)t["nbf"]));
			DateTimeOffset exp = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse((string)t["exp"]));

			DateTimeOffset now = DateTimeOffset.UtcNow;

			if (iat > now || nbf > now || exp < now) t["state"] = (int)JWTErrorCode.Expired;

			return (int)t["state"] == (int)JWTErrorCode.Valid;
    }

    // TODO
    public static JObject DecodeToken(string token) {
      try {
				// Inside of try because it may be in an invalid format
				string sTokenHeader = Encoding.UTF8.GetString(Base64UrlDecode(token.Split('.')[0]));
				string sTokenPayload = Encoding.UTF8.GetString(Base64UrlDecode(token.Split('.')[1]));
				byte[] sTokenHash = Base64UrlDecode(token.Split('.')[2]);

        if (sTokenHeader == null || sTokenPayload == null || sTokenHash == null)
          throw new FormatException("Token is in invalid format!");

				// Check for tampering
				var sha512 = new HMACSHA512(secret);
				byte[] sha512Hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(sTokenHeader + "." + sTokenPayload));

				if (!sTokenHash.SequenceEqual(sha512Hash)) return new { state = JWTErrorCode.Tampered }.toJObject();

				return sTokenPayload.toJObject();
      } catch {
        return new { state = JWTErrorCode.Invalid }.toJObject();
      }
    }

    public static byte[] CreateSHA1 (string origin) {
      return new SHA1CryptoServiceProvider().ComputeHash(Encoding.Unicode.GetBytes(origin));
    }

    private static string Base64UrlEncode(string input) {
      return Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    private static string Base64UrlEncode(byte[] input) {
      // Special "url-safe" base64 encode.
      return Convert.ToBase64String(input)
        .Replace('+', '-')
        .Replace('/', '_');
    }

		private static byte[] Base64UrlDecode(string input) {

			return Convert.FromBase64String(input
				.Replace('-', '+')
				.Replace('_', '/')
			);
		}
	}

  public static class SHA512Hash {
    
    public const int SaltByteSize = 48;
    public const int HashByteSize = 64;
    public const int Iterations = 1500;
    public const int IterationIndex = 0;
    public const int SaltIndex = 1;
    public const int Pbkdf2Index = 2;
    
    public enum Errorcodes {
      Locked,
      Disabled,
      Expired,
      NeverSet,
      NoPassword,
      Ok,
    }

    public static string Hash(string origin) {
      var cP = new RNGCryptoServiceProvider();
      byte[] salt = new byte[SaltByteSize];
      
      cP.GetBytes(salt);
      var hash = GetPbkdf2Bytes(origin, salt, Iterations, HashByteSize);
      
      return ""+Iterations + Global.PASS_DELIMITER +
              Convert.ToBase64String(salt) + Global.PASS_DELIMITER +
              Convert.ToBase64String(hash);
    }
    
    public static Errorcodes GetHashType(string hash) {
      switch (hash) {
        case "!":
          return Errorcodes.Locked;
        case "!!":
          return Errorcodes.NeverSet;
        case "*":
          return Errorcodes.Disabled;
        case "":
          return Errorcodes.NoPassword;
        default:
          return Errorcodes.Ok;
      }
    }

    public static bool Verify(string plain, string hashed) {

      if (hashed == "false") return false;
      
      Errorcodes errCode = GetHashType(hashed);
      if (errCode == Errorcodes.Ok) {
        var split = hashed.Split(Global.PASS_DELIMITER);
        var iterations = Int32.Parse(split[IterationIndex]);
        var salt = Convert.FromBase64String(split[SaltIndex]);
        var hash = Convert.FromBase64String(split[Pbkdf2Index]);
      
        var toTest = GetPbkdf2Bytes(plain, salt, iterations, hash.Length);
        return Compare(hash, toTest);
      } else {
        switch (errCode) {
          case Errorcodes.Locked:
            return false;
          case Errorcodes.NeverSet:
            return true;
          case Errorcodes.Disabled:
            return false;
          case Errorcodes.NoPassword:
            return true;
          default: return false;
        }
      }
    }
    
    static bool Compare(byte[] a, byte[] b) {
      var diff = (uint)a.Length ^ (uint) b.Length;
      for (int i = 0; i < a.Length && i < b.Length; i++) {
        diff |= (uint)(a[i] ^ b[i]);
      }
      return diff == 0;
    }
    
    static byte[] GetPbkdf2Bytes(string origin, byte[] salt, int iterations, int outBytes) {
      var pbkdf2 = new Rfc2898DeriveBytes(origin, salt);
      pbkdf2.IterationCount = iterations;
      return pbkdf2.GetBytes(outBytes);
    }
  }
}