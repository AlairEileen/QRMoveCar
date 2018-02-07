using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Models
{
    public class CarNumberModel
    {
        public string Name { get; set; }
        public string Province { get; set; }
        public List<CarNumberCodeModel> Codes { get; set; }
    }

    public class CarNumberCodeModel
    {
        public string Code { get; set; }
        public string City { get; set; }
    }
}
