const burgerBtn = document.getElementById("burgerBtn");
const sidebar = document.getElementById("sidebar");
const scenarioSelect = document.getElementById("scenario-select");
const errorCheckbox = document.getElementById("toggle-error");

const STORAGE_KEY_SCENARIO = "scenarioQualiteAir";
const STORAGE_KEY_ERROR = "modeErreurAPI";

let airMap = null;
let airMarker = null;

if (burgerBtn && sidebar) {
  burgerBtn.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}

// ===============================
// STOCKAGE LOCAL
// ===============================

function getScenarioQualiteAir() {
  return localStorage.getItem(STORAGE_KEY_SCENARIO) || "normal";
}

function setScenarioQualiteAir(value) {
  localStorage.setItem(STORAGE_KEY_SCENARIO, value);
}

function getModeErreur() {
  return localStorage.getItem(STORAGE_KEY_ERROR) === "true";
}

function setModeErreur(value) {
  localStorage.setItem(STORAGE_KEY_ERROR, value ? "true" : "false");
}

if (scenarioSelect) {
  scenarioSelect.value = getScenarioQualiteAir();
  scenarioSelect.disabled = getModeErreur();
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

  const scenario = getScenarioQualiteAir();

  if (scenario === "bon") {
    return "Data/mesures-IQA-Bon.json";
  }

  if (scenario === "moyen") {
    return "Data/mesures-IQA-Moyen.json";
  }

  if (scenario === "degrade") {
    return "Data/mesures-IQA-Degrade.json";
  }

  if (scenario === "mauvais") {
    return "Data/mesures-IQA-Mauvais.json";
  }

  if (scenario === "tres-mauvais") {
    return "Data/mesures-IQA-Tres-Mauvais.json";
  }

  if (scenario === "extremement-mauvais") {
    return "Data/mesures-IQA-Extremement-Mauvais.json";
  }

  return "Data/donnees_API_DataAirPL.json";
}

// ===============================
// SVG QUALITÉ AIR
// ===============================

function getAirQualitySvg(qualite) {
  const configs = {
    "Bon": {
      color1: "#9be15d",
      color2: "#5ea832",
      eyes: `
        <circle cx="55" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
        <circle cx="105" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
      `,
      mouth: `
        <path d="M45 92 Q80 125 115 92" fill="none" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `
    },

    "Moyen": {
      color1: "#ffd86b",
      color2: "#e6a700",
      eyes: `
        <circle cx="55" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
        <circle cx="105" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
      `,
      mouth: `
        <path d="M48 103 Q80 110 112 103" fill="none" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `
    },

    "Dégradé": {
      color1: "#ffb56b",
      color2: "#f07a24",
      eyes: `
        <circle cx="55" cy="62" r="9" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
        <circle cx="105" cy="62" r="9" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
      `,
      mouth: `
        <path d="M48 106 Q80 98 112 106" fill="none" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `
    },

    "Mauvais": {
      color1: "#ff7b7b",
      color2: "#d62828",
      eyes: `
        <circle cx="55" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
        <circle cx="105" cy="62" r="10" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
      `,
      mouth: `
        <path d="M48 115 Q80 88 112 115" fill="none" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `
    },

    "Très mauvais": {
      color1: "#d685ff",
      color2: "#8e24aa",
      eyes: `
        <path d="M45 55 L65 70 M65 55 L45 70" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
        <path d="M95 55 L115 70 M115 55 L95 70" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `,
      mouth: `
        <path d="M48 118 Q80 84 112 118" fill="none" stroke="#1f1f1f" stroke-width="6" stroke-linecap="round"/>
      `
    },

    "Extrêmement mauvais": {
      color1: "#8f8f8f",
      color2: "#2f2f2f",
      eyes: `
        <path d="M43 52 L67 74 M67 52 L43 74" stroke="#1f1f1f" stroke-width="6" stroke-linecap="round"/>
        <path d="M93 52 L117 74 M117 52 L93 74" stroke="#1f1f1f" stroke-width="6" stroke-linecap="round"/>
      `,
      mouth: `
        <path d="M48 120 Q80 78 112 120" fill="none" stroke="#1f1f1f" stroke-width="7" stroke-linecap="round"/>
      `
    },

    "Erreur API": {
      color1: "#ff6b6b",
      color2: "#7a0000",
      eyes: `
        <path d="M43 52 L67 74 M67 52 L43 74" stroke="#1f1f1f" stroke-width="6" stroke-linecap="round"/>
        <path d="M93 52 L117 74 M117 52 L93 74" stroke="#1f1f1f" stroke-width="6" stroke-linecap="round"/>
      `,
      mouth: `
        <path d="M50 118 Q80 86 110 118" fill="none" stroke="#1f1f1f" stroke-width="7" stroke-linecap="round"/>
      `
    },

    "Indisponible": {
      color1: "#cfcfcf",
      color2: "#777777",
      eyes: `
        <circle cx="55" cy="62" r="8" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
        <circle cx="105" cy="62" r="8" fill="#5f5f5f" stroke="#1f1f1f" stroke-width="4"/>
      `,
      mouth: `
        <path d="M52 105 L108 105" fill="none" stroke="#1f1f1f" stroke-width="5" stroke-linecap="round"/>
      `
    }
  };

  const config = configs[qualite] || configs["Indisponible"];

  return `
    <svg class="air-smiley-svg" viewBox="0 0 160 160" xmlns="http://www.w3.org/2000/svg">
      <defs>
        <radialGradient id="smileyGradient" cx="35%" cy="30%" r="70%">
          <stop offset="0%" stop-color="${config.color1}"/>
          <stop offset="100%" stop-color="${config.color2}"/>
        </radialGradient>
      </defs>

      <circle
        cx="80"
        cy="80"
        r="70"
        fill="url(#smileyGradient)"
        stroke="#1f1f1f"
        stroke-width="5"
      />

      ${config.eyes}
      ${config.mouth}
    </svg>
  `;
}

function updateAirQualitySvg(qualite) {
  const airIcon = document.getElementById("air-icon");

  if (!airIcon) {
    return;
  }

  airIcon.innerHTML = getAirQualitySvg(qualite);
}

// ===============================
// ADAPTATION DES DONNÉES
// ===============================

function trouverMesureHeureActuelle(polluants) {
  if (!Array.isArray(polluants) || polluants.length === 0) {
    return null;
  }

  const maintenant = new Date();
  const heureActuelle = maintenant.getHours();

  const mesure = polluants.find((ligne) => {
    if (!ligne.date_heure) {
      return false;
    }

    const dateLigne = new Date(ligne.date_heure.replace(" ", "T"));
    return dateLigne.getHours() === heureActuelle;
  });

  return mesure || polluants[polluants.length - 1];
}

function adapterDonneesOpenMeteo(data) {
  const mesureActuelle = trouverMesureHeureActuelle(data.polluants);

  if (!mesureActuelle) {
    throw new Error("Aucune mesure disponible dans le fichier JSON");
  }

  return {
    site: {
      nom: data.zone || "Saint-Gemmes-sur-Loire / Angers",
      latitude: data.latitude || "--",
      longitude: data.longitude || "--"
    },
    polluants: {
      pm25: mesureActuelle.pm25,
      pm10: mesureActuelle.pm10,
      no2: mesureActuelle.no2,
      o3: mesureActuelle.o3,
      so2: mesureActuelle.so2,
      co2: mesureActuelle.co2 || "--"
    },
    dashboard: {
      temperature: data.temperature || "--",
      humidite: data.humidite || "--"
    },
    meta: {
      source: data.source || "Fichier local",
      last_update: data.last_update || "--",
      date_heure: mesureActuelle.date_heure || "--"
    }
  };
}

// ===============================
// CALCULS INDICE ATMO
// ===============================

const NIVEAUX_QUALITE = [
  "Bon",
  "Moyen",
  "Dégradé",
  "Mauvais",
  "Très mauvais",
  "Extrêmement mauvais"
];

const SEUILS_ATMO = {
  pm10: {
    nom: "PM10",
    seuils: [20, 40, 50, 100, 150]
  },
  pm25: {
    nom: "PM2,5",
    seuils: [10, 20, 25, 50, 75]
  },
  no2: {
    nom: "NO2",
    seuils: [40, 90, 120, 230, 340]
  },
  o3: {
    nom: "O3",
    seuils: [50, 100, 130, 240, 380]
  },
  so2: {
    nom: "SO2",
    seuils: [100, 200, 350, 500, 750]
  }
};

function calculerNiveauPolluant(cle, valeur) {
  const config = SEUILS_ATMO[cle];

  if (!config || valeur === null || valeur === undefined || isNaN(Number(valeur))) {
    return null;
  }

  const v = Number(valeur);
  const seuils = config.seuils;

  if (v <= seuils[0]) return 0;
  if (v <= seuils[1]) return 1;
  if (v <= seuils[2]) return 2;
  if (v <= seuils[3]) return 3;
  if (v <= seuils[4]) return 4;

  return 5;
}

function calculerPolluantPrincipal(polluants) {
  const valeurs = Object.keys(SEUILS_ATMO)
    .map((cle) => {
      const valeur = Number(polluants[cle]);
      const niveau = calculerNiveauPolluant(cle, valeur);

      return {
        cle,
        nom: SEUILS_ATMO[cle].nom,
        valeur,
        niveau,
        qualite: niveau !== null ? NIVEAUX_QUALITE[niveau] : "Indisponible"
      };
    })
    .filter((item) => item.niveau !== null);

  if (valeurs.length === 0) {
    return {
      nom: "--",
      valeur: "--",
      niveau: null,
      qualite: "Indisponible"
    };
  }

  let pire = valeurs[0];

  for (let i = 1; i < valeurs.length; i++) {
    if (valeurs[i].niveau > pire.niveau) {
      pire = valeurs[i];
    }
  }

  return pire;
}

function calculerQualiteAir(polluants) {
  const principal = calculerPolluantPrincipal(polluants);

  if (principal.niveau === null) {
    return "Indisponible";
  }

  return principal.qualite;
}

function appliquerCouleurQualite(element, qualite) {
  if (!element) return;

  element.style.color = "";

  element.classList.remove(
    "qualite-bon",
    "qualite-moyen",
    "qualite-degrade",
    "qualite-mauvais",
    "qualite-tres-mauvais",
    "qualite-extremement-mauvais"
  );

  if (qualite === "Bon") {
    element.classList.add("qualite-bon");
  } else if (qualite === "Moyen") {
    element.classList.add("qualite-moyen");
  } else if (qualite === "Dégradé") {
    element.classList.add("qualite-degrade");
  } else if (qualite === "Mauvais") {
    element.classList.add("qualite-mauvais");
  } else if (qualite === "Très mauvais") {
    element.classList.add("qualite-tres-mauvais");
  } else if (qualite === "Extrêmement mauvais") {
    element.classList.add("qualite-extremement-mauvais");
  }

  updateAirQualitySvg(qualite);
}

// ===============================
// LOCALISATIONS ST GEMMES-SUR-LOIRE
// ===============================

const LOCATIONS_ST_GEMMES = {
  "rue-moulin-pain": {
    nom: "Rue du Moulin du Pain",
    latitude: 47.423785,
    longitude: -0.558774
  },
  "rue-mairie": {
    nom: "Rue de la Mairie",
    latitude: 47.4249,
    longitude: -0.5582
  },
  "route-bouchemaine": {
    nom: "Route de Bouchemaine",
    latitude: 47.4264,
    longitude: -0.5631
  },
  "rue-grands-jardins": {
    nom: "Rue des Grands Jardins",
    latitude: 47.4218,
    longitude: -0.5556
  },
  "chemin-hutreau": {
    nom: "Chemin du Hutreau",
    latitude: 47.4281,
    longitude: -0.5518
  }
};

function getSelectedLocation() {
  const locationSelect = document.getElementById("location-select");

  if (!locationSelect) {
    return null;
  }

  return LOCATIONS_ST_GEMMES[locationSelect.value] || LOCATIONS_ST_GEMMES["rue-moulin-pain"];
}

function updateLocationDisplay(location) {
  const siteCoords = document.getElementById("site-coords");

  if (siteCoords && location) {
    siteCoords.textContent = `${location.latitude}, ${location.longitude}`;
  }
}

function initLocationSelector(data) {
  const locationSelect = document.getElementById("location-select");

  if (!locationSelect) {
    initOrUpdateMap(data);
    return;
  }

  const selectedLocation = getSelectedLocation();

  if (selectedLocation) {
    const updatedData = {
      ...data,
      site: {
        ...data.site,
        nom: selectedLocation.nom,
        latitude: selectedLocation.latitude,
        longitude: selectedLocation.longitude
      }
    };

    updateLocationDisplay(selectedLocation);
    initOrUpdateMap(updatedData);
  }

  if (!locationSelect.dataset.initialized) {
    locationSelect.dataset.initialized = "true";

    locationSelect.addEventListener("change", () => {
      const location = getSelectedLocation();

      if (!location) {
        return;
      }

      const updatedData = {
        ...data,
        site: {
          ...data.site,
          nom: location.nom,
          latitude: location.latitude,
          longitude: location.longitude
        }
      };

      updateLocationDisplay(location);
      initOrUpdateMap(updatedData);
    });
  }
}

// ===============================
// CARTE INTERACTIVE
// ===============================

function initOrUpdateMap(data) {
  const mapContainer = document.getElementById("real-map");

  if (!mapContainer || typeof L === "undefined") {
    return;
  }

  const latitude = Number(data.site.latitude) || 47.423785;
  const longitude = Number(data.site.longitude) || -0.558774;
  const nomSite = data.site.nom || "ST Gemmes-sur-Loire";

  if (!airMap) {
    airMap = L.map("real-map").setView([latitude, longitude], 15);

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 19,
      attribution: "© OpenStreetMap"
    }).addTo(airMap);

    airMarker = L.marker([latitude, longitude]).addTo(airMap);
  } else {
    airMap.setView([latitude, longitude], 15);

    if (airMarker) {
      airMarker.setLatLng([latitude, longitude]);
    }
  }

  if (airMarker) {
    airMarker.bindPopup(`
      <strong>${nomSite}</strong><br>
      Latitude : ${latitude}<br>
      Longitude : ${longitude}
    `);
  }

  setTimeout(() => {
    airMap.invalidateSize();
  }, 100);
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

  const selectedLocation = getSelectedLocation();

  if (siteName) {
    if (selectedLocation) {
      siteName.textContent = selectedLocation.nom;
    } else {
      siteName.textContent = data.site.nom;
    }
  }

  if (siteCoords && !selectedLocation) {
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

  const heureMesure = document.getElementById("heure-mesure");

  if (heureMesure && data.meta) {
    heureMesure.textContent = data.meta.date_heure;
  }

  initLocationSelector(data);
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
  const qualite = calculerQualiteAir(data.polluants);

  if (qualiteAir) {
    qualiteAir.textContent = qualite;
    qualiteAir.style.color = "";
    appliquerCouleurQualite(qualiteAir, qualite);
  }

  if (polluantPrincipal) {
    polluantPrincipal.textContent = principal.nom;
  }

  if (valeurPolluant) {
    valeurPolluant.textContent = principal.valeur;
  }

  if (temperature) {
    temperature.textContent = data.dashboard.temperature;
  }

  if (humidite) {
    humidite.textContent = data.dashboard.humidite;
  }

  const derniereMaj = document.getElementById("derniere-maj");
  const heureMesure = document.getElementById("heure-mesure");

  if (derniereMaj && data.meta) {
    derniereMaj.textContent = data.meta.last_update;
  }

  if (heureMesure && data.meta) {
    heureMesure.textContent = data.meta.date_heure;
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
    "humidite",
    "derniere-maj",
    "heure-mesure"
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
    qualiteAir.classList.remove(
      "qualite-bon",
      "qualite-moyen",
      "qualite-degrade",
      "qualite-mauvais",
      "qualite-tres-mauvais",
      "qualite-extremement-mauvais"
    );

    qualiteAir.textContent = "Erreur API";
    qualiteAir.style.color = "red";
    updateAirQualitySvg("Erreur API");
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
      const donneesAdaptees = adapterDonneesOpenMeteo(data);

      remplirPageCarte(donneesAdaptees);
      remplirDashboard(donneesAdaptees);

      console.log("Données chargées :", donneesAdaptees);
    })
    .catch((error) => {
      console.error("Erreur de chargement JSON :", error);
      afficherErreur();
    });
}

// ===============================
// ÉVÉNEMENTS
// ===============================

if (scenarioSelect) {
  scenarioSelect.addEventListener("change", () => {
    setScenarioQualiteAir(scenarioSelect.value);

    if (errorCheckbox) {
      errorCheckbox.checked = false;
      setModeErreur(false);
    }

    scenarioSelect.disabled = false;
    chargerDonnees();
  });
}

if (errorCheckbox) {
  errorCheckbox.addEventListener("change", () => {
    setModeErreur(errorCheckbox.checked);

    if (scenarioSelect) {
      scenarioSelect.disabled = errorCheckbox.checked;
    }

    chargerDonnees();
  });
}

// ===============================
// INITIALISATION
// ===============================

updateAirQualitySvg("Moyen");
chargerDonnees();