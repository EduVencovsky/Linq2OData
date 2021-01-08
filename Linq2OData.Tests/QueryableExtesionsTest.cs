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
    public class QueryableHelpersTest
    {
        #region HasMethod
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
        public void Test_SkipInsideWhereShouldNotHaveMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Collection.Skip(1).FirstOrDefault() == "hey");

            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            
            hasSkip.Should().BeFalse();
        }

        [TestMethod]
        public void Test_NextedQueryableSkipInsideWhereShouldNotHaveMethod()
        {            
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.SelfCollection.AsQueryable()
                    .Skip(1).FirstOrDefault().Title == "hey");

            var hasSkip = query.HasMethod(nameof(Queryable.Skip));
            hasSkip.Should().BeFalse();
        }
        
        [TestMethod]
        public void Test_NextedQueryableTakeInsideWhereShouldNotHaveMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.SelfCollection.AsQueryable()
                    .Take(1).FirstOrDefault().Title == "hey");
            
            var hasTake = query.HasMethod(nameof(Queryable.Take));
            
            hasTake.Should().BeFalse();
        }
        
        [TestMethod]
        public void Test_MultipleWhereMethodsShouldHaveMethod()
        {
            var query = new List<TestModel>().AsQueryable()
                .Where(x => x.Id > 2)
                .Where(x => x.IsSomething)
                .Where(x => x.Title.Length > 0)
                .Where(x => x.ExtraData.SomeNumber == 10);
            
            var hasWhere = query.HasMethod(nameof(Queryable.Where));
            
            hasWhere.Should().BeTrue();
        }
        #endregion
        
        #region GetMethod
            
        #endregion        

        #region GetMethodArguments
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
        public void Test_MultipleWhereMethodsShouldAllArguments()
        {            
            var whereArguments = new List<Expression<Func<TestModel, bool>>> 
            {
                x => x.Id > 2,
                x => x.IsSomething,
                x => x.Title.Length > 0,
                x => x.ExtraData.SomeNumber == 10
            };

            var query = new List<TestModel>().AsQueryable()
                .Where(whereArguments[0])
                .Where(whereArguments[1])
                .Where(whereArguments[2])
                .Where(whereArguments[3]);
            
            var resultArguments = query.GetMethodArguments(nameof(Queryable.Where));
            
            resultArguments.Should().NotBeNull();
            resultArguments.Should().HaveCount(whereArguments.Count);
        }  
        #endregion

        #region GetMethodArgumentsValues
        [TestMethod]
        public void Test_GetSkipArgumentValue()
        {
            var skipArgumentValue = 1;
            var query = new List<TestModel>().AsQueryable()
                .Skip(skipArgumentValue);
            
            var skipArguments = query.GetMethodArgumentsValues(nameof(Queryable.Skip));
                        
            skipArguments.Should().NotBeNull();
            skipArguments.Should().HaveCount(1); 
            skipArguments.First().Should().Be(skipArgumentValue);           
        }

        [TestMethod]
        public void Test_GetTakeArgumentValue()
        {
            var takeArgumentValue = 1;
            var query = new List<TestModel>().AsQueryable()
                .Take(takeArgumentValue);
            
            var takeArguments = query.GetMethodArgumentsValues(nameof(Queryable.Take));
                        
            takeArguments.Should().NotBeNull();
            takeArguments.Should().HaveCount(1); 
            takeArguments.First().Should().Be(takeArgumentValue);           
        }

        [TestMethod]
        public void Test_GetWhereArgumentValue()
        {
            Expression<Func<TestModel, bool>> whereArgumentValue = x => x.Id == 1;
            var query = new List<TestModel>().AsQueryable()
                .Where(whereArgumentValue);
            
            var whereArguments = query.GetMethodArgumentsValues(nameof(Queryable.Where));
                        
            whereArguments.Should().NotBeNull();
            whereArguments.Should().HaveCount(1); 
            whereArguments.First().Should().Be(whereArgumentValue);           
        }

        [TestMethod]
        public void Test_GetNonExistingArgumentValue()
        {            
            var query = new List<TestModel>().AsQueryable()
                .Skip(1);
            
            var whereArguments = query.GetMethodArgumentsValues("NON_EXISTING_METHOD");
                        
            whereArguments.Should().NotBeNull();
            whereArguments.Should().HaveCount(0);             
        }
        
        [TestMethod]
        public void Test_MultipleWhereMethodsShouldAllArgumentsValues()
        {             
            var whereArguments = new List<Expression<Func<TestModel, bool>>> 
            {
                x => x.Id > 2,
                x => x.IsSomething,
                x => x.Title.Length > 0,
                x => x.ExtraData.SomeNumber == 10
            };

            var query = new List<TestModel>().AsQueryable()
                .Where(whereArguments[0])
                .Where(whereArguments[1])
                .Where(whereArguments[2])
                .Where(whereArguments[3]);
            
            var resultArguments = query.GetMethodArgumentsValues(nameof(Queryable.Where));
            
            resultArguments.Should().NotBeNull();
            resultArguments.Should().HaveCount(whereArguments.Count);
            resultArguments.Should().BeEquivalentTo(whereArguments);
        }
        #endregion
        
        #region GetMethods
            
        #endregion
        
        #region GetMethodsArguments
            
        #endregion

        #region GetMethodsArgumentsValues
            
        #endregion
    }
}