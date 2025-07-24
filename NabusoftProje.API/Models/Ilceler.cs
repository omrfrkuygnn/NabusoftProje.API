using System;
using System.Collections.Generic;

namespace Event.Models;

public partial class Ilceler
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public short KimlikNo { get; set; }

    public byte IlId { get; set; }
}
