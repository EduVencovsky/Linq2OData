using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using System.Linq;
using Community.OData.Linq;
using Linq2OData.Core;

namespace Linq2OData.Tests
{
    [TestClass]
    public class HasMethodTest
    {
        [TestMethod]
        public void Test_HasMethodSkip()
        {
            IQueryable<TestModel> query = new List<TestModel>().AsQueryable().Skip(1);
            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_HasMethodWhere()
        {
            IQueryable<TestModel> query = new List<TestModel>().AsQueryable().Where(x => x.Id == 1);
            var hasSkip = query.HasMethod(nameof(Queryable.Where));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_SkipInsideWhereShouldNotHaveSkipMethod()
        {
            IQueryable<TestModel> query = new List<TestModel>().AsQueryable()
                .Where(x => x.Collection.Skip(1).FirstOrDefault() == "hey")
                .Take(2);
            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeFalse();
        }

        [TestMethod]
        public void Test_NextedQueryableSkipInsideWhereShouldNotHaveSkipMethod()
        {
            // must loop on the first argument only to get only the methods in the `this`            
            IQueryable<TestModel> query = new List<TestModel>().AsQueryable()
                .Where(x => x.SelfCollection.AsQueryable().Skip(1).FirstOrDefault().Title == "hey")
                .Take(2);
            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeFalse();
        }
    }
}