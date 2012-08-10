namespace FimbulwinterClient.Core.IO.GRF
{
    /// <summary>
    ///   Grf header.
    /// </summary>
    public class GrfHeader
    {
        private readonly string _signature;
        private readonly string _encryptionKey;
        private readonly int _fileTableOffset;
        private readonly int _version;
        private readonly int _m1;
        private readonly int _m2;

        public int Version
        {
            get { return _version; }
        }

        public int FileTableOffset
        {
            get { return _fileTableOffset; }
        }

        public string Signature
        {
            get { return _signature; }
        }


        public string EncryptionKey
        {
            get { return _encryptionKey; }
        }

        public int M2
        {
            get { return _m2; }
        }

        public int M1
        {
            get { return _m1; }
        }

        public GrfHeader(string signature, string encryptionKey, int fileTableOffset, int skip, int count, int version)
        {
            _signature = signature;
            _encryptionKey = encryptionKey;
            _fileTableOffset = fileTableOffset;
            _m1 = skip;
            _m2 = count;
            _version = version;
        }
    }
}