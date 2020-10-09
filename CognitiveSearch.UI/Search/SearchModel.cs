// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

namespace CognitiveSearch.UI
{
    public class SearchModel
    {
        private string[] facets = new string[]
        {
            // Add UI facets here in order
            //"locations",
            //"organizations",
            //"keyphrases",
            "PlantCode",
            "Manufacturer",
            "EquipmentClass",
            //"EquipmentType",
            "Abbreviation",
            "Component",
            "FailureMode",
            "people",
        };

        private string[] tags = new string[]
        {
            // Add tags fields here in order
            //"locations",
            //"organizations",
            //"keyphrases"
            "PlantCode",
            "Manufacturer",
            "EquipmentClass",
            //"EquipmentType",
            //"Abbreviation",
            "Component",
            "FailureMode",
            "people",
        };

        private string[] resultFields = new string[]
        {
            "content",
            "metadata_storage_content_type",
            "metadata_storage_name",
            "metadata_storage_size",
            "metadata_storage_path",
            "metadata_storage_file_extension",
            "metadata_content_type",

            // Add fields needed to display results cards

            // NOTE: if you customize the resultFields, be sure to include metadata_storage_name,
            // metadata_storage_path as those fields are needed for the UI to work properly
            "people",
            //"locations",
            //"organizations",
            //"keyPhrases",
            "PlantCode",
            "Manufacturer",
            "EquipmentClass",
            "EquipmentType",
            "Abbreviation",
            "Component",
            "FailureMode",
            //"language",
            //"translated_text",
            "merged_content",
            "text",
            "layoutText",
            "imageTags",
            "imageCaption"
        };

        public List<SearchField> Facets { get; set; }
        public List<SearchField> Tags { get; set; }

        public string[] SelectFilter { get; set; }

        public string[] SearchableFields { get; set; }

        public Dictionary<string, string[]> SearchFacets { get; set; }

        public SearchModel(SearchSchema schema)
        {
            Facets = new List<SearchField>();
            Tags = new List<SearchField>();

            List<string> validatedResultFields = new List<string>();
            foreach (string s in resultFields)
            {
                if (schema.Fields.ContainsKey(s))
                {
                    validatedResultFields.Add(s);
                }
            }
            SelectFilter = validatedResultFields.ToArray();

            if (facets.Count() > 0)
            {
                // add field to facets if in facets arr
                foreach (var field in facets)
                {
                    if (schema.Fields[field] != null && schema.Fields[field].IsFacetable)
                    {
                        Facets.Add(schema.Fields[field]);
                    }
                }
            }
            else
            {
                foreach (var field in schema.Fields.Where(f => f.Value.IsFacetable))
                {
                    Facets.Add(field.Value);
                }
            }

            if (tags.Count() > 0)
            {
                foreach (var field in tags)
                {
                    if (schema.Fields[field] != null && schema.Fields[field].IsFacetable)
                    {
                        Tags.Add(schema.Fields[field]);
                    }
                }
            }
            else
            {
                foreach (var field in schema.Fields.Where(f => f.Value.IsFacetable))
                {
                    Tags.Add(field.Value);
                }
            }

            SearchableFields = schema.Fields.Where(f => f.Value.IsSearchable).Select(f => f.Key).ToArray();
        }
    }
}
