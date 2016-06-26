using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace WpfApplication1
{
    [DataContract]
    public class Stroke
    {
        [DataMember]
        public int id;
        [DataMember]
        public double x;
        [DataMember]
        public double y;
        [DataMember]
        public long time;
        [DataMember]
        public int strokeNumber;
        [DataMember]
        public float pressure;
        [DataMember]
        public Boolean isWrite;
        public Stroke(double x, double y, long time, float pressure)
        {
            this.x = x;
            this.y = y;
            this.time = time;
            this.pressure = pressure;
        }
        public void SetParam(int strokeNumber, int id, Boolean isWrite)
        {
            this.strokeNumber = strokeNumber;
            this.id = id;
            this.isWrite = isWrite;
        }

        public void View()
        {
            Console.WriteLine(String.Format("X:{0},Y:{1}, time:{2}", this.x, this.y, this.time));
        }
    }
}
