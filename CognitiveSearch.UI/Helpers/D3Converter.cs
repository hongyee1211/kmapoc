using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.Extensions.Azure;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public static class D3Converter
    {
        public static Dictionary<string, D3Node> Convert()
        {
            string opuString = "[\n  {\n    \"id\": \"M060\",\n    \"label\": \"OPU\",\n    \"type\": \"vertex\",\n    \"properties\": {\n      \"pk\": [\n        {\n          \"id\": \"M060|pk\",\n          \"value\": \"M060\"\n        }\n      ],\n      \"PlantCode\": [\n        {\n          \"id\": \"9e9dbd5e-e2b8-46ac-83e1-f1917c688047\",\n          \"value\": \"MLNG\"\n        }\n      ],\n      \"PlantName\": [\n        {\n          \"id\": \"ee274181-a217-4f4b-ba8c-48f8f0d59e34\",\n          \"value\": \"MLNG Maint Planning Plant\"\n        }\n      ]\n    }\n  }\n]";
            string opuLocationLinkString = "[\n  {\n    \"id\": \"M060-MOD3 UNIT 1100-15\",\n    \"label\": \"has\",\n    \"type\": \"edge\",\n    \"inVLabel\": \"Location\",\n    \"outVLabel\": \"OPU\",\n    \"inV\": \"MOD3 UNIT 1100\",\n    \"outV\": \"M060\"\n  },\n  {\n    \"id\": \"M060-MOD2 UNIT 5700-13\",\n    \"label\": \"has\",\n    \"type\": \"edge\",\n    \"inVLabel\": \"Location\",\n    \"outVLabel\": \"OPU\",\n    \"inV\": \"MOD2 UNIT 5700\",\n    \"outV\": \"M060\"\n  }\n]";
            string locationEquipmentLinkString = "[\n  {\n    \"id\": \"MOD3 UNIT 1100-3EM1105F-996\",\n    \"label\": \"consist of\",\n    \"type\": \"edge\",\n    \"inVLabel\": \"Equipment\",\n    \"outVLabel\": \"Location\",\n    \"inV\": \"3EM1105F\",\n    \"outV\": \"MOD3 UNIT 1100\"\n  },\n  {\n    \"id\": \"MOD3 UNIT 1100-3EM1105E-995\",\n    \"label\": \"consist of\",\n    \"type\": \"edge\",\n    \"inVLabel\": \"Equipment\",\n    \"outVLabel\": \"Location\",\n    \"inV\": \"3EM1105E\",\n    \"outV\": \"MOD3 UNIT 1100\"\n  },\n  {\n    \"id\": \"MOD2 UNIT 5700-3EN1105E-995\",\n    \"label\": \"consist of\",\n    \"type\": \"edge\",\n    \"inVLabel\": \"Equipment\",\n    \"outVLabel\": \"Location\",\n    \"inV\": \"3EN1105E\",\n    \"outV\": \"MOD2 UNIT 5700\"\n  }\n]";

            var children = Convert(locationEquipmentLinkString);
            var main = Convert(opuLocationLinkString, opuString, children);
            var json = JsonSerializer.Serialize<List<D3Node>>(main.Values.ToList<D3Node>());
            System.IO.File.WriteAllText(@"A:\ACN\output\path.json", json);
            return main;
        }

        public static Dictionary<string, D3Node> Convert(string relationshipJSON, string parentDetailsJSON, string childDetailsJSON)
        {
            var childJSONObj = JsonSerializer.Deserialize<CosmosNode[]>(childDetailsJSON);

            Dictionary<string, D3Node> childrenDict = new Dictionary<string, D3Node>();
            foreach (var child in childJSONObj)
            {
                var node = new D3Node()
                {
                    nodeName = child.id,
                    type = getType(child.label),
                    label = new D3Label() { nodeName = "", label = getLabel(child) },
                    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    children = new List<D3Node>()
                };
                childrenDict.Add(child.id, node);
            }

            return Convert(relationshipJSON, parentDetailsJSON, childrenDict);
        }

        public static Dictionary<string, D3Node> Convert(string relationshipJSON, string parentDetailsJSON, Dictionary<string, D3Node> childrenDict = null)
        {
            var parentJSONObj = JsonSerializer.Deserialize<CosmosNode[]>(parentDetailsJSON);
            var linkJSONObj = JsonSerializer.Deserialize<CosmosRelationship[]>(relationshipJSON);
            if (childrenDict == null)
            {
                childrenDict = new Dictionary<string, D3Node>();
            }

            Dictionary<string, D3Node> parentsDict = new Dictionary<string, D3Node>();
            foreach (var parent in parentJSONObj)
            {
                var node = new D3Node()
                {
                    nodeName = parent.id,
                    type = getType(parent.label),
                    label = new D3Label() { nodeName = "", label = getLabel(parent) },
                    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    children = new List<D3Node>()
                };
                parentsDict.Add(parent.id, node);
            }

            foreach (var relationship in linkJSONObj)
            {
                if (!childrenDict.ContainsKey(relationship.inV))
                {
                    var newChild = new D3Node()
                    {
                        nodeName = relationship.inV,
                        type = getType(relationship.inVLabel),
                        label = new D3Label() { nodeName = "", label = relationship.inV },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        children = new List<D3Node>()
                    };
                    childrenDict.Add(relationship.inV, newChild);
                }
                var child = childrenDict[relationship.inV];
                var parent = parentsDict[relationship.outV];
                if (child != null && parent != null)
                {
                    parent.children.Add(child);
                }
                else
                {
                    Console.Error.WriteLine("child " + relationship.inV + " does not exist");
                }
            }

            return parentsDict;
        }

        public static Dictionary<string, D3Node> Convert(string relationshipJSON, Dictionary<string, D3Node> childrenDict = null)
        {
            var linkJSONObj = JsonSerializer.Deserialize<CosmosRelationship[]>(relationshipJSON);
            Dictionary<string, D3Node> parentDict = new Dictionary<string, D3Node>();
            if (childrenDict == null)
            {
                childrenDict = new Dictionary<string, D3Node>();
            }

            foreach (var relationship in linkJSONObj)
            {
                if (!parentDict.ContainsKey(relationship.outV))
                {
                    var newParent = new D3Node()
                    {
                        nodeName = relationship.outV,
                        type = getType(relationship.outVLabel),
                        label = new D3Label() { nodeName = "", label = relationship.outV },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        children = new List<D3Node>()
                    };
                    parentDict.Add(relationship.outV, newParent);
                }
                var parent = parentDict[relationship.outV];
                if (!childrenDict.ContainsKey(relationship.inV))
                {
                    var newChild = new D3Node()
                    {
                        nodeName = relationship.inV,
                        type = getType(relationship.inVLabel),
                        label = new D3Label() { nodeName = "", label = relationship.inV },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        children = new List<D3Node>()
                    };
                    childrenDict.Add(relationship.inV, newChild);
                }
                var child = childrenDict[relationship.inV];
                parent.children.Add(child);
            }
            return parentDict;
        }

        public static string getType(string label)
        {
            switch (label)
            {
                case "OPU":
                    return "type1";
                case "Location":
                    return "type2";
                default:
                    return "type3";
            }

        }

        public static string getLabel(CosmosNode node)
        {
            switch (node.label)
            {
                case "OPU":
                    return node.properties["PlantCode"][0].value;
                case "Location":
                    return node.properties["pk"][0].value;
                default:
                    return node.properties["pk"][0].value;
            }

        }

    }

    public class D3Node
    {
        public string nodeName { get; set; }
        public string type { get; set; }
        public D3Label label { get; set; }
        public D3Link link { get; set; }
        public List<D3Node> children { get; set; }
    }

    public class D3Label
    {
        public string nodeName { get; set; }
        public string label { get; set; }

    }

    public class D3Link
    {
        public string nodeName { get; set; }
        public string direction { get; set; }
    }

    public class CosmosNode
    {
        public string id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public Dictionary<string, Properties[]> properties { get; set; }
    }

    public class CosmosRelationship
    {
        public string id { get; set; }
        public string label { get; set; }
        public string type { get; set; }
        public string inVLabel { get; set; }
        public string outVLabel { get; set; }
        public string inV { get; set; }
        public string outV { get; set; }
    }

    public class Properties
    {
        public string id { get; set; }
        public string value { get; set; }
    }
}
