function autoSaveAppointmentDraft() {
    const form = document.getElementById('appointmentForm');
    if (form) {
        setInterval(() => {
            const formData = new FormData(form);
            fetch('/Appointments/SaveDraft', {
                method: 'POST',
                body: formData
            });
        }, 30000); 
    }
}

document.addEventListener('DOMContentLoaded', autoSaveAppointmentDraft);