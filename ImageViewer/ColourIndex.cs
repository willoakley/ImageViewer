using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageViewer
{
    internal class ColorIndex : Dictionary<Color, int>
    {
        public ColorIndex(int capacity)
            : base(capacity)
        {
        }

        public void Increment(Color colour)
        {
            if (!ContainsKey(colour))
            {
                Add(colour, 1);
                return;
            }

            this[colour]++;
        }

        public int TotalRecords
        {
            get
            {
                return this.Sum(x => x.Value);
            }
        }
    }
}
