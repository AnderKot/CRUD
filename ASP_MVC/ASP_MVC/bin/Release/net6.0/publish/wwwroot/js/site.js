// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;
var DateToFilter = { text: moment().format('YYYY-MM-DD') };
var DateFromFilter = { text: moment().add(-1, 'M').format('YYYY-MM-DD') };
var NumberFilter = { text: "" };
var ProviderFilter = { text: "" };
var NameFilter = { text: "" };
var QuantityFilter = { text: "" };
var UnitFilter = { text: "" };

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

    let orederNumber = document.getElementById('Order_Number').value;
    if (orederNumber == '') {
        alert('Заполните поле "Номер" заказа');
        return;
    }

    let orederDate = document.getElementById('Order_Date').value;
    if (orederDate == '') {
        alert('Заполните поле "Дата" заказа');
        return;
    }

    let orederProvider = document.getElementById('ProviderNameLabel').innerHTML;
    if (orederProvider == '') {
        alert('Заполните поле "Поставщик" заказа');
        return;
    }

    let OrderJSON = {
        //OrderJSON: {
        id: order_id,
        Number: orederNumber,
        Date: orederDate,
        Provider: orederProvider,
        Items: []
        //}
    };

    let table = document.getElementById('OrderItemTable');

    for (let i = 0; i < table.rows.length; i++) {
        let tableRow = table.rows[i];
        let id = tableRow.id.replace('Item_', '');
        if (id == '')
            id = '-1';
        let itemJSON = {
            id: id,
            Name: tableRow.cells[0].innerHTML,
            Quantity: tableRow.cells[1].innerHTML,
            Unit: tableRow.cells[2].innerHTML
        }
        OrderJSON.Items.push(itemJSON);
    } 

    console.log('javascript_Order_TryOrderSave_' + JSON.stringify(OrderJSON));

    let url = document.location.protocol + "//" + document.location.host + "/Home/OrderPost";

    const response = await fetch(url, {
        method: 'POST',
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(OrderJSON)
    });

    let ok = await response.ok;
    let id;
    if (ok) {
        let DeleteBtn = document.getElementById('OrderDelete_-1');
        if (DeleteBtn != null) {
            id = await response.text();
            id = id.replaceAll('"', '');
            DeleteBtn.id = 'OrderDelete_' + id;
            order.id = 'OrderSave_' + id;
        }
        SetFilters(); // Пересобираем таблицу с заказами из бд
        console.log('javascript_Order_OrderSaved_' + id );
        
        alert('Заказ сохранен');
    }
    else
        alert('Ошибка при сохранении закакза, прверьте данные');
}

// Установка фильтров по изменению в input и select
function SelectFilters(imput) {
    console.log('javascript_TryChenge' + imput.id);
    let newFilterPart;
    switch (imput.id) {

        case 'DateTo':
            DateToFilter.text = imput.value;
            console.log('javascript_' + imput.id + '_' + DateToFilter.text);
            break;

        case 'DateFrom':
            DateFromFilter.text = imput.value;
            console.log('javascript_' + imput.id + '_' + DateFromFilter.text);
            break;

        case 'Number':
            newFilterPart = imput.options[imput.selectedIndex].text;
            FilterProc(newFilterPart, NumberFilter, 'Number');
            imput.selectedIndex = -1;
            console.log('javascript_Number_' + NumberFilter.text);
            break;

        case 'Provider':
            newFilterPart = imput.options[imput.selectedIndex].text;
            FilterProc(newFilterPart, ProviderFilter, 'Provider');
            imput.selectedIndex = -1;
            break;

        case 'Name':
            newFilterPart = imput.options[imput.selectedIndex].text;
            FilterProc(newFilterPart, NameFilter, 'Name');
            imput.selectedIndex = -1;
            break;

        case 'Quantity':
            newFilterPart = imput.options[imput.selectedIndex].text;
            FilterProc(newFilterPart, QuantityFilter, 'Quantity');
            imput.selectedIndex = -1;
            break;

        case 'Unit':
            newFilterPart = imput.options[imput.selectedIndex].text;
            FilterProc(newFilterPart, UnitFilter, 'Unit');
            imput.selectedIndex = -1;
            break;
    }
}

function FilterProc(newValue, Filter, label) {
    console.log('javascript_TryAddTo_' + label + '_' + Filter);
    if (!Filter.text.includes(newValue + ' '))
        Filter.text += newValue + ' ';
    else
        Filter.text = Filter.text.replace(newValue + ' ', '');

    document.getElementById(label + 'label').innerHTML = Filter.text;

    console.log('javascript_' + label + '_' + Filter.text);
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

// Применение фильтров таблицы товаров
function SetItemsFilters() {
    let order_id = order.id.replace('OrderSave_', '');

    var GETurl = document.location.protocol + "//" + document.location.host + "/Home/Items" +
        "?id=" + order_id +
        "&Name=" + NameFilter.text +
        "&Quantity=" + QuantityFilter.text +
        "&Unit=" + UnitFilter.text;

    console.log('javascript_' + GETurl);

    $.get(GETurl, function (data) {
        let JsonData = JSON.parse(data);

        // Обновляем таблицу
        let table = document.getElementById('OrderItemTable');
        while (table.firstChild) {
            table.removeChild(table.firstChild);
        }
        let Rows = document.createDocumentFragment();
        JsonData['TableData'].forEach(order => {
            let row = document.createElement('tr');

            row.onclick = function () {
                ClickOnLine(this);
            }

            row.id = order['Id']
            row.ondblclick = function () {
                DoubleClickOnLine(this);
            }

            let tdName = document.createElement('td');
            tdName.innerText = order['Name'];
            row.appendChild(tdName);

            let tdQuantity = document.createElement('td');
            tdQuantity.innerText = order['Quantity'];
            row.appendChild(tdQuantity);

            let tdUnit = document.createElement('td');
            tdProvName.innerText = order['Unit'];
            row.appendChild(tdUnit)

            Rows.appendChild(row);
        });
        table.appendChild(Rows);
    });
}

// Применение фильтров таблицы заказов (Перерисовка таблицы)
function SetFilters() {

    var GETurl = document.location.protocol + "//" + document.location.host + "/Home/Orders" +
        "?DateFrom=" + DateFromFilter.text +
        "&DateTo=" + DateToFilter.text +
        "&Number=" + NumberFilter.text +
        "&Provider=" + ProviderFilter.text;

    console.log('javascript_' + GETurl);

    $.get(GETurl, function (data) {
        let JsonData = JSON.parse(data);
        // Фильтр для номеров заказов
        
        let numberFilter = document.getElementById('Number');
        while (numberFilter.firstChild) {
            numberFilter.removeChild(numberFilter.firstChild);
        }
        let options = document.createDocumentFragment();
        JsonData['OrderNumber'].forEach(optionSTR => {
            let option = document.createElement('option');
            option.innerHTML = optionSTR;

            options.appendChild(option);
        });
        numberFilter.appendChild(options);

        // Фильтр для поставщиков
        let providerFilter = document.getElementById('Provider');
        while (providerFilter.firstChild) {
            providerFilter.removeChild(providerFilter.firstChild);
        }
        options = document.createDocumentFragment();
        JsonData['OrderProviderName'].forEach(optionSTR => {
            let option = document.createElement('option');
            option.innerHTML = optionSTR;

            options.appendChild(option);
        });
        providerFilter.appendChild(options);

        // Обновляем таблицу
        let table = document.getElementById('OrderTableBody');
        while (table.firstChild) {
            table.removeChild(table.firstChild);
        }
        let Rows = document.createDocumentFragment();
        JsonData['TableData'].forEach(order => {
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

