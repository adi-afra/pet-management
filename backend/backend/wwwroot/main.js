
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


function makeMeetingCard(name, datee, timee, animall, breedd) {

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
    title.appendChild(document.createTextNode("Meet " + name));

    headerDiv.appendChild(title);

    // date/time
    const date = document.createElement("p");
    date.className = "mb-1";

    const calendarIcon = document.createElement("i");
    calendarIcon.className = "bi bi-calendar-event me-2";

    date.appendChild(calendarIcon);
    date.appendChild(
        document.createTextNode(`${datee} • ${timee}`)
    );

    // animal
    const animal = document.createElement("p");
    animal.className = "mb-1";

    const pawIcon = document.createElement("i");
    pawIcon.className = "bi bi-paw me-2";

    animal.appendChild(pawIcon);
    animal.appendChild(
        document.createTextNode("Animal: " + animall)
    );

    // breed
    const breed = document.createElement("p");
    breed.className = "mb-2";

    const tagIcon = document.createElement("i");
    tagIcon.className = "bi bi-tag me-2";

    breed.appendChild(tagIcon);
    breed.appendChild(
        document.createTextNode("Breed: " + breedd)
    );

    // delete button
    const deleteButton = document.createElement("button");
    deleteButton.className = "btn btn-danger btn-sm";

    const trashIcon = document.createElement("i");
    trashIcon.className = "bi bi-trash me-1";

    deleteButton.appendChild(trashIcon);
    deleteButton.appendChild(document.createTextNode("Delete"));

    /* 
    // delete functionality
    deleteButton.addEventListener("click", () => {
        console.log("Deleting meeting:", meetingObj.id);

        card.remove();

        // later you could call:
        // deleteMeeting(meetingObj.id)
    });

    */
    

    // assembling the card
    cardBody.appendChild(headerDiv);
    cardBody.appendChild(date);
    cardBody.appendChild(animal);
    cardBody.appendChild(breed);
    cardBody.appendChild(deleteButton);

    card.appendChild(cardBody);

    return card;
}

async function showMeetings() {
    document.getElementById("meetingsContainer").appendChild(makeMeetingCard("jack", "25 jan", "16:00", "dog", "german"));
}


// Adoption modal open/close
const adoptionModal = document.getElementById("adoptionModal");
const openAdoptionsBtn = document.getElementById("openAdoptionsModal");
const closeAdoptionsBtn = document.getElementById("closeAdoptionsModal");


// Open filter modal
openAdoptionsBtn?.addEventListener("click", () => {
    adoptionModal.style.display = "flex";
    showMeetings();
});


// Close filter modal
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



// Show results and close modal
const showResultsBtn = document.querySelector(".filter-actions .show-btn");
showResultsBtn?.addEventListener("click", () => {
  // Navigate to results page
  showPage("results");

  // Close filter modal
  filterModal.style.display = "none";
});




