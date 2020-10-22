function loopHistogram(data) {
    var margin = { top: 20, right: 20, bottom: 30, left: 50 },
        width = 330 - margin.left - margin.right,
        height = 460 - margin.top - margin.bottom;

    // set the ranges
    var x = d3.scaleTime().range([0, width]);
    var y = d3.scaleLinear().range([height, 0]);

    // define the 1st line
    var valueline = d3.line()
        .x(function (d) { return x(d.failuredateyr); })
        .y(function (d) { return y(d.priority); });

    // define the 2nd line
    var valueline2 = d3.line()
        .x(function (d) { return x(d.failuredateyr); })
        .y(function (d) { return y(d.failurecount); });

    // append the svg obgect to the body of the page
    // appends a 'group' element to 'svg'
    // moves the 'group' element to the top left margin
    d3.select("#histogram-container-id").remove();
    var svg = d3.select("#histogram-container").append("svg")
        .attr("id", "histogram-container-id")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
            "translate(" + margin.left + "," + margin.top + ")");

    // format the data
    data.forEach(function (d) {
        d.priority = +d.priority;
        d.failurecount = +d.failurecount;
    });

    // Scale the range of the data
    x.domain(d3.extent(data, function (d) {
        return d.failuredateyr;
    }));
    y.domain([0, d3.max(data, function (d) {
        return Math.max(d.priority, d.failurecount);
    })]);

    // Add the valueline path.
    svg.append("path")
        .data([data])
        .attr("class", "line")
        .style("stroke", "steelblue")
        .attr("d", valueline);

    // Add the valueline2 path.
    svg.append("path")
        .data([data])
        .attr("class", "line")
        .style("stroke", "red")
        .attr("d", valueline2);

    // Add the X Axis
    svg.append("g")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(x));

    // Add the Y Axis
    svg.append("g")
        .call(d3.axisLeft(y));

    svg.append("text")
        .attr("x", (width / 2))
        .attr("y", "-5")
        .attr("text-anchor", "middle")
        .style("font-size", "16px")
        .style("text-decoration", "underline")
        .text("Failure Count");
}