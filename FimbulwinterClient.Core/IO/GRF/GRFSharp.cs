using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using FimbulwinterClient.Core.IO.ZLib;
using FimbulwinterClient.Extensions;

namespace FimbulwinterClient.Core.IO.GRF
{

    #region Event Delegates

    public delegate void ExtractCompleteEventHandler(object sender, GRFEventArg e);

    public delegate void FileReadCompleteEventHandler(object sender, GRFEventArg e);

    public delegate void FileAddCompleteEventHandler(object sender, GRFEventArg e);

    public delegate void GrfMetaWriteCompleteEventHandler(object sender);

    public delegate void FileBodyWriteCompleteEventHandler(object sender, GRFEventArg e);

    public delegate void FileTableWriteCompleteEventHandler(object sender, GRFEventArg e);

    public delegate void SaveCompleteEventHandler(object sender);

    public delegate void FileCountReadCompleteEventHandler(object sender);

    public delegate void GrfOpenCompleteEventHandler(object sender);

    #endregion

    public class Grf
    {
        #region Local variables

        private string _filePathToGrf;
        private readonly Hashtable _grfFiles = CollectionsUtil.CreateCaseInsensitiveHashtable();

        private int _compressedLength;

        private byte[] _bodyBytes;

        private int _fileCount;

        private string _signature;
        private byte[] _encryptionKey;
        private int _fileTableOffset;
        private int _version;
        private int _m1;
        private int _m2;
        private bool _isOpen;

        private Stream _grfStream;

        #endregion

        #region Public Events

        public event ExtractCompleteEventHandler ExtractComplete;
        public event FileReadCompleteEventHandler FileReadComplete;
        public event FileAddCompleteEventHandler FileAddComplete;
        public event GrfMetaWriteCompleteEventHandler GrfMetaWriteComplete;
        public event FileBodyWriteCompleteEventHandler FileBodyWriteComplete;
        public event FileTableWriteCompleteEventHandler FileTableWriteComplete;
        public event SaveCompleteEventHandler GrfSaveComplete;
        public event FileCountReadCompleteEventHandler FileCountReadComplete;
        public event GrfOpenCompleteEventHandler GrfOpenComplete;

        #endregion

        #region Protected Events

        protected virtual void OnExtractComplete(GRFEventArg e)
        {
            if (ExtractComplete != null)
                ExtractComplete(this, e);
        }

        protected virtual void OnFileReadComplete(GRFEventArg e)
        {
            if (FileReadComplete != null)
                FileReadComplete(this, e);
        }

        protected virtual void OnFileAddComplete(GRFEventArg e)
        {
            if (FileAddComplete != null)
                FileAddComplete(this, e);
        }

        protected virtual void OnGrfMetaWriteComplete()
        {
            if (GrfMetaWriteComplete != null)
                GrfMetaWriteComplete(this);
        }

        protected virtual void OnFileBodyWriteComplete(GRFEventArg e)
        {
            if (FileBodyWriteComplete != null)
                FileBodyWriteComplete(this, e);
        }

        protected virtual void OnFileTableWriteComplete(GRFEventArg e)
        {
            if (FileTableWriteComplete != null)
                FileTableWriteComplete(this, e);
        }

        protected virtual void OnGrfSaveComplete()
        {
            if (GrfSaveComplete != null)
            {
                GrfSaveComplete(this);
            }
        }

        protected virtual void OnFileCountReadComplete()
        {
            if (FileCountReadComplete != null)
            {
                FileCountReadComplete(this);
            }
        }

        protected virtual void OnGrfOpenComplete()
        {
            if (GrfOpenComplete != null)
            {
                GrfOpenComplete(this);
            }
        }

        #endregion

        #region Public properties

        public Hashtable Files
        {
            get { return _grfFiles; }
        }

        public int FileCount
        {
            get { return _fileCount; }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
        }

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


        public byte[] EncryptionKey
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

        #endregion

        #region Constructor

        public Grf() // Constructor
        {
            _signature = "Master of Magic";
            _encryptionKey = new byte[14];
        }

        public Grf(string filePathToGrf) // Constructor
        {
            _filePathToGrf = filePathToGrf;

            _signature = "Master of Magic";
            _encryptionKey = new byte[14];
        }

        #endregion

        #region Public Functions

        /// <summary>
        ///   Save the Grf file.
        /// </summary>
        public void Save()
        {
            SaveAs(_filePathToGrf);
        }

        /// <summary>
        ///   This this grf in the specified path.
        /// </summary>
        /// <param name="filepath"> The path where to save the grf. </param>
        public void SaveAs(string filepath)
        {
            // Write to temporary file
            string tempfile = Path.GetTempFileName();
            FileStream fs = new FileStream(tempfile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            byte[] signatureByte = new byte[Math.Max(_signature.Length, 15)];
            Encoding.ASCII.GetBytes(_signature).CopyTo(signatureByte, 0);
            bw.Write(signatureByte, 0, 15);
            bw.Write((byte) 0);
            bw.Write(_encryptionKey, 0, 14);

            bw.Write(0); // will be updated later
            bw.Write(_m1);
            bw.Write(_grfFiles.Count + _m1 + 7);
            bw.Write(0x200); // We always save as 2.0
            OnGrfMetaWriteComplete();
            foreach (DictionaryEntry entry in _grfFiles)
            {
                ((GrfFile) entry.Value).SaveBody(bw);
                OnFileBodyWriteComplete(new GRFEventArg((GrfFile) entry.Value));
            }

            bw.Flush();

            int fileTablePos = (int) fs.Position;

            MemoryStream bodyStream = new MemoryStream();
            BinaryWriter bw2 = new BinaryWriter(bodyStream);

            foreach (DictionaryEntry entry in _grfFiles)
            {
                ((GrfFile) entry.Value).Save(bw2);
                OnFileTableWriteComplete(new GRFEventArg((GrfFile) entry.Value));
            }

            bw2.Flush();
            //byte[] compressedBody = new byte[_uncompressedLength + 100];
            //int size = compressedBody.Length;
            //ZLib.compress(compressedBody, ref size, bodyStream.GetBuffer(), (int)bodyStream.Length);
            byte[] compressedBody = ZlibStream.CompressBuffer(bodyStream.GetBuffer());

            bw.Write(compressedBody.Length);
            bw.Write((int) bodyStream.Length);
            bw.Write(compressedBody, 0, compressedBody.Length);
            bw2.Close();

            // Update file table offset
            bw.BaseStream.Seek(30, SeekOrigin.Begin);
            bw.Write(fileTablePos - 46);

            bw.Close();

            if (_grfStream != null)
                _grfStream.Close();

            File.Copy(tempfile, filepath, true);
            OnGrfSaveComplete();

            _filePathToGrf = filepath;
            Close();
            Open();
        }

        /// <summary>
        ///   Open the Grf File to start reading.
        /// </summary>
        public void Open()
        {
            _grfFiles.Clear();
            _grfStream = new FileStream(_filePathToGrf, FileMode.Open);
            BinaryReader br = new BinaryReader(_grfStream);

            //Read Grf File Header -> Signature
            byte[] signatureByte = new byte[15];
            _grfStream.Read(signatureByte, 0, 15);
            string signature = Encoding.ASCII.GetString(signatureByte);
            br.ReadByte();

            // Read Grf File Header -> Encryption Key
            byte[] encryptionKey = new byte[14];
            _grfStream.Read(encryptionKey, 0, 14);

            int tableOffset = br.ReadInt32();
            int m1 = br.ReadInt32();
            int m2 = br.ReadInt32();
            int version = br.ReadInt32();

            _signature = signature;
            _encryptionKey = encryptionKey;
            _fileTableOffset = tableOffset;
            _m1 = m1;
            _m2 = m2;
            _version = version;

            _grfStream.Seek(_fileTableOffset, SeekOrigin.Current);

            _compressedLength = br.ReadInt32();
            br.ReadInt32();

            byte[] compressedBodyBytes = new byte[_compressedLength];
            _grfStream.Read(compressedBodyBytes, 0, _compressedLength);

            _bodyBytes = ZlibStream.UncompressBuffer(compressedBodyBytes);

            _fileCount = m2 - m1 - 7;
            OnFileCountReadComplete();
            MemoryStream bodyStream = new MemoryStream(_bodyBytes);
            BinaryReader bodyReader = new BinaryReader(bodyStream);

            for (int x = 0; x < _fileCount; x++)
            {
                string fileName = bodyReader.ReadCString();

                int fileCycle = 0;

                int fileCompressedLength = bodyReader.ReadInt32();
                int fileCompressedLengthAligned = bodyReader.ReadInt32();
                int fileUncompressedLength = bodyReader.ReadInt32();
                byte fileFlags = bodyReader.ReadByte();
                int fileOffset = bodyReader.ReadInt32();

                if (fileFlags == 1 || fileFlags == 5 || fileFlags == 3)
                {
                    int srclen = fileCompressedLength;

                    switch (fileFlags)
                    {
                        case 3:
                        {
                            int lop;
                            int srccount;
                            for (lop = 10, srccount = 1; srclen >= lop; lop *= 10, srccount++)
                                fileCycle = srccount;
                        }
                            break;
                        case 5:
                            fileCycle = 0;
                            break;
                        default:
                            fileCycle = -1;
                            break;
                    }
                }

                if (fileFlags == 2) // Do not add folders 
                {
                    OnFileReadComplete(new GRFEventArg(new GrfFile(fileName, 0, 0, 0, 0, 0, 0, this)));
                    continue;
                }

                GrfFile newGrfFile = new GrfFile(
                    fileName,
                    fileCompressedLength,
                    fileCompressedLengthAligned,
                    fileUncompressedLength,
                    fileFlags,
                    fileOffset,
                    fileCycle,
                    this);

                _grfFiles.Add(newGrfFile.Name, newGrfFile);
                OnFileReadComplete(new GRFEventArg(newGrfFile));
            }
            _isOpen = true;
            OnGrfOpenComplete();
        }

        /// <summary>
        ///   Open the Grf File to start reading. (Overload)
        /// </summary>
        /// <param name='filePath'> Path the the grf file to be opened </param>
        public void Open(string filePath)
        {
            _filePathToGrf = filePath;
            Open();
        }

        /// <summary>
        ///   Closes the grf so it can be used again
        /// </summary>
        public void Close()
        {
            if (_isOpen)
                _grfStream.Close();
            _isOpen = false;
        }

        /// <summary>
        ///   Gets the data of the file in the grf.
        /// </summary>
        /// <returns> byte[] the data in bytes </returns>
        /// <param name='file'> (GRFFile) The file to get </param>
        public byte[] GetDataFromFile(GrfFile file)
        {
            byte[] compressedBody = new byte[file.CompressedLengthAligned];

            lock (_grfStream)
            {
                _grfStream.Seek(46 + file.Offset, SeekOrigin.Begin);
                _grfStream.Read(compressedBody, 0, file.CompressedLengthAligned);
            }

            DES.GrfDecode(compressedBody, file.Flags, file.CompressedLength);

            return ZlibStream.UncompressBuffer(compressedBody);
        }

        /// <summary>
        ///   Gets the byte[] data of the file in the grf. (Uncompressed)
        /// </summary>
        /// <param name="file"> </param>
        /// <returns> </returns>
        public byte[] GetCompressedDataFromFile(GrfFile file)
        {
            byte[] compressedBody = new byte[file.CompressedLength];
            _grfStream.Seek(46 + file.Offset, SeekOrigin.Begin);
            _grfStream.Read(compressedBody, 0, file.CompressedLengthAligned);
            return compressedBody;
        }


        /// <summary>
        ///   Add a file inside the grf.
        /// </summary>
        public void AddFile(string inputFilePath, string outputFilePath)
        {
            byte[] data = File.ReadAllBytes(inputFilePath);

            GrfFile f = (GrfFile) _grfFiles[outputFilePath];
            if (f != null)
            {
                f.UncompressedBody = data;
                return;
            }

            f = new GrfFile(outputFilePath, 0, 0, 0, 1, 0, 0, this);
            f.UncompressedBody = data;
            _grfFiles.Add(f.Name, f);
            _fileCount++;
            OnFileAddComplete(new GRFEventArg(f));
        }

        /// <summary>
        ///   Delete a file in the grf.
        /// </summary>
        /// <param name="filename"> The file name to delete. </param>
        public void DeleteFile(string filename)
        {
            if (_grfFiles.ContainsKey(filename))
            {
                _grfFiles.Remove(filename);
                _fileCount--;
            }
        }


        /// <summary>
        ///   Extracts a file from the grf to the specified path.
        /// </summary>
        /// <param name="file"> The file inside the grf </param>
        /// <param name="path"> The path where to extract the file </param>
        public void ExtractFileToPath(GrfFile file, string path)
        {
            if (file.Flags == 1)
            {
                file.WriteToDisk(path);
                OnExtractComplete(new GRFFileExtractEventArg(file));
            }
        }

        #endregion

        public GrfFile GetFile(string asset)
        {
            return (GrfFile) _grfFiles[asset];
        }
    }
}