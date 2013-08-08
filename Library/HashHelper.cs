namespace BetaSigmaPhi.Library {
	using System;
	using System.Security.Cryptography;
	using System.Text;

	public interface IHashHelper {
		string GenerateSalt( int MinSaltLength, int MaxSaltLength );
		string GenerateHash( HashType HashType, string PlainText, string SaltText );
		bool VerifyHash( HashType HashType, string PlainText, string SaltText, string HashValue );
	}

	// http://www.obviex.com/samples/hash.aspx
	// http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

	public class HashHelper : IHashHelper {

		/// <summary>
		/// Generates random integer.
		/// </summary>
		/// <remarks>
		/// This methods overcomes the limitations of .NET Framework's Random
		/// class, which - when initialized multiple times within a very short
		/// period of time - can generate the same "random" number.  Lifted verbatum from
		/// http://www.obviex.com/samples/EncryptionWithSalt.aspx
		/// </remarks>
		public int GenerateRandomNumber( int minValue, int maxValue ) {
			// We will make up an integer seed from 4 bytes of this array.
			byte[] randomBytes = new byte[4];

			// Generate 4 random bytes.
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetNonZeroBytes( randomBytes );

			// Convert four random bytes into a positive integer value.
			int seed = ((randomBytes[0] & 0x7f) << 24) |
				(randomBytes[1] << 16) |
				(randomBytes[2] << 8) |
				(randomBytes[3]);

			// Now, this looks more like real randomization.
			Random random = new Random( seed );

			// Calculate a random number.
			return random.Next( minValue, maxValue );
		}

		public string GenerateSalt( int MinSaltLength, int MaxSaltLength ) {
			
			// Generate a random number for the size of the salt.
			int saltSize = this.GenerateRandomNumber( MinSaltLength, MaxSaltLength );

			// Create a cryptographic number this long
			byte[] saltBytes = new byte[saltSize];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetNonZeroBytes( saltBytes );

			// Flip to Base64 string
			string salt = Convert.ToBase64String( saltBytes );
			
			// Validate and augment length if needed
			while ( salt.Length < saltSize ) {
				salt += salt;
			}
			if ( salt.Length > saltSize ) {
				salt = salt.Substring( 0, saltSize );
			}

			return salt;
		}

		public string GenerateHash( HashType HashType, string PlainText, string SaltText ) {
			if (string.IsNullOrEmpty(PlainText)) {
				throw new ArgumentNullException( "PlainText", "Can't hash blank text" );
			}
			byte[] saltBytes = Encoding.UTF8.GetBytes( SaltText );
			if ( string.IsNullOrEmpty( SaltText ) || saltBytes.IsNullOrEmpty() ) {
				throw new ArgumentNullException( "Salt", "Salt can't be blank" );
			}

			byte[] plainTextBytes = Encoding.UTF8.GetBytes( PlainText );

			// Concatinate text and salt bytes
			byte[] plainTextWithSaltBytes = new byte[plainTextBytes.Length + saltBytes.Length];

			for ( int i = 0; i < plainTextBytes.Length; i++ ) {
				plainTextWithSaltBytes[i] = plainTextBytes[i];
			}
			for ( int i = 0; i < saltBytes.Length; i++ ) {
				plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];
			}

			HashAlgorithm hashAlgorithm = this.GetAlgorithm( HashType );
			byte[] hashBytes = hashAlgorithm.ComputeHash( plainTextWithSaltBytes );

			string hashValue = Convert.ToBase64String( hashBytes );
			return hashValue;
		}

		public bool VerifyHash( HashType HashType, string PlainText, string SaltText, string HashValue ) {
			if ( string.IsNullOrEmpty( PlainText ) || string.IsNullOrEmpty( SaltText ) || string.IsNullOrEmpty( HashValue ) ) {
				return false; // You asked for nothing and you got it
			}
			string expectedHashString = this.GenerateHash( HashType, PlainText, SaltText );
			return ( HashValue == expectedHashString );
		}

		private HashAlgorithm GetAlgorithm( HashType HashType ) {
			HashAlgorithm algorithm = null;
			switch ( HashType ) {
				case HashType.MD5:
					algorithm = new MD5CryptoServiceProvider();
					break;
				case HashType.SHA1:
					algorithm = new SHA1Managed();
					break;
				case HashType.SHA256:
					algorithm = new SHA256Managed();
					break;
				case HashType.SHA384:
					algorithm = new SHA384Managed();
					break;
				case HashType.SHA512:
					algorithm = new SHA512Managed();
					break;
				default:
					throw new ArgumentNullException( "HashType", HashType + " is not a valid HashType" );
			}
			return algorithm;
		}

		private int GetHashSizeInBits(HashType HashType) {
			int hashSizeInBits = 1024;
			// Size of hash is based on the specified algorithm.
			switch ( HashType ) {
				case HashType.MD5:
					hashSizeInBits = 128;
					break;
				case HashType.SHA1:
					hashSizeInBits = 160;
					break;
				case HashType.SHA256:
					hashSizeInBits = 256;
					break;
				case HashType.SHA384:
					hashSizeInBits = 384;
					break;
				case HashType.SHA512:
					hashSizeInBits = 512;
					break;
				default:
					throw new ArgumentNullException( "HashType", HashType + " is not a valid HashType" );
			}
			return hashSizeInBits;
		}

	}

	public enum HashType {
		MD5,
		SHA1,
		SHA256,
		SHA384,
		SHA512
	}

}
