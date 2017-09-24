using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Runtime.Serialization;
using Java.Interop;

namespace CaffeineTracker {
    public class Drink {
        public string Name { get; set; }
        public double Size { get; set; }
        public double Caffeine { get; set; }

        internal static string[] Serialize(Drink d) => new[] { d.Name, d.Size.ToString(), d.Caffeine.ToString() };
        internal static Drink Deserialize(string[] s) => new Drink { Name = s[0], Size = double.Parse(s[1]), Caffeine = double.Parse(s[2]) };
    }

    public class DetailedDrink : Drink {
        public DateTime TimeOfConsumption { get; set; }

        public DetailedDrink() => TimeOfConsumption = DateTime.Now;

        public static DetailedDrink ToDetailedDrink(Drink drink) =>
            new DetailedDrink { Name = drink.Name, Caffeine = drink.Caffeine, Size = drink.Size };

        internal static string[] Serialize(DetailedDrink d) =>
            new[] { d.Name, d.Size.ToString(), d.Caffeine.ToString(), d.TimeOfConsumption.Ticks.ToString() };

        internal new static DetailedDrink Deserialize(string[] s) => new DetailedDrink {
            Name = s[0],
            Size = double.Parse(s[1]),
            Caffeine = double.Parse(s[2]),
            TimeOfConsumption = new DateTime(long.Parse(s[3]))
        };

        public double ComputeCaffeine => Caffeine * Math.Pow(0.5, (DateTime.Now - TimeOfConsumption).TotalHours / 6.0);

        public double ComputeTime {
            get {
                double t = 0.0, c = ComputeCaffeine;
                while (c > 0.05 * 400) {
                    t += 6;
                    c /= 2;
                }
                return t;
            }
        }
    }
}
