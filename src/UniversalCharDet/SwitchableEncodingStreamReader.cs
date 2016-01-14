using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace CharDetSharp.UniversalCharDet
{
    public class SwitchableEncodingStreamReader : TextReader
    {
        private StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the SwitchableEncodingStreamReader class for the specified StreamReader.
        /// </summary>
        /// <param name="baseReader"></param>
        public SwitchableEncodingStreamReader(StreamReader baseReader)
        {
            //TODO: Add message to resources for localization
            if (baseReader == null)
                throw new ArgumentNullException("baseReader");

            reader = baseReader;
        }

        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified stream.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <exception cref="System.ArgumentException">
        ///   stream does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   stream is null.
        /// </exception>
        public SwitchableEncodingStreamReader(Stream stream) : this(new StreamReader(stream)) { }

#if !COREFX
        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified file name.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <exception cref="System.ArgumentException">
        ///   path is an empty string ("").
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   path is null.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///   The file cannot be found.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///   The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///   path includes an incorrect or invalid syntax for file name, directory name,
        ///   or volume label.
        /// </exception>
        public SwitchableEncodingStreamReader(string path) : this(new StreamReader(path)) { }
#endif

        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   stream, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentException">
        ///   stream does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   stream is null.
        /// </exception>
        public SwitchableEncodingStreamReader(Stream stream, bool detectEncodingFromByteOrderMarks) : this(new StreamReader(stream, detectEncodingFromByteOrderMarks)) { }

        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   stream, with the specified character encoding.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentException">
        ///   stream does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   stream or encoding is null.
        /// </exception>
        public SwitchableEncodingStreamReader(Stream stream, Encoding encoding) : this(new StreamReader(stream, encoding)) { }
        
#if !COREFX
        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   file name, with the specified byte order mark detection option.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentException">
        ///   path is an empty string ("").
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   path is null.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///   The file cannot be found.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///   The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///   path includes an incorrect or invalid syntax for file name, directory name,
        ///   or volume label.
        /// </exception>
        public SwitchableEncodingStreamReader(string path, bool detectEncodingFromByteOrderMarks) : this(new StreamReader(path, detectEncodingFromByteOrderMarks)) { }
#endif
        
#if !COREFX
        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   file name, with the specified character encoding.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="System.ArgumentException">
        ///   path is an empty string ("").
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   path or encoding is null.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///   The file cannot be found.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///   The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref=" System.NotSupportedException">
        ///   path includes an incorrect or invalid syntax for file name, directory name,
        ///   or volume label.
        /// </exception>
        public SwitchableEncodingStreamReader(string path, Encoding encoding) : this(new StreamReader(path, encoding)) { }
#endif

        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   stream, with the specified character encoding and byte order mark detection
        ///   option.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentException">
        ///   stream does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   stream or encoding is null.
        /// </exception>
        public SwitchableEncodingStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : this(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks)) { }
        
#if !COREFX
        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   file name, with the specified character encoding and byte order mark detection
        ///   option.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <exception cref="System.ArgumentException">
        ///   path is an empty string ("").
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   path or encoding is null.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///   The file cannot be found.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///   The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   path includes an incorrect or invalid syntax for file name, directory name,
        ///   or volume label.
        /// </exception>
        public SwitchableEncodingStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : this(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks)) { }
#endif

        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   stream, with the specified character encoding, byte order mark detection
        ///   option, and buffer size.
        /// </summary>
        /// <param name="stream">The stream to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <exception cref="System.ArgumentException">
        ///   The stream does not support reading.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   stream or encoding is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   bufferSize is less than or equal to zero.
        /// </exception>
        public SwitchableEncodingStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : this(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)) { }

#if !COREFX
        /// <summary>
        ///   Initializes a new instance of the SwitchableEncodingStreamReader class for the specified
        ///   file name, with the specified character encoding, byte order mark detection
        ///   option, and buffer size.
        /// </summary>
        /// <param name="path">The complete file path to be read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="detectEncodingFromByteOrderMarks">Indicates whether to look for byte order marks at the beginning of the file.</param>
        /// <param name="bufferSize">The minimum buffer size.</param>
        /// <exception cref="System.ArgumentException">
        ///   path is an empty string ("").
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   path or encoding is null.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        ///   The file cannot be found.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        ///   The specified path is invalid, such as being on an unmapped drive.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   path includes an incorrect or invalid syntax for file name, directory name,
        ///   or volume label.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   buffersize is less than or equal to zero.
        /// </exception>
        public SwitchableEncodingStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : this(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)) { }
#endif

        /// <summary>
        ///   Returns the underlying stream.
        /// </summary>
        ///Returns:
        ///   The underlying stream.
        public virtual Stream BaseStream { get { return reader.BaseStream; } }
        
        /// <summary>
        ///   Gets the current character encoding that the current System.IO.StreamReader object is using.
        /// </summary>
        ///Returns:
        ///   The current character encoding used by the current reader. The value can
        ///   be different after the first call to any Overload:System.IO.StreamReader.Read
        ///   method of System.IO.StreamReader, since encoding autodetection is not done
        ///   until the first call to a Overload:System.IO.StreamReader.Read method.
        public virtual Encoding CurrentEncoding { get { return reader.CurrentEncoding; } }
        
        /// <summary>
        ///   Gets a value that indicates whether the current stream position is at the end of the stream.
        /// </summary>
        ///Returns:
        ///   true if the current stream position is at the end of the stream; otherwise
        ///   false.
        /// <exception cref="System.ObjectDisposedException">
        ///   The underlying stream has been disposed.
        /// </exception>
        public bool EndOfStream { get { return reader.EndOfStream; } }

        /// <summary>
        ///   Closes the System.IO.StreamReader object and the underlying stream, and releases
        ///   any system resources associated with the reader.
        /// </summary>
#if !COREFX
        public override void Close() { reader.Close(); }
#endif
        
        /// <summary>
        ///   Allows a System.IO.StreamReader object to discard its current data.
        /// </summary>
        public void DiscardBufferedData() { reader.DiscardBufferedData(); }

        /// <summary>
        ///   Closes the underlying stream, releases the unmanaged resources used by the
        ///   System.IO.StreamReader, and optionally releases the managed resources.
        /// </summary>
        //
        ///Parameters:
        ///  disposing:
        ///   true to release both managed and unmanaged resources; false to release only
        ///   unmanaged resources.
        protected override void Dispose(bool disposing)
        {
            reader.Dispose();
            //reader.Dispose(disposing);
        }

        /// <summary>
        ///   Returns the next available character but does not consume it.
        /// </summary>
        ///Returns:
        ///   An integer representing the next character to be read, or -1 if no more characters
        ///   are available or the stream does not support seeking.
        /// <exception cref="System.IO.IOException">
        ///   An I/O error occurs.
        /// </exception>
        public override int Peek() { return reader.Peek(); }
        
        /// <summary>
        ///   Reads the next character from the input stream and advances the character
        ///   position by one character.
        /// </summary>
        ///Returns:
        ///   The next character from the input stream represented as an System.Int32 object,
        ///   or -1 if no more characters are available.
        //
        /// <exception cref="System.IO.IOException">
        ///   An I/O error occurs.
        /// </exception>
        public override int Read() { return reader.Read(); }
        
        /// <summary>
        ///   Reads a maximum of count characters from the current stream into buffer,
        ///   beginning at index.
        /// </summary>
        /// <param name="buffer">
        ///   When this method returns, contains the specified character array with the
        ///   values between index and (index + count - 1) replaced by the characters read
        ///   from the current source.
        /// </param>
        /// <param name="index">The index of buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of characters to read.</param>
        ///Returns:
        ///   The number of characters that have been read, or 0 if at the end of the stream
        ///   and no data was read. The number will be less than or equal to the count
        ///   parameter, depending on whether the data is available within the stream.
        //
        /// <exception cref="System.ArgumentException">
        ///   The buffer length minus index is less than count.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///   buffer is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   index or count is negative.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///   An I/O error occurs, such as the stream is closed.
        /// </exception>
        public override int Read(char[] buffer, int index, int count) { return reader.Read(buffer, index, count); }
        
        /// <summary>
        ///   Reads a line of characters from the current stream and returns the data as a string.
        /// </summary>
        ///Returns:
        ///   The next line from the input stream, or null if the end of the input stream
        ///   is reached.
        //
        /// <exception cref="System.OutOfMemoryException">
        ///   There is insufficient memory to allocate a buffer for the returned string.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///   An I/O error occurs.
        /// </exception>
        public override string ReadLine() { return reader.ReadLine(); }
        
        /// <summary>
        ///   Reads the stream from the current position to the end of the stream.
        /// </summary>
        /// <returns>
        ///   The rest of the stream as a string, from the current position to the end.
        ///   If the current position is at the end of the stream, returns the empty string("").
        /// </returns>
        /// <exception cref="System.OutOfMemoryException">
        ///   There is insufficient memory to allocate a buffer for the returned string.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        ///   An I/O error occurs.
        /// </exception>
        public override string ReadToEnd() { return reader.ReadToEnd(); }

        /// <summary>
        ///   Change the Character Encoding the stream is being read as.
        /// </summary>
        /// <param name="encoding"></param>
        public void SwitchEncoding(Encoding encoding)
        {
            reader = StreamReaderEncodingSwitcher.SwitchEncodings(reader, encoding);
        }
    }

    public static class StreamReaderEncodingSwitcher
    {
        public static StreamReader SwitchEncodings(
#if DOT_NET_3_5
            this
#endif
            StreamReader input, Encoding encoding)
        {
            //TODO: add messages to Resources, for localization
            if (input == null)
                throw new ArgumentNullException("input");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            Stream inputStream = input.BaseStream;

            if (!inputStream.CanSeek)
                throw new ArgumentException(Properties.Resources.StreamMustBeAbleToSeek, "inputStream");

            if (typeof(StreamReader).AssemblyQualifiedName == "System.IO.StreamReader, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")
            {
                int bytePos = (int)GetPrivateField(input, "bytePos");
                byte[] byteBuffer = GetPrivateField(input, "byteBuffer") as byte[];
                int byteLen = 0;
                int charPos = (int)GetPrivateField(input, "charPos");
                char[] charBuffer = GetPrivateField(input, "charBuffer") as char[];
                int charBytes = input.CurrentEncoding.GetByteCount(charBuffer, 0, charPos);

                long currentPos = inputStream.Position;
                long offset = byteLen - charBytes;

                inputStream.Seek(offset, SeekOrigin.Begin);

                return new StreamReader(inputStream, encoding);
            }

            throw new ArgumentException(Properties.Resources.UnknownTypeOfStreamReader, "inputStream");
        }

        [DebuggerStepThrough]
        private static object GetPrivateField(object input, string fieldName)
        {
            return input.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(input);
        }
    }
}
