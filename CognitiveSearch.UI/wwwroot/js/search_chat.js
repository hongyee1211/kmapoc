var chatSelectedFacets = [];
var tempSelectedFacets = [];
var chatSearchString = "";
var chatCurrentPage = 1;
var chatDocumentCount = 0;
var chatResults = [];
var chatFacets = [];
var chatTags;
var chatDocumentToken = "";
var chatSearchId;
var originalDocumentCount;

function ChatUpdateResultsView(searchString, disciplines) {
    // Get center of map to use to score the search results
    //Pass the polygon filter to the query: mapPolygon.data.geometry.coordinates[0][1]
    var polygonString = "";

    if (mapPolygon !== null && mapPolygon.data !== null && mapPolygon.data.geometry !== null && mapPolygon.data.geometry.coordinates !== null) {
        var pointArray = mapPolygon.data.geometry.coordinates[0];

        for (var i = 0; i < pointArray.length; i++) {
            if (polygonString.length > 0) { polygonString += ","; }

            polygonString += pointArray[i][0] + " " + pointArray[i][1];
        }
    }

    $.post('/home/searchview',
        {
            q: searchString,
            searchFacets: chatSelectedFacets,
            currentPage: chatCurrentPage,
            polygonString: polygonString,
            disciplines: disciplines
        },
        function (viewModel) {
            ChatUpdate(viewModel);
        });
}

function ChatUpdate(viewModel) {

    // Update UI controls to match view model incase we came from a direct link
    chatSelectedFacets = viewModel.selectedFacets;
    searchFbId = viewModel.searchFbId;
    let subscribed = viewModel.subscribed;

    var data = viewModel.documentResult;
    var standards = viewModel.standards;
    documentCount = data.count;
    originalDocumentCount = viewModel.originalCount;
    results = data.results;
    chatFacets = data.facets;
    tags = data.tags;
    token = data.token;
    searchId = data.searchId;

    //Facets
    ChatUpdateFacets();

    //Results List
    ChatUpdateResults(data, "#chat-doc-count","#chat-doc-details-div");

    //// Standards List
    ChatUpdateStandards(standards);

    ////Map
    //ChatUpdateMap(data);

    ////Pagination
    ChatUpdatePagination(data.count);

    //// Log Search Events
    //ChatLogSearchAnalytics(data.count);

    //Filters
    ChatUpdateFilterReset();

    let container = $("#chat-search-container")
    if (container.hasClass("hide")) {
        container.removeClass("hide");
        container.addClass("show");
    }
    //let subscribe = $("#subscribe-cb-container")
    //if (subscribe.hasClass("hide")) {
    //    subscribe.removeClass("hide");
    //    subscribe.addClass("show");
    //}
    //$('#search-subscribe-cb').prop('checked', subscribed);
    $('html, body').animate({ scrollTop: 1000 }, 'fast');
}

function ChatTriggerSearch(page = 1) {
    chatCurrentPage = page;

    let search = "";
    let keys = Object.keys(filterSelected);
    let disciplines = []
    for (let i = 0; i < keys.length; i++) {
        let key = keys[i]
        let values = filterSelected[key]
        if (key == "Discipline") {
            disciplines = values;
        }
        else if (key == "Model" && untrackedFilterSelected["ModelGroup"] != null && untrackedFilterSelected["ModelGroup"].length > 0) {
            for (let j = 0; j < untrackedFilterSelected["ModelGroup"].length; j++) {
                search += `,"${untrackedFilterSelected["ModelGroup"][j]}"`;
            }
            if (values.includes("AN200")) {
                search += `,AN200`;
            }
        }
        else {
            for (let j = 0; j < values.length; j++) {
                search += `,"${values[j]}"`;
            }
        }
    }

    //search += ", " + RemoveStopWords(lastChatMessage);
    chatSearchString = search

    ChatUpdateResultsView(search, disciplines);
    SearchEntities(search)
}

var stopWords = [
    `a`,
    `about`,
    `above`,
    `after`,
    `again`,
    `against`,
    `all`,
    `am`,
    `an`,
    `and`,
    `any`,
    `are`,
    `aren't`,
    `as`,
    `at`,
    `be`,
    `because`,
    `been`,
    `before`,
    `being`,
    `below`,
    `between`,
    `both`,
    `but`,
    `by`,
    `can't`,
    `cannot`,
    `could`,
    `couldn't`,
    `did`,
    `didn't`,
    `do`,
    `does`,
    `doesn't`,
    `doing`,
    `don't`,
    `down`,
    `during`,
    `each`,
    `few`,
    `for`,
    `from`,
    `further`,
    `had`,
    `hadn't`,
    `has`,
    `hasn't`,
    `have`,
    `haven't`,
    `having`,
    `he`,
    `he'd`,
    `he'll`,
    `he's`,
    `her`,
    `here`,
    `here's`,
    `hers`,
    `herself`,
    `him`,
    `himself`,
    `his`,
    `how`,
    `how's`,
    `give`,
    `i`,
    `i'd`,
    `i'll`,
    `i'm`,
    `i've`,
    `if`,
    `in`,
`    into`,
    `is`,
    `isn't`,
    `it`,
    `it's`,
    `its`,
    `itself`,
    `let's`,
    `me`,
    `more`,
    `most`,
    `mustn't`,
    `my`,
    `myself`,
    `no`,
    `nor`,
    `not`,
    `of`,
    `off`,
    `on`,
    `once`,
    `only`,
    `or`,
    `other`,
    `ought`,
    `our`,
    `ours`,
    `ourselves`,
    `out`,
    `over`,
    `own`,
    `same`,
    `shan't`,
    `she`,
    `she'd`,
    `she'll`,
    `she's`,
    `should`,
    `shouldn't`,
    `so`,
    `some`,
    `such`,
    `than`,
    `that`,
    `that's`,
    `the`,
    `their`,
    `theirs`,
    `them`,
    `themselves`,
    `then`,
    `there`,
    `there's`,
    `these`,
    `they`,
    `they'd`,
    `they'll`,
    `they're`,
    `they've`,
    `this`,
    `those`,
    `through`,
    `to`,
    `too`,
    `under`,
    `until`,
    `up`,
    `very`,
    `was`,
    `wasn't`,
    `we`,
    `we'd`,
    `we'll`,
    `we're`,
    `we've`,
    `were`,
    `weren't`,
    `what`,
    `what's`,
    `when`,
    `when's`,
    `where`,
    `where's`,
    `which`,
    `while`,
    `who`,
    `who's`,
    `whom`,
    `why`,
    `why's`,
    `with`,
    `won't`,
    `would`,
    `wouldn't`,
    `you`,
    `you'd`,
    `you'll`,
    `you're`,
    `you've`,
    `your`,
    `yours`,
    `yourself`,
    `yourselves`]

function RemoveStopWords(str) {
    let res = []
    let words = str.split(' ')
    for (i = 0; i < words.length; i++) {
        let word_clean = words[i].split(".").join("")
        if (!stopWords.includes(word_clean)) {
            res.push(word_clean)
        }
    }
    return (res.join(' '))
}