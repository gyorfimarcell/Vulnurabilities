import express from "express";
const app = express();
app.use(express.json());
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
