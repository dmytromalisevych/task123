const addPhotoButton = document.getElementById('addPhoto');
const fullscreenToggle = document.getElementById('fullscreenToggle');
const fullscreenExit = document.getElementById('fullscreenExit');
const geoLocationButton = document.getElementById('geoLocation');
const gallery = document.getElementById('gallery');
const timerDisplay = document.getElementById('timer');
const geoModal = document.getElementById('geoModal');
const geoBackdrop = document.getElementById('geoBackdrop');
const coordinatesDisplay = document.getElementById('coordinates');
const closeModalButton = document.getElementById('closeModal');

let totalTime = 0;
let timerInterval = null;

function startTimer() {
    if (!timerInterval) {
        timerInterval = setInterval(() => {
            totalTime++;
            updateTimerDisplay();
        }, 1000);
    }
}

function stopTimer() {
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }
}

function updateTimerDisplay() {
    if (totalTime < 60) {
        timerDisplay.textContent = `Час на сторінці: ${totalTime}s`;
    } else {
        const minutes = Math.floor(totalTime / 60);
        const seconds = totalTime % 60;
        timerDisplay.textContent = `Час на сторінці: ${minutes}m ${seconds}s`;
    }
}

document.addEventListener('visibilitychange', () => {
    if (document.hidden) {
        stopTimer();
    } else {
        startTimer();
    }
});

window.addEventListener('load', () => {
    startTimer();
    loadGalleryFromLocalStorage();
});

window.addEventListener('beforeunload', saveGalleryToLocalStorage);

addPhotoButton.addEventListener('click', () => {
    fetch('https://dog.ceo/api/breeds/image/random')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            const img = document.createElement('img');
            img.src = data.message;
            img.alt = 'Dog photo';
            gallery.appendChild(img);
            saveGalleryToLocalStorage();
        })
        .catch(error => {
            console.error('Failed to fetch photo:', error);
            alert('Не вдалося завантажити фото. Спробуйте ще раз пізніше.');
        });
});

function saveGalleryToLocalStorage() {
    const images = Array.from(gallery.children).map(img => img.src);
    localStorage.setItem('gallery', JSON.stringify(images));
}

function loadGalleryFromLocalStorage() {
    const images = JSON.parse(localStorage.getItem('gallery') || '[]');
    images.forEach(src => {
        const img = document.createElement('img');
        img.src = src;
        img.alt = 'Dog photo';
        gallery.appendChild(img);
    });
}

fullscreenToggle.addEventListener('click', () => {
    if (gallery.requestFullscreen) {
        gallery.requestFullscreen();
    }
    gallery.classList.add('fullscreen');
    fullscreenExit.style.display = 'block';
});

fullscreenExit.addEventListener('click', () => {
    if (document.exitFullscreen) {
        document.exitFullscreen();
    }
    gallery.classList.remove('fullscreen');
    fullscreenExit.style.display = 'none';
});

geoLocationButton.addEventListener('click', () => {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(
            position => {
                const { latitude, longitude } = position.coords;
                coordinatesDisplay.textContent = `Latitude: ${latitude.toFixed(5)}, Longitude: ${longitude.toFixed(5)}`;
                geoBackdrop.style.display = 'block';
                geoModal.style.display = 'block';
            },
            error => {
                coordinatesDisplay.textContent = 'Unable to fetch location.';
                console.error(error);
            }
        );
    } else {
        coordinatesDisplay.textContent = 'Geolocation is not supported by your browser.';
    }
});

closeModalButton.addEventListener('click', () => {
    geoModal.style.display = 'none';
    geoBackdrop.style.display = 'none';
});
