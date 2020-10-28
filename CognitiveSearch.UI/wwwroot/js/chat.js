var chatContext = null;
var categoryTypeList = ["Abbreviation", "Component", "Equipment Class", "Manufacturer", "People", "Plant Code","Failure Mode"];
var chatbotSocket = null;
var conversationId = null;

$(document).bind("click", function (event) {
    let categoryMenu = document.getElementById("category-rating-menu");
    if (categoryMenu.className == "show-contextmenu") {
        categoryMenu.className = "hide-contextmenu";
    }
});

function ShowCategoryContextMenu(event, category, value, current) {
    let contextmenu = document.getElementById('category-rating-menu');
    let resultHTML = '';
    for (let i = 0; i < categoryTypeList.length; i++) {
        let checkMatch = categoryTypeList[i].replace(/\s/g, '');
        if (!(category.toLowerCase() == checkMatch.toLowerCase())) {
            resultHTML += `<div class="cm-row">
                <span class="cm-element" onclick="CategoryLabeling('${checkMatch}','${category}','${value}','${current}');">Mark as ${categoryTypeList[i]}</span>
            </div>`
        }
    }
    contextmenu.style.bottom = ''
    contextmenu.style.right = ''
    contextmenu.style.top = ''
    contextmenu.style.left = ''

    let height = mouseY(event);
    //let calculatedHeight = screen.height - height;
    //if (calculatedHeight <= 300) {
    //    contextmenu.style.bottom = calculatedHeight + 'px';
    //}
    //else {
    contextmenu.style.top = height + 'px';
    //}
    let width = mouseX(event);
    let calculatedWidth = screen.width - width;
    if (calculatedWidth <= 150) {
        contextmenu.style.right = calculatedWidth + 'px';
    }
    else {
        contextmenu.style.left = width + 'px';
    }


    contextmenu.innerHTML = resultHTML;
    contextmenu.className = "show-contextmenu";
    event.stopPropagation();
    monitorCategoryTags(category, value, 2);
}

function CategoryLabeling(category, currentCategory, value, current) {
    $.post('/api/feedback/annotateCategoryTags',
        {
            searchFbId: searchFbId,
            annotation: category,
            tag: value,
            current: current
        },
        function (data) {
            //do nothing
        }
    )
}

function ConnectToChatBot() {
    $.ajax({
        url: 'https://directline.botframework.com/v3/directline/conversations',
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'Authorization': 'Bearer ' + chatToken
        },
        success: function (result) {
            chatbotSocket = new WebSocket(result.streamUrl);
            conversationId = result.conversationId;
            chatbotSocket.onmessage = function (event) {
                if (event.data != "") {
                    let activities = JSON.parse(event.data).activities;
                    for (i = 0; i < activities.length; i++) {
                        if (activities[i].from.id != chatUserId) {
                            createMessage("left", activities[i].text)
                            if (activities[i].channelData != null) {
                                ChatHandleChannelData(activities[i].channelData["Filters"].Entities)
                            }
                        }
                    }
                }
                else {
                    console.log(event);
                }
            }
        },
        error: function (error) {
            console.log(error)
        }
    });
}

var lastChatMessage = ""
function sendChatMessage() {
    if (chatbotSocket != null) {
        let message = $('#chatMessage').val();
        lastChatMessage = message;
        // tmp set cookies
        var d = new Date();
        d.setTime(d.getTime() + (1 * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = "chatMessage=" + message + ";" + expires + ";path=/";


        createMessage("right", message);
        $('#chatMessage').val('');
        $.ajax({
            url: 'https://directline.botframework.com/v3/directline/conversations/' + conversationId + '/activities',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                "locale": "en-EN",
                "type": "message",
                "from": {
                    "id": chatUserId
                },
                "text": message,
            }),
            headers: {
                'Authorization': 'Bearer ' + chatToken
            },
            success: function (result) {
                console.log(result)
            },
            error: function (error) {
                console.log(error)
            }
        });
    }
}


var userMessage = ""
function createMessage(chatSide, message) {
    let sideClass = "right-msg"
    let userImage = "https://image.flaticon.com/icons/svg/145/145867.svg"
    if (chatSide == "left") {
        sideClass = "left-msg"
        userImage = "https://image.flaticon.com/icons/svg/327/327779.svg"
    }
    else {
        userMessage = message
    }

    let chatHTML = `<div class="msg ${sideClass}">
                                <div class="msg-img"
                                     style="background-image: url(${userImage})"></div>

                                <div class="msg-bubble">
                                    <div class="msg-text">
                                        ${message}
                                    </div>
                                </div>
                            </div>`
    let chatArea = $("#msger-chat-area");
    chatArea.append(chatHTML)
    //chatArea.animate({ scrollTop: chatArea.scrollHeight }, 'slow');

    //jquery scroll height not working as expected
    var objDiv = document.getElementById("msger-chat-area");
    objDiv.scrollTop = objDiv.scrollHeight;
}


function ChatUpdateGraphFilters(test) {
    $("#chat-filters").html("");

    var facetResultsHTML = ''
    let classKeys = Object.keys(chatGraphFilters);
    for (let i = 0; i < classKeys.length; i++) {
        let name = classKeys[i];
        let data = chatGraphFilters[classKeys[i]];
        if (data !== null && data.length > 0) {
            var title = name.replace(/([A-Z])/g, ' $1').replace(/^./, function (str) { return str.toUpperCase(); })
            var cssClass = "others";
            var multiple = "multiple";
            if (name == "EquipmentClass") {
                cssClass = "EquipmentClass";
                multiple = `multiple data-max-options="1"`;
            } else if (name == "Manufacturer") {
                cssClass = "Manufacturer";
            } else if (name == "PlantCode") {
                cssClass = "PlantCode";
                title = "OPU";
                multiple = `multiple data-max-options="1"`;
            } else if (name == "Model") {
                cssClass = "Model";
            } else if (name == "Discipline") {
                cssClass = "Discipline";
            }

            if (cssClass != "others") {

                facetResultsHTML += `<div class="filter-category-container" oncontextmenu="trialContext(event)">
                <select onchange="filterSelectionChanged(event,'${name}')" id="select-${name}" oncontextmenu="trialContext(event)" class="filter-select" data-live-search="true" ${multiple} data-selected-text-format="static" title="${title}">`

                if (data !== null) {
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].value.toString().length < 100) {
                            if (data[j].childLevel == 0) {
                                facetResultsHTML += `<option>${data[j].value}</option>`
                            }
                            else {
                                facetResultsHTML += `<option style="margin-left: 15px;">${data[j].value}</option>`
                            }
                        }
                    }
                }

                facetResultsHTML += `</select>
                            </div>`
            }
        }
    }
    $("#chat-filters").append(facetResultsHTML);
    $('.filter-select').selectpicker();
}

untrackedFilterSelected = {};
filterSelected = {
    "PlantCode": [],
    "Manufacturer": [],
    "EquipmentClass": [],
    "Model": [],
    "Discipline": []
}
parentsFilters = {
    "EquipmentClass": {},
    "FailureMode": {},
    "Manufacturer": {"GE": ["NUOVO PIGNONE"]},
    "Component": {},
    "PlantCode": {},
    "Model": {},
    "Discipline": {},
    "ModelGroup": {
        "Frame 5": {
            "MRCSB": ["GTG-59101", "GTG-59201", "GTG-59301", "GTG-59401", "GTG-59601"],
            "MLNG": ["GT4010", "GT4020", "GT-94010", "GT-94020",
                "GT-94030", "GT-94040", "GT-94050", "40-QRA-600", "40-QRA-700", "9GT940910", "9GT940920", "9GT940930", "9GT940940",
                "9GT940950"],
            },
        "Frame 6": {
            "PGB": ["G1T-211B01", "G1T-211C01", "G1T-211D01", "G1T-211E01",
                    "G1T-211F01", "G1T-211G01", "G1T-211A01", "G2T-211A01", "G2T-211B01", "G2T-211C01", "G2T-211E01"],
            "MLNG":["4KG-2440", "5KG-2440", "6KG-2440"]
        }
    }
}
function filterSelectionChanged(event, key) {
    let selectionDropdown = $("#select-" + key)
    let parentList = parentsFilters[key];
    let parentKeys = Object.keys(parentList);
    let currentVal = selectionDropdown.val();
    if (key == "EquipmentClass" || key == "PlantCode") {
        filterSelected[key] = currentVal;
    }
    else {
        let newlyMarked = currentVal.filter(value => !filterSelected[key].includes(value))
        for (let i = 0; i < newlyMarked.length; i++) {
            if (parentKeys.includes(newlyMarked[i])) {
                let values = parentList[newlyMarked[i]]
                currentVal = currentVal.concat(values);
            }
        }
        selectionDropdown.selectpicker('val', currentVal);
        filterSelected[key] = currentVal;
    }

    QueryGraph(filterSelected.PlantCode, filterSelected.Model, filterSelected.EquipmentClass, filterSelected.Manufacturer, function (data) {
        treeBoxes(data)
    });
}

function setInitialDiscipline(discipline) {
    let selectionDropdown = $("#select-Discipline")
    selectionDropdown.selectpicker('val', [discipline]);
    filterSelected["Discipline"] = [discipline];
}
