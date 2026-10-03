CREATE SEQUENCE IF NOT EXISTS usuarios_id_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 1
    NO MAXVALUE
    CACHE 1;

CREATE TABLE IF NOT EXISTS usuarios (
    id                  integer      NOT NULL DEFAULT nextval('usuarios_id_seq'),
    nombres             varchar(100) NOT NULL,
    apellidos           varchar(100) NOT NULL,
    fecha_nacimiento    date         NOT NULL,
    direccion           varchar(250) NOT NULL,
    password            varchar(120) NOT NULL,
    telefono            varchar(20)  NOT NULL,
    email               varchar(150) NOT NULL,
    estado              varchar(1)   NOT NULL DEFAULT 'A',
    fecha_creacion      timestamp    NOT NULL,
    fecha_modificacion  timestamp    NULL,
    CONSTRAINT pk_usuarios            PRIMARY KEY (id),
    CONSTRAINT ck_usuarios_estado     CHECK (estado IN ('A', 'I')),
    CONSTRAINT uq_usuarios_telefono   UNIQUE (telefono),
    CONSTRAINT uq_usuarios_email      UNIQUE (email)
);

ALTER SEQUENCE usuarios_id_seq OWNED BY usuarios.id;

CREATE OR REPLACE FUNCTION trg_usuarios_set_fechas()
RETURNS trigger AS $$
BEGIN
    IF (TG_OP = 'INSERT') THEN
        NEW.fecha_creacion     := now();
        NEW.fecha_modificacion := NULL;
    ELSIF (TG_OP = 'UPDATE') THEN
        NEW.fecha_creacion     := OLD.fecha_creacion;
        NEW.fecha_modificacion := now();
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS usuarios_set_fechas ON usuarios;

CREATE TRIGGER usuarios_set_fechas
    BEFORE INSERT OR UPDATE ON usuarios
    FOR EACH ROW
    EXECUTE FUNCTION trg_usuarios_set_fechas();

INSERT INTO usuarios (nombres, apellidos, fecha_nacimiento, direccion, password, telefono, email)
VALUES
    ('Edwin',  'García',   DATE '1990-05-10', 'San Salvador, El Salvador',
     '$2a$11$EfS4Xi/htR014nEdCPex3.1cG5UvqG4VptE89/fg9xHGVDxhdTHLe', '70001111', 'edwin@abank.com.sv'),
    ('Rafael', 'Cuchilla', DATE '1988-03-17', 'Santa Tecla, El Salvador',
     '$2a$11$EfS4Xi/htR014nEdCPex3.1cG5UvqG4VptE89/fg9xHGVDxhdTHLe', '70002222', 'rafael@abank.com.sv')
ON CONFLICT (telefono) DO NOTHING;
