document.addEventListener("DOMContentLoaded", function () {
    actualizarContador();

    document.querySelectorAll(".add-to-cart").forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault();

            let platoId = this.dataset.id;
            fetch(`/Carrito/Agregar?id=${platoId}`, {
                method: "GET"
            })
                .then(response => response.json())
                .then(data => {
                    document.getElementById("pedidoCount").textContent = data.count;
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
});
