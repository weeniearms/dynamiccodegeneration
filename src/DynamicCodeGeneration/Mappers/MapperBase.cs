namespace DynamicCodeGeneration.Mappers
{
    public abstract class MapperBase<TObject>
    {
        public abstract void Map(TObject sourceObject, TObject targetObject);
    }
}