var chatContext = null;
var categoryTypeList = ["Abbreviation", "Component", "Equipment Class", "FMEA", "Manufacturer", "Plant Code", "Standards"];

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
