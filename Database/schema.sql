-- Личная библиотека игр
-- PostgreSQL. Один пользователь по роли, полный CRUD.

CREATE TABLE users (
    id              BIGSERIAL PRIMARY KEY,
    login           VARCHAR(64)  NOT NULL UNIQUE,
    password_hash   VARCHAR(256) NOT NULL,
    created_at      TIMESTAMPTZ  NOT NULL DEFAULT now()
);

-- Справочники

CREATE TABLE platforms (
    id    BIGSERIAL PRIMARY KEY,
    name  VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE genres (
    id    BIGSERIAL PRIMARY KEY,
    name  VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE tags (
    id    BIGSERIAL PRIMARY KEY,
    name  VARCHAR(64) NOT NULL UNIQUE
);

CREATE TABLE play_statuses (
    id    BIGSERIAL PRIMARY KEY,
    code  VARCHAR(32) NOT NULL UNIQUE,
    name  VARCHAR(64) NOT NULL
);

-- Карточка игры

CREATE TABLE games (
    id                 BIGSERIAL PRIMARY KEY,
    user_id            BIGINT       NOT NULL REFERENCES users (id) ON DELETE CASCADE,
    title              VARCHAR(200) NOT NULL,
    release_year       SMALLINT     NULL CHECK (release_year BETWEEN 1950 AND 2100),
    description        TEXT         NULL,
    status_id          BIGINT       NULL REFERENCES play_statuses (id) ON DELETE SET NULL,
    rating             NUMERIC(3,1) NULL CHECK (rating >= 1 AND rating <= 10),
    is_wishlist        BOOLEAN      NOT NULL DEFAULT FALSE,
    playtime_minutes   INTEGER      NOT NULL DEFAULT 0 CHECK (playtime_minutes >= 0),
    created_at         TIMESTAMPTZ  NOT NULL DEFAULT now(),
    updated_at         TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE INDEX ix_games_user_id    ON games (user_id);
CREATE INDEX ix_games_status_id  ON games (status_id);
CREATE INDEX ix_games_title      ON games (title);

-- Связи многие-ко-многим

CREATE TABLE game_platforms (
    game_id     BIGINT NOT NULL REFERENCES games (id) ON DELETE CASCADE,
    platform_id BIGINT NOT NULL REFERENCES platforms (id) ON DELETE RESTRICT,
    PRIMARY KEY (game_id, platform_id)
);

CREATE TABLE game_genres (
    game_id  BIGINT NOT NULL REFERENCES games (id) ON DELETE CASCADE,
    genre_id BIGINT NOT NULL REFERENCES genres (id) ON DELETE RESTRICT,
    PRIMARY KEY (game_id, genre_id)
);

CREATE TABLE game_tags (
    game_id BIGINT NOT NULL REFERENCES games (id) ON DELETE CASCADE,
    tag_id  BIGINT NOT NULL REFERENCES tags (id) ON DELETE RESTRICT,
    PRIMARY KEY (game_id, tag_id)
);

-- Заметки и сессии времени

CREATE TABLE notes (
    id          BIGSERIAL PRIMARY KEY,
    game_id     BIGINT      NOT NULL REFERENCES games (id) ON DELETE CASCADE,
    body        TEXT        NOT NULL,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX ix_notes_game_id ON notes (game_id);

CREATE TABLE play_sessions (
    id          BIGSERIAL PRIMARY KEY,
    game_id     BIGINT      NOT NULL REFERENCES games (id) ON DELETE CASCADE,
    minutes     INTEGER     NOT NULL CHECK (minutes > 0),
    played_on   DATE        NULL,
    comment     VARCHAR(500) NULL,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX ix_play_sessions_game_id ON play_sessions (game_id);

-- Стартовые строки справочников (не пользователей и не игр)

INSERT INTO play_statuses (code, name) VALUES
    ('backlog',   'В планах'),
    ('playing',   'Прохожу'),
    ('completed', 'Пройдено'),
    ('on_hold',   'Отложено'),
    ('dropped',   'Брошено');

INSERT INTO platforms (name) VALUES
    ('PC'),
    ('PlayStation 5'),
    ('Xbox Series'),
    ('Nintendo Switch'),
    ('Другое');

INSERT INTO genres (name) VALUES
    ('Action'),
    ('RPG'),
    ('Strategy'),
    ('Adventure'),
    ('Indie'),
    ('Simulation');
