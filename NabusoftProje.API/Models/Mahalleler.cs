using System;
using System.Collections.Generic;

namespace Event.Models;

public partial class Mahalleler
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string BilesenName { get; set; } = null!;

    public int KimlikNo { get; set; }

    public int IlId { get; set; }

    public int IlceId { get; set; }
}
