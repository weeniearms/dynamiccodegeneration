namespace ImageProcessing
{
    public class FixedImageFilter
    {
        private readonly double _f11;
        private readonly double _f12;
        private readonly double _f13;
        private readonly double _f21;
        private readonly double _f22;
        private readonly double _f23;
        private readonly double _f31;
        private readonly double _f32;
        private readonly double _f33;

        public FixedImageFilter(double f11, double f12, double f13, double f21, double f22, double f23, double f31, double f32, double f33)
        {
            this._f11 = f11;
            this._f12 = f12;
            this._f13 = f13;
            this._f21 = f21;
            this._f22 = f22;
            this._f23 = f23;
            this._f31 = f31;
            this._f32 = f32;
            this._f33 = f33;
        }

        public void Filter(byte[] src, byte[] dst, int stride, int bytesPerPixel)
        {
            int cBytes = src.Length;
            for (int iDst = 0; iDst < cBytes; iDst++)
            {
                double pixelsAccum = 0;
                double filterAccum = 0;

                // Filter cell F11
	            int iSrc = iDst - stride - bytesPerPixel;
	            if (iSrc >= 0 && iSrc < cBytes)
	            {
	                    pixelsAccum += src[iSrc] * this._f11;
	                    filterAccum += this._f11;
	            }

                // Filter cell F12
                iSrc = iDst - stride;
                if (iSrc >= 0 && iSrc < cBytes)
                {
	                    pixelsAccum += src[iSrc] * this._f12;
	                    filterAccum += this._f12;
                }

                // Filter cell F13
	            iSrc = iDst - stride + bytesPerPixel;
	            if (iSrc >= 0 && iSrc < cBytes)
	            {
	                    pixelsAccum += src[iSrc] * this._f13;
	                    filterAccum += this._f13;
	            }

                // Filter cell F21
                iSrc = iDst - bytesPerPixel;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f21;
                    filterAccum += this._f21;
                }

                // Filter cell F22
                iSrc = iDst;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f22;
                    filterAccum += this._f22;
                }

                // Filter cell F23
                iSrc = iDst + bytesPerPixel;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f23;
                    filterAccum += this._f23;
                }

                // Filter cell F31
                iSrc = iDst + stride - bytesPerPixel;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f31;
                    filterAccum += this._f31;
                }

                // Filter cell F32
                iSrc = iDst + stride;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f32;
                    filterAccum += this._f32;
                }

                // Filter cell F33
                iSrc = iDst + stride + bytesPerPixel;
                if (iSrc >= 0 && iSrc < cBytes)
                {
                    pixelsAccum += src[iSrc] * this._f33;
                    filterAccum += this._f33;
                }

                if (filterAccum != 0)
                    pixelsAccum /= filterAccum;

                dst[iDst] = pixelsAccum < 0 ? (byte)0 : (pixelsAccum > 255 ?
                                                  (byte)255 : (byte)pixelsAccum);
            }
        }
    }
}