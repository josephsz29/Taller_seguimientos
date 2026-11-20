// Lógica del navegador: solo dibuja y pregunta al servidor.
// El servidor guarda cuándo arrancó el cronómetro y cuánto lleva acumulado;
// el navegador va sumando el tiempo localmente para que la pantalla se vea fluida.
(function () {
    const root = document.getElementById('chronometer');
    if (!root) {
        return;
    }

    const id = root.dataset.id;
    const urls = {
        start: root.dataset.startUrl,
        pause: root.dataset.pauseUrl,
        reset: root.dataset.resetUrl,
        lap: root.dataset.lapUrl,
        state: root.dataset.stateUrl
    };

    const syncIntervalMs = 10000;

    const display = document.getElementById('display');
    const primaryButton = document.getElementById('primaryButton');
    const secondaryButton = document.getElementById('secondaryButton');
    const messageLabel = document.getElementById('message');
    const lapsSection = document.getElementById('lapsSection');
    const lapsBody = document.getElementById('lapsBody');

    let state = JSON.parse(root.dataset.state);
    let baseElapsed = state.elapsedMilliseconds;   // tiempo que informó el servidor
    let baseTime = performance.now();              // momento (en este navegador) al que corresponde ese tiempo
    let busy = false;                              // evita enviar dos órdenes a la vez
    let frameId = null;

    // Mismo formato que TimeFormatter.Format en C#: "05:23.47" o "1:05:23.47"
    function formatTime(totalMilliseconds) {
        const totalCentiseconds = Math.floor(totalMilliseconds / 10);
        const centiseconds = totalCentiseconds % 100;
        const totalSeconds = Math.floor(totalCentiseconds / 100);
        const seconds = totalSeconds % 60;
        const totalMinutes = Math.floor(totalSeconds / 60);
        const minutes = totalMinutes % 60;
        const hours = Math.floor(totalMinutes / 60);

        const pad = function (value) { return String(value).padStart(2, '0'); };
        const text = pad(minutes) + ':' + pad(seconds) + '.' + pad(centiseconds);

        return hours > 0 ? hours + ':' + text : text;
    }

    function getElapsed() {
        if (!state.isRunning) {
            return baseElapsed;
        }

        return baseElapsed + (performance.now() - baseTime);
    }

    // Dibuja el tiempo; mientras corre se repite en cada cuadro de la pantalla
    function renderDisplay() {
        display.textContent = formatTime(getElapsed());

        if (state.isRunning) {
            frameId = requestAnimationFrame(renderDisplay);
        } else {
            frameId = null;
        }
    }

    function ensureLoop() {
        if (frameId !== null) {
            cancelAnimationFrame(frameId);
            frameId = null;
        }

        renderDisplay();
    }

    function showMessage(text) {
        messageLabel.textContent = text || '';
    }

    // Botones según el estado: detenido, corriendo o en pausa
    function updateButtons() {
        const hasTime = state.elapsedMilliseconds > 0;

        display.classList.toggle('running', state.isRunning);

        if (state.isRunning) {
            primaryButton.textContent = 'Pausar';
            primaryButton.className = 'btn btn-danger btn-lg chronometer-button';
            secondaryButton.textContent = 'Vuelta';
            secondaryButton.disabled = false;
        } else {
            primaryButton.textContent = hasTime ? 'Reanudar' : 'Iniciar';
            primaryButton.className = 'btn btn-success btn-lg chronometer-button';
            secondaryButton.textContent = 'Reiniciar';
            secondaryButton.disabled = !hasTime && state.laps.length === 0;
        }
    }

    // Tabla de vueltas: la más reciente arriba; verde la más rápida y rojo la más lenta
    function renderLaps() {
        lapsBody.replaceChildren();

        const laps = state.laps.slice().reverse();

        laps.forEach(function (lap) {
            const row = document.createElement('tr');

            if (lap.number === state.bestLapNumber) {
                row.className = 'table-success';
            } else if (lap.number === state.worstLapNumber) {
                row.className = 'table-danger';
            }

            const numberCell = document.createElement('td');
            numberCell.textContent = lap.number;

            const lapCell = document.createElement('td');
            lapCell.className = 'text-end lap-time';
            lapCell.textContent = formatTime(lap.lapMilliseconds);

            const totalCell = document.createElement('td');
            totalCell.className = 'text-end lap-time';
            totalCell.textContent = formatTime(lap.totalMilliseconds);

            row.appendChild(numberCell);
            row.appendChild(lapCell);
            row.appendChild(totalCell);
            lapsBody.appendChild(row);
        });

        lapsSection.classList.toggle('d-none', laps.length === 0);
    }

    // Guarda el estado que llegó del servidor y actualiza toda la pantalla.
    // El servidor calculó el tiempo "a mitad de camino" entre el envío y la llegada de la respuesta,
    // así que usamos ese punto medio como referencia (compensa la demora de la red).
    function applyState(newState, sentAt, receivedAt) {
        state = newState;
        baseElapsed = newState.elapsedMilliseconds;
        baseTime = (sentAt + receivedAt) / 2;

        renderLaps();
        updateButtons();
        ensureLoop();
    }

    async function send(url) {
        if (busy) {
            return;
        }

        busy = true;
        showMessage('');

        try {
            const token = document.querySelector('#csrfForm input[name="__RequestVerificationToken"]').value;
            const sentAt = performance.now();

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: 'id=' + encodeURIComponent(id)
            });

            if (!response.ok) {
                throw new Error('Respuesta no válida');
            }

            const newState = await response.json();
            applyState(newState, sentAt, performance.now());
        } catch (error) {
            showMessage('No se pudo conectar con el servidor. Intenta de nuevo.');
        } finally {
            busy = false;
        }
    }

    // Sincronización periódica: si el mismo cronómetro se maneja desde otra pestaña o dispositivo, aquí se refleja
    async function syncWithServer() {
        if (busy) {
            return;
        }

        try {
            const sentAt = performance.now();
            const response = await fetch(urls.state + '?id=' + encodeURIComponent(id), {
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok || busy) {
                return;
            }

            const newState = await response.json();
            applyState(newState, sentAt, performance.now());
        } catch (error) {
            // Si falla, se intenta de nuevo en el siguiente ciclo
        }
    }

    primaryButton.addEventListener('click', function () {
        send(state.isRunning ? urls.pause : urls.start);
    });

    secondaryButton.addEventListener('click', function () {
        send(state.isRunning ? urls.lap : urls.reset);
    });

    renderLaps();
    updateButtons();
    ensureLoop();
    setInterval(syncWithServer, syncIntervalMs);
})();
