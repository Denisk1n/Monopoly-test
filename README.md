# Монополия (консольная версия на C#)

Упрощённая консольная игра «Монополия» для 2–6 игроков. Проект сделан по нашей
проектной документации (видение, прецеденты, модель предметной области, диаграмма классов).

## Что упрощено

- Нет Банка: деньги за «Старт» начисляются игроку, налоги и покупки просто уходят из игры.
- Нет дублей: каждый ход — один бросок двух кубиков.
- Нет фишек: позиция хранится у самого игрока (`Player.Position`).
- Владелец хранится в клетке (`Property.Owner`).
- Тюрьма: игрок пропускает 3 хода и выходит.
- Нет ипотеки и обмена между игроками.
- При банкротстве дома сначала продаются за полцены. Если денег всё равно не хватает,
  игрок выбывает, его имущество переходит кредитору (или становится свободным,
  если долг был налогом/штрафом).

## Структура

```
Monopoly.sln
src/Monopoly/                 — сама игра (консольное приложение)
  Players/                    — Player, Dice, Auction, ввод (IInput, ConsoleInput)
  Squares/                    — клетки: Square, Property, Street, Railroad, Utility, Tax, ...
  Cards/                      — Card, Deck, EffectType
  Core/                       — Board, Game, GameStatus
  Program.cs                  — консольное меню
tests/Monopoly.Tests/         — unit-тесты (xUnit)
  Fakes/                      — FakeInput, FixedDice, TestGame (заготовки для тестов)
```

## Установка на Mac

1. Установить .NET 8 SDK:
   - через Homebrew: `brew install --cask dotnet-sdk@8`
   - или скачать установщик для macOS с https://dotnet.microsoft.com/download/dotnet/8.0
     (для Apple M1/M2/M3 — версия **Arm64**, для Intel — **x64**)
2. Проверить: `dotnet --version` (нужна 8.x или новее).
3. Редактор на выбор:
   - **JetBrains Rider** (бесплатный для некоммерческого использования) — просто открыть `Monopoly.sln`;
   - **VS Code** + расширение **C# Dev Kit** — открыть папку проекта.

## Запуск

Из корня проекта:

```bash
dotnet run --project src/Monopoly
```

## Тесты

```bash
dotnet test
```

### Покрытие кода

```bash
dotnet test --collect:"XPlat Code Coverage"

# один раз установить генератор отчёта
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator -reports:"tests/**/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:Html
open coveragereport/index.html
```

`Program.cs` и `ConsoleInput.cs` исключены из покрытия (`[ExcludeFromCodeCoverage]`):
это работа с консолью, её не тестируем.

## Разделение работы

| Часть | Что делает | Файлы игры | Тесты |
|---|---|---|---|
| **1. Игрок, кубики, аукцион** | настройка проекта, игрок, кубики, аукцион, ввод | `Monopoly.sln`, оба `.csproj`, `.gitignore`, `Players/*` | `Fakes/*`, `PlayerTests`, `DiceTests`, `AuctionTests` |
| **2. Собственность** | базовая клетка, покупка/аренда, улицы, ж/д, коммуналка | `Squares/Square`, `Property`, `Street`, `Railroad`, `Utility` | `PropertyTests`, `StreetTests`, `RailroadAndUtilityTests` |
| **3. Спец. клетки, карточки, доска** | налог, шанс, казна, тюрьма и т.д., карточки, колоды, доска из 40 клеток | `Squares/Tax`, `Chance`, `CommunityChest`, `Go`, `Jail`, `FreeParking`, `GoToJail`, `Cards/*`, `Core/Board` | `SpecialSquaresTests`, `CardTests`, `DeckTests`, `BoardTests` |
| **4. Игра и консоль** | ход игрока, покупка, аукцион, оплата, банкротство, строительство, меню | `Core/Game`, `Core/GameStatus`, `Program.cs`, `README.md` | `GameTests` |

## Как работаем с GitHub

Классы ссылаются друг на друга (клетка вызывает методы `Game`, а `Game` хранит клетки),
поэтому проект полностью собирается только когда залиты все 4 части.

1. **Участник 1** создаёт репозиторий и заливает свою часть в `main`:
   ```bash
   git init
   git add .
   git commit -m "Настройка проекта: игрок, кубики, аукцион"
   git branch -M main
   git remote add origin https://github.com/<user>/monopoly.git
   git push -u origin main
   ```
   Остальных добавляет в Settings → Collaborators.
2. **Участники 2, 3, 4** клонируют репозиторий и работают в своей ветке:
   ```bash
   git clone https://github.com/<user>/monopoly.git
   cd monopoly
   git checkout -b part-2-property        # part-3-squares-cards / part-4-game
   # добавить свои файлы
   git add .
   git commit -m "Собственность: улицы, ж/д, коммуналка + тесты"
   git push -u origin part-2-property
   ```
   Затем на GitHub открывают Pull Request в `main`.
3. PR сливаются по порядку: 2 → 3 → 4. После последнего проверяем:
   ```bash
   git pull
   dotnet build
   dotnet test
   ```
