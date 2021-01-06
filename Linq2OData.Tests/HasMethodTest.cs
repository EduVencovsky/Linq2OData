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
        public void Test_HasWhereMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Id == 1);
            var hasSkip = query.HasMethod(nameof(Queryable.Where));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_HasSkipMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Skip(1);
            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_HasTakeMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Take(1);
            var hasSkip = query.HasMethod(nameof(Queryable.Take));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_HasSelectMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Select(x => x.ExtraData);

            var hasSkip = query.HasMethod(nameof(Queryable.Select));
            hasSkip.Should().BeTrue();
        }

        [TestMethod]
        public void Test_SkipInsideWhereShouldReturnFalse()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Collection.Skip(1).FirstOrDefault() == "hey");

            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            
            hasSkip.Should().BeFalse();
        }

        [TestMethod]
        public void Test_NextedQueryableSkipInsideWhereShouldReturnFalse()
        {            
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.SelfCollection.AsQueryable()
                    .Skip(1).FirstOrDefault().Title == "hey");

            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeFalse();
        }
        
        [TestMethod]
        public void Test_NextedQueryableTakeInsideWhereShouldReturnFalse()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.SelfCollection.AsQueryable()
                    .Take(1).FirstOrDefault().Title == "hey");
            
            var hasTake = query.HasMethod(nameof(Queryable.Take));
            
            hasTake.Should().BeFalse();
        }
        
        [TestMethod]
        public void Test_MultipleWhereMethodsShouldReturnTrue()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Id > 2)
                .Where(x => x.IsSomething)
                .Where(x => x.Title.Length > 0)
                .Where(x => x.ExtraData.SomeNumber == 10);
            
            var hasWhere = query.HasMethod(nameof(Queryable.Where));
            
            hasWhere.Should().BeTrue();
        }

        [TestMethod]
        public void Test_GetSkipArguments()
        {
            var query = new List<TestModel>().AsQueryable()
                .Skip(2);
            
            var skipArguments = query.GetMethodArguments(nameof(Queryable.Skip));
            
            // this argument and numeric argument
            skipArguments.Should().HaveCount(2);            
        }

        [TestMethod]
        public void Test_GetWhereArguments()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Id == 1);
            
            var whereArguments = query.GetMethodArguments(nameof(Queryable.Where));
            
            // this argument and numeric argument
            whereArguments.Should().HaveCount(2);            
        }

        [TestMethod]
        public void Test_GetTakeArguments()
        {
            var query = new List<TestModel>().AsQueryable()
                .Take(1);
            
            var takeArguments = query.GetMethodArguments(nameof(Queryable.Take));
            
            // this argument and numeric argument
            takeArguments.Should().HaveCount(2);            
        }

        [TestMethod]
        public void Test_GetSkipArgumentValue()
        {
            var skipArgumentValue = 1;
            var query = new List<TestModel>().AsQueryable()
                .Skip(skipArgumentValue);
            
            var takeArguments = query.GetMethodArgumentsValues(nameof(Queryable.Skip));
                        
            takeArguments.Should().NotBeNull();
            takeArguments.Should().HaveCount(1); 
            takeArguments.First().Should().Be(skipArgumentValue);           
        }
    }
}