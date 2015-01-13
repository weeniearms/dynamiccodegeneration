namespace ImageProcessing
{
    public class ImageFilter
    {
        private readonly double[] _filter;
        private readonly int _cxFilter;
        private readonly int _cyFilter;

        public ImageFilter(int cxFilter, double[] filter)
        {
            this._filter = filter;
            this._cxFilter = cxFilter;
            this._cyFilter = filter.Length / cxFilter;
        }

        public void Filter(byte[] src, byte[] dst, int stride, int bytesPerPixel)
        {
            int cBytes = src.Length;
            int cFilter = this._filter.Length;
            for (int iDst = 0; iDst < cBytes; iDst++)
            {
                double pixelsAccum = 0;
                double filterAccum = 0;

                for (int iFilter = 0; iFilter < cFilter; iFilter++)
                {
                    int yFilter = iFilter / this._cyFilter;
                    int xFilter = iFilter % this._cxFilter;

                    int iSrc = iDst + stride * (yFilter - this._cyFilter / 2) +
                                        bytesPerPixel * (xFilter - this._cxFilter / 2);

                    if (iSrc >= 0 && iSrc < cBytes)
                    {
                        pixelsAccum += this._filter[iFilter] * src[iSrc];
                        filterAccum += this._filter[iFilter];
                    }
                }

                if (filterAccum != 0)
                    pixelsAccum /= filterAccum;

                dst[iDst] = pixelsAccum < 0 ? (byte)0 : (pixelsAccum > 255 ?
                                                  (byte)255 : (byte)pixelsAccum);
            }
        }
    }
}