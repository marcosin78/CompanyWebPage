// dashboard.js

    // Mostrar el canvas al pulsar el botón POST de la sidebar
    document.getElementById('open-post-canvas-btn').onclick = function() {
        document.getElementById('post-canvas-overlay').style.display = 'block';
    };

    // Cerrar el canvas al pulsar la X
    document.getElementById('close-canvas-btn').onclick = function() {
        document.getElementById('post-canvas-overlay').style.display = 'none';
    };

    // Cerrar el canvas al pulsar fuera del cuadro
    document.getElementById('post-canvas-overlay').onclick = function(e) {
        if (e.target === this) {
            this.style.display = 'none';
        }
    };

    // Opcional: Cambiar fuente y tamaño
    let fonts = ['Arial', 'Georgia', 'Courier New'];
    let fontIdx = 0;
    document.getElementById('font-btn').onclick = function() {
        fontIdx = (fontIdx+1)%fonts.length;
        document.getElementById('post-text').style.fontFamily = fonts[fontIdx];
    };
    let sizes = ['16px','20px','24px'];
    let sizeIdx = 0;
    document.getElementById('size-btn').onclick = function() {
        sizeIdx = (sizeIdx+1)%sizes.length;
        document.getElementById('post-text').style.fontSize = sizes[sizeIdx];
    };
