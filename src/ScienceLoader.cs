using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Kopernicus;

namespace SciRev
{
    [RequireConfigType(ConfigType.Node)]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public class SituationLoader
    {
        [ParserTarget("name", Optional = false)]
        public String Name;

        [ParserTargetCollection("self", Key = "Situation", NameSignificance = NameSignificance.Key)]
        public List<SituationLoader> Situations;

        [ParserTargetCollection("self", Key = "value", NameSignificance = NameSignificance.Key)]
        public List<String> Values;
    }

    [RequireConfigType(ConfigType.Node)]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class BodyLoader : SituationLoader, IParserEventSubscriber
    {
        void IParserEventSubscriber.Apply(ConfigNode node)
        {
            // Unused
        }

        void IParserEventSubscriber.PostApply(ConfigNode node)
        {
            // Since this is parsing a body, treat the name as an UBI
            Name = UBI.GetName(Name);
        }
    }

    [RequireConfigType(ConfigType.Node)]
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
    public class ResultLoader : IParserEventSubscriber
    {
        [ParserTargetCollection("self", Key = "Body", NameSignificance = NameSignificance.Key)]
        private List<BodyLoader> _bodies;

        [ParserTargetCollection("self", Key = "Situation", NameSignificance = NameSignificance.Key)]
        public List<SituationLoader> Situations;

        void IParserEventSubscriber.Apply(ConfigNode node)
        {
            // Unused
        }

        void IParserEventSubscriber.PostApply(ConfigNode node)
        {
            // Add the entries from _bodies to Situations
            for (Int32 i = 0; i < _bodies.Count; i++)
            {
                Situations.Add(_bodies[i]);
            }
        }
    }
}