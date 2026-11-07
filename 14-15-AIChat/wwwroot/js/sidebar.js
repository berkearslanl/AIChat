// Sidebar: sohbet satırlarındaki "..." menüsü (yeniden adlandır / sil)
(() => {
    const menu = document.getElementById("conversationMenu");
    const renameDialog = document.getElementById("renameDialog");
    const deleteDialog = document.getElementById("deleteDialog");
    if (!menu || !renameDialog || !deleteDialog) return;

    const renameForm = renameDialog.querySelector("form");
    const renameInput = renameForm.elements["title"];
    const deleteForm = deleteDialog.querySelector("form");
    const deleteTitle = document.getElementById("deleteTitle");

    // Menünün şu an hangi sohbet için açık olduğunu tutar: { id, title, button }
    let current = null;

    function closeMenu() {
        menu.classList.add("hidden");
        current?.button.classList.remove("opacity-100");
        current = null;
    }

    function openMenu(button) {
        current = { id: button.dataset.id, title: button.dataset.title, button };
        button.classList.add("opacity-100"); // menü açıkken "..." butonu görünür kalsın
        menu.classList.remove("hidden");     // ölçü alabilmek için önce göster

        const rect = button.getBoundingClientRect();
        let left = rect.right - menu.offsetWidth; // menünün sağı butonun sağıyla hizalı
        let top = rect.bottom + 4;
        if (top + menu.offsetHeight > window.innerHeight - 8) {
            top = rect.top - menu.offsetHeight - 4; // aşağıda yer yoksa yukarı aç
        }
        menu.style.left = `${Math.max(8, left)}px`;
        menu.style.top = `${top}px`;
    }

    // Tek bir click dinleyicisi: "..." butonuna tıklanırsa aç/kapat, dışarı tıklanırsa kapat
    document.addEventListener("click", (e) => {
        const button = e.target.closest("[data-menu-button]");
        if (button) {
            const wasOpenForThis = current?.button === button;
            closeMenu();
            if (!wasOpenForThis) openMenu(button);
            return;
        }
        if (!menu.contains(e.target)) closeMenu();
    });

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") closeMenu();
    });
    // Liste kaydırılınca ya da pencere yeniden boyutlanınca konum eskir, menüyü kapat
    document.addEventListener("scroll", closeMenu, true);
    window.addEventListener("resize", closeMenu);

    // Yeniden adlandır: menüden seçilince pencereyi aç, mevcut başlığı doldur
    document.getElementById("renameAction").addEventListener("click", () => {
        if (!current) return;
        const { id, title } = current;
        closeMenu();

        renameForm.elements["id"].value = id;
        renameDialog.dataset.originalTitle = title;
        renameInput.value = title;
        renameDialog.showModal();
        renameInput.select();
    });

    // Gönderilmeden önce kontrol: boşsa gönderme, değişmediyse sadece kapat
    renameForm.addEventListener("submit", (e) => {
        const newTitle = renameInput.value.trim();
        if (!newTitle) {
            e.preventDefault();
            renameInput.focus();
            return;
        }
        if (newTitle === renameDialog.dataset.originalTitle) {
            e.preventDefault();
            renameDialog.close();
            return;
        }
        renameInput.value = newTitle;
    });

    // Sil: onay penceresini aç (başlık textContent ile yazılır, HTML olarak yorumlanmaz)
    document.getElementById("deleteAction").addEventListener("click", () => {
        if (!current) return;
        const { id, title } = current;
        closeMenu();

        deleteForm.elements["id"].value = id;
        deleteTitle.textContent = `"${title}"`;
        deleteDialog.showModal();
    });

    // İki pencere için ortak: "İptal/Vazgeç" butonu ya da pencerenin dışındaki karartıya tıklayınca kapat
    // (Esc tuşunu <dialog> kendisi zaten kapatır)
    for (const dialog of [renameDialog, deleteDialog]) {
        dialog.addEventListener("click", (e) => {
            if (e.target === dialog || e.target.closest("[data-dialog-close]")) dialog.close();
        });
    }
})();
