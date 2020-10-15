var chatContext = null;
var categoryTypeList = ["Abbreviation", "Component", "Equipment Class", "Manufacturer", "People", "Plant Code"];
var chatbotSocket = null;
var conversationId = null;

$(document).bind("click", function (event) {
    let categoryMenu = document.getElementById("category-rating-menu");
    if (categoryMenu.className == "show-contextmenu") {
        categoryMenu.className = "hide-contextmenu";
    }
});

function ShowCategoryContextMenu(event, category, value) {
    let contextmenu = document.getElementById('category-rating-menu');
    let resultHTML = '';
    for (let i = 0; i < categoryTypeList.length; i++) {
        let checkMatch = categoryTypeList[i].replace(/\s/g, '');
        if (!(category == checkMatch)) {
            resultHTML += `<div class="cm-row">
                <span class="cm-element" onclick="CategoryLabeling('${checkMatch}','${category}','${value}');">Mark as ${categoryTypeList[i]}</span>
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

function CategoryLabeling(category, currentCategory, value) {
    $.post('/api/feedback/annotateCategoryTags',
        {
            searchFbId: searchFbId,
            annotation: category,
            tag: value,
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
                                ChatHandleMultipleFacets(activities[i].channelData["Filters"].Entities)
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

function sendChatMessage() {
    if (chatbotSocket != null) {
        let message = $('#chatMessage').val();

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



function createMessage(chatSide, message) {
    let sideClass = "right-msg"
    let userImage = "https://image.flaticon.com/icons/svg/145/145867.svg"
    if (chatSide == "left") {
        sideClass = "left-msg"
        userImage = "https://image.flaticon.com/icons/svg/327/327779.svg"
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

function myTestFunction(event,id) {
    var x = $("#" + id).val();
    console.log(id);
    $("#" + id + " :selected").each(function () {
        var bc = $(this).val()
        $(this).click();
    });
}

function trialClick(event, text) {
    console.log(text)
    event.stopPropagation();
}
