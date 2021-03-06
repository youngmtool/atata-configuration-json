﻿using System;

namespace Atata.Configuration.Json
{
    public static class JsonConfigMapper
    {
        public static AtataContextBuilder Map<TConfig>(TConfig config, AtataContextBuilder builder)
            where TConfig : JsonConfig<TConfig>
        {
            if (config.BaseUrl != null)
                builder.UseBaseUrl(config.BaseUrl);

            if (config.RetryTimeout != null)
                builder.UseRetryTimeout(TimeSpan.FromSeconds(config.RetryTimeout.Value));

            if (config.RetryInterval != null)
                builder.UseRetryInterval(TimeSpan.FromSeconds(config.RetryInterval.Value));

            if (config.AssertionExceptionType != null)
                builder.UseAssertionExceptionType(Type.GetType(config.AssertionExceptionType, true));

            if (config.UseNUnitTestName)
                builder.UseNUnitTestName();

            if (config.LogNUnitError)
                builder.LogNUnitError();

            if (config.TakeScreenshotOnNUnitError)
            {
                if (config.TakeScreenshotOnNUnitErrorTitle != null)
                    builder.TakeScreenshotOnNUnitError(config.TakeScreenshotOnNUnitErrorTitle);
                else
                    builder.TakeScreenshotOnNUnitError();
            }

            if (config.LogConsumers != null)
            {
                foreach (var item in config.LogConsumers)
                    MapLogConsumer(item, builder);
            }

            if (config.ScreenshotConsumers != null)
            {
                foreach (var item in config.ScreenshotConsumers)
                    MapScreenshotConsumer(item, builder);
            }

            if (config.Drivers != null)
            {
                foreach (var item in config.Drivers)
                    MapDriver(item, builder);
            }

            return builder;
        }

        private static void MapLogConsumer(LogConsumerJsonSection section, AtataContextBuilder builder)
        {
            var consumerBuilder = builder.AddLogConsumer(section.Type);

            if (section.MinLevel != null)
                consumerBuilder.WithMinLevel(section.MinLevel.Value);

            if (section.SectionFinish == false)
                consumerBuilder.WithoutSectionFinish();

            consumerBuilder.WithProperties(section.ExtraPropertiesMap);
        }

        private static void MapScreenshotConsumer(ScreenshotConsumerJsonSection section, AtataContextBuilder builder)
        {
            var consumerBuilder = builder.AddScreenshotConsumer(section.Type);

            consumerBuilder.WithProperties(section.ExtraPropertiesMap);
        }

        private static void MapDriver(DriverJsonSection section, AtataContextBuilder builder)
        {
            IDriverJsonMapper mapper = DriverJsonMapperAliases.Resolve(section.Type);
            mapper.Map(section, builder);
        }
    }
}
