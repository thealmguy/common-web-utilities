using CMCS.Common.WebUtilities.Helpers;
using CMCS.Common.WebUtilities.Objects;
using CMCS.Common.WebUtilities.RedirectRules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CMCS.Common.WebUtilities.Testing.Helpers
{

    public class UrlHelpersTests
    {
        

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ConvertsToLowerCase()
        {
            //Arrange
            string valueUnderTest =  "AStRiNgWiThCaPiTaLlEtTeRs";

            //Act
            string result = UrlHelpers.Encode(valueUnderTest);

            //Assert
            Assert.AreEqual("astringwithcapitalletters", result);
        }

        [Test]
        public void ReplacesNonAlphaNumericCharactersWithAHyphen()
        {
            //Arrange
            string valueUnderTest = "A St!RiNg Wi+Th CaPi_TaL lEt%TeRs";

            //Act
            string result = UrlHelpers.Encode(valueUnderTest);

            //Assert
            Assert.AreEqual("a-st-ring-wi-th-capi-tal-let-ters", result);
        }


    }
}
