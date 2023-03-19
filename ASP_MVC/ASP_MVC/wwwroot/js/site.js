// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;
var DateToFilter = null;
var DateFromFilter = null;
var NumberFilter = null;
var ProviderFilter = null;

function ClickOnLine(row) {
    if (CurrRow == null) {
        row.className = "SelectedRow";
        CurrRow = row;
        console.log('javascript_' + CurrRow);
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
            console.log('javascript_' + CurrRow);
        }
    }
}

function DoubleClickOnLine(order_id) {
    $.get(document.location.protocol + "//" + document.location.host + "/Home/Order?id=" + order_id, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

function SelectFilters(imput) {
    switch (imput.id) {
        case 'DateTo':
            DateToFilter = imput.value;
            console.log('javascript_' + imput.id + '_' + DateToFilter);
            break;
        case 'DateFrom':
            DateFromFilter = imput.value;
            console.log('javascript_' + imput.id + '_' + DateFromFilter);
            break;
        case 'Number':
            let selectedOption = list.options[list.selectedIndex];
            NumberFilter = selectedOption.text;
            console.log('javascript_' + imput.id + '_' + NumberFilter);
            break;
        case 'Provider':
            let selectedOption = list.options[list.selectedIndex];
            ProviderFilter = selectedOption.text;
            console.log('javascript_' + imput.id + '_' + ProviderFilter);
            break;
    }
}

function SetFilters() {
    DateFrom = DateFrom || moment().add(1, 'M').format('"yyyy-MM-dd');
    DateTo = DateTo || moment().format('"yyyy-MM-dd');

    var GETurl = document.location.protocol + "//" + document.location.host + "/Home/Orders" +
        "?DateFrom = " + DateFromFilter +
        "&DateTo=" + DateToFilter +
        "&Number=" + NumberFilter +
        "&Provider=" + ProviderFilter;

    console.log('javascript_' + GETurl);

    $.get(GETurl, function (data) {
        let table = document.getElementById('OrderTableBody');
        while (table.firstChild) {
            table.removeChild(myNode.firstChild);
        }

        let JsonData = data.json();
        let Rows = document.createDocumentFragment();
        JsonData.forEach(order => {
            let row = document.createElement('tr');

            order.forEach(col => {
                let td = document.createElement('td');
                td.innerText = col;
                row.appendChild(td);
            });

            Rows.appendChild(row);
        });
        table.appendChild(Rows);
    });
}

