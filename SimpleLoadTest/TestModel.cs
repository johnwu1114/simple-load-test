using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace SimpleLoadTest
{
    public class TestModel
    {
        [BsonId]
        public string Id { get; set; }
        public string Filed_1 { get; set; }
        public int Filed_2 { get; set; }
        public long Filed_3 { get; set; }
        public DateTime Filed_4 { get; set; }
        public DateTimeOffset Filed_5 { get; set; }
        public bool Filed_6 { get; set; }
        public decimal Filed_7 { get; set; }
        public decimal Filed_8 { get; set; }
        public decimal Filed_9 { get; set; }
        public decimal Filed_10 { get; set; }
        public List<int> Filed_11 { get; set; }
    }
}
