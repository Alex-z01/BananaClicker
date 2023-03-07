using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Diagnostics;
using UnityEngine;

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
