function ChatUpdateStandards(standards) {
    var resultsHtml = "";

    let documentDiv = document.getElementById('chat-doc-details-div')
    documentDiv.clientHeight;

    resultsHtml += '<h4 style="text-align:center;">Related Documents Standard</h4>';
    for (var i = 0; i < standards.length; i++) {
        var iconStr = standards[i].slice(0, parseInt(standards[i].indexOf(" ")));
        var iconCSS = "";
        if (iconStr == "ISO") {
            iconCSS = "../images/iso_icon.png";
        } else if (iconStr == "NFPA") {
            iconCSS = "../images/nfpa_icon2.png";
        } else if (iconStr == "API") {
            iconCSS = "../images/api_icon.png";
        } else {
            iconCSS = "../images/logo.png";
        }
        resultsHtml += `<div style="display:flex">
                            <div style="width:50px">
                                <img src="${iconCSS}" class="standards_icon">
                            </div>
                            <div>
                                <h5 style="display: unset; margin-top: 0px;">${standards[i]}</h5>
                            </div>
                        </div>`
    }
    let standardsDiv = $("#chat-doc-standards-div")
    standardsDiv.css("min-height", documentDiv.clientHeight - 10 + 'px');
    standardsDiv.removeClass("hide");
    $("#maps-viewer").removeClass("hide");
    standardsDiv.html(resultsHtml);
}

function ChatUpdatePagination(docCount) {
    var totalPages = Math.round(docCount / 10);
    // Set a max of 5 items and set the current page in middle of pages
    var startPage = currentPage;

    var maxPage = startPage + 5;
    if (totalPages < maxPage)
        maxPage = totalPages + 1;
    var backPage = parseInt(currentPage) - 1;
    if (backPage < 1)
        backPage = 1;
    var forwardPage = parseInt(currentPage) + 1;

    var htmlString = "";
    if (currentPage > 1) {
        htmlString = `<li><a href="javascript:void(0)" onclick="GoToPage('${backPage}')" class="ms-Icon ms-Icon--ChevronLeftMed"></a></li>`;
    }

    htmlString += '<li class="active"><a href="#">' + currentPage + '</a></li>';

    if (currentPage <= totalPages) {
        htmlString += `<li><a href="javascript:void(0)" onclick="GoToPage('${forwardPage}')" class="ms-Icon ms-Icon--ChevronRightMed"></a></li>`;
    }
    $("#chat-pagination").html(htmlString);
    $("#chat-paginationFooter").html(htmlString);
}

function ChatUpdateResults(data, countDom = "#doc-count", detailsDom = "#doc-details-div") {
    var resultsHtml = '';
    let search = "*";
    const regex = /"/gi;

    search = search.replaceAll(regex, '&quot;');
    $(countDom).html(` Available Results: ${data.count}`);
    if (data.results != null) {
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
                                                    <div class="col-md-12 col-search-details-padding">
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
                //removed content preview for now
                //<div class="col-md-6 col-search-details-padding">
                //    ${contentPreview}
                //</div>
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
    }

    $(detailsDom).html(resultsHtml);
}

