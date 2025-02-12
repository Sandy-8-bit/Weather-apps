window.showToast = (message, type = 'info') => {
    let bgColor = '#007bff'; // Default to blue for info
    if (type === 'success') bgColor = '#28a745'; // Green
    if (type === 'error') bgColor = '#dc3545';   // Red
    if (type === 'warning') bgColor = '#ffc107'; // Yellow

    let toastContainer = document.getElementById("toast-container");
    if (!toastContainer) {
        toastContainer = document.createElement("div");
        toastContainer.id = "toast-container";
        toastContainer.style.position = "fixed";
        toastContainer.style.top = "20px";
        toastContainer.style.right = "20px";
        toastContainer.style.zIndex = "9999";
        document.body.appendChild(toastContainer);
    }

    let toast = document.createElement("div");
    toast.style.background = bgColor;
    toast.style.color = "white";
    toast.style.padding = "10px 15px";
    toast.style.marginBottom = "5px";
    toast.style.borderRadius = "5px";
    toast.style.boxShadow = "0 4px 6px rgba(0,0,0,0.1)";
    toast.style.minWidth = "200px";
    toast.style.fontSize = "14px";
    toast.textContent = message;

    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = "0";
        setTimeout(() => toast.remove(), 500);
    }, 3000);
};
