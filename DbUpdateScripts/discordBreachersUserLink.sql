-- Table: public.BreachersDiscordUserLinks

-- DROP TABLE IF EXISTS public."BreachersDiscordUserLinks";

CREATE TABLE IF NOT EXISTS public."BreachersDiscordUserLinks"
(
    "DiscordUserId" numeric(20,0) NOT NULL,
    "BreachersUserId" text COLLATE pg_catalog."default",
    CONSTRAINT "PK_BreachersDiscordUserLinks" PRIMARY KEY ("DiscordUserId")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."BreachersDiscordUserLinks"
    OWNER to "abr-prod_owner";