using System;

namespace Rhythm.Populate
{
    public interface IMappingRule
    {
        void Execute(IMappingSession session, MappingOptions options, object model, Type type, object source);
    }
}