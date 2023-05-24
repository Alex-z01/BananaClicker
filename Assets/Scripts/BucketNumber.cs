using System;
using UnityEditor;
using System.Runtime.Serialization;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

[Serializable]
public class BucketNumber : IComparable<BucketNumber>, ISerializable
{
    [SerializeField] private float coefficient;
    [SerializeField] private int magnitude; // 1 => thousands, 2 => millions, etc

    public static BucketNumber Zero
    {
        get
        {
            return new BucketNumber(0, 0);
        }
    }

    public float GetCoefficient() { return coefficient; }
    public int GetMagnitude() { return magnitude; }
    public float GetValue()
    {
        if(magnitude < 1)
        {
            return coefficient;
        }
        return coefficient * (float)Math.Pow(10, magnitude * 3);
    }

    public BucketNumber()
    {
        this.coefficient = 0;
        this.magnitude = 0;
    }

    public BucketNumber(float coefficient, int magnitude)
    {
        if (coefficient == 0 && magnitude != 0)
        {
            magnitude = 0;
            return;
        }

        if (coefficient >= 1000)
        {
            while (coefficient >= 1000)
            {
                coefficient /= 1000f;
                magnitude++;
            }
        } 

        if (coefficient < 1 && coefficient > 0)
        {
            coefficient *= 1000f;
            magnitude--;
        }

        if(coefficient < 0)
        {
            throw new ArgumentOutOfRangeException("Coefficient cannot be negative.");
        }

        if(magnitude < 2)
        {
            coefficient = (float)Math.Round(coefficient, 6);
        }
        this.coefficient = (float)Math.Round(coefficient, 6);
        this.magnitude = magnitude;
    }

    public BucketNumber Clone()
    {
        BucketNumber clone = new BucketNumber();
        clone.coefficient = this.coefficient;
        clone.magnitude = this.magnitude;
        return clone;
    }

    public static BucketNumber Random(BucketNumber minInclusive, BucketNumber maxExclusive)
    {
        float coef = UnityEngine.Random.Range(minInclusive.coefficient, maxExclusive.coefficient);
        int mag = UnityEngine.Random.Range(minInclusive.magnitude, maxExclusive.magnitude);

        return new BucketNumber(coef, mag);
    }

    public BucketNumber Round(int precision)
    {
        coefficient = (float)Math.Round(coefficient, precision);

        return this;
    }

    
    // Deserialization constructor
    protected BucketNumber(SerializationInfo info, StreamingContext context) 
    {
        coefficient = info.GetSingle("coefficient");
        magnitude = info.GetInt32("magnitude");
    }

    // Serialization method
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("coefficient", coefficient);
        info.AddValue("magnitude", magnitude);
    }
    
    public int CompareTo(BucketNumber other)
    {
        if (magnitude == other.magnitude)
        {
            return coefficient.CompareTo(other.coefficient);
        }

        return magnitude.CompareTo(other.magnitude);
    }

    public override string ToString()
    {
        if(magnitude == 1)
        {
            return $"{Mathf.Floor(coefficient * Mathf.Pow(10, magnitude * 3)).ToString("##,##0")}";
        }
        if(magnitude == 0) 
        {
            return $"{Math.Round(coefficient, 2)}";
        }
        return $"{coefficient.ToString("F3")}{Manager.Instance.dataManager.magnitudeNames[magnitude]}";
    }

    // ADDITION

    public static BucketNumber operator +(BucketNumber a, BucketNumber b)
    {
        float coefficientA = a.coefficient;
        int magnitudeA = a.magnitude;

        float coefficientB = b.coefficient;
        int magnitudeB = b.magnitude;

        float resultCoefficient = 0;
        int resultMagnitude = Math.Max(magnitudeA, magnitudeB);

        while (magnitudeA != magnitudeB)
        {
            if(magnitudeA > magnitudeB)
            {
                magnitudeB++;
                coefficientB /= 1000f;
            }

            if(magnitudeB > magnitudeA)
            {
                magnitudeA++;
                coefficientA /= 1000f;
            }
        }

        if (magnitudeA == magnitudeB)
        {
            resultCoefficient = coefficientA + coefficientB;
        }

        if(resultCoefficient > 1000)
        {
            resultCoefficient = resultCoefficient / 1000f;
            resultMagnitude++;
        }

        return new BucketNumber(resultCoefficient, resultMagnitude);
    }

    public static BucketNumber operator +(BucketNumber a, int value)
    {
        float coefficientA = a.coefficient;
        int magnitudeA = a.magnitude;

        BucketNumber b = new BucketNumber(value, 0);
        float coefficientB = b.coefficient;
        int magnitudeB = b.magnitude;

        float resultCoefficient = 0;
        int resultMagnitude = Math.Max(magnitudeA, magnitudeB);

        while (magnitudeA != magnitudeB)
        {
            if (magnitudeA > magnitudeB)
            {
                magnitudeB++;
                coefficientB /= 1000f;
            }

            if (magnitudeB > magnitudeA)
            {
                magnitudeA++;
                coefficientA /= 1000f;
            }
        }

        if (magnitudeA == magnitudeB)
        {
            resultCoefficient = coefficientA + coefficientB;
        }

        if (resultCoefficient > 1000)
        {
            resultCoefficient = resultCoefficient / 1000f;
            resultMagnitude++;
        }

        return new BucketNumber(resultCoefficient, resultMagnitude);
    }

    public static BucketNumber operator +(BucketNumber a, float value)
    {
        float coefficientA = a.coefficient;
        int magnitudeA = a.magnitude;

        BucketNumber b = new BucketNumber(value, 0);
        float coefficientB = b.coefficient;
        int magnitudeB = b.magnitude;

        float resultCoefficient = 0;
        int resultMagnitude = Math.Max(magnitudeA, magnitudeB);

        while (magnitudeA != magnitudeB)
        {
            if (magnitudeA > magnitudeB)
            {
                magnitudeB++;
                coefficientB /= 1000f;
            }

            if (magnitudeB > magnitudeA)
            {
                magnitudeA++;
                coefficientA /= 1000f;
            }
        }

        if (magnitudeA == magnitudeB)
        {
            resultCoefficient = coefficientA + coefficientB;
        }

        if (resultCoefficient > 1000)
        {
            resultCoefficient = resultCoefficient / 1000f;
            resultMagnitude++;
        }

        return new BucketNumber(resultCoefficient, resultMagnitude);
    }

    public static BucketNumber operator +(BucketNumber a, double value)
    {
        float coefficientA = a.coefficient;
        int magnitudeA = a.magnitude;

        BucketNumber b = new BucketNumber((float)value, 0);
        float coefficientB = b.coefficient;
        int magnitudeB = b.magnitude;

        float resultCoefficient = 0;
        int resultMagnitude = Math.Max(magnitudeA, magnitudeB);

        while (magnitudeA != magnitudeB)
        {
            if (magnitudeA > magnitudeB)
            {
                magnitudeB++;
                coefficientB /= 1000f;
            }

            if (magnitudeB > magnitudeA)
            {
                magnitudeA++;
                coefficientA /= 1000f;
            }
        }

        if (magnitudeA == magnitudeB)
        {
            resultCoefficient = coefficientA + coefficientB;
        }

        if (resultCoefficient > 1000)
        {
            resultCoefficient = resultCoefficient / 1000f;
            resultMagnitude++;
        }

        return new BucketNumber(resultCoefficient, resultMagnitude);
    }

    // SUBTRACTION

    public static BucketNumber operator -(BucketNumber a, BucketNumber b)
    {
        float coefficientA = a.coefficient;
        int magnitudeA = a.magnitude;

        float coefficientB = b.coefficient;
        int magnitudeB = b.magnitude;

        float resultCoefficient = 0;
        int resultMagnitude = Math.Min(magnitudeA, magnitudeB);

        while (magnitudeA != magnitudeB)
        {
            if (magnitudeA > magnitudeB)
            {
                magnitudeA--;
                coefficientA *= 1000f;
            }

            if (magnitudeB > magnitudeA)
            {
                magnitudeB--;
                coefficientB *= 1000f;
            }
        }

        if (magnitudeA == magnitudeB)
        {
            resultCoefficient = coefficientA - coefficientB;
            resultMagnitude = magnitudeA;
        }

        return new BucketNumber(resultCoefficient, resultMagnitude);
    }

    public static BucketNumber operator -(BucketNumber a, int value)
    {
        a.coefficient -= value;
        return a;
    }

    // MULTIPLICATION

    public static BucketNumber operator *(BucketNumber a, BucketNumber b)
    {
        return new BucketNumber(a.coefficient * b.coefficient, a.magnitude + b.magnitude);
    }

    public static BucketNumber operator *(BucketNumber a, int value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return new BucketNumber(a.coefficient * b.coefficient, a.magnitude + b.magnitude);
    }

    public static BucketNumber operator *(BucketNumber a, float value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return new BucketNumber(a.coefficient * b.coefficient, a.magnitude + b.magnitude);
    }

    public static BucketNumber operator *(BucketNumber a, double value)
    {
        BucketNumber b = new BucketNumber((float)value, 0);
        return new BucketNumber(a.coefficient * b.coefficient, a.magnitude + b.magnitude);
    }

    // DIVISION

    public static BucketNumber operator /(BucketNumber a, BucketNumber b)
    {
        return new BucketNumber(a.coefficient / b.coefficient, a.magnitude - b.magnitude);
    }

    public static BucketNumber operator /(BucketNumber a, float value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return new BucketNumber(a.coefficient / b.coefficient, a.magnitude - b.magnitude);
    }


    // GREATER THAN

    public static bool operator >(BucketNumber a, BucketNumber b)
    {
        return a.CompareTo(b) > 0;
    }

    public static bool operator >(BucketNumber a, int value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return a.CompareTo(b) > 0;
    }

    public static bool operator >(BucketNumber a, float value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return a.CompareTo(b) > 0;
    }

    public static bool operator >(BucketNumber a, double value)
    {
        BucketNumber b = new BucketNumber((float)value, 0);
        return a.CompareTo(b) > 0;
    }

    // LESS THAN

    public static bool operator <(BucketNumber a, BucketNumber b)
    {
        return a.CompareTo(b) < 0;
    }

    public static bool operator <(BucketNumber a, int value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return a.CompareTo(b) < 0;
    }

    public static bool operator <(BucketNumber a, float value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return a.CompareTo(b) < 0;
    }

    public static bool operator <(BucketNumber a, double value)
    {
        BucketNumber b = new BucketNumber((float)value, 0);
        return a.CompareTo(b) < 0;
    }

    // GREATER THAN OR EQUAL

    public static bool operator >=(BucketNumber a, BucketNumber b)
    {
        return a.CompareTo(b) >= 0;
    }

    public static bool operator <=(BucketNumber a, BucketNumber b)
    {
        return a.CompareTo(b) <= 0;
    }

    public static bool operator ==(BucketNumber a, BucketNumber b)
    {
        return a.coefficient == b.coefficient && a.magnitude == b.magnitude;
    }

    public static bool operator ==(BucketNumber a, int value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return a.coefficient == b.coefficient && a.magnitude == b.magnitude;
    }

    public static bool operator !=(BucketNumber a, BucketNumber b)
    {
        return !(a == b);
    }

    public static bool operator !=(BucketNumber a, int value)
    {
        BucketNumber b = new BucketNumber(value, 0);
        return !(a == b);
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(BucketNumber))]
public class BucketNumberDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(property == null)
        {
            Debug.LogError("SerializedProperty is null");
            return;
        }

        // Get the values of the class from the SerializedProperty
        SerializedProperty coefficient = property.FindPropertyRelative("coefficient");
        SerializedProperty magnitude = property.FindPropertyRelative("magnitude");

        position = EditorGUI.PrefixLabel(position, label);

        // Create a new Rect for the properties
        Rect coefficientRect = new Rect(position.x, position.y, 50, position.height);
        Rect magnitudeRect = new Rect(position.x + 55, position.y, 50, position.height);

        // Draw out the fields in the custom property drawer
        EditorGUI.PropertyField(coefficientRect, coefficient, GUIContent.none);
        EditorGUI.PropertyField(magnitudeRect, magnitude, GUIContent.none);
    }
}

#endif

public class BucketNumberConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Dictionary<object, BucketNumber>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        var result = new Dictionary<object, BucketNumber>();
        foreach (var property in jsonObject.Properties())
        {
            var value = property.Value.ToObject<BucketNumber>(serializer);
            result.Add(property.Name, value);
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var dictionary = (Dictionary<object, BucketNumber>)value;

        writer.WriteStartObject();
        foreach (var pair in dictionary)
        {
            writer.WritePropertyName(pair.Key.ToString());
            serializer.Serialize(writer, pair.Value);
        }
        writer.WriteEndObject();
    }
}
