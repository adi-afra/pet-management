// Select all filter buttons
const filterButtons = document.querySelectorAll('.filter-btn');

// Add click event to each button
filterButtons.forEach(button => {
    button.addEventListener('click', () => {

        // Remove active class from all buttons
        filterButtons.forEach(btn => {
            btn.classList.remove('active');
        });

        // Add active class to the clicked button
        button.classList.add('active');
    });
});


// Dashboard menu (Brandon)


const menuBtn = document.getElementById("menuBtn");
const dashPanel = document.getElementById("dashPanel");
const overlay = document.getElementById("overlay");
const closeBtn = document.getElementById("closeBtn");

function openDash() {
    overlay.classList.remove("hidden");
    dashPanel.classList.remove("hidden");

    // allows CSS animation to run
    requestAnimationFrame(() => {
        dashPanel.classList.add("show");
    });
}

function closeDash() {
    dashPanel.classList.remove("show");
    overlay.classList.add("hidden");

    setTimeout(() => {
        dashPanel.classList.add("hidden");
    }, 220);
}

// Safety check so nothing breaks
if (menuBtn && dashPanel && overlay && closeBtn) {
    menuBtn.addEventListener("click", openDash);
    overlay.addEventListener("click", closeDash);
    closeBtn.addEventListener("click", closeDash);
}

// Page switching (shows and hides sections)
const dashLinks = document.querySelectorAll(".dash-link[data-page]");
const pages = document.querySelectorAll(".page");

function showPage(pageName) {
  pages.forEach(p => p.classList.remove("is-active"));

  const target = document.querySelector(`.page[data-page="${pageName}"]`);
  if (target) target.classList.add("is-active");
}

dashLinks.forEach(btn => {
  btn.addEventListener("click", () => {
    // updates active highlight in dashboard
    dashLinks.forEach(b => b.classList.remove("active"));
    btn.classList.add("active");

    // shows the selected page
    showPage(btn.dataset.page);

    // closes dashboard menu after choosing
    closeDash();
  });
});



