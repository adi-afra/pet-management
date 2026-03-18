const API_BASE = "http://localhost:5235/api";


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
    navbar.classList.add("d-none")
  } else {
    navbar.classList.remove("d-none")
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
loginBtn?.addEventListener("click",() => {
    showPage("login");
    closeDash();
});

//switches from login to register
const signinBtn = document.getElementById("goToRegister");
signinBtn?.addEventListener("click",() => {
    showPage("registration");
    closeDash();
});

//switches from login or register to home
document.querySelectorAll(".backToHome").forEach(btn => {
  btn.addEventListener("click", () =>{
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


function makeMeetingCard(meeting) {

    // main card
    const card = document.createElement("div");
    card.className = "card shadow-sm";

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
    title.appendChild(document.createTextNode("Meet " + meeting.pet.name));

    headerDiv.appendChild(title);

    // date/time
    const meetingDate = new Date(meeting.date);
    const formattedDate = meetingDate.toLocaleDateString();

    const date = document.createElement("p");
    date.className = "mb-1";

    const calendarIcon = document.createElement("i");
    calendarIcon.className = "bi bi-calendar-event me-2";

    date.appendChild(calendarIcon);
    date.appendChild(
        document.createTextNode(formattedDate)
    );

    // animal
    const animal = document.createElement("p");
    animal.className = "mb-1";

    const pawIcon = document.createElement("i");
    pawIcon.className = "bi bi-paw me-2";

    animal.appendChild(pawIcon);
    animal.appendChild(
        document.createTextNode("Animal: " + meeting.pet.animalType)
    );

    // breed
    const breed = document.createElement("p");
    breed.className = "mb-2";

    const tagIcon = document.createElement("i");
    tagIcon.className = "bi bi-tag me-2";

    breed.appendChild(tagIcon);
    breed.appendChild(
        document.createTextNode("Breed: " + meeting.pet.breed)
    );

    // delete button
    const deleteButton = document.createElement("button");
    deleteButton.className = "btn btn-danger btn-sm";

    const trashIcon = document.createElement("i");
    trashIcon.className = "bi bi-trash me-1";

    deleteButton.appendChild(trashIcon);
    deleteButton.appendChild(document.createTextNode("Delete"));

    // DELETE API CALL
    deleteButton.addEventListener("click", async () => {

        try {

            const response = await fetch(
                `${API_BASE}/Clients/adoptionMeetings/${meeting.id}`,
                {
                    method: "DELETE",
                    credentials: "include"
                }
            );

            if (!response.ok) {
                console.error("Delete failed");
                return;
            }

            // remove card from UI
            card.remove();

        } catch (error) {
            console.error("Error deleting meeting:", error);
        }
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
    container.innerHTML = "<p class='text-center'>Loading meetings...</p>"; // UI feedback

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

        if (!response.ok) throw new Error("Failed to fetch meetings");

        const meetings = await response.json();
        container.innerHTML = ""; // Clear loader

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


// Adoption modal open/close
const adoptionModal = document.getElementById("adoptionModal");
const openAdoptionsBtn = document.getElementById("openAdoptionsModal");
const closeAdoptionsBtn = document.getElementById("closeAdoptionsModal");


document.getElementById("statCardMeetings").addEventListener("click", () => {
    document.getElementById("adoptionMeetingsModal").style.display = "flex";
    showMeetings(); // Your function to fetch and display data
});


// Close the modal
document.getElementById("closeMeetingsModal").addEventListener("click", () => {
    document.getElementById("adoptionMeetingsModal").style.display = "none";
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


// ------------------------------
// FRONTEND REGISTER / LOGIN API
// ------------------------------ 

// --- REGISTER ---
const registerBtn = document.getElementById("registerBtn");
registerBtn?.addEventListener("click", async (e) => {
    e.preventDefault();

    const fullName = document.getElementById("regFullName").value;
    const email = document.getElementById("regEmail").value;
    const password = document.getElementById("regPassword").value;
    const confirmPassword = document.getElementById("regConfirmPassword").value;

    if (password !== confirmPassword) {
        alert("Passwords do not match!");
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/clients/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include", // important for sessions
            body: JSON.stringify({ Username: email, Password: password })
        });

        const data = await res.json();

        if (!res.ok) {
            alert("Registration failed: " + (data.message || "Unknown error"));
            return;
        }

        alert("Registration successful!");
        showPage("login");
    } catch (err) {
        console.error(err);
        alert("Something went wrong!");
    }
});

// --- LOGIN ---
const loginSubmitBtn = document.getElementById("loginSubmitBtn");
loginSubmitBtn?.addEventListener("click", async (e) => {
    e.preventDefault();

    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;

    try {
        const res = await fetch(`${API_BASE}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ Username: email, Password: password })
        });

        const data = await res.json();

        if (!res.ok) {
            alert("Login failed: " + (data.message || "Invalid credentials"));
            return;
        }

        alert("Login successful!");
        showPage("gallery"); 
    } catch (err) {
        console.error(err);
        alert("Something went wrong!");
    }
});

// --- LOGOUT API ---
const logoutBtn = document.getElementById("logoutBtn");
logoutBtn?.addEventListener("click", async () => {
    try {
        const res = await fetch(`${API_BASE}/clients/logout`, {
            method: "DELETE",
            credentials: "include"
        });

        if (!res.ok) {
            alert("Logout failed");
            return;
        }

        alert("Logged out successfully");
        showPage("login");
        closeDash();
    } catch (err) {
        console.error(err);
        alert("Something went wrong!");
    }
});


async function checkLogin() {
    try {
        const res = await fetch(`${API_BASE}/auth/status`, {
            credentials: "include"
        });

        if (!res.ok) return;

        const data = await res.json();
        console.log("Logged in as:", data.username);
    } catch (err) {
        console.error(err);
    }
}

// checks is a user is  logged in when page loads
checkLogin();

async function getUserId() {
    const res = await fetch(`${API_BASE}/auth/status`, {
        credentials: "include"
    });

    if (!res.ok) return null;

    const data = await res.json();
    return data.id;
}

