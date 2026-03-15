const canvas = document.querySelector("canvas");
const cleatCanvas = document.querySelector(".clear-canvas");
const saveImage = document.querySelector(".save-img");
const results = document.querySelector("#result-number");
const confidence = document.querySelector("#result-confidence");
const ctx = canvas.getContext("2d");

let prevMouseX, prevMouseY, snapshot,
    isDrawing = false,
    brushWidth = 60;
const selectedColor = "#000";

const setCanvasBackground = () => {
    ctx.fillStyle = "#fff";
    ctx.fillRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = selectedColor;
}

window.addEventListener("load", () => {    
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
    setCanvasBackground();
});

const startDraw = (e) => {
    isDrawing = true;
    prevMouseX = e.offsetX; 
    prevMouseY = e.offsetY;
    ctx.beginPath();
    ctx.lineWidth = brushWidth;
    ctx.strokeStyle = selectedColor;
    ctx.fillStyle = selectedColor;
    snapshot = ctx.getImageData(0, 0, canvas.width, canvas.height);
}

const drawPencil = (e) => {
    ctx.lineTo(e.offsetX, e.offsetY);
    ctx.stroke();
}

const drawing = (e) => {
    if (!isDrawing) return;
    ctx.putImageData(snapshot, 0, 0);
    drawPencil(e);
}

cleatCanvas.addEventListener("click", () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    setCanvasBackground();
})

saveImage.addEventListener("click", () => {
    canvas.toBlob(blob => {

        const formData = new FormData();
        formData.append("image", blob);

        fetch("/", {
            body: formData,
            method: 'post'
        }).then(res => res.json()).then(x => {
            results.innerText = x.value
            confidence.innerText = x.confidence
        });
    },"image/png");
})

canvas.addEventListener("mousedown", startDraw);
canvas.addEventListener("mousemove", drawing);
canvas.addEventListener("mouseup", () => isDrawing = false);