using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vigil.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Owin.Testing;
using System.Net.Http;
using Vigil.Testing.Web.TestClasses;

namespace Vigil.Web.Controllers.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        [TestMethod()]
        public async Task AccountControllerTest()
        {
            using (var server = TestServer.Create<TestStartup>())
            {
                HttpResponseMessage response = await server.HttpClient.GetAsync("/");
                Assert.AreEqual<string>("Hello world using OWIN TestServer", await response.Content.ReadAsStringAsync());
            }
        }

        [TestMethod()]
        public void AccountControllerTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void LoginTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void LoginTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void VerifyCodeTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void VerifyCodeTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void RegisterTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void RegisterTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ConfirmEmailTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPasswordTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPasswordTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ForgotPasswordConfirmationTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPasswordTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPasswordTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ResetPasswordConfirmationTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SendCodeTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void SendCodeTest1()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginCallbackTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginConfirmationTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void LogOffTest()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void ExternalLoginFailureTest()
        {
            throw new NotImplementedException();
        }
    }
}
