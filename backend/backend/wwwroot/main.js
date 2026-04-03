
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


async function goToDashboard() {
    const session = await isUserLoggedIn();

    if (!session.loggedIn) {
        // Show a message in your login page message container
        const responseEl = document.getElementById("loginresponse");
        if (responseEl) {
            responseEl.style.color = "red";
            responseEl.innerText = "You must log in to access the dashboard.";

            // Clear the message after 10 seconds
            setTimeout(() => { responseEl.innerText = ""; }, 10000);
        }

        // Switch to login page
        showPage("login");
        return;
    }

    // User is logged in → show dashboard
    showPage("dashboard");
}

// Attach to dashboard button
document.getElementById("dashboardBTN")?.addEventListener("click", goToDashboard);



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

//calls the make meeting card for every meeting it has gotten by calling the api for getting all meetings
async function showMeetings() {
    const session = await isUserLoggedIn();
    const userId = session.user.userId;

    console.log(userId);
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
const surrenderModal = document.getElementById("surrenderModal");
const openSurrendersBtn = document.getElementById("openSurrendersModal");
const closeSurrendersBtn = document.getElementById("closeSurrendersModal");
const surrenderContainer = document.getElementById("surrendersContainer"); // container inside modal
const addSurrenderMeetingButton = document.getElementById("addSurrenderMeeting");


// Function to fetch and show surrender meetings
async function showSurrenders() {
    
    const session = await isUserLoggedIn();

    
    const userId = session.user.userId || 0;

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

    async function addSurrenders() {



        //getting all the values from the entry fields 
        const petName = document.getElementById("petName").value.trim();
        const petAge = document.getElementById("petAge").value.trim();
        const petBreed = document.getElementById("petBreed").value.trim();
        const petType = document.getElementById("petType").value.trim();
        const meetingDateValue = document.getElementById("meetingDate").value.trim();
        const petImageInput = document.getElementById("petImage");
        const petImage = petImageInput.files[0];



        //getting the message <p>
        const formMessage = document.getElementById("formMessage");

        //check if none of the fields are empty
        if (petName === "" || petAge === "" || petBreed === "" || petType === "" || meetingDateValue === "" || petImage === undefined) {
            formMessage.textContent = "all fields must be filled";
            return
        } else if (Number(petAge) < 0) {
            formMessage.textContent = "Age cannot be negative";
            return;
        }

        // Check meeting date
        const meetingDate = new Date(meetingDateValue);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (meetingDate <= today) {
            formMessage.textContent = "Meeting date must be after today";
            return;
        }

        formMessage.textContent = "";

        const session = await isUserLoggedIn();

        const userId = session.user.userId || 0;
        try {
            //adding the image 
            const formData = new FormData();
            formData.append("file", petImage);

            const res1 = await fetch("http://localhost:5212/api/Pets/upload", {
                method: "POST",
                body: formData
            });

            if (!res1.ok) {
                const errorData = await res1.json();
                console.log("adding meeting failed: " + errorData.message);
                return;
            }

            const uploadData = await res1.json();
            const imageUrl = uploadData.imageUrl;

            // adding the new pet
            const res2 = await fetch("http://localhost:5212/api/Clients/surrenderMeetings", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    "name": petName,
                    "age": Number(petAge),
                    "breed": petBreed,
                    "date": meetingDate,
                    "userId": userId,
                    "animalType": petType,
                    "imageUrl": imageUrl
                })
            });


            if (!res2.ok) {
                const errorData = await res2.json();
                console.log("adding meeting failed: " + errorData.message);
                return;
            }

            showSurrenders();

        } catch (err) {
            console.error(err);
            alert("Something went wrong!");
        }

    }

    // main card
    const formCard = document.createElement("div");
    formCard.className = "card shadow-sm mt-3";

    const formBody = document.createElement("div");
    formBody.className = "card-body d-flex flex-column gap-2";

    //  Name
    const nameInput = document.createElement("input");
    nameInput.className = "form-control";
    nameInput.placeholder = "Pet Name";

    //  Age
    const ageInput = document.createElement("input");
    ageInput.type = "number";
    ageInput.className = "form-control";
    ageInput.placeholder = "Age";
    ageInput.min = "";

    //  Breed
    const breedInput = document.createElement("input");
    breedInput.className = "form-control";
    breedInput.placeholder = "Breed";

    //  Animal Type Dropdown
    const typeSelect = document.createElement("select");
    typeSelect.className = "form-select";

    const types = ["Dog", "Cat", "Rabbit", "Other"];

    types.forEach(type => {
        const option = document.createElement("option");
        option.value = type;
        option.textContent = type;
        typeSelect.appendChild(option);
    });

    //  Date
    const dateInput = document.createElement("input");
    dateInput.type = "date";
    dateInput.className = "form-control";


    // Image label 
    const imageLabel = document.createElement("label");
    imageLabel.className = "form-label";
    imageLabel.textContent = "Upload Image";

    // adding a image upload 
    const imageInput = document.createElement("input");
    imageInput.type = "file";
    imageInput.className = "form-control";
    imageInput.accept = "image/*";
    imageInput.name = "file";

    

    //  Submit button
    const submitBtn = document.createElement("button");
    submitBtn.className = "btn btn-success mt-2";
    submitBtn.textContent = "Submit";

    //  Close button
    const closeBtn = document.createElement("button");
    closeBtn.className = "btn btn-secondary mt-2";
    closeBtn.textContent = "Close";

    // Create a <p> element for errors
    const messageParagraph = document.createElement("p");  
    messageParagraph.className = "text-danger"; 
    messageParagraph.textContent = "";

    //adding IDs to all the entry fields
    nameInput.id = "petName";
    ageInput.id = "petAge";
    breedInput.id = "petBreed";
    typeSelect.id = "petType";
    dateInput.id = "meetingDate";
    messageParagraph.id = "formMessage";
    imageInput.id = "petImage";

    
    //  Submit logic
    submitBtn.addEventListener("click", async () => {
        
        addSurrenders();
    });
    

    //  Close logic
    closeBtn.addEventListener("click", async () => {
        formCard.remove();
        addSurrenderMeetingButton.classList.remove("d-none");
        showSurrenders();
    });

    // assemble form
    formBody.appendChild(nameInput);
    formBody.appendChild(ageInput);
    formBody.appendChild(dateInput);
    formBody.appendChild(breedInput);
    formBody.appendChild(typeSelect);
    formBody.appendChild(imageLabel);
    formBody.appendChild(imageInput);
    formBody.appendChild(submitBtn);
    formBody.appendChild(closeBtn);
    formBody.appendChild(messageParagraph);

    formCard.appendChild(formBody);

    surrenderContainer.appendChild(formCard);
}

// Open modal
openSurrendersBtn?.addEventListener("click", () => {
    surrenderModal.style.display = "flex";
    addSurrenderMeetingButton.classList.remove("d-none");
    showSurrenders();
});

// Close modal
closeSurrendersBtn?.addEventListener("click", () => {
    surrenderModal.style.display = "none";
});

// Click outside to close
surrenderModal?.addEventListener("click", (e) => {
    if (e.target === surrenderModal) {
        surrenderModal.style.display = "none";
    }
});



const PETS_API_BASE = "/api/Pets";

function renderPets(pets, containerId) {
    const container = document.getElementById(containerId);

    if (!pets || pets.length === 0) {
        container.innerHTML = `<div class="col-12"><p class="text-center">No pets found.</p></div>`;
        return;
    }

    container.innerHTML = pets.map(pet => `
    <div class="col-12 col-md-6 col-lg-4">
        <div class="pet-card h-100 clickable-card"
             data-id="${pet.id}"
             data-name="${pet.name}"
             data-age="${pet.age}"
             data-breed="${pet.breed}"
             data-image="${pet.imageUrl}">

            <!-- 🐾 IMAGE -->
            <img class="pet-img"
                 src="${pet.imageUrl}"
                 alt="${pet.name}">
             
            <!-- 🐾 INFO -->
            <div class="info">

                <!-- NAME + SAVE BUTTON ROW -->
                <div class="d-flex justify-content-between align-items-center">
                    <h3 class="mb-0">${pet.name}</h3>

                    <button class="save-btn-inline"
                            data-save-id="${pet.id}"
                            type="button">
                        <i class="bi bi-bookmark" id="save-${pet.id}"></i>
                    </button>
                </div>

                <p class="mt-1">${pet.age} years • ${pet.type}</p>
            </div>

        </div>
    </div>
    `).join("");
}

const filters = {};

// Handle filter button clicks
document.querySelectorAll(".filter-btn").forEach(btn => {
    btn.addEventListener("click", function () {
        const group = this.dataset.group;

        // Remove active class from same group
        document.querySelectorAll(`.filter-btn[data-group="${group}"]`)
            .forEach(b => b.classList.remove("active"));

        this.classList.add("active");

        // Remove previous values
        delete filters[group];
        if (group === "age") {
            delete filters.minAge;
            delete filters.maxAge;
        }

        // If "All" clicked → do nothing (keeps filters empty)
        if (!this.dataset.value && !this.dataset.min) return;

        // Normal filters
        if (this.dataset.value) {
            filters[group] = this.dataset.value;
        }

        // Age filters
        if (this.dataset.min) {
            filters.minAge = this.dataset.min;
        }

        if (this.dataset.max) {
            filters.maxAge = this.dataset.max;
        }
    });
});

async function loadAllPets() {
    try {
        const res = await fetch(`http://localhost:5212/api/Pets/pet`);
        const pets = await res.json();
        renderPets(pets, "petGallery");
    } catch (error) {
        console.error("Error fetching pets:", error);
    }
}

async function loadFilteredPets() {
    
    if (Object.keys(filters).length === 0) {
        loadAllPets();
        return;
    }
    try {
        const queryString = new URLSearchParams(filters).toString();
        const res = await fetch(`http://localhost:5212/api/Pets/filter?${queryString}`);
        const pets = await res.json();
        renderPets(pets, "petGallery");
    } catch (error) {
        console.error("Error fetching pets:", error);
    }
}


async function searchPets() {
    const input = document.getElementById("searchInput");
    const breed = input.value.trim();

    const url = breed
        ? `${PETS_API_BASE}/filter?breed=${encodeURIComponent(breed)}`
        : PETS_API_BASE;

    const res = await fetch(url);
    const pets = await res.json();

    renderPets(pets, "resultsGallery");
    showPage("results");
}

document.addEventListener("DOMContentLoaded", () => {
    const resetBtn = document.getElementById("resetBtn");
    const showFilterResultBtn = document.getElementById("showFilterResultBtn");

    resetBtn?.addEventListener("click", () => {
        console.log("reset burron clicked");
        // Clear filters object
        for (let key in filters) {
            delete filters[key];
        }

        // Reset UI
        document.querySelectorAll(".filter-btn").forEach(btn => {
            btn.classList.remove("active");
        });

        // Re-activate all "All" buttons
        document.querySelectorAll('.filter-btn:not([data-value]):not([data-min])')
            .forEach(btn => btn.classList.add("active"));
    });

    showFilterResultBtn?.addEventListener("click", () => {
        loadFilteredPets();
        filterModal.style.display = "none";
    });

    // Open meeting form logic form
    addSurrenderMeetingButton?.addEventListener("click", () => {
        surrenderContainer.innerHTML = "";
        createMeetingForm();
        addSurrenderMeetingButton.classList.add("d-none");

    });

    loadAllPets();
});



//Login 

const loginForm = document.getElementById("loginsubmit"); // change to your button ID

loginForm?.addEventListener("click", async (e) => {
    e.preventDefault();

    // Get input values and trim spaces/newlines
    const username = document.getElementById("loginusername").value.trim();
    const password = document.getElementById("loginpassword").value.trim();

    // Element to show messages
    const responseEl = document.getElementById("loginresponse");
    

    try {
        const res = await fetch("http://localhost:5212/api/Clients/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ username, password })
        });

        const data = await res.json();

        if (!res.ok) {
            
            // Show backend error message
            responseEl.style.color = "red";
            responseEl.innerText = data.message || "Login failed.";
        } else {
            
            // Successful login
            responseEl.style.color = "green";
            responseEl.innerText = data.message || `Welcome, ${data.username}!`;
            
            //clearing the placeholders
            document.getElementById("loginusername").value = "";
            document.getElementById("loginpassword").value = "";
        }

        // Clear message after 10 seconds
        setTimeout(() => { responseEl.innerText = ""; }, 10000);

    } catch (err) { // ⚡ Make sure this is 'err', not 'e' if you reference 'err'
        console.error("Login error:", err);
        responseEl.style.color = "red";
        responseEl.innerText = "Server error, please try again.";
        setTimeout(() => { responseEl.innerText = ""; }, 10000);
    }
});


// Reusable function to check if user is logged in
async function isUserLoggedIn() {
    try {
        const res = await fetch("http://localhost:5212/api/Clients/session", {
            method: "GET",
            credentials: "include" // ⚡ important to include session cookie
        });

        if (!res.ok) {
            // Unauthorized → user not logged in
            return { loggedIn: false, user: null };
        }

        const data = await res.json();

        // Optional: check if userId exists
        
        if (data.userId) {
            
            return { loggedIn: true, user: data };
            
        } else {
            return { loggedIn: false, user: null };
        }

    } catch (err) {
        console.error("Error checking session:", err);
        return { loggedIn: false, user: null };
    }
}

//Logout
const logoutBTN = document.getElementById("logoutBtn");

logoutBTN?.addEventListener("click", async () => {
    try {
        const res = await fetch("http://localhost:5212/api/Clients/logout", {
            method: "POST",
            credentials: "include"
        });

        const data = await res.json();
        console.log(data.message);

        // Redirect to login page
        showPage("gallery");
        

    } catch (err) {
        console.error("Logout failed:", err);
    }
});

//adoption meeting
document.getElementById("petGallery").addEventListener("click", (e) => {
    const card = e.target.closest(".clickable-card");
    if (!card) return;

    const petData = {
        id: card.dataset.id,
        name: card.dataset.name,
        age: card.dataset.age,
        breed: card.dataset.breed,
        image: card.dataset.image
    };

    console.log("Yes", petData);
    openBookingModal(petData);
});


//modal logic for adopting
const bookingModal = document.getElementById("bookingModal");

let selectedPetId = null;

function openBookingModal(pet) {
    selectedPetId = pet.id;
    
    //image
    document.getElementById("bookingPetImage").src = pet.image;
    
    //details
    const details = document.getElementById("bookingPetDetails");
    details.innerHTML = `
        <h5>${pet.name}</h5>
        <p>Age: ${pet.age}</p>
        <p>Breed: ${pet.breed}</p>
    `;

    // reset previous state
    document.getElementById("bookingDate").value = "";
    document.getElementById("bookingMessage").innerText = "";
    
    bookingModal.style.display = "flex";
}

document.getElementById("closeBookingModal")?.addEventListener("click", () => {
    bookingModal.style.display = "none";
});

//booking an adoption meeting
async function bookAdoptionMeeting(userId, petId, date) {
    try {
        const response = await fetch(`http://localhost:5212/api/Clients/adoptionMeetings/${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
                body: JSON.stringify({
                petId: Number(petId),
                date: date // ISO string e.g., "2026-04-05T10:00:00"
            })
        });

        const data = await response.json();

        if (!response.ok) {
            // show error message from server
            document.getElementById("bookResponse").innerText = data.message || "Something went wrong";
            document.getElementById("bookResponse").style.color = "Red";
            setTimeout(() => { document.getElementById("bookResponse").innerText = ""; }, 20000);
            return false;
        }

        document.getElementById("bookResponse").innerText = data.message;
        document.getElementById("bookResponse").style.color = "Green";
        setTimeout(() => { document.getElementById("bookResponse").innerText = ""; }, 20000);
        console.log(data.meeting);
        return true;

    } catch (error) {
        console.log('Error:', error);
        document.getElementById("bookResponse").innerText = data.message;
        document.getElementById("bookResponse").style.color = "Red";
        setTimeout(() => { document.getElementById("bookResponse").innerText = ""; }, 20000);
        return false;
    }
}

document.getElementById("confirmBooking").addEventListener("click", async () => {
    const session = await isUserLoggedIn();
    if (!session.loggedIn) {
        document.getElementById("bookResponse").innerText = "You must be logged in!";
        document.getElementById("bookResponse").style.color = "Red";
        return;
    }

    const userId = session.user.userId;
    const dateInput = document.getElementById("bookingDate").value;
    

    const date = new Date(dateInput).toISOString();
    // Call your function

    const success = await bookAdoptionMeeting(userId, selectedPetId, date);

    // If booking succeeded, disable the button
    if (success) {
        const confirmBtn = document.getElementById("confirmBooking");
        confirmBtn.disabled = true;
        confirmBtn.textContent = "Booked ✔"; // optional: show feedback
    }
})



//showing the logged in user
/*
async function updateLoginStatus() {
    const statusEl = document.getElementById("whoLogged");
    const session = await isUserLoggedIn();

    if (session.loggedIn) {
        statusEl.innerText = `Logged in as ${session.user.username}`;
    }
}

// Call this on page load
document.addEventListener("DOMContentLoaded", updateLoginStatus);

// Also call it after login/logout
loginForm?.addEventListener("click", async () => {
    updateLoginStatus();
});

logoutBTN?.addEventListener("click", () => {
    updateLoginStatus();
});
*/


/*
const container = document.getElementById("petsContainer");

async function loadPets() {
    const res = await fetch("http://localhost:5212/api/Pets/pet"); // adjust if needed
    const pets = await res.json();

    container.innerHTML = ""; // clear existing content

    pets.forEach(pet => {
        const card = document.createElement("div");
        card.className = "col-12 col-md-6 col-lg-4";

        card.innerHTML = `
            <div class="pet-card h-100">
                <img class="pet-img"
                     src="${pet.imageUrl}"
                     alt="${pet.name}">
                <div class="info">
                    <h3>${pet.name}</h3>
                    <p>${pet.age} years • ${pet.type}</p>
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
*/

