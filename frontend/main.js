
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




