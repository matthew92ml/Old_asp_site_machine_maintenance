function CheckTagCode(lang) {
    var errorTagEmpy = { "it": "Inserire il codice TAG", "en": "Enter TAG code" };

    var alertInfo = document.getElementById("panel_error");

    var tagCode = document.getElementById("input_tag_code");

    if (tagCode.value == "") {
        alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagEmpy[lang]);
        tagCode.focus();

        return false;
    }

    return true;
}

function AssignTagToCity(cityid, lang) {
    document.getElementById("image_loading").style.display = "";

    var tagCode = document.getElementById("input_tag_code");

    ServiceCity.CS_AssignTagToCity(tagCode.value, cityid, lang, OnAssignTagToCity, OnRequestFailed);
}

function OnAssignTagToCity(results) {
    document.getElementById("image_loading").style.display = "none";

    var values = results.split("^");

    var errorTagAssignError = { "it": "Errore nell\'assegnazione del TAG", "en": "Error during TAG assignement" };
    var errorTagAssignedError = { "it": "TAG già assegnato ad un altro ente", "en": "TAG already assigned to other city" };
    var errorTagAssignedMyCityError = { "it": "TAG già assegnato", "en": "TAG already assigned" };
    var errorDbError = { "it": "Errore connessione database", "en": "Errore connessione database" };

    var messageTagAssigned = { "it": "TAG assegnato", "en": "TAG assigned" };

    var alertInfo = document.getElementById("panel_error");
    var tagCode = document.getElementById("input_tag_code");

    if (values[0] != "null") {
        alertInfo.innerHTML = ComFun_CreateAlert("alert-success", messageTagAssigned[values[1]]);
        tagCode.value = "";
    } else {
        switch (values[2]) {
            case "TAG_ASSIGN_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagAssignError[values[1]]);

                break;
            case "TAG_ASSIGNED_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagAssignedError[values[1]]);

                break;
            case "TAG_ASSIGNED_MYCITY_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagAssignedMyCityError[values[1]]);

                break;
            case "DB_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorDbError[values[1]]);

                break;
        }
    }

    tagCode.focus();
}

function CheckTag(cityid, lang) {
    document.getElementById("image_loading").style.display = "";

    var tagCode = document.getElementById("input_tag_code");

    ServiceCity.CS_CheckTag(tagCode.value, cityid, lang, OnCheckTag, OnRequestFailed);
}

function OnCheckTag(results) {
    document.getElementById("image_loading").style.display = "none";

    var values = results.split("^");

    var errorDbError = { "it": "Errore connessione database", "en": "Errore connessione database" };

    var messageTagAssigned = { "it": "TAG assegnato", "en": "TAG assigned" };
    var messageTagAssignedUnit = { "it": "TAG assegnato<br /><br />Unità collegata: {p1}<br />{p2}<br />{p3}", "en": "TAG assigned<br />Linked unit:{p1}<br />{p2}<br />{p3}" };
    var messageNotTagAssigned = { "it": "TAG non assegnato", "en": "TAG not assigned" };

    var alertInfo = document.getElementById("panel_error");
    var tagCode = document.getElementById("input_tag_code");

    if (values[0] != "null") {
        var modalTitle = { "it": "Verifica TAG", "en": "TAG check" };
        document.getElementById("modal_title_check").innerHTML = modalTitle[values[1]];

        var modalText = ""
        switch (values[2]) {
            case "TAG_ASSIGNED":
                modalText = messageTagAssigned[values[1]];

                break;
            case "TAG_ASSIGNED_UNIT":
                modalText = messageTagAssignedUnit[values[1]].replace("{p1}", values[3]).replace("{p2}", values[4]).replace("{p3}", values[5]);

                break;
            case "TAG_NOT_ASSIGNED":
                modalText = messageNotTagAssigned[values[1]];

                break;
        }
        document.getElementById("modal_text_check").innerHTML = modalText;
        
        $("#modal_message_check").modal('show');
    } else {
        switch (values[2]) {
            case "DB_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorDbError[values[1]]);

                break;
        }
    }

    tagCode.focus();
}

function SelectTagId(cityid, lang) {
    var tagCode = document.getElementById("input_tag_code");

    var modalTitle = { "it": "Eliminazione del TAG", "en": "TAG deleting" };
    document.getElementById("modal_title_delete").innerHTML = modalTitle[lang];

    var modalText = { "it": "Confermi l\'eliminazione del TAG selezionato?", "en": "Are you sure to delete selected TAG?" };
    document.getElementById("modal_text_delete").innerHTML = modalText[lang];

    var linkText = { "it": "Conferma", "en": "Submit" };
    document.getElementById("link_modal_delete").innerHTML = linkText[lang];
    document.getElementById("link_modal_delete").href = "javascript: DeleteTag(\'" + tagCode.value + "\',\'" + cityid + "\',\'" + lang + "\')";
}

function DeleteTag(tagcode, cityid, lang) {
    document.getElementById("image_loading").style.display = "";

    ServiceCity.CS_DeleteTagFromCity(tagcode, cityid, lang, OnDeleteTagFromCity, OnRequestFailed);
}

function OnDeleteTagFromCity(results) {
    document.getElementById("image_loading").style.display = "none";

    var values = results.split("^");

    var errorTagDeleteError = { "it": "Errore nell\'eliminazione del TAG", "en": "Error during TAG deleting" };
    var errorTagError = { "it": "Codice TAG non trovato", "en": "TAG code not exists" };
    var errorDbError = { "it": "Errore connessione database", "en": "Errore connessione database" };

    var messageTagDeleted = { "it": "TAG eliminato", "en": "TAG deleted" };

    var alertInfo = document.getElementById("panel_error");
    var tagCode = document.getElementById("input_tag_code");

    if (values[0] != "null") {       
        alertInfo.innerHTML = ComFun_CreateAlert("alert-success", messageTagDeleted[values[1]]);
        tagCode.value = "";
    } else {
        switch (values[2]) {
            case "TAG_DELETE_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagDeleteError[values[1]]);

                break;
            case "TAG_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagError[values[1]]);

                break;
            case "DB_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorDbError[values[1]]);

                break;
        }
    }

    $("#modal_message_delete").modal('hide');

    tagCode.focus();
}