// utility: open date picker (keeps your original helpers)
function openDateCalender() {
    const input = document.getElementById("dateInput");

    if (input && input.showPicker) {
        input.showPicker();
    } else if (input) {
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

// Accessible navigation class (robust version with blur + focusout)
class AccessibleNavigation {
    constructor() {
        this.menuBar = document.querySelector('.menu-bar');
        this.dropdowns = Array.from(document.querySelectorAll('.dropdown'));
        this.isKeyboardMode = false;
        this.currentOpenDropdown = null;
        this.bootstrapAvailable = typeof bootstrap !== 'undefined' && !!bootstrap.Dropdown;

        this.init();
    }

    init() {
        // If Bootstrap created Dropdown instances on the anchors, dispose them so we control behavior.
        this.disableBootstrapDropdowns();

        this.setupGlobalListeners();
        this.setupPerDropdownListeners();
    }

    disableBootstrapDropdowns() {
        if (!this.bootstrapAvailable) return;
        this.dropdowns.forEach(dropdown => {
            const toggle = dropdown.querySelector('.dropdown-toggle');
            try {
                const bs = bootstrap.Dropdown.getInstance(toggle);
                if (bs && typeof bs.dispose === 'function') bs.dispose();
            } catch (err) {
                // ignore if bootstrap not fully loaded
            }
            // remove attribute so bootstrap won't try to re-initialize
            if (toggle) toggle.removeAttribute('data-bs-toggle');
        });
    }

    setupGlobalListeners() {
        // Detect mouse usage and switch modes
        document.addEventListener('mousemove', () => {
            if (this.isKeyboardMode) this.switchToMouseMode();
        });

        // Detect keyboard usage (typical keys that indicate keyboard navigation)
        document.addEventListener('keydown', (e) => {
            const keys = ['Tab', 'Enter', 'Escape', 'ArrowDown', 'ArrowUp', ' '];
            if (keys.includes(e.key) && !this.isKeyboardMode) {
                this.switchToKeyboardMode();
            }
        });

        // Click outside closes any open dropdowns
        document.addEventListener('click', (e) => {
            if (!e.target.closest('.dropdown')) {
                this.closeAllDropdowns();
            }
        });
    }

    setupPerDropdownListeners() {
        this.dropdowns.forEach(dropdown => {
            const toggle = dropdown.querySelector('.dropdown-toggle');
            const menu = dropdown.querySelector('.dropdown-menu');
            const items = menu ? Array.from(menu.querySelectorAll('.dropdown-item')) : [];

            // Mouse pointer enters -> mouse mode + open this dropdown (mouse style)
            dropdown.addEventListener('pointerenter', () => {
                this.switchToMouseMode();
                this.openDropdownMouse(dropdown);
            });

            // Mouse pointer leaves -> close mouse dropdown
            dropdown.addEventListener('pointerleave', () => {
                this.closeDropdownMouse(dropdown);
            });

            // Click on toggle (prevent default / bootstrap) -> open as mouse interaction
            if (toggle) {
                toggle.addEventListener('click', (e) => {
                    e.preventDefault();
                    this.switchToMouseMode();
                    // toggle mouse-open
                    if (dropdown.classList.contains('mouse-active')) {
                        this.closeDropdownMouse(dropdown);
                    } else {
                        this.openDropdownMouse(dropdown);
                    }
                });

                // Keyboard on toggle
                toggle.addEventListener('keydown', (e) => {
                    switch (e.key) {
                        case 'Enter':
                        case ' ':
                        case 'ArrowDown':
                            e.preventDefault();
                            this.switchToKeyboardMode();
                            this.openDropdownKeyboard(dropdown);
                            this.focusFirstMenuItem(dropdown);
                            break;
                        case 'Escape':
                            e.preventDefault();
                            this.closeAllDropdowns();
                            toggle.blur();
                            break;
                    }
                });

                // When toggle receives focus via keyboard, open keyboard dropdown (keeps behaviour predictable)
                toggle.addEventListener('focus', () => {
                    if (this.isKeyboardMode) {
                        this.openDropdownKeyboard(dropdown);
                    }
                });
            }

            // Menu item keyboard navigation and click
            items.forEach((item, index) => {
                item.addEventListener('keydown', (e) => {
                    switch (e.key) {
                        case 'ArrowDown':
                            e.preventDefault();
                            this.focusNextMenuItem(dropdown, index);
                            break;
                        case 'ArrowUp':
                            e.preventDefault();
                            this.focusPrevMenuItem(dropdown, index);
                            break;
                        case 'Escape':
                            e.preventDefault();
                            this.closeDropdownKeyboard(dropdown);
                            if (toggle) toggle.focus();
                            break;
                        case 'Tab':
                            // close on tab so focus leaves cleanly
                            this.closeDropdownKeyboard(dropdown);
                            break;
                    }
                });

                item.addEventListener('click', () => {
                    // selecting an item (mouse or keyboard) should close everything
                    this.closeAllDropdowns();
                });
            });

            // FOCUS-OUT: when user tabs out of the dropdown completely, close it
            // (we use a timeout to allow document.activeElement to update)
            dropdown.addEventListener('focusout', () => {
                setTimeout(() => {
                    if (!dropdown.contains(document.activeElement)) {
                        // If it was keyboard-open, close keyboard; if mouse-open, close mouse.
                        if (dropdown.classList.contains('keyboard-active')) {
                            this.closeDropdownKeyboard(dropdown);
                        } else if (dropdown.classList.contains('mouse-active')) {
                            this.closeDropdownMouse(dropdown);
                        }
                    }
                }, 0);
            });
        });
    }

    // Helper to safely blur any currently focused element inside the menubar
    _blurMenuBarActive() {
        try {
            const active = document.activeElement;
            if (active && this.menuBar && this.menuBar.contains(active) && typeof active.blur === 'function') {
                active.blur();
            }
        } catch (err) {
            // ignore any exotic browser edge-cases
        }
    }

    switchToKeyboardMode() {
        this.isKeyboardMode = true;
        if (this.menuBar) this.menuBar.classList.add('keyboard-mode');
        // close any mouse driven dropdowns
        this.dropdowns.forEach(d => this._hideMenu(d));
        this.dropdowns.forEach(d => d.classList.remove('mouse-active'));
        this.currentOpenDropdown = null;
    }

    switchToMouseMode() {
        this.isKeyboardMode = false;
        if (this.menuBar) this.menuBar.classList.remove('keyboard-mode');

        // Blur any focused element inside menubar so keyboard highlight is cleared
        this._blurMenuBarActive();

        // remove any keyboard-opened dropdowns
        this.dropdowns.forEach(d => {
            d.classList.remove('keyboard-active', 'focus-active');
            this._hideMenu(d);
            const toggle = d.querySelector('.dropdown-toggle');
            if (toggle) toggle.setAttribute('aria-expanded', 'false');
        });
        this.currentOpenDropdown = null;
    }

    openDropdownKeyboard(dropdown) {
        // close all others then show this as keyboard-active
        this._closeOthers(dropdown);
        dropdown.classList.remove('mouse-active');
        dropdown.classList.add('keyboard-active');
        this._showMenu(dropdown);
        const toggle = dropdown.querySelector('.dropdown-toggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'true');
        this.currentOpenDropdown = dropdown;
    }

    closeDropdownKeyboard(dropdown) {
        dropdown.classList.remove('keyboard-active', 'focus-active');
        this._hideMenu(dropdown);
        const toggle = dropdown.querySelector('.dropdown-toggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'false');
        if (this.currentOpenDropdown === dropdown) this.currentOpenDropdown = null;
    }

    openDropdownMouse(dropdown) {
        // close other dropdowns, remove keyboard-active flags
        this._closeOthers(dropdown);
        dropdown.classList.remove('keyboard-active', 'focus-active');
        dropdown.classList.add('mouse-active');

        // blur any focused menubar element so the keyboard highlight doesn't remain
        this._blurMenuBarActive();

        this._showMenu(dropdown);
        const toggle = dropdown.querySelector('.dropdown-toggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'true');
        this.currentOpenDropdown = dropdown;
    }

    closeDropdownMouse(dropdown) {
        dropdown.classList.remove('mouse-active');
        this._hideMenu(dropdown);
        const toggle = dropdown.querySelector('.dropdown-toggle');
        if (toggle) toggle.setAttribute('aria-expanded', 'false');
        if (this.currentOpenDropdown === dropdown) this.currentOpenDropdown = null;
    }

    toggleDropdown(dropdown) {
        if (dropdown.classList.contains('keyboard-active')) {
            this.closeDropdownKeyboard(dropdown);
        } else if (dropdown.classList.contains('mouse-active')) {
            this.closeDropdownMouse(dropdown);
        } else {
            // prefer current input mode
            if (this.isKeyboardMode) this.openDropdownKeyboard(dropdown);
            else this.openDropdownMouse(dropdown);
        }
    }

    closeAllDropdowns() {
        this.dropdowns.forEach(d => {
            d.classList.remove('keyboard-active', 'mouse-active', 'focus-active');
            this._hideMenu(d);
            const toggle = d.querySelector('.dropdown-toggle');
            if (toggle) toggle.setAttribute('aria-expanded', 'false');
        });
        this.currentOpenDropdown = null;
    }

    _closeOthers(except) {
        this.dropdowns.forEach(d => {
            if (d !== except) {
                d.classList.remove('keyboard-active', 'mouse-active', 'focus-active');
                this._hideMenu(d);
                const toggle = d.querySelector('.dropdown-toggle');
                if (toggle) toggle.setAttribute('aria-expanded', 'false');
            }
        });
    }

    // Show/hide helpers: manipulate both classes and inline style, also remove any Bootstrap 'show' leftovers
    _showMenu(dropdown) {
        const menu = dropdown.querySelector('.dropdown-menu');
        if (!menu) return;
        menu.style.display = 'block';
        menu.classList.add('show');
        dropdown.classList.add('show');
    }

    _hideMenu(dropdown) {
        const menu = dropdown.querySelector('.dropdown-menu');
        if (!menu) return;
        menu.style.display = '';
        menu.classList.remove('show');
        dropdown.classList.remove('show');
    }

    // Keyboard focus helpers
    focusFirstMenuItem(dropdown) {
        const first = dropdown.querySelector('.dropdown-item');
        if (first) first.focus();
    }

    focusNextMenuItem(dropdown, currentIndex) {
        const items = Array.from(dropdown.querySelectorAll('.dropdown-item'));
        if (!items.length) return;
        const next = (currentIndex + 1) % items.length;
        items[next].focus();
    }

    focusPrevMenuItem(dropdown, currentIndex) {
        const items = Array.from(dropdown.querySelectorAll('.dropdown-item'));
        if (!items.length) return;
        const prev = currentIndex === 0 ? items.length - 1 : currentIndex - 1;
        items[prev].focus();
    }
}

// Initialize after DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window._accessibleNav = new AccessibleNavigation();
});
