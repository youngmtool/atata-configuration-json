﻿using System;
using System.Linq;

namespace Atata
{
    public abstract class JsonConfig<TConfig> : JsonSection
        where TConfig : JsonConfig<TConfig>
    {
        [ThreadStatic]
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static TConfig current;
#pragma warning restore S2743 // Static fields should not be used in generic types

        public static TConfig Current
        {
            get { return current; }
            set { current = value; }
        }

        public WebDriverJsonSection[] Drivers { get; set; }

        public WebDriverJsonSection Driver
        {
            get { return Drivers?.SingleOrDefault(); }
            set { Drivers = new[] { value }; }
        }

        public LogConsumerJsonSection[] LogConsumers { get; set; }

        public bool UseNUnitTestName { get; set; }

        public bool LogNUnitError { get; set; }
    }
}
