using System.Collections.Generic;

namespace Linq2OData.Tests
{
    public class TestModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public bool IsSomething { get; set; }

        public IEnumerable<string> Collection { get; set; } 

        public IEnumerable<TestModel> SelfCollection { get; set; } 

        public TestModelExtraData ExtraData { get; set; }
    }        

    public class TestModelExtraData
    {
        public int SomeNumber { get; set; }

        public TestModelItem Item { get; set; }            
    }

    public class TestModelItem
    {
        public string Name { get; set; }

        public TestModelItemData Data { get; set; }
    }

    public class TestModelItemData
    {
        public string Description { get; set; }

        public int Index { get; set; }
    }
}