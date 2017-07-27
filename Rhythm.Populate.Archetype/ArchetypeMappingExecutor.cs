using System;
using System.Collections.Generic;
using System.Linq;
using Archetype.Models;

namespace Rhythm.Populate.Archetype
{
    public class ArchetypeMappingExecutor<T> where T : class
    {
        private readonly string _dataType;
        private readonly ArchetypeModel _model;
        private readonly IList<T> _results;
        private readonly IMappingSession _session;
        private readonly Type _type;
        private readonly MappingOptions _options;

        public ArchetypeMappingExecutor(IMappingSession session, ArchetypeModel model, string dataType)
        {
            _session = session;
            _model = model;
            _type = typeof(T);
            _results = new List<T>();
            _options = new MappingOptions();
            _dataType = dataType;
        }

        private void Execute()
        {
            foreach (var fieldset in _model)
            {
                var alias = $"{_dataType}.{fieldset.Alias}";
                var currentType = Populate.GetRegisteredType(alias, _type);
                var typeHierarchy = currentType.GetHierarchy();

                var archetypeModel = Activator.CreateInstance(currentType) as T;

                foreach (var type in typeHierarchy)
                {
                    var mapping = Populate.GetMappingForType(type);

                    if (mapping == null)
                    {
                        continue;
                    }

                    var rules = mapping.GetRules();

                    foreach (var rule in rules)
                    {
                        rule.Execute(_session, _options, archetypeModel, type, fieldset);
                    }
                }

                _results.Add(archetypeModel);
            }
        }

        public T Single()
        {
            return List().FirstOrDefault();
        }

        public IEnumerable<T> List()
        {
            Execute();

            return _results;
        }
    }
}