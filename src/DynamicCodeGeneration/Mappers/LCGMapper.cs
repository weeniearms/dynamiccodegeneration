using System;
using System.Reflection.Emit;

namespace DynamicCodeGeneration.Mappers
{
    public class LCGMapper<TObject> : MapperBase<TObject>
    {
        private Action<TObject, TObject> _mapper;

        public override void Map(TObject sourceObject, TObject targetObject)
        {
            this.CreateMapper();

            this._mapper(sourceObject, targetObject);
        }

        private void CreateMapper()
        {
            if (this._mapper != null)
                return;

            var dynamicMethod = new DynamicMethod("Map", typeof (void), new[] {typeof (TObject), typeof (TObject)}, false);
            var il = dynamicMethod.GetILGenerator();

            foreach (var property in typeof(TObject).GetProperties())
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, property.GetGetMethod());
                il.Emit(OpCodes.Callvirt, property.GetSetMethod());
            }

            il.Emit(OpCodes.Ret);

            this._mapper = dynamicMethod.CreateDelegate(typeof (Action<TObject, TObject>)) as Action<TObject, TObject>;
        }
    }
}