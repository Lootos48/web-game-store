function download(key) {
    let url = "/Game/download/" + key;
    window.open(url, '_blank');
}

function setupCurrentPageValue() {
    if (document.filterForm.ItemsPerPage.value == 0) {
        document.filterForm.CurrentPage.value = 0;
    } else {
        document.filterForm.CurrentPage.value = 1;
    }
}

function setupChosedLocalization(localizationId) {
    let element = document.getElementById("ChosedLocalization");
    element.value = localizationId;
}

// что нужно сделать: получаю айди выбранного жанра -> получаю все спаны с этим айдишником (который в таком случае является родительским)
// -> в цикле достаю имя детского жанра -> получаю чекбокс по имени (имя это айди чекбокса) -> ставлю чекбокс = чекед (или убираю, если уже было выбрано)

function showDatePicker(selectObject) {
    let status = selectObject;

    if (status == 4) {
        showShippedDateEditBlock();
        hideExpirationDateEditBlock();
    }
    else if (status == 2) {
        showExpirationDateEditBlock();
        hideShippedDateEditBlock();
    }
    else {
        hideShippedDateEditBlock();
        hideExpirationDateEditBlock();
    }
}

function hideExpirationDateEditBlock() {
    let editBlock1 = document.getElementById("expDateEditBlock");
    editBlock1.setAttribute("hidden", "true");

    let datapicker1 = document.getElementById("expDatePicker");
    datapicker1.setAttribute("disabled", "true");
}

function hideShippedDateEditBlock() {
    let editBlock = document.getElementById("shippedDateEditBlock");
    editBlock.setAttribute("hidden", "true");

    let datapicker = document.getElementById("shippedDatePicker");
    datapicker.setAttribute("disabled", "true");
}

function showShippedDateEditBlock() {
    let editBlock = document.getElementById("shippedDateEditBlock");
    editBlock.removeAttribute("hidden");

    let datapicker = document.getElementById("shippedDatePicker");
    datapicker.removeAttribute("disabled");
}

function showExpirationDateEditBlock() {
    let editBlock = document.getElementById("expDateEditBlock");
    editBlock.removeAttribute("hidden");

    let datapicker = document.getElementById("expDatePicker");
    datapicker.removeAttribute("disabled");
}