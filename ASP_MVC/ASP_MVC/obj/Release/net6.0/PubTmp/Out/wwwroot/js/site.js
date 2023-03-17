// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;
var DateTo = null;
var DateFrom = null;

function ClickOnLine(row) {
    if (CurrRow == null) {
        row.className = "SelectedRow";
        CurrRow = row;
    }
    else {
        if (row.className == "SelectedRow") {
            row.className = "";
            CurrRow = null;
        }
        else
        {
            CurrRow.className = "";

            row.className = "SelectedRow";
            CurrRow = row;
        }
    }
}

function DoubleClickOnLine(order_id) {
    

    $.get(document.location.protocol + "//" + document.location.host + "/Home/Order?id=" + order_id, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function SelectFilters(list) {
    let selectedOption = list.options[list.selectedIndex];
    console.log('javascript_' + list.id);
    switch (list.id) {
        case 'DateTo':
            DateTo = selectedOption.text;
            break;
        case 'DateFrom':
            DateFrom = selectedOption.text;
            break;
    }
}

function SetFilters() {
    DateFrom = DateFrom || "Не Выбран";

    DateTo = DateTo || "Не Выбран";

    if ((DateFrom == "Не Выбран") && (DateTo == "Не Выбран")) {
        document.location.href = document.location.protocol + "//" +document.location.host + "/Home/CRUD";
    }
    else {
        document.location.href = document.location.protocol+ "//" +document.location.host + "/Home/CRUD?From=" + DateFrom + "&To=" + DateTo;
    }

}

