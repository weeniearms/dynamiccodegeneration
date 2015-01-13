using DynamicCodeGeneration.Mappers;
using NUnit.Framework;

namespace DynamicCodeGeneration.Tests.Mappers
{
    [TestFixture]
    public abstract class MapperTestsBase<TMapper>
        where TMapper : MapperBase<MappedObject>, new()
    {
        private TMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            this._mapper = new TMapper();
        }

        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        public void ShouldProperlyMapObject(int repeats)
        {
            for (int i = 0; i < repeats; i++)
            {
                var sourceObject = new MappedObject
                                       {
                                           Id = 1,
                                           Name = "Asmodeus",
                                           Age = 27
                                       };

                var targetObject = new MappedObject();

                this._mapper.Map(sourceObject, targetObject);

                Assert.AreEqual(sourceObject, targetObject);
            }
        }
    }
}