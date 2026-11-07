// 1) Bağlantıyı oluştur (diğer her şey buna bağlı, o yüzden en üstte)
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .withAutomaticReconnect()
    .build();

// 2) Sayfadaki öğeler ve durum
const main = document.querySelector("main");
let conversationId = Number(main.dataset.conversationId) || null;

const container = document.getElementById("messageContainer");
const typingIndicator = document.getElementById("typingIndicator");
const userTemplate = document.getElementById("userMessageTemplate");
const aiTemplate = document.getElementById("aiMessageTemplate");
const input = document.getElementById("messageInput");
const sendButton = document.getElementById("sendButton");
let isWaiting = false;
let currentAiContent = null;
let currentAiText = "";

// 3) Yardımcı fonksiyonlar
function scrollToBottom(behavior = "smooth") {
    const scroller = container.parentElement;
    scroller.scrollTo({ top: scroller.scrollHeight, behavior });
}
function startAiBubble() {
    const node = aiTemplate.content.cloneNode(true);
    currentAiContent = node.querySelector("[data-content]");
    currentAiText = ""; 
    container.insertBefore(node, typingIndicator);
}

function addMessage(template, text) {
    const node = template.content.cloneNode(true);
    node.querySelector("[data-content]").textContent = text;
    container.insertBefore(node, typingIndicator);
    scrollToBottom();
}

function setWaiting(value) {
    isWaiting = value;
    typingIndicator.classList.toggle("hidden", !value);
    input.disabled = value;
    if (!value) input.focus();
}

function renderMarkdown(text) {
    return DOMPurify.sanitize(marked.parse(text));
}

// Sidebar'daki sohbet listesini sunucudan yeniden çek ve yerine koy
async function refreshSidebar(selectedId) {
    const response = await fetch(`/Home/Sidebar/${selectedId}`);
    if (!response.ok) return;

    const doc = new DOMParser().parseFromString(await response.text(), "text/html");
    const freshList = doc.getElementById("conversationList");
    if (freshList) {
        document.getElementById("conversationList").innerHTML = freshList.innerHTML;
    }
}
// 4) Sunucudan gelen mesajları dinle (start'tan ÖNCE tanımla)
connection.on("ReceiveChunk", (chunk) => {
    if (!currentAiContent) {
        typingIndicator.classList.add("hidden");
        startAiBubble();
    }
    currentAiText += chunk;
    currentAiContent.innerHTML = renderMarkdown(currentAiText);
    scrollToBottom("auto");
});

connection.on("ReceiveComplete", async (data) => {
    currentAiContent = null;
    setWaiting(false);
    conversationId = data.conversationId;

    if (data.isNewConversation) {
        main.dataset.conversationId = data.conversationId;
        history.replaceState(null, "", `/Home/Index/${data.conversationId}`);
    }

    await refreshSidebar(data.conversationId);
});

connection.on("ReceiveError", (text) => {
    currentAiContent = null;
    setWaiting(false);
    addMessage(aiTemplate, text);
});
connection.onreconnecting(() => setConnectionStatus("connecting"));
connection.onreconnected(() => setConnectionStatus("connected"));
connection.onclose(() => setConnectionStatus("disconnected"));
// 5) Bağlantıyı başlat
connection.start()
    .then(() => {
        console.log("SignalR bağlandı");
        setConnectionStatus("connected");
    })
    .catch(err => {
        console.error("Bağlantı hatası:", err);
        setConnectionStatus("disconnected");
    });

// 6) Mesaj gönderme
async function sendMessage() {
    const text = input.value.trim();
    if (!text || isWaiting) return;

    if (connection.state !== signalR.HubConnectionState.Connected) {
        console.warn("Henüz bağlı değil");
        return;
    }

    // Boş sohbet karşılama bloğu ilk mesajla birlikte kalkar
    document.getElementById("emptyState")?.remove();

    addMessage(userTemplate, text);
    input.value = "";
    setWaiting(true);

    try {
        await connection.invoke("SendMessage", conversationId, text);
    } catch (err) {
        console.error(err);
        setWaiting(false);
        addMessage(aiTemplate, "Bir hata oluştu.");
    }
}

// 7) Olaylar
sendButton.addEventListener("click", sendMessage);

input.addEventListener("keydown", (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        sendMessage();
    }
});

// 8) Sunucunun sayfaya bastığı geçmiş AI mesajlarını Markdown'a çevir
document.querySelectorAll("[data-markdown]").forEach(el => {
    el.innerHTML = renderMarkdown(el.textContent);
});

const statusDot = document.querySelector("[data-status-dot]");
const statusText = document.querySelector("[data-status-text]");

// Her durum için: gösterilecek yazı + nokta renk sınıfları
const connectionStates = {
    connecting: { text: "Bağlanıyor...", dot: "bg-outline animate-pulse" },
    connected: { text: "Bağlı", dot: "bg-primary" },
    disconnected: { text: "Bağlantı yok", dot: "bg-error" },
};

function setConnectionStatus(state) {
    const s = connectionStates[state];
    statusText.textContent = s.text;
    statusDot.className = `w-2 h-2 rounded-full ${s.dot}`;
}

// 9) Sesli okuma (TTS)
const speakIcons = { idle: "volume_up", loading: "progress_activity", playing: "stop_circle" };

let currentAudio = null;
let currentSpeakButton = null;
let speakAbort = null;

function setSpeakState(button, state) {
    const icon = button.querySelector(".material-symbols-outlined");
    icon.textContent = speakIcons[state];
    icon.classList.toggle("animate-spin", state === "loading");
    button.closest("[data-ai-message]").classList.toggle("speaking", state === "playing");
}

// Kod bloklarını (<pre>) atıp geri kalan metni al
function getSpeakableText(contentEl) {
    const clone = contentEl.cloneNode(true);
    clone.querySelectorAll("pre").forEach(p => p.remove());
    return clone.textContent.trim();
}

function stopSpeaking() {
    speakAbort?.abort();          // henüz inmekte olan istek varsa iptal et
    speakAbort = null;

    if (currentAudio) {
        currentAudio.pause();
        URL.revokeObjectURL(currentAudio.src);   // bellekteki geçici adresi serbest bırak
        currentAudio = null;
    }
    if (currentSpeakButton) {
        setSpeakState(currentSpeakButton, "idle");
        currentSpeakButton = null;
    }
}

async function toggleSpeak(button) {
    const wasThisOne = currentSpeakButton === button;
    stopSpeaking();
    if (wasThisOne) return;       // aynı butona tekrar basıldı: sadece durdur

    const root = button.closest("[data-ai-message]");
    const text = getSpeakableText(root.querySelector("[data-content]"));
    if (!text) return;

    currentSpeakButton = button;
    setSpeakState(button, "loading");
    speakAbort = new AbortController();

    try {
        const response = await fetch("/api/voice/speak", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ text }),
            signal: speakAbort.signal
        });
        if (!response.ok) throw new Error(`Ses isteği başarısız (${response.status})`);

        const audio = new Audio(URL.createObjectURL(await response.blob()));
        audio.addEventListener("ended", stopSpeaking);
        currentAudio = audio;
        setSpeakState(button, "playing");
        await audio.play();
    } catch (err) {
        if (err.name !== "AbortError") console.error(err);   // iptal normal, hata değil
        if (currentSpeakButton === button) stopSpeaking();
    }
}

// Mesajlar sonradan eklendiği için tek bir dinleyici kapta duruyor (event delegation)
container.addEventListener("click", (e) => {
    const button = e.target.closest("[data-speak-button]");
    if (button) toggleSpeak(button);
});
// 10) Mikrofon (STT)
const micButton = document.getElementById("micButton");
let mediaRecorder = null;
let micStream = null;
let audioChunks = [];

function setMicState(state) {   // "idle" | "recording" | "transcribing"
    const icon = micButton.querySelector(".material-symbols-outlined");
    icon.textContent = { idle: "mic", recording: "stop_circle", transcribing: "progress_activity" }[state];
    icon.classList.toggle("animate-spin", state === "transcribing");
    micButton.classList.toggle("recording", state === "recording");
    micButton.disabled = state === "transcribing";
}

async function startRecording() {
    stopSpeaking();   // hoparlörden ses çalıyorsa kayda karışmasın

    try {
        micStream = await navigator.mediaDevices.getUserMedia({ audio: true });
    } catch (err) {
        console.error("Mikrofon izni alınamadı:", err);
        return;
    }

    audioChunks = [];
    mediaRecorder = new MediaRecorder(micStream);
    mediaRecorder.addEventListener("dataavailable", (e) => {
        if (e.data.size > 0) audioChunks.push(e.data);
    });
    mediaRecorder.addEventListener("stop", onRecordingStopped);
    mediaRecorder.start();
    setMicState("recording");
}

function stopRecording() {
    mediaRecorder.stop();                            // 'stop' olayını tetikler
    micStream.getTracks().forEach(t => t.stop());    // mikrofonu kapat
}

async function onRecordingStopped() {
    const blob = new Blob(audioChunks, { type: mediaRecorder.mimeType });
    mediaRecorder = null;

    if (blob.size === 0) {
        setMicState("idle");
        return;
    }

    setMicState("transcribing");
    const form = new FormData();
    form.append("audio", blob, blob.type.includes("mp4") ? "kayit.mp4" : "kayit.webm");

    try {
        const response = await fetch("/api/voice/transcribe", { method: "POST", body: form });
        if (!response.ok) throw new Error(`Ses tanıma başarısız (${response.status})`);

        const { text } = await response.json();
        if (text) {
            input.value = input.value ? `${input.value} ${text}` : text;
            input.focus();
        }
    } catch (err) {
        console.error(err);
    } finally {
        setMicState("idle");
    }
}

micButton.addEventListener("click", () => {
    if (isWaiting) return;   // AI cevap yazarken kayıt başlatma
    if (mediaRecorder?.state === "recording") stopRecording();
    else startRecording();
});