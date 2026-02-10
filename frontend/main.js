
// Filters (to keep active button)

const filterButtons = document.querySelectorAll(".filter-btn");

filterButtons.forEach((button) => {
  button.addEventListener("click", () => {
    filterButtons.forEach((btn) => btn.classList.remove("active"));
    button.classList.add("active");
  });
});



// Dashboard dropdown (open/close)

const menuBtn = document.getElementById("menuBtn");
const dashPanel = document.getElementById("dashPanel");
const overlay = document.getElementById("overlay");
const closeBtn = document.getElementById("closeBtn");

function openDash() {
  overlay.classList.remove("hidden");
  dashPanel.classList.remove("hidden");

  // allows CSS animation to run
  requestAnimationFrame(() => dashPanel.classList.add("show"));
}

function closeDash() {
  dashPanel.classList.remove("show");
  overlay.classList.add("hidden");

  setTimeout(() => dashPanel.classList.add("hidden"), 220);
}

if (menuBtn && dashPanel && overlay && closeBtn) {
  menuBtn.addEventListener("click", openDash);
  overlay.addEventListener("click", closeDash);
  closeBtn.addEventListener("click", closeDash);
}


// Page switching (show/hide pages)

const pages = document.querySelectorAll(".page");
const dashLinks = document.querySelectorAll(".dash-link[data-page]");

function showPage(pageName) {
  pages.forEach((p) => p.classList.remove("is-active"));

  const target = document.querySelector(`.page[data-page="${pageName}"]`);
  if (target) target.classList.add("is-active");

  // dashboard highlight
  dashLinks.forEach((b) => b.classList.remove("active"));
  const activeBtn = document.querySelector(`.dash-link[data-page="${pageName}"]`);
  if (activeBtn) activeBtn.classList.add("active");
}

dashLinks.forEach((btn) => {
  btn.addEventListener("click", () => {
    showPage(btn.dataset.page);
    closeDash();
  });
});



// Logout (demo, still needs work)

const logoutBtn = document.getElementById("logoutBtn");
if (logoutBtn) {
  logoutBtn.addEventListener("click", () => {
    alert("Logged out (demo)");
    closeDash();
  });
}



//Search input handling (show results after searching in the box)

const searchInput = document.querySelector('.search input');

searchInput?.addEventListener('keydown', (e) => {
  if (e.key === 'Enter') {
    const query = searchInput.value.trim();
    if (query !== "") {
      showPage("results");
      
      //Close the filter modal if it is open
      filterModal.style.display = "none"; 
    }
  }
});



// Filter modal open/close

const filterModal = document.getElementById("filterModal");
const openFiltersBtn = document.getElementById("openFiltersModal");
const closeFiltersBtn = document.getElementById("closeFiltersModal");

// Open filter modal
openFiltersBtn?.addEventListener("click", () => {
  filterModal.style.display = "flex";
});

// Close filter modal
closeFiltersBtn?.addEventListener("click", () => {
  filterModal.style.display = "none";
});

// Click outside to close
filterModal?.addEventListener("click", (e) => {
  if (e.target === filterModal) {
    filterModal.style.display = "none";
  }
});

//Filter button logic

//Ensure only one filter button is active per secton
filterModal?.querySelectorAll(".filter-btn").forEach((button) => {
  button.addEventListener(
    "click",
    (e) => {
      // Stop the global filterButtons listener
      e.stopImmediatePropagation();

      const section = button.closest(".filter-section");
      if (!section) return;

      // Remove active state from all buttons in the same section
      section.querySelectorAll(".filter-btn").forEach((btn) =>
        btn.classList.remove("active")
      );

      //Activate the selected button
      button.classList.add("active");
    },
    true // Capture phase so this runs first
  );
});

// Reset filter to default state
const resetBtn = document.querySelector(".filter-actions .reset-btn");

resetBtn?.addEventListener("click", () => {
  document.querySelectorAll(".filter-section").forEach((section) => {
    const buttons = section.querySelectorAll(".filter-btn");

    //Clear all active states
    buttons.forEach((btn) => btn.classList.remove("active"));

    //Set the first option (All) as default
    buttons[0]?.classList.add("active");
  });
});

// Show results and close modal

const showResultsBtn = document.querySelector(".filter-actions .show-btn");

showResultsBtn?.addEventListener("click", () => {
  // Navigate to results page
  showPage("results");

  // Close filter modal
  filterModal.style.display = "none";
});

