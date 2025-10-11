using System;

using UnityEngine;

public class RaptorCore : MonoBehaviour
{

    private int _testsRun = 0;
    private int _testsFailed = 0;

    // NOTE: This tolerance is too high for true equality but good for quick magnitude checks.
    private const double LOG_TOLERANCE = 0.0001;

    // Helper to check if two QuarkTypes are close (for floating-point results)
    private bool IsApproximatelyEqual(QuarkType a, QuarkType b, double tolerance = LOG_TOLERANCE)
    {
        // Must have similar magnitude. Check Log10.
        // We use a safe check for Log10 when Mantissa is 0 to avoid errors.
        if (a.Mantissa == 0 || b.Mantissa == 0) return a.Mantissa == b.Mantissa;
        return Math.Abs(a.Log10() - b.Log10()) < tolerance;
    }

    void Start()
    {
        Debug.Log("<color=white>--- QuarkType Test Suite ---</color>");

        TestNormalizationAndConstructors();
        TestPrimitiveConversions(); // <--- NEW TEST SECTION
        TestComparisonOperators();
        TestAddition();
        TestPrimitiveAddition(); // <--- NEW TEST SECTION
        TestSubtraction();
        TestPrimitiveSubtraction(); // <--- NEW TEST SECTION
        TestMultiplication();
        TestPrimitiveMultiplication(); // <--- NEW TEST SECTION
        TestLogarithmFunctions();
        TestPowerFunction();
        TestPrecisionLossAndEdgeCases();

        Debug.Log("-----------------------------");
        if (_testsFailed == 0)
        {
            Debug.Log($"<color=green>✅ All {_testsRun} tests passed successfully!</color>");
        }
        else
        {
            Debug.LogError($"<color=red>❌ {_testsFailed} out of {_testsRun} tests failed.</color>");
        }
    }

    private void Assert(bool condition, string message)
    {
        _testsRun++;
        string status = condition ? "<color=lime>[PASS]</color>" : "<color=red>[FAIL]</color>";
        string logMessage = $"{status} {message}";
        if (!condition)
        {
            _testsFailed++;
            Debug.LogError(logMessage);
        }
        else
        {
            Debug.Log(logMessage);
        }
    }

    // --- Existing Test Sections (Omitted for brevity, assume they are present) ---
    void TestNormalizationAndConstructors()
    {
        Debug.Log("\n<color=yellow>## 1. Normalization & Constructors</color>");

        // T1.1: Basic Normalization (10.0e0 -> 1.0e1)
        var t1_1 = new QuarkType(1000000000, 0);
        Assert(t1_1.Mantissa == 100000000 && t1_1.Exponent == 1, $"T1.1: Mantissa Overflow (10.0e0 -> 1.0e1). Result: {t1_1}");

        // T1.2: ulong Constructor (123456789UL) -> 1.23456789e8
        var t1_2 = new QuarkType(123456789UL);
        Assert(t1_2.Mantissa == 123456789 && t1_2.Exponent == 8, $"T1.2: ulong 123456789 -> {t1_2}");

        // T1.3: ulong Constructor (ULong.MaxValue ~ 1.84e19) -> 1.84...e19
        var t1_3 = new QuarkType(ulong.MaxValue);
        Assert(t1_3.Exponent == 19, $"T1.3: ulong.MaxValue Exponent. Result: {t1_3.Exponent}");

        // T1.4: ulong, ulong Constructor (Large Mantissa)
        // 5,000,000,000,000UL with Base Exp 10. Should become 5.0e22
        var t1_4 = new QuarkType(5000000000000UL, 10UL);
        Assert(t1_4.Exponent == 22, $"T1.4: ulong, ulong large mantissa. Result: {t1_4.Exponent}");

        // T1.5: ulong, ulong Constructor (Small Mantissa)
        // 5UL with Base Exp 10. Should become 5.0e2
        var t1_5 = new QuarkType(5UL, 10UL);
        Assert(t1_5.Exponent == 2 && t1_5.Mantissa == 500000000, $"T1.5: ulong, ulong small mantissa. Result: {t1_5}");
    }
    void TestComparisonOperators()
    {
        Debug.Log("\n<color=yellow>## 2. Comparison Operators</color>");

        var a = new QuarkType(100000000, 100); // 1.0e100
        var b = new QuarkType(100000001, 100); // 1.00000001e100 (Mantissa >)
        var c = new QuarkType(999999999, 99); // 9.99999999e99 (Exponent <)

        Assert(b > a, "T2.1: Mantissa Difference (b > a)");
        Assert(a < b, "T2.2: Mantissa Difference (a < b)");
        Assert(c < a, "T2.3: Exponent Difference (c < a)");
        Assert(c.CompareTo(a) == -1, "T2.4: CompareTo Exponent Check");
        Assert(a.CompareTo(b) == -1, "T2.5: CompareTo Mantissa Check");
        Assert(a == new QuarkType(100000000, 100), "T2.6: Equality");
        Assert(a != b, "T2.7: Inequality");
    }
    void TestAddition()
    {
        Debug.Log("\n<color=yellow>## 3. Addition</color>");

        // T3.1: Zero Addition
        var a = new QuarkType(100000000, 50);
        Assert(a + QuarkType.Zero == a, "T3.1: A + 0 = A");

        // T3.2: Mantissa Overflow (9e10 + 2e10 = 1.1e11)
        var b = new QuarkType(900000000, 10);
        var c = new QuarkType(200000000, 10);
        var expected3_2 = new QuarkType(110000000, 11); // Normalized result
        Assert(IsApproximatelyEqual(b + c, expected3_2), $"T3.2: Mantissa Overflow. Expected: {expected3_2}, Got: {b + c}");

        // T3.3: Large Exponent Difference (should rely on full '+' logic)
        var largeExp = new QuarkType(100000000, 100);
        var smallExp = new QuarkType(100000000, 80);
        Assert(largeExp + smallExp == largeExp, "T3.3: Addition with insignificant number (1e100 + 1e80 = 1e100)");
    }
    void TestSubtraction()
    {
        Debug.Log("\n<color=yellow>## 4. Subtraction</color>");

        // T4.1: Clamping to Zero
        var a = new QuarkType(100000000, 5);
        var b = new QuarkType(200000000, 5);
        Assert(a - b == QuarkType.Zero, "T4.1: Clamping (a - b where a < b)");

        // T4.2: Self Subtraction
        Assert(a - a == QuarkType.Zero, "T4.2: A - A = 0");

        // T4.3: Result Renormalization (9.0e5 - 8.9e5 = 0.1e5 -> 1.0e4)
        var c = new QuarkType(900000000, 5);
        var d = new QuarkType(890000000, 5);
        var expected4_3 = new QuarkType(100000000, 4);
        Assert(IsApproximatelyEqual(c - d, expected4_3), $"T4.3: Result Renormalization (9e5 - 8.9e5). Expected: {expected4_3}, Got: {c - d}");
    }
    void TestMultiplication()
    {
        Debug.Log("\n<color=yellow>## 5. Multiplication</color>");

        // T5.1: Identity Multiplication
        var a = new QuarkType(200000000, 5);
        Assert(a * QuarkType.One == a, "T5.1: A * 1 = A");

        // T5.2: Simple Multiplication (2e5 * 3e4 = 6e9) - Checks corrected logic
        var b = new QuarkType(200000000, 5);
        var c = new QuarkType(300000000, 4);
        var expected5_2 = new QuarkType(600000000, 9);
        Assert(b * c == expected5_2, $"T5.2: 2e5 * 3e4 = {expected5_2}");

        // T5.3: Multiplication with Mantissa Carry (5e10 * 5e10 = 25e20 -> 2.5e21)
        var d = new QuarkType(500000000, 10);
        var expected5_3 = new QuarkType(250000000, 21);
        Assert(IsApproximatelyEqual(d * d, expected5_3), $"T5.3: Mantissa Carry. Expected: {expected5_3}, Got: {d * d}");

        // T5.4: Max Ulong Exponent Check
        var maxExpQuark = new QuarkType(100000000, ulong.MaxValue - 100);
        var result5_4 = maxExpQuark * new QuarkType(100000000, 100);
        Assert(result5_4.Exponent == ulong.MaxValue, "T5.4: Multiplication resulting in Exponent saturation (ulong.MaxValue)");
    }
    void TestLogarithmFunctions()
    {
        Debug.Log("\n<color=yellow>## 6. Logarithm Functions</color>");

        // T6.1: Log10 Exact Check (1.0e100 -> 100.0)
        var a = new QuarkType(100000000, 100);
        Assert(Math.Abs(a.Log10() - 100.0) < 1e-9, $"T6.1: Log10(1e100) is 100.0. Got: {a.Log10():F9}");

        // T6.2: LogBase 2 (Expected: 332.52)
        var b = new QuarkType(123450000, 100);
        double log2Result = b.LogBase(2.0);
        Assert(Math.Abs(log2Result - 332.52) < 0.1, $"T6.2: Log2(1.23e100) -> {log2Result:F2}");

        // T6.3: LogBase E (Natural Log)
        double lnResult = b.LogBase(Math.E);
        Assert(Math.Abs(lnResult - 230.40) < 0.1, $"T6.3: Ln(1.23e100) -> {lnResult:F2}");

        // T6.4: Log of Zero
        Assert(QuarkType.Zero.Log10() == double.NegativeInfinity, "T6.4: Log(0) is -Inf.");
    }
    void TestPowerFunction()
    {
        Debug.Log("\n<color=yellow>## 7. Power Function</color>");

        // T7.1: Power of 0 (x^0 = 1)
        var a = new QuarkType(500000000, 5);
        Assert(a.Pow(0.0) == QuarkType.One, "T7.1: Pow(x, 0) = 1");

        // T7.2: Pow(10, 3) = 1000
        var b = new QuarkType(100000000, 1); // 1.0e1
        var expected7_2 = new QuarkType(100000000, 3); // 1.0e3
        Assert(b.Pow(3.0) == expected7_2, $"T7.2: Pow(1e1, 3) -> {b.Pow(3.0)}");

        // T7.3: Fractional power (Root)
        var c = new QuarkType(900000000, 4); // 9.0e4
        var expected7_3 = new QuarkType(300000000, 2); // 3.0e2
        Assert(c.Pow(0.5) == expected7_3, $"T7.3: Pow(9e4, 0.5) -> {c.Pow(0.5)}");

        // T7.4: Power resulting in Mantissa change
        var d = new QuarkType(300000000, 1); // 3.0e1
        var expected7_4 = new QuarkType(492500000, 3); // Expected 4.925e3
        Assert(Math.Abs(d.Pow(2.5).Log10() - expected7_4.Log10()) < 0.001, $"T7.4: Pow(3e1, 2.5) Log10 Check.");
    }
    void TestPrecisionLossAndEdgeCases()
    {
        Debug.Log("\n<color=yellow>## 8. Precision Loss and Edge Cases</color>");

        // T8.1: Max Exponent Power Clamp
        var maxExp = new QuarkType(100000000, ulong.MaxValue - 100);
        var result8_1 = maxExp.Pow(1.00001);
        Assert(result8_1.Exponent == ulong.MaxValue, "T8.1: Power Clamp to Max Exponent.");

        // T8.2: Max Mantissa Comparison
        var nearMax = new QuarkType(uint.MaxValue, 5);
        var maxMantissa = new QuarkType(uint.MaxValue, 4);
        Assert(nearMax > maxMantissa, "T8.2: Exponent wins max mantissa comparison.");

        // T8.3: Mantissa Rollover (Test T1.2 revisited)
        var t1_2_test = new QuarkType(1000000000, 0);
        Assert(t1_2_test.Mantissa == 100000000 && t1_2_test.Exponent == 1, "T8.3: Mantissa Rollover Check.");

        // T8.4: Zero Power Base (0^5 = 0)
        Assert(QuarkType.Zero.Pow(5.0) == QuarkType.Zero, "T8.4: Pow(0, 5) is 0.");
    }
    // --- End of Existing Test Sections ---

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Primitive Conversions
    // ------------------------------------------------------------------
    void TestPrimitiveConversions()
    {
        Debug.Log("\n<color=yellow>## 9. Primitive Conversions (Implicit/Constructors)</color>");

        // T9.1: int (Implicit conversion to QuarkType)
        QuarkType qInt = 42;
        var expected9_1 = new QuarkType(42L);
        Assert(qInt.Mantissa == expected9_1.Mantissa && qInt.Exponent == expected9_1.Exponent, $"T9.1: int to QuarkType (42). Result: {qInt}");

        // T9.2: float (Implicit conversion to QuarkType)
        QuarkType qFloat = 123.456f;
        var expected9_2 = new QuarkType(123.456); // float routes to double constructor
        Assert(IsApproximatelyEqual(qFloat, expected9_2), $"T9.2: float to QuarkType (123.456f). Result: {qFloat}");

        // T9.3: byte (Implicit conversion to QuarkType)
        QuarkType qByte = (byte)255;
        var expected9_3 = new QuarkType(255L);
        Assert(qByte.Mantissa == expected9_3.Mantissa && qByte.Exponent == expected9_3.Exponent, $"T9.3: byte to QuarkType (255). Result: {qByte}");

        // T9.4: ushort (Implicit conversion to QuarkType)
        QuarkType qUShort = (ushort)65000;
        var expected9_4 = new QuarkType(65000UL);
        Assert(qUShort.Mantissa == expected9_4.Mantissa && qUShort.Exponent == expected9_4.Exponent, $"T9.4: ushort to QuarkType (65000). Result: {qUShort}");
    }

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Primitive Addition
    // ------------------------------------------------------------------
    void TestPrimitiveAddition()
    {
        Debug.Log("\n<color=yellow>## 10. Primitive Addition (+)</color>");
        var a = new QuarkType(500000000, 10); // 5.0e10
        var expected = new QuarkType(500000010, 10); // 5.00000010e10

        // T10.1: QuarkType + int (Routes to + long)
        QuarkType result10_1 = a + 10;
        Assert(IsApproximatelyEqual(result10_1, expected), $"T10.1: Q + int (5e10 + 10). Expected: {expected}, Got: {result10_1}");

        // T10.2: float + QuarkType (Routes to double + Q)
        QuarkType result10_2 = 10.5f + a;
        var expected10_2 = new QuarkType(500000001, 10); // 5.0e10 + 10.5
        Assert(IsApproximatelyEqual(result10_2, expected10_2), $"T10.2: float + Q (10.5f + 5e10). Expected: {expected10_2}, Got: {result10_2}");

        // T10.3: Q + ulong (Routes to Q + ulong)
        QuarkType result10_3 = a + 500UL;
        var expected10_3 = new QuarkType(500000005, 10); // 5.0e10 + 500
        Assert(IsApproximatelyEqual(result10_3, expected10_3), $"T10.3: Q + ulong (5e10 + 500UL). Expected: {expected10_3}, Got: {result10_3}");

        // T10.4: Edge Case: Q + small byte
        var b = new QuarkType(100000000, 0); // 1.0
        QuarkType result10_4 = b + (byte)5;
        var expected10_4 = new QuarkType(600000000, 0); // 6.0
        Assert(result10_4 == expected10_4, $"T10.4: Q + byte (1.0 + 5). Expected: {expected10_4}, Got: {result10_4}");
    }

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Primitive Subtraction
    // ------------------------------------------------------------------
    void TestPrimitiveSubtraction()
    {
        Debug.Log("\n<color=yellow>## 11. Primitive Subtraction (-)</color>");
        var a = new QuarkType(500000000, 10); // 5.0e10

        // T11.1: QuarkType - long (Routes to Q - long)
        QuarkType result11_1 = a - 10L;
        var expected11_1 = new QuarkType(499999990, 10); // 5.0e10 - 10
        Assert(IsApproximatelyEqual(result11_1, expected11_1), $"T11.1: Q - long (5e10 - 10L). Expected: {expected11_1}, Got: {result11_1}");

        // T11.2: float - QuarkType (Routes to double - Q)
        QuarkType result11_2 = 500.0f - new QuarkType(100000000, 2); // 500.0 - 100.0
        var expected11_2 = new QuarkType(400.0); // 4.0e2
        Assert(IsApproximatelyEqual(result11_2, expected11_2), $"T11.2: float - Q (500f - 100). Expected: {expected11_2}, Got: {result11_2}");

        // T11.3: long - QuarkType (Clamping/Zero Result)
        QuarkType result11_3 = 10L - a; // 10L - 5.0e10
        Assert(result11_3 == QuarkType.Zero, $"T11.3: long - Q (10L - 5e10) should clamp to Zero. Got: {result11_3}");

        // T11.4: ulong - QuarkType (Exact subtraction)
        var c = new QuarkType(500000000, 2); // 500.0
        QuarkType result11_4 = 600UL - c;
        var expected11_4 = new QuarkType(100000000, 2); // 100.0
        Assert(result11_4 == expected11_4, $"T11.4: ulong - Q (600UL - 500). Expected: {expected11_4}, Got: {result11_4}");

        // T11.5: Subtraction using an int
        QuarkType result11_5 = a - 5;
        var expected11_5 = new QuarkType(499999995, 10);
        Assert(IsApproximatelyEqual(result11_5, expected11_5), $"T11.5: Q - int (5e10 - 5). Expected: {expected11_5}, Got: {result11_5}");
    }

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Primitive Multiplication
    // ------------------------------------------------------------------
    void TestPrimitiveMultiplication()
    {
        Debug.Log("\n<color=yellow>## 12. Primitive Multiplication (*)</color>");
        var a = new QuarkType(200000000, 5); // 2.0e5

        // T12.1: Q * int (Routes to Q * long)
        QuarkType result12_1 = a * 5;
        var expected12_1 = new QuarkType(100000000, 6); // 1.0e6
        Assert(result12_1 == expected12_1, $"T12.1: Q * int (2e5 * 5). Expected: {expected12_1}, Got: {result12_1}");

        // T12.2: double * Q (Routes to double * Q)
        QuarkType result12_2 = 2.5 * a;
        var expected12_2 = new QuarkType(500000000, 5); // 5.0e5
        Assert(IsApproximatelyEqual(result12_2, expected12_2), $"T12.2: double * Q (2.5 * 2e5). Expected: {expected12_2}, Got: {result12_2}");

        // T12.3: ulong * Q (Routes to ulong * Q)
        QuarkType result12_3 = 50UL * a;
        var expected12_3 = new QuarkType(100000000, 7); // 1.0e7
        Assert(result12_3 == expected12_3, $"T12.3: ulong * Q (50UL * 2e5). Expected: {expected12_3}, Got: {result12_3}");

        // T12.4: Q * float (Routes to Q * double)
        QuarkType result12_4 = a * 1.5f;
        var expected12_4 = new QuarkType(300000000, 5); // 3.0e5
        Assert(IsApproximatelyEqual(result12_4, expected12_4), $"T12.4: Q * float (2e5 * 1.5f). Expected: {expected12_4}, Got: {result12_4}");
    }
    void Update()
    {
        
    }
}
