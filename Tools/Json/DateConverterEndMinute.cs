using Newtonsoft.Json;
using System;

namespace Tools.Json
{
    public class DateConverterEndMinute : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
        string format = "yyyy-MM-dd HH:mm";
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType!=JsonToken.String)
            {
                throw new Exception(String.Format("Unexpected token parsing ObjectId. Expected String, got {0}.",reader.TokenType));
            }
            var value = (string)reader.Value;
            return String.IsNullOrEmpty(value) ? DateTime.Now : DateTime.ParseExact(value, format, System.Globalization.CultureInfo.CurrentCulture); ;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var objId = (DateTime)value;
                writer.WriteValue(objId != null&& !objId.Equals(DateTime.MinValue) ? objId.ToString(format):"暂无");
            }
            else
            {
                throw new Exception("Expected ObjectId value.");
            }
        }
    }
}
