﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Graph Configuration
var nodeRadius = 30;
var nodeChargeStrength = -300;
var nodeChargeAccuracy = 0.8;
var nodeDesaturation = 0.1;

currentMaxLevels = 3;
currentMaxNodes = 10;
currentDistance = 1;

var selectedGraphFields = {};

$("#e").keyup(function (e) {
    if (e.keyCode === 13) {
        SearchEntities();
    }
});

$(".checkbox-menu").on("change", "input[type='checkbox']", function () {
    $(this).closest("li").toggleClass("active", this.checked);
    if (this.checked)
        selectedGraphFields[this.value] = true;
    else
        delete selectedGraphFields[this.value];

    SearchEntities();
});

$(document).on('click', '.allow-focus', function (e) {
    e.stopPropagation();
});

function SearchEntities() {
    //temporary workaround for chat entity map
    if (isChat) {
        if (chatSearchString == null || chatSearchString == "") {
            //let keys = Object.keys(filterSelected);
            //for (let i = 0; i < keys.length; i++) {
            //    let key = keys[i]
            //    let values = filterSelected[key]
            //    for (let j = 0; j < values.length; j++) {
            //        search += `,"${values[j]}"`;
            //    }
            //}
        }
        else {
            document.getElementById("entity-loading-indicator").style.display = "block";
            GetGraph(chatSearchString);
        }
    }
    else {
        if (currentPage > 1) {
            if (q !== $("#e").val()) {
                currentPage = 1;
            }
        }
        q = $("#e").val();
        $("#q").val(q);
        UpdateLocationBar();
        UpdateGraphParameterUI();

        document.getElementById("entity-loading-indicator").style.display = "block";

        GetGraph(q);

        // Get center of map to use to score the search results
        UpdateResultsView();
    }
}

// Load Graph with Search data
function GetGraph(q) {
    if (q === null) {
        q = "*";
    }
    $.ajax({
        type: "POST",
        url: "/Home/GetGraphData",
        data: {
            query: q,
            fields: Object.keys(selectedGraphFields),
            maxLevels: currentMaxLevels,
            maxNodes: currentMaxNodes
        },
        success: function (data) {
            Unload();
            viewParams.nodes = data.nodes;
            viewParams.links = data.links;
            UpdateEntityGraph();
        }
    });
}

function LoadEntityMap() {
    document.getElementById("results-container").style.display = "none";
    document.getElementById("details-modal").style.display = "none";
    document.getElementById("entity-map").style.display = "block";
    document.getElementById("entity-loading-indicator").style.display = "block";

    document.getElementById("e").value = q;
    SearchEntities();
}

function UnloadEntityMap() {
    document.getElementById("results-container").style.display = "block";
    document.getElementById("entity-map").style.display = "none";
    Unload();

    document.getElementById("results-container").style = "row content-results";
    document.getElementById("q").value = q;
    document.getElementById("search-button").click();
}

function EntityMapClick() {
    if (document.getElementById("entity-map").style.display === "none") {
        LoadEntityMap();
    }
    else {
        UnloadEntityMap();
    }
}


function Unload() {
    svg.selectAll(".link").remove();
    svg.selectAll(".edgepath").remove();
    svg.selectAll(".node").remove();
    svg.selectAll(".edgelabel").remove();
    svg.selectAll(".text").remove();
}

// debounce resize events
var resizeSearchToken;
$(window).resize(function () {
    clearTimeout(resizeSearchToken);
    resizeSearchToken = setTimeout(SearchEntities, 200); // after debounce timeout, re Search the entity graph
}); 

var colors = d3.scaleOrdinal(d3.schemeCategory10);
var svg = d3.select("svg");
var viewParams = {
    width: 10, // Safe Defaults
    height: 10, // Safe Defaults
    nodes: null,
    links: null
};
var node;
var link;

function nodeBounds() {
    var nodes;

    function force() {
        var i,
            n = nodes.length,
            node;

        for (i = 0; i < n; ++i) {
            node = nodes[i];
            var clampedx = Math.max(nodeRadius, Math.min(viewParams.width - nodeRadius, node.x));
            var clampedy = Math.max(nodeRadius, Math.min(viewParams.height - nodeRadius, node.y));
            node.x = clampedx;
            node.y = clampedy;
        }
    }

    force.initialize = function (_) {
        nodes = _;
    };

    return force;
};

function setupSimulation(simulation) {
    return simulation
        .force("link", d3.forceLink()
            .id(function (d) { return d.id; })
            .distance(function (d) { return d.distance; })
            .strength(.25))
        .force("charge", d3.forceManyBody()
            .strength(nodeChargeStrength)
            .theta(nodeChargeAccuracy))
        .force("center", nodeBounds())
        .force("collide", d3.forceCollide(nodeRadius))
        ;
}
var simulation = setupSimulation(d3.forceSimulation());


function UpdateEntityGraph() {
    // Graph implementation
    let chatMap = $("#entity-map");
    if (chatMap != null) {
        $("#entity-map").removeClass("hide")
    }

    var colors = d3.scaleOrdinal(d3.schemeCategory10);

    // calculate size
    var svgElement = $("#entity-map-svg");
    viewParams.width = +svgElement.parent().width(); // Get the parent width with jquery instead of d3.
    viewParams.height = +svgElement.height();

    // set svg size
    var svg = d3.select("#entity-map-svg");
    svg.attrs({
        width: viewParams.width,
        height: viewParams.height
    });
        
    svg.append('defs').append('marker')
        .attrs({
            'id': 'arrowhead',
            'viewBox': '-0 -5 10 10',
            'refX': 10,
            'refY': 0,
            'orient': 'auto',
            'markerWidth': 5,
            'markerHeight': 5,
            'xoverflow': 'visible'
        })
        .append('svg:path')
        .attr('d', 'M 0,-5 L 10 ,0 L 0,5')
        .attr('fill', '#999')
        .style('stroke', 'none');

    simulation = setupSimulation(d3.forceSimulation());

    link = svg.selectAll(".link")
        .data(viewParams.links)
        .enter()
        .append("line")
        .attr("class", "link")
        .style("stroke", function (d) {
            return "#ccc"
            //let idx = d.target
            //let node = viewParams.nodes[idx]
            //if (node.facetName == "EquipmentClass") {
            //    return "violet"; //violet
            //} else if (node.facetName == "Manufacturer") {
            //    return "darkorchid"; //darkorchid
            //} else if (node.facetName == "PlantCode") {
            //    return "khaki"; //khaki
            //} else if (node.facetName == "Header") {
            //    // random color
            //    return "#ccc"
            //} else {
            //    return "#00a19c";
            //}
        });

    node = svg.selectAll(".node")
        .data(viewParams.nodes)
        .enter()
        .append("g")
        .attr("class", "node")
        .call(d3.drag()
            .on("start", dragstarted)
            .on("drag", dragged)
            .on("end", dragended)
        );

    node.append("circle")
        .attr("r", function (d) {
            // Determine an initial position
            if (d.cornerStone > -1) {
                // Root element is on the left side of the screen
                var spreadStride = .2;
                var margin = (1 - spreadStride * (currentMaxLevels-1)) * .5;
                d.fx = viewParams.width * (margin + spreadStride * d.cornerStone);

                var shift = d.cornerStone % 2 ? -0.1 : 0.1;
                d.fy = viewParams.height * (0.5 + shift);
            }
            else {
                // Arrange other nodes along the right side of the screen. 
                //  start them some varyin offset so the simulation is stable on start.
                d.x = viewParams.width * 0.8;
                d.y = viewParams.height * (d.id / 100);
            }

            d.radius = nodeRadius / (d.layer + 1);

            return d.radius;
        })
        .style("fill", function (d) {

            if (d.facetName == "EquipmentClass") {
                return "violet"; //violet
            } else if (d.facetName == "Manufacturer") {
                return "darkorchid"; //darkorchid
            } else if (d.facetName == "PlantCode") {
                return "khaki"; //khaki
            } else if (d.facetName == "Header") {
                // random color
                return applySaturationToHexColor(colors(d.color), 1.0 - d.layer * nodeDesaturation);
            } else {
                return "#00a19c";
            }
            
        })
        .on("click", function (d) {
            $("#e").val(d.name);
            SearchEntities();
        })
        .append("#entity-map-svg:title")
        .text(function (d) {
            if (d.facetName == 'Header')
                return "";
            return d.name;
        });


    edgepaths = svg.selectAll(".edgepath")
        .data(viewParams.links)
        .enter()
        .append('path')
        .attrs({
            'class': 'edgepath',
            'fill-opacity': 0,
            'stroke-opacity': 0,
            'marker-end': 'url(#arrowhead)'
        })
        .style("pointer-events", "none")
        .style("stroke", function (d) {
            return 'green' 
        });

    // Render text in a second pass so it's on top of all the gfx.
    texts = svg.selectAll(".text")
        .data(viewParams.nodes)
        .enter()
        .append("g")
            .attr("class", "text")
        .append("text")
            .attr("dx", 15)
            .attr("dy", ".35em")
            .attr("font-family", "sans-serif")
            .attr("font-size", "12px")
            .attr("font-weight", "bold")
            .attr("pointer-events", "none")
            .attr("fill", function (d) {
                return d.layer > 1 ? "#808080" : "#D8D8D8";
            })
        .text(function (d) {
            if (d.facetName == 'Header')
                return "";
            return d.name;
        });


    simulation
        .nodes(viewParams.nodes)
        .on("tick", ticked)
        .on("end", function () {
            for (let i = 0; i < viewParams.nodes.length; i++) {
                viewParams.nodes[i].fx = viewParams.nodes[i].x;
                viewParams.nodes[i].fy = viewParams.nodes[i].y;
            }
        });
    simulation.force("link")
        .links(viewParams.links);
    document.getElementById("entity-loading-indicator").style.display = "none";

    // Step the simulation to let it settle
    simulation.tick(10000);
}

function ticked() {
    node
        .attr("transform", function (d) { return "translate(" + d.x + ", " + d.y + ")"; });
    texts
        .attr("transform", function (d) { return "translate(" + d.x + ", " + d.y + ")"; });

    link
        .attr("x1", function (d) { return d.source.x; })
        .attr("y1", function (d) { return d.source.y; })
        .attr("x2", function (d) { return d.target.x; })
        .attr("y2", function (d) { return d.target.y; });

    edgepaths.attr('d', function (d) {
        // Set the end of the path back by the target radius so it just touches the edge
        var tx = d.target.x;
        var ty = d.target.y;
        var sx = d.source.x;
        var sy = d.source.y;
        var stx = sx - tx;
        var sty = sy - ty;
        var len = Math.sqrt(stx * stx + sty * sty);
        var ox = stx / len * d.target.radius;
        var oy = sty / len * d.target.radius;

        // create a path from source to target, less the target radius
        return 'M ' + sx + ' ' + sy + ' L ' + (tx + ox) + ' ' + (ty + oy);
    });

}


function dragstarted(d) {
    if (!d3.event.active) {
        simulation.alphaTarget(0.3).restart();
    }
    //d.fx = d.x;
    //d.fy = d.y;
}
function dragged(d) {

    // Check if movement beyond svg width/height and set to node
    d.fx = Math.max(nodeRadius, Math.min(viewParams.width - nodeRadius, d3.event.x));
    d.fy = Math.max(nodeRadius, Math.min(viewParams.height - nodeRadius, d3.event.y));
}

function dragended(d) {
    //if (!d3.event.active) simulation.alphaTarget(0);

    //// Dont unlock the node if it's the root
    //if (d.id != 0) {
    //    d.fx = undefined;
    //    d.fy = undefined;
    //}
}

// adapted from here: https://stackoverflow.com/a/31675514
function applySaturationToHexColor(hex, saturationPercent) {
    if (!/^#([0-9a-f]{6})$/i.test(hex)) {
        throw ('Unexpected color format');
    }

    var saturationFloat = Math.max(0, Math.min(saturationPercent, 1)),
        rgbIntensityFloat = [
            parseInt(hex.substr(1, 2), 16) / 255,
            parseInt(hex.substr(3, 2), 16) / 255,
            parseInt(hex.substr(5, 2), 16) / 255
        ];

    var rgbIntensityFloatSorted = rgbIntensityFloat.slice(0).sort(function (a, b) { return a - b; }),
        maxIntensityFloat = rgbIntensityFloatSorted[2],
        mediumIntensityFloat = rgbIntensityFloatSorted[1],
        minIntensityFloat = rgbIntensityFloatSorted[0];

    if (maxIntensityFloat == minIntensityFloat) {
        // All colors have same intensity, which means 
        // the original color is gray, so we can't change saturation.
        return hex;
    }

    // New color max intensity wont change. Lets find medium and weak intensities.
    var newMediumIntensityFloat,
        newMinIntensityFloat = maxIntensityFloat * (1 - saturationFloat);

    if (mediumIntensityFloat == minIntensityFloat) {
        // Weak colors have equal intensity.
        newMediumIntensityFloat = newMinIntensityFloat;
    }
    else {
        // Calculate medium intensity with respect to original intensity proportion.
        var intensityProportion = (maxIntensityFloat - mediumIntensityFloat) / (mediumIntensityFloat - minIntensityFloat);
        newMediumIntensityFloat = (intensityProportion * newMinIntensityFloat + maxIntensityFloat) / (intensityProportion + 1);
    }

    var newRgbIntensityFloat = [],
        newRgbIntensityFloatSorted = [newMinIntensityFloat, newMediumIntensityFloat, maxIntensityFloat];

    // We've found new intensities, but we have then sorted from min to max.
    // Now we have to restore original order.
    rgbIntensityFloat.forEach(function (originalRgb) {
        var rgbSortedIndex = rgbIntensityFloatSorted.indexOf(originalRgb);
        newRgbIntensityFloat.push(newRgbIntensityFloatSorted[rgbSortedIndex]);
    });

    var floatToHex = function (val) { return ('0' + Math.round(val * 255).toString(16)).substr(-2); },
        rgb2hex = function (rgb) { return '#' + floatToHex(rgb[0]) + floatToHex(rgb[1]) + floatToHex(rgb[2]); };

    var newHex = rgb2hex(newRgbIntensityFloat);

    return newHex;
}


function changeMaxLevels(value, commit) {
    if (currentMaxLevels != value || commit) {
        currentMaxLevels = value;
        if (commit)
            SearchEntities();
        else
            UpdateGraphParameterUI(); // Preview
    }
}

function changeMaxNodes(value, commit) {
    if (currentMaxNodes != value || commit) {
        currentMaxNodes = value;
        if (commit)
            SearchEntities();
        else
            UpdateGraphParameterUI(); // Preview
    }
}

function UpdateGraphParameterUI() {
    $("#lbl-currentMaxLevels").text(currentMaxLevels);
    $("#slider-currentMaxLevels").val(currentMaxLevels);
    $("#lbl-currentMaxNodes").text(currentMaxNodes);
    $("#slider-currentMaxNodes").val(currentMaxNodes);
}