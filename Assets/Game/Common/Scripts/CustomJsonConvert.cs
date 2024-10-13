using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CustomJsonConvert : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var command = (GameStateSaveBaseModel)value;
        var typeName = command.GetType().AssemblyQualifiedName;
        var jo = JObject.FromObject(command, serializer);
        jo.AddFirst(new JProperty("$type", typeName));
        jo.WriteTo(writer);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        var typeName = (string)jo["$type"];
        var type = Type.GetType(typeName);

        if (type == null)
        {
            throw new InvalidOperationException("Could not find type: " + typeName);
        }

        var temp = (GameStateSaveBaseModel)serializer.Deserialize(jo.CreateReader(), type);
        return temp;
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(GameStateSaveBaseModel).IsAssignableFrom(objectType);
    }
}
