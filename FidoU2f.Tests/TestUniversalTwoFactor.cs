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
using Moq;
using NUnit.Framework;

namespace FidoU2f.Tests
{
	[TestFixture]
	public class TestUniversalTwoFactor
	{
		private static readonly FidoAppId TestAppId = new FidoAppId("http://localhost");

		[Test]
		public void StartRegistration()
		{
			var randomChallenge = Encoding.Default.GetBytes("random challenge");

			var mockGenerateChallenge = new Mock<IGenerateFidoChallenge>();
			mockGenerateChallenge.Setup(x => x.GenerateChallenge()).Returns(randomChallenge);

			var fido = new UniversalTwoFactor(mockGenerateChallenge.Object);
			var startedRegistration = fido.StartRegistration(TestAppId);

			mockGenerateChallenge.Verify(x => x.GenerateChallenge(), Times.Once);

			Assert.AreEqual(TestAppId, startedRegistration.AppId);
			Assert.AreEqual(randomChallenge, WebSafeBase64Converter.FromBase64String(startedRegistration.Challenge));
		}

		[Test]
		public void FinishRegistration_Works()
		{
			var fido = new UniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestAppId);

			var registerResponse = new FidoRegisterResponse
			{
				RegistrationData = GetValidRegistrationData(),
				ClientData = new FidoClientData
				{
					Challenge = startedRegistration.Challenge,
					Origin = "http://localhost",
					Type = UniversalTwoFactor.RegisterType
				}
			};

			var trustedFacets = new[] { new FidoFacetId("http://localhost") };

			var deviceRegistration = fido.FinishRegistration(startedRegistration, registerResponse, trustedFacets);
			// TODO: this is not working yet
			Assert.IsNotNull(deviceRegistration);
		}

		[Test]
		public void FinishRegistration_IncorrectType_Throws()
		{
			var fido = new UniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestAppId);

			var registerResponse = new FidoRegisterResponse
			{
				RegistrationData = GetValidRegistrationData(),
				ClientData = new FidoClientData
				{
					Challenge = startedRegistration.Challenge,
					Origin = "http://localhost",
					Type = "incorrect type"
				}
			};

			var trustedFacets = new[] {new FidoFacetId("http://localhost")};

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, trustedFacets));
		}

		[Test]
		public void FinishRegistration_IncorrectChallenge_Throws()
		{
			var fido = new UniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestAppId);

			var registerResponse = new FidoRegisterResponse
			{
				RegistrationData = GetValidRegistrationData(),
				ClientData = new FidoClientData
				{
					Challenge = WebSafeBase64Converter.ToBase64String(Encoding.Default.GetBytes("incorrect challenge")),
					Origin = "http://localhost",
					Type = UniversalTwoFactor.RegisterType
				}
			};

			var trustedFacets = new[] { new FidoFacetId("http://localhost") };

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, trustedFacets));
		}

		[Test]
		public void FinishRegistration_UntrustedOrigin_Throws()
		{
			var fido = new UniversalTwoFactor();
			var startedRegistration = fido.StartRegistration(TestAppId);

			var registerResponse = new FidoRegisterResponse
			{
				RegistrationData = GetValidRegistrationData(),
				ClientData = new FidoClientData
				{
					Challenge = startedRegistration.Challenge,
					Origin = "http://not.trusted",
					Type = UniversalTwoFactor.RegisterType
				}
			};

			var trustedFacets = new[] { new FidoFacetId("http://localhost") };

			Assert.Throws<InvalidOperationException>(() => fido.FinishRegistration(startedRegistration, registerResponse, trustedFacets));
		}

		private string GetValidRegistrationData()
		{
			return "validation data"; // TODO:
		}
    }
}
