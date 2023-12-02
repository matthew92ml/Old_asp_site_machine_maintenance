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

function AssignTagToUnit(cityid, lang) {
    document.getElementById("image_loading").style.display = "";

    var tagCode = document.getElementById("input_tag_code");
    var unitId = document.getElementById("hidden_id");
    var unitCode = document.getElementById("input_unit_code");
    var infos = document.getElementById("input_infos");

    ServiceUnits.CS_AssignTagToUnit(tagCode.value, cityid, unitId.value, unitCode.value, infos.value, lang, OnAssignTagToUnit, OnRequestFailed);
}

function OnAssignTagToUnit(results) {
    document.getElementById("image_loading").style.display = "none";

    var values = results.split("^");

    var errorTagAssignError = { "it": "Errore nell\'assegnazione del TAG", "en": "Error during TAG assignement" };
    var errorTagAssignedError = { "it": "TAG già assegnato", "en": "TAG already assigned" };
    var errorTagError = { "it": "Codice TAG inesistente", "en": "TAG code not exists" };
    var errorDbError = { "it": "Errore connessione database", "en": "Errore connessione database" };

    var alertInfo = document.getElementById("panel_error");
    var tagCode = document.getElementById("input_tag_code");
    var infos = document.getElementById("input_infos");

    if (values[0] != "null") {
        document.getElementById("panel_tag_list").innerHTML = values[1];
        tagCode.value = "";
        infos.value = "";
    } else {
        switch (values[2]) {
            case "TAG_ASSIGN_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagAssignError[values[1]]);

                break;
            case "TAG_ASSIGNED_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagAssignedError[values[1]]);

                break;
            case "TAG_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorTagError[values[1]]);

                break;
            case "DB_ERROR":
                alertInfo.innerHTML = ComFun_CreateAlert("alert-danger", errorDbError[values[1]]);

                break;
        }
    }

    tagCode.focus();
}

function SelectUnitId(deletelink, lang) {
    var modalTitle = { "it": "Eliminazione del nucleo", "en": "Unit deleting" };
    document.getElementById("modal_title").innerHTML = modalTitle[lang];

    var modalText = { "it": "Confermi l\'eliminazione del nucleo selezionato?", "en": "Are you sure to delete selected unit?" };
    document.getElementById("modal_text").innerHTML = modalText[lang];
    
    var linkText = { "it": "Conferma", "en": "Submit" };
    document.getElementById("link_modal_delete").innerHTML = linkText[lang];
    document.getElementById("link_modal_delete").href = deletelink;
}

function SelectTagId(cityid, lang) {
    var tagCode = document.getElementById("input_tag_code");
    var unitId = document.getElementById("hidden_id");
    var unitCode = document.getElementById("input_unit_code");

    var modalTitle = { "it": "Eliminazione del TAG", "en": "TAG deleting" };
    document.getElementById("modal_title").innerHTML = modalTitle[lang];

    var modalText = { "it": "Confermi l\'eliminazione del TAG selezionato?", "en": "Are you sure to delete selected TAG?" };
    document.getElementById("modal_text").innerHTML = modalText[lang];

    var linkText = { "it": "Conferma", "en": "Submit" };
    document.getElementById("link_modal_delete").innerHTML = linkText[lang];
    document.getElementById("link_modal_delete").href = "javascript: DeleteTag(\'" + tagCode.value + "\',\'" + cityid + "\',\'" + unitId.value + "\',\'" + unitCode.value + "\',\'" + lang + "\')";
}

function DeleteTag(tagcode, cityid, unitid, unitcode, lang) {
    document.getElementById("image_loading").style.display = "";

    ServiceUnits.CS_DeleteTagFromUnit(tagcode, cityid, unitid, unitcode, lang, OnDeleteTagFromUnit, OnRequestFailed);
}

function OnDeleteTagFromUnit(results) {
    document.getElementById("image_loading").style.display = "none";

    var values = results.split("^");

    var errorTagDeleteError = { "it": "Errore nell\'eliminazione del TAG", "en": "Error during TAG deleting" };
    var errorTagError = { "it": "Codice TAG non trovato", "en": "TAG code not exists" };
    var errorDbError = { "it": "Errore connessione database", "en": "Errore connessione database" };

    var alertInfo = document.getElementById("panel_error");
    var tagCode = document.getElementById("input_tag_code");

    if (values[0] != "null") {       
        document.getElementById("panel_tag_list").innerHTML = values[1];
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

function ReloadPage(link, objid, parmname, otherparms) {
    var obj = document.getElementById(objid);

    var redirectLink = link + "&" + parmname + "=" + obj.value.replace(" ", "-");
    if (otherparms.indexOf(";") > -1) {
        var parms = otherparms.split(";")

        var i = 0;
        for (i = 0; i < parms.length; i++) {
            var values = parms[i].split(":");

            if (parmname != values[0] && values[1] != "0") redirectLink += "&" + values[0] + "=" + values[1].replace(" ", "-");
        }
    } else {
        var values = parms.split(":");

        if (parmname != values[0] && values[1] != "0") redirectLink += "&" + values[0] + "=" + values[1].replace(" ", "-");
    }

    window.location = redirectLink;
}
       