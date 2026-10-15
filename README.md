# 🎵 Melodia

Abonelik paketine göre şarkı dinleme yetkisi veren, uçtan uca bir müzik platformu.
Backend **.NET 9 · Clean Architecture + DDD + CQRS · Minimal API**, frontend **Angular 21 · PrimeNG 21 · Signals**.

Kullanıcılar Basic, Gold, Premium veya Elit pakete sahiptir; her şarkı dinlenebilmesi için en az bir paket gerektirir. Basic bir kullanıcı Gold bir şarkıyı çalmaya çalıştığında API **403** döner ve arayüz paket yükseltme penceresini açar. Kullanıcı yetkiliyse kısa ömürlü, imzalı bir stream bağlantısı üretilir ve MP3 ileri/geri sarma desteğiyle akıtılır.

<img width="1898" height="940" alt="Ekran görüntüsü 2026-10-15 223728" src="https://github.com/user-attachments/assets/c5012488-ff2c-4b90-affd-6a4cc043161b" />


---

## İçindekiler

- [Ekran Görüntüleri](#ekran-görüntüleri)
- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Mimari](#mimari)
- [Domain Modeli](#domain-modeli)
- [Öne Çıkan Tasarım Kararları](#öne-çıkan-tasarım-kararları)
- [Frontend](#frontend)
- [API Endpoint'leri](#api-endpointleri)
- [Hata Formatı](#hata-formatı)
- [Kurulum](#kurulum)
- [Test](#test)
- [Bilinen Sınırlar](#bilinen-sınırlar)
- [Yol Haritası](#yol-haritası)

---

## Ekran Görüntüleri
<img width="1914" height="944" alt="Ekran görüntüsü 2026-10-15 223447" src="https://github.com/user-attachments/assets/348f71f6-d203-400b-b2ec-c866e6ffc5de" />
<img width="1918" height="660" alt="Ekran görüntüsü 2026-10-15 223433" src="https://github.com/user-attachments/assets/a37e331d-d2e7-4b0b-9b17-7c951b24c6f8" />
<img width="1900" height="938" alt="Ekran görüntüsü 2026-10-15 223318" src="https://github.com/user-attachments/assets/ce43b21b-cf28-4bbc-a620-a45061fe333e" />
<img width="1906" height="942" alt="Ekran görüntüsü 2026-10-15 223302" src="https://github.com/user-attachments/assets/9046d623-1d4e-4866-9f2c-40633e513f41" />
<img width="1914" height="936" alt="Ekran görüntüsü 2026-10-15 223454" src="https://github.com/user-attachments/assets/b5ac831c-4816-4b26-900e-5d2e841f7b46" />

<img width="1896" height="932" alt="Ekran görüntüsü 2026-10-15 223225" src="https://github.com/user-attachments/assets/e1ed6a9a-aad2-40e5-ada2-f6d52530101f" />
<img width="1900" height="936" alt="Ekran görüntüsü 2026-10-15 223158" src="https://github.com/user-attachments/assets/de9065f7-88cb-4b3c-991a-64f5f199d37a" />
<img width="1898" height="936" alt="Ekran görüntüsü 2026-10-15 223150" src="https://github.com/user-attachments/assets/5135e65f-e172-43ff-90c9-6e6725f2a169" />
<img width="1904" height="940" alt="Ekran görüntüsü 2026-10-15 223256" src="https://github.com/user-attachments/assets/b52e8ca5-c1e9-4b00-b397-ecc56d9a81a8" />
<img width="1904" height="946" alt="Ekran görüntüsü 2026-10-15 223239" src="https://github.com/user-attachments/assets/885f0bcd-cd4c-4a7d-a46d-c47724341245" />

---

## Özellikler

- **Paket tabanlı erişim:** Basic < Gold < Premium < Elit hiyerarşisi; çalma ve çalma listesine ekleme kuralları Domain'de
- **Güvenli müzik akışı:** JWT gönderemeyen `<audio>` etiketi için HMAC imzalı, 5 dakikalık stream bağlantısı; Range desteği ile ileri/geri sarma
- **Kimlik doğrulama:** JWT access token (15 dk) + hash'lenmiş, **rotation** yapılan refresh token (7 gün), çalıntı token tespiti
- **403 → yükseltme akışı:** Kilitli şarkı backend'e sorulur, 403 gelince pencere açılır, paket değişir, token yenilenir ve şarkı otomatik çalar
- **Kalıcı player:** Sayfalar arasında gezerken müzik durmaz; ilerleme çubuğu, ses kontrolü, ileri/geri sarma
- **Katalog yönetimi (Admin):** Tür, sanatçı, albüm ve şarkı CRUD'ları; MP3 yükleme (süre dosyadan otomatik okunur)
- **Çalma listeleri:** Oluşturma, yeniden adlandırma, silme, şarkı ekleme/çıkarma; sahiplik ve paket kuralları
- **Keşfet:** Debounce'lu arama, tür filtresi, sunucu taraflı sayfalama
- **Dinleme geçmişi ve öneriler:** Son dinlenenler; tür/sanatçı zevki ve popülerliğe göre kişisel öneriler
- **Soft delete:** Yayından kaldırılan şarkılar geçmişi bozmadan gizlenir, listelerde "Kullanılamıyor" görünür
- **Domain event'ler:** Kayıt ve paket değişikliğinde e-posta bildirimi
- **Tutarlı hata yönetimi:** Backend'de `Result` + `IExceptionHandler` → RFC 7807 ProblemDetails; frontend bu formatı tek bir yerden yorumlar
- **Yapılandırılmış loglama:** Serilog → Console + SQL Server, TraceId ile istek takibi
- **Güvenlik:** Rate limiting, brute force kilidi, path traversal koruması, timing-safe imza karşılaştırma, open redirect koruması

---

## Teknolojiler

### Backend
| Alan | Teknoloji |
|---|---|
| Framework | .NET 9, ASP.NET Core Minimal API |
| Veritabanı | SQL Server, Entity Framework Core 9 |
| Kimlik | ASP.NET Core Identity, JWT Bearer |
| CQRS / Mediator | MediatR |
| Doğrulama | FluentValidation |
| Loglama | Serilog (Console + MSSqlServer) |
| E-posta | MailKit |
| Dokümantasyon | OpenAPI + Scalar |

### Frontend
| Alan | Teknoloji |
|---|---|
| Framework | Angular 21 (standalone, zoneless, signals) |
| UI | PrimeNG 21 (Aura preset üzerine özel "Obsidian Emerald" teması) |
| State | Angular Signals (`signal`, `computed`, `effect`) |
| Asenkron | RxJS (`switchMap`, `debounceTime`, `shareReplay`) |
| Formlar | Reactive Forms (typed, `NonNullableFormBuilder`) |
| Stil | SCSS, CSS custom properties, BEM |

> **Neden PrimeNG 21?** PrimeNG 22 ile lisans modeli değişti ve lisans anahtarı zorunlu hale geldi. Açık kaynak bir repoda anahtar paylaşmamak için MIT lisanslı son ana sürüm olan 21 tercih edildi.

---

## Mimari

Bağımlılıklar her zaman **içe doğru**: Domain hiçbir katmanı tanımaz; teknoloji değişikliği iş kurallarını etkilemez.

```
┌──────────────────────────────────────────────────────┐
│  Api            Program.cs, Serilog, exception handler│
│ ┌──────────────────────────────────────────────────┐ │
│ │ Presentation │  Persistence  │  Infrastructure   │ │
│ │ ┌──────────────────────────────────────────────┐ │ │
│ │ │              Application                     │ │ │
│ │ │ ┌──────────────────────────────────────────┐ │ │ │
│ │ │ │                Domain                    │ │ │ │
│ │ │ └──────────────────────────────────────────┘ │ │ │
│ │ └──────────────────────────────────────────────┘ │ │
│ └──────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────┘
```

| Katman | Sorumluluk |
|---|---|
| **Domain** | Entity, value object, aggregate, domain event, domain service, repository arayüzleri, `Result` / `Error` |
| **Application** | Command/query ve handler'lar, validator'lar, pipeline behavior'lar, DTO'lar, altyapı arayüzleri |
| **Persistence** | `AppDbContext`, EF configuration'lar, repository'ler, migration'lar, domain event dağıtımı |
| **Infrastructure** | Identity, JWT, refresh token rotation, imzalı stream URL, dosya depolama, e-posta |
| **Presentation** | Minimal API endpoint'leri, `Result` → ProblemDetails dönüşümü |
| **Api** | Composition root: DI, middleware sırası, Serilog, rate limiting, seed |

### Proje yapısı

```
Melodia
├── src
│   ├── Melodia.Domain
│   │   ├── Abstractions      Entity, Result, Error, ValidationError, IDomainEvent, IUnitOfWork
│   │   ├── Shared            Package, Duration, Email (value object'ler)
│   │   ├── Songs             Song, AudioFile, SongErrors, ISongRepository
│   │   ├── Listeners         Listener, event'ler, ListenerErrors, IListenerRepository
│   │   ├── Playlists         Playlist, PlaylistSong, PlaylistErrors, IPlaylistRepository
│   │   ├── Artists | Albums | Genres | Listenings
│   │   └── Services          PlaybackPolicy
│   ├── Melodia.Application
│   │   ├── Abstractions      Messaging, Data, Authentication, Storage, Notifications
│   │   ├── Behaviors         LoggingBehavior, ValidationBehavior
│   │   ├── Common            PagedResponse, ValidationExtensions
│   │   └── Features          Auth, Songs, Artists, Albums, Genres, Playlists, Listeners, Listenings
│   ├── Melodia.Persistence   AppDbContext, Configurations, Repositories, Migrations, Identity
│   ├── Melodia.Infrastructure Authentication, Storage, Notifications
│   ├── Melodia.Presentation  Endpoints, Extensions
│   └── Melodia.Api           Program.cs, GlobalExceptionHandler, DatabaseSeeder
├── melodia-web               Angular istemci (aşağıda)
├── samples                   Örnek ses dosyaları ve kurulum script'i
└── docs                      Ekran görüntüleri, Postman koleksiyonu
```

Domain ve Application **kavrama göre** klasörlenmiştir (`Songs`, `Playlists`); bir senaryoya ait command, validator ve handler aynı klasördedir.

---

## Domain Modeli

### Aggregate'ler

| Aggregate | Açıklama |
|---|---|
| **Song** | Başlık, süre, ses dosyası, gerekli paket; sanatçı, tür ve albüme **sadece Id ile** bağlı. Soft delete. |
| **Listener** | Dinleyici; e-posta, görünen ad, paket. Identity kullanıcısıyla **aynı Id**'yi paylaşır. |
| **Playlist** | Sahibi olan çalma listesi; **PlaylistSong** parçalarıyla birlikte tek bir aggregate. |
| **ListeningRecord** | Değişmez dinleme kaydı; öneriler ve geçmiş buradan beslenir. |
| **Artist, Album, Genre** | Katalog aggregate'leri. |

### Value object'ler

| Value Object | Kural |
|---|---|
| **Package** | Smart enum: Basic(1), Gold(2), Premium(3), Elit(4). `CanAccess(required)` erişim kuralının tek kaynağı. |
| **Duration** | Pozitif saniye |
| **Email** | Normalize edilmiş (trim + küçük harf), formatı doğrulanmış |
| **AudioFile** | Sadece `.mp3`, path traversal içermeyen göreli yol |

### Domain event'ler

| Event | Tetikleyen | Tepki |
|---|---|---|
| `ListenerRegisteredEvent` | `Listener.Register` | Hoş geldin e-postası |
| `PackageChangedEvent` | `Listener.ChangePackage` | Paket değişikliği e-postası (eski ve yeni paketle) |
| `SongListenedEvent` | `ListeningRecord.Create` | Öneri/istatistik için genişletme noktası |

### Domain service

**PlaybackPolicy:** Listener ile Song'un buluştuğu "bu kullanıcı bu şarkıyı çalabilir mi?" kuralı. Hem çalmada hem çalma listesine eklemede kullanılır.

---

## Öne Çıkan Tasarım Kararları

### 1. İmzalı stream bağlantısı
HTML `<audio>` etiketi `Authorization` header'ı gönderemez; stream endpoint'i JWT ile korunamaz. Çözüm iki adımlı akış:

1. `POST /api/songs/{id}/play` (JWT): Paket kontrolü → dinleme kaydı → `HMAC-SHA256(songId:expires)` ile imzalı, 5 dakikalık URL
2. `GET /api/stream/{id}?expires=…&signature=…` (anonim): İmza ve süre doğrulanır, dosya `enableRangeProcessing` ile akıtılır

İmza `FixedTimeEquals` ile karşılaştırılır (timing attack koruması). MP3'ler `wwwroot` dışında tutulur; dosya yolları kök klasör dışına çıkamaz.

### 2. Yetki token'dan değil, veritabanından
JWT'deki paket claim'i sadece arayüz içindir. Kullanıcı paketini düşürdüğünde eski token geçerliliğini korusa bile, çalma kararı her istekte Listener'ın **güncel** paketiyle verilir. Frontend'deki kilit ikonları da yalnızca bir göstergedir; asıl karar `/play`'de verilir.

### 3. Result pattern + ProblemDetails
Beklenen hatalar (paket yetersiz, kayıt yok, çakışma) exception değil `Result` olarak akar. `ErrorType` → HTTP kodu dönüşümü yalnızca Presentation'da yapılır; Domain HTTP'yi bilmez. Beklenmeyen hatalar `IExceptionHandler` ile yakalanır ve aynı formatta döner.

### 4. Refresh token rotation ve çalıntı tespiti
Refresh token'lar 64 baytlık kriptografik rastgele değerdir; veritabanında yalnızca SHA-256 hash'i saklanır. Her kullanımda yenisiyle değiştirilir. Daha önce kullanılmış bir token tekrar gelirse, kullanıcının bütün oturumları kapatılır.

### 5. Tek transaction'da kayıt
Identity kullanıcısı ve Listener aynı `DbContext` üzerinde, tek transaction içinde oluşturulur. "Giriş yapabilen ama Listener'ı olmayan" yarım kullanıcı oluşamaz.

### 6. CQRS: ayrı okuma ve yazma yolları
- **Command'lar** repository → Domain → `IUnitOfWork` yolunu izler; kurallar Domain'de çalışır.
- **Query'ler** Domain'i atlar; `IApplicationDbContext` üzerinden `AsNoTracking` ve projection ile doğrudan DTO üretir.

### 7. Soft delete ve global query filter
Şarkılar silinmez, yayından kaldırılır. EF global query filter bütün sorgularda kaldırılan şarkıları gizler; çalma listelerinde ise `IgnoreQueryFilters` ile "kullanılamıyor" olarak gösterilir.

### 8. Veritabanı son güvence
Domain ve handler kurallarına ek olarak unique index'ler (tür adı, e-posta, listede şarkı tekrarı) ve `Restrict` / `Cascade` / `SetNull` ilişkileri race condition'lara karşı son savunma hattıdır.

---

## Frontend

### Klasör yapısı

```
melodia-web/src/app
├── core                   Uygulamada bir kez olan şeyler
│   ├── api                Modeller, CatalogService, MeService, PlaylistService, AdminService
│   ├── auth               AuthService, interceptor, guard'lar, paket erişim kuralı
│   ├── http               ProblemDetails yorumlama
│   ├── player             PlayerService (<audio> + signal state)
│   └── theme              PrimeNG preset (Obsidian Emerald)
├── layout                 Shell (sidebar + üst bar), PlayerBar, UpgradeDialog
├── shared                 Birden çok özellikte kullanılan parçalar
│   ├── ui                 SongList, PackageBadge, Cover
│   ├── pipes              DurationPipe
│   └── utils              cover-art, parseUtc
└── features               Sayfalar
    ├── auth               Giriş, kayıt, ortak AuthShell
    ├── home | discover | history | packages | playlists
    └── admin              Layout + şarkı, sanatçı, albüm, tür sayfaları (ayrı lazy paket)
```

### Öne çıkanlar

**1. Signals ile tek doğruluk kaynağı**
Servisler state'i `private signal` olarak tutar, dışarıya `asReadonly()` ile açar. Sayfalar durumu yalnızca servis metotlarıyla değiştirebilir. Token yenilenince `user` → `isUnlocked` → `lockedCount` zinciri `computed`'lar sayesinde kendiliğinden güncellenir; paket yükseltilince kilitler, rozet ve sayaçlar sayfa yenilenmeden değişir.

**2. JWT interceptor ve tek seferlik refresh**
Her API isteğine `Bearer` eklenir, 401 gelince token yenilenip istek **bir kez** tekrarlanır. Aynı anda birden çok istek 401 alırsa hepsi **tek bir** refresh isteğini paylaşır (`shareReplay`). Aksi halde ikinci istek eski refresh token'la gider ve backend'in çalıntı token tespiti bütün oturumları kapatırdı. Token yalnızca kendi API'mize gönderilir; login/refresh çağrılarında refresh denenmez (sonsuz döngü koruması).

**3. Guard'lar ve lazy loading**
- `authGuard`: giriş yapmamış kullanıcıyı `returnUrl` ile login'e yönlendirir. Login sonrası dönüş adresi **open redirect**'e karşı doğrulanır.
- `guestGuard`: giriş yapmış kullanıcıyı login/kayıt sayfalarından uzak tutar.
- `adminGuard` (`canMatch`) + `loadChildren`: admin paneli ayrı bir pakettir ve admin olmayan kullanıcının tarayıcısına **hiç indirilmez**.

**4. Player ve 403 akışı**
`PlayerService` tek bir `<audio>` nesnesini yönetir; `timeupdate`, `play`, `pause` olayları signal'lere yazılır, arayüz bunlardan türer. Çalma isteği 403 dönerse şarkı `blocked` signal'ine konur, `UpgradeDialog` buna bağlı olarak açılır. Paket değişince sırasıyla `PUT /me/package` → `refresh` → `GET /me` çalışır ve şarkı otomatik başlar. Hızlı tıklamalarda önceki istek iptal edilir (race condition koruması).

**5. RxJS desenleri**
- **Arama:** `debounceTime` + `distinctUntilChanged` + `switchMap`: yazarken her harfte istek atılmaz, eski cevaplar yenilerin üstüne yazılmaz.
- **Filtre değişince sayfa 1'e dönülür**; PrimeNG'nin 0 tabanlı sayfalaması backend'in 1 tabanlı sayfalamasına çevrilir.
- Abonelikler `takeUntilDestroyed` ve `toSignal` ile yaşam döngüsüne bağlıdır.

**6. Hata yönetimi**
`toProblem()` her hatayı tek bir şekle çevirir: ağ hatası (status 0), rate limit (429) ve backend ProblemDetails. Validation hataları (`errors`) ilgili form alanının altında, diğerleri bildirim ya da form üstü uyarı olarak gösterilir.

**7. Erişilebilirlik ve detaylar**
Semantik HTML (`ol`, `time`, `role="radiogroup"`), `aria-label`'lar, `:focus-visible` odak halkası, `prefers-reduced-motion` desteği, Türkçe locale (`tr`) ve Türkçe büyük harf dönüşümü (`toLocaleUpperCase('tr-TR')`).

---

## API Endpoint'leri

**Yetki:** 🌐 Anonim · 🔒 Giriş gerekli · 👑 Admin

### Auth
| Yöntem | Endpoint | Yetki | Açıklama |
|---|---|---|---|
| POST | `/api/auth/register` | 🌐 | Kayıt (Basic paketle başlar) |
| POST | `/api/auth/login` | 🌐 | Access + refresh token |
| POST | `/api/auth/refresh` | 🌐 | Token yenileme (rotation) |

### Me
| Yöntem | Endpoint | Yetki | Açıklama |
|---|---|---|---|
| GET | `/api/me` | 🔒 | Profil |
| PUT | `/api/me/package` | 🔒 | Paket değiştirme |
| GET | `/api/me/playlists` | 🔒 | Çalma listelerim |
| GET | `/api/me/recently-played` | 🔒 | Son dinlenenler |
| GET | `/api/me/recommendations` | 🔒 | Kişisel öneriler |

### Songs & Stream
| Yöntem | Endpoint | Yetki | Açıklama |
|---|---|---|---|
| GET | `/api/songs` | 🌐 | Liste (arama, sanatçı/tür/albüm filtresi, sayfalama) |
| GET | `/api/songs/{id}` | 🌐 | Detay |
| POST | `/api/songs/{id}/play` | 🔒 | Paket kontrolü + imzalı stream URL |
| GET | `/api/stream/{id}` | 🌐 (imzalı) | MP3 akışı |
| POST | `/api/songs` | 👑 | Şarkı yükleme (multipart, maks. 20 MB) |
| PUT | `/api/songs/{id}` | 👑 | Ad ve paket güncelleme |
| DELETE | `/api/songs/{id}` | 👑 | Yayından kaldırma (soft delete) |
| PUT | `/api/songs/{id}/album` | 👑 | Albüme bağlama |
| DELETE | `/api/songs/{id}/album` | 👑 | Albümden çıkarma |

### Playlists
| Yöntem | Endpoint | Yetki | Açıklama |
|---|---|---|---|
| POST | `/api/playlists` | 🔒 | Oluşturma |
| GET | `/api/playlists/{id}` | 🔒 | Detay (sadece sahibi) |
| PUT | `/api/playlists/{id}` | 🔒 | Yeniden adlandırma |
| DELETE | `/api/playlists/{id}` | 🔒 | Silme |
| POST | `/api/playlists/{id}/songs` | 🔒 | Şarkı ekleme |
| DELETE | `/api/playlists/{id}/songs/{songId}` | 🔒 | Şarkı çıkarma |

### Katalog: Artists · Albums · Genres
| Yöntem | Endpoint | Yetki |
|---|---|---|
| GET | `/api/artists`, `/api/artists/{id}` | 🌐 |
| GET | `/api/albums`, `/api/albums/{id}` | 🌐 |
| GET | `/api/genres` | 🌐 |
| POST / PUT / DELETE | `/api/artists`, `/api/albums`, `/api/genres` | 👑 |

Bütün endpoint'ler geliştirme ortamında `/scalar/v1` adresinden incelenip denenebilir.

---

## Hata Formatı

Bütün hatalar [RFC 7807 ProblemDetails](https://www.rfc-editor.org/rfc/rfc7807) formatındadır. `errorCode` istemcinin davranış seçmesi, `traceId` loglarda istek takibi içindir.

**Paket yetersiz (403):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.4",
  "title": "Forbidden",
  "status": 403,
  "detail": "Bu şarkıyı dinlemek için paketiniz yeterli değil.",
  "errorCode": "Song.PackageNotSufficient",
  "traceId": "00-35fc81df7fe5037e7cc56c73c534d4d7-10b61e7f29a1b339-00"
}
```

**Doğrulama hatası (400):**
```json
{
  "status": 400,
  "detail": "Bir veya daha fazla doğrulama hatası oluştu.",
  "errorCode": "Validation.General",
  "errors": {
    "Title": ["Şarkı adı boş olamaz."],
    "DurationSeconds": ["Süre sıfırdan büyük olmalıdır."]
  }
}
```

| ErrorType | HTTP |
|---|---|
| Validation | 400 |
| Unauthorized | 401 |
| Forbidden | 403 |
| NotFound | 404 |
| Conflict | 409 |
| Beklenmeyen hata | 500 |

---

## Kurulum

### Gereksinimler
- .NET 9 SDK
- SQL Server (LocalDB, Express veya tam sürüm)
- Node.js 20+ (Angular 21 için; geliştirme Node 24 ile yapıldı)
- (Opsiyonel) Lokal SMTP test sunucusu: [Mailpit](https://mailpit.axllent.org/) veya smtp4dev

### 1. Projeyi klonla
```bash
git clone https://github.com/UmutCan37/Melodia.git
cd Melodia
```

### 2. Backend

**Bağlantı dizesi:** `Melodia.Api/appsettings.json` içindeki `ConnectionStrings:Database` değerini kendi SQL Server'ına göre düzenle.

**Gizli ayarlar (User Secrets):** Gizli anahtarlar repoda tutulmaz. `Melodia.Api` klasöründe:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<en az 32 karakterlik rastgele anahtar>"
dotnet user-secrets set "Storage:StreamSigningKey" "<JWT anahtarından farklı, en az 32 karakter>"
dotnet user-secrets set "Admin:Email" "admin@melodia.local"
dotnet user-secrets set "Admin:Password" "Admin12345"
```

Ayarlar eksik veya hatalıysa uygulama açılışta açıklayıcı bir mesajla durur (`ValidateOnStart`).

**HTTPS sertifikası** (tarayıcının localhost'a güvenmesi için, bir kez):
```bash
dotnet dev-certs https --trust
```

**Çalıştır:**
```bash
dotnet run --project src/Melodia.Api
```

Geliştirme ortamında ilk açılışta otomatik olarak:
- Migration'lar uygulanır
- `Admin` rolü, admin kullanıcı ve örnek katalog (5 tür, 6 sanatçı, 5 albüm, 16 şarkı) oluşturulur
- Serilog `Logs` tablosunu oluşturur

API dokümantasyonu: `https://localhost:7140/scalar/v1`

### 3. Örnek ses dosyaları

`appsettings.json` → `Storage:AudioRootPath` (varsayılan `C:\MelodiaData\audio`). Ses dosyaları bu klasörde, `wwwroot` dışında tutulur.

1. `samples/audio/songs` klasörünü `C:\MelodiaData\audio\songs` olarak kopyala.
2. `samples/update-song-files.sql` script'ini veritabanında çalıştır. Örnek şarkıları kendi dosyalarına bağlar.

> Örnek dosyalar telifsizdir; her biri katalogdaki süreyle birebir aynı uzunlukta, programatik olarak üretilmiş basit melodilerdir. Gerçek şarkılar admin panelinden yüklenebilir.

### 4. Frontend
```bash
cd melodia-web
npm install
npx ng serve
```

Uygulama: `http://localhost:4200` (backend CORS ayarı bu adrese izin verir).

### Test kullanıcıları

| E-posta | Şifre | Not |
|---|---|---|
| `basic@melodia.local` | `Basic12345` | Basic paketli dinleyici |
| `admin@melodia.local` | User Secrets'taki `Admin:Password` | Admin rolü, katalog yönetimi |

---

## Test

Postman koleksiyonu: [`docs/Melodia.postman_collection.json`](docs/Melodia.postman_collection.json)

| Senaryo | Beklenen |
|---|---|
| Kayıt ve giriş | 201 / 200 + token'lar |
| Token olmadan korumalı endpoint | 401 |
| Normal kullanıcıyla admin endpoint'i | 403 |
| **Basic kullanıcı → Gold şarkı çal** | **403 `Song.PackageNotSufficient`** |
| Basic kullanıcı → Basic şarkı çal | 200 + imzalı `streamUrl` |
| İmzası değiştirilmiş stream bağlantısı | 403 `Stream.InvalidLink` |
| Aynı tür adını iki kez ekleme | 409 `Genre.NameAlreadyExists` |
| Aynı şarkıyı listeye iki kez ekleme | 409 `Playlist.SongAlreadyInPlaylist` |
| Başkasının listesini düzenleme | 403 `Playlist.NotOwner` |
| Geçersiz form verisi | 400 + alan bazlı `errors` |

### Arayüzde uçtan uca senaryo
1. `basic@melodia.local` ile giriş yap; Basic şarkılar açık, diğerleri kilitli.
2. Kilitli bir şarkıda **KİLİDİ AÇ** → Network sekmesinde `play` **403** → yükseltme penceresi açılır.
3. **Elit pakete geç** → `package` 204 → `refresh` 200 → `me` 200 → `play` 200; şarkı çalar, kilitler kalkar, rozet ELİT olur.
4. **Paketler** sayfasından Basic'e geri dön (onaylı); çalan Elit şarkı durur.

---

## Bilinen Sınırlar

- **Senkron e-posta:** Kayıt ve paket değişikliğinde e-posta istek içinde gönderilir; SMTP yavaşsa istek birkaç saniye sürer. Çözüm: Outbox pattern.
- **Ödeme yok:** Paket değişikliği demo amaçlıdır, anında uygulanır.
- **UTC tarihler:** EF Core'un okuduğu `DateTime` değerleri JSON'a `Z` eki olmadan yazılıyor; frontend `parseUtc` ile düzeltiyor. Kalıcı çözüm: UTC value converter veya `DateTimeOffset`.
- **OpenAPI cevap şemaları:** Endpoint'ler `Result` → `IResult` döndüğü için Scalar'da cevap tipleri görünmüyor. Çözüm: `.Produces<T>()` / `.ProducesProblem()`.
- **Token saklama:** Token'lar `localStorage`'da. Daha güvenli seçenek: refresh token'ı httpOnly cookie'de tutmak.
- **Logout:** Çıkışta refresh token sadece istemciden silinir; sunucu tarafında iptal endpoint'i yok.

---

## Yol Haritası

- [x] Angular istemci
- [ ] Domain kuralları için unit testler (`PlaybackPolicy`, `Playlist`, `Package`)
- [ ] Frontend testleri (servisler, interceptor, guard'lar; Vitest)
- [ ] Outbox pattern ile güvenilir domain event / e-posta gönderimi
- [ ] Süresi dolmuş refresh token'ları temizleyen arka plan işi
- [ ] Sanatçı fotoğrafı ve albüm kapağı yükleme
- [ ] Keşfet filtrelerinin URL'de tutulması (`?q=&genre=&page=`)
- [ ] Yeni sürüm yayınlandığında eski lazy chunk hatalarını yakalayıp sayfayı yenileme
- [ ] Logların Seq veya Elasticsearch'e taşınması

---

Geliştiren: [Umut Can Yavru](https://www.umutcanyavru.com/) · [GitHub](https://github.com/UmutCan37)
