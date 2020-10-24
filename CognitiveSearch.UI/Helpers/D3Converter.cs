using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.Extensions.Azure;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Helpers
{
    public static class D3Converter
    {
        public static Dictionary<string, D3Node> StaticQuery(string opuString, string opuLocationLinkString, string locationEquipmentLinkString, string equipmentLinkString)
        {
            var children = ConvertInverse(locationEquipmentLinkString, null as string, equipmentLinkString, new string[1] { "Location" });
            var main = Convert(opuLocationLinkString, opuString, children);
            var json = JsonSerializer.Serialize<List<D3Node>>(main.Values.ToList<D3Node>());
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
                    info = getInfo(child),
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
            Dictionary<string, D3Node> outputParentsDict = new Dictionary<string, D3Node>();
            foreach (var parent in parentJSONObj)
            {
                var node = new D3Node()
                {
                    nodeName = parent.id,
                    type = getType(parent.label),
                    label = new D3Label() { nodeName = "", label = getLabel(parent) },
                    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    info = getInfo(parent),
                    children = new List<D3Node>()
                };
                parentsDict.Add(parent.id, node);
            }

            foreach (var relationship in linkJSONObj)
            {
                if (!childrenDict.ContainsKey(relationship.inV))
                {
                    //var newChild = new D3Node()
                    //{
                    //    nodeName = relationship.inV,
                    //    type = getType(relationship.inVLabel),
                    //    label = new D3Label() { nodeName = "", label = relationship.inV },
                    //    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    //    children = new List<D3Node>()
                    //};
                    //childrenDict.Add(relationship.inV, newChild);
                }
                else
                {
                    var child = childrenDict[relationship.inV];
                    var parent = parentsDict[relationship.outV];
                    if (child != null && parent != null)
                    {
                        parent.children.Add(child);
                        if (!outputParentsDict.ContainsKey(relationship.outV))
                        {
                            outputParentsDict.Add(relationship.outV, parent);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("child " + relationship.inV + " does not exist");
                    }
                }
            }

            return outputParentsDict;
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
                        info = new List<D3Label>(),
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
                        info = new List<D3Label>(),
                        children = new List<D3Node>()
                    };
                    childrenDict.Add(relationship.inV, newChild);
                }
                var child = childrenDict[relationship.inV];
                parent.children.Add(child);
            }
            return parentDict;
        }

        public static Dictionary<string, D3Node> ConvertInverse(string relationshipJSON, string parentDetailsJSON, string childDetailsJSON, string[] parentAllowList = null)
        {
            var childJSONObj = JsonSerializer.Deserialize<CosmosNode[]>(childDetailsJSON);

            Dictionary<string, D3Node> childrenDict = new Dictionary<string, D3Node>();
            foreach (var child in childJSONObj)
            {
                if (!childrenDict.ContainsKey(child.id))
                {
                    var node = new D3Node()
                    {
                        nodeName = child.id,
                        type = getType(child.label),
                        label = new D3Label() { nodeName = "", label = getLabel(child) },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        info = getInfo(child),
                        children = new List<D3Node>()
                    };
                    childrenDict.Add(child.id, node);
                }
            }

            if (parentDetailsJSON == null)
            {
                return ConvertInverse(relationshipJSON, childrenDict, parentAllowList);
            }
            else
            {
                return ConvertInverse(relationshipJSON, parentDetailsJSON, childrenDict, parentAllowList);
            }
        }

        public static Dictionary<string, D3Node> ConvertInverse(string relationshipJSON, string parentDetailsJSON, Dictionary<string, D3Node> childrenDict = null, string[] parentAllowList = null)
        {
            var parentJSONObj = JsonSerializer.Deserialize<CosmosNode[]>(parentDetailsJSON);
            var linkJSONObj = JsonSerializer.Deserialize<CosmosRelationship[]>(relationshipJSON);
            if (childrenDict == null)
            {
                childrenDict = new Dictionary<string, D3Node>();
            }

            Dictionary<string, D3Node> parentsDict = new Dictionary<string, D3Node>();
            Dictionary<string, D3Node> outputParentsDict = new Dictionary<string, D3Node>();
            foreach (var parent in parentJSONObj)
            {
                var node = new D3Node()
                {
                    nodeName = parent.id,
                    type = getType(parent.label),
                    label = new D3Label() { nodeName = "", label = getLabel(parent) },
                    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    info = getInfo(parent),
                    children = new List<D3Node>()
                };
                parentsDict.Add(parent.id, node);
            }

            foreach (var relationship in linkJSONObj)
            {
                if (parentAllowList != null)
                {
                    if (!parentAllowList.Contains(relationship.inVLabel))
                    {
                        continue;
                    }
                }
                if (!childrenDict.ContainsKey(relationship.outV))
                {
                    //var newChild = new D3Node()
                    //{
                    //    nodeName = relationship.inV,
                    //    type = getType(relationship.inVLabel),
                    //    label = new D3Label() { nodeName = "", label = relationship.inV },
                    //    link = new D3Link() { nodeName = "", direction = "SYNC" },
                    //    children = new List<D3Node>()
                    //};
                    //childrenDict.Add(relationship.inV, newChild);
                }
                else
                {
                    var child = childrenDict[relationship.outV];
                    var parent = parentsDict[relationship.inV];
                    if (child != null && parent != null)
                    {
                        parent.children.Add(child);
                        if (!outputParentsDict.ContainsKey(relationship.inV))
                        {
                            outputParentsDict.Add(relationship.inV, parent);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine("child " + relationship.outV + " does not exist");
                    }
                }
            }

            return outputParentsDict;
        }

        public static Dictionary<string, D3Node> ConvertInverse(string relationshipJSON, Dictionary<string, D3Node> childrenDict = null, string[] parentAllowList = null)
        {
            var linkJSONObj = JsonSerializer.Deserialize<CosmosRelationship[]>(relationshipJSON);
            Dictionary<string, D3Node> parentDict = new Dictionary<string, D3Node>();
            if (childrenDict == null)
            {
                childrenDict = new Dictionary<string, D3Node>();
            }

            foreach (var relationship in linkJSONObj)
            {
                if(parentAllowList != null)
                {
                    if (!parentAllowList.Contains(relationship.inVLabel)) {
                        continue;
                    }
                }
                if (!parentDict.ContainsKey(relationship.inV))
                {
                    var newParent = new D3Node()
                    {
                        nodeName = relationship.inV,
                        type = getType("Location"),
                        label = new D3Label() { nodeName = "", label = relationship.inV },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        info = new List<D3Label>(),
                        children = new List<D3Node>()
                    };
                    parentDict.Add(relationship.inV, newParent);
                }
                var parent = parentDict[relationship.inV];
                if (!childrenDict.ContainsKey(relationship.outV))
                {
                    var newChild = new D3Node()
                    {
                        nodeName = relationship.outV,
                        type = getType(relationship.outVLabel),
                        label = new D3Label() { nodeName = "", label = relationship.outV },
                        link = new D3Link() { nodeName = "", direction = "SYNC" },
                        info = new List<D3Label>(),
                        children = new List<D3Node>()
                    };
                    childrenDict.Add(relationship.outV, newChild);
                }
                var child = childrenDict[relationship.outV];
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

        public static List<D3Label> getInfo(CosmosNode node)
        {
            var infoList = new List<D3Label>();
            switch (node.label)
            {
                case "Equipment":
                    infoList.Add(new D3Label() { nodeName = "FuncLocDesc", label = node.properties["FuncLocDesc"][0].value });
                    infoList.Add(new D3Label() { nodeName = "EquipmentClass", label = node.properties["EquipmentClass"][0].value });
                    infoList.Add(new D3Label() { nodeName = "EquipmentModel", label = node.properties["EquipmentModel"][0].value });
                    infoList.Add(new D3Label() { nodeName = "Manufacturer", label = node.properties["Manufacturer"][0].value });
                    return infoList;
                default:
                    return infoList;
            }

        }

    }

    public class D3Node
    {
        public string nodeName { get; set; }
        public string type { get; set; }
        public D3Label label { get; set; }
        public D3Link link { get; set; }
        public List<D3Label> info { get; set; }
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
