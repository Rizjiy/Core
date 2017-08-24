using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Core.Internal;
using Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.MSTest.Validation
{
    [TestClass]
    public class ValidationUtilsTest
    {
        private ValidationFailureListDto _failureListDto;

        [TestInitialize]
        public void Init()
        {
            _failureListDto = new ValidationFailureListDto
            {
                Violations = new List<ValidationFailureDto>(new[]
                {
                    new ValidationFailureDto
                    {
                        Message = "Message 1",
                        PropertyName = "Property 1",
                        Severity = SeverityEnum.Error.ToString(),
                        SeverityCode = 1001
                    },
                    new ValidationFailureDto
                    {
                        Message = "Message 2",
                        PropertyName = "Property 2",
                        Severity = SeverityEnum.Waring.ToString(),
                        SeverityCode = 2002
                    }
                })
            };
        }

        [TestMethod]
        public void TestValidationFailureException()
        {
            try
            {
                throw new ValidationFailureException(_failureListDto);
            }
            catch (ValidationFailureException failure)
            {
                Assert.IsNotNull(failure.FailureList);
                Assert.IsNotNull(failure.FailureList.Violations);
                Assert.AreEqual(failure.FailureList, _failureListDto);
                Assert.AreEqual(failure.FailureList.Violations.Count, _failureListDto.Violations.Count);
            }

        }

        [TestMethod]
        public void TestValidationFailureExceptionToHttp()
        {
            var failure = new ValidationFailureException(_failureListDto);
            var responseException = failure.ToHttpResponseException();
            Assert.IsNotNull(responseException);
            Assert.AreEqual(responseException.Response.StatusCode, HttpStatusCode.Conflict);
            Assert.IsNotNull(responseException.Response.Content);
            Assert.IsTrue(responseException.Response.Content is StringContent);
            var content = (StringContent)responseException.Response.Content;
            var contentValue = content.ReadAsStringAsync().Result;
            var originalValue = _failureListDto.ToJson();
            Assert.AreEqual(contentValue, originalValue);
        }

    }
}
