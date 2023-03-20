// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;
var DateToFilter = moment().format('YYYY-MM-DD');
var DateFromFilter = moment().add(-1, 'M').format('YYYY-MM-DD');
var NumberFilter = "";
var ProviderFilter = "";

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

// Установка фильтров по изменению в input и select
function SelectFilters(imput) {
    let newFilterPart;
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
            newFilterPart = imput.options[imput.selectedIndex].text;

            if (!NumberFilter.includes(' '+newFilterPart))
                NumberFilter += ' ' + newFilterPart;
            else
                NumberFilter = NumberFilter.replace(' ' + newFilterPart, '');

            document.getElementById('Numberlabel').innerHTML = NumberFilter;

            console.log('javascript_' + imput.id + '_' + NumberFilter);
            imput.selectedIndex = -1;
            break;

        case 'Provider':
            newFilterPart = imput.options[imput.selectedIndex].text;

            if (!ProviderFilter.includes(' ' + newFilterPart))
                ProviderFilter += ' ' + newFilterPart;
            else
                ProviderFilter = ProviderFilter.replace(' ' + newFilterPart, '');

            document.getElementById('Providerlabel').innerHTML =  ProviderFilter;

            console.log('javascript_' + imput.id + '_' + ProviderFilter);
            imput.selectedIndex = -1;
            break;
    }
}

// Применение фильтров (Переривка таблицы)
function SetFilters() {

    var GETurl = document.location.protocol + "//" + document.location.host + "/Home/Orders" +
        "?DateFrom=" + DateFromFilter +
        "&DateTo=" + DateToFilter +
        "&Number=" + NumberFilter +
        "&Provider=" + ProviderFilter;

    console.log('javascript_' + GETurl);

    $.get(GETurl, function (data) {
        let table = document.getElementById('OrderTableBody');
        while (table.firstChild) {
            table.removeChild(table.firstChild);
        }

        let JsonData = JSON.parse(data);
        let Rows = document.createDocumentFragment();
        JsonData.forEach(order => {
            let row = document.createElement('tr');

            let tdNumber = document.createElement('td');
            tdNumber.innerText = order['Number'];
            row.appendChild(tdNumber);

            let tdDate = document.createElement('td');
            tdDate.innerText = order['Date'];
            row.appendChild(tdDate);

            let tdProvName = document.createElement('td');
            tdProvName.innerText = order['Provider']['Name'];
            row.appendChild(tdProvName)

            Rows.appendChild(row);
        });
        table.appendChild(Rows);
    });
}

