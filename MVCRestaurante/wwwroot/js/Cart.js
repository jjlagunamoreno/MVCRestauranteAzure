document.addEventListener("DOMContentLoaded", function () {
    actualizarContador();

    document.querySelectorAll(".add-to-cart").forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault(); // Evita que el enlace recargue la página

            let platoId = this.dataset.id; // Obtener el ID del plato
            fetch(`/Carrito/Agregar`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: `id=${platoId}`
            })
                .then(response => response.json())
                .then(data => {
                    document.getElementById("pedidoCount").textContent = data.count;
                });
        });
    });
});

function actualizarContador() {
    fetch(`/Carrito/ObtenerCantidad`)
        .then(response => response.json())
        .then(data => {
            document.getElementById("pedidoCount").textContent = data.count;
        });
}
