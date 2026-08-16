// ---------- Services (Desktop) ----------
const servicesDropdown = document.getElementById("servicesDropdown");
const servicesButton = document.getElementById("servicesBtn");
const servicesMenu = document.getElementById("servicesMenu");
const servicesIcon = document.getElementById("servicesIcon");

// ---------- About (Desktop) ----------
const aboutDropdown = document.getElementById("aboutDropdown");
const aboutButton = document.getElementById("aboutBtn");
const aboutMenu = document.getElementById("aboutMenu");
const aboutIcon = document.getElementById("aboutIcon");

let currentOpen = null;

function initDropdown(dropdown, button, menu, icon) {
    function openMenu() {
        if (currentOpen && currentOpen !== closeMenu) { currentOpen(); }

        menu.classList.remove("opacity-0", "invisible", "translate-y-2");
        menu.classList.add("opacity-100", "visible", "translate-y-0");
        icon.classList.add("rotate-180");

        currentOpen = closeMenu;
    }

    function closeMenu() {
        menu.classList.add("opacity-0", "invisible", "translate-y-2");
        menu.classList.remove("opacity-100", "visible", "translate-y-0");
        icon.classList.remove("rotate-180");

        if (currentOpen === closeMenu) {
            currentOpen = null;
        }
    }

    function toggleMenu() {
        menu.classList.contains("visible") ? closeMenu() : openMenu();
    }

    // Click
    button.addEventListener("click", function (e) {
        e.stopPropagation();
        toggleMenu();
    });

    // Hover (Desktop only)
    dropdown.addEventListener("mouseenter", () => {
        if (window.matchMedia("(hover: hover)").matches) {
            openMenu();
        }
    });

    dropdown.addEventListener("mouseleave", () => {
        if (window.matchMedia("(hover: hover)").matches) {
            closeMenu();
        }
    });

    // Outside Click
    document.addEventListener("click", function (e) {
        if (!dropdown.contains(e.target)) {
            closeMenu();
        }
    });
}

// Initialize
initDropdown(servicesDropdown, servicesButton, servicesMenu, servicesIcon);

initDropdown(aboutDropdown, aboutButton, aboutMenu, aboutIcon);