const DATA_PATH = "Data/historique-IQA.json";

const burgerBtn = document.getElementById("burgerBtn");
const sidebar = document.getElementById("sidebar");
const svg = document.getElementById("pollutantsSvg");
const errorBox = document.getElementById("data-error");
const legendBox = document.getElementById("chart-legend");

if (burgerBtn && sidebar) {
  burgerBtn.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}

function showError() {
  if (errorBox) {
    errorBox.classList.remove("hidden");
  }
}

function hideError() {
  if (errorBox) {
    errorBox.classList.add("hidden");
  }
}

function isInvalidData(data) {
  return !Array.isArray(data) || data.length === 0;
}

const pollutants = [
  { key: "pm25", label: "PM2.5", color: "#0070c0" },
  { key: "pm10", label: "PM10", color: "#ff1e1e" },
  { key: "no2", label: "NO2", color: "#2aaa20" },
  { key: "o3", label: "O3", color: "#f4b400" },
  { key: "so2", label: "SO2", color: "#7d4cc2" }
];

function createSvgElement(tag, attributes) {
  const element = document.createElementNS("http://www.w3.org/2000/svg", tag);

  Object.keys(attributes).forEach(key => {
    element.setAttribute(key, attributes[key]);
  });

  return element;
}

function drawPollutantsChart(data) {
  svg.innerHTML = "";

  const width = 900;
  const height = 430;

  const margin = {
    top: 30,
    right: 30,
    bottom: 55,
    left: 65
  };

  const graphWidth = width - margin.left - margin.right;
  const graphHeight = height - margin.top - margin.bottom;

  const maxValue = Math.max(
    10,
    ...data.flatMap(item =>
      pollutants.map(pollutant => Number(item[pollutant.key]) || 0)
    )
  );

  const roundedMax = Math.ceil(maxValue / 50) * 50;

  function getX(index) {
    if (data.length === 1) {
      return margin.left + graphWidth / 2;
    }

    return margin.left + (index / (data.length - 1)) * graphWidth;
  }

  function getY(value) {
    return margin.top + graphHeight - (value / roundedMax) * graphHeight;
  }

  // Fond
  svg.appendChild(createSvgElement("rect", {
    x: 0,
    y: 0,
    width: width,
    height: height,
    fill: "#ffffff"
  }));

  // Grille + valeurs axe Y
  const ySteps = 5;

  for (let i = 0; i <= ySteps; i++) {
    const value = (roundedMax / ySteps) * i;
    const y = getY(value);

    svg.appendChild(createSvgElement("line", {
      x1: margin.left,
      y1: y,
      x2: width - margin.right,
      y2: y,
      stroke: "#d0d0d0",
      "stroke-width": 1
    }));

    const text = createSvgElement("text", {
      x: margin.left - 12,
      y: y + 5,
      "text-anchor": "end",
      "font-size": 13,
      fill: "#333"
    });

    text.textContent = Math.round(value);
    svg.appendChild(text);
  }

  // Axe X et Y
  svg.appendChild(createSvgElement("line", {
    x1: margin.left,
    y1: margin.top,
    x2: margin.left,
    y2: height - margin.bottom,
    stroke: "#111",
    "stroke-width": 2
  }));

  svg.appendChild(createSvgElement("line", {
    x1: margin.left,
    y1: height - margin.bottom,
    x2: width - margin.right,
    y2: height - margin.bottom,
    stroke: "#111",
    "stroke-width": 2
  }));

  // Labels X
  data.forEach((item, index) => {
    const x = getX(index);

    const text = createSvgElement("text", {
      x: x,
      y: height - 22,
      "text-anchor": "middle",
      "font-size": 14,
      fill: "#333"
    });

    text.textContent = item.date;
    svg.appendChild(text);
  });

  // Titre axe Y
  const yLabel = createSvgElement("text", {
    x: 18,
    y: height / 2,
    "text-anchor": "middle",
    "font-size": 14,
    fill: "#333",
    transform: `rotate(-90 18 ${height / 2})`
  });

  yLabel.textContent = "µg/m³";
  svg.appendChild(yLabel);

  // Courbes
  pollutants.forEach(pollutant => {
    const points = data.map((item, index) => {
      const value = Number(item[pollutant.key]) || 0;
      return `${getX(index)},${getY(value)}`;
    }).join(" ");

    svg.appendChild(createSvgElement("polyline", {
      points: points,
      fill: "none",
      stroke: pollutant.color,
      "stroke-width": 4,
      "stroke-linecap": "round",
      "stroke-linejoin": "round"
    }));

    data.forEach((item, index) => {
      const value = Number(item[pollutant.key]) || 0;

      svg.appendChild(createSvgElement("circle", {
        cx: getX(index),
        cy: getY(value),
        r: 4,
        fill: pollutant.color
      }));
    });
  });

  drawLegend();
}

function drawLegend() {
  legendBox.innerHTML = "";

  pollutants.forEach(pollutant => {
    const item = document.createElement("div");
    item.className = "legend-item";

    const color = document.createElement("span");
    color.className = "legend-color";
    color.style.background = pollutant.color;

    const label = document.createElement("span");
    label.textContent = pollutant.label;

    item.appendChild(color);
    item.appendChild(label);
    legendBox.appendChild(item);
  });
}

function displayCO2(data) {
  let max = data[0];

  for (let i = 1; i < data.length; i++) {
    if (Number(data[i].co2) > Number(max.co2)) {
      max = data[i];
    }
  }

  const current = data[data.length - 1];

  document.getElementById("co2-max-value").textContent = max.co2;
  document.getElementById("co2-max-date").textContent = max.date;
  document.getElementById("co2-max-bar-value").textContent = max.co2;

  document.getElementById("co2-current-value").textContent = current.co2;
  document.getElementById("co2-current-bar-value").textContent = current.co2;

  const ratio = Number(current.co2) / Number(max.co2);
  const heightPercent = Math.max(8, ratio * 100);

  const currentBar = document.getElementById("co2-current-bar");

  if (currentBar) {
    currentBar.style.height = heightPercent + "%";
  }
}

fetch(DATA_PATH, { cache: "no-store" })
  .then(response => {
    if (!response.ok) {
      throw new Error("Fichier historique introuvable");
    }

    return response.json();
  })
  .then(data => {
    if (isInvalidData(data)) {
      showError();
      return;
    }

    hideError();
    drawPollutantsChart(data);
    displayCO2(data);
  })
  .catch(error => {
    console.error(error);
    showError();
  });