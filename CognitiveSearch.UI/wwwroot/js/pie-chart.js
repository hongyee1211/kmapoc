function loopPieChart(data) {
   
    var width = 400;
    var height = 100;
    var radius = Math.min(width, height) / 2;
    var donutWidth = 30;
    var legendRectSize = 18;
    var legendSpacing = 4;

    var color = d3.scaleOrdinal(d3.schemeCategory20b);

    d3.select("#pie-chart-container-id").remove();
    var svg = d3.select('#pie-chart-container').append('svg')
        .attr('id', 'pie-chart-container-id')
        .attr('width', width)
        .attr('height', 150)
        .attr("class", "chartMargin")
        .append('g')
        .attr('transform', 'translate(' + (width / 2) +
            ',' + 85 + ')');

    //var arc = d3.arc()
    //    .innerRadius(radius - donutWidth)
    //    .outerRadius(radius);
    var arc = d3.arc()
        .innerRadius(radius - donutWidth)
        .outerRadius(radius);

    var arcOutter = d3.arc()
        .innerRadius(radius - donutWidth)
        .outerRadius(radius + 4);

    var arcPhantom = d3.arc()
        .innerRadius(0)
        .outerRadius(radius + 8);

    var pie = d3.pie()
        .value(function (d) {
            return d.FailureCount;
        })
        .sort(null);

    //Set up outter arc groups
    var outterArcs = svg.selectAll("g.outter-arc")
        .data(pie(data))
        .enter()
        .append("g")
        .attr("class", "outter-arc")

    var arcs = svg.selectAll("g.arc")
        .data(pie(data))
        .enter()
        .append("g")
        .attr("class", "arc")

    //Set up phantom arc groups
    var phantomArcs = svg.selectAll("g.phantom-arc")
        .data(pie(data))
        .enter()
        .append("g")
        .attr("class", "phantom-arc")

    //Draw outter arc paths
    outterArcs.append("path")
        .attr("fill", 'orange')
        .attr("d", arcOutter).style('stroke', '#666')
        .style('stroke-width', 0);

    //Draw arc paths
    arcs.append("path")
        .attr('fill', function (d, i) {
            return color(d.data.Priority + " (" + d.data.FailureCount + ")");
        }).attr("d", arc);

    //Draw phantom arc paths
    phantomArcs.append("path")
        .attr("fill", '#666')
        .attr("fill-opacity", 0.1)
        .attr("d", arcPhantom).style('stroke', '#666')
        .style('stroke-width', 5);

    //var path = svg.selectAll('path')
    //    .data(pie(data))
    //    .enter()
    //    .append('path')
    //    .attr('d', arc)
    //    .attr('fill', function (d, i) {
    //        return color(d.data.Priority + " (" + d.data.FailureCount + ")");
    //    });

    var legend = svg.selectAll('.legend')
        .data(color.domain())
        .enter()             
        .append('g')         
        .attr('class', 'legend')
        .attr('transform', function (d, i) {
            var height = legendRectSize + legendSpacing;
            var offset = height * color.domain().length / 2;     
            var horz = -200;                       
            var vert = i * height - offset;                       
            return 'translate(' + horz + ',' + vert + ')';        
        });                                                     

    legend.append('rect')                                     
        .attr('width', legendRectSize)                          
        .attr('height', legendRectSize)                         
        .style('fill', color)                                   
        .style('stroke', color);                                

    legend.append('text')                                     
        .attr('x', legendRectSize + legendSpacing)              
        .attr('y', legendRectSize - legendSpacing)              
        .text(function (d) { return d; }); 

    svg.append("text")
        .attr("x", 0)
        .attr("y", "-70")
        .attr("text-anchor", "middle")
        .style("font-size", "16px")
        .style("text-decoration", "underline")
        .text("Risk Prediction");
}