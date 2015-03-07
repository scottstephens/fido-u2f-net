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
using System.Text;
using FidoU2f.Models;
using NUnit.Framework;

namespace FidoU2f.Tests.Models
{
	[TestFixture]
	public class TestFidoClientData
	{
		[Test]
		public void Validate_Wellformed_NoException()
		{
			var clientData = CreateGoodClientData();
			clientData.Validate();
		}

		[Test]
		public void Validate_ChallengeMissing_Throws()
		{
			var clientData = CreateGoodClientData();
			clientData.Challenge = "";

			Assert.Throws<InvalidOperationException>(() => clientData.Validate());
		}

		[Test]
		public void Validate_OriginMissing_Throws()
		{
			var clientData = CreateGoodClientData();
			clientData.Origin = "";

			Assert.Throws<InvalidOperationException>(() => clientData.Validate());
		}

		[Test]
		public void Validate_TypeMissing_Throws()
		{
			var clientData = CreateGoodClientData();
            clientData.Type = "";

			Assert.Throws<InvalidOperationException>(() => clientData.Validate());
		}

		internal static FidoClientData CreateGoodClientData()
		{
			return new FidoClientData
			{
				Challenge = WebSafeBase64Converter.ToBase64String(Encoding.Default.GetBytes("random challenge")),
				Origin = "http://localhost",
				Type = "type"
			};
		}
    }
}
