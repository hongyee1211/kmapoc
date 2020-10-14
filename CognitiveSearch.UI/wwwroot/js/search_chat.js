var chatSelectedFacets = [];
var tempSelectedFacets = [];
var chatSearchString = "*";
var chatCurrentPage = 1;
var chatDocumentCount = 0;
var chatResults = [];
var chatFacets = [];
var chatTags;
var chatDocumentToken = "";
var chatSearchId;

function ChatUpdateResultsView() {
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

    chatSearchString = ""
    for (let i = 0; i < tempSelectedFacets.length; i++) {
        for (let j = 0; j < tempSelectedFacets[i].value.length; j++) {
            chatSearchString += ",\"" + tempSelectedFacets[i].value[j] + "\""
        }
    }
    if (chatSearchString == "") {
        chatSearchString = "*"
    }

    $.post('/home/searchview',
        {
            q: chatSearchString,
            searchFacets: chatSelectedFacets,
            currentPage: chatCurrentPage,
            polygonString: polygonString
        },
        function (viewModel) {
            ChatUpdate(viewModel);
        });
}

function ChatUpdate(viewModel) {

    // Update UI controls to match view model incase we came from a direct link
    chatSelectedFacets = viewModel.selectedFacets;
    searchFbId = viewModel.searchFbId;

    var data = viewModel.documentResult;
    var standards = viewModel.standards;
    documentCount = data.count;
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

    //Update Graph
    var selectedData = "";
    var message = "";
    var name = "chatMessage=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            message = c.substring(name.length, c.length);
        }
    }

    if (message.match(/give me all pumps manufactured by nuovo pignone at pgb/g)) {
        $.getJSON("../json/Sample_Data-PGB.json", function (json) {
            treeBoxes('', json.tree);
        });
    } else if (message.match(/give me the same for MLNG/g)) {
        $.getJSON("../json/Sample_Data-MLNG.json", function (json) {
            treeBoxes('', json.tree);
        });
    } else if (message.match(/give me the failure mode for these pumps/g)) {
        $.getJSON("../json/Sample_Data-MLNG.json", function (json) {
            treeBoxes('', json.tree);
        });
    }

    // 
    let container = $("#chat-search-container")
    if (container.hasClass("hide")) {
        container.removeClass("hide");
        container.addClass("show");
    }
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}

function ChatTriggerSearch() {
    chatCurrentPage = 1;
    ChatUpdateResultsView();
}