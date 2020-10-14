function HandleSubscribe(event) {
    let checkbox = event.target
    let subscribeAction = checkbox.checked
    if (subscribeAction == true) {
        AddSubscription(q, documentCount)
    }
    else {
        RemoveSubscription(q)
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