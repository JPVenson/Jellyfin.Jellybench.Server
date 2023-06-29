using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Jellybench.Server.Shared
{
    public class ChartDataSeries<TValue>
    {
        public IReadOnlyList<string> Labels { get; set; }
        public IReadOnlyList<TValue> Values { get; set; }
    }
}
