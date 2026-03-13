const canvas = document.querySelector("canvas");
const sizeSlider = document.querySelector("#size-slider");
const cleatCanvas = document.querySelector(".clear-canvas");
const saveImage = document.querySelector(".save-img");
const ctx = canvas.getContext("2d");

//global variabels wiht default values
let prevMouseX, prevMouseY, snapshot,
    isDrawing = false,
    brushWidth = 5;
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

sizeSlider.addEventListener("change", () => brushWidth = sizeSlider.value)

cleatCanvas.addEventListener("click", () => {
    ctx.clearRect(0, 0, canvas.width, canvas.height)
    setCanvasBackground();
})

saveImage.addEventListener("click", () => {
    // TODO: Send to backend and run through Neural network
})

canvas.addEventListener("mousedown", startDraw);
canvas.addEventListener("mousemove", drawing);
canvas.addEventListener("mouseup", () => isDrawing = false);