# 🤖 Aiva Chat — Yapay Zeka Sohbet Uygulaması

**Aiva Chat**, **ASP.NET Core MVC (.NET 10)** ile geliştirilmiş, **SignalR** üzerinden gerçek zamanlı akan yanıtlar sunan, **sesli soru sorma ve yanıtları sesli dinleme** desteğine sahip bir yapay zeka sohbet uygulamasıdır.

Uygulama; yanıtları **OpenAI Responses API** üzerinden kelime kelime akış (streaming) olarak ileten, **ElevenLabs** ile konuşmayı metne ve metni sese dönüştüren, **ASP.NET Core Identity** ile her kullanıcıya yalnızca kendi sohbet geçmişini sunan ve tüm verileri **PostgreSQL** üzerinde saklayan uçtan uca bir yapıya sahiptir.

---

## 🌟 Öne Çıkan Özellikler

| Özellik | Açıklama |
|---|---|
| ⚡ **Gerçek Zamanlı Akış (SignalR)** | Yapay zeka yanıtlarının tamamlanmasını beklemeden, üretildiği anda parça parça ekrana aktarılması. |
| 🧠 **OpenAI Responses API** | Sohbet geçmişinin tamamı bağlam olarak gönderilerek tutarlı, çok turlu konuşmalar; `IAsyncEnumerable` ile akış okuma. |
| 🎙️ **Sesli Soru Sorma (Speech-to-Text)** | Tarayıcıda mikrofonla kaydedilen sesin ElevenLabs ile metne dönüştürülüp mesaj kutusuna aktarılması. |
| 🔊 **Yanıtları Sesli Dinleme (Text-to-Speech)** | Her yapay zeka yanıtının tek tıkla Türkçe seslendirilmesi; okunma sırasında animasyonlu geri bildirim. |
| 🔐 **ASP.NET Core Identity** | Kayıt, giriş ve çıkış işlemleri; cookie tabanlı oturum yönetimi ve hesap kilitleme. |
| 🛡️ **Kullanıcıya Özel Veri İzolasyonu** | Sohbet, mesaj, silme ve yeniden adlandırma sorgularının tamamında oturumdaki kullanıcıya göre sahiplik kontrolü. |
| 📝 **Markdown Desteği** | Yanıtlardaki başlık, liste, tablo ve kod bloklarının biçimlendirilmiş gösterimi; **DOMPurify** ile XSS koruması. |
| 🗂️ **Tarihe Göre Gruplanmış Geçmiş** | Sohbetlerin *Bugün*, *Dün*, *Önceki 7 Gün* ve *Daha Eski* başlıkları altında listelenmesi. |
| 🔄 **Sayfa Yenilemeden Güncelleme** | Yeni sohbet oluştuğunda kenar çubuğunun ve adres çubuğunun sayfa yeniden yüklenmeden güncellenmesi. |
| 📶 **Canlı Bağlantı Durumu** | SignalR bağlantısının *Bağlı*, *Bağlanıyor* ve *Bağlantı yok* durumlarının anlık gösterimi; otomatik yeniden bağlanma. |

---

## 🖥️ Fonksiyonel Özellikler

### 💬 Sohbet
- **Yeni Sohbet:** İlk mesajla birlikte sohbetin otomatik oluşturulması ve ilk mesajdan başlık üretilmesi.
- **Akışlı Yanıtlar:** "Yapay zeka düşünüyor" göstergesi ile başlayan ve metnin canlı olarak yazıldığı yanıt balonları.
- **Bağlamlı Konuşma:** Sohbetteki önceki mesajların her istekte modele iletilmesi.
- **Aktif Model Gösterimi:** Kullanılan modelin üst bilgi çubuğunda gösterilmesi.

### 🎧 Ses
- **Mikrofon ile Mesaj:** `MediaRecorder` ile kayıt, kayıt sırasında animasyonlu mikrofon butonu, metne dönüştürülen sesin mesaj kutusuna yazılması.
- **Sesli Dinleme:** Yanıt altındaki dinle butonu ile seslendirme, tekrar tıklayınca durdurma, okunurken avatar ve dalga animasyonu.

### 🗂️ Sohbet Yönetimi
- **Yeniden Adlandırma:** Üç nokta menüsünden açılan pencere ile başlık güncelleme.
- **Silme:** Onay penceresi ile sohbetin ve tüm mesajlarının kalıcı olarak silinmesi.
- **Sohbetler Arası Geçiş:** Kenar çubuğundan geçmiş sohbetlerin açılması ve aktif sohbetin vurgulanması.

### 👤 Hesap
- **Kayıt ve Giriş:** Ad soyad, e-posta ve şifre ile kayıt; "Beni hatırla" seçeneği ile giriş.
- **Türkçe Hata Mesajları:** Identity doğrulama mesajlarının özel bir `IdentityErrorDescriber` ile Türkçeleştirilmesi.
- **Güvenli Yönlendirme:** Girişten sonra yalnızca uygulama içi adreslere dönüş (`returnUrl` kontrolü).

---

## 🔐 Güvenlik Yaklaşımı

- **Sahiplik kontrolü:** Kimlik doğrulamanın yanında her veri erişiminde kaydın oturumdaki kullanıcıya ait olup olmadığı kontrol edilir; başka bir kullanıcının sohbet numarası adres çubuğuna yazılsa bile içeriğe erişilemez.
- **SignalR Hub yetkilendirmesi:** Hub `[Authorize]` ile korunur, kullanıcı kimliği istemciden alınmaz, doğrudan oturumdan (`Context.UserIdentifier`) okunur.
- **XSS koruması:** Yapay zekadan gelen Markdown içeriği `marked` ile HTML'e çevrildikten sonra `DOMPurify` ile temizlenerek sayfaya eklenir.
- **CSRF koruması:** Silme, yeniden adlandırma ve çıkış işlemleri antiforgery token ile korunan POST formları üzerinden yapılır.
- **Gizli bilgiler:** API anahtarları ve bağlantı dizesi kaynak kodda değil, **User Secrets** içinde tutulur.
- **İstek sınırları:** Seslendirilecek metin ve yüklenen ses dosyası için boyut sınırları uygulanır.

---

## 🛠️ Kullanılan Teknolojiler

| Kategori | Teknolojiler |
|---|---|
| **Framework** | ASP.NET Core MVC (.NET 10) |
| **Programlama Dili** | C# |
| **Gerçek Zamanlı İletişim** | SignalR |
| **Yapay Zeka** | OpenAI Responses API (GPT-5) |
| **Ses** | ElevenLabs Text-to-Speech ve Speech-to-Text (Scribe) |
| **ORM** | Entity Framework Core 10 |
| **Veritabanı** | PostgreSQL (Npgsql) |
| **Kimlik Doğrulama** | ASP.NET Core Identity, Cookie Authentication |
| **API İletişimi** | Typed `HttpClient`, Server-Sent Events okuma |
| **Arayüz** | Razor, ViewComponent, Tailwind CSS, JavaScript |
| **Markdown** | marked, DOMPurify |
| **Tarayıcı API'leri** | MediaRecorder, getUserMedia, Audio, Fetch, History API |
| **Gizli Bilgi Yönetimi** | .NET User Secrets |

---

## 🎬 Demo

https://github.com/user-attachments/assets/4ff96c09-9daf-4dcc-b41c-1b6358d0dfcd

