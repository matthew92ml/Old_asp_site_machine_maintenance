function CreateTable(objid, tableheaders, tabledata, tabletype, tablelabels, displayitems) {
    if (tableheaders == null || tabledata == null) return;

    switch (tabletype) {
        case 1:
            $(objid).html("<div class=\"table-responsive\"><table id=\"data_table_3\" class=\"table table-striped table-bordered\" style=\"font-size: 11px;\"></table></div>");

            $("#data_table_3").dataTable({
                "aaData": tabledata,
                "aoColumns": tableheaders,
                "iDisplayLength": displayitems,
                "bLengthChange": false,
                "bFilter": false,
                "aaSorting": [[0, "desc"]],
                "oLanguage": {
                    "sInfo": tablelabels[0],
                    "sLengthMenu": tablelabels[1],
                    "sSearch": tablelabels[2],
                    "sZeroRecords": tablelabels[3],
                    "sInfoEmpty": tablelabels[4],
                    "sInfoFiltered": tablelabels[5],
                    "oPaginate": {
                        "sNext": "",
                        "sPrevious": ""
                    }
                }
            });

            break;
        case 2:
            $(objid).dataTable({
                "aaData": tabledata,
                "aoColumns": tableheaders,
                "iDisplayLength": displayitems,
                "oLanguage": {
                    "sInfo": tablelabels[0],
                    "sLengthMenu": tablelabels[1],
                    "sSearch": tablelabels[2],
                    "sZeroRecords": tablelabels[3],
                    "sInfoEmpty": tablelabels[4],
                    "sInfoFiltered": tablelabels[5],
                    "oPaginate": {
                        "sNext": "",
                        "sPrevious": ""
                    }
                }
            });

            break;
    }
}