using FluentAssertions;
using Linq2OData.Core;
using Linq2OData.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Tests
{
    [TestClass]
    public class ODataPartsTest
    {
        // TODO: TEST EVERY FUNCTIONALITY OF ODataParts
        
        [TestMethod]
        public void Test_ToString()
        {
            var odata = "$count=false&$filter=&$orderby=&$top=10&$skip=0";
            var parts = new OData<TestModel>(odata);
            parts.ToString().Should().Be(odata);
        }  
    }
}
