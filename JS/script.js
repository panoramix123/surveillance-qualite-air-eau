const burgerBtn = document.getElementById("burgerBtn");
const sidebar = document.getElementById("sidebar");

if (burgerBtn && sidebar) {
  burgerBtn.addEventListener("click", () => {
    sidebar.classList.toggle("open");
  });
}