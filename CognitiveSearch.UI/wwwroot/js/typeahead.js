function instantiateTypeAhead() {

    var availableTags = [
        "abb", "dresser", "dresser-rand", "ingersoll-rand", "rolls royce", "brook crompton", "brook hansen", "allweiler", "alfa laval", "alstom", "mitsubishi heavy industries",
        "hyundai heavy industries", "atlas copco", "siemens", "baker hughes", "baldor", "bauer", "bayley", "begemann", "blackmer", "blagdon", "bosch", "bran & luebbe", "brown & root",
        "bugatti", "burgess", "carrier", "caterpillar", "clyde union", "cooper energy services", "crompton", "cummins", "david brown", "dawson & downie", "demag delaval", "detroit diesel",
        "doosan", "dongyang", "donghwa", "dresser-roots", "ebara", "edwards", "elliot", "favco", "flakt", "flender", "flowserve", "flowguard", "fmc", "fuji", "gardner-denver", "ge",
        "nuovo pignone", "general motors", "goulds", "graffenstaden", "grundfos", "halberg", "halifax", "hamilton", "hansen", "harbour marine", "haskel", "hitachi", "hitachi heavy industries",
        "toshiba", "mitsubishi", "hoffman", "holtec", "honda", "howden", "hsl engineering", "hyundai", "ingersoll-dresser", "japan steel", "japan aircraft", "japan machinery company",
        "john deere", "kaercher", "kaeser", "kaji iron works", "kato", "kobelco", "kobe steel", "kone", "kruger", "krupp", "kubota", "lightnin", "linde", "loher", "lufkin", "milton roy",
        "man turbo", "mannesmann", "marelli motori", "marushichi", "mitsubishi electric company", "mitsui engineering", "mitsui", "monroe", "niigata", "nikkiso", "nissan diesel", "omron",
        "ohio electric", "patterson", "prominent", "randolph gear", "reliance", "rexroth", "rickmeier", "robbins & myers", "robert birkenbeul", "rockwell automation", "rohr system technik",
        "roots", "rotor", "rotork", "ruhr pompen", "saab", "saab iveco", "sandpiper", "sanwa", "scheerle", "schenck", "seat", "seika corporation", "shimadzu", "shin nippon", "showa denki",
        "solar turbines", "stamford", "stahl", "stork", "sullair", "sulzer", "sumitomo", "sunstrand", "taiko kikai", "teco", "thomassen", "thyssen", "thyssenkrupp", "tractor malaysia",
        "tuthill", "umw", "united centrifugal pump", "viking marine", "voith", "volvo", "volvo penta", "vogel", "warren rupp", "western electric", "weg", "weir", "westinghouse", "wilden",
        "wood", "winkelmann", "williams", "worthington", "yamada", "yokota", "ziehl", "papl", "pupl", "pdpl", "pcm", "pcino", "pc", "phco", "pcsb", "zlng", "pcg", "pcml", "abf", "pcfk",
        "pml", "mtbe", "pdh", "emsb", "pmsb", "pemsb", "kpsb", "amsb", "pasb", "pcldpe", "mlng", "pgb", "pegt", "pjh", "pptsb", "pmssb", "gdc", "klcch", "mrcsb", "gdcklia", "itp", "pmtsb",
        "supsb", "psi", "lisb", "psb", "petco", "ngv", "rgt", "klccp", "pcmc", "klccpm", "pttsb", "mpo", "dfp", "prsb", "klccph", "pflng1", "ppsb", "pl9sb", "voltaire", "rgt2", "rgt3",
        "pet-dgt", "pet-ict", "prpc", "gdcm", "gdch", "mksb", "klccreit", "icsb", "klconvention", "klccps", "ctsb", "hlsb", "klccuh", "pet", "vpsb", "pflng2", "ptssb", "pgtssb", "cefs",
        "plng2", "ttmmsb", "pic", "pcml-", "pcp", "epoms", "ptvsb", "hcu", "klcc", "tpc", "pcmi", "pcosb", "sda", "an200", "frame 5", "frame 6"
    ];
    let myString = "";
    for (let i = 0; i < availableTags.length; i++) {
        myString += `"${availableTags[i].toLowerCase()}",`
    }
    console.log(myString);
    function split(val) {
        return val.split(/ \s*/);
    }
    function extractLast(term) {
        return split(term).pop();
    }

    $("#chatMessage")
        .bind("keydown", function (event) {
            //can also use this to track when user presses SPACE
            if (event.which === 32 || event.which === 46 || event.which === 8)
                $('#chat-typeahead-container').html($(this).val());

            if (event.keyCode === $.ui.keyCode.TAB &&
                $(this).data("autocomplete").menu.active) {
                event.preventDefault();
            }
        })
        .autocomplete({
            minLength: 2,
            source: function (request, response) {
                let lastWord = extractLast(request.term);
                if (lastWord.length < 3) {
                    response([]);
                }
                else {
                    let results = $.ui.autocomplete.filter(
                        availableTags, lastWord)
                    response(results.slice(0, 10));
                }
            },
            focus: function () {
                return false;
            },
            select: function (event, ui) {
                var terms = split(this.value);
                terms.pop();
                terms.push(ui.item.value);
                terms.push("");
                this.value = terms.join(" ");
                //when item selected, add current value to span
                $('#chat-typeahead-container').html($(this).val());
                return false;
            },
            open: function (event, ui) {
                var span = $('#chat-typeahead-container');
                span.html($('#chatMessage').val());
                var width = span.width();
                if (width != 0) {
                    width > $('#chatMessage').width() ?
                        width = parseInt($('#chatMessage').position().left + $('#chatMessage').width()) :
                        width = parseInt($('#chatMessage').position().left + width);

                    $('.ui-autocomplete.ui-menu').css('left', width + 'px');
                    $('.ui-autocomplete.ui-menu').css('margin-left', '3%');
                }
            }
        });

}