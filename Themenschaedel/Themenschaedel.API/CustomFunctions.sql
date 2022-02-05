-- Database generated with pgModeler (PostgreSQL Database Modeler).
-- pgModeler version: 0.9.4
-- PostgreSQL version: 13.0
-- Project Site: pgmodeler.io
-- Model Author: ---

-- Database creation must be performed outside a multi lined SQL file. 
-- These commands were put in this file only as a convenience.
-- 
-- object: "Themenarchiv" | type: DATABASE --
-- DROP DATABASE IF EXISTS "Themenarchiv";
CREATE DATABASE "Themenarchiv"
	ENCODING = 'UTF8'
	LC_COLLATE = 'German_Germany.1252'
	LC_CTYPE = 'German_Germany.1252'
	TABLESPACE = pg_default
	OWNER = postgres;
-- ddl-end --


-- object: public.episodes | type: TABLE --
-- DROP TABLE IF EXISTS public.episodes CASCADE;
CREATE TABLE public.episodes (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	uuid character varying,
	title character varying,
	episode_number bigint,
	subtitle character varying,
	description character varying,
	media_file character varying,
	spotify_file character varying,
	duration integer,
	type character varying,
	image character varying,
	explicit boolean,
	verified boolean,
	published_at timestamp,
	created_at timestamp,
	updated_at timestamp,
	CONSTRAINT episodes_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.episodes OWNER TO postgres;
-- ddl-end --

-- -- object: public.episodes_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.episodes_id_seq CASCADE;
-- CREATE SEQUENCE public.episodes_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.episodes_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.topic | type: TABLE --
-- DROP TABLE IF EXISTS public.topic CASCADE;
CREATE TABLE public.topic (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	name character varying,
	timestamp_start integer,
	timestamp_end integer,
	duration integer,
	community_contributed boolean,
	ad boolean,
	created_at timestamp,
	updated_at timestamp,
	id_episodes integer,
	id_user integer,
	CONSTRAINT topic_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.topic OWNER TO postgres;
-- ddl-end --

-- -- object: public.topic_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.topic_id_seq CASCADE;
-- CREATE SEQUENCE public.topic_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.topic_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.subtopics | type: TABLE --
-- DROP TABLE IF EXISTS public.subtopics CASCADE;
CREATE TABLE public.subtopics (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	name character varying,
	created_at timestamp,
	updated_at timestamp,
	id_topic integer,
	id_user integer,
	CONSTRAINT subtopics_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.subtopics OWNER TO postgres;
-- ddl-end --

-- -- object: public.subtopics_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.subtopics_id_seq CASCADE;
-- CREATE SEQUENCE public.subtopics_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.subtopics_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.users | type: TABLE --
-- DROP TABLE IF EXISTS public.users CASCADE;
CREATE TABLE public.users (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	uuid varchar(32) NOT NULL,
	username varchar(30),
	description text,
	email varchar(65),
	email_verification_id varchar(64),
	email_verified_at timestamp,
	password varchar(500),
	salt bytea,
	created_at timestamp,
	updated_at timestamp,
	id_roles integer,
	CONSTRAINT user_pk PRIMARY KEY (id),
	CONSTRAINT unique_username_constraint UNIQUE (username),
	CONSTRAINT unique_uuid_constraint UNIQUE (uuid)
);
-- ddl-end --
ALTER TABLE public.users OWNER TO postgres;
-- ddl-end --

-- -- object: public.user_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.user_id_seq CASCADE;
-- CREATE SEQUENCE public.user_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.user_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.claims | type: TABLE --
-- DROP TABLE IF EXISTS public.claims CASCADE;
CREATE TABLE public.claims (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	claimed_at timestamp,
	valid_until timestamp,
	created_at timestamp,
	updated_at timestamp,
	id_user integer,
	id_episodes integer,
	CONSTRAINT claims_pk PRIMARY KEY (id),
	CONSTRAINT unique_user UNIQUE (id_user),
	CONSTRAINT unique_episode UNIQUE (id_episodes)
);
-- ddl-end --
ALTER TABLE public.claims OWNER TO postgres;
-- ddl-end --

-- -- object: public.claims_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.claims_id_seq CASCADE;
-- CREATE SEQUENCE public.claims_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.claims_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.votes | type: TABLE --
-- DROP TABLE IF EXISTS public.votes CASCADE;
CREATE TABLE public.votes (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	positive boolean,
	created_at timestamp,
	updated_at timestamp,
	id_episodes integer,
	id_user integer,
	CONSTRAINT votes_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.votes OWNER TO postgres;
-- ddl-end --

-- -- object: public.votes_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.votes_id_seq CASCADE;
-- CREATE SEQUENCE public.votes_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.votes_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.flags | type: TABLE --
-- DROP TABLE IF EXISTS public.flags CASCADE;
CREATE TABLE public.flags (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	reason character varying,
	created_at timestamp,
	updated_at timestamp,
	id_episodes integer,
	id_users integer,
	CONSTRAINT flags_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.flags OWNER TO postgres;
-- ddl-end --

-- -- object: public.flags_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.flags_id_seq CASCADE;
-- CREATE SEQUENCE public.flags_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.flags_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.person | type: TABLE --
-- DROP TABLE IF EXISTS public.person CASCADE;
CREATE TABLE public.person (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	name character varying NOT NULL,
	host boolean,
	description character varying,
	created_at timestamp,
	updated_at timestamp,
	CONSTRAINT person_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.person OWNER TO postgres;
-- ddl-end --

-- -- object: public.person_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.person_id_seq CASCADE;
-- CREATE SEQUENCE public.person_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.person_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.episode_person | type: TABLE --
-- DROP TABLE IF EXISTS public.episode_person CASCADE;
CREATE TABLE public.episode_person (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START WITH 1 CACHE 1 ),
	created_at timestamp,
	updated_at timestamp,
	id_person integer,
	id_episodes integer,
	CONSTRAINT episode_person_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.episode_person OWNER TO postgres;
-- ddl-end --

-- -- object: public.episode_person_id_seq | type: SEQUENCE --
-- -- DROP SEQUENCE IF EXISTS public.episode_person_id_seq CASCADE;
-- CREATE SEQUENCE public.episode_person_id_seq
-- 	INCREMENT BY 1
-- 	MINVALUE 1
-- 	MAXVALUE 2147483647
-- 	START WITH 1
-- 	CACHE 1
-- 	NO CYCLE
-- 	OWNED BY NONE;
-- 
-- -- ddl-end --
-- ALTER SEQUENCE public.episode_person_id_seq OWNER TO postgres;
-- -- ddl-end --
-- 
-- object: public.token | type: TABLE --
-- DROP TABLE IF EXISTS public.token CASCADE;
CREATE TABLE public.token (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
	value varchar(512),
	created_at timestamp,
	id_users integer,
	CONSTRAINT token_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.token OWNER TO postgres;
-- ddl-end --

-- object: users_fk | type: CONSTRAINT --
-- ALTER TABLE public.token DROP CONSTRAINT IF EXISTS users_fk CASCADE;
ALTER TABLE public.token ADD CONSTRAINT users_fk FOREIGN KEY (id_users)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: public.roles | type: TABLE --
-- DROP TABLE IF EXISTS public.roles CASCADE;
CREATE TABLE public.roles (
	id integer NOT NULL GENERATED ALWAYS AS IDENTITY ,
	name varchar(64),
	CONSTRAINT roles_pk PRIMARY KEY (id)
);
-- ddl-end --
ALTER TABLE public.roles OWNER TO postgres;
-- ddl-end --

INSERT INTO public.roles (id, name) VALUES (DEFAULT, E'User');
-- ddl-end --
INSERT INTO public.roles (id, name) VALUES (DEFAULT, E'Verified User');
-- ddl-end --
INSERT INTO public.roles (id, name) VALUES (DEFAULT, E'Froid');
-- ddl-end --
INSERT INTO public.roles (id, name) VALUES (DEFAULT, E'Moderator');
-- ddl-end --
INSERT INTO public.roles (id, name) VALUES (DEFAULT, E'Admin');
-- ddl-end --

-- object: roles_fk | type: CONSTRAINT --
-- ALTER TABLE public.users DROP CONSTRAINT IF EXISTS roles_fk CASCADE;
ALTER TABLE public.users ADD CONSTRAINT roles_fk FOREIGN KEY (id_roles)
REFERENCES public.roles (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: episodes_fk | type: CONSTRAINT --
-- ALTER TABLE public.flags DROP CONSTRAINT IF EXISTS episodes_fk CASCADE;
ALTER TABLE public.flags ADD CONSTRAINT episodes_fk FOREIGN KEY (id_episodes)
REFERENCES public.episodes (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: users_fk | type: CONSTRAINT --
-- ALTER TABLE public.flags DROP CONSTRAINT IF EXISTS users_fk CASCADE;
ALTER TABLE public.flags ADD CONSTRAINT users_fk FOREIGN KEY (id_users)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: unique_combined_episode_user | type: CONSTRAINT --
-- ALTER TABLE public.flags DROP CONSTRAINT IF EXISTS unique_combined_episode_user CASCADE;
ALTER TABLE public.flags ADD CONSTRAINT unique_combined_episode_user UNIQUE (id_episodes,id_users);
-- ddl-end --

-- object: episodes_fk | type: CONSTRAINT --
-- ALTER TABLE public.topic DROP CONSTRAINT IF EXISTS episodes_fk CASCADE;
ALTER TABLE public.topic ADD CONSTRAINT episodes_fk FOREIGN KEY (id_episodes)
REFERENCES public.episodes (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: user_fk | type: CONSTRAINT --
-- ALTER TABLE public.topic DROP CONSTRAINT IF EXISTS user_fk CASCADE;
ALTER TABLE public.topic ADD CONSTRAINT user_fk FOREIGN KEY (id_user)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: topic_fk | type: CONSTRAINT --
-- ALTER TABLE public.subtopics DROP CONSTRAINT IF EXISTS topic_fk CASCADE;
ALTER TABLE public.subtopics ADD CONSTRAINT topic_fk FOREIGN KEY (id_topic)
REFERENCES public.topic (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: user_fk | type: CONSTRAINT --
-- ALTER TABLE public.subtopics DROP CONSTRAINT IF EXISTS user_fk CASCADE;
ALTER TABLE public.subtopics ADD CONSTRAINT user_fk FOREIGN KEY (id_user)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: user_fk | type: CONSTRAINT --
-- ALTER TABLE public.claims DROP CONSTRAINT IF EXISTS user_fk CASCADE;
ALTER TABLE public.claims ADD CONSTRAINT user_fk FOREIGN KEY (id_user)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: episodes_fk | type: CONSTRAINT --
-- ALTER TABLE public.claims DROP CONSTRAINT IF EXISTS episodes_fk CASCADE;
ALTER TABLE public.claims ADD CONSTRAINT episodes_fk FOREIGN KEY (id_episodes)
REFERENCES public.episodes (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: episodes_fk | type: CONSTRAINT --
-- ALTER TABLE public.votes DROP CONSTRAINT IF EXISTS episodes_fk CASCADE;
ALTER TABLE public.votes ADD CONSTRAINT episodes_fk FOREIGN KEY (id_episodes)
REFERENCES public.episodes (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: user_fk | type: CONSTRAINT --
-- ALTER TABLE public.votes DROP CONSTRAINT IF EXISTS user_fk CASCADE;
ALTER TABLE public.votes ADD CONSTRAINT user_fk FOREIGN KEY (id_user)
REFERENCES public.users (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: person_fk | type: CONSTRAINT --
-- ALTER TABLE public.episode_person DROP CONSTRAINT IF EXISTS person_fk CASCADE;
ALTER TABLE public.episode_person ADD CONSTRAINT person_fk FOREIGN KEY (id_person)
REFERENCES public.person (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: episodes_fk | type: CONSTRAINT --
-- ALTER TABLE public.episode_person DROP CONSTRAINT IF EXISTS episodes_fk CASCADE;
ALTER TABLE public.episode_person ADD CONSTRAINT episodes_fk FOREIGN KEY (id_episodes)
REFERENCES public.episodes (id) MATCH FULL
ON DELETE SET NULL ON UPDATE CASCADE;
-- ddl-end --

-- object: "grant_CU_eb94f049ac" | type: PERMISSION --
GRANT CREATE,USAGE
   ON SCHEMA public
   TO postgres;
-- ddl-end --

-- object: "grant_CU_cd8e46e7b6" | type: PERMISSION --
GRANT CREATE,USAGE
   ON SCHEMA public
   TO PUBLIC;
-- ddl-end --















-- Custom Functions --


CREATE OR REPLACE FUNCTION udf_GetEpisodes()
   RETURNS TABLE (id integer,
	uuid character varying,
	title character varying,
	episode_number bigint,
	subtitle character varying,
	description character varying,
	media_file character varying,
	spotify_file character varying,
	duration integer,
	type character varying,
	image character varying,
	explicit boolean,
	verified boolean,
	published_at timestamp,
	created_at timestamp,
	updated_at timestamp,
	flags bigint,
	upvotes bigint,
	downvotes bigint,
	claimed boolean)
  LANGUAGE plpgsql AS
$BODY$
BEGIN

  RETURN QUERY
    SELECT a.*,
           (SELECT COUNT(*) FROM flags f WHERE f.id_episodes  = a.id) AS flags,
           (SELECT COUNT(CASE WHEN positive THEN 1 END) FROM votes v WHERE v.id_episodes  = a.id) AS upvotes,
           (SELECT COUNT(CASE WHEN NOT positive THEN 1 END) FROM votes v WHERE v.id_episodes  = a.id) AS downvotes,
           CASE WHEN b.id IS NOT NULL THEN true else false END claimed
    FROM episodes a
    LEFT JOIN claims b on a.id = b.id_episodes
    ORDER BY a.published_at DESC;
END;
$BODY$;


CREATE OR REPLACE FUNCTION udf_GetEpisodesByPageNumberAndSize(
 PageNumber INTEGER = NULL,
 PageSize INTEGER = NULL
 )
   RETURNS TABLE (id integer,
	uuid character varying,
	title character varying,
	episode_number bigint,
	subtitle character varying,
	description character varying,
	media_file character varying,
	spotify_file character varying,
	duration integer,
	type character varying,
	image character varying,
	explicit boolean,
	verified boolean,
	published_at timestamp,
	created_at timestamp,
	updated_at timestamp,
	flags bigint,
	upvotes bigint,
	downvotes bigint,
	claimed boolean)
  LANGUAGE plpgsql AS
$BODY$
 DECLARE
  PageOffset INTEGER :=0;
 BEGIN

  PageOffset := ((PageNumber-1) * PageSize);

  RETURN QUERY
    SELECT * FROM udf_GetEpisodes()
    LIMIT PageSize
    OFFSET PageOffset;
END;
$BODY$;


CREATE OR REPLACE FUNCTION udf_episodes_GetRowsByPageNumberAndSize(
 PageNumber INTEGER = NULL,
 PageSize INTEGER = NULL
 )
 RETURNS SETOF public.episodes AS
 $BODY$
 DECLARE
  PageOffset INTEGER :=0;
 BEGIN

  PageOffset := ((PageNumber-1) * PageSize);

  RETURN QUERY
   SELECT *
   FROM public.episodes
   ORDER BY published_at DESC
   LIMIT PageSize
   OFFSET PageOffset;
END;
$BODY$
LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION udf_GetUnverifiedEpisodes()
 RETURNS SETOF public.episodes AS
 $BODY$
 BEGIN

  RETURN QUERY
   SELECT DISTINCT a.*
   FROM public.episodes a
   INNER JOIN public.topic b ON b.id_episodes = a.id
   WHERE a.verified=false AND NOT EXISTS (
       SELECT
       FROM   claims
       WHERE  claims.id_episodes = a.id)
   ORDER BY a.updated_at DESC;
END;
$BODY$
LANGUAGE plpgsql;


CREATE OR REPLACE FUNCTION udf_episodes_unverified_GetRowsByPageNumberAndSize(
 PageNumber INTEGER = NULL,
 PageSize INTEGER = NULL
 )
 RETURNS SETOF public.episodes AS
 $BODY$
 DECLARE
  PageOffset INTEGER :=0;
 BEGIN

  PageOffset := ((PageNumber-1) * PageSize);

  RETURN QUERY
   SELECT *
   FROM udf_GetUnverifiedEpisodes()
   LIMIT PageSize
   OFFSET PageOffset;
END;
$BODY$
LANGUAGE plpgsql;