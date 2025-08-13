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

                // Format date as: YYYY-MM-DD hh:mm AM/PM
                const year = now.getFullYear();
                const month = String(now.getMonth() + 1).padStart(2, '0');
                const day = String(now.getDate()).padStart(2, '0');
                const hours = now.getHours();
                const ampm = hours >= 12 ? 'PM' : 'AM';
                const hour12 = hours % 12 || 12;
                const minutes = String(now.getMinutes()).padStart(2, '0');

                const formatted = `${year}-${month}-${day} ${hour12}:${minutes} ${ampm}`;
                dateInput.value = formatted;

                // This updates the UseCurrentDateTime model property
                document.getElementById('UseCurrentDateTime').value = 'true';
            } else {
                dateInput.value = '';
                document.getElementById('UseCurrentDateTime').value = 'false';
            }
        });
    }
});
