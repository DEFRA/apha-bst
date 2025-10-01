// utility: open date picker (keeps your original helpers)
function openDateCalender() {
    const visibleInput = document.getElementById("dateInput");
    const hiddenInput = document.getElementById("hiddenDateInput");

    if (visibleInput.disabled) {
        return; // Don't open if input is disabled
    }

    hiddenInput.value = ''; // Reset hidden input

    hiddenInput.onchange = function () {
        if (!this.value) return;

        const date = new Date(this.value);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();

        visibleInput.value = `${day}/${month}/${year}`;
    };

    // Show picker (modern) or fallback to click
    if (typeof hiddenInput.showPicker === 'function') {
        hiddenInput.showPicker();
    } else {
        hiddenInput.focus();
        hiddenInput.click();
    }
}
document.addEventListener('DOMContentLoaded', function () {
    const checkbox = document.getElementById('news-date-checkbox');
    const dateInput = document.getElementById('news-date-published');
   
    
    if (checkbox && dateInput) {
        checkbox.addEventListener('change', function () {
            if (this.checked) {
                const ukDate = getCurrentUKTimeFormatted();                
                dateInput.value = ukDate;
                // This updates the UseCurrentDateTime model property
                const useCurrent = document.getElementById('UseCurrentDateTime');
                if (useCurrent) useCurrent.value = 'true';
            } else {
                dateInput.value = '';
                const useCurrent = document.getElementById('UseCurrentDateTime');
                if (useCurrent) useCurrent.value = 'false';
            }
        });
    }
});
function getCurrentUKTimeFormatted() {
    const now = new Date();

    // Use Intl.DateTimeFormat with Europe/London timezone and options to get parts
    const options = {
        timeZone: 'Europe/London',
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
        hour12: false,
    };

    const formatter = new Intl.DateTimeFormat('en-GB', options);
    const parts = formatter.formatToParts(now);

    // Extract parts by type
    const dateParts = {};
    parts.forEach(({ type, value }) => {
        dateParts[type] = value;
    });

    // Format as dd/MM/yyyy HH:mm:ss
    return `${dateParts.day}/${dateParts.month}/${dateParts.year} ${dateParts.hour}:${dateParts.minute}:${dateParts.second}`;
}


// Accessible navigation class (robust version with blur + focusout)
class AccessibleNavigation {
    constructor() {
        this.menuBar = document.querySelector(".menu-bar");
        this.dropdowns = [...document.querySelectorAll(".dropdown")];
        this.isKeyboardMode = false;
        this.currentOpen = null;
        this.bootstrapAvailable = typeof bootstrap !== "undefined" && !!bootstrap.Dropdown;
        this.init();
    }

    init() {
        this.disableBootstrap();
        this.globalListeners();
        this.dropdownListeners();
    }

    disableBootstrap() {
        if (!this.bootstrapAvailable) return;
        this.dropdowns.forEach(d => {
            const t = d.querySelector(".dropdown-toggle");
            try { bootstrap.Dropdown.getInstance(t)?.dispose(); } catch { }
            if (t) t.removeAttribute("data-bs-toggle");
        });
    }

    globalListeners() {
        document.addEventListener("mousemove", () => this.isKeyboardMode && this.switchMode(false));
        document.addEventListener("keydown", e => {
            if (["Tab", "Enter", "Escape", "ArrowDown", "ArrowUp", " "].includes(e.key) && !this.isKeyboardMode)
                this.switchMode(true);
        });
        document.addEventListener("click", e => !e.target.closest(".dropdown") && this.closeAll());
    }

    dropdownListeners() {
        this.dropdowns.forEach(d => {
            const t = d.querySelector(".dropdown-toggle"),
                items = [...d.querySelectorAll(".dropdown-item")];

            d.addEventListener("pointerenter", () => (this.switchMode(false), this.open(d, "mouse")));
            d.addEventListener("pointerleave", () => this.close(d, "mouse"));

            if (t) {
                t.addEventListener("click", e => {
                    e.preventDefault(); this.switchMode(false);
                    d.classList.contains("mouse-active") ? this.close(d, "mouse") : this.open(d, "mouse");
                });
                t.addEventListener("keydown", e => {
                    if (["Enter", " ", "ArrowDown"].includes(e.key)) {
                        e.preventDefault(); this.switchMode(true);
                        this.open(d, "keyboard"); this.focusFirst(d);
                    } else if (e.key === "Escape") {
                        e.preventDefault(); this.closeAll(); t.blur();
                    }
                });
                t.addEventListener("focus", () => this.isKeyboardMode && this.open(d, "keyboard"));
            }

            items.forEach((item, i) => {
                item.addEventListener("keydown", e => {
                    if (e.key === "ArrowDown") { e.preventDefault(); this.focusNext(d, i); }
                    else if (e.key === "ArrowUp") { e.preventDefault(); this.focusPrev(d, i); }
                    else if (e.key === "Escape") { e.preventDefault(); this.close(d, "keyboard"); t?.focus(); }
                    else if (e.key === "Tab") this.close(d, "keyboard");
                });
                item.addEventListener("click", () => this.closeAll());
            });

            d.addEventListener("focusout", () => setTimeout(() => {
                if (!d.contains(document.activeElement)) this.close(d, d.classList.contains("keyboard-active") ? "keyboard" : "mouse");
            }));
        });
    }

    switchMode(kb) {
        this.isKeyboardMode = kb;
        this.menuBar?.classList.toggle("keyboard-mode", kb);
        if (!kb) { this._blurActive(); this.dropdowns.forEach(d => this.close(d, "keyboard")); }
        else this.dropdowns.forEach(d => this.close(d, "mouse"));
    }

    open(d, mode) {
        this._closeOthers(d);
        d.classList.add(`${mode}-active`); d.classList.remove(mode === "mouse" ? "keyboard-active" : "mouse-active");
        if (mode === "mouse") this._blurActive();
        this._show(d); this._aria(d, true);
        this.currentOpen = d;
    }

    close(d, mode) {
        d.classList.remove(`${mode}-active`, "focus-active");
        this._hide(d); this._aria(d, false);
        if (this.currentOpen === d) this.currentOpen = null;
    }

    closeAll() { this.dropdowns.forEach(d => this.close(d, "mouse") || this.close(d, "keyboard")); }

    _closeOthers(except) { this.dropdowns.forEach(d => d !== except && this.close(d, "mouse") && this.close(d, "keyboard")); }
    _show(d) { d.querySelector(".dropdown-menu")?.classList.add("show"), d.classList.add("show"); }
    _hide(d) { d.querySelector(".dropdown-menu")?.classList.remove("show"), d.classList.remove("show"); }
    _aria(d, v) { d.querySelector(".dropdown-toggle")?.setAttribute("aria-expanded", v); }
    _blurActive() { try { let a = document.activeElement; if (a && this.menuBar?.contains(a)) a.blur() } catch { } }

    focusFirst(d) { d.querySelector(".dropdown-item")?.focus(); }
    focusNext(d, i) { const items = [...d.querySelectorAll(".dropdown-item")]; items[(i + 1) % items.length]?.focus(); }
    focusPrev(d, i) { const items = [...d.querySelectorAll(".dropdown-item")]; items[(i ? i : items.length) - 1]?.focus(); }
}

document.addEventListener("DOMContentLoaded", () => window._accessibleNav = new AccessibleNavigation());
