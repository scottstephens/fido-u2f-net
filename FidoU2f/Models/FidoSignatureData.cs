﻿//
// The MIT License(MIT)
//
// Copyright(c) 2015 Hans Wolff
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.IO;

namespace FidoU2f.Models
{
	public class FidoSignatureData
	{
		public byte UserPresence { get; }

		public uint Counter { get; }

		public FidoSignature Signature { get; private set; }

		private FidoSignatureData(byte userPresence, uint counter, FidoSignature signature)
		{
			UserPresence = userPresence;
			Counter = counter;
			Signature = signature;
		}

		public static FidoSignatureData FromWebBase64(string webSafeBase64)
		{
			return FromBytes(WebSafeBase64Converter.FromBase64String(webSafeBase64));
		}

		public static FidoSignatureData FromBytes(byte[] rawRegistrationData)
		{
			using (var mem = new MemoryStream(rawRegistrationData))
			{
				return FromStream(mem);
			}
		}

		private static FidoSignatureData FromStream(Stream stream)
		{
			using (var binaryReader = new BinaryReader(stream))
			{
				var userPresence = binaryReader.ReadByte();
				var counterBytes = binaryReader.ReadBytes(4);

				if (BitConverter.IsLittleEndian)
					Array.Reverse(counterBytes);

				var counter = BitConverter.ToUInt32(counterBytes, 0);

				var size = binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
				var signatureBytes = binaryReader.ReadBytes((int)size);

				return new FidoSignatureData(
					userPresence,
					counter,
					new FidoSignature(signatureBytes));
			}
		}
	}
}
