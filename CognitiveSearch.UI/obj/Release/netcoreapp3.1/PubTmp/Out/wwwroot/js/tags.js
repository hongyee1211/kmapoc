﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

function GetTagsHTML(result) {
    var title;
    if (result["metadata_storage_name"] !== undefined) {
        title = result.metadata_storage_name;
    }
    var tagsHTML = "";

    var i = 0;

    if (tags[0].key == "people") [tags[0], tags[4]] = [tags[4], tags[0]];
    
    tags.forEach(function (item, index, array) {
        var name = item.key;
        var dedupedEntities = [];

        if (Array.isArray(result[name])) {
            result[name].forEach(function (tagValue, i, tagArray) {
                if (i <= 10) {
                    if ($.inArray(tagValue, dedupedEntities) === -1) { //! in array
                        dedupedEntities.push(tagValue);
                            if (tagValue.length > 30) { // check tag name length
                                // create substring of tag name length if too long
                                tagValue = tagValue.substring(0, 30);
                            }

                            switch (tagValue.toLowerCase()) {
                                case "appendix":
                                case "untuk dalaman":
                                case "c. the":
                                case "lo":
                                case "max":
                                case "ij":
                                case "categoris":
                                case "v. gtg":
                                case "ordine":
                                case "commessa":
                                case "cliente":
                                case "commesso":
                                case "cogen":
                                case "train":
                                case "solar":
                                case "bunga kertas":
                                case "bekok":
                                case "baronia":
                                case "inlet":
                                case "f. macroscopic":
                                case "dia":
                                case "amine":
                                case "doc":
                                case "monitor":
                                case "disabled comm":
                                case "achieved sil":
                                case "rotation":
                                case "pitch":
                                case "wall":
                                case "c. equipment":
                                case "max switch":
                                case "a. perimiter":
                                case "grade c.":
                                case "micropack":
                                case "fac":
                                case "major":
                                case "reboiler":
                                case "unclassified":
                                case "pam":
                                case "a. bill":
                                case "b. contractor":
                                case "c. contractor":
                                case "d. contractor":
                                case "e. contractor":
                                case "f. contractor":
                                case "g. contractor":
                                case "k. contractor":
                                case "target sil":
                                case "average normal max":
                                case "norm":
                                case "norm.max":
                                case "aux boiler":
                                case "period":
                                case "page":
                                case "analsis":
                                    break;
                                default:
                                    tagsHTML += `<button class="tag tag-${name}" onclick="HighlightTag(event,'${title}')">${tagValue}</button>`;
                            }
                            i++;
                        }
                }
            });
        }
    });

    return tagsHTML;
}

function HighlightTag(tag, title = null) {
    var searchText = $(event.target).text();

    if ($(event.target).parents('#tags-panel').length) {
        GetReferences(searchText, false);
    }
    else {
        if (title != null) {
            MonitorSearchViaDocumentTag(searchText, title)
        }
        event.stopPropagation();

        query = $('#q').val();

        if (query === "*") {

            // Remove the "*" if we are adding some terms to the query
            query = `"${searchText}"`;
        }
        else {
            query = query + ` "${searchText}"`;
        }

        $('#q').val(query);
        Search();
    }
}

function GetReferences(searchText, allowMultiple, monitor = true) {
    if (monitor) {
        MonitorDocumentSearch(searchText)
    }

    var transcriptText;

    if (!allowMultiple) {
        $('#reference-viewer').html("");
        transcriptText = $('#transcript-viewer-pre').text();
    }
    else {
        transcriptText = $('#transcript-viewer-pre').html();
    }

    // find all matches in transcript
    var regex = new RegExp(searchText, 'gi');

    var i = -1;
    var response = transcriptText.replace(regex, function (str) {
        i++;
        var shortname = str.slice(0, 20).replace(/[^a-zA-Z ]/g, " ").replace(new RegExp(" ", 'g'), "_");
        return `<span id='${i}_${shortname}' class="highlight">${str}</span>`;
    })

    $('#transcript-viewer-pre').html(response);

    // for each match, select prev 50 and following 50 characters and add selections to list
    var transcriptCopy = transcriptText;

    // Calc height of reference viewer
    var contentHeight = $('.ms-Pivot-content').innerHeight();
    var tagViewerHeight = $('#tag-viewer').innerHeight();
    var detailsViewerHeight = $('#details-viewer').innerHeight();
    var askViewerHeight = $('#ask-row').innerHeight();

    $('#reference-viewer').css("height", contentHeight - tagViewerHeight - detailsViewerHeight - askViewerHeight - 110)


    $.each(transcriptCopy.match(regex), function (index, value) {

        var startIdx;
        var ln = 400;

        if (value.length > 150) {
            startIdx = transcriptCopy.indexOf(value);
            ln = value.length;
        }
        else {
            if (transcriptCopy.indexOf(value) < 200) {
                startIdx = 0;
            }
            else {
                startIdx = transcriptCopy.indexOf(value) - 200;
            }

            ln = 400 + value.length;
        }

        var reference = transcriptCopy.substr(startIdx, ln);
        transcriptCopy = transcriptCopy.replace(value, "");

        reference = reference.replace(value, function (str) {
            return `<span class="highlight">${str}</span>`;
        });

        var shortName = value.slice(0, 20).replace(/[^a-zA-Z ]/g, " ").replace(new RegExp(" ", 'g'), "_");
        $('#reference-viewer').append(`<li class='reference list-group-item' onclick='GoToReference("${index}_${shortName}")'>...${reference}...</li>`);
    });
}
async function MonitorDocumentSearch(query) {
    $.post('/home/monitordocumentquery',
        {
            searchFbId: searchFbId,
            documentName: currentSelectedPDF,
            query:query
        },
        function (data) {
            //do nothing
        }
    )
}

async function MonitorSearchViaDocumentTag(query, title) {
    $.post('/home/monitordocumentresulttags',
        {
            searchFbId: searchFbId,
            documentName: title,
            tag: query
        },
        function (data) {
            //do nothing
        }
    )
}

function GoToReference(selector) {
    // show transcript
    $('#file-pivot-link').removeClass('is-selected');
    $('#letters-pivot-link').removeClass('is-selected');
    $('#transcript-pivot-link').addClass('is-selected');

    $('#file-pivot').css('display', 'none');
    $('#letters-pivot').css('display', 'none');
    $('#transcript-pivot').css('display', 'block');

    var container = $('#transcript-viewer');
    var scrollTo = $("#" + selector);

    container.animate({
        scrollTop: scrollTo.offset().top - container.offset().top + container.scrollTop()
    });
}