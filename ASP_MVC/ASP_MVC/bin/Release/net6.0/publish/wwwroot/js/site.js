// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var CurrRow = null;

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
    }

}