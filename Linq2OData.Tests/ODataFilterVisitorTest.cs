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
    public class ODataFilterVisitorTest
    {

        public static int StaticConst = 1;

        public const int ConstProperty = 1;
        
        public bool BoolProperty { get; set; }

        public int NumberProperty { get; set; } = StaticProperty;

        public int GetterOnlyProperty => GetStaticProperty();
        
        public static int StaticProperty { get; set; } = StaticConst;
        
        public static TestModel TestModelStatic = new TestModel()
        {
            Id = 1,
            Title = "Entity 1",
            ExtraData = new TestModelExtraData()
            {
                Item = new TestModelItem()
                {
                    Name = "Item 1",
                    Data = new TestModelItemData() { Description = "Item Description 1", Index = 1 }
                },
                SomeNumber = 1
            }
        };
        
        public static TestModel TestModelStaticProperty { get; set; } = TestModelStatic;
        
        public TestModel TestModelProperty { get; set; } = TestModelStatic;

        public TestModel TestModelGetter => TestModelStatic;

        public int GetStaticProperty() => StaticProperty;

        public IEnumerable<TestModel> EntityList { get; set; } = new List<TestModel>()
        {
            
            new TestModel()
            {
                Id = 1,
                Title = "Entity 1",
                IsSomething = true,
                ExtraData = new TestModelExtraData()
                {
                    Item = new TestModelItem()
                    {
                        Name = "Item 1",
                        Data = new TestModelItemData() { Description = "Item Description 1", Index = 1 }
                    },
                    SomeNumber = 1
                }
            },
            new TestModel()
            { 
                Id = 2,
                IsSomething = true,
                Title = "Entity 2",
                ExtraData = new TestModelExtraData()
                { 
                    Item = new TestModelItem()
                    { 
                        Name = "Item 2",
                        Data = new TestModelItemData() { Description = "Item Description 2", Index = 2}
                    },
                    SomeNumber = 2
                } 
            },
            new TestModel()
            { 
                Id = 3,
                Title = "Entity 3",
                ExtraData = new TestModelExtraData()
                { 
                    Item = new TestModelItem()
                    { 
                        Name = "Item 3",
                        Data = new TestModelItemData() { Description = "Item Description 3", Index = 3}
                    },
                    SomeNumber = 3,                                        
                },
            },
            new TestModel()
            { 
                Id = 4,
                ExtraData = new TestModelExtraData()
                { 
                    Item = new TestModelItem(),
                    SomeNumber = 4
                } 
            }
        };

        private void CompareExpressionOData(string expectedOdata, Expression<Func<TestModel, bool>> expression)
        {
            var odata = expression.ToOdataFilter();
            odata.Should().Be(expectedOdata);

            var expectedQuery = EntityList.AsQueryable().Where(expression);
            var resultQuery = EntityList.AsQueryable().OData().Filter(odata);

            var expectedData = expectedQuery.ToList();
            var resultData = resultQuery.ToList();

            // Data should always return at least one item
            // If you return 0 items, there's no way of checking if the result are correct
            resultData.Should().HaveCountGreaterThan(0); 
            resultData.Should().BeEquivalentTo(expectedData);
        }

        [TestMethod]
        public void Test_NullValue()
        {
            Expression<Func<TestModel, bool>> expression = null;

            Action act = () => expression.ToOdataFilter();
            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Test_TrueExpression()
        {
            var expectedOdata = "true";
            Expression<Func<TestModel, bool>> trueExpression = x => true;
            
            CompareExpressionOData(expectedOdata, trueExpression);
        }

        [TestMethod]
        public void Test_FalseExpression()
        {
            var expectedOdata = "false";
            Expression<Func<TestModel, bool>> falseExpression = x => false;
            
            var odata = falseExpression.ToOdataFilter();
            odata.Should().Be(expectedOdata);

            var expectedQuery = EntityList.AsQueryable().Where(falseExpression);
            var resultQuery = EntityList.AsQueryable().OData().Filter(odata);

            var expectedData = expectedQuery.ToList();
            var resultData = resultQuery.ToList(); 

            resultData.Should().HaveCount(0);
            resultData.Should().BeEquivalentTo(expectedData);
        }

        [TestMethod]
        public void Test_GreaterThanExpression()
        {
            var expectedOdata = "(Id gt 3)";
            Expression<Func<TestModel, bool>> greaterExpression = x => x.Id > 3;

            CompareExpressionOData(expectedOdata, greaterExpression);
        }

        [TestMethod]
        public void Test_GreaterThanOrEqualExpression()
        {
            var expectedOdata = "(Id ge 3)";
            Expression<Func<TestModel, bool>> greaterThanOrEqualExpression = x => x.Id >= 3;

            CompareExpressionOData(expectedOdata, greaterThanOrEqualExpression);
        }

        [TestMethod]
        public void Test_LessThanExpression()
        {
            var expectedOdata = "(Id lt 3)";
            Expression<Func<TestModel, bool>> LessThanExpression = x => x.Id < 3;

            CompareExpressionOData(expectedOdata, LessThanExpression);
        }

        [TestMethod]
        public void Test_LessThanOrEqualExpression()
        {
            var expectedOdata = "(Id le 3)";
            Expression<Func<TestModel, bool>> lessThanOrEqualExpression = x => x.Id <= 3;

            CompareExpressionOData(expectedOdata, lessThanOrEqualExpression);
        }

        [TestMethod]
        public void Test_EqualExpression()
        {
            var expectedOdata = "(Id eq 3)";
            Expression<Func<TestModel, bool>> equalExpression = x => x.Id == 3;

            CompareExpressionOData(expectedOdata, equalExpression);
        }

        [TestMethod]
        public void Test_AndExpression()
        {
            var expectedOdata = "((Id ge 1) and (Id lt 4))";
            Expression<Func<TestModel, bool>> andExpression = x => x.Id >= 1 && x.Id < 4;

            CompareExpressionOData(expectedOdata, andExpression);
        }

        [TestMethod]
        public void Test_OrExpression()
        {
            var expectedOdata = "((Id le 2) or (Id ge 3))";
            Expression<Func<TestModel, bool>> orExpression = x => x.Id <= 2 || x.Id >= 3;

            CompareExpressionOData(expectedOdata, orExpression);
        }

        [TestMethod]
        public void Test_NullExpression()
        {
            var expectedOdata = "(Title eq null)";
            Expression<Func<TestModel, bool>> orExpression = x => x.Title == null;

            CompareExpressionOData(expectedOdata, orExpression);
        }

        [TestMethod]
        public void Test_NotNullExpression()
        {
            var expectedOdata = "(Title ne null)";
            Expression<Func<TestModel, bool>> orExpression = x => x.Title != null;

            CompareExpressionOData(expectedOdata, orExpression);
        }
        
        [TestMethod]
        public void Test_ParenthesesExpression()
        {
            var expectedOdata = "(Id eq 3)";
            Expression<Func<TestModel, bool>> parenthesesExpression = x => (x.Id == 3);

            CompareExpressionOData(expectedOdata, parenthesesExpression);
        }
        
        [TestMethod]
        public void Test_UselessParenthesesExpression()
        {
            var expectedOdata = "((((Id eq 3) or (Id eq 4)) or (Title eq 'Entity 1')) or (Title ne 'Entity 2'))";
            Expression<Func<TestModel, bool>> uselessParenthesesExpression = x => (((x.Id == 3) || (x.Id == 4)) || (x.Title == "Entity 1")) || (x.Title != "Entity 2");

            CompareExpressionOData(expectedOdata, uselessParenthesesExpression);
        }
        
        [TestMethod]
        public void Test_UsefullParenthesesExpression()
        {
            var expectedOdata1 = "((Id eq 3) and ((Id eq 4) or (Title eq 'Entity 3')))";
            Expression<Func<TestModel, bool>> usefullParenthesesExpression1 = x => x.Id == 3 && (x.Id == 4 || x.Title == "Entity 3");

            CompareExpressionOData(expectedOdata1, usefullParenthesesExpression1);
                        
            var expectedOdata2 = "(((Id eq 3) and (Title eq 'Entity 3')) or (Title eq 'Entity 2'))";
            Expression<Func<TestModel, bool>> usefullParenthesesExpression2 = x => x.Id == 3 && x.Title == "Entity 3" || x.Title == "Entity 2";
            
            CompareExpressionOData(expectedOdata2, usefullParenthesesExpression2);
                        
            var expectedOdata3 = "((Id eq 3) and ((Title eq 'Entity 3') or (Title eq 'Entity 2')))";
            Expression<Func<TestModel, bool>> usefullParenthesesExpression3 = x => x.Id == 3 && (x.Title == "Entity 3" || x.Title == "Entity 2");
            
            CompareExpressionOData(expectedOdata3, usefullParenthesesExpression3);
            
            var expectedOdata4 = "(((Id eq 3) and (Title eq 'Entity 3')) or (Title eq 'Entity 2'))";
            Expression<Func<TestModel, bool>> usefullParenthesesExpression4 = x => (x.Id == 3 && x.Title == "Entity 3") || x.Title == "Entity 2";
            
            CompareExpressionOData(expectedOdata4, usefullParenthesesExpression4);
        }
        
        [TestMethod]
        public void Test_MemberAccessExpression()
        {
            var expectedOdata = "(ExtraData/SomeNumber gt 2)";
            Expression<Func<TestModel, bool>> memberAccessExpression = x => x.ExtraData.SomeNumber > 2;

            CompareExpressionOData(expectedOdata, memberAccessExpression);                        
        }        
        
        [TestMethod]
        public void Test_MultipleMemberAccessExpression()
        {
            var expectedOdata = "(ExtraData/Item/Name eq 'Item 1')";
            Expression<Func<TestModel, bool>> multipleMemberAccessExpression = x => x.ExtraData.Item.Name == "Item 1";

            CompareExpressionOData(expectedOdata, multipleMemberAccessExpression);                        
        }      
        
        [TestMethod]
        public void Test_TryAccessPossibleNullMemberExpression()
        {
            var expectedOdata = "(ExtraData/Item/Data/Description eq 'Item Description 1')";
            Expression<Func<TestModel, bool>> nullMemberAccessExpression = x => x.ExtraData.Item.Data.Description == "Item Description 1";
            
            var odata = nullMemberAccessExpression.ToOdataFilter();
            odata.Should().Be(expectedOdata);

            var expectedQuery = EntityList.AsQueryable().Where(nullMemberAccessExpression);
            var resultQuery = EntityList.AsQueryable().OData().Filter(expectedOdata);
            
            Action act1 = () => resultQuery.ToList();             
            Action act2 = () => expectedQuery.ToList();            

            act1.Should().Throw<NullReferenceException>();
            act2.Should().Throw<NullReferenceException>();                    
        }   
        
        [TestMethod]
        public void Test_NullMemberAccessCheckExpression()
        {
            var expectedOdata = "((ExtraData/Item/Data ne null) and (ExtraData/Item/Data/Description eq 'Item Description 1'))";
            Expression<Func<TestModel, bool>> nullMemberAccessCheck = x => x.ExtraData.Item.Data != null && x.ExtraData.Item.Data.Description == "Item Description 1";

            CompareExpressionOData(expectedOdata, nullMemberAccessCheck);                        
        }   
        
        [TestMethod]
        public void Test_ConstantVariableExpression()
        {
            var number = 1;
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> constantVariableExpression = x => x.Id == number;

            CompareExpressionOData(expectedOdata, constantVariableExpression);                        
        }     
        
        [TestMethod]
        public void Test_UsingPropertyExpression()
        {
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> usingPropertyExpression = x => x.Id == NumberProperty;

            CompareExpressionOData(expectedOdata, usingPropertyExpression);                        
        } 
        
        [TestMethod]
        public void Test_UsingGetterOnlyPropertyExpression()
        {
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> usingGetterOnlyPropertyExpression = x => x.Id == GetterOnlyProperty;

            CompareExpressionOData(expectedOdata, usingGetterOnlyPropertyExpression);                        
        }    
        
        [TestMethod]
        public void Test_UsingStaticPropertyExpression()
        {
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> usingStaticPropertyExpression = x => x.Id == StaticProperty;

            CompareExpressionOData(expectedOdata, usingStaticPropertyExpression);                        
        }  
        
        [TestMethod]
        public void Test_UsingStaticConstExpression()
        {
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> usingStaticConstExpression = x => x.Id == StaticConst;

            CompareExpressionOData(expectedOdata, usingStaticConstExpression);                        
        }        
        
        [TestMethod]
        public void Test_UsingConstPropertyExpression()
        {
            var expectedOdata = "(Id eq 1)";
            Expression<Func<TestModel, bool>> usingConstPropertyExpression = x => x.Id == ConstProperty;

            CompareExpressionOData(expectedOdata, usingConstPropertyExpression);                        
        }                  
        
        [TestMethod]
        public void Test_MultipleMemberAccessPropertyExpression()
        {
            var expectedOdata = "(ExtraData/Item/Name eq 'Item 1')";
            Expression<Func<TestModel, bool>> multipleMemberAccessPropertyExpression = x => x.ExtraData.Item.Name == TestModelProperty.ExtraData.Item.Name;

            CompareExpressionOData(expectedOdata, multipleMemberAccessPropertyExpression);                        
        }              
        
        [TestMethod]
        public void Test_MultipleMemberAccessStaticExpression()
        {
            var expectedOdata = "(ExtraData/Item/Name eq 'Item 1')";
            Expression<Func<TestModel, bool>> multipleMemberAccessStaticExpression = x => x.ExtraData.Item.Name == TestModelStatic.ExtraData.Item.Name;

            CompareExpressionOData(expectedOdata, multipleMemberAccessStaticExpression);                        
        }    
        
        [TestMethod]
        public void Test_MultipleMemberAccessStaticPropertyExpression()
        {
            var expectedOdata = "(ExtraData/Item/Name eq 'Item 1')";
            Expression<Func<TestModel, bool>> multipleMemberAccessStaticPropertyExpression = x => x.ExtraData.Item.Name == TestModelStaticProperty.ExtraData.Item.Name;

            CompareExpressionOData(expectedOdata, multipleMemberAccessStaticPropertyExpression);                        
        }              
        
        [TestMethod]
        public void Test_BoolPropertyExpression()
        {
            var expectedOdata = "IsSomething";
            Expression<Func<TestModel, bool>> boolPropertyExpression = x => x.IsSomething;

            CompareExpressionOData(expectedOdata, boolPropertyExpression);                        
        }   
        
        [TestMethod]
        public void Test_BoolPropertyCheckExpression()
        {
            var expectedOdata = "(IsSomething eq false)";
            Expression<Func<TestModel, bool>> boolPropertyCheckExpression = x => x.IsSomething == false;

            CompareExpressionOData(expectedOdata, boolPropertyCheckExpression);                        
        } 

        [TestMethod]
        public void Test_BoolPropertyCheckNegateValueExpression()
        {
            var expectedOdata = "(IsSomething eq true)";
            Expression<Func<TestModel, bool>> boolPropertyCheckNegateValueExpression = x => x.IsSomething == !false;

            CompareExpressionOData(expectedOdata, boolPropertyCheckNegateValueExpression);                        
        } 
                
        [TestMethod]
        public void Test_NegateValueExpression()
        {
            var expectedOdata = "true";
            Expression<Func<TestModel, bool>> negateValueExpression = x => !false;

            CompareExpressionOData(expectedOdata, negateValueExpression);                        
        }   

        [TestMethod]
        public void Test_ShouldNotNegateVariable()
        {
            var value = false;
            Expression<Func<TestModel, bool>> negateVariableExpression = x => !value;
            
            Action act = () => negateVariableExpression.ToOdataFilter();

            act.Should().Throw<NotSupportedException>("You don't have negation operator in OData");
        }    
        
        [TestMethod]
        public void Test_ShouldNotNegateProperty()
        {
            Expression<Func<TestModel, bool>> negatePropertyExpression = x => !BoolProperty;
                        
            Action act = () => negatePropertyExpression.ToOdataFilter();

            act.Should().Throw<NotSupportedException>("You don't have negation operator in OData");              
        } 
        
        [TestMethod]
        public void Test_ShouldNotNegateParameterProperty()
        {
            Expression<Func<TestModel, bool>> negateParameterPropertyExpression = x => !x.IsSomething;
                        
            Action act = () => negateParameterPropertyExpression.ToOdataFilter();

            act.Should().Throw<NotSupportedException>("You don't have negation operator in OData");                    
        }   
    }
}
