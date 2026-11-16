# TipCalculator (ASP.NET Core MVC + EF Core + migraciones)

## Primera ejecución
1. Abre `TipCalculator.slnx` en Visual Studio.
2. Consola del Administrador de paquetes: `Update-Database`
   (aplica la migración `InitialCreate` y crea la base `TipCalculatorDb` en LocalDB).
3. F5.

## Cuando cambies un modelo
```
Add-Migration NombreDelCambio
Update-Database
```

## Si algo falla con la migración incluida
Borra la carpeta `Migrations` y la base (`Drop-Database`), y regenera:
```
Add-Migration InitialCreate
Update-Database
```
