
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



async function goToDashboard() {
    const session = await isUserLoggedIn();

    console.log(session.loggedIn)
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
    const userIdContiner = document.getElementById("loggInId");
    console.log(session.user.userId);
    userIdContiner.innerText = "Logged in as: " + session.user.username;
    return;
}

// Attach to dashboard button
document.getElementById("dashboardBTN")?.addEventListener("click", goToDashboard);



async function deleteAdoptionMeeting(id) {
    try {
        const response = await fetch(
            `http://localhost:5212/api/Clients/adoptionMeetings/${id}`,
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
            `http://localhost:5212/api/Clients/surrenderMeetings/${id}`,
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

    if (meeting.type == 0) {
        // DELETE API CALL
        deleteButton.addEventListener("click", () => {
            deleteAdoptionMeeting(meeting.id);
            // remove card from UI
            card.remove();
        });
    } else {
        // DELETE API CALL
        deleteButton.addEventListener("click", () => {
            deleteSurrenderMeeting(meeting.id);
            // remove card from UI
            card.remove();
        });
    }

    // APPEND EVERYTHING
    cardBody.appendChild(topRow);
    cardBody.appendChild(dateP);
    cardBody.appendChild(breedP);
    cardBody.appendChild(deleteButton);

    card.appendChild(cardBody);

    return card;
}


//registeration
const registerForm = document.getElementById("registerBTN");
registerForm?.addEventListener("click", async (e) => {
    e.preventDefault();

    const username = document.getElementById("regUsername").value;
    const email = document.getElementById("regEmail").value;
    const password = document.getElementById("regPassword").value;

    try {
        const res = await fetch("http://localhost:5212/api/Clients/register", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password, email })
        });

        // Get the element where we display messages
        const responseEl = document.getElementById("response");

        // Try to read the backend response as JSON
        const errorData = await res.json();

        // Check if the response from the backend was NOT successful
        if (!res.ok) {

            // Default message in case backend doesn't provide one
            let message = "registration unsuccessful";

            try {
                // Use the backend message if available, otherwise fallback to default
                message = errorData.message || message;
            }
            //incase of any unexpected error
            catch (e) { }

            // Show the error message to the user
            responseEl.style.color = "red";
            responseEl.innerText = message;


            // Clear the message automatically after 30 seconds

            setTimeout(() => {
                responseEl.innerText = "";
            }, 10000);

        }

        else {
            //default message
            let message = "registration successful";

            try {
                message = errorData.message || message;
            }
            catch (e) { }
            // Show the original message in green
            responseEl.style.color = "green";
            responseEl.innerText = message;

            // After 5 seconds, show redirect message
            setTimeout(() => {
                responseEl.innerText = "Redirecting to login in 10 seconds...";

                // After another 10 seconds, clear message and navigate to login
                setTimeout(() => {
                    responseEl.innerText = "";
                    showPage("login");
                }, 10000);

            }, 5000);
        }
    } catch (err) {
        console.log(err);
    }
});




// Example formatDateTime function (same as booked meetings)
function formatDateTime(dateStr) {
    const date = new Date(dateStr);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
}

// Adoption modal open/close
const adoptionModal = document.getElementById("adoptionMeetingsModal");
const openAdoptionsBtn = document.getElementById("statCardMeetings");
const closeAdoptionsBtn = document.getElementById("closeAdoptionMeetingsModal");
const adoptionContainer = document.getElementById("meetingsContainer");

//calls the make meeting card for every meeting it has gotten by calling the api for getting all meetings
async function showMeetings() {
    const session = await isUserLoggedIn();
    const userId = session.user.userId;

    console.log(userId);
    try {

        const response = await fetch(`http://localhost:5212/api/Clients/adoptionMeetings/${userId}`);
        if (!response.ok) {
            console.error("failed");
        }


        const meetings = await response.json();

        const adoptionContainer = document.getElementById("meetingsContainer");


        adoptionContainer.innerHTML = "";

        meetings.forEach(meeting => {
            const card = makeMeetingCard(meeting);
            adoptionContainer.appendChild(card);
        });

    } catch (error) {
        console.error("Error show meeting:", error);
    }

}



// Open adoption modal
openAdoptionsBtn?.addEventListener("click", () => {
    adoptionModal.style.display = "flex";
    showMeetings();
});


// Close adoption modal
closeAdoptionsBtn?.addEventListener("click", () => {
    adoptionModal.style.display = "none";
});


// Click outside to close
adoptionModal?.addEventListener("click", (e) => {
    if (e.target === adoptionModal) {
        adoptionModal.style.display = "none";
    }
});




// --- surrenders ---
const surrenderModal = document.getElementById("surrenderModal");
const openSurrendersBtn = document.getElementById("statCardSurrenders");
const closeSurrendersBtn = document.getElementById("closeSurrendersModal");
const surrenderModalForm = document.getElementById("surrenderModalForm");
const openSurrenderFormBtn = document.getElementById("openSurrenderFormBtn");
const closeSurrendersFormBtn = document.getElementById("closeSurrendersModalForm");
const surrenderContainer = document.getElementById("surrendersContainer");



// Open surrender modal
openSurrendersBtn?.addEventListener("click", () => {
    surrenderModal.style.display = "flex";
    showSurrenders();
});

// Close surrender modal
closeSurrendersBtn?.addEventListener("click", () => {
    surrenderModal.style.display = "none";
});

// Click outside to close surrender modal
surrenderModal?.addEventListener("click", (e) => {
    if (e.target === surrenderModal) {
        surrenderModal.style.display = "none";
    }
});


// Open surrender modal form
openSurrenderFormBtn?.addEventListener("click", () => {
    surrenderModalForm.style.display = "flex";
    //getting all the values from the entry fields 
    const petName = document.getElementById("surrenderPetName").value = "";
    const petAge = document.getElementById("surrenderPetAge").value = 0;
    const petBreed = document.getElementById("surrenderPetBreed").value = "";
    const meetingDateValue = document.getElementById("surrenderDate").value = "";
    const petImageInput = document.getElementById("surrenderPetImageUrl") = "";
    
});

// Close surrender modal form
closeSurrendersFormBtn?.addEventListener("click", () => {
    surrenderModalForm.style.display = "none";
});

// Click outside to close surrender modal form
surrenderModalForm?.addEventListener("click", (e) => {
    if (e.target === surrenderModalForm) {
        surrenderModalForm.style.display = "none";
    }
});


// Function to fetch and show surrender meetings
async function showSurrenders() {

    const session = await isUserLoggedIn();


    const userId = session.user.userId || 0;

    try {
        const response = await fetch(`http://localhost:5212/api/Clients/surrenderMeetings/${userId}`);
        if (!response.ok) {
            console.error("Failed to fetch surrender meetings");
            return;
        }

        const surrenders = await response.json();
        surrenderContainer.innerHTML = "";

        surrenders.forEach(surrender => {
            const card = makeMeetingCard(surrender);
            surrenderContainer.appendChild(card);
        });


    } catch (error) {
        console.error("Error showing surrender meetings:", error);
    }
}

//adding suurnder meetings
async function addSurrenders() {

    //getting all the values from the entry fields 
    const petName = document.getElementById("surrenderPetName").value.trim();
    const petAge = document.getElementById("surrenderPetAge").value.trim();
    const petBreed = document.getElementById("surrenderPetBreed").value.trim();
    const petType = document.getElementById("surrenderAnimalType").value.trim();
    const meetingDateValue = document.getElementById("surrenderDate").value.trim();
    const petImageInput = document.getElementById("surrenderPetImageUrl");
    const petImage = petImageInput.files[0];


    //getting the message <p>
    const formMessage = document.getElementById("formMessage");

    //check if none of the fields are empty
    if (petName === "" || petAge === "" || petBreed === "" || petType === "" || meetingDateValue === "" || petImage === undefined) {
        formMessage.style.color = "red";
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
        formMessage.style.color = "red";
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
            formMessage.style.color = "red";
            formMessage.innerText = errorData.message;
            return;
        }

        formMessage.style.color = "green";
        formMessage.innerText = res2.message;

        // Clear message after 10 seconds
        setTimeout(() => {
            responseEl.innerText = "";
            showPage("gallery");
        }, 10000);

    } catch (err) {
        console.error(err);
        alert("Something went wrong!");
    }

}

const addSurrenderBtn = document.getElementById("surrenderSubmitBtn");
// Close surrender modal form
addSurrenderBtn?.addEventListener("click", async() => {
    addSurrenders();
});


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
             
            <!--  INFO -->
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

// function for updating all the filter buttons
function syncFilterUI() {

    document.querySelectorAll(".filter-btn").forEach(btn => {
        btn.classList.remove("active");

        const group = btn.dataset.group;

        // "All" buttons
        if (!btn.dataset.value && !btn.dataset.min) {
            if (
                !tempFilters[group] &&
                !(group === "age" && (tempFilters.minAge || tempFilters.maxAge))
            ) {
                btn.classList.add("active");
            }
        }

        // Normal filters
        if (btn.dataset.value && tempFilters[group] === btn.dataset.value) {
            btn.classList.add("active");
        }

        // Age filters
        if (
            group === "age" &&
            btn.dataset.min == tempFilters.minAge &&
            btn.dataset.max == tempFilters.maxAge
        ) {
            btn.classList.add("active");
        }
    });
}

//holding filter and temp filter
let tempFilters = {};
let filters = {};



// Filter modal open/close
const filterModal = document.getElementById("filterModal");
const openFiltersBtn = document.getElementById("openFiltersModal");
const closeFiltersBtn = document.getElementById("closeFiltersModal");

// Open filter modal
openFiltersBtn?.addEventListener("click", () => {
    tempFilters = { ...filters }; // discard changes
    filterModal.style.display = "flex";
});

// Close filter modal
closeFiltersBtn?.addEventListener("click", () => {
    tempFilters = { ...filters }; // discard changes
    syncFilterUI();
    filterModal.style.display = "none";
});

// Click outside to close
filterModal?.addEventListener("click", (e) => {
    if (e.target === filterModal) {
        tempFilters = { ...filters }; // discard changes
        syncFilterUI();
        filterModal.style.display = "none";
    }
});

// Handle filter button clicks
document.querySelectorAll(".filter-btn").forEach(btn => {
    btn.addEventListener("click", function () {
        const group = this.dataset.group;

        // Remove active class from same group
        document.querySelectorAll(`.filter-btn[data-group="${group}"]`)
            .forEach(b => b.classList.remove("active"));

        this.classList.add("active");

        // Remove previous values
        delete tempFilters[group];
        if (group === "age") {
            delete tempFilters.minAge;
            delete tempFilters.maxAge;
        }

        // If "All" clicked → do nothing (keeps filters empty)
        if (!this.dataset.value && !this.dataset.min) return;

        // Normal filters
        if (this.dataset.value) {
            tempFilters[group] = this.dataset.value;
        }

        // Age filters
        if (this.dataset.min) {
            tempFilters.minAge = this.dataset.min;
        }

        if (this.dataset.max) {
            tempFilters.maxAge = this.dataset.max;
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
    console.log(filters);
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

//seraching functionality 

async function searchPets() {
    const input = document.getElementById("searchInput");
    const searchValue = input.value.trim();

    //return if there is nothging in the search bar
    if (searchValue === "") {
        return 
    }
    
    try {
        const params = new URLSearchParams({ query: searchValue });
        const res = await fetch(`http://localhost:5212/api/Pets/search?${params.toString() }`);
        const pets = await res.json();

        showPage("results");
        console.log(pets);
        renderPets(pets, "resultsGallery");
    } catch (error) {
        console.error("Error fetching pets:", error);
    }
}

closeSearchResultBtn = document.getElementById("closeSearchResultBtn");
closeSearchResultBtn?.addEventListener("click", () => {

    const searchValue = document.getElementById("searchInput").value = "";

    showPage("gallery");
    closeDash();
});

searchInput.addEventListener("keydown", (event) => {
    if (event.key === "Enter") {
        searchPets();
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const resetBtn = document.getElementById("resetBtn");
    const showFilterResultBtn = document.getElementById("showFilterResultBtn");

    resetBtn?.addEventListener("click", () => {
        console.log("reset burron clicked");
        // Clear filters object
        for (let key in filters) {
            delete tempFilters[key];
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
        filters = { ...tempFilters };
        loadFilteredPets();
        filterModal.style.display = "none";
    });

    

    loadAllPets();
});



//Login 

const loginForm = document.getElementById("loginSubmitBtn"); // change to your button ID

loginForm?.addEventListener("click", async (e) => {
    e.preventDefault();

    // Get input values and trim spaces/newlines
    const username = document.getElementById("loginUsername").value.trim();
    const password = document.getElementById("loginPassword").value.trim();

    // Element to show messages
    const responseEl = document.getElementById("loginResponse");


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
            document.getElementById("loginUsername").value = "";
            document.getElementById("loginPassword").value = "";
        }

        // Clear message after 10 seconds
        setTimeout(() => {
            responseEl.innerText = "";
            showPage("gallery");
        }, 10000);

    } catch (err) {
        console.error("Login error:", err);
    }
});


// Reusable function to check if user is logged in
async function isUserLoggedIn() {
    try {
        const res = await fetch("http://localhost:5212/api/Clients/session", {
            method: "GET",
            credentials: "include" // important to include session cookie
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

document.getElementById("resultsGallery").addEventListener("click", (e) => {
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