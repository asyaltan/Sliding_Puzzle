# Sliding_Puzzle# Sliding Puzzle Oyunu

## Genel Bakış

**Sliding Puzzle Oyunu**, **C# Windows Forms** kullanılarak geliştirilmiş bir masaüstü bulmaca uygulamasıdır. Bu proje, klasik 3x3 kaydırmalı yapboz oyununu yeniden oluşturur ve oyuncunun karıştırılmış bir görseli doğru sıraya getirmesini amaçlar.

Uygulama; görsel işleme, sürükle-bırak etkileşimi, hamle sınırlaması ve geri sayım gibi mekanikleri bir araya getirerek kullanıcıya dinamik bir oyun deneyimi sunar.

---

## Özellikler

### Temel Oynanış
- Klasik **3x3 yapboz sistemi**
- Çözülebilir rastgele karıştırma algoritması
- Fare sürükleme ile parça hareket ettirme
- Otomatik kazanma kontrol sistemi

### Oyun Mekanikleri
- **60 saniyelik süre sınırı**
- **50 hamle limiti**
- Kazanma / kaybetme durumları
- Oyun sonunda yeniden başlatma veya devam etme seçenekleri

### Görsel Destek
- Yerleşik rastgele görseller
- Kullanıcı tarafından görsel yükleme imkanı
- Görsellerin otomatik olarak 9 parçaya bölünmesi

### Kullanıcı Deneyimi
- Temiz Windows Forms arayüzü
- Anlık hamle sayacı
- Gerçek zamanlı geri sayım
- Anlık bilgilendirme pencereleri

---

## Kullanılan Teknolojiler

| Teknoloji | Açıklama |
|--------|---------|
| C# | Ana programlama dili |
| Windows Forms | Masaüstü arayüz |
| System.Drawing | Görsel işleme ve bölme |
| System.Numerics | Fare yön hesaplamaları |
| .NET Framework / .NET | Çalışma ortamı |

---

## Proje Yapısı

```text
SlidingPuzzle/
│── Form1.cs                # Ana oyun mantığı
│── Form1.Designer.cs       # Arayüz bileşenleri
│── Program.cs              # Başlangıç noktası
│── Images/                 # Varsayılan görseller
│── README.md
