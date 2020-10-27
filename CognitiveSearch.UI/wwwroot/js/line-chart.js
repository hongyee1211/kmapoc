function loopLineChart(data) {
    var margin = { top: 20, right: 20, bottom: 30, left: 50 },
        width = 400 - margin.left - margin.right,
        height = 130 - margin.top - margin.bottom;

    // set the ranges
    var x = d3.scalePoint().range([0, width]);
    var y = d3.scaleLinear().range([height, 0]);


    // define the 1st line
    var valueline = d3.line()
        .x(function (d) { return x(d.FailureDateYr); })
        .y(function (d) { return y(d.FailureCount); });

    // define the 2nd line
    /*var valueline2 = d3.line()
        .x(function (d) { return x(d.FailureDateYr); })
        .y(function (d) { return y(d.FailureCount); });*/

    // append the svg obgect to the body of the page
    // appends a 'group' element to 'svg'
    // moves the 'group' element to the top left margin
    d3.select("#line-chart-container-id").remove();
    var svg = d3.select("#line-chart-container").append("svg")
        .attr("id", "line-chart-container-id")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");

    // format the data
    data.forEach(function (d) {
        d.FailureCount = +d.FailureCount;
        /*d.FailureCount = +d.FailureCount;*/
    });

    // Scale the range of the data
    x.domain(data.map(function (d) { return d.FailureDateYr; }));
    y.domain([0, d3.max(data, function (d) {
        /*return Math.max(d.FailureCount, d.FailureCount);*/
        return Math.max(d.FailureCount);
    })]);

    // Add the valueline path.
    svg.append("path")
        .data([data])
        .attr("class", "line")
        .style("stroke", "steelblue")
        .attr("d", valueline);

    // Add the valueline2 path.
    /*svg.append("path")
        .data([data])
        .attr("class", "line")
        .style("stroke", "red")
        .attr("d", valueline2);*/

    // Add the X Axis
    svg.append("g")
        .attr("transform", "translate(0," + height + ")")
        .append("g")
        .call(d3.axisBottom(x).ticks(10));
        /*.selectAll("text")
        .style("text-anchor", "end")
        .attr("dx", "-.8em")
        .attr("dy", ".15em")
        .attr("transform", "rotate(-65)");*/

    // Add the Y Axis
    svg.append("g")
        .call(d3.axisLeft(y));

    svg.append("text")
        .attr("x", (width / 2))
        .attr("y", "-5")
        .attr("text-anchor", "middle")
        .style("font-size", "16px")
        .style("text-decoration", "underline")
        .text("Failure Trend");
}