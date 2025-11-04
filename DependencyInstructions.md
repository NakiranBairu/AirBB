# Required Frontend Dependencies

1. Date Range Picker:
```html
<!-- CSS -->
<link rel="stylesheet" type="text/css" href="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.css" />

<!-- JavaScript -->
<script type="text/javascript" src="https://cdn.jsdelivr.net/jquery/latest/jquery.min.js"></script>
<script type="text/javascript" src="https://cdn.jsdelivr.net/momentjs/latest/moment.min.js"></script>
<script type="text/javascript" src="https://cdn.jsdelivr.net/npm/daterangepicker/daterangepicker.min.js"></script>
```

2. Font Awesome:
```html
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
```

3. Additional Required NuGet Packages:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SQLite" Version="7.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.0" />
```

4. Installation Steps:
- Add the above CDN links to _Layout.cshtml
- Install NuGet packages via Package Manager Console:
  ```powershell
  Install-Package Microsoft.EntityFrameworkCore.SQLite
  Install-Package Microsoft.EntityFrameworkCore.Tools
  ```
- Create and apply migrations:
  ```powershell
  Add-Migration InitialCreate
  Update-Database
  ```