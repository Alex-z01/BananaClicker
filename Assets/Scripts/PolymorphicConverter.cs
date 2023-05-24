using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PolymorphicConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        // If subclass of GenericUpgrade can convert
        return typeof(GenericUpgrade).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);
        object target = existingValue;

        switch((string)jObject["$type"])
        {
            case "IdleUpgrade":
                target = jObject.ToObject<IdleUpgrade>();
                break;

            case "ActiveUpgrade":
                target = jObject.ToObject<ActiveUpgrade>();
                break;

            case "StatUpgrade":
                target = jObject.ToObject<StatUpgrade>();
                break;

            case "BuffUpgrade":
                target = jObject.ToObject<BuffUpgrade>();
                break;

            default:
                UnityEngine.Debug.LogWarning("Type not found");
                break;
        }

        serializer.Populate(jObject.CreateReader(), target);
        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JObject jObject = JObject.FromObject(value);
        jObject.Add("$type", value.GetType().Name);
        jObject.WriteTo(writer);
    }
}

public class DictionaryConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Dictionary<Stats.StatType, object>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        Dictionary<Stats.StatType, object> dict = new Dictionary<Stats.StatType, object>();

        foreach (JProperty jp in jo.Properties())
        {
            Stats.StatType key = (Stats.StatType)Enum.Parse(typeof(Stats.StatType), jp.Name, true);
            object value = GetObject(jp.Value, serializer);
            dict.Add(key, value);
        }

        return dict;
    }

    private object GetObject(JToken token, JsonSerializer serializer)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                return token.ToObject<BucketNumber>(serializer);
            case JTokenType.Array:
                return token.ToObject<Single[]>(serializer);
            case JTokenType.Integer:
                return token.ToObject<int>(serializer);
            case JTokenType.Float:
                return token.ToObject<float>(serializer);
            case JTokenType.Boolean:
                return token.ToObject<bool>(serializer);
            case JTokenType.String:
                return token.ToObject<string>(serializer);
            case JTokenType.Null:
            case JTokenType.Undefined:
                return null;
            default:
                throw new InvalidOperationException($"Unsupported token type: {token.Type}");
        }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        Dictionary<Stats.StatType, object> dict = (Dictionary<Stats.StatType, object>)value;

        writer.WriteStartObject();

        foreach (KeyValuePair<Stats.StatType, object> kvp in dict)
        {
            writer.WritePropertyName(kvp.Key.ToString());

            if (kvp.Value is BucketNumber)
            {
                serializer.Serialize(writer, kvp.Value, typeof(BucketNumber));
            }
            else
            {
                serializer.Serialize(writer, kvp.Value);
            }
        }

        writer.WriteEndObject();
    }
}