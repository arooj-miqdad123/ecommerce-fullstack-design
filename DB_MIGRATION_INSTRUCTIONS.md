# Database Migration Required - Run These Commands

Kyunke seed data mein images .jpg se .png ho gaye hain,
isliye aapko database update karna hoga.

## Package Manager Console mein run karo (Visual Studio):

```
Add-Migration FixImageExtensions
Update-Database
```

## Agar error aaye "pending model changes":
```
Drop-Database
Update-Database
```

## SSMS mein verify karo:
- EcommerceDb > Tables > dbo.Products
- ImageUrl column mein .png hona chahiye

Done! App restart karo aur images show hongi.
