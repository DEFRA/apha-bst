using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? UserName { get; set; }

    public string? UserLoc { get; set; }

    public byte? UserLevel { get; set; }

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
