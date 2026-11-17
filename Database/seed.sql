-- Тестовые данные для GameLibrary
-- Запускать в pgAdmin по базе GameLibrary ПОСЛЕ миграции (dotnet ef database update)
-- Пароли сейчас хранятся как обычный текст (как в Registration.axaml.cs)

BEGIN;

-- Очистка (если уже что-то было)
TRUNCATE TABLE
    "PlaySessions",
    "Notes",
    "GameTags",
    "GameGenres",
    "GamePlatforms",
    "Games",
    "Tags",
    "Genres",
    "Platforms",
    "PlayStatuses",
    "Users"
RESTART IDENTITY CASCADE;

-- ========== Users ==========
-- логин / пароль: demo / 123456  и  friend / 123456
INSERT INTO "Users" ("Id", "Login", "PasswordHash", "CreateAt") VALUES
(1, 'demo',   '123456', TIMESTAMPTZ '2026-01-10 10:00:00+00'),
(2, 'friend', '123456', TIMESTAMPTZ '2026-01-11 11:00:00+00');

-- ========== PlayStatuses ==========
INSERT INTO "PlayStatuses" ("Id", "Code", "Name") VALUES
(1, 'backlog',   'В планах'),
(2, 'playing',   'Прохожу'),
(3, 'completed', 'Пройдено'),
(4, 'on_hold',   'Отложено'),
(5, 'dropped',   'Брошено');

-- ========== Platforms ==========
INSERT INTO "Platforms" ("Id", "Name") VALUES
(1, 'PC'),
(2, 'PlayStation 5'),
(3, 'Xbox Series'),
(4, 'Nintendo Switch'),
(5, 'Другое');

-- ========== Genres ==========
INSERT INTO "Genres" ("Id", "Name") VALUES
(1, 'Action'),
(2, 'RPG'),
(3, 'Strategy'),
(4, 'Adventure'),
(5, 'Indie'),
(6, 'Simulation'),
(7, 'Shooter'),
(8, 'Horror');

-- ========== Tags ==========
INSERT INTO "Tags" ("Id", "Name") VALUES
(1, 'Singleplayer'),
(2, 'Multiplayer'),
(3, 'Open World'),
(4, 'Story Rich'),
(5, 'Roguelike'),
(6, 'Co-op'),
(7, 'Pixel Art'),
(8, 'Souls-like');

-- ========== Games (user demo = 1) ==========
-- PlayStatuses_Id: 1 backlog, 2 playing, 3 completed, 4 on_hold, 5 dropped
INSERT INTO "Games"
("Id", "Title", "ReleaseYear", "Description", "Rating", "IsWishlist", "PlaytimeMinutes", "CoverPath", "CreatedAt", "UpdateAt", "User_Id", "PlayStatuses_Id")
VALUES
(1,  'Hades',                 2020, 'Roguelike про побег из Подземного мира.',           9, FALSE, 840,  'hades.jpg',         TIMESTAMPTZ '2026-02-01 12:00:00+00', TIMESTAMPTZ '2026-02-10 12:00:00+00', 1, 2),
(2,  'The Witcher 3',         2015, 'РПГ про Геральта и поиск Цири.',                    10, FALSE, 3600, 'witcher3.jpg',      TIMESTAMPTZ '2026-02-01 12:05:00+00', TIMESTAMPTZ '2026-02-15 12:00:00+00', 1, 3),
(3,  'Stardew Valley',        2016, 'Ферма, рыбалка и жизнь в деревне.',                 9, FALSE, 1200, 'stardew.jpg',       TIMESTAMPTZ '2026-02-02 09:00:00+00', TIMESTAMPTZ '2026-02-20 09:00:00+00', 1, 2),
(4,  'Celeste',               2018, 'Платформер про гору и тревожность.',                9, FALSE, 480,  'celeste.jpg',       TIMESTAMPTZ '2026-02-03 10:00:00+00', TIMESTAMPTZ '2026-02-12 10:00:00+00', 1, 3),
(5,  'Elden Ring',            2022, 'Открытый мир FromSoftware.',                        10, FALSE, 2100, 'eldenring.jpg',     TIMESTAMPTZ '2026-02-04 11:00:00+00', TIMESTAMPTZ '2026-03-01 11:00:00+00', 1, 2),
(6,  'Balatro',               2024, 'Покерный роглайк.',                                 8, FALSE, 300,  'balatro.jpg',       TIMESTAMPTZ '2026-02-05 12:00:00+00', TIMESTAMPTZ '2026-02-28 12:00:00+00', 1, 1),
(7,  'Hollow Knight',         2017, 'Метроидвания в Халлоунесте.',                       9, FALSE, 900,  'hollowknight.jpg',  TIMESTAMPTZ '2026-02-06 13:00:00+00', TIMESTAMPTZ '2026-02-25 13:00:00+00', 1, 4),
(8,  'Cyberpunk 2077',        2020, 'РПГ в Найт-Сити.',                                  8, FALSE, 1500, 'cyberpunk.jpg',     TIMESTAMPTZ '2026-02-07 14:00:00+00', TIMESTAMPTZ '2026-03-02 14:00:00+00', 1, 3),
(9,  'Dead Space Remake',     2023, 'Хоррор на космическом корабле.',                    8, FALSE, 600,  'deadspace.jpg',     TIMESTAMPTZ '2026-02-08 15:00:00+00', TIMESTAMPTZ '2026-02-18 15:00:00+00', 1, 5),
(10, 'Clair Obscur: Expedition 33', 2025, 'Хочу взять, когда выйдет/купится.',           0, TRUE,  0,    'expedition33.jpg',  TIMESTAMPTZ '2026-02-09 16:00:00+00', TIMESTAMPTZ '2026-02-09 16:00:00+00', 1, 1),
(11, 'Silksong',              2025, 'Сиквел Hollow Knight в вишлисте.',                  0, TRUE,  0,    'silksong.jpg',      TIMESTAMPTZ '2026-02-09 16:30:00+00', TIMESTAMPTZ '2026-02-09 16:30:00+00', 1, 1),
(12, 'Civilization VI',       2016, 'Пошаговая стратегия.',                              8, FALSE, 2000, 'civ6.jpg',          TIMESTAMPTZ '2026-02-10 17:00:00+00', TIMESTAMPTZ '2026-03-05 17:00:00+00', 1, 2);

-- ========== Games (user friend = 2) — чтобы проверить фильтрацию по User_Id ==========
INSERT INTO "Games"
("Id", "Title", "ReleaseYear", "Description", "Rating", "IsWishlist", "PlaytimeMinutes", "CoverPath", "CreatedAt", "UpdateAt", "User_Id", "PlayStatuses_Id")
VALUES
(13, 'Portal 2',              2011, 'Головоломки с порталами.',                          10, FALSE, 500,  'portal2.jpg',       TIMESTAMPTZ '2026-02-11 10:00:00+00', TIMESTAMPTZ '2026-02-11 10:00:00+00', 2, 3),
(14, 'It Takes Two',          2021, 'Кооп-приключение.',                                 9, FALSE, 700,  'ittakestwo.jpg',    TIMESTAMPTZ '2026-02-11 10:30:00+00', TIMESTAMPTZ '2026-02-11 10:30:00+00', 2, 2);

-- ========== GamePlatforms ==========
INSERT INTO "GamePlatforms" ("Game_Id", "Platform_Id") VALUES
(1, 1), (1, 2), (1, 4),
(2, 1), (2, 2), (2, 3),
(3, 1), (3, 4),
(4, 1), (4, 4),
(5, 1), (5, 2), (5, 3),
(6, 1), (6, 4),
(7, 1), (7, 4),
(8, 1), (8, 2), (8, 3),
(9, 1), (9, 2),
(10, 1), (10, 2),
(11, 1), (11, 4),
(12, 1),
(13, 1),
(14, 1), (14, 2);

-- ========== GameGenres ==========
INSERT INTO "GameGenres" ("Game_Id", "Genre_Id") VALUES
(1, 1), (1, 5),
(2, 2), (2, 4),
(3, 6), (3, 5),
(4, 1), (4, 5),
(5, 1), (5, 2),
(6, 5), (6, 3),
(7, 1), (7, 4), (7, 5),
(8, 2), (8, 1),
(9, 8), (9, 1),
(10, 2),
(11, 1), (11, 4),
(12, 3),
(13, 4),
(14, 4);

-- ========== GameTags ==========
INSERT INTO "GameTags" ("Game_Id", "Tag_Id") VALUES
(1, 1), (1, 5), (1, 4),
(2, 1), (2, 3), (2, 4),
(3, 1), (3, 7),
(4, 1), (4, 7), (4, 4),
(5, 1), (5, 3), (5, 8),
(6, 1),
(7, 1), (7, 4),
(8, 1), (8, 3), (8, 4),
(9, 1), (9, 4),
(10, 1), (10, 4),
(11, 1),
(12, 1), (12, 2),
(13, 1), (13, 6),
(14, 2), (14, 6);

-- ========== Notes ==========
INSERT INTO "Notes" ("Id", "Body", "CreateAt", "Game_Id") VALUES
(1, 'Третий забег, оружие «Щит» пока не открыл.', TIMESTAMPTZ '2026-02-10 18:00:00+00', 1),
(2, 'Нужно добить все предсказания.',             TIMESTAMPTZ '2026-02-11 19:00:00+00', 1),
(3, 'Кровь и вино ещё не начинал.',               TIMESTAMPTZ '2026-02-15 20:00:00+00', 2),
(4, 'Посадить древние фрукты на ферме.',          TIMESTAMPTZ '2026-02-20 21:00:00+00', 3),
(5, 'B-side уровней на потом.',                   TIMESTAMPTZ '2026-02-12 22:00:00+00', 4),
(6, 'Застрял у Раданна, вернуться позже.',        TIMESTAMPTZ '2026-03-01 12:00:00+00', 5),
(7, 'Отложил на Path of Pain.',                   TIMESTAMPTZ '2026-02-25 13:00:00+00', 7),
(8, 'Слишком страшно ночью — бросил на середине.',TIMESTAMPTZ '2026-02-18 23:00:00+00', 9);

-- ========== PlaySessions ==========
-- Comment в миграции NOT NULL, поэтому пустую строку не оставляем
INSERT INTO "PlaySessions" ("Id", "Minutes", "PlayedOn", "Comment", "CreateAt", "Game_Id") VALUES
(1, 120, TIMESTAMPTZ '2026-02-08 20:00:00+00', 'Забег до босса Евридика', TIMESTAMPTZ '2026-02-08 22:00:00+00', 1),
(2,  90, TIMESTAMPTZ '2026-02-09 21:00:00+00', 'Ещё один забег',          TIMESTAMPTZ '2026-02-09 22:30:00+00', 1),
(3, 180, TIMESTAMPTZ '2026-02-14 19:00:00+00', 'Квест Барона',            TIMESTAMPTZ '2026-02-14 22:00:00+00', 2),
(4,  60, TIMESTAMPTZ '2026-02-19 18:00:00+00', 'Рыбалка и шахта',         TIMESTAMPTZ '2026-02-19 19:00:00+00', 3),
(5, 240, TIMESTAMPTZ '2026-02-28 17:00:00+00', 'Исследование Лимурнии',   TIMESTAMPTZ '2026-02-28 21:00:00+00', 5),
(6, 100, TIMESTAMPTZ '2026-03-04 16:00:00+00', 'Партия за Рим',           TIMESTAMPTZ '2026-03-04 18:00:00+00', 12),
(7, 150, TIMESTAMPTZ '2026-02-11 15:00:00+00', 'Кооп с другом',           TIMESTAMPTZ '2026-02-11 17:30:00+00', 14);

-- Сброс sequences, чтобы следующие INSERT из приложения не конфликтовали с Id
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'),          (SELECT MAX("Id") FROM "Users"));
SELECT setval(pg_get_serial_sequence('"PlayStatuses"', 'Id'),   (SELECT MAX("Id") FROM "PlayStatuses"));
SELECT setval(pg_get_serial_sequence('"Platforms"', 'Id'),      (SELECT MAX("Id") FROM "Platforms"));
SELECT setval(pg_get_serial_sequence('"Genres"', 'Id'),         (SELECT MAX("Id") FROM "Genres"));
SELECT setval(pg_get_serial_sequence('"Tags"', 'Id'),           (SELECT MAX("Id") FROM "Tags"));
SELECT setval(pg_get_serial_sequence('"Games"', 'Id'),          (SELECT MAX("Id") FROM "Games"));
SELECT setval(pg_get_serial_sequence('"Notes"', 'Id'),          (SELECT MAX("Id") FROM "Notes"));
SELECT setval(pg_get_serial_sequence('"PlaySessions"', 'Id'),   (SELECT MAX("Id") FROM "PlaySessions"));

COMMIT;
