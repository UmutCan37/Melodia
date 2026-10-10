# 🎵 Melodia

Abonelik paketine göre şarkı dinleme yetkisi veren bir müzik platformu backend'i.
**Clean Architecture + DDD + CQRS** ile, .NET 9 ve Minimal API üzerine kuruldu.

Kullanıcılar Basic, Gold, Premium veya Elit pakete sahiptir; her şarkı dinlenebilmesi için en az bir paket gerektirir. Basic bir kullanıcı Gold bir şarkıyı çalmaya çalıştığında API **403** döner; yetkiliyse kısa ömürlü, imzalı bir stream bağlantısı üretilir ve MP3 ileri/geri sarma desteğiyle akıtılır.

---

## İçindekiler

- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Mimari](#mimari)
- [Domain Modeli](#domain-modeli)
- [Öne Çıkan Tasarım Kararları](#öne-çıkan-tasarım-kararları)
- [API Endpoint'leri](#api-endpointleri)
- [Hata Formatı](#hata-formatı)
- [Kurulum](#kurulum)
- [Test](#test)
- [Yol Haritası](#yol-haritası)

---

## Özellikler

- **Paket tabanlı erişim:** Basic < Gold < Premium < Elit hiyerarşisi; çalma ve çalma listesine ekleme kuralları Domain'de
- **Güvenli müzik akışı:** JWT gönderemeyen `<audio>` etiketi için HMAC imzalı, 5 dakikalık stream bağlantısı; Range desteği ile ileri/geri sarma
- **Kimlik doğrulama:** JWT access token (15 dk) + hash'lenmiş, **rotation** yapılan refresh token (7 gün), çalıntı token tespiti
- **Katalog yönetimi (Admin):** Tür, sanatçı, albüm ve şarkı CRUD'ları; MP3 yükleme
- **Çalma listeleri:** Oluşturma, şarkı ekleme/çıkarma, sıra yönetimi; sahiplik ve paket kuralları
- **Dinleme geçmişi ve öneriler:** Son dinlenenler; tür/sanatçı zevki ve popülerliğe göre kişisel öneriler
- **Soft delete:** Yayından kaldırılan şarkılar geçmişi ve istatistikleri bozmadan gizlenir
- **Domain event'ler:** Kayıt ve paket değişikliğinde e-posta bildirimi
- **Tutarlı hata yönetimi:** Beklenen hatalar `Result` ile, beklenmeyenler `IExceptionHandler` ile; hepsi RFC 7807 ProblemDetails
- **Yapılandırılmış loglama:** Serilog → Console + SQL Server, TraceId ile istek takibi
- **Güvenlik:** Rate limiting, brute force kilidi, path traversal koruması, timing-safe imza karşılaştırma

---

## Teknolojiler

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
src
├── Melodia.Domain
│   ├── Abstractions      Entity, Result, Error, ValidationError, IDomainEvent, IUnitOfWork
│   ├── Shared            Package, Duration, Email (value object'ler)
│   ├── Songs             Song, AudioFile, SongErrors, ISongRepository
│   ├── Listeners         Listener, event'ler, ListenerErrors, IListenerRepository
│   ├── Playlists         Playlist, PlaylistSong, PlaylistErrors, IPlaylistRepository
│   ├── Artists | Albums | Genres | Listenings
│   └── Services          PlaybackPolicy
├── Melodia.Application
│   ├── Abstractions      Messaging, Data, Authentication, Storage, Notifications
│   ├── Behaviors         LoggingBehavior, ValidationBehavior
│   ├── Common            PagedResponse, ValidationExtensions
│   └── Features          Auth, Songs, Artists, Albums, Genres, Playlists, Listeners, Listenings
├── Melodia.Persistence   AppDbContext, Configurations, Repositories, Migrations, Identity
├── Melodia.Infrastructure Authentication, Storage, Notifications
├── Melodia.Presentation  Endpoints, Extensions
└── Melodia.Api           Program.cs, GlobalExceptionHandler, DatabaseSeeder
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
JWT'deki paket claim'i sadece arayüz içindir. Kullanıcı paketini düşürdüğünde eski token geçerliliğini korusa bile, çalma kararı her istekte Listener'ın **güncel** paketiyle verilir.

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
- (Opsiyonel) Lokal SMTP test sunucusu: [Mailpit](https://mailpit.axllent.org/) veya smtp4dev

### 1. Projeyi klonla
```bash
git clone https://github.com/UmutCan37/Melodia.git
cd Melodia
```

### 2. Bağlantı dizesi
`Melodia.Api/appsettings.json` içindeki `ConnectionStrings:Database` değerini kendi SQL Server'ına göre düzenle.

### 3. Gizli ayarlar (User Secrets)
Gizli anahtarlar repoda tutulmaz. `Melodia.Api` klasöründe:

```bash
dotnet user-secrets set "Jwt:SecretKey" "<en az 32 karakterlik rastgele anahtar>"
dotnet user-secrets set "Storage:StreamSigningKey" "<JWT anahtarından farklı, en az 32 karakter>"
dotnet user-secrets set "Admin:Email" "admin@melodia.local"
dotnet user-secrets set "Admin:Password" "Admin12345"
```

Ayarlar eksik veya hatalıysa uygulama açılışta açıklayıcı bir mesajla durur (`ValidateOnStart`).

### 4. Ses dosyaları klasörü
`appsettings.json` → `Storage:AudioRootPath` (varsayılan `C:\MelodiaData\audio`). Yüklenen MP3'ler bu klasörün altına kaydedilir; klasör `wwwroot` dışında olmalıdır.

### 5. Çalıştır
```bash
dotnet run --project Melodia.Api
```

Geliştirme ortamında ilk açılışta otomatik olarak:
- Migration'lar uygulanır
- `Admin` rolü ve admin kullanıcı oluşturulur
- Serilog `Logs` tablosunu oluşturur

API dokümantasyonu: `https://localhost:<port>/scalar/v1`

---



Kapsanan ana senaryolar:

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

---

## Yol Haritası

- [ ] Angular istemci
- [ ] Domain kuralları için unit testler (`PlaybackPolicy`, `Playlist`, `Package`)
- [ ] Outbox pattern ile güvenilir domain event / e-posta gönderimi
- [ ] Süresi dolmuş refresh token'ları temizleyen arka plan işi
- [ ] Logların Seq veya Elasticsearch'e taşınması

---

Geliştiren: [Umut Can Yavru](https://www.umutcanyavru.com/) · [GitHub](https://github.com/UmutCan37)
