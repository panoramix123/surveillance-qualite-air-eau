const menuContainer = document.getElementById("menu-container");

if (menuContainer) {
  menuContainer.innerHTML = `
    <aside class="sidebar" id="sidebar">
      <div class="sidebar-inner">
        <div class="sidebar-title">Menu</div>

        <nav class="menu">
          <a href="index.html" class="menu-item" data-page="dashboard">
            Tableau de bord
          </a>

          <div class="menu-item has-submenu">
            <button type="button" class="menu-btn">
              Carte
              <span class="arrow">▶</span>
            </button>

            <div class="submenu">
              <div class="submenu-item">
                <a href="map_ST_G.html" class="submenu-link" data-page="map-st-gemmes">
                  ST Gemmes-sur-Loire
                </a>
              </div>

              <div class="submenu-item">
                <button type="button" class="submenu-link">
                  Les Ponts-de-Cé
                </button>
              </div>

              <div class="submenu-item">
                <button type="button" class="submenu-link">
                  Loire-Authion
                </button>
              </div>
            </div>
          </div>

          <a href="graphs.html" class="menu-item" data-page="graphs">
            Graphiques
          </a>

          <a href="infos.html" class="menu-item" data-page="infos">
            Informations complémentaires
          </a>

          <a href="#" class="menu-item" data-page="settings">
            Paramètres
          </a>

          <a href="#" class="menu-item" data-page="future">
            À venir
          </a>
        </nav>
      </div>
    </aside>
  `;
}

function setActiveMenuLink() {
  const currentPage = window.location.pathname.split("/").pop() || "index.html";

  const links = document.querySelectorAll(".menu-item, .submenu-link");

  links.forEach((link) => {
    link.classList.remove("active-link");
    link.classList.remove("active-map-link");
  });

  if (currentPage === "index.html" || currentPage === "") {
    const dashboardLink = document.querySelector('[data-page="dashboard"]');

    if (dashboardLink) {
      dashboardLink.classList.add("active-link");
    }
  }

  if (currentPage === "map_ST_G.html") {
    const mapLink = document.querySelector('[data-page="map-st-gemmes"]');

    if (mapLink) {
      mapLink.classList.add("active-map-link");
    }
  }

  if (currentPage === "graphs.html") {
    const graphsLink = document.querySelector('[data-page="graphs"]');

    if (graphsLink) {
      graphsLink.classList.add("active-link");
    }
  }

  if (currentPage === "infos.html") {
    const infosLink = document.querySelector('[data-page="infos"]');

    if (infosLink) {
      infosLink.classList.add("active-link");
    }
  }
}

setActiveMenuLink();