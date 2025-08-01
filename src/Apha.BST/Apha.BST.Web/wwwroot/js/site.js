// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function openDateCalender() {
    const input = document.getElementById("dateInput");

    if (input.showPicker) {
        input.showPicker();
    } else {
        input.focus();
        input.click();
    }
}
document.addEventListener('DOMContentLoaded', function () {
    const checkbox = document.getElementById('news-date-checkbox');
    const dateInput = document.getElementById('news-date-published');

    if (checkbox && dateInput) {
        checkbox.addEventListener('change', function () {
            if (this.checked) {
                const now = new Date();
                const formatted = now.toISOString().slice(0, 16).replace('T', ' ');
                dateInput.value = formatted;
            } else {
                dateInput.value = '';
            }
        });
    }
})
