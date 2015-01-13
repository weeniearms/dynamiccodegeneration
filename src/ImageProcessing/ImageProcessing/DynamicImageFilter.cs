using System.Reflection.Emit;

namespace ImageProcessing
{
    public class DynamicImageFilter
    {
        private readonly double[] _filter;
        private readonly int _cxFilter;
        private readonly int _cyFilter;

        public DynamicImageFilter(int cxFilter, double[] filter)
        {
            this._filter = filter;
            this._cxFilter = cxFilter;
            this._cyFilter = filter.Length / cxFilter;
        }

        public void Filter(byte[] src, byte[] dst, int stride, int bytesPerPixel)
        {
            int cBytes = src.Length;

            var dynameth = new DynamicMethod("Filter", typeof(void), new[] { typeof(byte[]), typeof(byte[]) }, GetType());
            var generator = dynameth.GetILGenerator();

            generator.DeclareLocal(typeof(int));       // Index 0 = iDst
            generator.DeclareLocal(typeof(double));    // Index 1 = pixelsAccum
            generator.DeclareLocal(typeof(double));    // Index 2 = filterAccum

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Stloc_0);

            Label labelTop = generator.DefineLabel();
            generator.MarkLabel(labelTop);

            generator.Emit(OpCodes.Ldc_R8, 0.0);
            generator.Emit(OpCodes.Dup);
            generator.Emit(OpCodes.Stloc_1);
            generator.Emit(OpCodes.Stloc_2);

            for (int iFilter = 0; iFilter < this._filter.Length; iFilter++)
            {
                if (this._filter[iFilter] == 0)
                    continue;

                int xFilter = iFilter % this._cxFilter;
                int yFilter = iFilter / this._cxFilter;
                int offset = stride * (yFilter - this._cyFilter / 2) + bytesPerPixel * (xFilter - this._cxFilter / 2);

                generator.Emit(OpCodes.Ldarg_0);

                Label labelLessThanZero = generator.DefineLabel();
                Label labelGreaterThan = generator.DefineLabel();
                Label labelLoopBottom = generator.DefineLabel();

                generator.Emit(OpCodes.Ldloc_0);        // dst index on stack
                generator.Emit(OpCodes.Ldc_I4, offset); // offset on stack
                generator.Emit(OpCodes.Add);            // Add the two
                generator.Emit(OpCodes.Dup);            // Duplicate twice
                generator.Emit(OpCodes.Dup);

                generator.Emit(OpCodes.Ldc_I4_0);
                generator.Emit(OpCodes.Blt_S, labelLessThanZero);

                generator.Emit(OpCodes.Ldc_I4, cBytes);
                generator.Emit(OpCodes.Bge_S, labelGreaterThan);

                generator.Emit(OpCodes.Ldelem_U1);
                generator.Emit(OpCodes.Conv_R8);

                if (this._filter[iFilter] == 1)
                {
                    // src element is on stack, so do nothing
                }
                else if (this._filter[iFilter] == -1)
                {
                    generator.Emit(OpCodes.Neg);
                }
                else
                {
                    generator.Emit(OpCodes.Ldc_R8, this._filter[iFilter]);
                    generator.Emit(OpCodes.Mul);
                }

                generator.Emit(OpCodes.Ldloc_1);
                generator.Emit(OpCodes.Add);
                generator.Emit(OpCodes.Stloc_1);

                generator.Emit(OpCodes.Ldc_R8, this._filter[iFilter]);
                generator.Emit(OpCodes.Ldloc_2);
                generator.Emit(OpCodes.Add);
                generator.Emit(OpCodes.Stloc_2);
                generator.Emit(OpCodes.Br, labelLoopBottom);

                generator.MarkLabel(labelLessThanZero);
                generator.Emit(OpCodes.Pop);
                generator.MarkLabel(labelGreaterThan);
                generator.Emit(OpCodes.Pop);
                generator.Emit(OpCodes.Pop);
                generator.MarkLabel(labelLoopBottom);
            }

            generator.Emit(OpCodes.Ldarg_1);     // dst array
            generator.Emit(OpCodes.Ldloc_0);     // iDst index

            Label labelSkipDivide = generator.DefineLabel();
            Label labelCopyQuotient = generator.DefineLabel();
            Label labelBlack = generator.DefineLabel();
            Label labelWhite = generator.DefineLabel();
            Label labelDone = generator.DefineLabel();

            generator.Emit(OpCodes.Ldloc_1);        // pixelsAccum
            generator.Emit(OpCodes.Ldloc_2);        // filterAccum
            generator.Emit(OpCodes.Dup);            // Make a copy
            generator.Emit(OpCodes.Ldc_R8, 0.0);    // Put 0 on stack
            generator.Emit(OpCodes.Beq_S, labelSkipDivide);

            generator.Emit(OpCodes.Div);
            generator.Emit(OpCodes.Br_S, labelCopyQuotient);

            generator.MarkLabel(labelSkipDivide);
            generator.Emit(OpCodes.Pop);             // Pop filterAccum

            generator.MarkLabel(labelCopyQuotient);
            generator.Emit(OpCodes.Dup);            // Make a copy of quotient
            generator.Emit(OpCodes.Dup);            // And another

            generator.Emit(OpCodes.Ldc_R8, 0.0);
            generator.Emit(OpCodes.Blt_S, labelBlack);

            generator.Emit(OpCodes.Ldc_R8, 255.0);
            generator.Emit(OpCodes.Bgt_S, labelWhite);

            generator.Emit(OpCodes.Conv_U1);
            generator.Emit(OpCodes.Br_S, labelDone);

            generator.MarkLabel(labelBlack);
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ldc_I4_S, 0);
            generator.Emit(OpCodes.Br_S, labelDone);

            generator.MarkLabel(labelWhite);
            generator.Emit(OpCodes.Pop);
            generator.Emit(OpCodes.Ldc_I4_S, 255);

            generator.MarkLabel(labelDone);
            generator.Emit(OpCodes.Stelem_I1);

            generator.Emit(OpCodes.Ldloc_0);    // Put iDst on stack
            generator.Emit(OpCodes.Ldc_I4_1);   // Put 1 on stack
            generator.Emit(OpCodes.Add);        // Add 1 to iDst
            generator.Emit(OpCodes.Dup);        // Duplicate
            generator.Emit(OpCodes.Stloc_0);    // Store result in iDst
            generator.Emit(OpCodes.Ldc_I4, cBytes);  // Put cBytes value on stack
            generator.Emit(OpCodes.Blt, labelTop);   // Go to top if iDst < cBytes

            generator.Emit(OpCodes.Ret);

            dynameth.Invoke(this, new object[] { src, dst });
        } 
    }
}