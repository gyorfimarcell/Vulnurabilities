"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const express_1 = __importDefault(require("express"));
const app = (0, express_1.default)();
app.use(express_1.default.json());
const port = 8000;
app.post("/", (req, res) => {
    console.log(req.body);
    const labels = [
        "Spoofing",
        "Tampering",
        "Repudiation",
        "Information disclosure",
        "Denial of service",
        "Elevation of privilege",
    ];
    const selected = labels[Math.floor(Math.random() * labels.length)];
    res.send(selected);
});
app.listen(port, () => {
    console.log(`Listening on localhost:${port}`);
});
