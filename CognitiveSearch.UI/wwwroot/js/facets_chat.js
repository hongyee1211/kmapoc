//Filters
function ChatUpdateFilterReset() {
    // This allows users to remove filters
    var htmlString = '';
    $("#chatFilterReset").html("");

    let newFacets = tempSelectedFacets.concat(chatSelectedFacets);

    if (newFacets && newFacets.length > 0) {

        htmlString += `<div class="panel panel-default">
                            <div class="panel-heading">
                                <h4 class="panel-title">Current Filters</h4>
                            </div>
                            <div>
                                <div class="panel-body">`;

        newFacets.forEach(function (item, index, array) { // foreach facet with a selected value
            var name = item.key;
            var result = chatFacets.filter(function (f) { return f.key === name; })[0];

            if (item.value && item.value.length > 0) {
                item.value.forEach(function (item2, index2, array) {
                    htmlString += item2 + ` <a href="javascript:void(0)" onclick="ChatRemoveFilter(\'${name}\', \'${item2}'\)"><span class="ms-Icon ms-Icon--Clear"></span></a><br>`;
                    var result = chatFacets.find(function (f) { return f.key === name; })
                    if (result) {
                        var idx = result.value.indexOf(result.value.filter(function (v) {
                            return v.value.toString() === item2.toString();
                        }
                        )[0]);
                        $('#chat-label-' + name + '_' + idx).addClass('is-checked');
                    }

                });
            }
        });

        htmlString += `</div></div></div>`;
    }
    $("#chatFilterReset").html(htmlString);
}

function ChatRemoveFilter(facet, value) {
    // Remove a facet
    var result = chatSelectedFacets.find(function (f) { return f.key === facet});

    if (result) { // if that facet exists
        let idx = result.value.indexOf(value);
        if (idx == 0 && result.value.length <= 1) {
            chatSelectedFacets.splice(chatSelectedFacets.indexOf(result), 1);
        }
        else {
            result.value.splice(idx, 1);
        }
        ChatUpdateFilterReset();
    }
    else {
        result = tempSelectedFacets.find(function (f) { return f.key === facet });
        if (result) {
            let idx = result.value.indexOf(value);
            if (idx == 0 && result.value.length <= 1) {
                tempSelectedFacets.splice(tempSelectedFacets.indexOf(result), 1);
            }
            else {
                result.value.splice(idx, 1);
            }
            ChatUpdateFilterReset();
        }
    }
    var accordionResult = chatFacets.find(function (f) { return f.key === result.key; })
    if (accordionResult) {
        for (let i = 0; i < accordionResult.value.length; i++) {
            if (accordionResult.value[i].value == value) {
                $('#chat-label-' + result.key + '_' + i).removeClass('is-checked');
                break;
            }

        }
    }

}

// Facets
function ChatUpdateFacets() {
    $("#chat-facet-nav").html("");

    var facetResultsHTML = `<div class="panel-group" id="accordion-chat">`;
    chatFacets.forEach(function (item, index, array) {
        var name = item.key;
        var data = item.value;

        if (data !== null && data.length > 0) {

            var title = name.replace(/([A-Z])/g, ' $1').replace(/^./, function (str) { return str.toUpperCase(); })
            var cssClass = "others";
            if (name == "EquipmentClass") {
                cssClass = "EquipmentClass";
            } else if (name == "Manufacturer") {
                cssClass = "Manufacturer";
            } else if (name == "PlantCode") {
                cssClass = "PlantCode";
            } else if (name == "Component") {
                cssClass = "Component";
            } else if (name == "FailureMode") {
                cssClass = "FailureMode";
            }

            facetResultsHTML += `<div class="panel panel-default">
                                <div class="panel-heading panel-heading-${cssClass}">
                                    <h4 class="panel-title" id="${name}-chat-facets">
                                        <a data-toggle="collapse" href="#${name}-chat">${title}</a>
                                    </h4>
                                </div>`;
            if (index === 0) {
                facetResultsHTML += `<div id="${name}-chat" class="panel-collapse collapse show">
                <div class="panel-body">`;
            }
            else {
                facetResultsHTML += `<div id="${name}-chat" class="panel-collapse collapse">
                <div class="panel-body">`;
            }

            if (data !== null) {
                for (var j = 0; j < data.length; j++) {
                    if (data[j].value.toString().length < 100) {
                        var idName = name + "_" + j;
                        facetResultsHTML += `<div class="ms-CheckBox chat-checkbox">
                                            <input tabindex="-1" type="checkbox" class="ms-CheckBox-input" onclick="ChatChooseFacet('${name}','${data[j].value}','${j}');"></input>
                                            <label id="chat-label-${idName}" role="checkbox" class="ms-CheckBox-field" tabindex="0" aria-checked="false" name="checkboxa" onclick="ChatChooseFacet('${name}','${data[j].value}','${j}');">
                                                <span class="ms-Label">
                                                    ${data[j].value} (${data[j].count})
                                                </span> 
                                            </label>
                                            <div class="chat-facet-thumbs-up menuThumbsDown" style="margin: 7px 3px 7px 7px;" onclick="ShowCategoryContextMenu(event,'${name}','${data[j].value}');"></div>
                                        </div>`;
                    }
                }
            }
            //"monitorCategoryTags('${name}','${data[j].value}', ${1});"

            facetResultsHTML += `</div>
                        </div>
                    </div>`;
        }
    });
    facetResultsHTML += `</div>`;
    $("#chat-facet-nav").append(facetResultsHTML);
}


function ChatToggleCheckbox(id) {
    let checkbox = $('#chat-label-' + id);
    
    if (checkbox.hasClass('is-checked')) {
        checkbox.removeClass('is-checked');
    }
    else {
        checkbox.addClass('is-checked');
    }
}

function ChatChooseFacet(facet, value, position) {
    //var boxStatus = document.getElementById(`${facet}_${position}`);
    //if (boxStatus) {
    //    RemoveFilter(facet, value);
    //}
    ChatToggleCheckbox(facet + '_' + position);
    if (chatSelectedFacets !== undefined) {

        // facetValues where key == selected facet
        var result = chatSelectedFacets.filter(function (f) { return f.key === facet; })[0];

        if (result) { // if that facet exists
            var idx = chatSelectedFacets.indexOf(result);

            if (!result.value.includes(value)) {
                result.value.push(value);
                chatSelectedFacets[idx] = result;
            }
            else {
                if (result.value.length <= 1) {
                    chatSelectedFacets.pop(result);
                }
                else {
                    result.value.pop(value);
                }
            }
        }
        else {
            chatSelectedFacets.push({
                key: facet,
                value: [value]
            });
        }
    }

    ChatUpdateFilterReset();
}

function ChatHandleMultipleFacets(filters) {
    let keys = Object.keys(filters);
    chatSearchString = "";
    for (let i = 0; i < keys.length; i++) {
        let key = keys[i]
        let tempKey = "temp" + keys[i]
        var result = tempSelectedFacets.find(function (f) { return f.key === tempKey; });
        if (filters[key].length <= 0) {
            //ignore
        }
        else if (result) {
            if (result.value == null) {
                result.value = [];
            }
            result.value = result.value.concat(filters[key].filter((item) => result.value.indexOf(item) < 0))
            for (let j = 0; j < filters[key].length; j++) {
                chatSearchString += ",\"" + filters[key][j] + "\""
            }
        }
        else {
            tempSelectedFacets.push({
                key: tempKey,
                value: filters[key]
            })
            chatSearchString += ",\"" + filters[key] + "\""
        }
    }
    if (chatSearchString == "") {
        chatSearchString = "*"
    }
    chatCurrentPage = 1;
    ChatUpdateResultsView();
}

function ChatUpdatePagination(docCount) {
    var totalPages = Math.round(docCount / 10);
    // Set a max of 5 items and set the current page in middle of pages
    var startPage = currentPage;

    var maxPage = startPage + 5;
    if (totalPages < maxPage)
        maxPage = totalPages + 1;
    var backPage = parseInt(chatCurrentPage) - 1;
    if (backPage < 1)
        backPage = 1;
    var forwardPage = parseInt(chatCurrentPage) + 1;

    var htmlString = "";
    if (chatCurrentPage > 1) {
        htmlString = `<li><a href="javascript:void(0)" onclick="ChatGoToPage('${backPage}')" class="ms-Icon ms-Icon--ChevronLeftMed"></a></li>`;
    }

    htmlString += '<li class="active"><a href="#">' + currentPage + '</a></li>';

    if (chatCurrentPage <= totalPages) {
        htmlString += `<li><a href="javascript:void(0)" onclick="ChatGoToPage('${forwardPage}')" class="ms-Icon ms-Icon--ChevronRightMed"></a></li>`;
    }
    $("#pagination").html(htmlString);
    $("#chat-paginationFooter").html(htmlString);
}

function ChatGoToPage(page) {
    chatCurrentPage = page;
    ChatUpdateResultsView();
}