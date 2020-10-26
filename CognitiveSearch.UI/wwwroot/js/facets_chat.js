//Filters
function ChatUpdateFilterReset2() {
    // This allows users to remove filters
    var htmlString = '';
    let newFacets = tempSelectedFacets.concat(chatSelectedFacets);

    if (newFacets && newFacets.length > 0) {
        newFacets.forEach(function (item, index, array) { // foreach facet with a selected value
            if (item.value && item.value.length > 0) {
                var name = item.key;
                $(`#select-${name}`).selectpicker('val', item.value);
            }
        });
    }
}

function ChatUpdateFilterReset() {
    // This allows users to remove filters
    var htmlString = '';
    $("#chatFilterReset").html("");

    let newFacets = chatSelectedFacets;

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
                                            <div class="chat-facet-thumbs-up menuThumbsDown" style="margin: 7px 3px 7px 7px;" onclick="ShowCategoryContextMenu(event,'${name}','${data[j].value}', '${title}');"></div>
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

// Facets
function ChatUpdateFacets2() {
    $("#chat-filters").html("");

    var facetResultsHTML = ''
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
                title = "OPU";
            } else if (name == "Component") {
                cssClass = "Component";
            } else if (name == "FailureMode") {
                cssClass = "FailureMode";
            }

            if (cssClass != "others") {

                facetResultsHTML += `<div class="filter-category-container" oncontextmenu="trialContext(event)">
                <h6 style="color:#ffffff">${title}</h6>
                <select onchange="myTestFunction(event,'select-${name}')" id="select-${name}" oncontextmenu="trialContext(event)" class="filter-select" data-live-search="true" multiple data-selected-text-format="count > 2" title="Filter by...">`

                if (data !== null) {
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].value.toString().length < 100) {
                            var idName = name + "_" + j;
                            facetResultsHTML += `<option onclick="trialClick(event,'hey')" data-subtext="(${data[j].count})" data-content="-> ${data[j].value}">${data[j].value}</option>`
                        }
                    }
                }

                facetResultsHTML += `</select>
                            </div>`
            }
        }
    });
    $("#chat-filters").append(facetResultsHTML);
    $('.filter-select').selectpicker();
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

//function ChatHandleMultipleFacets(filters) {
//    let keys = Object.keys(filters);
//    chatSearchString = "";
//    for (let i = 0; i < keys.length; i++) {
//        let key = keys[i]
//        let tempKey = keys[i]
//        var result = chatSelectedFacets.find(function (f) { return f.key === tempKey; });
//        if (filters[key].length <= 0) {
//            //ignore
//        }
//        else if (result) {
//            if (result.value == null) {
//                result.value = [];
//            }
//            result.value = result.value.concat(filters[key].filter((item) => result.value.indexOf(item) < 0))
//            for (let j = 0; j < filters[key].length; j++) {
//                chatSearchString += ",\"" + filters[key][j] + "\""
//            }
//        }
//        else {
//            chatSelectedFacets.push({
//                key: tempKey,
//                value: filters[key]
//            })
//            chatSearchString += ",\"" + filters[key] + "\""
//        }
//    }
//    chatCurrentPage = 1;
//    ChatUpdateResultsView();
//}

function ChatHandleChannelData(filters) {
    let keys = Object.keys(filters);
    for (let i = 0; i < keys.length; i++) {
        let key = keys[i]
        if (filterSelected.hasOwnProperty(key)) {
            if (key == "EquipmentClass" || key == "PlantCode") {
                if (filters[key].length > 0) {
                    filterSelected[key] = [filters[key][filters[key].length - 1]]
                }
            }
            filterSelected[key] = filterSelected[key].concat(filters[key].filter((item) => filterSelected[key].indexOf(item) < 0))
        }
        else if (key == "ModelGroup") {
            parentList = parentsFilters[key];
            parentKeys = Object.keys(parentList);
            for (let j = 0; j < parentKeys.length; j++) {
                if (filters[key].includes(parentKeys[j])) {
                    if (parentList[parentKeys[j]].hasOwnProperty(filterSelected["PlantCode"][0]))
                    {
                        filterSelected["Model"] = parentList[parentKeys[j]][filterSelected["PlantCode"][0]].filter((item) => filterSelected["Model"].indexOf(item) < 0)
                    }
                }
            }
            untrackedFilterSelected[key] = filters[key];
        }
        else {
            if (untrackedFilterSelected[key] != null) {
                untrackedFilterSelected[key] = untrackedFilterSelected[key].concat(filters[key].filter((item) => untrackedFilterSelected[key].indexOf(item) < 0))
            }
            else {
                untrackedFilterSelected[key] = filters[key];
            }
        }
    }

    QueryGraph(filterSelected.PlantCode, filterSelected.Model, filterSelected.EquipmentClass, filterSelected.Manufacturer, function (data) {
        //let tree = { tree: data[0] };
        treeBoxes(data)
    });

    //if (userMessage.match(/give me all pumps manufactured by nuovo pignone at mlng/g)) {
    //    $.getJSON("../json/testing.json", function (json) {
    //        treeBoxes(json);
    //    });
    //} else if (userMessage.match(/give me the same for MLNG/g)) {
    //    $.getJSON("../json/Sample_Data-MLNG.json", function (json) {
    //        treeBoxes(json.tree);
    //    });
    //} else if (userMessage.match(/give me the failure mode for these pumps/g)) {
    //    $.getJSON("../json/Sample_Data-MLNG.json", function (json) {
    //        treeBoxes(json.tree);
    //    });
    //}
    ChatUpdateGraphFilter();
}

function ChatUpdateGraphFilter() {
    let keys = Object.keys(filterSelected);

    if (keys && keys.length > 0) {
        for (let i = 0; i < keys.length; i++) {
            $(`#select-${keys[i]}`).selectpicker('val', filterSelected[keys[i]]);
        }
    }
}