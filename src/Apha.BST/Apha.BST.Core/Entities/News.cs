﻿using System;
using System.Collections.Generic;

namespace Apha.BST.Core.Entities;

public partial class News
{
    public string? Title { get; set; }

    public string? NewsContent { get; set; }

    public DateTime DatePublished { get; set; }

    public string? Author { get; set; }

    public virtual User? AuthorNavigation { get; set; }
}
