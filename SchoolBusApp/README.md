# 🚌 شركة نقل الطلاب | Student Transportation System

نظام إدارة شركة نقل الطلاب — تطبيق Windows Forms بـ C# و SQL Server

---

## 📋 المتطلبات | Requirements

| المطلب | الإصدار |
|--------|---------|
| Windows | 10 / 11 |
| .NET SDK | 6.0 أو أحدث |
| Visual Studio | 2022 (Community مجاني) |
| SQL Server | أي إصدار (Express مجاني) |

---

## 🚀 خطوات التشغيل | Setup Steps

### 1. تثبيت المتطلبات | Install Prerequisites

- **.NET 6 SDK** → https://dotnet.microsoft.com/download/dotnet/6.0
- **Visual Studio 2022** → https://visualstudio.microsoft.com/
  - أثناء التثبيت اختر: **.NET desktop development**
- **SQL Server Express (مجاني)** → https://www.microsoft.com/sql-server/sql-server-downloads

---

### 2. فتح المشروع | Open Project

1. افتح `SchoolBusApp.sln` في Visual Studio 2022
2. انتظر تحميل المكتبات (NuGet packages)

---

### 3. إعداد قاعدة البيانات | Configure Database

افتح الملف: `SchoolBusApp/Database.cs`

غيّر سلسلة الاتصال حسب إعدادات SQL Server لديك:

```csharp
// الاتصال المباشر (Windows Authentication) — الافتراضي:
"Server=.;Database=SchoolBusDB;Integrated Security=True;TrustServerCertificate=True;"

// باستخدام كلمة مرور (SQL Authentication):
"Server=.;Database=SchoolBusDB;User Id=sa;Password=كلمة_المرور;TrustServerCertificate=True;"

// اسم سيرفر مختلف:
"Server=MYPC\SQLEXPRESS;Database=SchoolBusDB;Integrated Security=True;TrustServerCertificate=True;"
```

يمكنك أيضاً تغيير الاتصال من داخل التطبيق → **الإعدادات | Settings**

---

### 4. تشغيل التطبيق | Run the App

اضغط **F5** أو زر ▶️ في Visual Studio.

سيتم إنشاء جداول البيانات تلقائياً عند أول تشغيل.

---

## 📦 الوحدات | Modules

| الوحدة | الوصف |
|--------|-------|
| 🏠 لوحة التحكم | إحصاءات سريعة |
| 👨‍🎓 الطلاب | إضافة، تعديل، حذف الطلاب |
| 🚌 الحافلات والسائقون | إدارة الأسطول والسائقين |
| 🗺️ المسارات | تعريف مسارات الرحلات |
| ✅ الحضور والغياب | تسجيل يومي بالذهاب والإياب |
| 💰 المدفوعات | تتبع رسوم النقل الشهرية |
| ⚙️ الإعدادات | ضبط الاتصال بقاعدة البيانات |

---

## 🛠️ بناء التطبيق | Build

```bash
cd SchoolBusApp
dotnet build
dotnet run
```

---

## 📄 الترخيص | License
MIT License — للاستخدام الحر
