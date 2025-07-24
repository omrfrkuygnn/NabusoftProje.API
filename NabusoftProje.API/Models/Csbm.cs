using System;
using System.Collections.Generic;

namespace Event.Models;

public partial class Csbm
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string BilesenName { get; set; } = null!;

    public int IlId { get; set; }

    public int IlceId { get; set; }

    public int MahalleId { get; set; }
}
