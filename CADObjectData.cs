using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoCAD_NET_4_8_Framework
{
    public class CADObjectData
    {
        public string Handle { get; set; }
        public string ObjectType { get; set; }
        public string Layer { get; set; }

        public double[] StartPoint { get; set; }
        public double[] EndPoint { get; set; }
        public double[] CenterPoint { get; set; }
        public double? Radius { get; set; }

    }
}
