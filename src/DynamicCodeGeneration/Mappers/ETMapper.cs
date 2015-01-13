using System;
using System.Linq;
using System.Linq.Expressions;

namespace DynamicCodeGeneration.Mappers
{
    public class ETMapper<TObject> : MapperBase<TObject>
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

            var sourceParameter = Expression.Parameter(typeof(TObject), "source");
            var targetParameter = Expression.Parameter(typeof(TObject), "target");

            var propertyAssignments =
                typeof (TObject).GetProperties().Select(
                    p =>
                    Expression.Assign(Expression.MakeMemberAccess(targetParameter, p),
                                      Expression.MakeMemberAccess(sourceParameter, p)));

            var blockExpression = Expression.Block(propertyAssignments);

            this._mapper =
                Expression.Lambda<Action<TObject, TObject>>(blockExpression, sourceParameter, targetParameter).Compile();
        }
    }
}