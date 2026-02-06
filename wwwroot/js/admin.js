document.getElementById('generate-code-btn').onclick = function() {
    const code = Math.random().toString(36).substring(2, 10).toUpperCase();
    document.getElementById('code-input').value = code;
};
document.getElementById('open-create-user-modal-btn').onclick = function() {
    document.getElementById('create-user-modal').style.display = 'block';
};
document.getElementById('close-create-user-modal').onclick = function() {
    document.getElementById('create-user-modal').style.display = 'none';
};