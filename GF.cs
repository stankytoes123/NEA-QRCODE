using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_QRCODE
{
    

    public class GF
    {
        byte GFbin;

        public GF(int GFint)
        {
            if (GFint <= 0 || GFint > 255)
            {
                throw new ArgumentException("Value must be 0-255 inclusive");
            }
            GFbin = (byte)GFint;
        }

        public static GF operator +(GF g1, GF g2)
        {
            return new GF((byte)(g1.GFbin ^ g2.GFbin));
        }

        public static GF operator -(GF g1, GF g2)
        {
            return new GF((byte)(g1.GFbin ^ g2.GFbin));
        }



    }
}
