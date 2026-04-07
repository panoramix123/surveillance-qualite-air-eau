const burgerBtn = document.getElementById("burgerBtn");
const sidebar = document.getElementById("sidebar");
const checkbox = document.getElementById("toggle-json");
const errorCheckbox = document.getElementById("toggle-error");

const STORAGE_KEY_POLLUTION = "modePollutionElevee";
const STORAGE_KEY_ERROR = "modeErreurAPI";

if (burgerBtn && sidebar) {
  burgerBtn.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}

// ===============================
// STOCKAGE LOCAL
// ===============================

function getModePollution() {
  return localStorage.getItem(STORAGE_KEY_POLLUTION) === "true";
}

function setModePollution(value) {
  localStorage.setItem(STORAGE_KEY_POLLUTION, value ? "true" : "false");
}

function getModeErreur() {
  return localStorage.getItem(STORAGE_KEY_ERROR) === "true";
}

function setModeErreur(value) {
  localStorage.setItem(STORAGE_KEY_ERROR, value ? "true" : "false");
}

// Synchronisation des cases à cocher au chargement
if (checkbox) {
  checkbox.checked = getModePollution();
}

if (errorCheckbox) {
  errorCheckbox.checked = getModeErreur();
}

// ===============================
// CHOIX DU FICHIER JSON
// ===============================

function getJsonPath() {
  if (getModeErreur()) {
    return "Data/fichier-inexistant.json";
  }

  if (getModePollution()) {
    return "Data/mesures-IQA-Mauvais.json";
  }

  return "Data/mesures-IQA-bon.json";
}

// ===============================
// CALCULS
// ===============================

function calculerPolluantPrincipal(polluants) {
  const valeurs = [
    { nom: "PM2,5", valeur: Number(polluants.pm25) },
    { nom: "PM10", valeur: Number(polluants.pm10) },
    { nom: "NO2", valeur: Number(polluants.no2) },
    { nom: "O3", valeur: Number(polluants.o3) },
    { nom: "SO2", valeur: Number(polluants.so2) },
    { nom: "CO2", valeur: Number(polluants.co2) }
  ];

  let max = valeurs[0];

  for (let i = 1; i < valeurs.length; i++) {
    if (valeurs[i].valeur > max.valeur) {
      max = valeurs[i];
    }
  }

  return max;
}

function calculerQualiteAir(valeurMax) {
  if (valeurMax < 20) {
    return "Bon";
  }
  if (valeurMax < 50) {
    return "Moyen";
  }
  return "Mauvais";
}

// ===============================
// AFFICHAGE PAGE CARTE
// ===============================

function remplirPageCarte(data) {
  const siteName = document.getElementById("site-name");
  const siteCoords = document.getElementById("site-coords");

  const pm25 = document.getElementById("pm25-value");
  const pm10 = document.getElementById("pm10-value");
  const no2 = document.getElementById("no2-value");
  const o3 = document.getElementById("o3-value");
  const so2 = document.getElementById("so2-value");
  const co2 = document.getElementById("co2-value");

  if (siteName) {
    siteName.textContent = data.site.nom;
  }

  if (siteCoords) {
    siteCoords.textContent = `${data.site.latitude}, ${data.site.longitude}`;
  }

  if (pm25) {
    pm25.textContent = data.polluants.pm25;
  }

  if (pm10) {
    pm10.textContent = data.polluants.pm10;
  }

  if (no2) {
    no2.textContent = data.polluants.no2;
  }

  if (o3) {
    o3.textContent = data.polluants.o3;
  }

  if (so2) {
    so2.textContent = data.polluants.so2;
  }

  if (co2) {
    co2.textContent = data.polluants.co2;
  }
}

// ===============================
// AFFICHAGE DASHBOARD
// ===============================

function remplirDashboard(data) {
  const qualiteAir = document.getElementById("qualite-air");
  const polluantPrincipal = document.getElementById("polluant-principal");
  const valeurPolluant = document.getElementById("valeur-polluant");
  const temperature = document.getElementById("temperature");
  const humidite = document.getElementById("humidite");

  const principal = calculerPolluantPrincipal(data.polluants);
  const qualite = calculerQualiteAir(principal.valeur);

  if (qualiteAir) {
    qualiteAir.textContent = qualite;
    qualiteAir.style.color = "";
  }

  if (polluantPrincipal) {
    polluantPrincipal.textContent = principal.nom;
  }

  if (valeurPolluant) {
    valeurPolluant.textContent = principal.valeur;
  }

  if (temperature && data.dashboard) {
    temperature.textContent = data.dashboard.temperature;
  }

  if (humidite && data.dashboard) {
    humidite.textContent = data.dashboard.humidite;
  }
}

// ===============================
// AFFICHAGE ERREUR
// ===============================

function afficherErreur() {
  const siteName = document.getElementById("site-name");
  const siteCoords = document.getElementById("site-coords");

  const idsCarte = [
    "pm25-value",
    "pm10-value",
    "no2-value",
    "o3-value",
    "so2-value",
    "co2-value"
  ];

  const qualiteAir = document.getElementById("qualite-air");
  const idsDashboard = [
    "polluant-principal",
    "valeur-polluant",
    "temperature",
    "humidite"
  ];

  if (siteName) {
    siteName.textContent = "Erreur de chargement";
  }

  if (siteCoords) {
    siteCoords.textContent = "API ou fichier indisponible";
  }

  idsCarte.forEach((id) => {
    const el = document.getElementById(id);
    if (el) {
      el.textContent = "--";
    }
  });

  if (qualiteAir) {
    qualiteAir.textContent = "Erreur API";
    qualiteAir.style.color = "red";
  }

  idsDashboard.forEach((id) => {
    const el = document.getElementById(id);
    if (el) {
      el.textContent = "--";
    }
  });
}

// ===============================
// CHARGEMENT DES DONNÉES
// ===============================

function chargerDonnees() {
  const jsonPath = getJsonPath();

  fetch(`${jsonPath}?t=${Date.now()}`, { cache: "no-store" })
    .then((response) => {
      if (!response.ok) {
        throw new Error(`Erreur HTTP ${response.status} sur ${jsonPath}`);
      }
      return response.json();
    })
    .then((data) => {
      remplirPageCarte(data);
      remplirDashboard(data);
    })
    .catch((error) => {
      console.error("Erreur de chargement JSON :", error);
      afficherErreur();
    });
}

// ===============================
// ÉVÉNEMENTS
// ===============================

if (checkbox) {
  checkbox.addEventListener("change", () => {
    setModePollution(checkbox.checked);

    if (checkbox.checked && errorCheckbox) {
      errorCheckbox.checked = false;
      setModeErreur(false);
    }

    chargerDonnees();
  });
}

if (errorCheckbox) {
  errorCheckbox.addEventListener("change", () => {
    setModeErreur(errorCheckbox.checked);

    if (errorCheckbox.checked && checkbox) {
      checkbox.checked = false;
      setModePollution(false);
    }

    chargerDonnees();
  });
}

// ===============================
// INITIALISATION
// ===============================

chargerDonnees();