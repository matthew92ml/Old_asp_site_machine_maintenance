function SelectStationId(deletelink, lang) {
    var modalTitle = { "it": "Eliminazione della stazione", "en": "Station deleting" };
    document.getElementById("modal_title_delete").innerHTML = modalTitle[lang];

    var modalText = { "it": "Confermi l\'eliminazione della stazione selezionata?", "en": "Are you sure to delete selected station?" };
    document.getElementById("modal_text_delete").innerHTML = modalText[lang];

    var linkText = { "it": "Conferma", "en": "Submit" };
    document.getElementById("link_modal_delete").innerHTML = linkText[lang];
    document.getElementById("link_modal_delete").href = deletelink;
}