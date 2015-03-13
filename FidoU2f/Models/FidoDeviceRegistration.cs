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
using Newtonsoft.Json;

namespace FidoU2f.Models
{
	public class FidoDeviceRegistration : IEquatable<FidoDeviceRegistration>
	{
		public FidoKeyHandle KeyHandle { get; private set; }

		public FidoPublicKey PublicKey { get; private set; }

		public FidoAttestationCertificate Certificate { get; private set; }

		public uint Counter { get; private set; }

		public FidoDeviceRegistration(FidoKeyHandle keyHandle, FidoPublicKey publicKey, FidoAttestationCertificate certificate, uint counter)
		{
			if (keyHandle == null) throw new ArgumentNullException("keyHandle");
			if (publicKey == null) throw new ArgumentNullException("publicKey");
			if (certificate == null) throw new ArgumentNullException("certificate");

			KeyHandle = keyHandle;
			PublicKey = publicKey;
			Certificate = certificate;
			Counter = counter;
		}

		public void UpdateCounter(uint clientCounter)
		{
			if (clientCounter <= Counter)
			{
				throw new InvalidOperationException("Counter value too small!");
			}
			Counter = clientCounter;
		}

		public static FidoDeviceRegistration FromJson(string json)
		{
			return JsonConvert.DeserializeObject<FidoDeviceRegistration>(json);
		}

		public string ToJson()
		{
			var json = JsonConvert.SerializeObject(this);
			return json;
		}

		public bool Equals(FidoDeviceRegistration other)
		{
			if (other == null) return false;

			return
				Counter == other.Counter &&
				Certificate.Equals(other.Certificate) &&
				KeyHandle.Equals(other.KeyHandle) &&
				PublicKey.Equals(other.PublicKey);
		}
	}
}
