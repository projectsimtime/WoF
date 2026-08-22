# Vertigo Demo — 2 Günlük Geçme Odaklı Revizyon Planı

> Bu bir “projeyi kusursuzlaştırma” listesi değil. Amaç, iki gün içinde ekip geri bildirimini gerçekten anladığını ve doğru yönde düzeltebildiğini kanıtlamak.
>
> Bu dosya kişisel çalışma rehberidir. Göndermeden önce repoda kalmasını isteyip istemediğine karar ver.

---

## 1. Başarı tanımı

Teslim sonunda reviewer şunları kolayca görebilmeli:

| Geri bildirim | Teslimde göstereceğin kanıt |
|---|---|
| SRP FAIL | `GameSession` zone üretmiyor, ekonomi hesaplamıyor ve UI sürmüyor; yalnızca oyun akışını yönetiyor. |
| DIP FAIL | Runtime kodunda `FindObjectOfType` yok; butonlar `GameSession` aramıyor; `GameSession` somut panel referansı tutmuyor. |
| OCP WARN | Zone type ve rarity seçimi hardcoded `if/else` zinciri yerine editlenebilir veri tablosundan geliyor. |
| LSP/ISP WARN | Boş `virtual` metot yok; sahte `Base` adı yok; her buton text ve `GameSession` taşımak zorunda değil. |
| Görsel kalite | 20:9, 16:9 ve 4:3 görüntülerinde wheel büyük ve merkezi; ikonlar stretch değil; HUD parçaları bilinçli yerleşmiş. |
| Akış | Kazanılan slot görünür kalıyor; ödül gösterildikten sonra zone değişiyor; bomba ile lose panel arasında anlaşılır bir geçiş var. |
| Akışkanlık | Spin, sonuç vurgusu, bomba ve kritik panel geçişleri yumuşak. |
| Teslim | Fiziksel cihaz videosu Drive'da oturum kapalıyken oynuyor; APK GitHub Release'e ekli. |

### Değişmez teknik hedefler

- [ ] `FindObjectOfType` sonucu: **0**
- [ ] `GameSession` içindeki panel/controller field sonucu: **0**
- [ ] Button sınıflarındaki `GameSession` field sonucu: **0**
- [ ] Kazanan slot, zone değişmeden önce gözle okunabiliyor
- [ ] Exit yalnızca `AwaitingSpin` durumunda ve safe/super zone'da kullanılabiliyor
- [ ] 20:9, 16:9 ve 4:3 layout'ları kasıtlı görünüyor
- [ ] Drive videosu incognito pencerede oynuyor

`GameSession 110 satır olsun`, `interface sayısı 4 olsun`, `buton sayısı 3 olsun` gibi hedefler **yok**. Bunlar kalite ölçüsü değildir. Önemli olan sorumluluklar ve bağımlılık yönleridir.

---

## 2. İki günlük çalışma kuralları

1. Her büyük bloktan önce çalışan halin commit'ini al.
2. Bir değişiklikten sonra Unity compile hatası varsa yeni işe geçme.
3. Aynı hatada 20 dakikadan fazla takılırsan son çalışan commit'e dön ve daha küçük adım uygula.
4. Görsel polish yaparken oyun kurallarını değiştirme.
5. Refactor sırasında davranışı koru; flow düzeltmesini ayrı commit'te yap.
6. Yeni global event bus, service locator, dependency injection framework veya her state için ayrı class ekleme.
7. Reviewer'a açıklayamayacağın hiçbir abstraction ekleme.

### Bu iki günde özellikle yapma

- Bütün projeyi baştan yazma.
- Bütün prefabları yeniden adlandırma.
- Sırf sayı artsın diye interface ekleme.
- Bütün panellere farklı ve uzun animasyonlar yapma.
- Rarity için subclass hiyerarşisi kurma; farklı olan davranış değil, veridir.
- 10.000 roll ile istatistik testi yazma; yavaş ve flaky olabilir.
- Explicit State pattern class'ları kurma. Küçük bir enum ve açık geçişler yeterli.
- Çalışan ScriptableObject yapısını değiştirme; reviewer bunu zaten beğenmiş.
- `ZoneRuleEditor` gibi çalışan ve değerli editör araçlarını silme.

---

## 3. Hedef mimariyi önce anla

Mevcut problem şu şekildedir:

```text
Butonlar ───────► GameSession ◄────── Paneller
                      │
                      ├── zone üretimi
                      ├── spin hesabı
                      ├── ödül ve ekonomi
                      ├── revive
                      ├── hint
                      └── UI aç/kapat
```

İki gün sonunda hedef:

```text
                         ┌── ZoneWheelBuilder
Button intent ─► GameSession ── RunState
                         └── RunEconomy
                 │
                 │ anlamlı event / sonuç
                 ▼
GameSceneInstaller ─► WheelPresenter ve somut paneller
```

### Sınıfların tek cümlelik görevleri

| Sınıf | Tek görevi |
|---|---|
| `GameSession` | Koşunun hangi durumda olduğunu ve sıradaki geçerli geçişi yönetir. |
| `RunState` | Mevcut zone, kazanılan ödüller ve koşuya ait flag'leri tutar/değiştirir. |
| `RunEconomy` | Currency harcama ve revive maliyetini yönetir. |
| `ZoneWheelBuilder` | Verilen zone için wheel içeriğini üretir. |
| `WheelPresenter` | Wheel'i görsel olarak kurar, döndürür ve sonucu vurgular. |
| `GameSceneInstaller` | Inspector referanslarını alır ve buton–flow–view bağlantılarını kurar. |

`GameSceneInstaller` içinde dokuz somut panel referansı bulunması hata değildir. Composition root'un görevi zaten somut nesneleri tanımak ve bağlamaktır. Hata, oyun kurallarını yöneten `GameSession`'ın bu panelleri tanımasıdır.

### Basit akış modeli

```text
EnteringZone
    ↓
AwaitingSpin
    ↓ Spin tıklandı
Spinning
    ↓ Wheel tween tamamlandı
RevealingResult
    ├── ödül animasyonu tamamlandı → EnteringZone
    └── bomba animasyonu tamamlandı → DeathDecision

DeathDecision
    ├── revive → AwaitingSpin (aynı zone)
    └── give up → Finishing
```

Bu state'ler davranışı koruyan guard'lardır:

- `Spin` yalnızca `AwaitingSpin` durumunda kabul edilir.
- `Exit` yalnızca `AwaitingSpin` + safe/super zone iken kabul edilir.
- `RevealingResult` sırasında butonlar kapalı kalır.
- `DeathDecision` sırasında yalnızca revive/give-up kararları kabul edilir.

---

# DAY 1 — Mimari ve doğru oyun akışı

Toplam hedef: yaklaşık 8 saat. Her blok sonunda Unity'de kısa smoke test yap.

## 0. Çalışan başlangıç noktası — 30 dakika

- [ ] Ayrı bir revision branch aç.
- [ ] Mevcut çalışan hali commit'le.
- [ ] Unity'yi aç; Console'un temiz olduğunu doğrula.
- [ ] Şu akışları bir kez oyna ve mevcut davranışı not et:
  - normal reward
  - bomb + give up
  - bomb + paid revive
  - safe zone exit
- [ ] Mevcut Android build'in açıldığını doğrula.

### Neden?

Refactor sırasında bir davranış bozulduğunda “önceden de böyle miydi?” sorusunun cevabı olmalı. Çalışan commit senin emniyet kemerindir.

### Bitti ölçütü

- Başlangıç sürümü geri döndürülebilir.
- Console temiz.
- Dört kritik akışın mevcut hali biliniyor.

---

## 1. Akış sözleşmesini yaz — 30 dakika

Kod yazmadan önce yukarıdaki state listesini kendi kelimelerinle bir kâğıda geçir. Her public aksiyonun hangi state'te geçerli olduğunu yaz.

Örnek:

| Kullanıcı aksiyonu | Kabul edilen state | Sonraki state |
|---|---|---|
| Spin | `AwaitingSpin` | `Spinning` |
| Wheel tamamlandı | `Spinning` | `RevealingResult` |
| Reward sunumu tamamlandı | `RevealingResult` | `EnteringZone` |
| Bomb sunumu tamamlandı | `RevealingResult` | `DeathDecision` |
| Paid revive | `DeathDecision` + yeterli para | `AwaitingSpin` |
| Give up | `DeathDecision` | `Finishing` |

### Neden?

Mevcut kodda `_isWheelSpinning` yalnızca bir bool. Tween bittiği anda `false` oluyor; fakat oyuncu sonucu hâlâ görmedi. “Dönmüyor” ile “yeni aksiyon kabul edilebilir” aynı şey değildir. State modeli bu farkı görünür yapar.

### Bitti ölçütü

- Her butonun hangi state'te çalıştığını söyleyebiliyorsun.
- Result reveal ayrı bir aşama.
- Exit kuralı `_isWheelSpinning` yerine akış durumuna bağlı.

---

## 2. DIP: Sahne aramasını ve button bağımlılığını kaldır — 2 saat 30 dakika

Bu blok reviewer'ın en net `FAIL` maddesini kapatır.

### 2.1 `GameSceneInstaller` oluştur

- [ ] Sahnedeki `GameSession`, butonlar, `WheelPresenter`/`WheelParent`, panel controller'lar ve `AdPanelController` referanslarını `[SerializeField]` ile installer'a ver.
- [ ] Referansları Inspector'dan sürükle.
- [ ] Installer yalnızca `Awake`/`OnDestroy` içinde bağlantı kursun ve kaldırsın.
- [ ] Installer içinde zone kuralı, revive fiyatı veya “bombaysa ne olur?” kararı bulunmasın.

Bağlantı örneği, kod değil sorumluluk olarak:

```text
SpinButton click       → GameSession spin isteği
GameSession zone event → CurrentZonePanel güncelle
GameSession bomb event → LosePanel açılış sunumu
Currency event         → CurrencyPanel güncelle
```

### 2.2 Button base class'ını incelt

- [ ] `ButtonController` yalnızca kendi `Button` component'ini otomatik bulsun.
- [ ] Click davranışı dışarıdan `Bind(Action)` benzeri bir bağlantıyla verilsin.
- [ ] Kaldırırken yalnızca kendi eklediği callback'i çıkarsın; `RemoveAllListeners` kullanmasın.
- [ ] `_buttonText` her butona zorunlu olmasın. Text gerekiyorsa opsiyonel component/presenter olsun.
- [ ] `protected GameSession _gameSession` tamamen kalksın.
- [ ] Yalnızca bir `GameSession` metodunu çağıran button subclass'larını generic button ile değiştir.
- [ ] Gerçekten ekstra davranışı olan `SpinButton` ve revive-cost gösteren button özel kalabilir.

#### Öğrenmen gereken fikir

Butonun görevi “oyunu bilmek” değildir. Buton yalnızca tıklandığını bildirir. Hangi işin yapılacağına installer karar vererek bağlantıyı kurar.

Önce:

```text
ExitButton, GameSession'ı arar ve onun metodunu seçer.
```

Sonra:

```text
Installer, ExitButton tıklamasını exit isteğine bağlar.
```

Bu küçük yön değişikliği DIP'in özüdür: bağımlılığı kullanan component gidip bulmaz; oluşturma/bağlama noktasından alır.

### 2.3 `OnValidate` kuralını doğru uygula

Şunları koru:

- `GetComponent<Button>()`
- `GetComponentInChildren<...>()` — gerçekten kendi prefab hierarchy'sindeki yerel referanslar için

Şunları kaldır:

- `FindObjectOfType<GameSession>()`
- `FindObjectOfType<AdPanelController>()`
- `GameSession` içindeki dokuz panel araması

Kural:

> Kendi component/hierarchy'mi bulmak yerel kurulumdur. Bütün sahneyi tarayıp başka bir sistemi bulmak gizli bağımlılıktır.

### 2.4 Panel bağlantılarını ters çevir

- [ ] `GameSession` içinden dokuz somut panel field'ını çıkar.
- [ ] `_wheelParent` gibi görsel wheel referansını da `WheelPresenter` tarafına taşı.
- [ ] `GameSession` UI metodu çağırmak yerine anlamlı sonuç/event bildirsin.
- [ ] Installer bu bildirimleri ilgili panel metoduna bağlasın.

İhtiyaç duyacağın bildirimlerin anlamı yaklaşık şöyle olabilir:

- zone değişti
- currency değişti
- reward kazanıldı
- bomb sonucu açıklandı
- exit uygunluğu değişti
- run bitti / summary istendi

Event sayısını artırmak amaç değildir. Aynı UI sonucunu kullanan verileri tek bir küçük result/snapshot içinde taşımak daha temiz olabilir.

#### Yanlış event örneği

`OpenLosePanel` oyun çekirdeğinin UI'ı bildiğini gösterir.

#### Daha iyi event örneği

`BombRevealed` oyunda gerçekleşen şeyi anlatır. Lose panelin açılması sunum tarafının kararıdır.

Wheel tween'inin tamamlanması için de benzer bir el sıkışma kullan:

```text
GameSession: spin planını üretir ve Spinning durumuna geçer
WheelPresenter: planı görsel olarak oynatır
WheelPresenter completion: GameSession'a “spin sunumu bitti” der
GameSession: sonucu çözer ve RevealingResult durumuna geçer
```

Burada installer yalnızca iki tarafı bağlar; sonucu veya sonraki zone'u kendisi hesaplamaz.

### 2.5 Reklam ödülü bağımlılığını düzelt

- [ ] `LosePanelSeeAdButton` ve `ExitPanelSeeAdButton`, `AdPanelController` aramasın.
- [ ] Reklam reward nesnelerini butonlar oluşturmasın; installer oluştursun.
- [ ] `AdRewardBase` doğrudan `GameSession` tutmasın.
- [ ] Revive reward yalnızca “ad revive uygula” yeteneğine; hint reward yalnızca “hint aç” yeteneğine bağımlı olsun.

Burada iki adet tek-metotlu dar interface anlamlı olabilir. Interface sayısını artırmak için değil, her reward'ın ihtiyacı olmayan bütün `GameSession` API'sini görmesini engellemek için eklenir.

### Kontrol komutları

PowerShell/terminalde:

```text
rg -n "FindObjectOfType" Assets/_WoF/Scripts/Runtime
rg -n "GameSession" Assets/_WoF/Scripts/Runtime --glob "*Button*.cs"
rg -n "PanelController _" Assets/_WoF/Scripts/Runtime/GameSession.cs
```

İlk ve üçüncü komut sonuç vermemeli. İkinci komutta yalnızca gerçekten özel ve açıklanabilir bir kullanım kaldıysa tekrar değerlendir; hedef button tarafında da sıfırdır.

### Bitti ölçütü

- Runtime'da `FindObjectOfType` yok.
- Button base class'ında `GameSession` ve zorunlu text yok.
- `GameSession` hiçbir paneli tanımıyor.
- Reklam reward'ları somut `GameSession`'a bağlı değil.
- Inspector'da eksik script/reference yok.
- Normal spin ve bomb akışı hâlâ çalışıyor.

### Zaman aşımı planı

Generic button'a prefab migration yetişmezse eski subclass dosyalarını hemen silme. Önce hepsini inceltilmiş base üzerinden dışarıdan bind edilebilir hale getir; prefabları tek tek migrate edip en son gereksiz subclass'ları sil. Eksik script bırakmaktan daha iyidir.

---

## 3. SRP: `GameSession` içinden üç gerçek sorumluluk çıkar — 2 saat

Beş–on yeni class çıkarma. Aşağıdaki üç sınır yeterince güçlü ve açıklanabilir.

### 3.1 `ZoneWheelBuilder`

Taşınacak sorumluluklar:

- Zone type → `ZoneTypeData` seçimi
- override wheel kontrolü
- wheel reward listesini oluşturma
- bomb/special slot yerleştirme
- super-zone reward seçimi
- rarity picker kullanımı

Girdi örneği:

```text
zone index + zone rules + zone type table + reward data
```

Çıktı örneği:

```text
wheel type + reward list + reserved slot index
```

Builder, `WheelParent`, panel veya button bilmemeli. Yalnızca veri üretmeli.

### 3.2 `RunEconomy`

Taşınacak sorumluluklar:

- Başlangıç currency'si
- Mevcut currency
- Revive fiyatını sorma
- Revive için ödeme yapılabilir mi?
- Ödeme sonrası bir sonraki revive fiyatı
- Yeni run'da ekonomi reset'i

Örnek karar:

```text
GameSession: “paid revive istendi”
RunEconomy: “başarılı, kalan para 425, sonraki fiyat 125”
GameSession: akışı Reviving durumuna geçirir
UI: gelen sonucu gösterir
```

`RunEconomy` lose paneli açmaz veya text yazmaz.

### 3.3 `RunState`

Tutacağı koşu verileri:

- current zone
- earned rewards/container
- current/next wheel sonucu için gereken küçük flag'ler
- mevcut flow state
- bombadan sonra aynı zone'da devam bilgisi

Mümkünse davranış da verinin yanında olsun: `ResetRun`, `AdvanceZone`, `AddReward`, `MarkRevived` gibi. Sadece public field dolu pasif bir çanta oluşturma.

### `GameSession`ta ne kalır?

- New run başlatma
- Zone'a giriş sırası
- Spin isteğini kabul etme
- Spin sonucu sonrası doğru state'e geçme
- Revive/give-up/exit akışını koordine etme
- Servislerden gelen sonucu anlamlı event olarak yayınlama

Kendine şu soruyu sor:

> `GameSession` bir değeri hesaplıyor mu, yoksa doğru servise sorup akış kararı mı veriyor?

Hesaplıyorsa büyük ihtimalle yanlış sorumluluk hâlâ içeridedir.

### Bitti ölçütü

- `GameSession` zone içeriği oluşturmuyor.
- `GameSession` revive maliyeti/currency hesabı yapmıyor.
- Run state tek yerde tutuluyor.
- `GameSession` herhangi bir view/panel çağırmıyor.
- Her çıkarılan sınıf tek cümleyle, “ve” demeden açıklanabiliyor.
- Satır sayısı ne çıkarsa çıksın, dependency listesi belirgin şekilde küçülmüş.

---

## 4. OCP + LSP/ISP hızlı düzeltmeleri — 45 dakika

### 4.1 Zone type lookup

- [ ] Üç ayrı `_normal/_safe/_superZoneTypeData` field yerine `(EZoneType, ZoneTypeData)` eşleşmeleri içeren serialized bir liste kullan.
- [ ] Çalışma başında bu listeyi dictionary/lookup haline getir.
- [ ] Duplicate veya eksik zone type için açık validation/error ver.
- [ ] `GetZoneTypeData` içindeki zone-type `if` zincirini kaldır.

Bu, “davranış polymorphism'i” değildir. Burada farklı olan yalnızca veridir; tablo doğru araçtır.

### 4.2 Rarity weight table

- [ ] `LegendaryChance`, `EpicChance`, `RareChance` şeklindeki ayrı field'ları ordered weight listesine dönüştür.
- [ ] `Casual` dahil bütün rarity'ler tabloda görünür olsun.
- [ ] Sadece mevcut wheel content'inde bulunan rarity'leri değerlendir.
- [ ] Toplamın 100 olmaması ve bir rarity'nin wheel'de bulunmaması için net kural belirle.
- [ ] Sıralı `if/else` yerine cumulative roll kullan.

Örnek veri:

| Rarity | Weight |
|---|---:|
| Casual | 70 |
| Rare | 20 |
| Epic | 9 |
| Legendary | 1 |

Önerilen basit fallback: Wheel'de bulunmayan rarity'nin payını `Casual`a bırak. Alternatif olarak kalanları normalize edebilirsin; hangisini seçersen test ve dokümanda açıkça belirt. Gizli davranış bırakma.

### 4.3 LSP/ISP temizliği

- [ ] Generic bind yaklaşımına geçtiysen boş `virtual OnButtonClicked()` tamamen ortadan kalkmalı.
- [ ] Inheritance'ı koruyorsan base class ve click metodu `abstract` olmalı; unutulan override compile-time hata vermeli.
- [ ] `ZoneViewBase` alt sınıfsız olduğu için `ZoneView` olarak yeniden adlandırılmalı.
- [ ] Kullanılmayan `using` satırlarını temizle.
- [ ] `_buttonText` bütün butonlara zorunlu bağımlılık olmamalı.

### Bitti ölçütü

- Reviewer'ın verdiği iki conditional zinciri yok.
- Yeni rarity yalnızca enum + weight verisi/content eklenerek destekleniyor; merkezi seçim algoritması değişmiyor.
- Boş override sözleşmesi yok.
- “Base” isimli fakat alt sınıfsız component yok.

---

## 5. Spin sonucu ve ölüm akışını düzelt — 1 saat 30 dakika

Bu blok feedback'in “spin geldiğinde olan yer” ve “ölüm sonrası akış” bölümünü doğrudan çözer.

### 5.1 Normal reward sonucu

Mevcut hata:

```text
Tween tamamlanır → reward eklenir → GoToNextZone → wheel aynı frame'de resetlenir
```

Yeni sıra:

```text
Tween tamamlanır
→ state = RevealingResult
→ kazanan slot highlight edilir
→ wheel son açısında tutulur
→ reward miktarı görünür
→ reward rail animasyonu tamamlanır
→ ancak şimdi next zone'a geçilir
```

- [ ] `SetWheelSpinning(false)` ile “oyun aksiyon kabul ediyor” kavramını ayır.
- [ ] Sonucu en az yaklaşık 0.35–0.50 saniye okunabilir tut.
- [ ] Kazanan slotta kısa pulse/glow/outline kullan.
- [ ] Reward rail'e miktarı animasyonun doğru anında ekle.
- [ ] Yeni zone'a geçmeden wheel rotasyonunu sıfırlama.

### 5.2 Bomb sonucu

Yeni sıra örneği:

```text
Bomb slotunda dur
→ slot vurgusu / flash
→ kısa wheel shake + ses/haptic
→ 0.15–0.25 saniye bekle
→ lose blocker fade-in
→ lose content scale/slide-in
→ state = DeathDecision
```

- [ ] Lose panel spin tamamlandığı aynı frame'de açılmasın.
- [ ] Bombanın seçildiği açıkça görülsün.
- [ ] Revive aynı zone'a dönsün ve bomb slotu pasif/işaretli kalsın.
- [ ] Give up bilinçli şekilde summary veya new-run akışına gitsin.
- [ ] Tween/state çıkışında çalışan animasyonları kill/cancel et; çift tıklama eski callback'i tetiklemesin.

### Bitti ölçütü

- Normal result ekran kaydında pause yapmadan anlaşılabiliyor.
- Bombanın geldiği slot lose panelden önce görülüyor.
- Reveal sırasında Spin/Exit tıklanamıyor.
- Revive sonrasında aynı zone bozulmadan devam ediyor.
- Hızlı çift tıklama iki spin başlatmıyor.

---

## DAY 1 kapanış kontrolü — 45 dakika

- [ ] Unity Console temiz.
- [ ] Normal reward oynandı.
- [ ] Bomb + paid revive oynandı.
- [ ] Bomb + ad revive oynandı.
- [ ] Give up oynandı.
- [ ] Safe zone exit oynandı.
- [ ] `FindObjectOfType` araması sıfır.
- [ ] Scene/prefab üzerinde Missing Script yok.
- [ ] Çalışan hali commit'lendi.

Day 1 sonunda mimari compile etmiyor veya temel oyun çalışmıyorsa Day 2 layout'a geçme. Önce çalışan küçük sürüme dön.

---

# DAY 2 — Layout, hareket, doğrulama ve teslim

## 6. Responsive layout'u düzelt — 2 saat 30 dakika

Amaç bütün UI'ı yeniden tasarlamak değil; örnekteki görsel hiyerarşiyi kurmak:

```text
Full-bleed background
└── Safe Area
    ├── Top HUD: zone + next safe/super + currency + exit
    ├── Main Stage: büyük, merkezi wheel
    └── Reward rail / sonuç bilgisi
```

### 6.1 Orientation

- [ ] Proje landscape kalacaksa AutoRotation'ı kapat ve tek landscape yönünü seç.
- [ ] Video boyunca orientation değişmemeli.
- [ ] Canvas Scaler `Expand` kalmalı; brief bunu istiyor.

### 6.2 Wheel sahnesi

- [ ] Sabit `485×485` wheel yaklaşımını bırak.
- [ ] `MainStage` kullanılabilir alanı stretch anchor ile doldursun.
- [ ] İçerideki `WheelBounds` kare aspect'i koruyarak parent'a sığsın.
- [ ] Wheel görseli bu kareyi doldursun; transform scale `1,1,1` kalsın.
- [ ] Hedef olarak wheel kullanılabilir ekran yüksekliğinin yaklaşık `%65–75`ini kaplasın.
- [ ] Animasyon uygulanan wheel child'ı ile layout yöneten parent ayrı olsun.

Neden iki obje?

```text
WheelBounds: anchor/layout/aspect sorumluluğu — hareket etmez
WheelContent: rotate/punch/shake sorumluluğu — animasyon alır
```

Böylece tween layout sisteminin hesapladığı root transformu bozmaz.

### 6.3 HUD ve reward rail

- [ ] Current zone, next safe/super, currency ve exit üstte tek bir düzen hissi versin.
- [ ] Reward rail dev boş bir sol kolon gibi görünmesin.
- [ ] 20:9'da kompakt yan rail kullanılabilir.
- [ ] 4:3'te aynı rail merkezi alanı sıkıştırıyorsa daha dar veya yatay alt düzen kullan.
- [ ] Layout Group/Content Size Fitter çatışması oluşturma; aynı ekseni iki component yönetmesin.

### 6.4 Görsel stretch kontrolü

- [ ] İkonlarda `Preserve Aspect` veya kare aspect parent kullan.
- [ ] Frame/button background sprite'larında 9-slice kullan.
- [ ] RectTransform scale x ve y eşit olsun; boyutu width/height/anchor ile değiştir.
- [ ] Background full-bleed olabilir; fakat karakter/ikon gibi oranı önemli sprite'ları non-uniform esnetme.

Not: Mevcut background gerçekten render oluyor; problem “kapalı olması” değil, düz ve boş görünmesi. Olmayan bir bug'ı çözmeye çalışma. Wheel ve HUD hiyerarşisini güçlendirmek daha değerlidir.

### 6.5 Üç oranı birlikte test et

Her değişiklikten sonra Game View'da:

- [ ] 2400×1080 — 20:9
- [ ] 1920×1080 — 16:9
- [ ] 1440×1080 — 4:3

Her oran için sor:

1. Wheel ilk bakışta ana obje mi?
2. Metinler kesiliyor mu?
3. Reward rail anlamsız boşluk üretiyor mu?
4. UI safe area/kenarlara fazla yakın mı?
5. Görsellerin oranı bozuluyor mu?

### Bitti ölçütü

- Üç oran da aynı tasarım ailesine ait fakat kendi alanını bilinçli kullanıyor.
- 4:3 yalnızca “20:9 + ekstra boşluk” gibi görünmüyor.
- Wheel büyük, kare ve merkezi.
- Stretch edilmiş ikon yok.
- Yeni 20:9, 16:9 ve 4:3 screenshot'ları `Docs` altında güncel.

---

## 7. Akışkanlık polish'i — 1 saat 30 dakika

Öncelik sırası:

1. Wheel dönüşü ve duruşu
2. Winning-slot reveal
3. Bomb → lose panel
4. Reward summary/exit panel
5. Diğer küçük paneller

### 7.1 Wheel

- [ ] Slot açısı 45° iken mevcut ±20° offset kenara fazla yakın. Görsel olarak net bir güvenli bant seç; örneğin merkez çevresinde yaklaşık ±8–12°.
- [ ] “Near miss” efekti sonucu belirsizleştiriyorsa azalt veya bu demo için kapat.
- [ ] Başlangıçta 0.08–0.12 saniyelik küçük reverse anticipation deneyebilirsin.
- [ ] Ana spin için yaklaşık 2.4–2.9 saniye aralığını cihazda test et.
- [ ] Tek easing adına takılma; hedef güçlü başlangıç, okunabilir yavaşlama ve net settle.
- [ ] Pointer tick/ses ekleyeceksen hafif tut.

### 7.2 Paneller

- [ ] Bütün panelleri tek generic scale tween'e zorlamak yerine önce lose paneli düzelt.
- [ ] Lose panelin mevcut merkezî bomb-card düzeni referansa zaten yakın; sıfırdan tasarlamak yerine geçişini, spacing'ini ve okunurluğunu düzelt.
- [ ] Full-screen blocker `CanvasGroup` ile fade olabilir.
- [ ] Panelin `ui_group_*_content` child'ı scale/slide alabilir.
- [ ] Root RectTransform anchor/layout için sabit kalmalı.
- [ ] Exit/summary için kısa 0.20–0.30 saniyelik fade/slide yeterli.

### 7.3 Tween güvenliği

- [ ] Panel/wheel yeniden açılırken eski tween'i kill et.
- [ ] `OnDisable` veya yeni sequence başlangıcında eski callback'in kalmadığını doğrula.
- [ ] Animasyon tamamlanmadan state advance etme.
- [ ] TimeScale değişirse hangi tween'lerin etkilenmesi gerektiğini bilinçli seç.

### Bitti ölçütü

- Wheel'in durduğu slot tartışmasız okunuyor.
- Panel “pop” hissi kritik akışlarda yok.
- Aynı butona hızlı basmak duplicate callback üretmiyor.
- Animasyonlar layout root'larını bozmuyor.

---

## 8. Brief compliance sweep — 30 dakika

- [ ] Canvas mode: Expand
- [ ] Text: TextMeshPro
- [ ] Değişen UI elementlerinde `_value` isimlendirmesi
- [ ] Gereksiz RaycastTarget kapalı
- [ ] Inspector Button OnClick listeleri boş; bağlantılar koddan
- [ ] Button'ın kendi `Button` referansı `OnValidate/GetComponent` ile otomatik
- [ ] Sliced olması gereken frame/background sprite'ları sliced
- [ ] Root transform üzerinde UI animasyonu yok
- [ ] Anchors ve pivots üç aspect ratio'da doğru
- [ ] Orientation kilitli

RaycastTarget sayısını körlemesine sıfırlama. Button image, full-screen blocker ve scroll viewport gibi input alanları bunu gerçekten kullanır. Önce dekoratif text/icon/frame olup olmadığına bak.

Prefab isimlerini sırf convention için topluca değiştirme. İki günlük sürede kullanıcıya görünmeyen rename operasyonu, GUID/reference kontrolü ve hata riski yaratır. Yalnızca gerçekten yanlış/anlaşılmaz yeni objeleri doğru isimlendir.

---

## 9. Az ama anlamlı test — 45 dakika

Hedef 12 test değil; reviewer'a kuralların doğrulanabilir olduğunu gösterecek 4–6 test.

- [ ] EditMode test asmdef oluştur ve `WoF.Runtime` referansı ver.
- [ ] `ZoneCalculator`: zone 1 normal, 5 safe, 30 super; 30'da super önceliği açık olsun.
- [ ] `ReviveCostCalculator/RunEconomy`: başlangıç fiyatı, ödeme, sonraki fiyat, reset.
- [ ] `RunState`: spin/reveal sırasında exit kapalı; safe/super + awaiting durumunda açık.
- [ ] `RarityPicker`: sabit random değeriyle boundary'ler ve unavailable-rarity fallback'i.
- [ ] Mümkünse spin slot-index → hedef açı eşleşmesi için deterministik bir test.

### Test edilebilirlik dersi

Random davranışı test etmek için 10.000 kez çalıştırmak yerine random kaynağını dışarıdan verilebilir yap. Test “42 döndür” dediğinde hangi rarity/slot seçileceği kesin olsun. Bu abstraction'ın gerçek gerekçesi vardır: deterministik test.

### Zaman aşımı planı

Test altyapısı 20 dakikada ayağa kalkmıyorsa önce `ZoneCalculator` ve revive cost için en az iki saf test bırak. Build/video süresini test sayısı uğruna kaybetme.

---

## 10. Final QA — 45 dakika

Aşağıdaki senaryoların tamamını cihaz build'inde veya son build'e çok yakın ortamda oyna:

- [ ] Normal spin → slot highlight → reward rail → next zone
- [ ] Bomb → effect → lose panel → give up
- [ ] Bomb → paid revive → aynı zone'da devam
- [ ] Yetersiz currency ile paid revive
- [ ] Bomb → ad revive → aynı zone'da devam
- [ ] Safe zone'da exit butonu açık
- [ ] Spin ve reveal sırasında exit kapalı
- [ ] Super zone'da bomb yok ve özel ödül var
- [ ] New run'da currency/revive/reward state reset
- [ ] 20:9, 16:9 ve 4:3'te kesilen UI yok

Bulduğun bug'ı düzeltirken yeni feature ekleme.

---

## 11. Reviewer için kısa mimari not — 20 dakika

Repo'ya kısa bir `ARCHITECTURE.md` koyabilirsin. Bir sayfayı geçmesin.

İçerik:

1. Önceki bağımlılık diyagramı
2. Yeni bağımlılık diyagramı
3. `GameSession`, `RunState`, `RunEconomy`, `ZoneWheelBuilder`, presenter ve installer görevleri
4. Neden table-driven zone/rarity seçtiğin
5. Neden explicit State class'ları veya DI framework kullanmadığın
6. `ZoneRuleEditor` ile mevcut designer validation'ı
7. Eklenen testlerin hangi kuralları koruduğu

Örnek açıklama dili:

> `GameSession` previously located and drove concrete scene views while also calculating wheel content and economy. Scene composition is now isolated in `GameSceneInstaller`; run state, economy and wheel construction have separate owners. Presentation reacts to domain events, so the flow no longer depends on concrete panels.

Buzzword listesi yazma. Her cümlenin projede gösterilebilir karşılığı olsun.

---

## 12. Android/iOS build, video ve teslim — 1 saat 30 dakika

### Build

- [ ] Development Build kapalı bir Android build al.
- [ ] Fiziksel cihazda orientation, safe area ve performansı kontrol et.
- [ ] APK'yı GitHub'da gerçek bir **Release** oluşturup asset olarak yükle.
- [ ] Release sayfasından APK'nın indirilebildiğini kontrol et.

### Video çekim planı

60–90 saniyelik temiz bir video yeterli:

1. Uygulama açılışı ve genel layout
2. Normal spin, net winning-slot reveal ve reward eklenmesi
3. Safe zone'da exit kullanılabilirliği
4. Bomb sonucu, patlama ve lose panel geçişi
5. Revive sonrası aynı zone'da devam
6. Reward summary veya run sonucu

- [ ] Unity Game View değil, gerçek Android/iOS cihaz kaydı kullan.
- [ ] Gereksiz bekleme ve tekrarları kırp.
- [ ] MP4 / H.264 video + AAC audio kullan.
- [ ] Video orientation'ı oyunla aynı ve sabit olsun.

### Google Drive

- [ ] Yeni dosyayı yükle.
- [ ] Drive processing bitene kadar bekle.
- [ ] “Anyone with the link — Viewer” yap.
- [ ] Linki signed-out/incognito pencerede aç.
- [ ] Sadece dosya adının görünmesini değil, videonun gerçekten oynamasını doğrula.
- [ ] Mümkünse ikinci cihaz veya başka tarayıcıda da dene.

Mevcut linkte preview hatası görüldüğü için bu son kontrol zorunlu.

---

# İki günlük saat planı

Bu saatler timebox'tır; görev uzarsa “daha güzel” yapmak yerine bitti ölçütüne dön.

## Gün 1

| Süre | İş |
|---:|---|
| 00:00–00:30 | Branch, baseline, mevcut akışları doğrula |
| 00:30–01:00 | State/transition sözleşmesi |
| 01:00–03:30 | DIP: installer, button binding, panel/event wiring, ads |
| 03:30–05:30 | SRP: `ZoneWheelBuilder`, `RunEconomy`, `RunState` |
| 05:30–06:15 | OCP + LSP/ISP quick wins |
| 06:15–07:45 | Spin result + death flow |
| 07:45–08:30 | Smoke test, missing reference kontrolü, commit |

## Gün 2

| Süre | İş |
|---:|---|
| 00:00–02:30 | Responsive layout ve üç aspect ratio |
| 02:30–04:00 | Wheel/panel polish |
| 04:00–04:30 | Brief compliance sweep |
| 04:30–05:15 | 4–6 hedefli test |
| 05:15–06:00 | Final QA |
| 06:00–06:20 | Kısa architecture note |
| 06:20–07:50 | Device build, kayıt, Release ve Drive upload |
| 07:50–08:30 | Buffer ve son link/build doğrulama |

---

# Süre yetmezse kesme sırası

Önce kes:

1. Ekstra panel animasyonları
2. Ses/haptic/pointer tick
3. Test sayısını 6'dan 2–3'e indirme
4. `ARCHITECTURE.md` görsel süslemesi
5. Hint akışının ayrıca servisleştirilmesi — fakat UI bağımlılığını `GameSession`ta bırakma

Kesme:

- `FindObjectOfType` temizliği
- Button'lardaki `GameSession` bağımlılığının kaldırılması
- `GameSession`tan panel referanslarının çıkarılması
- Zone wheel üretimi ve ekonominin ayrılması
- Kazanan slotun görünür tutulması
- Bomb → lose akışı
- Responsive 20:9 / 16:9 / 4:3 kontrolü
- Fiziksel cihaz videosu ve incognito Drive testi

---

# Final teslim kontrol listesi

- [ ] Unity Console temiz
- [ ] EditMode testleri geçiyor
- [ ] Android/iOS cihaz build'i çalışıyor
- [ ] Runtime `FindObjectOfType` = 0
- [ ] `GameSession` concrete panel ref = 0
- [ ] Button → `GameSession` ref = 0
- [ ] Normal spin sonucu okunuyor
- [ ] Bomb akışı okunuyor
- [ ] Revive doğru zone'a dönüyor
- [ ] Exit kuralı doğru
- [ ] 20:9 screenshot güncel
- [ ] 16:9 screenshot güncel
- [ ] 4:3 screenshot güncel
- [ ] APK GitHub Release asset'i olarak indirilebiliyor
- [ ] Drive linki signed-out durumda oynuyor
- [ ] README'deki video/release linkleri güncel
- [ ] Reviewer'a gönderilecek kısa değişiklik özeti hazır

## Reviewer'a gönderilecek değişiklik özeti örneği

Son durum doğruysa, kendi cümlelerinle şu dört noktayı söyle:

- Scene-wide lookups were removed and scene wiring was moved to a composition root.
- Game flow no longer drives concrete panels; wheel construction, run state and economy now have separate responsibilities.
- The spin result and bomb/death sequences now include explicit reveal states before progression.
- The UI was revised and verified at 20:9, 16:9 and 4:3, and the new gameplay video was recorded on a physical device.

Sayılarla övünmek yerine neden ve gözle görülen sonucu anlat.

---

## Son hatırlatma

Bu testte senden production ölçüsünde framework beklemiyorlar. Şunu görmek istiyorlar:

1. Geri bildirimi savunmaya geçmeden anlayabiliyor musun?
2. Oyuncunun gördüğü problemi gerçekten düzeltebiliyor musun?
3. Bağımlılıkların yönünü bilinçli kurabiliyor musun?
4. İki günlük sürede doğru scope seçebiliyor musun?
5. Yazdığın her değişikliği açıklayabiliyor musun?

Her görevde “daha fazla class” değil, “daha net sorumluluk ve daha iyi oyuncu deneyimi” hedefle.
