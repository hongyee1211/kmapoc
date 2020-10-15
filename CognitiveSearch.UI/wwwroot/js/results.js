// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

function Base64Decode(token) {
    if (token.length === 0) return null;
    // The last character in the token is the number of padding characters.
    var numberOfPaddingCharacters = token.slice(-1);
    // The Base64 string is the token without the last character.
    token = token.slice(0, -1);
    // '-'s are '+'s and '_'s are '/'s.
    token = token.replace(/-/g, '+');
    token = token.replace(/_/g, '/');
    // Pad the Base64 string out with '='s
    for (var i = 0; i < numberOfPaddingCharacters; i++)
        token += "=";
    return atob(token);
}

function GetResultsMapsHTML() {
    var mapsContainerHTML = '';
    mapsContainerHTML += '<div id="myMap" style="height:400px" ></div>';

    // Buttons to enable setting and resetting search polygon
    mapsContainerHTML += '<div style="position:absolute;top:50px;left:50px;">';
    mapsContainerHTML += '    <input type="button" value="Set Search Polygon" onclick="drawingTools.startDrawing();" />';
    mapsContainerHTML += '    <input type="button" value="Clear Search Polygon" onclick="drawingTools.clear();" />';
    mapsContainerHTML += '</div>';

    return mapsContainerHTML;
}

//  Authenticates the map and shows some locations.
function AuthenticateResultsMap(results) {
    $.post('/home/getmapcredentials', {},
        function (data) {

            if (data.mapKey === null || data.mapKey === "")
            {
                showMap = false;
                return;
            }

            var mapsContainerHTML = GetResultsMapsHTML();
            $('#maps-viewer').html(mapsContainerHTML);

            // default map coordinates
            var coordinates = [-122.32, 47.60];

            // Authenticate the map using the key 
            resultsMap = new atlas.Map('myMap', {
                center: coordinates,
                zoom: 9,
                visibility: "visible",
                width: "500px",
                height: "500px",
                style: "grayscale_dark",
                language: 'en-US',
                authOptions: {
                    authType: 'subscriptionKey',
                    subscriptionKey: data.mapKey
                }
            });

            //Wait until the map resources are ready.
            resultsMap.events.add('ready', function () {
  
                /* Construct a zoom control*/
                var zoomControl = new atlas.control.ZoomControl();
      
                /* Add the zoom control to the map*/
                resultsMap.controls.add(zoomControl, {
                    position: "bottom-right"
                    });
                });

            AddMapPoints(results);

            return;
        });
    return;
}

// Adds map points and re-centers the map based on results
function AddMapPoints(results) {
    var coordinates;

    if (mapDataSource !== null) {
        // clear the data source, add new POIs and re-center the map
        mapDataSource.clear();
        coordinates =  UpdatePOIs(results, mapDataSource);
        if (coordinates) {
            resultsMap.setCamera ({ center: coordinates });
        }
    }
    else {
        //Create a data source to add it to the map 
        mapDataSource = new atlas.source.DataSource();
        coordinates = UpdatePOIs(results, mapDataSource);
        
        //Wait until the map resources are ready for first set up.
        resultsMap.events.add('ready', function () {

            //take the last coordinates.
            if (coordinates) { resultsMap.setCamera ({ center: coordinates }); }

            //Add data source and create a symbol layer.
            resultsMap.sources.add(mapDataSource);
            var symbolLayer = new atlas.layer.SymbolLayer(mapDataSource);
            resultsMap.layers.add(symbolLayer);

            //Create a popup but leave it closed so we can update it and display it later.
            popup = new atlas.Popup({
                pixelOffset: [0, -18],
                closeButton: false
            });
            
            //Add a hover event to the symbol layer.
            resultsMap.events.add('click', symbolLayer, function (e) {
                //Make sure that the point exists.
                if (e.shapes && e.shapes.length > 0) {
                    var content, coordinate;
                    var properties = e.shapes[0].getProperties();
                    var id = properties.id;
                    var popupTemplate = `<div class="customInfobox">
                                         <div class="name" onclick="ShowDocument('${id}');" >{name}</div>
                                         <div onclick="ShowDocument('${id}');">{description}</div>
                                         </div>`;
                    content = popupTemplate.replace(/{name}/g, properties.name).replace(/{description}/g, properties.description);
                    coordinate = e.shapes[0].getCoordinates();

                    popup.setOptions({
                        //Update the content of the popup.
                        content: content,

                        //Update the popup's position with the symbol's coordinate.
                        position: coordinate

                    });

                    if (popup.isOpen() !== true) {
                        //Open the popup.
                        popup.open(resultsMap);
                    }
                    else
                    {
                        popup.close();
                    }
                }
            });

            drawingTools = new PolygonDrawingTool(resultsMap, null, function (polygon) {
                //Do something with the polygon.
                mapPolygon = polygon;
            });
        });

        // This is necessary for the map to resize correctly after the 
        // map is actually in view.
        $('#maps-pivot-link').on("click", function () {
            window.setTimeout(function () {
                map.map.resize();
            }, 100);
        });
    }
}

function UpdatePOIs(results, dataSource) {
    var coordinates;
    for (var i = 0; i < results.length; i++) {
        var result = results[i].document;
        var latlon = result.geoLocation;
        if (latlon !== null) {
            if (latlon.isEmpty === false) {
                coordinates = [latlon.longitude, latlon.latitude];
                //Add the symbol to the data source.
                dataSource.add(new atlas.data.Feature(new atlas.data.Point(coordinates), {
                    name: result.metadata_storage_name,
                    description: "Learn more...",
                    id: result.metadata_storage_path
                }));
            }
        }
    }

    return coordinates;
}

function UpdateMap(data) {
    if (showMap === true) {
        if (resultsMap === null) {
            // Create the map
            AuthenticateResultsMap(data.results);
        }
        else {
            AddMapPoints(results);
        }
    }
}

function UpdateStandards(standards, standardDom = "#doc-standards-div") {
    var resultsHtml = "";

    resultsHtml += '<div class="search-row-standards" style="min-height: 2100px !important;">';
    resultsHtml += '<h4 style="text-align:center;">Related Documents Standard</h4>';
    for (var i = 0; i < standards.length; i++) {
        var iconStr = standards[i].trim().slice(0, 3);
        var iconCSS = "";
        if (iconStr == "ISO") {
            iconCSS = "../images/iso_icon.png";
        } else if (iconStr == "NFP") {
            iconCSS = "../images/nfpa_icon2.png";
        } else if (iconStr == "API") {
            iconCSS = "../images/api_icon.png";
        } else {
            iconCSS = "../images/logo.png";
        }
        resultsHtml += '<div><img src="' + iconCSS + '" class="standards_icon"><h5 style="position: absolute;float: right;display: unset; margin-top: 0px;width: 130%">' + standards[i] + '</h5></div>';
    }
    resultsHtml += "</div>";
    $(standardDom).html(resultsHtml);
}

function UpdateResults(data, countDom = "#doc-count", detailsDom ="#doc-details-div") {
    var resultsHtml = '';
    let search = "*";
    const regex = /"/gi;

    search = search.replaceAll(regex, '&quot;');
    $(countDom).html(` Available Results: ${data.count}`);

    for (var i = 0; i < data.results.length; i++) {

        var result = data.results[i];
        var azDocument = result.document;
        azDocument.idx = i;

        var score = parseInt(result.score) + "%";
        var scoreCSS = "";

        if (parseInt(result.score) <= 30) {
            scoreCSS = "searchScore_red";
        } else if (parseInt(result.score) >= 31 && parseInt(result.score) <= 60) {
            scoreCSS = "searchScore_yellow";
        } else {
            scoreCSS = "searchScore_green";
        }

        var fileSize = azDocument.metadata_storage_size;
        var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        if (fileSize == 0) {
            fileSize = '0 Byte';
        } else {
            var indSizes = parseInt(Math.floor(Math.log(fileSize) / Math.log(1024)));
            fileSize = Math.round(fileSize / Math.pow(1024, indSizes), 2) + ' ' + sizes[indSizes];
        }

        var name;
        var title;
        var content;

        if (result.highlights) {
            if (result.highlights?.merged_content) {
                content = "..." + result.highlights?.merged_content + "...";
            } else if (result.highlights?.content) {
                content = "..." + result.highlights?.content + "...";
            }
        } else {
            content = "";
        }

        var pathURL = Base64Decode(azDocument.metadata_storage_path);
        var isSKILL = pathURL.split("/").length - 1 - (pathURL.indexOf("http://") == -1 ? 0 : 2);
        var indFile = isSKILL == 4 ? 'SKILL' : 'myExperts';

        var icon = " ms-Icon--Page";
        var id = azDocument[data.idField]; 
        var tags = GetTagsHTML(azDocument,);
        var path;

        // get path
        if (data.isPathBase64Encoded) {
            path = Base64Decode(azDocument.metadata_storage_path) + token;
        }
        else {
            path = azDocument.metadata_storage_path + token;
        }

        if (azDocument["metadata_storage_name"] !== undefined) {
            //name = document.metadata_storage_name.split(".")[0];
            name = azDocument.metadata_storage_name;
        }
        
        if (azDocument["metadata_title"] !== undefined && azDocument["metadata_title"] !== null) {
            title = azDocument.metadata_title;
        }
        else {
            // Bring up the name to the top
            title = name;
            name = "";
        }


        if (path !== null) {
            var classList = "results-div ";
            if (i === 0) classList += "results-sizer";

            var pathLower = path.toLowerCase();

            if (pathLower.includes(".pdf")) {
                icon = "ms-Icon--PDF";
            }
            else if (pathLower.includes(".htm")) {
                icon = "ms-Icon--FileHTML";
            }
            else if (pathLower.includes(".xml")) {
                icon = "ms-Icon--FileCode";
            }
            else if (pathLower.includes(".doc")) {
                icon = "ms-Icon--WordDocument";
            }
            else if (pathLower.includes(".ppt")) {
                icon = "ms-Icon--PowerPointDocument";
            }
            else if (pathLower.includes(".xls")) {
                icon = "ms-Icon--ExcelDocument";
            }

            var resultContent = "";
            var imageContent = "";

            if (pathLower.includes(".jpg") || pathLower.includes(".png")) {
                icon = "ms-Icon--FileImage";
                imageContent = `<img class="img-result" style='max-width:100%;' src="${path}"/>`;
            }
            else if (pathLower.includes(".mp3")) {
                icon = "ms-Icon--MusicInCollection";
                resultContent = `<div class="audio-result-div">
                                    <audio controls>
                                        <source src="${path}" type="audio/mp3">
                                        Your browser does not support the audio tag.
                                    </audio>
                                </div>`;
            }
            else if (pathLower.includes(".mp4")) {
                icon = "ms-Icon--Video";
                resultContent = `<div class="video-result-div">
                                    <video controls class="video-result">
                                        <source src="${path}" type="video/mp4">
                                        Your browser does not support the video tag.
                                    </video>
                                </div>`;
            }

            var tagsContent = tags ? `<div class="results-body">
                                        <div id="tagdiv${i}" class="tag-container chat-max-lines" style="margin-top:10px;">${tags}</div>
                                    </div>` : "";
            // display:none
            // <div class="col-md-1"><img id="tagimg${i}" src="/images/expand.png" height="30px" onclick="event.stopPropagation(); ShowHideTags(${i});"></div>

            // <div class="results-icon col-md-1 menuIcon" onclick="ShowFeedback('${i}');"></div>
            var contentPreview = content ? `<p class="max-lines">${content}</p>` : "";

            resultsHtml += `<div id="resultdiv${i}" class="${classList}" >
                                <div class="search-row-result ">
                                    <div class="row" style="display: flex;">
                                        <div class="results-icon results-icon-column">
                                            <div class="ms-CommandButton-icon">
                                                <i class="html-icon ms-Icon ${icon}" style="font-size: 26px;"></i>
                                            </div>
                                            <div class="ms-CommandButton-icon">
                                                <i class="html-icon searchScore ${scoreCSS}" style="font-size: 26px;">${score}</i>
                                            </div>
                                        </div>
                                        <div class="results-details-column">
                                            <div class="row">
                                                <div class="search-dropdown">
                                                    <div class="results-icon menuThumbsUp" onclick="ShowFeedback('${i}'); thumbsUp('${id}', '${title}', '${search}');"></div>
                                                    <div class="results-icon menuThumbsDown" onclick="thumbsDown('${id}', '${title}', '${search}')"></div>
                                                    <div id="modalFeedback${i}" class="search-dropdown-content">
                                                        <div class="search-ratings-star-cluster">
                                                            <span class="fa fa-star" id="star-1-${i}" onclick="toggleRating(${1},${i})"></span>
                                                            <span class="fa fa-star" id="star-2-${i}" onclick="toggleRating(${2},${i})"></span>
                                                            <span class="fa fa-star" id="star-3-${i}" onclick="toggleRating(${3},${i})"></span>
                                                            <span class="fa fa-star" id="star-4-${i}" onclick="toggleRating(${4},${i})"></span>
                                                            <span class="fa fa-star" id="star-5-${i}" onclick="toggleRating(${5},${i})"></span>
                                                        </div>
                                                        <div class="form-horizontal">
                                                            <textarea style="width:200px; resize: none;" id="search-ratings-comment-${i}" class="form-control search-ratings-textarea" rows="4" placeholder="Write your comment here..."></textarea>
                                                        </div>
                                                        <div style="text-align: center" class="search-ratebtn-container">
                                                            <button type="button" class="btn btn-info search-ratebtn" onclick="submitRating(${i},'${id}', '${title}', '${search}')">Rate</button>
                                                        </div>
                                                        <span onclick="ShowFeedback('${i}');" class="search-rating-close-button">&times;</span>
                                                    </div>
                                                </div>
                                                <div class="search-result results-column">
                                                    ${imageContent}
                                                    <div class="results-body">
                                                        <div style="cursor: pointer;" onclick="ShowDocument('${id}');">
                                                            <h4>[${indFile}] ${title} [${fileSize}]</h4>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="search-result">
                                                    <div class="col-md-6 col-search-details-padding">
                                                        ${contentPreview}
                                                    </div>
                                                    <div class="col-md-6 col-search-details-padding">
                                                        <h5>${name}</h5>
                                                        ${tagsContent}
                                                        ${resultContent}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
        }
        else {
            resultsHtml += `<div class="${classList}" );">
                                <div class="search-result">
                                    <div class="results-header">
                                        <h4>Could not get metadata_storage_path for this result.</h4>
                                    </div>
                                </div>
                            </div>`; 
        }
    }

    $(detailsDom).html(resultsHtml);
}

function toggleRating(rating, rowId) {
    currentRating = 0;
    for (let i = 5; i > 0; i--) {
        if ($(`#star-${i}-${rowId}`).hasClass("ratings-star-checked")) {
            currentRating = i
            break
        }
    }
    if (rating == currentRating) {
        rating = 0
    }
    for (let i = 5; i > 0; i--) {
        if (rating >= i) {
            $(`#star-${i}-${rowId}`).addClass("ratings-star-checked")
        }
        else {
            $(`#star-${i}-${rowId}`).removeClass("ratings-star-checked")
        }
    }
}

function submitRating(rowId, id, title, search) {
    let rating = 0
    for (let i = 5; i > 0; i--) {
        if ($(`#star-${i}-${rowId}`).hasClass("ratings-star-checked")) {
            rating = i
            break
        }
    }
    let comment = $(`#search-ratings-comment-${rowId}`).val()
    function callback(data, status) {
        if (status == "success") {
            RemoveFeedbackDisplay(rowId)
        }
        else {
            console.error(`Error encountered in saving rating to db, received status:${status}`)
        }
    }
    sendFeedback(id, title, search, comment, rating, callback)
}



function ShowHideTags(i) {
    var node = document.getElementById("tagdiv" + i);
    var image = document.getElementById("tagimg" + i);
    if (node.style.display === "none") {
        node.style.display = "block";
        image.src = "/images/collapse.png";
    }
    else {
        node.style.display = "none";
        image.src = "/images/expand.png";
    }

    $grid.masonry('layout');
}