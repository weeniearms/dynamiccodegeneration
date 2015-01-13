using System;
using DynamicCodeGeneration.Mappers;
using NUnit.Framework;

namespace DynamicCodeGeneration.Tests.Mappers
{
    [TestFixture]
    public class SimpleMapperTests : MapperTestsBase<SimpleMapperTests.SimpleMapper>
    {
        public class SimpleMapper : MapperBase<MappedObject>
        {
            public override void Map(MappedObject sourceObject, MappedObject targetObject)
            {
                targetObject.Id = sourceObject.Id;
                targetObject.Name = sourceObject.Name;
                targetObject.Age = sourceObject.Age;
            }
        }
    }
}