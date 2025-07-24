using System;
using System.Collections.Generic;

namespace Event.Models;

public partial class Iller
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public byte Plaka { get; set; }
}
