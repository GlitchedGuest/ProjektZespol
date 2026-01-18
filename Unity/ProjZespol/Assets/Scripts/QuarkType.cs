using System;
using UnityEngine;
using System.IO;


public class QuarkType: IComparable<QuarkType>, IEquatable<QuarkType>
{
    public long Mantissa;
    public long Exponent;

    //Helper const values
    private const int MantissaLength = 9;
    private const long NormalizeDivisor = 100000000;
    private const long ExponentRangeForPrecision = MantissaLength + 1;

    //helper boolean check
    public bool IsNegative => Mantissa < 0;

    //Construction section

    public QuarkType() => new QuarkType(0, 0);

    public QuarkType(long mantissa, long exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize();
    }


    public QuarkType(double doubleValue)
    {

        Mantissa = 0;
        Exponent = 0;

        if (doubleValue == 0.0 || double.IsNaN(doubleValue) || double.IsInfinity(doubleValue))
        {
            return;
        }

        double absValue = Math.Abs(doubleValue);

        double log10 = Math.Log10(absValue);
        long derivedExponent = (long)Math.Floor(log10);

        double powerOf10 = Math.Pow(10.0, derivedExponent);
        double trueDecimalMantissa = absValue / powerOf10;

        // Calculate magnitude, then apply sign from doubleValue
        long magnitudeMantissa = (long)Math.Round(trueDecimalMantissa * NormalizeDivisor);

        Mantissa = (doubleValue < 0.0) ? -magnitudeMantissa : magnitudeMantissa;
        Exponent = derivedExponent;
        Normalize();
    }

    public QuarkType(long longValue) : this((double)longValue) { }
    public QuarkType(ulong ulongValue) : this((double)ulongValue) { }


    // constructors casts
    public static implicit operator double(QuarkType q)
    {
        if (q.Mantissa == 0)
            return 0.0;

        double value = (double)q.Mantissa / NormalizeDivisor;

        if (q.Exponent != 0)
            value *= Math.Pow(10.0, q.Exponent);

        return value;
    }

    public static implicit operator QuarkType(int value) => new QuarkType((double)value);
    public static implicit operator QuarkType(long value) => new QuarkType((double)value);
    public static implicit operator QuarkType(uint value) => new QuarkType((double)value);
    public static implicit operator QuarkType(ulong value) => new QuarkType((double)value);
    public static implicit operator QuarkType(float value) => new QuarkType((double)value);

    //HelperValues
    public static QuarkType Zero => new QuarkType(0, 0);
    public static QuarkType One => new QuarkType(NormalizeDivisor, 0);
    public static QuarkType NegativeOne => new QuarkType(-NormalizeDivisor, 0);

    //Normalize function
    public void Normalize()
    {
        if (Mantissa == 0)
        {
            Exponent = 0;
            return;
        }

        long mantissaMagnitude = Math.Abs(Mantissa);


        int iter = 0;
        while (mantissaMagnitude >= (NormalizeDivisor * 10) && iter++ < 100)
        {
            if (Exponent == long.MaxValue) { return; }  // Overflow check 
            mantissaMagnitude /= 10;
            Exponent++;
        }

        iter = 0;
        while (mantissaMagnitude < NormalizeDivisor && iter++ < 100)
        {
            if (Exponent == long.MinValue) { break; }  // Underflow check 
            mantissaMagnitude *= 10;
            Exponent--;
        }

        Mantissa = this.IsNegative ? -mantissaMagnitude : mantissaMagnitude;

        if (Exponent == long.MinValue && mantissaMagnitude < NormalizeDivisor)
        {
            Mantissa = 0;
            Exponent = 0;
        }


    }





    //comparison
    public override bool Equals(object obj)
    {
        return Equals(obj as QuarkType);
    }

    public bool Equals(QuarkType other)
    {
        return this.Mantissa == other.Mantissa && this.Exponent == other.Exponent;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Mantissa, Exponent);
    }

    public int CompareTo(QuarkType other)
    {
        bool thisIsNegative = this.IsNegative;
        bool otherIsNegative = other.IsNegative;

        if (thisIsNegative != otherIsNegative)
        {
            // Negative is less than positive
            return thisIsNegative ? -1 : 1;
        }
        int magnitudeComparison = CompareMagnitudeTo(other);

        if (thisIsNegative)
        {
            return -magnitudeComparison;
        }
        else
        {
            return magnitudeComparison;
        }
    }

    private int CompareMagnitudeTo(QuarkType other)
    {
        if (Exponent != other.Exponent)
        {
            return Exponent.CompareTo(other.Exponent);
        }
        return Math.Abs(Mantissa).CompareTo(Math.Abs(other.Mantissa));
    }

    //Operator overloading
    public static bool operator ==(QuarkType left, QuarkType right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(QuarkType left, QuarkType right)
    {
        return !left.Equals(right);
    }

    public static QuarkType operator -(QuarkType a)
    {
        if (a.Mantissa == 0) return Zero;
        return new QuarkType(-a.Mantissa, a.Exponent);
    }


    //Core addition logic
    public static QuarkType operator +(QuarkType a, QuarkType b)
    {
        if (a.Mantissa == 0) return b;
        if (b.Mantissa == 0) return a;


        QuarkType larger = a.Exponent >= b.Exponent ? a : b;
        QuarkType smaller = a.Exponent >= b.Exponent ? b : a;

        long exponentDifference = larger.Exponent - smaller.Exponent;

        if (exponentDifference > ExponentRangeForPrecision)
        {
            return larger;
        }

        long smallerMantissaDenormalized = smaller.Mantissa;

        for (long i = 0; i < exponentDifference; i++)
        {
            smallerMantissaDenormalized /= 10;
        }

        long newMantissaLong = larger.Mantissa + smallerMantissaDenormalized;

        return new QuarkType(newMantissaLong, larger.Exponent);
    }
    public static QuarkType operator -(QuarkType a, QuarkType b)
    {
        return a + (-b);
    }

    public static QuarkType operator *(QuarkType a, QuarkType b)
    {
        if (a.Mantissa == 0 || b.Mantissa == 0) return Zero;

        double productMagnitude = ((double)Math.Abs(a.Mantissa) * Math.Abs(b.Mantissa)) / NormalizeDivisor;
        long newMantissaMagnitude = (long)Math.Round(productMagnitude);
        long newExponent = 0;
        try
        {
            checked
            {
                newExponent = a.Exponent + b.Exponent;
            }
        }
        catch (OverflowException)
        {
            return Zero;
        }
        

        // Determine sign of the result
        bool newIsNegative = a.IsNegative != b.IsNegative;
        long newMantissa = newIsNegative ? -newMantissaMagnitude : newMantissaMagnitude;

        return new QuarkType(newMantissa, newExponent);
    }

    public static QuarkType operator /(QuarkType a, QuarkType b)
    {
        if (b.Mantissa == 0)
        {
            long sign = (a.Mantissa < 0) != (b.Mantissa < 0) ? -1 : 1;
            return new QuarkType(sign * long.MaxValue, long.MaxValue);
        }
        if (a.Mantissa == 0) return Zero;

        bool newIsNegative = (a.Mantissa < 0) != (b.Mantissa < 0);

        long newExponent = a.Exponent - b.Exponent;


        decimal mantissaA = Math.Abs(a.Mantissa);
        decimal mantissaB = Math.Abs(b.Mantissa);
        decimal newMantissaDecimal = (mantissaA / mantissaB) * NormalizeDivisor;

        if (newMantissaDecimal > long.MaxValue / 10)
        {
            while (newMantissaDecimal >= NormalizeDivisor * 10)
            {
                newMantissaDecimal /= 10;
                newExponent++;
            }
        }

        while (newMantissaDecimal < NormalizeDivisor)
        {
            newMantissaDecimal *= 10;
            newExponent--;
        }

        long newMantissaMagnitude = (long)Math.Round(newMantissaDecimal);
        long newMantissa = newIsNegative ? -newMantissaMagnitude : newMantissaMagnitude;

        return new QuarkType(newMantissa, newExponent);
    }

    public static QuarkType operator +(QuarkType a, double b) => a + new QuarkType(b);
    public static QuarkType operator +(double a, QuarkType b) => new QuarkType(a) + b;
    public static QuarkType operator -(QuarkType a, double b) => a - new QuarkType(b);
    public static QuarkType operator -(double a, QuarkType b) => new QuarkType(a) - b;
    public static QuarkType operator *(QuarkType a, double b) => a * new QuarkType(b);
    public static QuarkType operator *(double a, QuarkType b) => new QuarkType(a) * b;
    public static QuarkType operator /(QuarkType a, double b) => a / new QuarkType(b);
    public static QuarkType operator /(double a, QuarkType b) => new QuarkType(a) / b;
    public static QuarkType operator ++(QuarkType a) => a + 1.0;


    public override string ToString()
    {
        if (Mantissa == 0)
            return "0";

        long mantissa = Math.Abs(Mantissa);

        // Define suffixes
        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc" };
        int suffixIndex = (int)(Exponent / 3);

        if (suffixIndex >= suffixes.Length)
        {
            double displayMantissa = (double)mantissa / 1e8; // normalize mantissa
            long displayExponent = Exponent;
            if (IsNegative)
                displayMantissa = -displayMantissa;
            return $"{displayMantissa.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)}e{displayExponent}";
        }

        // Remainder exponent to scale mantissa within suffix range
        int expRemainder = (int)(Exponent - suffixIndex * 3);

        // Scale mantissa for display
        double displayValue = (double)mantissa / 1e8; // normalize mantissa
        if (expRemainder > 0)
            displayValue *= Math.Pow(10, expRemainder);
        else if (expRemainder < 0)
            displayValue /= Math.Pow(10, -expRemainder);

        if (IsNegative)
            displayValue = -displayValue;

        // Format value: up to 2 decimals for small numbers
        string formatted = displayValue < 10 ? displayValue.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                         : displayValue < 100 ? displayValue.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)
                         : displayValue.ToString("0", System.Globalization.CultureInfo.InvariantCulture);

        return $"{formatted}{suffixes[suffixIndex]}";
    }


    public void SerializeToStream(BinaryWriter writer)
    {
        writer.Write(Mantissa);      // 8 bytes
        writer.Write(Exponent);       // 8 bytes
    }

    public void DeserializeFromStream(BinaryReader reader)
    {
        Mantissa = reader.ReadInt64();
        Exponent = reader.ReadInt64();
    }

    public QuarkType Ceil()
    {
        if (Mantissa == 0) return new QuarkType(0, 0);

        const int NormalizeDivisorPower = 8;
        if (Exponent >= NormalizeDivisorPower)
            return new QuarkType(Mantissa, Exponent);

        long divisor = NormalizeDivisor;

        if (Exponent > 0)
        {
            for (long i = 0; i < Exponent; i++) divisor /= 10;
        }
        else if (Exponent < 0)
        {
            long magnitude = Math.Abs(Exponent);
            for (long i = 0; i < magnitude; i++)
            {
                if (long.MaxValue / 10 < divisor)
                    return IsNegative ? new QuarkType(0, 0) : new QuarkType(NormalizeDivisor, 0);
                divisor *= 10;
            }
        }

        long truncatedPart = Mantissa / divisor;
        long remainder = Mantissa % divisor;

        if (remainder != 0 && !IsNegative) truncatedPart += 1;

        long newMantissa = truncatedPart * NormalizeDivisor;
        return new QuarkType(newMantissa, 0);
    }

    public QuarkType Pow(double power)
    {
        if (Mantissa == 0)
            return new QuarkType(0, 0);

        double mantissaDouble = Mantissa / (double)NormalizeDivisor;

        double result = Math.Pow(mantissaDouble, power);

        double newExponentDouble = Exponent * power;

        long newExponent = (long)Math.Floor(newExponentDouble);
        double fractionalExponent = newExponentDouble - newExponent;

        result *= Math.Pow(10, fractionalExponent);

        long newMantissa = (long)(result * NormalizeDivisor);

        return new QuarkType(newMantissa, newExponent);
    }



    public QuarkType Floor()
    {
        if (Mantissa == 0) return new QuarkType(0, 0);

        const int NormalizeDivisorPower = 8;
        if (Exponent >= NormalizeDivisorPower)
            return new QuarkType(Mantissa, Exponent);

        long divisor = NormalizeDivisor;

        if (Exponent > 0)
        {
            for (long i = 0; i < Exponent; i++) divisor /= 10;
        }
        else if (Exponent < 0)
        {
            long magnitude = Math.Abs(Exponent);
            for (long i = 0; i < magnitude; i++)
            {
                if (long.MaxValue / 10 < divisor)
                    return IsNegative ? new QuarkType(-NormalizeDivisor, 0) : new QuarkType(0, 0);
                divisor *= 10;
            }
        }

        long truncatedPart = Mantissa / divisor;
        long remainder = Mantissa % divisor;

        if (remainder != 0 && IsNegative) truncatedPart -= 1;

        long newMantissa = truncatedPart * NormalizeDivisor;
        return new QuarkType(newMantissa, 0);
    }
}
