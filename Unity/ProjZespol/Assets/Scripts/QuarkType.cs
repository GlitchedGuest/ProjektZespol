using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Numerics;
using Unity.VisualScripting;

[StructLayout(LayoutKind.Sequential)]
public class QuarkType : IComparable<QuarkType>, IEquatable<QuarkType>
{
    public uint Mantissa;
    private uint _mantisLim;
    
    public ulong Exponent;    
    private ulong _ExponentLim;

    private const int MantissaLength = 9;
    private const uint NormalizeDivisor = 100000000;
    private const int MaxPrecisionLossShift = MantissaLength + 2;

    public QuarkType(uint mantissa, ulong exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize();
    }
    public QuarkType(double doubleValue)
    {

        Mantissa = 0;
        Exponent = 0;

        if (doubleValue <= 0.0)
        {
            if (doubleValue < 0.0)
                throw new ArgumentException("QuarkType does not support negative numbers.", nameof(doubleValue));
            return; 
        }


        double log10 = Math.Log10(doubleValue);
        ulong derivedExponent = (ulong)Math.Floor(log10);


        double powerOf10 = Math.Pow(10.0, derivedExponent);
        double trueDecimalMantissa = doubleValue / powerOf10;

        Mantissa = (uint)Math.Round(trueDecimalMantissa * NormalizeDivisor);
        Exponent = derivedExponent;

        Normalize();
    }

    public QuarkType(long longValue)
    {
        Mantissa = 0;
        Exponent = 0;

        if (longValue < 0)
        {
            throw new ArgumentException("QuarkType does not support negative numbers.", nameof(longValue));
        }
        if (longValue == 0) return;

        ulong tempValue = (ulong)longValue;

        double log10 = Math.Log10(tempValue);
        ulong derivedExponent = (ulong)Math.Floor(log10);

        double powerOf10 = Math.Pow(10.0, derivedExponent);
        double trueDecimalMantissa = (double)tempValue / powerOf10;

        Mantissa = (uint)Math.Round(trueDecimalMantissa * NormalizeDivisor);
        Exponent = derivedExponent;

        Normalize();
    }

    public QuarkType(ulong ulongValue)
    {
        Mantissa = 0;
        Exponent = 0;

        if (ulongValue == 0) return;

        double log10 = Math.Log10(ulongValue);
        ulong derivedExponent = (ulong)Math.Floor(log10);

        double powerOf10 = Math.Pow(10.0, derivedExponent);
        double trueDecimalMantissa = (double)ulongValue / powerOf10;

        Mantissa = (uint)Math.Round(trueDecimalMantissa * NormalizeDivisor);
        Exponent = derivedExponent;

        Normalize();
    }

    public QuarkType(ulong largeMantissa, ulong exponent)
    {
        Mantissa = 0;
        Exponent = 0;

        if (largeMantissa == 0)
        {
            return;
        }

        ulong tempMantissa = largeMantissa;
        ulong finalExponent = exponent;

        while (tempMantissa >= (ulong)(NormalizeDivisor * 10))
        {
            tempMantissa /= 10;
            finalExponent++;
        }

        while (tempMantissa < NormalizeDivisor && finalExponent > 0)
        {
            tempMantissa *= 10;
            finalExponent--;
        }

        if (finalExponent == 0 && tempMantissa < NormalizeDivisor)
        {
            while (tempMantissa < NormalizeDivisor && tempMantissa > 0)
            {
                tempMantissa *= 10;
            }
        }

        Mantissa = (uint)tempMantissa;
        Exponent = finalExponent;
    }



    public static implicit operator QuarkType(int value) => new QuarkType((long)value);
    public static implicit operator QuarkType(short value) => new QuarkType((long)value);
    public static implicit operator QuarkType(byte value) => new QuarkType((long)value);
    public static implicit operator QuarkType(uint value) => new QuarkType((ulong)value);
    public static implicit operator QuarkType(ushort value) => new QuarkType((ulong)value);
    public static implicit operator QuarkType(float value) => new QuarkType((double)value);



    public static QuarkType Zero = new QuarkType(0, 0);
    public static QuarkType One = new QuarkType(NormalizeDivisor, 0);
    public void Normalize()
    {
        if (Mantissa == 0)
        {
            Exponent = 0;
            return;
        }
        
        while (Mantissa >= (NormalizeDivisor * 10))
        {
            if (Exponent == ulong.MaxValue) return;
            Mantissa /= 10;
            Exponent++;
        }
        while (Mantissa < NormalizeDivisor && Exponent > 0)
        {
            Mantissa *= 10;
            Exponent--;
        }

        if (Exponent == 0 && Mantissa < NormalizeDivisor)
        {
            while (Mantissa < NormalizeDivisor && Mantissa > 0)
            {
                Mantissa *= 10;
            }
        }

        //limit the value if needed.
        if (!(_ExponentLim == 0 && 0 == _mantisLim))
        {
            if (Exponent > _ExponentLim)
            { Exponent = _ExponentLim;
                Mantissa = _mantisLim;
            }

            if (Exponent == _ExponentLim) {
                if (Mantissa > _mantisLim)
                {
                    Mantissa = _mantisLim;
                }
            
            }


        }
    }
    private static QuarkType Add(QuarkType larger, QuarkType smaller)
    {
        return larger + smaller;
    }

    public bool Equals(QuarkType other)
    {
        return this.Mantissa == other.Mantissa && this.Exponent == other.Exponent;
    }

    public int CompareTo(QuarkType other)
    {
        if (Exponent != other.Exponent)
        {
            return Exponent.CompareTo(other.Exponent);
        }

        return Mantissa.CompareTo(other.Mantissa);
    }

    // --- Equality and Inequality ---

    public static bool operator ==(QuarkType left, QuarkType right)
    {
        // Use the Equals method defined above for standard equality checking.
        return left.Equals(right);
    }

    public static bool operator !=(QuarkType left, QuarkType right)
    {
        return !(left == right);
    }

    // --- Comparison Operators ---

    public static bool operator <(QuarkType left, QuarkType right)
    {
        // left < right if CompareTo returns a value less than zero (-1)
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(QuarkType left, QuarkType right)
    {
        // left > right if CompareTo returns a value greater than zero (1)
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(QuarkType left, QuarkType right)
    {
        // left <= right if CompareTo returns a value less than or equal to zero (-1 or 0)
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(QuarkType left, QuarkType right)
    {
        // left >= right if CompareTo returns a value greater than or equal to zero (1 or 0)
        return left.CompareTo(right) >= 0;
    }

    public static QuarkType operator +(QuarkType a, QuarkType b)
    {

        if (a.Mantissa == 0) return b;
        if (b.Mantissa == 0) return a;

        if (a.Exponent < b.Exponent)
        {
            return Add(b, a); 
        }

        ulong exponentDifference = a.Exponent - b.Exponent;

        if (exponentDifference > (ulong)MantissaLength + 1)
        {
            return a;
        }

        uint bMantissaDenormalized = b.Mantissa;

        for (ulong i = 0; i < exponentDifference; i++)
        {
            bMantissaDenormalized /= 10;
        }

        ulong newMantissaULong = (ulong)a.Mantissa + bMantissaDenormalized;

        QuarkType result = new QuarkType((uint)newMantissaULong, a.Exponent);
        result.Normalize();
        return result;
    }

    public static QuarkType operator +(QuarkType a, double b)
    {
        // Convert the primitive type 'b' into a QuarkType, then use the core QuarkType + QuarkType operator.
        QuarkType bQuark = new QuarkType(b);
        return a + bQuark;
    }

    public static QuarkType operator +(double a, QuarkType b)
    {
        return b + a;
    }

    public static QuarkType operator +(QuarkType a, long b)
    {
        if (b < 0)
        {
            throw new ArgumentException("Cannot add a negative 'long' value to QuarkType.", nameof(b));
        }
        QuarkType bQuark = new QuarkType(b);
        return a + bQuark;
    }

    public static QuarkType operator +(long a, QuarkType b)
    {
        return b + a;
    }

    public static QuarkType operator +(QuarkType a, ulong b)
    {
        QuarkType bQuark = new QuarkType(b);
        return a + bQuark;
    }

    public static QuarkType operator +(ulong a, QuarkType b)
    {
        return b + a;
    }
    public static QuarkType operator +(float a, QuarkType b) => (double)a + b;
    public static QuarkType operator +(QuarkType a, float b) => a + (double)b;

    public static QuarkType operator +(QuarkType a, short b) => a + (long)b;
    public static QuarkType operator +(short a, QuarkType b) => (long)a + b;

    public static QuarkType operator +(QuarkType a, byte b) => a + (long)b;
    public static QuarkType operator +(byte a, QuarkType b) => (long)a + b;

    public static QuarkType operator +(QuarkType a, uint b) => a + (ulong)b;
    public static QuarkType operator +(uint a, QuarkType b) => (ulong)a + b;

    public static QuarkType operator *(QuarkType a, QuarkType b)
    {
        if (a.Mantissa == 0 || b.Mantissa == 0) return Zero;

        // 1. Multiply mantissas (can overflow a uint, so use ulong)
        ulong newMantissaULong = (ulong)a.Mantissa * b.Mantissa / NormalizeDivisor;

        // 2. Add exponents (E_A + E_B)
        ulong newExponent = a.Exponent + b.Exponent;

        return new QuarkType(newMantissaULong, newExponent);
    }

    public static QuarkType operator *(QuarkType a, double b)
    {
       
        if (b < 0.0)
        {
            throw new ArgumentException("Cannot multiply by a negative double/float value; QuarkType does not support negative results.", nameof(b));
        }

        QuarkType bQuark = new QuarkType(b);
        return a * bQuark; 
    }

    public static QuarkType operator *(QuarkType a, long b)
    {
        if (b < 0)
        {
            throw new ArgumentException("Cannot multiply by a negative 'long' value; QuarkType does not support negative results.", nameof(b));
        }

        QuarkType bQuark = new QuarkType(b);
        return a * bQuark;
    }

    public static QuarkType operator *(QuarkType a, ulong b)
    {
        QuarkType bQuark = new QuarkType(b);
        return a * bQuark;
    }

    public static QuarkType operator *(double a, QuarkType b) =>  b * a;
    public static QuarkType operator *(long a, QuarkType b) => b * a;
    public static QuarkType operator *(ulong a, QuarkType b) => b * a;
    public static QuarkType operator *(float a, QuarkType b) => (double)a * b;
    public static QuarkType operator *(QuarkType a, float b) => a * (double)b;

    public static QuarkType operator *(QuarkType a, short b) => a * (long)b;
    public static QuarkType operator *(short a, QuarkType b) => (long)a * b;

    public static QuarkType operator *(QuarkType a, byte b) => a * (long)b;
    public static QuarkType operator *(byte a, QuarkType b) => (long)a * b;

    public static QuarkType operator *(QuarkType a, uint b) => a * (ulong)b;
    public static QuarkType operator *(uint a, QuarkType b) => (ulong)a * b;
    public static QuarkType operator -(QuarkType a, QuarkType b)
    {
        if (b.Mantissa == 0) return a;
        if (a.Mantissa == 0) return Zero;
  
        if (a.CompareTo(b) < 0)
        {
            return Zero; 
        }
       
        ulong exponentDifference = a.Exponent - b.Exponent;

        if (exponentDifference >= (ulong)MaxPrecisionLossShift)
        {
            return a;
        }

        ulong bMantissaDenormalized = b.Mantissa;

        for (ulong i = 0; i < exponentDifference; i++)
        {
            bMantissaDenormalized /= 10;
        }

        long newMantissaLong = (long)a.Mantissa - (long)bMantissaDenormalized;

        uint resultMantissa = (uint)newMantissaLong;
        ulong resultExponent = a.Exponent;

        if (resultMantissa == 0)
        {
            return Zero;
        }

        QuarkType result = new QuarkType(resultMantissa, resultExponent);
        return result;
    }

    public static QuarkType operator -(QuarkType a, double b)
    {
        QuarkType bQuark = new QuarkType(b);
        return a - bQuark; 
    }

    public static QuarkType operator -(double a, QuarkType b)
    {
        QuarkType aQuark = new QuarkType(a);
        return aQuark - b;
    }

    public static QuarkType operator -(ulong a, QuarkType b)
    {
        QuarkType aQuark = new QuarkType(a);
        return aQuark - b;
    }

    public static QuarkType operator -(QuarkType a, ulong b)
    {
        QuarkType bQuark = new QuarkType(b);
        return a - bQuark;
    }

    public static QuarkType operator -(long a, QuarkType b)
    {
        if (a < 0)
        {
            throw new ArgumentException("Cannot start subtraction with a negative 'long' value, as QuarkType does not support negative results.", nameof(a));
        }
        QuarkType aQuark = new QuarkType((ulong)a);
        return aQuark - b;
    }
    public static QuarkType operator -(QuarkType a, long b)
    {
        QuarkType bQuark = new QuarkType(b);
        return a - bQuark;
    }

    public static QuarkType operator -(float a, QuarkType b) => (double)a - b;
    public static QuarkType operator -(QuarkType a, float b) => a - (double)b;

    public static QuarkType operator -(QuarkType a, short b) => a - (long)b;
    public static QuarkType operator -(short a, QuarkType b) => (long)a - b;

    public static QuarkType operator -(QuarkType a, byte b) => a - (long)b;
    public static QuarkType operator -(byte a, QuarkType b) => (long)a - b;

    public static QuarkType operator -(QuarkType a, uint b) => a - (ulong)b;
    public static QuarkType operator -(uint a, QuarkType b) => (ulong)a - b;


    public double Log10()
    {
        if (Mantissa == 0)
        {
            return double.NegativeInfinity;
        }

        double decimalMantissa = (double)Mantissa / NormalizeDivisor;

        double logM = Math.Log10(decimalMantissa);

        return logM + (double)Exponent;
    }

    public static QuarkType operator ++(QuarkType a) => a + 1;
    public QuarkType Pow(double power)
    {
        if (Mantissa == 0) return Zero;
        if (power == 0.0) return One;
        if (power == 1.0) return this;

        double logValue = this.Log10();

        double rawNewExponent = logValue * power;

        if (rawNewExponent > ulong.MaxValue)
        {
            return new QuarkType(uint.MaxValue, ulong.MaxValue);
        }

        ulong newExponent = (ulong)Math.Floor(rawNewExponent);

        double mantissaFraction = rawNewExponent - newExponent;

        double newMantissaDecimal = Math.Pow(10.0, mantissaFraction);

        uint newMantissa = (uint)Math.Round(newMantissaDecimal * NormalizeDivisor);

        return new QuarkType(newMantissa, newExponent);
    }

    public double LogBase(double baseValue)
    {
        if (Mantissa == 0)
        {
            return double.NegativeInfinity;
        }

        if (baseValue <= 0 || baseValue == 1.0)
        {
            throw new ArgumentException("Logarithm base must be positive and not equal to 1.", nameof(baseValue));
        }

        double log10_X = this.Log10();

        double log10_Base = Math.Log10(baseValue);

        if (log10_Base == 0.0)
        {
            return double.NaN;
        }

        return log10_X / log10_Base;
    }

    public override string ToString()
    {
        if (Mantissa == 0) return "0";
        double decimalMantissa = (double)Mantissa / NormalizeDivisor;
        return $"{decimalMantissa.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}e{Exponent}";
    }
    public void clearLimit() { 
        _mantisLim = 0;
        _ExponentLim = 0;
    }
    public void SetLimit(QuarkType t) {
        _mantisLim = t.Mantissa;
        _ExponentLim = t.Exponent;
    }
    public QuarkType GetLimit() { return new QuarkType(_mantisLim, _ExponentLim); }
}
