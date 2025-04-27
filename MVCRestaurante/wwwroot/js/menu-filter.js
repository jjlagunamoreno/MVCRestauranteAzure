document.addEventListener("DOMContentLoaded", function () {
    var $grid = document.querySelector('.grid');
    var iso = new Isotope($grid, {
        itemSelector: '.all',
        layoutMode: 'fitRows'
    });

    // Filtrar elementos al hacer clic en un botón de categoría
    document.querySelectorAll('.filters_menu li').forEach(function (button) {
        button.addEventListener("click", function () {
            var filterValue = this.getAttribute("data-filter");
            iso.arrange({ filter: filterValue });

            // Remover la clase activa de los otros elementos
            document.querySelector(".filters_menu .active").classList.remove("active");
            this.classList.add("active");
        });
    });
});
