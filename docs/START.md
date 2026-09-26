# Первый самостоятельный шаг

## Цель

Самостоятельно создать и запустить пустое ASP.NET Core приложение, затем понять каждую строку созданного `Program.cs`.

Установленный SDK: `10.0.100`. Проверка: `dotnet --version`.

## Последовательность

В корне репозитория создайте пустой веб-проект:

```powershell
dotnet new web -n EveryNode.Api -o src/EveryNode.Api --framework net10.0
dotnet new sln -n EveryNode
dotnet sln EveryNode.slnx add src/EveryNode.Api/EveryNode.Api.csproj
dotnet run --project src/EveryNode.Api
```

В .NET 10 команда `dotnet new sln` по умолчанию создаёт файл `.slnx`: [описание изменения](https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/10.0/dotnet-new-sln-slnx-default).

Откройте адрес, который покажет `dotnet run`, и убедитесь, что сервер отвечает. Затем найдите в официальном [учебнике по Minimal API](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0), за что отвечают `WebApplication.CreateBuilder`, `Build`, `MapGet` и `Run`.

## Первый небольшой результат

Продумайте HTTP-ответ для чтения пустого графа: какие данные нужны браузеру для отображения вершин и рёбер? Сначала запишите собственный вариант формата JSON, затем реализуйте один GET-маршрут. Следующая учебная тема — входные данные и серверная проверка при создании вершины.

Если встретится ошибка, принесите текст ошибки и собственную гипотезу о её причине. ИИ поможет найти соответствующую документацию и выбрать направление проверки.
