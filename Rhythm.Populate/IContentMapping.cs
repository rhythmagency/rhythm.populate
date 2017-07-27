﻿using System;
using System.Collections.Generic;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Rhythm.Populate
{
    public interface IContentMapping : IMapping
    {
        Func<UmbracoHelper, IEnumerable<IPublishedContent>> ContentSource { get; }
    }
}