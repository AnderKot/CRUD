// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;
var DateToFilter = moment().format('YYYY-MM-DD');
var DateFromFilter = moment().add(-1, 'M').format('YYYY-MM-DD');
var NumberFilter = "";
var ProviderFilter = "";

// Выделение выбранной строки
function ClickOnLine(row) {
    if (CurrRow == null) {
        row.className = "SelectedRow";
        CurrRow = row;
        console.log('javascript_Row_' + CurrRow.id);
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
            console.log('javascript_Row_' + CurrRow.id);
        }
    }
}

// Открытие заказа на дабл клик
function DoubleClickOnLine(row) {
    let order_id = row.id; 
    $.get(document.location.protocol + "//" + document.location.host + "/Home/Order?id=" + order_id, function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

// Создание пустоко заказа
function NewOrder() {
    $.get(document.location.protocol + "//" + document.location.host + "/Home/Order?id=", function (data) {
        $('#dialogContent').html(data);
        $('#modDialog').modal('show');
    });
}

// Удаление заказа
async function DeleteOrder(order) {
    let order_id = order.id.replace('OrderDelete_', ''); 

    let url = document.location.protocol + "//" + document.location.host + "/Home/OrderDelete?id=" + order_id;

    const response = await fetch(url, {
        method: 'DELETE'
    })

    let ok = await response.ok;
    if (ok) {
        $('#modDialog').modal('hide');
        SetFilters(); // Пересобираем таблицу с заказами из бд
    }

}

// Сохранение заказа
async function SaveOrder(order) {
    let order_id = order.id.replace('OrderSave_', '');
    
    let OrderJSON = {
        //OrderJSON: {
            id: order_id,
            Number: document.getElementById('Order_Number').value,
            Date: document.getElementById('Order_Date').value,
            Provider: document.getElementById('ProviderNameLabel').innerHTML
        //}
    };
    //id Number Date Provider
    console.log('javascript_Order_OrderSave_' + JSON.stringify(OrderJSON));

    let url = document.location.protocol + "//" + document.location.host + "/Home/OrderPost";

    const response = await fetch(url, {
        method: 'POST',
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(OrderJSON)
    })

    let ok = await response.ok;
    if (ok) {
        SetFilters(); // Пересобираем таблицу с заказами из бд
        console.log('javascript_Order_OrderSave_' + order_id);
    }
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
// Установка выбраной опции в поле
function Selectlabel(imput) {
    let label = document.getElementById(imput.id + 'Label');
    label.innerHTML = imput.options[imput.selectedIndex].text;
    imput.selectedIndex = -1;
}

// Добавление строки с товаром
function Additem() {
    let table = document.getElementById('OrderItemTable');
    let row = document.createElement('tr');
    row.onclick = function () {
        ClickOnLine(this);
    }
    let tdName = document.createElement('td');
    tdName.contentEditable = true;
    tdName.innerText = "*";
    row.appendChild(tdName);

    let tdQuantity = document.createElement('td');
    tdQuantity.innerText = "0";
    tdQuantity.contentEditable = true;
    row.appendChild(tdQuantity);

    let tdUnit = document.createElement('td');
    tdUnit.contentEditable = true;
    tdUnit.innerText = "*";
    row.appendChild(tdUnit);

    let tdImage = document.createElement('td');
    let Img = document.createElement('img');
    Img.src = "Images\\Icons\\delete.png";
    Img.width = 30;
    Img.height = 30;
    Img.onclick = function () {
        Deleteitem(this);
    }
    tdImage.appendChild(Img);
    row.appendChild(tdImage);

    table.appendChild(row);
}

// Удаление строки
function Deleteitem(row) {
    row.parentNode.parentNode.parentNode.removeChild(row.parentNode.parentNode);
}

// Применение фильтров (Перерисовка таблицы)
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

            row.onclick = function () {
                ClickOnLine(this);
            }

            row.id = order['Id']
            row.ondblclick = function () {
                DoubleClickOnLine(this);
            }

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

