var chatSelectedFacets = [];
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

    $.post('/home/searchview',
        {
            q: "*",
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

    let container = $("#chat-search-container")
    if (container.hasClass("hide")) {
        container.removeClass("hide");
        container.addClass("show");
    }
    $('html, body').animate({ scrollTop: 0 }, 'fast');
}

function ChatTriggerSearch() {
    currentPage = 1;
    ChatUpdateResultsView();
}