# EcommerceApp - .NET 10 MVC
## Complete Setup Guide (Step by Step)

---

## ✅ REQUIREMENTS (Pehle Install Karo)

1. **.NET 10 SDK** → https://dotnet.microsoft.com/download/dotnet/10.0
2. **Visual Studio 2022** (Community free hai) → https://visualstudio.microsoft.com/
   - Workload: "ASP.NET and web development" select karna
3. **SQL Server** (SSMS ke saath already hai) → already installed
4. **SSMS** → already installed

---

## 📁 STEP 1: Project Visual Studio Mein Open Karo

1. Zip extract karo
2. Visual Studio open karo
3. **File → Open → Project/Solution**
4. `EcommerceApp.csproj` file select karo
5. VS open ho jayega

---

## 📦 STEP 2: NuGet Packages Install Karo

Visual Studio mein:
1. **Tools → NuGet Package Manager → Package Manager Console**
2. Yeh commands ek ek karke run karo:

```
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 9.0.0
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 9.0.0
Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore -Version 9.0.0
Install-Package Stripe.net -Version 43.0.0
Install-Package Newtonsoft.Json -Version 13.0.3
```

---

## 🗄️ STEP 3: Database Setup (SSMS)

### appsettings.json mein Connection String check karo:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EcommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Agar aapka SQL Server alag naam ka hai (jaise `LAPTOP-NAME\SQLEXPRESS`), toh change karo:
```json
"Server=LAPTOP-NAME\\SQLEXPRESS;Database=EcommerceDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Package Manager Console mein Migration run karo:
```
Add-Migration InitialCreate
Update-Database
```

Yeh automatically:
- `EcommerceDb` database banayega SSMS mein
- Sari tables banayega (Products, Categories, Orders, etc.)
- 10 sample products aur 5 categories add karega
- Admin user banayega

---

## 🖼️ STEP 4: Images Add Karo

Apni images ko `wwwroot/images/` folder mein paste karo:

### Products (`wwwroot/images/products/`):
```
product-1.jpg  → T-Shirt image
product-2.jpg  → Backpack image
product-3.jpg  → Water Boiler image
product-4.jpg  → Resort Shirt
product-5.jpg  → GoPro Camera
product-6.jpg  → Canon DSLR
product-7.jpg  → Apple Watch
product-8.jpg  → Headphones
product-9.jpg  → iPhone
product-10.jpg → MacBook
```

### Categories (`wwwroot/images/categories/`):
```
electronics.jpg
clothes.jpg
gadgets.jpg
accessories.jpg
smartwatches.jpg
```

### Misc (`wwwroot/images/misc/`):
```
no-image.svg   → Already hai (placeholder)
flag-us.png    → US flag
visa.png       → Visa logo
mastercard.png → Mastercard logo
```

### Banner (`wwwroot/images/banner/`):
```
banner-1.jpg → Hero banner
banner-2.jpg → Second banner
```

**NOTE:** Agar image nahi hai toh bhi app chalega, `no-image.svg` show hoga.

---

## ▶️ STEP 5: App Run Karo

1. Visual Studio mein **F5** press karo (ya green Play button)
2. Browser automatically khulega
3. URL: `https://localhost:XXXX`

---

## 👤 Admin Panel Access

- URL: `/Admin`
- Email: `admin@ecommerce.com`
- Password: `Admin@123`

---

## 💳 Payment Testing

### Cash on Delivery (COD):
- Checkout pe COD select karo
- "Place Order" click karo
- Order turant confirm ho jayega

### Card Payment (Test Mode):
- Checkout pe Card select karo
- Card fields fill karo:
  - **Card Number:** `4242 4242 4242 4242`
  - **Expiry:** `12 / 26` (any future date)
  - **CVC:** `123`
  - **Name:** `Test User`
- "Pay Securely" click karo
- 2 second processing ke baad order confirm hoga
- Status "Processing" set ho jayegi

---

## 🔐 User Registration/Login

- Register: `/Account/Register`
- Login: `/Account/Login`
- Normal users orders dekh sakte hain
- Admin `/Admin` panel access kar sakta hai

---

## 📱 Mobile View

- App fully responsive hai
- Mobile pe bottom navigation bar show hota hai
- Hamburger menu se categories access karo

---

## 🗃️ Database Tables (SSMS mein check karo)

```
EcommerceDb/
├── AspNetUsers          → Login users
├── AspNetRoles          → Admin, User roles
├── Categories           → 5 categories
├── Products             → 10 sample products
├── Orders               → Customer orders
└── OrderItems           → Order line items
```

---

## ❗ Common Errors & Fixes

### Error: "Cannot connect to SQL Server"
→ appsettings.json mein Server name check karo
→ SSMS kholo aur server name copy karo

### Error: "Table doesn't exist"
→ Package Manager Console mein run karo:
```
Update-Database
```

### Error: "NuGet package not found"
→ Package Manager Console mein phir se Install-Package commands run karo

### Error: "Port already in use"
→ launchSettings.json mein port change karo ya VS restart karo

---

## 📋 Pages List

| Page | URL |
|------|-----|
| Home | `/` |
| Products | `/Product` |
| Product Detail | `/Product/Detail/1` |
| Cart | `/Cart` |
| Checkout | `/Order/Checkout` |
| Login | `/Account/Login` |
| Register | `/Account/Register` |
| My Orders | `/Order/MyOrders` |
| Admin Dashboard | `/Admin` |
| Admin Products | `/Admin/Products` |
| Admin Orders | `/Admin/Orders` |

---

## 🎯 Deadline: 5th June 2026

Good luck with your internship! 🚀
