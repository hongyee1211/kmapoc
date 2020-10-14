currentHighlightedText = ""
function ShowAnnotationContextMenu(event) {
    let contextmenu = document.getElementById("annotation-menu")
    selection = window.getSelection().toString();
    let regex = /(\.\s*\n\.)/g
    console.log(selection);
    if (selection != null && selection != "" && !selection.match(regex)) {
        contextmenu.className = "show-contextmenu";
        contextmenu.style.top = mouseY(event) + 'px';
        contextmenu.style.left = mouseX(event) + 'px';
        currentHighlightedText = selection;
        event.preventDefault();
    }
}

function mouseX(evt) {
    if (evt.pageX) {
        return evt.pageX;
    } else if (evt.clientX) {
        return evt.clientX + (document.documentElement.scrollLeft ?
            document.documentElement.scrollLeft :
            document.body.scrollLeft);
    } else {
        return null;
    }
}

function mouseY(evt) {
    if (evt.pageY) {
        return evt.pageY;
    } else if (evt.clientY) {
        return evt.clientY + (document.documentElement.scrollTop ?
            document.documentElement.scrollTop :
            document.body.scrollTop);
    } else {
        return null;
    }
}

function AnnotateSelection(category) {
    console.log('annotate')
    selection = window.getSelection().toString();
    $.post('/api/feedback/annotateTags',
        {
            searchFbId: searchFbId,
            documentName: currentSelectedPDF,
            annotation: category,
            tag: currentHighlightedText
        },
        function (data) {
            //do nothing
        }
    )
}