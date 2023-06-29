using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Jellybench.Database.Entities;

public class BenchRun
{
    public int BenchRunId { get; set; }
    public string CpuName { get; set; }
    public string CpuManufacture { get; set; }
    public string NumberCores { get; set; }
    public string SystemRam { get; set; }

    public string GpuName { get; set; }
    public string GpuManufacture { get; set; }
    public string GpuRam { get; set; }

    public DiscType ConfigDirectoryDiscType { get; set; }
    public DiscType MediaDirectoryDiscType { get; set; }
    public string DataRequestKey { get; set; }

    public ICollection<DataPoint> DataPoints { get; set; } = null!;
}