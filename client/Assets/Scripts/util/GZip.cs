using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace util
{
    public static class GZip
    {
        public static byte[] Decompress (byte[] data)
        {
            using (GZipStream gzStream = new GZipStream(new MemoryStream(data), CompressionMode.Decompress, true)) {
                const int bufferSize = 4096;
                int bytesRead = 0;
                
                byte[] buffer = new byte[bufferSize];
                
                using (MemoryStream ms = new MemoryStream()) {
                    while((bytesRead = gzStream.Read(buffer, 0, bufferSize)) > 0){
                        ms.Write(buffer, 0, bytesRead);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}