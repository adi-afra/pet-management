document.addEventListener("DOMContentLoaded", () => {
  // ===== Auth DOM
  const authScreen = document.getElementById("authScreen");
  const appShell = document.getElementById("appShell");

  const loginSection = document.getElementById("loginSection");
  const registerSection = document.getElementById("registerSection");

  const goRegister = document.getElementById("goRegister");
  const goLogin = document.getElementById("goLogin");

  const loginForm = document.getElementById("loginForm");
  const registerForm = document.getElementById("registerForm");

  const loginMsg = document.getElementById("loginMsg");
  const registerMsg = document.getElementById("registerMsg");

  // ===== Dashboard logout button
  const logoutBtn = document.getElementById("logoutBtn");

  // ===== Storage keys
  const USERS_KEY = "ph_users";
  const SESSION_KEY = "ph_session";

  const loadUsers = () => {
    try { return JSON.parse(localStorage.getItem(USERS_KEY)) || []; }
    catch { return []; }
  };
  const saveUsers = (users) => localStorage.setItem(USERS_KEY, JSON.stringify(users));

  const setSession = (session) => localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  const getSession = () => {
    try { return JSON.parse(localStorage.getItem(SESSION_KEY)); }
    catch { return null; }
  };
  const clearSession = () => localStorage.removeItem(SESSION_KEY);

  const showApp = () => {
    authScreen.classList.add("d-none");
    appShell.classList.remove("d-none");
  };
  const showAuth = () => {
    appShell.classList.add("d-none");
    authScreen.classList.remove("d-none");
  };

  const showLogin = () => {
    registerSection.classList.remove("ph-active");
    loginSection.classList.add("ph-active");
    if (loginMsg) loginMsg.textContent = "";
    if (registerMsg) registerMsg.textContent = "";
  };

  const showRegister = () => {
    loginSection.classList.remove("ph-active");
    registerSection.classList.add("ph-active");
    if (loginMsg) loginMsg.textContent = "";
    if (registerMsg) registerMsg.textContent = "";
  };

  // ===== On load: if session exists, show dashboard
  const session = getSession();
  if (session?.email) showApp();
  else showAuth();

  // ===== Toggle
  goRegister?.addEventListener("click", showRegister);
  goLogin?.addEventListener("click", showLogin);

  // ===== Register
  registerForm?.addEventListener("submit", (e) => {
    e.preventDefault();
    registerMsg.textContent = "";

    const shelterName = document.getElementById("regShelter").value.trim();
    const email = document.getElementById("regEmail").value.trim().toLowerCase();
    const password = document.getElementById("regPassword").value;

    if (password.length < 6) {
      registerMsg.textContent = "Password must be at least 6 characters.";
      return;
    }

    const users = loadUsers();
    if (users.some(u => u.email === email)) {
      registerMsg.textContent = "This email is already registered. Please sign in.";
      return;
    }

    users.push({ shelterName, email, password }); // demo only
    saveUsers(users);

    setSession({ email, shelterName });
    showApp();
  });

  // ===== Login
  loginForm?.addEventListener("submit", (e) => {
    e.preventDefault();
    loginMsg.textContent = "";

    const email = document.getElementById("loginEmail").value.trim().toLowerCase();
    const password = document.getElementById("loginPassword").value;

    const users = loadUsers();
    const user = users.find(u => u.email === email && u.password === password);

    if (!user) {
      loginMsg.textContent = "Invalid email or password.";
      return;
    }

    setSession({ email: user.email, shelterName: user.shelterName });
    showApp();
  });

  // ===== Logout
  logoutBtn?.addEventListener("click", () => {
    clearSession();
    showAuth();
    showLogin();
  });

  // Forgot password placeholder
  document.getElementById("forgotBtn")?.addEventListener("click", (e) => {
    e.preventDefault();
    alert("Forgot password is a placeholder for now.");
  });

  // ✅ keep your existing menu/page switching logic below (if you already have it)
});
