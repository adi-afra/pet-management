const API_BASE = "http://localhost:5212/api";

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

//function for switching between pages
function showPage(pageName) {
    pages.forEach((p) => p.classList.remove("is-active"));

    const target = document.querySelector(`.page[data-page="${pageName}"]`);
    if (target) target.classList.add("is-active");

    // dashboard highlight
    dashLinks.forEach((b) => b.classList.remove("active"));
    const activeBtn = document.querySelector(`.dash-link[data-page="${pageName}"]`);
    if (activeBtn) activeBtn.classList.add("active");

    //deleting the navbar if we are in login or register
    const navbar = document.getElementById("mainNavbar");

    if (pageName === "login" || pageName === "registration") {
        navbar?.classList.add("d-none")
    } else {
        navbar?.classList.remove("d-none")
    }
}

dashLinks.forEach((btn) => {
    btn.addEventListener("click", () => {
        showPage(btn.dataset.page);
        closeDash();
    });
});

//show registration page
const navRegistrationBtn = document.getElementById("registrationPage");
navRegistrationBtn?.addEventListener("click", () => {
    showPage("registration");
    closeDash();
});

//switches from register to login
const loginBtn = document.getElementById("loginBtn");
loginBtn?.addEventListener("click", () => {
    showPage("login");
    closeDash();
});

//switches from login to register
const signinBtn = document.getElementById("goToRegister");
signinBtn?.addEventListener("click", () => {
    showPage("registration");
    closeDash();
});

//switches from login or register to home
document.querySelectorAll(".backToHome").forEach(btn => {
    btn.addEventListener("click", () => {
        showPage("gallery");
    });
});

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

async function deleteAdoptionMeeting(id) {
    try {
        const response = await fetch(
            `${API_BASE}/Clients/adoptionMeetings/${id}`,
            {
                method: "DELETE",
                credentials: "include"
            }
        );

        if (!response.ok) {
            console.error("Delete failed");
            return;
        }
    } catch (error) {
        console.error("Error deleting meeting:", error);
    }
}

async function deleteSurrendeMeeting(id) {
    try {
        const response = await fetch(
            `${API_BASE}/Clients/surrenderMeetings/${id}`,
            {
                method: "DELETE",
                credentials: "include"
            }
        );
        if (!response.ok) {
            console.error("Delete failed");
            return;
        }
    } catch (error) {
        console.error("Error deleting surrender meeting:", error);
    }
}

function makeMeetingCard(meeting) {
    // main card
    const card = document.createElement("div");
    card.className = "card shadow-sm mb-3";

    const cardBody = document.createElement("div");
    cardBody.className = "card-body";

    // header section
    const headerDiv = document.createElement("div");
    headerDiv.className = "d-flex justify-content-between";

    const title = document.createElement("h5");
    title.className = "card-title mb-1";

    const heartIcon = document.createElement("i");
    heartIcon.className = "bi bi-heart me-2";

    title.appendChild(heartIcon);
    if (meeting.type == 0) {
        title.appendChild(document.createTextNode("Meet " + meeting.pet.name));
    } else {
        title.appendChild(document.createTextNode("surrender " + meeting.pet.name));
    }

    headerDiv.appendChild(title);

    // date/time
    const formattedDate = new Date(meeting.date).toLocaleDateString();
    const date = document.createElement("p");
    date.className = "mb-1";
    date.innerHTML = `<i class="bi bi-calendar-event me-2"></i>${formattedDate}`;

    // animal
    const animal = document.createElement("p");
    animal.className = "mb-1";
    animal.innerHTML = `<i class="bi bi-paw me-2"></i>Animal: ${meeting.pet.animalType}`;

    // breed
    const breed = document.createElement("p");
    breed.className = "mb-2";
    breed.innerHTML = `<i class="bi bi-tag me-2"></i>Breed: ${meeting.pet.breed}`;

    // delete button
    const deleteButton = document.createElement("button");
    deleteButton.className = "btn btn-danger btn-sm";
    deleteButton.innerHTML = `<i class="bi bi-trash me-1"></i>Delete`;

    // DELETE API CALL
    deleteButton.addEventListener("click", async () => {
        if (meeting.type == 0) {
            await deleteAdoptionMeeting(meeting.id);
        } else {
            await deleteSurrendeMeeting(meeting.id);
        }
        // remove card from UI
        card.remove();
    });

    // assembling the card
    cardBody.appendChild(headerDiv);
    cardBody.appendChild(date);
    cardBody.appendChild(animal);
    cardBody.appendChild(breed);
    cardBody.appendChild(deleteButton);
    card.appendChild(cardBody);

    return card;
}

//calls the make meeting card for every meeting it has gotten by calling the api for getting all meetings
async function showMeetings() {
    const container = document.getElementById("meetingsContainer");
    if (!container) return;
    container.innerHTML = "<p class='text-center'>Loading meetings...</p>";

    const userId = await getUserId();
    if (!userId) {
        alert("Please log in to see your meetings.");
        showPage("login");
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Clients/adoptionMeetings/${userId}`, {
            credentials: "include"
        });

        if (!response.ok) {
            console.error("failed");
        }

        const meetings = await response.json();
        container.innerHTML = "";

        if (meetings.length === 0) {
            container.innerHTML = "<p class='text-muted text-center'>No meetings scheduled yet.</p>";
            return;
        }

        meetings.forEach(meeting => {
            container.appendChild(makeMeetingCard(meeting));
        });
    } catch (error) {
        container.innerHTML = "<p class='text-danger'>Error loading meetings.</p>";
        console.error(error);
    }
}

async function getUserId() {
    const res = await fetch(`${API_BASE}/auth/status`, {
        credentials: "include"
    });
    if (!res.ok) return null;
    const data = await res.json();
    return data.id;
}

// --- INITIAL LOAD ---
document.addEventListener("DOMContentLoaded", () => {
    showMeetings();
});