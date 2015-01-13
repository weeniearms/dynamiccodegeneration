namespace DynamicCodeGeneration.Mappers
{
    public class ReflectionMapper<TObject> : MapperBase<TObject>
    {
        public override void Map(TObject sourceObject, TObject targetObject)
        {
            foreach (var property in typeof(TObject).GetProperties())
            {
                var value = property.GetValue(sourceObject, null);
                property.SetValue(targetObject, value, null);
            }
        }
    }
}