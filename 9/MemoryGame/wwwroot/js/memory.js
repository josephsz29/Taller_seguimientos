// Lógica del navegador: solo dibuja y pregunta al servidor.
// El servidor decide si hubo pareja, cuenta los movimientos y calcula el puntaje.
(function () {
    const board = document.getElementById('board');
    if (!board) {
        return;
    }

    const flipUrl = board.dataset.flipUrl;
    const gameId = board.dataset.gameId;
    const mismatchDelayMs = 900;

    let finished = board.dataset.finished === 'true';
    let started = board.dataset.started === 'true';
    let elapsedSeconds = parseInt(board.dataset.elapsed, 10) || 0;
    let busy = false;      // evita clics mientras se espera al servidor o se voltean cartas de vuelta
    let timerId = null;

    const movesLabel = document.getElementById('movesLabel');
    const pairsLabel = document.getElementById('pairsLabel');
    const timeLabel = document.getElementById('timeLabel');
    const messageLabel = document.getElementById('gameMessage');
    const resultPanel = document.getElementById('resultPanel');

    function formatTime(totalSeconds) {
        const minutes = Math.floor(totalSeconds / 60);
        const seconds = totalSeconds % 60;
        return String(minutes).padStart(2, '0') + ':' + String(seconds).padStart(2, '0');
    }

    function showMessage(text) {
        messageLabel.textContent = text || '';
    }

    // Cronómetro visual (el tiempo oficial lo calcula el servidor)
    function startTimer() {
        if (timerId !== null || finished) {
            return;
        }

        timerId = setInterval(function () {
            elapsedSeconds++;
            timeLabel.textContent = formatTime(elapsedSeconds);
        }, 1000);
    }

    function stopTimer() {
        clearInterval(timerId);
        timerId = null;
    }

    function getCard(index) {
        return board.querySelector('[data-index="' + index + '"]');
    }

    function reveal(card, symbol) {
        card.querySelector('.memory-card-back').textContent = symbol;
        card.classList.add('revealed');
    }

    function hide(card) {
        card.classList.remove('revealed');

        // Quitamos el símbolo cuando termina la animación de giro
        setTimeout(function () {
            card.querySelector('.memory-card-back').textContent = '';
        }, 400);
    }

    function finishGame(result) {
        finished = true;
        stopTimer();

        document.getElementById('scoreLabel').textContent = result.score;
        document.getElementById('finalMovesLabel').textContent = result.moves;
        document.getElementById('finalTimeLabel').textContent = result.durationLabel;
        timeLabel.textContent = result.durationLabel;

        resultPanel.classList.remove('d-none');
        resultPanel.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }

    async function flipCard(card) {
        if (busy || finished || card.classList.contains('revealed')) {
            return;
        }

        busy = true;
        showMessage('');

        try {
            const token = document.querySelector('#csrfForm input[name="__RequestVerificationToken"]').value;

            const response = await fetch(flipUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token
                },
                body: 'id=' + encodeURIComponent(gameId) + '&cardIndex=' + encodeURIComponent(card.dataset.index)
            });

            const result = await response.json();

            if (!result.success) {
                showMessage(result.error);
                busy = false;
                return;
            }

            reveal(card, result.symbol);

            if (!started) {
                started = true;
                startTimer();
            }

            movesLabel.textContent = result.moves;
            pairsLabel.textContent = result.matchedPairs;

            // Primera carta del intento: esperamos la segunda
            if (!result.isSecondFlip) {
                busy = false;
                return;
            }

            const firstCard = getCard(result.firstCardIndex);

            if (result.isMatch) {
                card.classList.add('matched');
                firstCard.classList.add('matched');

                if (result.isFinished) {
                    finishGame(result);
                }

                busy = false;
            } else {
                // No coinciden: las dejamos ver un momento y las volteamos de vuelta
                setTimeout(function () {
                    hide(card);
                    hide(firstCard);
                    busy = false;
                }, mismatchDelayMs);
            }
        } catch (error) {
            showMessage('No se pudo conectar con el servidor. Intenta de nuevo.');
            busy = false;
        }
    }

    // Un solo "escuchador" para todo el tablero
    board.addEventListener('click', function (event) {
        const card = event.target.closest('.memory-card');
        if (card) {
            flipCard(card);
        }
    });

    timeLabel.textContent = formatTime(elapsedSeconds);

    // Si la página se recargó a mitad de partida, el cronómetro continúa
    if (started && !finished) {
        startTimer();
    }
})();
