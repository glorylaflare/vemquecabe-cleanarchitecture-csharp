window.onload = function () {
    const authBtn = document.querySelector('.swagger-ui .authorize');
    if (authBtn) {
        authBtn.textContent = '🔑 Autenticar';
    }
};