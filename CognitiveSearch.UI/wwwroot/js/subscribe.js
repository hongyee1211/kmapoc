function HandleSubscribe(event) {
    let checkbox = event.target
    let subscribeAction = checkbox.checked
    let query = ""
    if (isChat) {
        query = chatSearchString;
    }
    else {
        query = q;
    }
    if (subscribeAction == true) {
        AddSubscription(query, originalDocumentCount)
    }
    else {
        RemoveSubscription(query)
    }
}

function RemoveSubscription(query) {
    $.post('/subscribe/unsubscribe',
        {
            query: query
        },
        function (data) {
            subscribed = false
            $('#search-subscribe-cb').prop('checked', subscribed);
        });
}

function AddSubscription(query, count) {
    $.post('/subscribe/subscribe',
        {
            query: query,
            count: count
        },
        function (data) {
            subscribed = true
            $('#search-subscribe-cb').prop('checked', subscribed);
        });
}