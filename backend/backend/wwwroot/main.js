const API_BASE = "http://localhost:5212/api";

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

    if (pageName === "gallery") {
        loadPetGallery();
    }


    if (pageName === "dashboard") {
        loadSavedPets();
    }

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

// FIND your existing searchInput listener and UPDATE it:
searchInput?.addEventListener('keydown', async (e) => {
    if (e.key === 'Enter') {
        const query = searchInput.value.trim();
        showPage("results");

        // Actually load the pets from the backend
        loadPetGallery(query);

        if (filterModal) filterModal.style.display = "none";
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

async function deleteSurrenderMeeting(id) {
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
    const card = document.createElement("div");
    card.className = "card shadow-sm mb-3 border-0 rounded-3";

    const cardBody = document.createElement("div");
    cardBody.className = "card-body";

    // HEADER (icon + title)
    const headerDiv = document.createElement("div");
    headerDiv.className = "d-flex align-items-center";

    const title = document.createElement("h6");
    title.className = "fw-bold mb-0";

    const iconClass = meeting.type === 0
        ? "bi-heart-fill text-danger"
        : "bi-box-arrow-up text-primary";

    const prefix = meeting.type === 0 ? "Meet" : "Surrender";

    title.innerHTML = `<i class="bi ${iconClass} me-2"></i>${prefix} ${meeting.pet.name}`;
    headerDiv.appendChild(title);

    // TOP ROW (header + badge)
    const topRow = document.createElement("div");
    topRow.className = "d-flex justify-content-between align-items-center mb-2";

    const badge = document.createElement("span");
    badge.className = "badge bg-light text-dark small";
    badge.innerText = meeting.pet.animalType;

    topRow.appendChild(headerDiv);
    topRow.appendChild(badge);

    // DATE
    const dateStr = new Date(meeting.date).toLocaleString([], {
        dateStyle: 'medium',
        timeStyle: 'short'
    });

    const dateP = document.createElement("p");
    dateP.className = "small text-muted mb-1";
    dateP.innerHTML = `<i class="bi bi-calendar3 me-2"></i>${dateStr}`;

    // BREED
    const breedP = document.createElement("p");
    breedP.className = "small text-muted mb-3";
    breedP.innerHTML = `<i class="bi bi-tag me-2"></i>${meeting.pet.breed}`;

    // DELETE BUTTON
    const deleteButton = document.createElement("button");
    deleteButton.className = "btn btn-outline-danger btn-sm w-100 rounded-pill";
    deleteButton.innerHTML = `<i class="bi bi-trash3 me-1"></i> Cancel Meeting`;

    deleteButton.addEventListener("click", async () => {
        const endpoint = meeting.type === 0
            ? "adoptionMeetings"
            : "surrenderMeetings";

        if (confirm(`Are you sure you want to cancel the meeting for ${meeting.pet.name}?`)) {
            try {
                const response = await fetch(`${API_BASE}/Clients/${endpoint}/${meeting.id}`, {
                    method: "DELETE",
                    credentials: "include"
                });

                if (response.ok) {
                    card.remove();
                }
            } catch (error) {
                console.error("Error deleting:", error);
            }
        }
    });

    // APPEND EVERYTHING
    cardBody.appendChild(topRow);
    cardBody.appendChild(dateP);
    cardBody.appendChild(breedP);
    cardBody.appendChild(deleteButton);

    card.appendChild(cardBody);

    return card;
}

// 2. Fetch and Display the list
async function showSurrenderRequests() {
    const container = document.getElementById("surrendersRequestContainer");
    if (!container) return;

    const userId = await getUserId();
    if (!userId) return;

    try {
        const res = await fetch(`${API_BASE}/Clients/surrenderMeetings/${userId}`, {
            credentials: "include"
        });
        if (!res.ok) throw new Error("Failed to fetch surrender requests");

        const requests = await res.json();
        container.innerHTML = "";

        requests.forEach(requests => {
            // mark type = 1 for surrender
            requests.type = 1;
            container.appendChild(makeMeetingCard(requests));
        });

        if (requests.length === 0) {
            container.innerHTML = "<p class='text-center text-muted py-3'>No active surrender requests.</p>";
        }

    } catch (err) {
        console.error(err);
        container.innerHTML = "<p class='text-center text-danger py-3'>Failed to load requests.</p>";
    }
}

// Example formatDateTime function (same as booked meetings)
function formatDateTime(dateStr) {
    const date = new Date(dateStr);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
}

// Helper to delete and remove from UI
async function deleteSurrenderRequest(id, btn) {
    if (confirm("Cancel this surrender request?")) {
        const success = await deleteSurrenderMeeting(id); // Using your existing delete function
        btn.closest('.card').remove();
    }
}

//calls the make meeting card for every meeting it has gotten by calling the api for getting all meetings
async function showMeetings() {
    const container = document.getElementById("meetingsContainer");
    if (!container) return;

    const userId = await getUserId();
    if (!userId) return;

    try {
        const response = await fetch(`${API_BASE}/Clients/adoptionMeetings/${userId}`, {
            credentials: "include"
        });

        if (response.ok) {
            const meetings = await response.json();
            container.innerHTML = ""; 
            meetings.forEach(meeting => {
                container.appendChild(makeMeetingCard(meeting));
            });
        }
    } catch (err) {
        console.error("Error loading meetings:", err);
    }
}


async function getUserId() {
    try {
        const response = await fetch(`${API_BASE}/Auth/status`, {
            credentials: "include" 
        });

        if (response.ok) {
            const data = await response.json();
            return data.id; 
        }
        return null;
    } catch (err) {
        console.error("Auth check failed:", err);
        return null;
    }
}

// --- MODAL SELECTORS ---
const surrenderFormModal = document.getElementById("surrenderModal"); // The "Give up a pet" form
const surrenderRequestsModal = document.getElementById("surrenderRequestsModal"); // The "View my requests" list
const adoptionMeetingsModal = document.getElementById("adoptionMeetingsModal");

document.getElementById("closeSurrendersModal")?.addEventListener("click", () => {
    surrenderFormModal.style.display = "none";
});

document.getElementById("closeSurrendersRequestModal")?.addEventListener("click", () => {
    surrenderRequestsModal.style.display = "none";
});

// --- FIX 3: Adoption Meetings Modal ---
document.getElementById("closeMeetingsModal")?.addEventListener("click", () => {
    adoptionMeetingsModal.style.display = "none";
});

// --- OPENING LOGIC ---
document.getElementById("openSurrenderFormBtn")?.addEventListener("click", () => {
    surrenderFormModal.style.display = "flex";
});

document.getElementById("statCardSurrenders")?.addEventListener("click", () => {
    surrenderRequestsModal.style.display = "flex";
    showSurrenderRequests();
});

document.getElementById("statCardMeetings")?.addEventListener("click", () => {
    adoptionMeetingsModal.style.display = "flex";
    showMeetings();
});

// --- CLICK OUTSIDE TO CLOSE (Consolidated) ---
[surrenderFormModal, surrenderRequestsModal, adoptionMeetingsModal].forEach(modal => {
    modal?.addEventListener("click", (e) => {
        if (e.target === modal) modal.style.display = "none";
    });
});

// Open Adoption Meetings from Dashboard
document.getElementById("statCardMeetings")?.addEventListener("click", () => {
    document.getElementById("adoptionMeetingsModal").style.display = "flex";
    showMeetings(); 
});

// Close logic for the new modals
document.getElementById("closeMeetingsModal")?.addEventListener("click", () => {
    document.getElementById("adoptionMeetingsModal").style.display = "none";
});

// Click outside to close for Meetings and Surrenders modals
[document.getElementById("adoptionMeetingsModal"), document.getElementById("surrenderRequestsModal")].forEach(modal => {
    modal?.addEventListener("click", (e) => {
        if (e.target === modal) {
            modal.style.display = "none";
        }
    });
});



async function loadPetGallery(searchQuery = "", filterType = "all") {
    const isResultsPage = document.querySelector('.page.is-active')?.dataset.page === "results";

    const container = isResultsPage
        ? document.getElementById("resultsContainer")
        : document.getElementById("petsContainer");
    if (!container) return;

    // ✅ start fade out
    container.classList.add("fade-out");

    setTimeout(async () => {

        container.innerHTML = "";

        const userId = await getUserId();

        // If not logged in, show the message and STOP
        if (!userId) {
            container.innerHTML = `
                <div class="col-12 text-center py-5">
                    <i class="bi bi-person-lock" style="font-size: 3rem; color: #ccc;"></i>
                    <h4 class="mt-3">Members Only</h4>
                    <p class="text-muted">Please log in to see our adorable pets!</p>
                    <button class="btn btn-warning rounded-pill px-4" onclick="showPage('login')">Login</button>
                </div>`;

            // ✅ fade back in
            container.classList.remove("fade-out");
            container.classList.add("fade-in");
            return;
        }

        try {
            const [petsRes, savedRes] = await Promise.all([
                fetch(`${API_BASE}/Pets`),
                fetch(`${API_BASE}/Pets/savedPets/${userId}`)
            ]);

            const pets = await petsRes.json();
            let savedPetIds = [];

            if (savedRes.ok) {
                const savedData = await savedRes.json();
                savedPetIds = savedData.map(s => s.petId);
            }

            // --- FILTERING LOGIC ---
            let filteredPets = pets.filter(pet => pet.status == 1 || pet.status === "Registered");

            if (filterType !== "all") {
                filteredPets = filteredPets.filter(p => p.animalType === filterType);
            }

            if (searchQuery) {
                filteredPets = filteredPets.filter(p =>
                    p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
                    p.breed.toLowerCase().includes(searchQuery.toLowerCase())
                );
            }

            if (filteredPets.length === 0) {
                container.innerHTML = `<div class="col-12 text-center"><p>No pets found.</p></div>`;

                container.classList.remove("fade-out");
                container.classList.add("fade-in");
                return;
            }

            filteredPets.forEach(pet => {
                const isSaved = savedPetIds.includes(pet.id);
                const iconClass = isSaved ? "bi-bookmark-fill is-saved" : "bi-bookmark";

                const card = `
                    <div class="col-12 col-md-6 col-lg-4 mb-4">
                        <div class="card h-100 shadow-sm border-0 pet-card">
                            <img src="${pet.imageUrl || 'images/placeholder.jpg'}" class="card-img-top pet-img">
                    
                            <div class="card-body d-flex justify-content-between align-items-center">
                                <div class="pet-details">
                                    <h5 class="fw-bold mb-1">${pet.name}</h5>
                                    <p class="text-muted mb-0 small">${pet.breed}</p>
                                    <p class="text-secondary mb-0 small">${pet.age} years old</p>
                                </div>

                                <div class="pet-actions d-flex flex-column align-items-center">
                                    <div class="save-icon-wrapper mb-2" onclick="toggleSavePet(${pet.id})">
                                        <i class="bi ${iconClass}" id="save-${pet.id}"></i>
                                    </div>
                                    <button class="btn btn-sm btn-outline-dark rounded-pill px-3" onclick="openBookingModal(${pet.id}, '${pet.name}', '${pet.breed}', '${pet.animalType}')">
                                        Book
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                container.innerHTML += card;
            });

        } catch (err) {
            console.error("Failed to load pets:", err);
        }

        // ✅ fade back in
        container.classList.remove("fade-out");
        container.classList.add("fade-in");

    }, 200);
}


function openBookingModal(id, name, breed, type) {
    // 1. Get the modal element (Make sure you have this in your HTML!)
    const modal = document.getElementById("bookingModal");

    // 2. Store the pet data in the modal's dataset so the "Confirm" button can find it later
    modal.dataset.selectedPetId = id;
    modal.dataset.selectedPetName = name;
    modal.dataset.selectedPetBreed = breed;
    modal.dataset.selectedPetType = type;

    // 3. Update the modal title so the user knows which pet they are booking
    const title = modal.querySelector('h3') || modal.querySelector('.modal-title');
    if (title) title.innerText = `Book Meeting for ${name}`;

    // 4. Show the modal
    modal.style.display = "flex";
}

// Assuming your booking modal has a button with id="confirmBookingBtn"
// and a date input with id="bookingDate"
document.getElementById("confirmBookingBtn")?.addEventListener("click", async () => {
    const modal = document.getElementById("bookingModal");
    const dateInput = document.getElementById("bookingDate");

    const petId = modal.dataset.selectedPetId;
    const petName = modal.dataset.selectedPetName;
    const petBreed = modal.dataset.selectedPetBreed;
    const petType = modal.dataset.selectedPetType;
    const selectedDate = dateInput.value;

    if (!selectedDate) {
        return alert("Please select a date and time.");
    }

    // Call your existing booking function
    await bookMeeting(petId, selectedDate, petType, petName, petBreed);

    // Close modal on success
    modal.style.display = "none";
});


async function bookMeeting(petId, date, petType, petName, petBreed) {
    const userId = await getUserId();
    if (!userId) return alert("Please log in first!");

    const meetingData = {
        date: date,
        userId: userId,
        type: 0,
        pet: { // You still send the whole object as requested
            "$type": petType,
            "id": petId,
            "name": petName,
            "breed": petBreed,
            "age": 0
        }
    };

    try {
        // CHANGED TO PUT and added petId to the URL
        const response = await fetch(`${API_BASE}/Pets/bookPet/${petId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(meetingData),
            credentials: "include"
        });

        if (response.ok) {
            alert("Meeting booked successfully!");
            showMeetings();
        } else {
            const error = await response.json();
            alert("Error: " + (error.detail || "Booking failed"));
        }
    } catch (err) {
        console.error("Booking error:", err);
    }
}


// Login
// Login logic with on-page feedback
const loginButton = document.getElementById("loginSubmitBtn");

loginButton?.addEventListener("click", async (e) => {
    e.preventDefault();

    const responseEl = document.getElementById("loginResponse"); 
    const username = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;

    // Basic validation before hitting the server
    if (!username || !password) {
        if (responseEl) {
            responseEl.style.color = "orange";
            responseEl.innerText = "Please enter both email and password.";
        }
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/Clients/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
            credentials: "include"
        });

        if (responseEl) {
            if (res.ok) {
                // SUCCESS
                responseEl.style.color = "green";
                responseEl.innerText = "Login successful! Redirecting...";

                // Small delay so they can read the success message
                setTimeout(() => {
                    showPage("gallery");
                    responseEl.innerText = "";
                }, 1500);

            } else {
                // FAILURE (Unauthorized / Wrong Password)
                const errorData = await res.json();
                responseEl.style.color = "red";
                responseEl.innerText = errorData.message || "Invalid email or password.";
            }

            // Auto-clear error messages after 5 seconds
            setTimeout(() => {
                if (responseEl.style.color === "red") responseEl.innerText = "";
            }, 5000);
        }

    } catch (err) {
        console.error("Login Error:", err);
        if (responseEl) {
            responseEl.style.color = "red";
            responseEl.innerText = "Connection error. Please try again later.";
        }
    }
});


// Registration Submit Logic
const registerBtn = document.getElementById("registerBtn");

registerBtn?.addEventListener("click", async (e) => {
    e.preventDefault();

    const responseEl = document.getElementById("response");

    // Exact IDs from your HTML
    const username = document.getElementById("regFullName").value;
    const email = document.getElementById("regEmail").value;
    const password = document.getElementById("regPassword").value;
    const confirmPassword = document.getElementById("regConfirmPassword").value;

    // Necessary check for matching passwords
    if (password !== confirmPassword) {
        responseEl.style.color = "orange";
        responseEl.innerText = "Passwords do not match!";
        return;
    }

    // Basic check for empty fields
    if (!username || !email || !password) {
        responseEl.style.color = "orange";
        responseEl.innerText = "Please fill in all fields.";
        return;
    }

    try {
        const response = await fetch("http://localhost:5212/api/Clients/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                Username: username,
                Email: email,
                Password: password
            })
        });

        const result = await response.json();

        if (response.ok) {
            responseEl.style.color = "green";
            responseEl.innerText = result.message || "Registration successful!";

            setTimeout(() => {
                showPage("login");
                responseEl.innerText = "";
            }, 2000);
        } else {
            responseEl.style.color = "red";
            responseEl.innerText = result.message || "Registration failed.";
        }
    } catch (err) {
        responseEl.style.color = "red";
        responseEl.innerText = "Server error. Is the backend running?";
    }
});

const logoutBtn = document.getElementById("logoutBtn");

logoutBtn?.addEventListener("click", async () => {
    try {
        const response = await fetch(`${API_BASE}/Clients/logout`, {
            method: "DELETE",
            credentials: "include" // REQUIRED to clear the session cookie
        });

        if (response.ok) {
            alert("Logged out successfully.");
            window.location.reload(); // Refresh to clear app state
        }
    } catch (err) {
        console.error("Logout failed:", err);
    }
});


// Add this to the bottom of main.js
const surrenderSubmitBtn = document.getElementById("surrenderSubmitBtn");

surrenderSubmitBtn?.addEventListener("click", async (e) => {
    e.preventDefault();

    const userId = await getUserId();
    if (!userId) return alert("Please log in first!");

    // Keys must be LOWERCASE to match your C# GetProperty calls
    const surrenderData = {
        animalType: document.getElementById("surrenderAnimalType").value, // "Dog" or "Cat"
        name: document.getElementById("surrenderPetName").value,
        age: parseInt(document.getElementById("surrenderPetAge").value),
        breed: document.getElementById("surrenderPetBreed").value,
        date: document.getElementById("surrenderDate").value,
        userId: userId
    };

    // Validation
    if (!surrenderData.name || !surrenderData.date) {
        return alert("Please fill in the pet name and date.");
    }

    try {
        const response = await fetch(`${API_BASE}/Clients/surrenderMeetings`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(surrenderData),
            credentials: "include"
        });

        if (response.ok) {
            alert("Surrender request submitted!");
            document.getElementById("surrenderModal").style.display = "none";
            // Optional: Refresh the dashboard stats
        } else {
            const error = await response.json();
            alert("Error: " + (error.detail || "Submission failed"));
        }
    } catch (err) {
        console.error("Surrender error:", err);
    }
});



async function toggleSavePet(petId) {
    const userId = await getUserId();
    if (!userId) return showPage("login");

    try {
        const response = await fetch(`${API_BASE}/Pets/savePet`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userId, petId }),
            credentials: "include"
        });

        if (!response.ok) return;

        // Find **all icons** for this pet in gallery and saved pets
        const icons = document.querySelectorAll(`#save-${petId}`);
        const isUnsaving = icons[0].classList.contains("bi-bookmark-fill");

        icons.forEach(icon => {
            if (isUnsaving) {
                icon.classList.remove("bi-bookmark-fill", "is-saved");
                icon.classList.add("bi-bookmark");
            } else {
                icon.classList.remove("bi-bookmark");
                icon.classList.add("bi-bookmark-fill", "is-saved");
            }
        });

        // If unsaving from saved pets page, remove the card immediately
        if (isUnsaving) {
            const savedCard = document.getElementById("savedPetsContainer")?.querySelector(`#save-${petId}`)?.closest(".col-12");
            if (savedCard) savedCard.remove();

            // Show placeholder if empty
            const container = document.getElementById("savedPetsContainer");
            if (container && container.children.length === 0) {
                container.innerHTML = `
                    <div class="col-12 text-center py-4">
                        <p class="text-muted small">Your saved pets will appear here.</p>
                    </div>`;
            }
        }

    } catch (err) {
        console.error("Error toggling saved pet:", err);
    }
}


async function loadSavedPets() {
    const container = document.getElementById("savedPetsContainer");
    if (!container) return;

    container.innerHTML = "";

    const userId = await getUserId();

    // 🚫 Not logged in
    if (!userId) {
        container.innerHTML = `
            <div class="col-12 text-center py-4">
                <p class="text-muted small">Please log in to see saved pets.</p>
            </div>
        `;
        return;
    }

    try {
        // ✅ SAME LOGIC AS GALLERY
        const [petsRes, savedRes] = await Promise.all([
            fetch(`${API_BASE}/Pets`),
            fetch(`${API_BASE}/Pets/savedPets/${userId}`, {
                credentials: "include"
            })
        ]);

        if (!petsRes.ok) throw new Error("Failed to fetch pets");
        if (!savedRes.ok) throw new Error("Failed to fetch saved pets");

        const allPets = await petsRes.json();
        const savedData = await savedRes.json();

        // Extract saved pet IDs
        const savedIds = savedData.map(s => s.petId);

        // Filter full pet objects
        const savedPets = allPets.filter(p => savedIds.includes(p.id));

        // 🚫 No saved pets
        if (savedPets.length === 0) {
            container.innerHTML = `
                <div class="col-12 text-center py-4">
                    <p class="text-muted small">Your saved pets will appear here.</p>
                </div>
            `;
            return;
        }

        // ✅ Render cards (same style as gallery)
        savedPets.forEach(pet => {
            const card = `
                <div class="col-12 col-md-6 col-lg-4 mb-4">
                    <div class="card h-100 shadow-sm border-0 pet-card">
                        <img src="${pet.imageUrl || 'images/placeholder.jpg'}" class="card-img-top pet-img">

                        <div class="card-body d-flex justify-content-between align-items-center">
                            <div class="pet-details">
                                <h5 class="fw-bold mb-1">${pet.name}</h5>
                                <p class="text-muted mb-0 small">${pet.breed}</p>
                                <p class="text-secondary mb-0 small">${pet.age} years old</p>
                            </div>

                            <div class="pet-actions d-flex flex-column align-items-center">
                                <div class="save-icon-wrapper mb-2" onclick="toggleSavePet(${pet.id})">
                                    <i class="bi bi-bookmark-fill is-saved" id="save-${pet.id}"></i>
                                </div>

                                <button class="btn btn-sm btn-outline-dark rounded-pill px-3"
                                    onclick="openBookingModal(${pet.id}, '${pet.name}', '${pet.breed}', '${pet.animalType}')">
                                    Book
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;

            container.innerHTML += card;
        });

    } catch (error) {
        console.error("Error loading saved pets:", error);
        container.innerHTML = `
            <div class="col-12 text-center py-4">
                <p class="text-danger small">Failed to load saved pets.</p>
            </div>
        `;
    }
}


// --- INITIALIZATION ---
window.addEventListener("DOMContentLoaded", () => {

    // 1. SET UP FILTER BUTTONS
    const filterButtons = document.querySelectorAll(".filter-container .filter-btn");

    filterButtons.forEach((button) => {
        button.addEventListener("click", () => {
            // UI: Move the 'active' class
            filterButtons.forEach((btn) => btn.classList.remove("active"));
            button.classList.add("active");

            // Data: Get the animal type (Dog, Cat, or all)
            const type = button.getAttribute("data-type") || "all";

            // Action: Reload the gallery with the filter
            loadPetGallery("", type);
        });
    });

    // 2. LOAD INITIAL PAGE
    showPage("gallery");
});