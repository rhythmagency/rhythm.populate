﻿using System;
using System.Collections.Generic;

namespace Rhythm.Populate
{
    public class MappingOptions
    {
        public IDictionary<Type, IList<string>> IncludedProperties { get; private set; }

        public MappingOptions()
        {
            IncludedProperties = new Dictionary<Type, IList<string>>();
        }
    }
}