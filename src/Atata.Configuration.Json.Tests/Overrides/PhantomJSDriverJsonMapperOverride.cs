﻿namespace Atata.Configuration.Json.Tests
{
    public class PhantomJSDriverJsonMapperOverride : PhantomJSDriverJsonMapper
    {
        protected override PhantomJSAtataContextBuilder CreateDriverBuilder(AtataContextBuilder builder)
        {
            return builder.UseDriver(new PhantomJSAtataContextBuilderOverride(builder.BuildingContext));
        }
    }
}
