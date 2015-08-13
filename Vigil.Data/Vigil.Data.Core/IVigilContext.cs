﻿using System;
using Vigil.Data.Core.System;

namespace Vigil.Data.Core
{
    public interface IVigilContext
    {
        VigilUser AffectedBy { get; }
        DateTime Now { get; }

        int SaveChanges();
    }
}