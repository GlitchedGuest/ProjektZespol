using System;
using UnityEngine;

public class QuarkTestNode : MonoBehaviour
{

    private int _testsRun = 0;
    private int _testsFailed = 0;

    // A small tolerance for approximate equality checks where precision loss is expected (e.g., division).
    // Note: Since Log10 is removed, this approximation is now based on direct component comparison.
    private const long MANTISSA_TOLERANCE = 1; // Allows for rounding error of 1 in the least significant digit (1e-8)

    // Helper to check if two QuarkTypes are close (for floating-point results)
    private bool IsApproximatelyEqual(QuarkType a, QuarkType b, long mantissaTolerance = MANTISSA_TOLERANCE)
    {
        // Must have the same sign and similar exponent
        if ((a.Mantissa < 0) != (b.Mantissa < 0)) return false;
        if (a.Exponent != b.Exponent) return false;

        // Check for exact zero equality
        if (a.Mantissa == 0 || b.Mantissa == 0) return a.Mantissa == b.Mantissa;

        // Compare magnitudes within tolerance
        return Math.Abs(a.Mantissa - b.Mantissa) <= mantissaTolerance;
    }

    void Start()
    {
        Debug.Log("<color=white>--- QuarkType Test Suite ---</color>");

        TestNormalizationAndConstructors();
        TestPrimitiveConversions();
        TestComparisonOperators();
        TestAddition();
        TestPrimitiveAddition();
        TestSubtraction();
        TestPrimitiveSubtraction();
        TestMultiplication();
        TestPrimitiveMultiplication();
        TestMultiplicationEXT();

        // --- NEW CRITICAL TEST SECTIONS ---
        TestDivision();
        TestSignedAndRangeArithmetic();
        TestPrecisionLossAndEdgeCases(); // Moved to the end
        // ----------------------------------

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

    // --- EXISTING TEST SECTIONS (UPDATED FOR SIGNED/LONG TYPES) ---

    void TestNormalizationAndConstructors()
    {
        Debug.Log("\n<color=yellow>## 1. Normalization & Constructors</color>");

        // T1.1: Basic Normalization (10.0e0 -> 1.0e1)
        var t1_1 = new QuarkType(1000000000L, 0L);
        Assert(t1_1.Mantissa == 100000000L && t1_1.Exponent == 1L, $"T1.1: Mantissa Overflow (10.0e0 -> 1.0e1). Result: {t1_1}");

        // T1.2: Negative long Constructor (-123456789L) -> -1.23456789e8
        var t1_2 = new QuarkType(-123456789L);
        // Note: Raw 123456789 is 1.23456789e8. Mantissa is 1.23... * 10^8.
        Assert(t1_2.Mantissa == -123456789L && t1_2.Exponent == 8L, $"T1.2: long -123456789 -> {t1_2}");

        // T1.3: double Constructor (Small Fraction) -> 1.234e-5
        var t1_3 = new QuarkType(0.00001234);
        var expected1_3 = new QuarkType(123400000L, -5L);
        Assert(t1_3.Mantissa == expected1_3.Mantissa && t1_3.Exponent == expected1_3.Exponent,
               $"T1.3: Double Constructor (0.00001234 -> 1.234e-5). Result: {t1_3}");

        // T1.4: Constructor with negative exponent, small mantissa (5e-2)
        var t1_4 = new QuarkType(5L, -2L); // Raw 0.05 * 10^-2. Normalize to 5.0e-10.
        var expected1_4 = new QuarkType(500000000L, -10L);
        Assert(t1_4.Exponent == expected1_4.Exponent && t1_4.Mantissa == expected1_4.Mantissa, $"T1.4: Raw 5e-2 -> {t1_4}. Exponent should be -10.");

        // T1.5: Constructor with negative mantissa, large exponent
        var t1_5 = new QuarkType(-500000000L, 10L);
        Assert(t1_5.Mantissa == -500000000L && t1_5.Exponent == 10L, $"T1.5: Negative mantissa. Result: {t1_5}");
    }

    void TestComparisonOperators()
    {
        Debug.Log("\n<color=yellow>## 2. Comparison Operators</color>");

        var a = new QuarkType(100000000L, 100L); // 1.0e100
        var b = new QuarkType(100000001L, 100L); // 1.00000001e100
        var c = new QuarkType(-100000000L, 100L); // -1.0e100
        var d = new QuarkType(999999999L, 99L); // 9.99999999e99

        Assert(b > a, "T2.1: Positive Mantissa Difference (b > a)");
        Assert(c < a, "T2.2: Sign Difference (c < a)");
        Assert(a > c, "T2.3: Sign Difference (a > c)");
        Assert(c < QuarkType.Zero, "T2.4: Negative vs Zero");
        Assert(d < a, "T2.5: Exponent Difference (d < a)");
        Assert(a == new QuarkType(100000000L, 100L), "T2.6: Equality");
        Assert(c.CompareTo(new QuarkType(-100000001L, 100L)) == 1, "T2.7: Negative Comparison (Larger magnitude is smaller value)");
    }

    void TestAddition()
    {
        Debug.Log("\n<color=yellow>## 3. Addition</color>");

        // T3.1: A + (-A) = 0
        var a = new QuarkType(500000000L, 10L);
        var b = -a;
        Assert(a + b == QuarkType.Zero, "T3.1: A + (-A) = 0");

        // T3.2: Large Positive + Small Negative (Approx same magnitude)
        var c = new QuarkType(900000000L, 10L); // 9.0e10
        var d = new QuarkType(-890000000L, 10L); // -8.9e10
        var expected3_2 = new QuarkType(100000000L, 9L); // Raw 0.1e10 -> 1.0e9
        Assert(IsApproximatelyEqual(c + d, expected3_2), $"T3.2: c + d = 1.0e9. Got: {c + d}");

        // T3.3: Negative + Negative (Mantissa Overflow)
        var e = new QuarkType(-900000000L, 10L);
        var f = new QuarkType(-200000000L, 10L);
        var expected3_3 = new QuarkType(-110000000L, 11L); // Normalized -1.1e11
        Assert(IsApproximatelyEqual(e + f, expected3_3), $"T3.3: Negative Overflow. Expected: {expected3_3}, Got: {e + f}");

        // T3.4: Large Positive + Small Positive (Exponent Alignment)
        var g = new QuarkType(100000000L, 100L);
        var h = new QuarkType(100000000L, 90L);
        Assert(g + h == g, "T3.4: Addition with insignificant number (1e100 + 1e90 = 1e100)");
    }

    void TestSubtraction()
    {
        Debug.Log("\n<color=yellow>## 4. Subtraction</color>");

        // T4.1: Positive - Negative = Addition
        var a = new QuarkType(500000000L, 5L); // 5.0e5
        var b = new QuarkType(-200000000L, 5L); // -2.0e5
        var expected4_1 = new QuarkType(700000000L, 5L); // 7.0e5
        Assert(a - b == expected4_1, $"T4.1: Positive - Negative = Addition (5e5 - (-2e5) = 7e5). Got: {a - b}");

        // T4.2: Negative - Positive = Larger Negative
        var c = new QuarkType(-500000000L, 5L); // -5.0e5
        var d = new QuarkType(200000000L, 5L); // 2.0e5
        var expected4_2 = new QuarkType(-700000000L, 5L); // -7.0e5
        Assert(c - d == expected4_2, $"T4.2: Negative - Positive = Larger Negative (-5e5 - 2e5 = -7e5). Got: {c - d}");

        // T4.3: Result Renormalization (1.0e5 - 9.9e4 = 0.1e5 -> 1.0e4)
        var e = new QuarkType(100000000L, 5L);
        var f = new QuarkType(990000000L, 4L);
        var expected4_3 = new QuarkType(100000000L, 3L); // 1.0e3
        Assert(IsApproximatelyEqual(e - f, expected4_3), $"T4.3: Result Renormalization (1.0e5 - 9.9e4). Expected: {expected4_3}, Got: {e - f}");
    }

    void TestMultiplication()
    {
        Debug.Log("\n<color=yellow>## 5. Multiplication</color>");

        // T5.1: Negative * Positive = Negative
        var a = new QuarkType(-200000000L, 5L);
        var b = new QuarkType(300000000L, 4L);
        var expected5_1 = new QuarkType(-600000000L, 9L); // -6.0e9
        Assert(a * b == expected5_1, $"T5.1: Negative * Positive = Negative (-2e5 * 3e4 = -6e9)");

        // T5.2: Negative * Negative = Positive
        var c = new QuarkType(-500000000L, 10L);
        var expected5_2 = new QuarkType(250000000L, 21L); // Normalized 2.5e21
        Assert(IsApproximatelyEqual(c * c, expected5_2), $"T5.2: Negative * Negative = Positive. Expected: {expected5_2}, Got: {c * c}");

        // T5.3: Multiplication with small fraction (2e5 * 1e-10 = 2e-5)
        var d = new QuarkType(100000000L, -10L);
        var e = new QuarkType(200000000L, 5L);
        var expected5_3 = new QuarkType(200000000L, -5L);
        Assert(e * d == expected5_3, $"T5.3: 2e5 * 1e-10 = 2e-5. Got: {e * d}");
    }

    // ------------------------------------------------------------------
    // REMOVED: TestLogarithmFunctions (Section 6)
    // REMOVED: TestPowerFunction (Section 7)
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

        // T9.3: Negative long (Implicit conversion)
        QuarkType qNegLong = -9876543210L;
        var expected9_3 = new QuarkType(-987654321L, 9L);
        Assert(qNegLong.Mantissa == expected9_3.Mantissa && qNegLong.Exponent == expected9_3.Exponent, $"T9.3: Negative long to QuarkType. Result: {qNegLong}");
    }

    void TestPrimitiveAddition()
    {
        Debug.Log("\n<color=yellow>## 10. Primitive Addition (+)</color>");
        var a = new QuarkType(500000000L, 10L); // 5.0e10

        // T10.1: QuarkType + int (Routes to + long)
        QuarkType result10_1 = a + 10;
        var expected10_1 = new QuarkType(500000000L, 10L); // 5.0e10 + 10
        Assert(IsApproximatelyEqual(result10_1, expected10_1), $"T10.1: Q + int (5e10 + 10). Expected: {expected10_1}, Got: {result10_1}");

        // T10.2: float + QuarkType (Routes to double + Q)
        QuarkType result10_2 = 10.5f + a;
        var expected10_2 = new QuarkType(500000001L, 10L); // 5.0e10 + 10.5 (rounded)
        Assert(IsApproximatelyEqual(result10_2, expected10_2), $"T10.2: float + Q (10.5f + 5e10). Expected: {expected10_2}, Got: {result10_2}");
    }

    void TestPrimitiveSubtraction()
    {
        Debug.Log("\n<color=yellow>## 11. Primitive Subtraction (-)</color>");
        var a = new QuarkType(500000000L, 8L); // 5.0e3

        // T11.1: QuarkType - long (Routes to Q - long)
        QuarkType result11_1 = a - 10L;
        var expected11_1 = new QuarkType(499999990L, 8L); // 5.0e10 - 10
        Assert(IsApproximatelyEqual(result11_1, expected11_1), $"T11.1: Q - long (5e10 - 10L). Expected: {expected11_1}, Got: {result11_1}");

        // T11.2: float - QuarkType (Routes to double - Q)
        QuarkType result11_2 = 500.0f - new QuarkType(100000000L, 2L); // 500.0 - 100.0
        var expected11_2 = new QuarkType(400.0); // 4.0e2
        Assert(IsApproximatelyEqual(result11_2, expected11_2), $"T11.2: float - Q (500f - 100). Expected: {expected11_2}, Got: {result11_2}");

        // T11.3: long - QuarkType (Resulting in negative)
        var c = new QuarkType(100000000L, 2L); // 100.0
        QuarkType result11_3 = 10L - c; // 10 - 100 = -90
        var expected11_3 = new QuarkType(-900000000L, 1L);
        Assert(result11_3 == expected11_3, $"T11.3: long - Q (10L - 100). Expected: {expected11_3}, Got: {result11_3}");
    }

    void TestPrimitiveMultiplication()
    {
        Debug.Log("\n<color=yellow>## 12. Primitive Multiplication (*)</color>");
        var a = new QuarkType(200000000L, 5L); // 2.0e5

        // T12.1: Q * int (Routes to Q * long)
        QuarkType result12_1 = a * 5;
        var expected12_1 = new QuarkType(100000000L, 6L); // 1.0e6
        Assert(result12_1 == expected12_1, $"T12.1: Q * int (2e5 * 5). Expected: {expected12_1}, Got: {result12_1}");

        // T12.2: double * Q (Routes to double * Q)
        QuarkType result12_2 = 2.5 * a;
        var expected12_2 = new QuarkType(500000000L, 5L); // 5.0e5
        Assert(IsApproximatelyEqual(result12_2, expected12_2), $"T12.2: double * Q (2.5 * 2e5). Expected: {expected12_2}, Got: {result12_2}");
    }

    void TestMultiplicationEXT()
    {
        Debug.Log("\n<color=yellow>## 13. Multiplication Extended</color>");

        // T13.1: Basic Multiplication (10 * 2 = 20)
        var t13_1a = new QuarkType(10L);
        var t13_1 = t13_1a * 2.0;
        Assert(t13_1.Mantissa == 200000000L && t13_1.Exponent == 1L,
            $"T13.1: 10 * 2.0 = 20.0. Result: {t13_1}");

        // T13.2: Multiply by 0.5 (10 * 0.5 = 5)
        var t13_2a = new QuarkType(10L);
        var t13_2 = t13_2a * 0.5;
        Assert(t13_2.Mantissa == 500000000L && t13_2.Exponent == 0L,
            $"T13.2: 10 * 0.5 = 5.0. Result: {t13_2}");
    }

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Division Operator
    // ------------------------------------------------------------------
    void TestDivision()
    {
        Debug.Log("\n<color=yellow>## 14. Division Operator</color>");

        // T14.1: Simple Division (6e5 / 3e2 = 2e3)
        var a = new QuarkType(600000000L, 5L);
        var b = new QuarkType(300000000L, 2L);
        var expected14_1 = new QuarkType(200000000L, 3L);
        Assert(a / b == expected14_1, $"T14.1: Simple Division (6e5 / 3e2 = 2e3). Got: {a / b}");

        // T14.2: Division resulting in Mantissa Renormalization (9e2 / 2e2 = 4.5e0)
        var c = new QuarkType(900000000L, 2L);
        var d = new QuarkType(200000000L, 2L);
        var expected14_2 = new QuarkType(450000000L, 0L);
        Assert(c / d == expected14_2, $"T14.2: Mantissa Renormalization (9e2 / 2e2 = 4.5e0). Got: {c / d}");

        // T14.3: Division resulting in Exponent Adjustment (1e2 / 5e2 = 0.2e0 -> 2.0e-1)
        var e = new QuarkType(100000000L, 2L);
        var f = new QuarkType(500000000L, 2L);
        var expected14_3 = new QuarkType(200000000L, -1L);
        Assert(e / f == expected14_3, $"T14.3: Exponent Adjustment (1e2 / 5e2 = 2e-1). Got: {e / f}");

        // T14.4: Division with large range: 1e100 / 1e-100 = 1e200
        var large = new QuarkType(100000000L, 100L);
        var small = new QuarkType(100000000L, -100L);
        var expected14_4 = new QuarkType(100000000L, 200L);
        Assert(large / small == expected14_4, $"T14.4: Range Check (1e100 / 1e-100 = 1e200). Got: {large / small}");

        // T14.5: Negative Division: -6e5 / 3e2 = -2e3
        var expected14_5 = new QuarkType(-200000000L, 3L);
        Assert(new QuarkType(-600000000L, 5L) / b == expected14_5, $"T14.5: Negative Result (-6e5 / 3e2 = -2e3). Got: {new QuarkType(-600000000L, 5L) / b}");

        // T14.6: Division by Zero (must return 'Infinity' marker)
        QuarkType divByZero = a / QuarkType.Zero;
        Assert(divByZero.Exponent == long.MaxValue && divByZero.Mantissa == long.MaxValue, "T14.6: Division by Zero returns MaxValue/Infinity marker.");
    }

    // ------------------------------------------------------------------
    // NEW TEST SECTION: Signed and Range Arithmetic
    // ------------------------------------------------------------------
    void TestSignedAndRangeArithmetic()
    {
        Debug.Log("\n<color=yellow>## 15. Signed and Range Arithmetic</color>");

        // T15.1: Subtraction resulting in a negative number
        var a = new QuarkType(100000000L, 5L); // 1.0e5
        var b = new QuarkType(200000000L, 5L); // 2.0e5
        var expected15_1 = new QuarkType(-100000000L, 5L); // -1.0e5
        Assert(a - b == expected15_1, $"T15.1: Subtraction to Negative (1e5 - 2e5 = -1e5). Got: {a - b}");

        // T15.2: Division of large magnitude positive by small magnitude negative, resulting in a large negative exponent
        var largePos = new QuarkType(100000000L, 100L); // 1e100
        var smallNeg = new QuarkType(-500000000L, 90L); // -5e90
        var expected15_2 = new QuarkType(-200000000L, 9L); // -2.0e9
        Assert(largePos / smallNeg == expected15_2, $"T15.2: Large Pos / Small Neg = Negative. Got: {largePos / smallNeg}");

        // T15.3: Check Unary Minus (flip sign)
        var c = new QuarkType(-500000000L, -50L);
        var expected15_3 = new QuarkType(500000000L, -50L);
        Assert(-c == expected15_3, $"T15.3: Unary Minus (-c = expected). Got: {-c}");

        // T15.4: Addition with significant precision loss but correct sign (1.0e10 - 1.0e-10)
        var d = new QuarkType(100000000L, 10L); // 1e10
        var e = new QuarkType(100000000L, -10L); // 1e-10
        // Result should still be 1e10 due to precision cutoff
        Assert(d - e == d, $"T15.4: Subtraction with large exponent difference (1e10 - 1e-10 = 1e10). Got: {d - e}");
    }

    // ------------------------------------------------------------------
    // END: NEW TEST SECTION
    // ------------------------------------------------------------------

    void TestPrecisionLossAndEdgeCases()
    {
        Debug.Log("\n<color=yellow>## 16. Precision Loss and Edge Cases</color>");

        // T16.1: Max Exponent Check (Uses long.MaxValue now)
        var maxExp = new QuarkType(999999999L, long.MaxValue - 100L);
        var result16_1 = maxExp * new QuarkType(200000000L, 100L); // 2.0
        Assert(result16_1.Exponent == long.MaxValue, "T16.1: Multiplication resulting in Exponent saturation (long.MaxValue).");

        // T16.2: Underflow to Zero (Using a very small exponent)
        var smallExp = new QuarkType(100000000L, long.MinValue + 10L);
        var result16_2 = smallExp * new QuarkType(100000000L, -20L); // Should result in 0
        Assert(result16_2.Mantissa == 0 && result16_2.Exponent == 0, "T16.2: Underflow resulting in Zero.");

        // T16.3: Large magnitude division resulting in Mantissa precision loss
        var x = new QuarkType(999999999L, 0L); // ~10
        var y = new QuarkType(300000000L, 0L); // 3
        var expected16_3 = new QuarkType(333333333L, 0L); // Expected 3.33333333 * 10^0
        // Actual result is 3.33333333... but we are clamped to 9 digits.
        Assert(IsApproximatelyEqual(x / y, expected16_3, 2), $"T16.3: Division precision check (10/3). Expected: {expected16_3}, Got: {x / y}");
    }

    void Update() { }
}