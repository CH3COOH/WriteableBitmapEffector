﻿﻿//
// StorageExtensions.cs
//
// Copyright (c) 2012 Kenji Wada, http://ch3cooh.jp/
// 
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files 
// (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Softbuild.Storage
{
    public static class StorageExtensions
    {
        /// <summary>
        /// Pictures Libraryへファイルを保存する
        /// 既存の同名ファイルが存在している場合はファイルを上書きする
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream">エンコード済みの画像ストリーム</param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveToPicturesLibraryAsync(string fileName, IRandomAccessStream stream)
        {
            var library = KnownFolders.PicturesLibrary;
            var file = await library.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var writeStrm = await file.OpenStreamForWriteAsync())
            {
                var readStrm = stream.AsStreamForRead();
                readStrm.CopyTo(writeStrm);
            }
            return file;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveToTemporary(string fileName, IRandomAccessStream stream)
        {
            // IRandomAccessStreamのストリームを保存する
            var file = await StorageFile.CreateStreamedFileAsync(fileName,
                async writeStream =>
                {
                    var imageBuffer = new byte[stream.Size];
                    var ibuffer = imageBuffer.AsBuffer();
                    await stream.ReadAsync(ibuffer, (uint)stream.Size, InputStreamOptions.None);
                    await writeStream.WriteAsync(ibuffer);
                }, null);
            return file;
        }
    }
}