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

class AccessibleNavigation {
	constructor() {
		this.menuBar = document.querySelector('.menu-bar');
		this.dropdowns = document.querySelectorAll('.dropdown');
		this.isKeyboardMode = false;
		this.currentOpenDropdown = null;

		this.init();
	}

	init() {
		this.setupEventListeners();
		this.setupKeyboardNavigation();
	}

	setupEventListeners() {
		// Track mouse usage
		document.addEventListener('mousemove', () => {
			if (this.isKeyboardMode) {
				this.switchToMouseMode();
			}
		});

		// Track keyboard usage
		document.addEventListener('keydown', (e) => {
			if (e.key === 'Tab' || e.key === 'Enter' || e.key === 'Escape' || e.key === 'ArrowDown' || e.key === 'ArrowUp') {
				if (!this.isKeyboardMode) {
					this.switchToKeyboardMode();
				}
			}
		});

		// Handle clicks outside dropdowns
		document.addEventListener('click', (e) => {
			if (!e.target.closest('.dropdown')) {
				this.closeAllDropdowns();
			}
		});
	}

	setupKeyboardNavigation() {
		this.dropdowns.forEach(dropdown => {
			const toggle = dropdown.querySelector('.dropdown-toggle');
			const menu = dropdown.querySelector('.dropdown-menu');
			const menuItems = menu.querySelectorAll('.dropdown-item');

			// Handle dropdown toggle
			toggle.addEventListener('click', (e) => {
				e.preventDefault();
				this.toggleDropdown(dropdown);
			});

			toggle.addEventListener('keydown', (e) => {
				switch (e.key) {
					case 'Enter':
					case ' ':
					case 'ArrowDown':
						e.preventDefault();
						this.openDropdown(dropdown);
						this.focusFirstMenuItem(dropdown);
						break;
					case 'Escape':
						this.closeAllDropdowns();
						break;
				}
			});

			// Handle menu item navigation
			menuItems.forEach((item, index) => {
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
							this.closeDropdown(dropdown);
							toggle.focus();
							break;
						case 'Tab':
							this.closeDropdown(dropdown);
							break;
					}
				});

				item.addEventListener('click', () => {
					this.closeAllDropdowns();
				});
			});

			// Handle focus events
			toggle.addEventListener('focus', () => {
				if (this.isKeyboardMode) {
					this.closeOtherDropdowns(dropdown);
				}
			});
		});
	}

	switchToKeyboardMode() {
		this.isKeyboardMode = true;
		this.menuBar.classList.add('keyboard-mode');
		this.closeAllDropdowns();
	}

	switchToMouseMode() {
		this.isKeyboardMode = false;
		this.menuBar.classList.remove('keyboard-mode');
		this.closeAllDropdowns();
	}

	toggleDropdown(dropdown) {
		if (dropdown.classList.contains('keyboard-active')) {
			this.closeDropdown(dropdown);
		} else {
			this.openDropdown(dropdown);
		}
	}

	openDropdown(dropdown) {
		this.closeOtherDropdowns(dropdown);
		dropdown.classList.add('keyboard-active');
		const toggle = dropdown.querySelector('.dropdown-toggle');
		toggle.setAttribute('aria-expanded', 'true');
		this.currentOpenDropdown = dropdown;
	}

	closeDropdown(dropdown) {
		dropdown.classList.remove('keyboard-active');
		const toggle = dropdown.querySelector('.dropdown-toggle');
		toggle.setAttribute('aria-expanded', 'false');
		if (this.currentOpenDropdown === dropdown) {
			this.currentOpenDropdown = null;
		}
	}

	closeOtherDropdowns(exceptDropdown) {
		this.dropdowns.forEach(dropdown => {
			if (dropdown !== exceptDropdown) {
				this.closeDropdown(dropdown);
			}
		});
	}

	closeAllDropdowns() {
		this.dropdowns.forEach(dropdown => {
			this.closeDropdown(dropdown);
		});
	}

	focusFirstMenuItem(dropdown) {
		const firstItem = dropdown.querySelector('.dropdown-item');
		if (firstItem) {
			firstItem.focus();
		}
	}

	focusNextMenuItem(dropdown, currentIndex) {
		const menuItems = dropdown.querySelectorAll('.dropdown-item');
		const nextIndex = (currentIndex + 1) % menuItems.length;
		menuItems[nextIndex].focus();
	}

	focusPrevMenuItem(dropdown, currentIndex) {
		const menuItems = dropdown.querySelectorAll('.dropdown-item');
		const prevIndex = currentIndex === 0 ? menuItems.length - 1 : currentIndex - 1;
		menuItems[prevIndex].focus();
	}
}

// Initialize the accessible navigation
document.addEventListener('DOMContentLoaded', () => {
	new AccessibleNavigation();
});