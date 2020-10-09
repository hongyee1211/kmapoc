var chatContext = null;
function sendChat() {
    let msg = $("#chatMessage").val();
    $.ajax({
        type: "POST",
        url: "chat/send",
        data: JSON.stringify({
            text: msg,
            context: chatContext
        }),
        contentType: "application/json",
        complete: function (data) {
            console.log(data.responseJSON.text);
            $('#chat-reply').text(data.responseJSON.text);
            chatContext = data.responseJSON.context;
            let equipments = data.responseJSON.info.EquipmentClass;
            var listHtml = ''
            for (let i = 0; i < equipments.length; i++) {
                let equipment = equipments[i];
                listHtml += `<span style="margin:3px" class="label label-info">${equipment}</span>`
            }
            $("#chat-equipment-class-filter-list").html(listHtml);
        }
    });
}