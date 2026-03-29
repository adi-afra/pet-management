
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
const registrationBtn = document.getElementById("registrationPage");
registrationBtn?.addEventListener("click", () => {
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
            `http://localhost:5212/api/Clients/adoptionMeetings/${id}`,
            {
                method: "DELETE"
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
            `http://localhost:5212/api/Clients/surrenderMeetings/${id}`,
            { method: "DELETE" }
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
    if (meeting.type == 0) {
        title.appendChild(document.createTextNode("Meet " + meeting.pet.name));
    } else {
        title.appendChild(document.createTextNode("surrender " + meeting.pet.name));
    }
    

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
            deleteSurrendeMeeting(meeting.id);
            // remove card from UI
            card.remove();
        });
    }
    


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

    const userId = 3; // whatever your logged-in user id is

    try {

        const response = await fetch(`http://localhost:5212/api/Clients/adoptionMeetings/${userId}`);
        if (!response.ok) {
            console.error("failed");
        }


        const meetings = await response.json();

        const container = document.getElementById("meetingsContainer");


        container.innerHTML = "";

        meetings.forEach(meeting => {
            const card = makeMeetingCard(meeting);
            container.appendChild(card);
        });

    } catch (error) {
        console.error("Error show meeting:", error);
    }

}


// Adoption modal open/close
const adoptionModal = document.getElementById("adoptionModal");
const openAdoptionsBtn = document.getElementById("openAdoptionsModal");
const closeAdoptionsBtn = document.getElementById("closeAdoptionsModal");





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


const registerForm = document.getElementById("submit");
registerForm?.addEventListener("click" ,async (e) => {
  e.preventDefault();

  const username = document.getElementById("username").value;
  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;

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
        catch (e) {}

        // Show the error message to the user
        responseEl.style.color = "red";
        responseEl.innerText = message;
        

        // Clear the message automatically after 30 seconds
        
        setTimeout(() => {
           responseEl.innerText = "";
        }, 10000);
        
    }
    
    else{
        //default message
        let message = "registration successful";
        
        try{
            message = errorData.message || message;
        }
        catch (e) {}
        
        responseEl.style.color = "Green";
        responseEl.innerText = message;
        
        // Clear the message automatically after 30 seconds
        setTimeout(() => {
           responseEl.innerText = "";
        }, 10000);
        
    }
    
   
  } catch (err) {
    console.log(err);
  }
});

// ----- SURRENDER MEETINGS MODAL -----

// Grab the modal and buttons
const surrenderModal = document.getElementById("surrenderModal");
const openSurrendersBtn = document.getElementById("openSurrendersModal");
const closeSurrendersBtn = document.getElementById("closeSurrendersModal");
const surrenderContainer = document.getElementById("surrendersContainer"); // container inside modal
const addSurrenderMeetingButton = document.getElementById("addSurrenderMeeting");


// Function to fetch and show surrender meetings
async function showSurrenders() {
    
    const session = await isUserLoggedIn();

    
    const userId = session.userId || 0;

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
        addSurrenderMeetingButton.classList.remove("d-none");


    } catch (error) {
        console.error("Error showing surrender meetings:", error);
    }
}

async function addSurrenders() {
    
    
    
    //getting all the values from the entry fields 
    const petName = document.getElementById("petName").value.trim();
    const petAge = document.getElementById("petAge").value.trim();
    const petBreed = document.getElementById("petBreed").value.trim();
    const petType = document.getElementById("petType").value.trim();
    const meetingDateValue = document.getElementById("meetingDate").value.trim();
    
    
    //getting the message <p>
    const formMessage = document.getElementById("formMessage");

    if (petName === "" || petAge === "" || petBreed === "" || petType === "" || meetingDateValue === "") {
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

    const userId = 1; // replace with logged-in user id
    try {
        const res = await fetch("http://localhost:5212/api/Clients/surrenderMeetings", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                "name": petName,
                "age": Number(petAge),
                "breed": petBreed,
                "date": meetingDate,
                "userId": userId,
                "animalType": petType
            })
        });


        if (!res.ok) {
            const errorData = await res.json();
            console.log("adding meeting failed: " + errorData.message);
            return;
        }

        showSurrenders();
        
    } catch (err) {
        console.error(err);
        alert("Something went wrong!");
    }

}

function createMeetingForm() {

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

    
    //  Submit logic
    submitBtn.addEventListener("click", async () => {
        console.log("clicked");
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

// Open form
addSurrenderMeetingButton?.addEventListener("click", () => {
    surrenderContainer.innerHTML = "";
    createMeetingForm();
    addSurrenderMeetingButton.classList.add("d-none");

});

const PETS_API_BASE = "/api/Pets";

function renderPets(pets, containerId) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error("Missing container:", containerId);
        return;
    }

    if (!pets || pets.length === 0) {
        container.innerHTML = `<div class="col-12"><p class="text-center">No pets found.</p></div>`;
        return;
    }

    container.innerHTML = pets.map(pet => `
        <div class="col-12 col-md-6 col-lg-4">
            <div class="pet-card h-100">
                <img class="pet-img"
                     src="${pet.imageUrl}"
                     alt="${pet.name}">
                <div class="info">
                    <h3>${pet.name}</h3>
                    <p>${pet.age} years • ${pet.type}</p>
                </div>
            </div>
        </div>
    `).join("");
}

async function loadAllPets() {
    const res = await fetch(`http://localhost:5212/api/Pets/pet`);
    const pets = await res.json();
    renderPets(pets, "petGallery");
}

async function loadDogs() {
    const res = await fetch(`${PETS_API_BASE}/filter?animalType=Dog`);
    const pets = await res.json();
    renderPets(pets, "petGallery");
}

async function loadCats() {
    const res = await fetch(`${PETS_API_BASE}/filter?animalType=Cat`);
    const pets = await res.json();
    renderPets(pets, "petGallery");
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
    const galleryAllBtn = document.getElementById("galleryAllBtn");
    const galleryDogsBtn = document.getElementById("galleryDogsBtn");
    const galleryCatsBtn = document.getElementById("galleryCatsBtn");
    const searchInput = document.getElementById("searchInput");

    console.log("galleryAllBtn:", galleryAllBtn);
    console.log("galleryDogsBtn:", galleryDogsBtn);
    console.log("galleryCatsBtn:", galleryCatsBtn);
    console.log("searchInput:", searchInput);

    galleryAllBtn?.addEventListener("click", () => {
        console.log("All Pets clicked");
        galleryAllBtn.classList.add("active");
        galleryDogsBtn.classList.remove("active");
        galleryCatsBtn.classList.remove("active");
        loadAllPets();
    });

    galleryDogsBtn?.addEventListener("click", () => {
        console.log("Dogs clicked");
        galleryAllBtn.classList.remove("active");
        galleryDogsBtn.classList.add("active");
        galleryCatsBtn.classList.remove("active");
        loadDogs();
    });

    galleryCatsBtn?.addEventListener("click", () => {
        console.log("Cats clicked");
        galleryAllBtn.classList.remove("active");
        galleryDogsBtn.classList.remove("active");
        galleryCatsBtn.classList.add("active");
        loadCats();
    });

    searchInput?.addEventListener("keydown", (e) => {
        if (e.key === "Enter") {
            console.log("Search Enter pressed");
            searchPets();
        }
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
            </div>
        `;

        container.appendChild(card);
    });
}
*/

