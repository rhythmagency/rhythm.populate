using Archetype.Models;

namespace Rhythm.Populate.Archetype.Extensions
{
    public static class IMappingSessionExtensions
    {
        public static ArchetypeMappingExecutor<T> Map<T>(this IMappingSession session, ArchetypeModel model, string dataType) where T : class
        {
            return new ArchetypeMappingExecutor<T>(session, model, dataType);
        }
    }
}